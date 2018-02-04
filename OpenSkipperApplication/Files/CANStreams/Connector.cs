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

using System;

namespace CANStreams
{
    /// <summary>
    /// The Connector class links to output of one stream to the input of the other.
    /// It transfers the frame via itself, so it has the ability to filter the frames it passes on (Not currently in use)
    /// </summary>
    public class Connector
    {
        public int fromID
        {
            get
            {
                return _fromID;
            }
            set
            {
                if (_fromID != default(int))
                    throw new Exception("Cannot change connector ID !");
                _fromID = value;
            }
        }

        public int toID
        {
            get
            {
                return _toID;
            }
            set
            {
                if (_toID != default(int))
                    throw new Exception("Cannot change connector ID !");
                _toID = value;
            }
        }

        private int _fromID;
        private int _toID;
        public readonly CANStreamer _fromStream;
        public readonly CANStreamer _toStream;

        private Connector()
        {
            // For xml serializer only
        }

        public Connector(CANStreamer fromStream, CANStreamer toStream)
        {
            _fromStream = fromStream;
            _toStream = toStream;

            fromID = _fromStream.ID;
            toID = _toStream.ID;

            _fromStream.RawReceived += Filter;
        }

        public void Disconnect()
        {
            _fromStream.RawReceived -= Filter;
        }

        public void Filter(object sender, FrameReceivedEventArgs e)
        {
            _toStream.SendFrame(e.ReceivedFrame);
        }
    }

} // namespace
