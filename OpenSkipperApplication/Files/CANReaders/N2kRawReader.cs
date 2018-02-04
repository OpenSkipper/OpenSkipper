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
using CANMessages;
using OpenSkipperApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CANReaders
{
    /// <summary>
    /// Reader for complete NMEA 2000 frames sent in RAW format
    /// </summary>
    /// <remarks>Jan 7, 2018 - mlucas</remarks>
    public class N2kRawReader : FrameReader
    {
        // Set array index values for the data bytes
        // e.g.
        // 17:33:21.107 R 19F51323 01 2F 30 70 00 2F 30 70
        private const int RAW_TIME_IDX = 0;
        private const int RAW_DIRECTION_IDX = 1;
        private const int RAW_CANID_IDX = 2;

        private readonly StringSplitOptions SPLIT_OPTIONS = StringSplitOptions.RemoveEmptyEntries;
        private readonly char[] SPLIT_LINE_SEPERATOR = Environment.NewLine.ToCharArray();
        private readonly char[] SPLIT_DATA_SEPERATOR = new char[] { ' ' };

        public override int ProcessBytes(byte[] bytes, int offset, int size)
        {
            var frames = GetFrames(bytes);

            foreach (var frame in frames)
            {
                OnFrameCreated(frame);
            }

            return frames.Count;
        }

        /// <summary>
        /// Assumes the bytes are for a single PGN
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public N2kFrame GetAsSingleFrame(byte[] bytes)
        {
            var frames = GetFrames(bytes);
            var builder = new N2kFrame.Builder();

            var pgnCount = frames.Select(f => f.Header.PGN).Distinct().Count();
            if (pgnCount > 1)
            {
                throw new ArgumentOutOfRangeException("The parsed byte array contains more than one PGN.");
            }

            foreach (var frame in frames)
            {
                var built = builder.AddFrame(frame);
                if (built != null)
                {
                    return built;
                }
            }

            return null;
        }

        public List<N2kFrame> GetFrames(byte[] bytes)
        {
            List<N2kRawMessage> msgs = ParseBytes(bytes);
            string strReturnData = Encoding.ASCII.GetString(bytes);

            if (string.IsNullOrEmpty(strReturnData))
            {
                return null;
            }

            List<N2kFrame> frames = new List<N2kFrame>();

            foreach (var msg in msgs)
            {
                // Create the N2k Header and Frame
                N2kHeader Header = new N2kHeader(msg.Priority, msg.Pgn, msg.Destination, msg.Source);
                N2kFrame frame = new N2kFrame(Header, msg.Data, msg.Timestamp);

                // Set the frame's PGN definition looked up in the new N2kMessage
                frame.Defn = new N2kMessage(msg.Pgn).Definition;

                // Add the completed frame to the list of received messages
                frames.Add(frame);
            }

            return frames;
        }

        public List<N2kRawMessage> ParseBytes(byte[] bytes)
        {
            // N2K RAW message format (not SeaSmart format)
            #region Documentation
            /*
            NMEA 2000 Wi-Fi Gateway YDWG-02
            See: http://www.yachtd.com/products/wifi_gateway.html
            See: http://www.yachtd.com/downloads/ydwg02.pdf

            In RAW mode, the YDWG-02 network messages are converted to plain text format. 
            We recommend that software developers support this format in applications 
            because it is the easiest option. In the terminal window, NMEA 2000 messages 
            look like a log in a chart plotter.  
            
            Messages sent from Device to PC have the following form:

                hh:mm:ss.ddd D msgid b0 b1 b2 b3 b4 b5 b6 b7<CR><LF>

            where:
                * hh:mm:sss.ddd — time of message transmission or reception, ddd are milliseconds;
                * D — direction of the message ('R' — from NMEA 2000 to application, 'T' — from application to NMEA 2000);
                * msgid — 29-bit message identifier in hexadecimal format (contains NMEA 2000 PGN and other fields);
                * b0..b7 — message data bytes (from 1 to 8) in hexadecimal format;
                * <CR><LF> — end of line symbols (carriage return and line feed, decimal 13 and 10).
            
            Example:
                17:33:21.107 R 19F51323 01 2F 30 70 00 2F 30 70
                17:33:21.108 R 19F51323 02 00
                17:33:21.141 R 09F80115 A0 7D E6 18 C0 05 FB D5
                17:33:21.179 R 09FD0205 64 1E 01 C8 F1 FA FF FF
                17:33:21.189 R 1DEFFF00 A0 0B E5 98 F1 08 02 02
                17:33:21.190 R 1DEFFF00 A1 00 DF 83 00 00
                17:33:21.219 R 15FD0734 FF 02 2B 75 A9 1A FF FF

            Time of message is UTC time if the Device has received the time from the NMEA 2000 network, 
            otherwise it is the time from Device start.
            */
            #endregion

            List<N2kRawMessage> msgs = new List<N2kRawMessage>();

            // Convert the byte array into the N2k Raw ASCII string
            string strReturnData = Encoding.ASCII.GetString(bytes);
            if (string.IsNullOrEmpty(strReturnData))
            {
                return msgs;
            }

            // Split the input data into a string array
            // using the line break as the split characters
            // and remove any empty lines.
            string[] lines = strReturnData.Trim().Split(SPLIT_LINE_SEPERATOR, SPLIT_OPTIONS);

            var pgnList = new Dictionary<int, N2kRawMessage>();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    // Blank line, skip processing
                    continue;
                }

                string[] parts = line.Split(SPLIT_DATA_SEPERATOR, SPLIT_OPTIONS);
                if (parts.Length < 3)
                {
                    // Not enough values, skip processing
                    continue;
                }

                System.Diagnostics.Debug.WriteLine(line);

                // Get CAN ID
                int canID = parts[RAW_CANID_IDX].HexToInt();

                // Create the new N2kRawMessage object:
                // The N2kRawMessage will parse the 
                // CanID pulling out the encoded values: 
                //  pgn, priority, source, destination
                int dataLen = parts.Length - 3;
                N2kRawMessage msg = new N2kRawMessage(canID)
                {
                    // Set the time and direction
                    Timestamp = DateTime.Parse(parts[RAW_TIME_IDX]),
                    Direction = parts[RAW_DIRECTION_IDX],

                    Data = new byte[dataLen],
                    Lenght = dataLen.ToByte()
                 };

                // Put the data bytes into a byte array
                int x = 0;
                for (int i = 3; i < parts.Length; i++)
                {
                    msg.Data[x] = parts[i].HexToByte();
                    x += 1;
                }

                msgs.Add(msg);
            }

            // No Fast Message processing here.  It is done upstream 
            // in the IncomingMessageHandler's N2kFrame.Builder()
            return msgs;
        }

    } // class

} // namespace
