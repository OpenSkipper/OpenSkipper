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
    /// Represents a header of an AIS message, specifying the message ID
    /// </summary>
    public class AISHeader : FrameHeader
    {
        public int MessageID
        {
            get
            {
                return _messageID;
            }
        }

        public override string Identifier
        {
            get { return _messageID.ToString().PadLeft(2, '0'); }
        }

        public override string Source
        {
            get { return "-"; }
        }

        public override string Destination
        {
            get { return "-"; }
        }

        public override string Priority
        {
            get { return "-"; }
        }

        private readonly int _messageID;

        public AISHeader(int messageID)
        {
            _messageID = messageID;
        }

        public override int CompareTo(object p)
        {
            AISHeader p2 = p as AISHeader;
            if (p2 == null)
            {
                return 1;
            }
            return Identifier.CompareTo(p2.Identifier);
        }

        public override bool Equals(FrameHeader p)
        {
            AISHeader p2 = p as AISHeader;
            if (p2 == null)
            {
                return false;
            }
            return Identifier == p2.Identifier;
        }

        public override string ToString()
        {
            return "MsgID=" + _messageID;
        }
    }

} // namespace
