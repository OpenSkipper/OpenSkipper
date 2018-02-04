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
using System.Text;

namespace CANHandler
{
    /// <summary>
    /// Provides methods for encoding byte arrays to AIS format, and the reverse.
    /// </summary>
    public static class AISEncoding
    {
        private static byte[] asciiToSixbit; // For decoding an ascii byte into the corresponding 6-bit value; aisByteToSixbit[ascii] = sixbit
        private static byte[] sixbitToAscii; // For encoding a 6-bit value into the corresponding ascii byte; sixbitToAisByte[sixbit] = ascii
        private static char[] sixbitToChar;  // For decoding a 6-bit value into the corresponding character; sixbitToChar[sixbit] = character

        static AISEncoding()
        {
            // The following is based on tables found at http://gpsd.berlios.de/AIVDM.html (Among others)

            sixbitToAscii = Encoding.ASCII.GetBytes(new char[] {'0', '1', '2', '3', '4', '5', '6', '7',
                                                                '8', '9', ':', ';', '<', '=', '>', '?',
                                                                '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G',
                                                                'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
                                                                'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
                                                                '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
                                                                'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
                                                                'p', 'q', 'r', 's', 't', 'u', 'v', 'w'});

            asciiToSixbit = new byte[128];
            for (byte i = 0; i < sixbitToAscii.Length; i++)
                asciiToSixbit[sixbitToAscii[i]] = i;

            sixbitToChar = new char[] {'@', 'A', 'B', 'C', 'D',  'E', 'F', 'G',
                                       'H', 'I', 'J', 'K', 'L',  'M', 'N', 'O',
                                       'P', 'Q', 'R', 'S', 'T',  'U', 'V', 'W',
                                       'X', 'Y', 'Z', '[', '\\', ']', '^', '_',
                                       ' ', '!', '"', '#', '$',  '%', '&', '\'',
                                       '(', ')', '*', '+', ',',  '-', '.', '/',
                                       '0', '1', '2', '3', '4',  '5', '6', '7',
                                       '8', '9', ':', ';', '<',  '=', '>', '?'};
        }

        public static byte[] GetBytes(string aisString)
        {
            // IMPORTANT : Partial bytes are discarded !
            // i.e. if given 2 characters, == 12 bits, only 1 byte will be formed (and the other 4 bits discarded)
            // However, the first 2 bits from the second character will be used in completing the first byte.
            // If the discarded bytes were relevant (i.e. Sender was really sending 12 bits) it would have been recognised as requiring 2 bytes to store,
            // and hence 3 characters would have been sent (18 bits covering the 16 bits (Two discarded), with the last bits of second byte being zeroed explicitly)

            // Get AIS bytes from string
            byte[] aisBytes = Encoding.ASCII.GetBytes(aisString);

            // Create byte array to hold result (Note integer division)
            byte[] bytes = new byte[(6 * aisBytes.Length) / 8];

            // Loop through each byte, packing it into our byte array.
            for (int i = 0; i < aisBytes.Length; i++)
            {
                byte sixbit = asciiToSixbit[aisBytes[i]];
                int byteIdx = (6 * i) / 8;
                int innerIdx = (6 * i) % 8; // = 0, 2, 4, 6

                if (innerIdx <= 2) // innerIdx == 0, 2
                {
                    bytes[byteIdx] |= (byte)(sixbit << (2 - innerIdx));
                }
                else // innerIdx == 4, 6
                {
                    bytes[byteIdx] |= (byte)(sixbit >> (innerIdx - 2));

                    // Also overflows into next byte (if we are interested in it)
                    if (byteIdx + 1 < bytes.Length)
                        bytes[byteIdx + 1] |= (byte)((sixbit << (10 - innerIdx)) & 0xFF);
                }
            }

            return bytes;
        }

        public static string EncodeBytes(byte[] data, int byteOffset, int byteLength)
        {
            byte[] ascii = new byte[(int)Math.Ceiling((8 * byteLength) / 6.0)];

            for (int i = 0; i < ascii.Length; i++)
            {
                int byteIdx = byteOffset + (6 * i) / 8;
                int innerIdx = (6 * i) % 8; // = 0, 2, 4, 6
                int sixbit;

                if (innerIdx <= 2) // innerIdx == 0, 2 
                {
                    if (innerIdx == 0)
                        sixbit = data[byteIdx] >> 2;
                    else
                        sixbit = data[byteIdx] & 0x3F;
                }
                else // innerIdx ==  4, 6
                {
                    sixbit = (data[byteIdx] << (innerIdx - 2)) & 0x3F;

                    // Will also need bits from next byte (If available)
                    if (byteIdx + 1 < data.Length)
                        sixbit |= (data[byteIdx + 1] >> (10 - innerIdx));
                }

                ascii[i] = sixbitToAscii[sixbit];
            }

            return Encoding.ASCII.GetString(ascii);
        }

        public static ulong GetUnsigned(byte[] ais, int bitOffset, int bitLength)
        {
            int firstByte = bitOffset / 8;
            int lastByte = (bitOffset + bitLength - 1) / 8;
            ulong returnValue = 0;

            // How many bytes to shift forward by
            // This starts at a possibly negative value, to trim the excess bits in last byte.
            int shiftBy = (bitOffset + bitLength) - (lastByte + 1) * 8;

            // First we handle the easy bytes, all those other than the first
            for (int i = lastByte; i > firstByte; i--)
            {
                if (shiftBy > 0)
                    returnValue += (ulong)(ais[i] << shiftBy);
                else
                    returnValue += (ulong)(ais[i] >> -shiftBy);

                shiftBy += 8;
            }

            // The first byte may contain leading excess bits, we mask these away
            int firstByteMask = (1 << (8 - bitOffset % 8)) - 1;
            byte adjFirstByte = (byte)(ais[firstByte] & firstByteMask);

            // Handle the first byte just like any other byte
            if (shiftBy > 0)
                returnValue += (ulong)(adjFirstByte << shiftBy);
            else
                returnValue += (ulong)(adjFirstByte >> -shiftBy);

            return returnValue;
        }

        public static long GetSigned(byte[] ais, int bitOffset, int bitLength)
        {
            ulong unsigned = GetUnsigned(ais, bitOffset, bitLength);

            // If signed (Highest-bit == 1) return signed value.
            if ((unsigned & (1UL << (bitLength - 1))) > 0)
                return (long)(unsigned - (1ul << bitLength));

            return (long)unsigned;
        }

        public static string GetString(byte[] ais, int bitOffset, int bitLength)
        {
            int numberChars = bitLength / 6; // We get a whole number of chars, rounded down if necessary.
            StringBuilder sb = new StringBuilder(numberChars);

            for (int i = 0; i < numberChars; i++)
            {
                int bitIdx = bitOffset + 6 * i;
                int byteIdx = bitIdx / 8;
                int innerIdx = bitIdx % 8; // = 0, 2, 4, 6
                int sixbit;

                if (innerIdx <= 2) // innerIdx == 0, 2 
                {
                    if (innerIdx == 0)
                        sixbit = ais[byteIdx] >> 2;
                    else
                        sixbit = ais[byteIdx] & 0x3F;
                }
                else // innerIdx ==  4, 6
                {
                    sixbit = (ais[byteIdx] << (innerIdx - 2)) & 0x3F;

                    // Will also need bits from next byte (If available)
                    if (byteIdx + 1 < ais.Length)
                        sixbit |= (ais[byteIdx + 1] >> (10 - innerIdx));
                }

                // The character representing zero is the terminating character for these strings. So we exit when sixbit is zero.
                if (sixbit == 0)
                    return sb.ToString().TrimEnd();

                sb.Append(sixbitToChar[sixbit]);
            }

            return sb.ToString().TrimEnd();
        }
    }

} // namespace
