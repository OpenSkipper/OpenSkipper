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
using System.ComponentModel;

namespace CANHandler
{
    /// <summary>
    /// Base class for all NMEA 2000 fields
    /// </summary>
    public abstract class N2kField : msgField
    {
        #region Public Constants

        public const double DoubleNA = 0xff;
        public const int IntNA = 0xff;

        #endregion

        [Description("Bit offset of start of data, counting left-to-right. e.g. ---XXXX- has a bit offset of 3")]
        public int BitOffset { get; set; } // counting left to right, eg ---XXXX- has a bit offset of 3
        [Description("Bit length of data, use 0 for 'all the data'")]
        public int BitLength { get; set; }  // 0 means "all the data"

        // The following get the offset and length in terms of bytes, if possible.
        // The following attributes do not show in the PropertyGrid.
        [BrowsableAttribute(false)]
        public int ByteOffset
        {
            get
            {
                if ((BitOffset & 7) != 0)
                {
                    throw new Exception("The field's bit offset is not on a byte boundary.");
                }
                else
                {
                    return BitOffset >> 3;
                }
            }
        }

        [BrowsableAttribute(false)]
        public int ByteLength
        {
            get
            {
                if ((BitLength & 7) != 0)
                {
                    throw new Exception("The field's bit length is not a byte multiple.");
                }
                else
                {
                    return BitLength >> 3;
                }
            }
        }

        [BrowsableAttribute(false)]
        public int BitOffsetWithinByte { get { return BitOffset & 7; } }

        // Default method for displaying any field; in this case as hex
        public virtual string ToString(byte[] d)
        {
            if (BitLength == 0)
            {
                // This dummy bit length means all the data
                return BitConverter.ToString(d, ByteOffset, d.Length - ByteOffset);
            }
            else if (BitLength < 8)
            {
                return Tools.Binary(d[BitOffset >> 3], BitOffsetWithinByte, BitLength);
            }
            else if (((BitLength & 7) == 0) && ((BitOffset & 7) == 0))
            {
                return BitConverter.ToString(d, ByteOffset, ByteLength);
            }
            else if (BitLength <= 16)
            {
                ushort ival = (ushort)(BitConverter.ToUInt16(d, BitOffset >> 3) >> (BitOffset & 7));
                ushort mask = (ushort)((1 << BitLength) - 1);
                ival &= mask;
                return ival.ToString();
            }
            else if (BitLength <= 32)
            {
                uint ival = (uint)(BitConverter.ToUInt32(d, BitOffset >> 3) >> (BitOffset & 7));
                uint mask = ((uint)1 << BitLength) - 1;
                ival &= mask;
                return ival.ToString();
            }
            else if (BitLength <= 64)
            {
                UInt64 ival = BitConverter.ToUInt64(d, BitOffset >> 3) >> (BitOffset & 7);
                UInt64 mask = ((UInt64)1 << BitLength) - 1;
                ival &= mask;
                return ival.ToString();
            }
            else
            {
                return "Unhandled field data";
            }
        }

        // Default method for displaying any field; in this case as hex
        public virtual string DebugString(byte[] d)
        {
            if (BitLength == 0)
            {
                // This dummy bit length means all the data
                return Tools.Dump(d, BitOffset, d.Length * 8);
            }
            else
            {
                return Tools.Dump(d, BitOffset, BitLength);
            }
        }

        // List of this field and all derived fields
        public static Type[] AllFieldTypes()
        {
            // var TypeNameList = new List<string>();
            var TypeList = new List<Type>();
            foreach (Type t in typeof(N2kField).Assembly.GetTypes())
            {
                if (!t.IsAbstract && (t.IsSubclassOf(typeof(N2kField)) || t == typeof(N2kField)))
                {
                    // string theClass = t.FullName.Substring(t.FullName.LastIndexOf(".")+1);
                    // TypeNameList.Add(t.DisplayName);
                    TypeList.Add(t);
                }
            }
            return TypeList.ToArray();
        }

        [BrowsableAttribute(true)]
        // Returns the class tpye of the field
        public string FieldType
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public static N2kField CreateNewField(Type fieldType)
        {
            //Define the type of the control you want to create an instance of using reflection. 
            try
            {
                return (N2kField)(Activator.CreateInstance(fieldType));
            }
            catch
            {
                //Set the control to null 
                return null;
            }
        }

        public override int CompareTo(object o)
        {
            // We sort by BitOffset, then (if these are equal) BitLength, then (if equal) by DisplayName
            N2kField f = o as N2kField;
            if ((f) == null) throw new Exception("Cannot compare a field with an " + o.GetType().ToString());
            if (BitOffset < f.BitOffset) return -1;
            if (BitOffset > (f).BitOffset) return +1;
            if (BitLength < f.BitLength) return -1;
            if (BitLength > f.BitLength) return +1;
            if (Name != null)
            {
                if (f.Name != null)
                {
                    return Name.CompareTo(f.Name);
                }
                else
                {
                    return +1;  // DisplayName!=null > o.DisplayName==null
                }

            }
            else
            {  // DisplayName==null
                if (f.Name == null)
                {
                    return 0;
                }
                else
                { // o.DisplayName != null
                    return -1;
                }
            }
        }

    } // class

} // namespace
