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
    public enum IncomingProtocolEnum
    {
        NMEA0183,
        Actisense,
        N2KDatagram,
        N2KRaw,
        Xml
    }

    /// <summary>
    /// Provides methods to convert a sequence of bytes into completed frames
    /// </summary>
    public abstract class FrameReader
    {
        public event Action<Frame> FrameCreated;

        protected FrameReader()
        {
        }

        public static FrameReader Create(IncomingProtocolEnum protocol)
        {
            switch (protocol)
            {
                case IncomingProtocolEnum.NMEA0183:
                    return new N0183Reader();

                case IncomingProtocolEnum.Actisense:
                    return new ActisenseReader();

                case IncomingProtocolEnum.N2KDatagram:
                    return new N2kDatagramReader();

                case IncomingProtocolEnum.N2KRaw:
                    return new N2kRawReader();

                //  case IncomingProtocolEnum.Xml:
                //      return new XmlFrameReader();

                default:
                    throw new Exception("Unknown frame protocol '" + protocol.ToString() + "'");
            }
        }

        protected void OnFrameCreated(Frame createdFrame)
        {
            var e = FrameCreated;   // Protect against multi-threading issues
            if (e != null)
                e(createdFrame);
        }

        /// <summary>
        /// Attempts to process the given byte[] into frames
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns>Number of bytes consumed by this reader</returns>
        public abstract int ProcessBytes(byte[] bytes, int offset, int size);
    }

} // namespace
