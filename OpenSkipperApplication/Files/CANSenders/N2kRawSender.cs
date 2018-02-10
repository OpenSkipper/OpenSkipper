using CANHandler;
using OpenSkipperApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANSenders
{
    public class N2kRawSender : FrameSender
    {
        public override byte[] GetBytes(Frame frame)
        {
            return Encoding.ASCII.GetBytes(GetLines(frame));
        }

        public string GetLines(Frame frame)
        {
            N2kFrame n2kFrame = frame as N2kFrame;
            if (frame == null)
            {
                ReportHandler.LogWarning("Attempt to send non-N2k frame as N2k datagram");
                return null;
            }

            int pgn = n2kFrame.Header.PGN;
            int lenght = (int)n2kFrame.Defn.ByteLength;
            int priority = Convert.ToInt32(n2kFrame.Header.Priority);
            int source = Convert.ToInt32(n2kFrame.Header.Source);
            int destination = (n2kFrame.Header.Destination.ToLower() == "all") ? 255 : Convert.ToInt32(n2kFrame.Header.Destination);
            int canId = getCanIdFromISO11783Bits(priority, pgn, source, destination);

            #region Documentation
            // YDWG-02, when setup as a RAW port in Transmit or Both mode can receive
            // NMEA 2000 message.
            //
            // Outgoing messages must end with<CR><LF>. If the message from application is accepted, 
            // passes filters and is transmitted to NMEA 2000, it will be sent back to the application 
            // with "T" direction.
            //
            // For example, the application sends the following sentence to the YDWG-02 Device:
            //      19F51323 01 02 <CR><LF>
            //
            // When this message is sent to the NMEA 2000 network, the Application receives an answer like:
            //      17:33:21.108 T 19F51323 01 02 <CR><LF>
            //
            // The Application will get no answer if the message filtered or the message syntax is invalid.
            #endregion

            StringBuilder sb = new StringBuilder();
            buildOutput(sb, canId, lenght, n2kFrame.Data);

            return sb.ToString();
        }

        /// <summary>
        /// Adds the Frame to the output string
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="canId"></param>
        /// <param name="bytes"></param>
        private void buildOutput(StringBuilder sb, int canId, int length, byte[] bytes)
        {
            #region Documentation
            /*
            Yacht Devices: NMEA 2000 Wi-Fi Gateway (YDWG-02)

            The format of messages sent from application to Device is the same, but without time and 
            direction field. Outgoing messages must end with <CR><LF>. If the message from application 
            is accepted, passes filters and is transmitted to NMEA 2000, it will be sent back to the 
            application with ‘T’ direction.
            
            For example, the application sends the following sentence to the Device:
                19F51323 01 02<CR><LF>
            
            When this message is sent to the NMEA 2000 network, the Application receives an answer like:
                17:33:21.108 T 19F51323 01 02<CR><LF>
            
            The Application will get no answer if the message filtered or the message syntax is invalid.

            NMEA 2000 Fast Message processing:
                15F51301 00 0E FF FF FF FF FF FF
                15F51301 01 3A 04 00 00 3A 04 00
                15F51301 02 00  xx xx xx xx xx xx
            
                Line 1: CanID, Sequence (3 bit), Message Number (5 bits), Data (6 bytes)
                Line 2: CanID, Sequence (3 bit), Data (7 bytes)
                Line 3: CanID, Sequence (3 bit), Data (remaining bytes)
                        Last line can be sent as 2 bytes or as 8 bytes length
            */
            #endregion
            if (length > 8)
            {
                // NMEA 2000 fast message
                
                // Add CanID, Sequence Number and Message Length
                int sequence = 0;
                sb.AppendFormat("{0} {1} {2}", canId.ToString("X4"), sequence.ToString("X2"), length.ToString("X2"));

                // Add 5 Data bytes
                var first5Bytes = bytes.Take(6).ToArray();
                addBytesAndEndMessage(sb, first5Bytes);

                // Add remaining x Data bytes
                var remainBytes = bytes.Skip(6).ToArray().SplitArray(7);

                foreach(var line in remainBytes)
                {
                    // Add CanID, Sequence Number
                    sequence += 1;
                    sb.AppendFormat("{0} {1}", canId.ToString("X4"), sequence.ToString("X2"));

                    // Add message data
                    addBytesAndEndMessage(sb, line);
                }

                // TODO: Should we fill out the remaining X bytes?  The YDWG-02 does not require them, but our internal code does!
            }
            else
            {
                // NMEA 2000 normal message
                sb.AppendFormat(" {0}", canId.ToString("X4"));  // Add CAN ID
                addBytesAndEndMessage(sb, bytes);               // Add message data
            }
        }

        private void addBytesAndEndMessage(StringBuilder sb, byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                // Add each Data byte
                sb.AppendFormat(" {0}",  bytes[i].ToString("X2"));
            }

            // Add Carriage Return <CR> and Line Feed <LF>
            sb.Append("\r\n");
        }

        /// <summary>
        /// Splits Fast-Frames byte array into multiple 8 byte arrays
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private List<N2kFrame> splitData(N2kFrame frame)
        {
            List<N2kFrame> frames = new List<N2kFrame>();
            List<byte[]> parts = new List<byte[]>();
            int byteLen = (int)frame.Defn.ByteLength;

            if (byteLen > 8)
            {
                parts = frame.Data.SplitArray(8);

                foreach (var part in parts)
                {
                    frames.Add(new N2kFrame(frame.Header, part, DateTime.Now));
                }
            }
            else
            {
                frames = new List<N2kFrame> { frame };
            }

            return frames;
        }

        /// <summary>
        /// This does the opposite from getISO11783BitsFromCanId: given n2k fields produces the extended frame CAN id
        /// </summary>
        /// <param name="prio"></param>
        /// <param name="pgn"></param>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        private int getCanIdFromISO11783Bits(int prio, int pgn, int src, int dst)
        {
            //int canId = Convert.ToInt32(src | 0x80000000U); // src bits are the lowest ones of the CAN ID. Also set the highest bit to 1 as n2k uses only extended frames (EFF bit).
            int canId = Convert.ToInt32(src);

            if ((byte)pgn == 0)
            { // PDU 1 (assumed if 8 lowest bits of the PGN are 0)
                canId += dst << 8;
                canId += pgn << 8;
                canId += prio << 26;
            }
            else
            { // PDU 2
                canId += pgn << 8;
                canId += prio << 26;
            }

            return canId;
        }

    } // class

} // namespace
