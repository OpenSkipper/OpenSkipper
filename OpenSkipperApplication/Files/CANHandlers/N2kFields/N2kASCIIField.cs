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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANHandler
{
    public class N2kASCIIField : N2kField
    {
        public static string ReadString(byte[] d, int byteOffset, int byteLength)
        {
            string s = System.Text.Encoding.UTF8.GetString(d, byteOffset, byteLength);
            int end = s.IndexOf('\0');
            if (end >= 0)
            {
                return s.Substring(0, end);
            }
            return s;
        }

        // Display the string as text
        public override string ToString(byte[] d)
        {
            return ReadString(d, ByteOffset, ByteLength);
        }

        public string StringValue(byte[] d)
        {
            return ToString(d);
        }
    }

} // namespace
