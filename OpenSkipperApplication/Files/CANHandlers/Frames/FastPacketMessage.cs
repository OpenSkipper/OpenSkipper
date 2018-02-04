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

using CANDefinitions;
using System;

namespace CANHandler
{
    /// <summary>
    /// Represents a partially completed N2kFrame, 
    /// </summary>
    public class FastPacketMessage : N2kFrame
    {
        // TODO : Remove inheritance. Add private N2kFrame member variable. Then move the logic from this class into 'N2kFrame.Builder'

        // This is the class that we use to receive multi-packet PGNs. It is called repeatedly to add successive frames.
        // It handles the same frame being repeated in the data (generating only a warning).
        public int LastFrameIdentifier = -1;
        public int ExpectedByteCount = -1;
        public int ReceivedByteCount = 0;
        public PGNDefn pgnInfo { get; set; }

        // Creator. Call with the first frame of the fast-Packet frame sequence
        public FastPacketMessage(N2kFrame FirstFrame, PGNDefn PgnInfo)
            : base(FirstFrame.Header, FirstFrame.Data, FirstFrame.TimeStamp)
        {
            ReportHandler.LogInfo("First Fast Packet Received: " + FirstFrame.Header + " : " + System.BitConverter.ToString(FirstFrame.Data));

            InitFastPacketSequence(FirstFrame);
            pgnInfo = PgnInfo;
        }

        public void InitFastPacketSequence(N2kFrame FirstFrame)
        {
            // Remember the header from the first frame; should match all the other frames
            Header = FirstFrame.Header;
            
            // Remember time stamp
            TimeStamp = FirstFrame.TimeStamp;
            
            // Get the frame identifier and the total number of bytes to read from the first frame in positions 0 and 1 respectively.
            LastFrameIdentifier = FirstFrame.Data[0];
            if ((LastFrameIdentifier & 0x1F) != 0)
            {
                // TODO: Might be better to ignore the frame?
                throw new Exception("First frame of fast packet sequence does not start with identifier 00, 20, 40, ...");
            }

            ExpectedByteCount = FirstFrame.Data[1];
            if (ExpectedByteCount < 8)
            {
                // TODO: Might be better to ignore the frame?
                throw new Exception("Fast packet frame encountered with fewer than 8 bytes.");
            }
            
            // And copy the remaining 6 bytes of data
            Data = new byte[ExpectedByteCount];
            ReceivedByteCount = 0;

            for (int i = 2; i < 8; i++)
            {
                Data[ReceivedByteCount++] = FirstFrame.Data[i];
            }
        }

        // Add the next received frame to the data. Returns true if the last packet has been received.
        public bool AddNextFrame(N2kFrame Frame)
        {
            ReportHandler.LogInfo("Fast Packet Received: " + Frame.Header + " : " + System.BitConverter.ToString(Frame.Data));

            if (!Frame.Header.Equals(Header))
            {
                // TODO: Might be better to ignore the frame?
                throw new Exception("Headers don't match in successive fast packet frames.");
            }

            int FrameIdentifier = Frame.Data[0];

            // We specifically handle repeated frames, which we see in Kee's sample data
            if (FrameIdentifier == LastFrameIdentifier)
            {
                // Check that the data matches; note index goes from 1 to 7
                int FrameDiffsCount = 0;
                for (int i = 1; i <= Math.Min(7, ReceivedByteCount); i++)
                {
                    if (Frame.Data[8 - i] != Data[ReceivedByteCount - i])
                    {
                        Data[ReceivedByteCount - i] = Frame.Data[8 - i];
                        FrameDiffsCount++;
                    }
                }

                if (FrameDiffsCount == 0)
                {
                    ReportHandler.LogWarning(string.Format("PGN {0} has fast packet frame {1} duplicated. ", ((N2kHeader)Frame.Header).PGN, FrameIdentifier));
                }

                if (FrameDiffsCount > 0)
                {
                    ReportHandler.LogWarning(string.Format("PGN {0} has fast packet frame {1} duplicated, but with {2} differences. ", ((N2kHeader)Frame.Header).PGN, FrameIdentifier, FrameDiffsCount));
                }

                return false;
            }

            if ((FrameIdentifier & 0x1F) == 0)
            {
                // This appears to be the first frame in a fast packet sequence. Reset our sequence
                ReportHandler.LogWarning(string.Format("New fast packet sequence started before previous (with {0} bytes received) completed for PGN {1}.\n", ReceivedByteCount, ((N2kHeader)Frame.Header).PGN));

                if ((Frame.Data[1] != ExpectedByteCount) && pgnInfo.HasMultipleDefinitions)
                {
                    throw new Exception("Code does not handle unexpected re-starting of fast packet sequence with a different length for a multiple-defn PGN.");
                }

                InitFastPacketSequence(Frame);

                return false;
            }

            if (FrameIdentifier != (LastFrameIdentifier + 1))
            {
                throw new Exception("Frame identifiers out of order in successive fast packet frames, perhaps because of missing frames?");
            }

            int BytesToReceive = Math.Min(7, ExpectedByteCount - ReceivedByteCount);

            for (int i = 1; i <= BytesToReceive; i++)
            {
                Data[ReceivedByteCount++] = Frame.Data[i];
            }

            LastFrameIdentifier++;

            return ReceivedByteCount == ExpectedByteCount;
        }
    }

}  // namespace
