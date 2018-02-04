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

namespace CANHandler
{
    /// <summary>
    /// Represents a header of an NMEA 2000 message, specifying the PGN, priority, destination, and source.
    /// </summary>
    public class N2kHeader : FrameHeader
    {
        public byte Byte0 { get; set; }
        public byte Byte1 { get; set; }
        public byte Byte2 { get; set; }
        public byte Byte3 { get; set; }

        public int PGN
        {
            get
            {
                return ((int)(Byte0 & 3) << 16)
                     + ((int)Byte1 << 8)
                     + ((int)(Byte1 > 239 ? Byte2 : 0));
            }
            private set
            {
                byte b0 = (byte)((value >> 16) & 3);
                byte b1 = (byte)((value >> 8) & 255);
                byte b2 = (byte)(value & 255);
                Byte0 = (byte)((Byte0 & (byte)252) | b0);
                Byte1 = b1;
                if (b1 > 239)
                {
                    Byte2 = b2;
                }
                else if (b2 != 0)
                {
                    throw new Exception("Illegal PGN code");
                }
            }
        }

        public byte PGNPriority
        {
            get { return (byte)((Byte0 >> 2) & 7); }
            private set { Byte0 = (byte)((Byte0 & 227) | ((value & 7) << 2)); }
        }

        public byte PGNDestination
        {
            get { return (Byte1 < 240) ? Byte2 : (byte)255; }   // 255 means global destination
            private set
            {
                if (Byte1 > 239)
                {
                    // Only valid destination is 255, which is implied anyway and not set in the Header
                    if (value != 255) throw new Exception("Cannot set non-global Destination in J1939 Header for this PGN");
                }
                else
                {
                    Byte2 = (byte)value;
                }
            }
        }

        public byte PGNSource
        {
            get { return Byte3; }
            private set { Byte3 = (byte)value; }
        }

        public override string Identifier { get { return PGN.ToString().PadLeft(6, '0'); } }
        public override string Source { get { return PGNSource.ToString().PadLeft(3); } }
        public override string Destination { get { return (PGNDestination == FrameHeader.GlobalDestination) ? "All" : PGNDestination.ToString().PadLeft(3); } }
        public override string Priority { get { return PGNPriority.ToString(); } }

        public N2kHeader(byte priority, int pgn, byte destination, byte source)
        {
            // This creator ensures the PGN is set before the Destination 
            PGNPriority = priority;
            PGN = pgn;
            PGNDestination = destination;
            PGNSource = source;
        }

        public N2kHeader(byte b0, byte b1, byte b2, byte b3)
        {
            // This creator ensures the PGN is set before the Destination 
            Byte0 = b0;
            Byte1 = b1;
            Byte2 = b2;
            Byte3 = b3;
        }

        public uint AsUInt
        {
            // Return the header in the form of a uint32
            get
            {
                return ((uint)Byte0 << 24) + ((uint)Byte1 << 16) + ((uint)Byte2 << 8) + (uint)Byte3;
            }
        }

        public override bool Equals(FrameHeader p)
        {
            // If parameter is null return false:
            N2kHeader p2 = p as N2kHeader;
            if (p2 == null)
            {
                return false;
            }
            return p2.AsUInt == AsUInt;
        }

        public override string ToString()
        {
            return "PGN=" + PGN + ", Source=" + PGNSource + ", Dest=" + PGNDestination + ", Priority=" + PGNPriority;
        }

        public override int CompareTo(object p)
        {
            N2kHeader p2 = p as N2kHeader;
            if (p2 == null)
            {
                return 1; // N2k comes after N0183
            }
            return AsUInt.CompareTo(p2.AsUInt);
        }
    }

} // namespace
