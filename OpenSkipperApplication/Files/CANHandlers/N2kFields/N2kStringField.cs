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

    // Variable string. First byte tells length of string and next byte is 1 - don't know the meaning yet
    public class N2kStringField : N2kField
    {
        // Display the string as text
        public static string ReadString(byte[] d, int byteOffset, ref int byteLength)
        {
            int offset = byteOffset + 2;
            int length = d[byteOffset] - 2;
            byteLength = d[byteOffset];
            string s = "";

            if (length > 0)
            {
                s = System.Text.Encoding.UTF8.GetString(d, offset, length);
            }

            return s;
        }

        public override string ToString(byte[] d)
        {
            int byteLength = 0;
            string s = ReadString(d, ByteOffset, ref byteLength);
            return s;
        }

        public string StringValue(byte[] d)
        {
            return ToString(d);
        }

    } // class

} // namespace
