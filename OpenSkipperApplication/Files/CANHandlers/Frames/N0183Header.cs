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
    /// Represents a header of an NMEA 0183 message, specifying the talker ID and typecode
    /// </summary>
    public class N0183Header : FrameHeader
    {
        // Public properties
        public string HeaderText { get { return _headerText; } }
        public string TalkerID { get { return _talkerID; } }
        public string TypeCode { get { return _typeCode; } }

        public override string Identifier { get { return TypeCode; } }
        public override string Source { get { return TalkerID; } }
        public override string Destination { get { return "All"; } }
        public override string Priority { get { return "-"; } }

        // Private vars
        private readonly string _headerText;
        private readonly string _startCharacter;
        private readonly string _talkerID;
        private readonly string _typeCode;

        // Constructors
        public N0183Header(string headerText)
        {
            _headerText = headerText;
            _startCharacter = headerText.Substring(0, 1);
            _talkerID = headerText.Substring(1, 2);
            _typeCode = headerText.Substring(3, 3);
        }

        public N0183Header(string startChar, string talkerID, string typeCode)
        {
            _startCharacter = startChar;
            _talkerID = talkerID;
            _typeCode = typeCode;
            _headerText = startChar + talkerID + typeCode;
        }

        // Public methods
        public override bool Equals(FrameHeader p)
        {
            N0183Header p2 = p as N0183Header;
            if (p2 == null)
            {
                return false;
            }
            return HeaderText == p2.HeaderText;
        }

        public override string ToString()
        {
            return "TalkerID=" + TalkerID + ", Code=" + TypeCode;
        }

        public override int CompareTo(object p)
        {
            N0183Header p2 = p as N0183Header;
            if (p2 == null)
            {
                return -1; // N2k comes after N0183
            }
            return HeaderText.CompareTo(p2.HeaderText);
        }
    }

} // namepspace
