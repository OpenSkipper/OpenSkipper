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

namespace CANSenders
{
    /// <summary>
    /// Converts all frame types into NMEA 0183 format
    /// </summary>
    public class N0183Sender : FrameSender
    {
        public override byte[] GetBytes(Frame frame)
        {
            N0183Frame n0183Frame = frame as N0183Frame;
            if (n0183Frame != null)
                return Encoding.ASCII.GetBytes(n0183Frame.FullMessage + "\r\n");

            N2kFrame n2kFrame = frame as N2kFrame;
            if (n2kFrame != null)
                return Encoding.ASCII.GetBytes(FrameConversion.PackN2k(n2kFrame).FullMessage + "\r\n");

            AISFrame aisFrame = frame as AISFrame;
            if (aisFrame != null)
            {
                // TODO : Breaking up message if too long

                int padbits = 6 * aisFrame.AISData.AISString.Length - 8 * aisFrame.AISData.AISBytes.Length; // [0, 5]
                string message = "!AIVDM,1,1,,A," + aisFrame.AISData.AISString + "," + padbits;

                // Add checksum
                byte checksum = 0;
                for (int i = 1; i < message.Length; i++)
                    checksum ^= Convert.ToByte(message[i]);
                message += "*" + checksum.ToString("X2");

                return Encoding.ASCII.GetBytes(message + "\r\n");
            }

            throw new Exception("Attempt to send unknown frame type: '" + frame.GetType().Name + "'");
        }
    }

} // namespace
