/*
	Copyright (C) 2009-2010, Andrew Mason <amas008@users.sourceforge.net>
	Copyright (C) 2009-2010, Jason Drake <jdra@users.sourceforge.net>

	This file is part of Open Skipper.
	
	Open Skipper is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Open Skipper is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using CANHandler;
using System;

namespace CANReaders
{
    /// <summary>
    /// Reader for frames sent by an Actisense USB device, also provides information about the raw Actisense messages
    /// </summary>
    public class ActisenseReader : FrameReader
    {
        // Public
        public event Action<ActisenseMessage> ValidMessageReceived;
        public event Action<ActisenseMessage> BadCRCMessageReceived; // Bad CRC

        public event Action<ActisenseMessage> N2kMessageReceived;
        //        public event Action<ActisenseMessage> UnknownMessageReceived;

        // Constants
        private const byte Escape = 0x10;
        private const byte StartOfText = 0x02;
        private const byte EndOfText = 0x03;
        private const byte N2kDataReceived = 0x93;
        private const byte BEMCMDReceived = 0xA0;

        // Private
        private byte[] _buffer = new byte[1024];
        private int _bufferIdx = 0;
        //       private bool firstArrived; // Have we created a packet
        //       private DateTime baseTime; // Our base time, being the data/time we add onto the ms count receive from the NGT1-USB
        //       private uint lastMs; // Millisecond of last frame (to detect rollover)

        // Message reading
        private bool _headerRead = false;
        private bool _escapeRead = false;

        // Constructors
        public ActisenseReader()
        {
            N2kMessageReceived += ProcessN2kMessage;
        }

        // Public methods
        public static int PackFrame(N2kFrame n2kFrame, byte[] buffer)
        {
            /* [0] Message Protocol: 0x93 = N2kDataReceived
             * [1] Message body length (number of bytes 2, 3, ..., up to last byte before CRC)
             * [2] Priority
             * [3] PGN LSB=Least Significant Byte
             * [4] PGN Middle Byte
             * [5] PGN MSB=MostSignificant Byte
             * [6] Destination
             * [7] Source
             * [8,9,10,11] Time stamp, LSB...MSB
             * [12] N2k payload length
             * [13, 14, ..., 13+[12]=20 ] N2k payload (always 8 bytes in data seen so far)
             * [21] CRC byte, chosen so that the sum over all bytes is 0 (mod 256).
             */

            // TODO : Test sending
            // - confirm CRC is generated correctly

            N2kHeader n2kHeader = n2kFrame.Header;

            buffer[0] = Escape;
            buffer[1] = StartOfText;
            buffer[2] = N2kDataReceived; // TODO : What should this be ?

            int pgn = n2kHeader.PGN;
            byte pgnLSB = (byte)(pgn & 0xFF);
            pgn >>= 8;
            byte pgnMDB = (byte)(pgn & 0xFF);
            pgn >>= 8;
            byte pgnMSB = (byte)(pgn & 0xFF);

            buffer[4] = n2kHeader.PGNPriority;
            buffer[5] = pgnLSB;
            buffer[6] = pgnMDB;
            buffer[7] = pgnMSB;
            buffer[8] = n2kHeader.PGNDestination;
            buffer[9] = n2kHeader.PGNSource;

            // Set timestamp bytes [10, 11, 12, 13]
            Array.Copy(BitConverter.GetBytes(DateTime.Now.Millisecond), 0, buffer, 10, 4);

            // Byte 14 : Length of n2k data
            buffer[14] = (byte)n2kFrame.Data.Length; // N2k data length : Bytes [15, CRCbyte)

            byte msgIdx = 15;

            for (int i = 0; i < n2kFrame.Data.Length; i++)
            {
                if (n2kFrame.Data[i] == Escape)
                {
                    buffer[msgIdx++] = Escape;
                    buffer[msgIdx++] = Escape;
                }
                else
                {
                    buffer[msgIdx++] = n2kFrame.Data[i];
                }
            }
            int byteSum = 0;
            for (int i = 2; i < msgIdx; i++)
                byteSum += buffer[i];
            byteSum %= 256;

            buffer[msgIdx++] = (byte)((byteSum == 0) ? 0 : (256 - byteSum));

            buffer[3] = (byte)(msgIdx - 5); // Message length : Bytes [4, CRCbyte)

            buffer[msgIdx++] = Escape;
            buffer[msgIdx++] = EndOfText;

            return msgIdx;
        }

        public override int ProcessBytes(byte[] bytes, int offset, int size)
        {
            // Loop through data
            int numRead = 0;
            for (int i = 0; i < size; i++)
            {
                byte nextByte = bytes[offset + i];

                if (_escapeRead)
                {
                    switch (nextByte)
                    {
                        case Escape:
                            _buffer[_bufferIdx++] = Escape;
                            break;

                        case StartOfText:
                            _bufferIdx = 0;
                            _headerRead = true;
                            break;

                        case EndOfText:
                            if (!_headerRead)
                            {
                                // End of text, but no header read => Came in half way through frame
                                _escapeRead = false;
                                return numRead;
                            }
                            else
                            {
                                _headerRead = false;
                                _escapeRead = false;

                                // Message complete
                                ActisenseMessage actiMsg = new ActisenseMessage(_buffer, 0);
                                OnMessageReceived(actiMsg);

                                // Continue reading.
                                numRead = i + 1;
                                break;
                            }

                        default:
                            // Unexpected character !
                            _headerRead = false;
                            _escapeRead = false;
                            return numRead;
                    }

                    _escapeRead = false;
                }
                else
                {
                    if (nextByte == Escape)
                    {
                        _escapeRead = true;
                    }
                    else if (_headerRead)
                    {
                        _buffer[_bufferIdx++] = nextByte;
                    }
                    else
                    {
                        // Unknown character. Not escaped, but not part of the message
                        return numRead;
                    }
                }
            }

            // Read entire byte[] given
            return size;
        }

        // Private methods
        private void ProcessN2kMessage(ActisenseMessage msg)
        {
            /* This N2kDataReceived message goes:
             * [0] Priority
             * [1] PGN LSB=Least Significant Byte
             * [2] PGN Middle Byte
             * [3] PGN MSB=MostSignificant Byte
             * [4] Destination
             * [5] Source
             * [6,7,8,9] Time stamp, LSB...MSB
             * [10] N2k payload length
             * [11, 12, ...] N2k payload (always 8 bytes in data seen so far)
             */

            byte[] payload = msg.MessagePayload;

            byte Priority = payload[0];
            int PGN = payload[1] + (payload[2] << 8) + (payload[3] << 16);
            byte Destination = payload[4];
            byte Source = payload[5];
            uint ms = BitConverter.ToUInt32(payload, 6);

            // TODO: Work out what the Actisense is transmitting in its time-stamp field
            //       as the number of milliseconds appears to go down as well as up (according to a user report)
            // Note that the Actisense clock appears to be reset when the unit is reset by software,
            /* as indicated by the following R(read)/W(rite) sequence of commands
             *                                              Clock 4 bytes
                W:	10	2	A1	1	13	4B	10	3																																		
                R:	10	2	93	13	2	2	FD	1	FF	3A	66	9F	0	0	8	FF	3	3	69	0	FA	FF	FF	AC	10	3																
                R:	10	2	A0	0E	13	1	0E	0	FA	A4	1	0	0	0	0	0	0	1	90	10	3																					
                W:	10	2	A1	3	13	0	0	49	10	3																																
                R:	10	2	A0	0E	13	1	0E	0	FA	A4	1	0	0	0	0	0	0	0	91	10	3																					
                W:	10	2	A1	1	10	10	4E	10	3																																	
                R:	10	2	93	13	2	12	F1	1	FF	3A	98	9F	0	0	8	FF	0	0	FF	7F	FF	7F	FD	E4	10	3																
                R:	10	2	A0	22	10	10	1	0E	0	FA	A4	1	0	0	0	0	0	6D	0	3C	9C	25	93	67	8	3C	9C	2A	93	67	0	BB	AF	0	0	3	0	0	0	AB	10	3
                W:	10	2	A1	2	4C	2	0F	10	3   <- some sort of reset?
                R:	10	2	A0	0D	4C	1	0E	0	FA	A4	1	0	0	0	0	0	2	57	10	3																						
                W:	10	2	A1	1	13	4B	10	3   <- some sort of reset?
                R:	10	2	A0	0F	F0	1	0E	0	FA	A4	1	0	0	0	0	0	67	8	0	44	10	3																				
                R:	10	2	93	13	2	12	F1	1	FF	3A	2D	0	0	0	8	FF	0	0	FF	7F	FF	7F	FD	EE	10	3																
            */
            // For now, we just use the computer's clock (and so don't get millisecond accuracy)
            DateTime thisTime;
            thisTime = DateTime.Now;
            //if (firstArrived)
            //{
            //    // Check for rollover.
            //    if (ms < lastMs)
            //        baseTime = baseTime.AddMilliseconds(uint.MaxValue); // being 8.1715 years

            //    thisTime = baseTime.AddMilliseconds(ms);
            //}
            //else
            //{
            //    firstArrived = true;
            //    thisTime = DateTime.Now;
            //    baseTime = thisTime.AddMilliseconds(-ms);
            //}
            //lastMs = ms;

            int N2kDataLength = payload[10];
            byte[] N2kData = new byte[N2kDataLength];
            Array.Copy(payload, 11, N2kData, 0, N2kDataLength);

            N2kHeader Header = new N2kHeader(Priority, PGN, Destination, Source);
            N2kFrame frame = new N2kFrame(Header, N2kData, thisTime);

            OnFrameCreated(frame);
        }

        private void OnMessageReceived(ActisenseMessage msg)
        {
            if (!msg.CRCisOK)
            {
                if (BadCRCMessageReceived != null)
                    BadCRCMessageReceived(msg);

                return;
            }

            if (ValidMessageReceived != null)
                ValidMessageReceived(msg);

            switch (msg.MessageType)
            {
                case N2kDataReceived:
                    var e = N2kMessageReceived;
                    if (e != null)
                        e(msg);
                    break;

                default:
                    // TODO: This is some other Actisense message, eg a status message. Process it somehow...
                    break;
            }
        }
    }

} // namespace
