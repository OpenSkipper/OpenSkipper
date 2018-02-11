using CANHandler;
using CANReaders;
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

        public string GetLines(Frame frame, bool fullTx = false)
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
            int canId = N2kRawMessage.CreateCanID(priority, pgn, source, destination);

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
            buildOutput(sb, canId, lenght, n2kFrame.Data, fullTx);

            return sb.ToString();
        }

        /// <summary>
        /// Adds the Frame to the output string
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="canId"></param>
        /// <param name="bytes"></param>
        private void buildOutput(StringBuilder sb, int canId, int length, byte[] bytes, bool fullTx = false)
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

                // Add Time and Direction
                addTimeAndDirection(sb, true, fullTx);

                // Add CanID, Sequence Number and Message Length
                int sequence = 0;
                sb.AppendFormat("{0} {1} {2}", canId.ToString("X8"), sequence.ToString("X2"), length.ToString("X2"));

                // Add 5 Data bytes
                var first5Bytes = bytes.Take(6).ToArray();
                addBytesAndEndMessage(sb, first5Bytes);

                // Add remaining x Data bytes
                var remainBytes = bytes.Skip(6).ToArray().SplitArray(7);

                foreach(var line in remainBytes)
                {
                    // Add Time and Direction
                    addTimeAndDirection(sb, true, fullTx);

                    // Add CanID, Sequence Number
                    sequence += 1;
                    sb.AppendFormat("{0} {1}", canId.ToString("X8"), sequence.ToString("X2"));

                    // Check if the line does not have enough bytes
                    // and add filler bytes if required.
                    // The YDWG-02 does not require them, but Open Skipper does!
                    var lineBytes = line;
                    if (lineBytes.Length < 7)
                    {
                        var bitCount = (7 - lineBytes.Length) * 8;
                        var filler = FieldConverter.SetNaBytes(bitCount);
                        lineBytes = lineBytes.Concat(filler).ToArray();
                    }

                    // Add message data
                    addBytesAndEndMessage(sb, lineBytes);
                }
            }
            else
            {
                // NMEA 2000 normal message
                addTimeAndDirection(sb, false, fullTx);         // Add Time and Direction
                sb.AppendFormat(" {0}", canId.ToString("X8"));  // Add CAN ID
                addBytesAndEndMessage(sb, bytes);               // Add message data
            }
        }

        private void addTimeAndDirection(StringBuilder sb, bool trailSpace, bool fullTx)
        {
            if (fullTx)
            {
                // Add Time and direction (T = Transmit)
                sb.AppendFormat("{0:HH:mm:ss.fff} T", DateTime.Now);
                if (trailSpace)
                {
                    sb.Append(" ");
                }
            }
        }

        private void addBytesAndEndMessage(StringBuilder sb, byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                // Add each Data byte
                sb.AppendFormat(" {0}", bytes[i].ToString("X2"));
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
        /// Takes the provided byte length and rounds it up to the nearest 8 bytes.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int GetFillerByteCount(int value)
        {
            // See: https://stackoverflow.com/questions/2705542/returning-the-nearest-multiple-value-of-a-number
            // 16 * ((n + 8) / 16) is the formula you want if, in particular, 
            // you want to convert 8 to 16(it's equally close to 0 as to 16, 
            // so it's impossible to decide how to convert it based exclusively 
            // on the "nearest multiple" concept, you have to decide!-), and 
            // of course consistently 24 to 32, 40 to 48, and so forth. 
            // Use + 7 in lieu of +8 if you'd rather convert 8 to 0 rather 
            // than to 16 (and consistently 24 to 16, and so forth).
            //
            // To use a generic X in lieu of the hardcoded 16, then the 
            // formula is X * ((n + X / 2) / X) (with the same proviso as in the above).

            int factor = 8;
            return factor * ((value + factor / 2) / factor);
        }

    } // class

} // namespace
