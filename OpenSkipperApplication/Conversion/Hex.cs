/*
	Copyright (C) 2005 Rudy Guzman

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
using System.Linq;
using System.Text;

namespace OpenSkipperApplication
{
    /// <summary>
    /// This class stores a hexadecimal formatted value.
    /// </summary>
    /// <remarks>
    /// This class stores a hex value representation in an array of bytes.  There is  
    /// no limit to how big hex representation can be; however, this class has not
    /// been optimized to handle extremely large arrays.  The whole purpose is to
    /// make small sets of hex values human readable.
    /// </remarks>
    /// <permission cref="https://www.codeproject.com/Articles/10189/Hexadecimal-Binary-and-Decimal-conversions">
	/// Use of this software is privilege of CodeProject members. Feel free to modify
	/// and report any bugs to Codeproject.com.
    /// </permission>
	/// <history>
	/// 	[Rudy Guzman]	4/10/2005	Created
    /// 	[Michael Lucas] 2/1/2018    Added Reverse
	/// </history>
    public class Hex
    {
        byte[] bytes;

        /// <summary>
        /// Get number of bytes
        /// </summary>
        public int Count
        {
            get
            {
                if (bytes == null)
                {
                    return 0;
                }
                else
                {
                    return bytes.Length;
                }
            }
        }

        #region CONSTRUCTORS

        public Hex()
        {
            bytes = null;
        }

        /// <summary>
        /// Initialize a new instance of this class from another instance of "Hex" value.
        /// </summary>
        /// <param name="hexData">Hex value.</param>
        public Hex(Hex hexData)
        {
            bytes = hexData.ToBytes();
        }

        /// <summary>
        /// Initialize a new instance of this class from a string that contains a valid 
        /// sequence of hexadecimal numbers.
        /// </summary>
        /// <param name="data"></param>
        public Hex(string data)
        {
            bytes = this.ParseByteString(data);
        }

        /// <summary>
        /// Initialize a new instance of this class from an array of bytes.
        /// </summary>
        /// <param name="byteArray">Array of bytes.</param>
        public Hex(byte[] byteArray)
        {
            bytes = this.CopyBytes(byteArray);
        }

        /// <summary>
        /// Initialize a new instance of this class from an unsigned integer number.
        /// </summary>
        /// <param name="data">Initial data to create an instance of this class.</param>
        /// <param name="numBytes">Number of bytes to create this class.</param>
        public Hex(uint data, int numBytes)
        {
            string format = String.Format("X{0}", numBytes * 2);
            string hexString = data.ToString(format);

            bytes = this.ParseByteString(hexString);
        }

        /// <summary>
        /// Initialize a new instance of this class from a signed integer number.
        /// </summary>
        /// <param name="data">Initial data to create an instance of this class.</param>
        /// <param name="numBytes">Number of bytes to create this class.</param>
        public Hex(int data, int numBytes)
        {
            string format = String.Format("X{0}", numBytes * 2);

            if (data < 0)
            {
                data += (int)Math.Pow(2, numBytes * 8);
            }

            string hexString = data.ToString(format);

            bytes = this.ParseByteString(hexString);
        }

        public Hex(long data, int numBytes)
        {
            string format = String.Format("X{0}", numBytes * 2);

            if (data < 0)
            {
                data += (long)Math.Pow(2, numBytes * 8);
            }

            string hexString = data.ToString(format);

            bytes = this.ParseByteString(hexString);
        }

        #endregion

        public void Reverse()
        {
            bytes = bytes.Reverse().ToArray();
        }

        /// <summary>
        /// Converts each byte stored in this class to hex number and return string containing all hex bytes.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (bytes != null && bytes.Length > 0)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("X2"));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert to unsigned integer.
        /// </summary>
        /// <returns>Unsigned integer that represents the hexadecimal value of this instance.</returns>
        public uint ToUInt()
        {
            return new Bits(this.ToBytes()).ToUInt();
        }

        /// <summary>
        /// Convert to signed integer.
        /// </summary>
        /// <returns>Signed integer that represents the hexadecimal value of this instance.</returns>
        public int ToInt()
        {
            return new Bits(this.ToBytes()).ToInt();
        }

        /// <summary>
        /// Convert to bytes
        /// </summary>
        /// <returns>Byte array that represents the hexadecimal values stored by this instance.</returns>
        /// <remarks>
        /// Index 0 of the byte array is the least significant value of the hexadecimal representation of this method.
        /// </remarks>
        public byte[] ToBytes()
        {
            byte[] newBytes = null;

            if (bytes != null && bytes.Length > 0)
            {
                newBytes = new byte[bytes.Length];

                for (int i = 0; i < bytes.Length; i++)
                {
                    newBytes[i] = bytes[i];
                }
            }

            return bytes;
        }

        #region PRIVATE METHODS

        /// <summary>
        /// Parse string into bytes but then place bytes in reverse because the first byte in a string
        /// is the MSB (last byte).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        private byte[] ParseByteString(string data)
        {
            byte[] byteArray = null;
            int byteCount = 0;
            data = data.Replace("-", "").Replace(" ", "");   // Remove dashes and spaces

            if (data != null && data.Length > 0)
            {
                if (data.Length % 2 != 0)  //make sure we have an even number of characters
                {
                    data = "0" + data;
                }

                byteCount = data.Length / 2;
                byteArray = new byte[byteCount];
                string sTemp;

                for (int i = 0; i < byteCount; i++)
                {
                    sTemp = data.Substring(i * 2, 2);
                    byteArray[i] = Convert.ToByte(sTemp, 16);
                }
            }

            return byteArray;
        }

        private byte[] CopyBytes(byte[] byteArray)
        {
            byte[] newByteArray = null;
            int numberOfBytes = byteArray.Length;

            if (numberOfBytes > 0)
            {
                newByteArray = new byte[numberOfBytes];

                for (int i = 0; i < newByteArray.Length; i++)
                {
                    newByteArray[i] = byteArray[i];
                }
            }

            return newByteArray;
        }
        #endregion

    } // class

} // namespace
