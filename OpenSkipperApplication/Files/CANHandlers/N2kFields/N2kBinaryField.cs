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

namespace CANHandler
{
    public class N2kBinaryField : N2kField
    {
        /*      
        public override string ToString(byte[] d)
        {
            if (BitLength == 0) {
                // This dummy bit length means all the data
                return BitConverter.ToString(d, ByteOffset, d.Length - ByteOffset);
            } else if (BitLength < 8) {
                return Tools.Binary(d[BitOffset >> 3],BitOffsetWithinByte,BitLength);
            } else
                try
                {
                    return BitConverter.ToString(d, ByteOffset, ByteLength);
                }
                catch
                {
                    return "Field error";
                }
        }
         */
    }

} // namespace
