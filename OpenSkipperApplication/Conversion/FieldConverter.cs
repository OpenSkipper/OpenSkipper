/*
	Copyright (C) 2018 Michael Lucas

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

namespace OpenSkipperApplication
{
    internal class FieldConverter
    {
        public static byte[] SetBytes(int value, int bitLength, int byteOffset, out byte[] chkBytes)
        {
            // Convert the value to Hex
            var hex = new Hex(value, bitLength / 8);

            return doSetBytes(hex, byteOffset, out chkBytes);
        }

        public static byte[] SetBytes(uint value, int bitLength, int byteOffset, out byte[] chkBytes)
        {
            // Convert the value to Hex
            var hex = new Hex(value, bitLength / 8);

            return doSetBytes(hex, byteOffset, out chkBytes);
        }

        public static byte[] SetBytes(short value, int bitLength, int byteOffset, out byte[] chkBytes)
        {
            // Convert the value to Hex
            var hex = new Hex(value, bitLength / 8);

            return doSetBytes(hex, byteOffset, out chkBytes);
        }

        public static byte[] SetBytes(ushort value, int bitLength, int byteOffset, out byte[] chkBytes)
        {
            // Convert the value to Hex
            var hex = new Hex(value, bitLength / 8);

            return doSetBytes(hex, byteOffset, out chkBytes);
        }

        public static byte[] SetBytes(long value, int bitLength, int byteOffset, out byte[] chkBytes)
        {
            // Convert the value to Hex
            var hex = new Hex(value, bitLength / 8);

            return doSetBytes(hex, byteOffset, out chkBytes);
        }

        public static byte[] SetNaBytes(int bitLength)
        {
            // Convert 255 to FF x number of required bytes
            byte[] bytes = new byte[bitLength / 8];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 0xFF;
            }

            return bytes;
        }

        private static byte[] doSetBytes(Hex hex, int byteOffset, out byte[] chkBytes)
        {
            // TODO: Does the hex always need to be reversed, or only on Intel chips?
            //if (BitConverter.IsLittleEndian)
            //{
            // We want the hex value in the reverse order
            hex.Reverse();
            ////}

            // Convert the hex value in binary bits
            var bits = new Bits(hex);

            // Convert the binary bits into a Byte array
            byte[] bytes = bits.ToBytes();

            // Pad the byte array with leading bytes because
            // CheckValue assumes the full frame Data byte array
            // and uses the Field BitOffset and BitLength to locate 
            // the value in the Data array.
            chkBytes = bytes.LeftPad(0, bytes.Length + byteOffset);

            return bytes;
        }

    } // class

} // namespace
