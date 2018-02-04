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
using CANSenders;
using System;

namespace CANSenders
{
    /// <summary>
    /// Converts NMEA 2000 frames into NMEA 2000 format, should only be used with UDP datagrams or a protocol where length of each transmission is known
    /// </summary>
    public class N2KDatagramSender : FrameSender
    {
        public override byte[] GetBytes(Frame frame)
        {
            N2kFrame n2kFrame = frame as N2kFrame;
            if (frame == null)
            {
                ReportHandler.LogWarning("Attempt to send non-N2k frame as N2k datagram");
                return null;
            }

            byte[] datagram = new byte[4 + n2kFrame.Data.Length];
            datagram[0] = n2kFrame.Header.Byte0;
            datagram[1] = n2kFrame.Header.Byte1;
            datagram[2] = n2kFrame.Header.Byte2;
            datagram[3] = n2kFrame.Header.Byte3;
            Array.Copy(n2kFrame.Data, 0, datagram, 4, n2kFrame.Data.Length);

            return datagram;
        }
    }

} // namespace
