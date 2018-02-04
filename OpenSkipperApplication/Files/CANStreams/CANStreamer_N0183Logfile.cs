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
using CANReaders;
using System;
using System.Collections.Generic;
using System.IO;

namespace CANStreams
{

    public class CANStreamer_N0183Logfile : CANStreamer_Logfile
    {
        // Properties
        public static string FileFilter = "NMEA 0183 Log File (*.txt)|*.txt";

        // Variables for reading the frames
        private FrameReader _N0183Builder;
        private Queue<Frame> _frames;
        private bool setDate;
        private DateTime _lastDate;

        // Constructors
        public CANStreamer_N0183Logfile()
            : base()
        {
            _N0183Builder = new N0183Reader();
            _N0183Builder.FrameCreated += N0183Builder_FrameCreated;
            _frames = new Queue<Frame> { };
        }

        private void N0183Builder_FrameCreated(Frame createdFrame)
        {
            if (setDate)
            {
                _lastDate = createdFrame.TimeStamp = _lastDate.AddSeconds(0.1);
            }
            else
            {
                createdFrame.TimeStamp = DateTime.Now;
                _lastDate = createdFrame.TimeStamp;
                setDate = true;
            }

            _frames.Enqueue(createdFrame);
        }

        // Gets next packet from file
        public override Frame getNextPacket()
        {
            if (_frames.Count > 0)
            {
                return _frames.Dequeue();
            }
            else
            {
                byte[] buffer = new byte[256];
                int readBytes;

                while (_frames.Count == 0)
                {
                    readBytes = _fileStream.Read(buffer, 0, 256);
                    if (readBytes == 0)
                        return null;

                    for (int i = 0; i < readBytes; i++)
                        i += _N0183Builder.ProcessBytes(buffer, i, readBytes - i);
                }

                return _frames.Dequeue();
            }
        }
        public override void Rewind()
        {
            lock (packetLocker)
            {
                MessageHandler.Reset();
                _fileStream.Seek(0, SeekOrigin.Begin);

                nextPacket = getNextPacket();
                if (nextPacket == null)
                    return; // Empty log file..

                _logTime = nextPacket.TimeStamp;
                _lastTime = DateTime.Now;
            }
        }
    }

} // namespace
