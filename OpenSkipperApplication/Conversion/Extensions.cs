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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenSkipperApplication
{
    public static class Extensions
    {
        /// <summary>
        /// Round a double to x places using the Field.Scale value
        /// </summary>
        /// <param name="number"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static double Round(this double number, double scale)
        {
            return (double)((decimal)scale * Math.Round((decimal)number / (decimal)scale, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Splits a byte array into smaller a list of byte arrays in the provided size.  
        /// The last array in the list may include less bytes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<byte[]> SplitArray(this byte[] bytes, int size)
        {
            int pos = 0;
            int remaining = size;

            List<byte[]> result = new List<byte[]>();
            byte[] block = null;

            while ((remaining = bytes.Length - pos) > 0)
            {
                block = new byte[Math.Min(remaining, size)];

                Array.Copy(bytes, pos, block, 0, block.Length);
                result.Add(block);

                pos += block.Length;
            }

            return result;
        }

        /// <summary>
        /// Converts int to a Byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte ToByte(this int value)
        {
            return Convert.ToByte(value);
        }

        public static int HexToInt(this string value)
        {
            // Check if the string is actually HEX
            if (Regex.IsMatch(value, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                return Convert.ToInt32(value, 16);
            }

            throw new ArgumentException("Input string is not a HEX value.");
        }

        public static byte HexToByte(this string value)
        {
            // Check if the string is actually HEX
            if (Regex.IsMatch(value, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                return Convert.ToByte(value, 16);
            }

            throw new ArgumentException("Input string is not a HEX value.");
        }

        public static byte[] LeftPad(this byte[] input, byte padValue, int len)
        {
            var temp = Enumerable.Repeat(padValue, len).ToArray(); ;
            var startAt = temp.Length - input.Length;
            Array.Copy(input, 0, temp, startAt, input.Length);
            return temp;
        }

        public static byte[] RightPad(this byte[] input, byte padValue, int len)
        {
            var temp = Enumerable.Repeat(padValue, len).ToArray();
            Array.Copy(input, 0, temp, 0, input.Length);
            return temp;
        }

    } // class

} // namespace
