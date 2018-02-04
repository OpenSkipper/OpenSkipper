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
using System.ComponentModel;
using System.Xml.Serialization;

namespace CANHandler
{
    /// <summary>
    /// Base class for all numeric PGN fields, including enum fields
    /// This treats the field as an unsigned int; derived classes change this behavour
    /// All numeric fields can be treated as doubles, regardless of their underlying types
    /// </summary>
    public abstract class N2kNumericField : N2kField
    {

        // Every numeric field has a formatter
        protected FormatEnum _FormatAs;

        [Description("The current formatter")]
        [XmlIgnore]
        public Formatter Formatter { get; set; }

        [Description("Determines how the numeric value will be formatted")]
        public FormatEnum FormatAs
        {
            get
            {
                return _FormatAs;
            }
            set
            {
                _FormatAs = value;
                Formatter = Formatter.Create(value);
            }
        }

        public virtual bool ShouldSerializeFormatAs() { return FormatAs != FormatEnum.Number; }

        // Constants used to detect missing/invalid data. These are all unsigned ints
        // We have an array for the constants used for low bit counts
        //public uint ErrorValue(int BitLength) {
        //    switch (BitLength) {
        //        case 
        //    }
        public double Min { get; set; }
        public double Max { get; set; }

        public virtual bool ShouldSerializeMin() { return Min != 0; }
        public virtual bool ShouldSerializeMax() { return Max != 0; }

        [Browsable(false)]
        public int MaxRawIntValue
        {
            get
            {
                switch (BitLength)
                {
                    case 32: return 0x7FFFFFFF;
                    case 24: return 0x7FFFFF;
                    case 16: return 0x7FFF;
                    case 8: return 0x7F;
                    case 7: return 0x3F;
                    case 6: return 0x1F;
                    case 5: return 0x0F;
                    case 4: return 0x07;
                    case 3: return 0x03;
                    case 2: return 0x01;
                    default: return (int)((uint)1 << (BitLength - 1)) - 1;
                }
            }
        }

        [Browsable(false)]
        public long MaxRawLongValue
        {
            get
            {
                if (BitLength == 64)
                {
                    return 0x7FFFFFFFFFFFFFFF;
                }
                else if (BitLength > 32)
                {
                    return (long)((ulong)1 << (BitLength - 1)) - 1;
                }
                else
                {
                    return MaxRawIntValue;
                }
            }
        }

        [Browsable(false)]
        public uint MaxRawUIntValue
        {
            get
            {
                switch (BitLength)
                {
                    case 32: return 0xFFFFFFFF;
                    case 24: return 0xFFFFFF;
                    case 16: return 0xFFFF;
                    case 8: return 0xFF;
                    case 7: return 0x7F;
                    case 6: return 0x3F;
                    case 5: return 0x1F;
                    case 4: return 0x0F;
                    case 3: return 0x07;
                    case 2: return 0x03;
                    default: return ((uint)1 << BitLength) - 1;
                }
            }
        }

        [Browsable(false)]
        public ulong MaxRawULongValue
        {
            get
            {
                if (BitLength == 64)
                {
                    return 0xFFFFFFFFFFFFFFFF;
                }
                else if (BitLength > 32)
                {
                    return ((ulong)1 << BitLength) - 1;
                }
                else
                {
                    return MaxRawUIntValue;
                }
            }
        }

        protected int RawIntValue(byte[] d)
        {
            // Get the raw signed (up to 32 bit) value from the bits in m, least significant byte first
            if (!System.BitConverter.IsLittleEndian)
            {
                // Not yet implemented
                throw new NotImplementedException();
            }
            // Handle byte-aligned values
            if ((BitOffset & 7) == 0)
            {
                int ByteOffset = this.ByteOffset;
                if (BitLength == 8)
                {
                    return d[ByteOffset] < 128 ? (int)d[ByteOffset] : (int)d[ByteOffset] - 256;
                }
                else if (BitLength == 16)
                {
                    return (int)System.BitConverter.ToInt16(d, ByteOffset);
                }
                else if (BitLength == 24)
                {
                    uint value = d[ByteOffset++] + (uint)(d[ByteOffset++] << 8) + (uint)(d[ByteOffset++] << 16);
                    return value < 0x800000 ? (int)value : (int)(value - 0x1000000);
                }
                else if (BitLength == 32)
                {
                    return (int)System.BitConverter.ToInt32(d, ByteOffset);
                }
            }
            // Handle single-byte values
            if ((BitLength > 3) && (BitLength < 8) && (BitOffset >> 3 == (BitOffset + BitLength - 1) >> 3))
            {
                byte b = d[BitOffset >> 3];
                // Discard low order bits
                b = (byte)(b >> (BitOffset & 7));
                // And now mask out high bits;
                b = (byte)(b & ((byte)0xFF >> (7 - ((BitLength - 1) & 7))));
                if (b >= (1 << BitLength - 1))
                {
                    return b - (1 << BitLength); //make a negative value
                }
                else
                {
                    return (int)b;
                }
            }
            // Handle remaining cases
            ulong v = RawUIntValue(d);
            return (int)(((v & ((ulong)1 << (BitLength - 1))) > 0) ? (((long)1 << BitLength) - (long)v) : (long)v);
        }

        protected long RawLongValue(byte[] d)
        {
            if (BitLength == 64)
            {
                return System.BitConverter.ToInt64(d, ByteOffset);
            }
            else if (BitLength > 32)
            {
                throw new Exception("Long values between 33 and 63 bits long are not handled.");
            }
            else
            {
                return (long)RawIntValue(d);
            }
        }

        protected uint RawUIntValue(byte[] d)
        {
            if (BitLength == 8)
            {
                int ByteOffset = this.ByteOffset;
                return d[ByteOffset];
            }
            else if (BitLength == 16)
            {
                int ByteOffset = this.ByteOffset;
                return (uint)d[ByteOffset++] + (uint)(d[ByteOffset] << 8); // or use BitConverter.ToUInt16(m.Data, BitOffset >> 3) if we can be sure of little/big endian;
            }
            else if (BitLength == 24)
            {
                int ByteOffset = this.ByteOffset;
                return d[ByteOffset++] + (uint)(d[ByteOffset++] << 8) + (uint)(d[ByteOffset++] << 16);
            }
            else if (BitLength == 32)
            {
                int ByteOffset = this.ByteOffset;
                return d[ByteOffset++] + (uint)(d[ByteOffset++] << 8) + (uint)(d[ByteOffset++] << 16) + (uint)(d[ByteOffset] << 24);
            }
            else if (BitLength < 8 && (BitOffset >> 3 == (BitOffset + BitLength - 1) >> 3))
            {
                // We are extracting part of a byte
                byte b = d[BitOffset >> 3];
                // Discard high order bits
                b = (byte)(b & (0xFF >> (7 - ((BitOffset + BitLength - 1) & 7))));
                // And now place low order bits in the correct position;
                b = (byte)(b >> (BitOffset & 7));
                return (uint)b;
            }
            else
            {
                if (BitLength > 32) throw new Exception("Integers over 32 bits but not 64 bits are not implemented.");
                int iFirstByte = BitOffset >> 3;
                int iLastByte = (BitOffset + BitLength - 1) >> 3;
                if (iLastByte - iFirstByte + 1 > 8) throw new Exception("64 bit numbers spanning 9 bytes are not implemented.");
                ulong v = (ulong)(d[iFirstByte] >> (BitOffset & 7));
                int iShiftCount = 8 - (BitOffset & 7);
                for (int i = iFirstByte + 1; i < iLastByte; i++)
                {
                    v = v + ((ulong)d[i] << iShiftCount);
                    iShiftCount += 8;
                }
                if (iLastByte > iFirstByte)
                {
                    v = v + (((ulong)d[iLastByte] & ((ulong)0xFF >> (7 - ((BitOffset + BitLength - 1) & 7)))) << iShiftCount);
                }
                else
                {
                    v = v & ((ulong)0xFF >> (7 - ((BitLength - 1) & 7)));
                }
                return (uint)v;
            }
        }

        protected ulong RawULongValue(byte[] d)
        {
            if (BitLength == 64)
            {
                return System.BitConverter.ToUInt64(d, ByteOffset);
            }
            else if (BitLength > 32)
            {
                throw new Exception("ULong values between 33 and 63 bits long are not handled.");
            }
            else
            {
                return (ulong)RawUIntValue(d);
            }
        }

        //// Return the value as a double, regardless of its underlying type, and ignoring any error/unavailable values
        //protected virtual double RawUDblValue(byte[] d) {
        //    return (double)RawUIntValue(d);
        //}

        // Check if the value is one of the special signal values; these need to be overridden if required
        // public abstract double DblValue(byte[] d);
        // Every numeric field must return its value as a double so it can be plotted etc
        // An exception should be thrown if the value is not available or an error.

        // public abstract string _ToString(byte[] d);
        // This always returns a text representation of the numeric value without any units, and with no exception
        // handling for not available or error values

        public abstract FieldValueState GetState(double v);
        public abstract double GetValue(byte[] d, out FieldValueState state);
        public abstract double GetRawValue(byte[] d, out FieldValueState state);

        public override string ToString(byte[] d)
        {
            // This returns a string suitable for displaying to the user
            FieldValueState valueState;
            double value = GetValue(d, out valueState);

            return Formatter.Format(value, valueState);
            //return IsNotAvailable(d) ? "NotAvailable" : IsError(d) ? "IsError" : _ToString(d);
        }

    } // class

} // namespace
