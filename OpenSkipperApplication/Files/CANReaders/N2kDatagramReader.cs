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
    /// Reader for complete NMEA 2000 messages sent in UDP datagrams
    /// </summary>
    public class N2kDatagramReader : FrameReader
    {
        public override int ProcessBytes(byte[] bytes, int offset, int size)
        {
            N2kFrame n2kFrame = GetFrame(bytes, size);
            OnFrameCreated(n2kFrame);

            return size;
        }

        public N2kFrame GetFrame(byte[] bytes, int size)
        {
            N2kHeader n2kHeader = new N2kHeader(bytes[0], bytes[1], bytes[2], bytes[3]);
            byte[] n2kData = new byte[size - 4];
            Array.Copy(bytes, 4, n2kData, 0, size - 4);

            return new N2kFrame(n2kHeader, n2kData, DateTime.Now);
        }
    }

} // namespace
