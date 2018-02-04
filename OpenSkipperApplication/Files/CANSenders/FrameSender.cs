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

namespace CANSenders
{
    public enum OutgoingProtocolEnum
    {
        NMEA0183,
        //Actisense,
        N2KDatagram,
        N2KRaw,
        Xml
    }

    /// <summary>
    /// Base class for an object which supports converting a frame to a byte[] corresponding a specific protocol
    /// </summary>
    public abstract class FrameSender
    {
        public static FrameSender Create(OutgoingProtocolEnum format)
        {
            switch (format)
            {
                case OutgoingProtocolEnum.NMEA0183:
                    return new N0183Sender();

                //   case OutgoingProtocolEnum.Actisense:
                //       throw new NotImplementedException("Actisense sender not yet implemented");

                case OutgoingProtocolEnum.Xml:
                    return new XmlFrameSender();

                case OutgoingProtocolEnum.N2KDatagram:
                    return new N2KDatagramSender();

                case OutgoingProtocolEnum.N2KRaw:
                    return new N2kRawSender();

                default:
                    throw new Exception("Unrecognised outgoing format: '" + format.ToString() + "'");
            }
        }

        public abstract byte[] GetBytes(Frame frame);
    }

} // namespace
