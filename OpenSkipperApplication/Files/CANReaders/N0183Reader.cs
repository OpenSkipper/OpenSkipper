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
using System.Text;

namespace CANReaders
{
    /// <summary>
    /// Reader for raw NMEA 0183 format ('[$!]...\r\n') frames.
    /// </summary>
    public class N0183Reader : FrameReader
    {
        // Constants
        private const byte startChar1 = (byte)'$';
        private const byte startChar2 = (byte)'!';
        private const byte endChar1 = (byte)'\n';
        private const byte endChar2 = (byte)'\r';

        // Message reading
        private byte[] _buffer = new byte[1024];
        private int _bufferIdx = 0;
        private bool waitingForStart = true;

        // Public functions
        public override int ProcessBytes(byte[] bytes, int offset, int size)
        {
            int numRead = 0;
            for (int i = 0; i < size; i++)
            {
                byte nextByte = bytes[offset + i];
                bool startCharRead = (nextByte == startChar1) || (nextByte == startChar2);
                bool endCharRead = (nextByte == endChar1) || (nextByte == endChar2);

                if (waitingForStart)
                {
                    if (endCharRead)
                    {
                        // We skip over end characters while waiting for start (There is no requirement to read them all when finishing a frame)
                    }
                    else if (startCharRead)
                    {
                        // Read in start character, toggle flag
                        _bufferIdx = 0;
                        _buffer[_bufferIdx++] = nextByte;
                        waitingForStart = false;
                    }
                    else
                    {
                        // Start byte does not match, return
                        return numRead;
                    }
                }
                else
                {
                    if (startCharRead || endCharRead)
                    {
                        if (startCharRead)
                            i--;

                        // End character read, and we don't add it to the buffer (Would be trimmed by frame constructor anyway)

                        // Grab message from buffer
                        string msgData = Encoding.ASCII.GetString(_buffer, 0, _bufferIdx);

                        // Create message
                        Frame createdFrame = N0183Frame.TryCreate(msgData, DateTime.Now);
                        if (createdFrame != null)
                            OnFrameCreated(createdFrame);

                        // Reset flags and return
                        waitingForStart = true;
                        numRead = i + 1;
                    }
                    else
                    {
                        _buffer[_bufferIdx++] = nextByte;
                    }
                }
            }

            // Read entire byte[] given.
            return size;
        }
    }

} // namespace
