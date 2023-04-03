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
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using OpenSkipperApplication;
using System.Windows.Forms;
using OpenSkipperApplication.Properties;
using System.Text;
using System.Reflection;
using Parameters;
using CANStreams;
using CANDefinitions;
using CANReaders;

namespace CANHandler
{
    public enum FileTypeEnum
    {
        NativeXMLFile = 0,
        Internal = 2
    }

    /// <summary>
    /// Handles application-wide reporting of information, warnings, and errors.
    /// Consumers (i.e. the debug console) subscribe to the handlers events
    /// </summary>
    public static class ReportHandler
    {
        public static event Action<string> InfoReceived;
        public static event Action<string> WarningReceived;
        public static event Action<string> ErrorReceived;

        public static void LogInfo(string s)
        {
            if (InfoReceived != null)
                InfoReceived(s);
        }
        public static void LogWarning(string s)
        {
            if (WarningReceived != null)
                WarningReceived(s);
        }
        public static void LogError(string s)
        {
            if (ErrorReceived != null)
                ErrorReceived(s);
        }
    }

    /// <summary>
    /// Provides miscellaneous functions to display binary data
    /// </summary>
    public static class Tools
    {
        public static string Binary(byte b) {
            string s= "";
            for (int i=0; i<8; i++) {
                if ((b & 0x80) > 0) {
                    s = s + '1';
                } else {
                    s = s + '0';
                }
                b=(byte)(b << 1);
            }
            return s;
        }
        public static string Binary(byte b, int iFirst, int iLength) {
            return Binary(b).Substring(8 - iFirst - iLength, iLength);
        }
        public static string BinaryDump(byte[] data, int iFirst, int iLength) {
            string s = "";
            byte b = data[iFirst >> 3];
            int iFirstBit = iFirst & 7;
            int iLastBit = (iFirst+iLength-1) & 7;
            for (int i = 0; i < 8; i++) {
                if (((7-i) < iFirstBit) || ((7-i) > iLastBit)) {
                    s = s + '.';
                } else  if ((b & 0x80) > 0) {
                    s = s + '1';
                } else {
                    s = s + '0';
                }
                b = (byte)(b << 1);
            }
            return s;
        }
        public static string Dump(byte[] b, int iFirst, int iLength) {
            string s="";
            //if (iLength > 8) {
            int iFirstByte = iFirst >> 3;
            int iLastByte = ((iFirst+iLength+7) >> 3)-1;
            int iFirstByteToOutput = iFirstByte & 0xFFFFF8;
            int iEndByteToOutput = ((iLastByte+8) & 0xFFFFF8);
            // if (iFirstByte > iLastByte) { int j = 0; }
            // int iOutputCount = 8*((b.Length+7)>>3);
            s = String.Format("  {0,3}: ", iFirstByteToOutput);
            for (int i = iFirstByteToOutput; i < iEndByteToOutput; i++) {
                if (i > iFirstByteToOutput && (i & 7) == 0) {
                    s = s + "\n  ";
                    s = s+ String.Format("{0,3}: ", i);
                } 
                if (i >= b.Length) {
                    s = s + "   ";
                } else  if ((i < iFirstByte) || (i > iLastByte)) {
                    s = s + "-- ";
                } else {
                    s = s + System.BitConverter.ToString(b, i, 1) + " ";
                }
            }
            if (iLength > 8) {
                return s + "........";
            } else {
                return s + BinaryDump(b,iFirst, iLength);
            }
        }
    }

    /// <summary>
    /// Helper class for displaying our EnumPair as a pretty object in the PropertyGrid
    /// See http://msdn.microsoft.com/en-us/library/aa302326.aspx
    /// We should really edit the dictionary directly; see http://www.differentpla.net/content/2005/02/using-propertygrid-with-dictionary
    /// </summary>
    public class EnumPairConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context,
                                      System.Type destinationType)
        {
            if (destinationType == typeof(EnumPair))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context,
                                       CultureInfo culture,
                                       object value,
                                       System.Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is EnumPair)
            {
                EnumPair e = (EnumPair)value;
                return e.Key + "=" + e.Value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// This class allows pretty display and editing of key/value pairs in a PropertyGrid
    /// </summary>
    [TypeConverterAttribute(typeof(EnumPairConverter))]
    public class EnumPair
    {
        [XmlAttribute("Value")]
        public uint Key { get; set; }
        [XmlAttribute("Name")]
        public string Value { get; set; }

        public EnumPair() { }
        public EnumPair(uint k, string v)
        {
            Key = k;
            Value = v;
        }
        public override string ToString()
        {
            return Key + "=" + Value;
        }
    }

    /// <summary>
    /// Helper class for displaying our EnumPair as a pretty object in the PropertyGrid
    /// See http://msdn.microsoft.com/en-us/library/aa302326.aspx
    /// We should really edit the dictionary directly; see http://www.differentpla.net/content/2005/02/using-propertygrid-with-dictionary
    /// </summary>
    public class SEnumPairConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context,
                                      System.Type destinationType)
        {
            if (destinationType == typeof(EnumPair))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context,
                                       CultureInfo culture,
                                       object value,
                                       System.Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is EnumPair)
            {
                EnumPair e = (EnumPair)value;
                return e.Key + "=" + e.Value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// This class allows pretty display and editing of key/value pairs in a PropertyGrid
    /// </summary>
    [TypeConverterAttribute(typeof(SEnumPairConverter))]
    public class SEnumPair
    {
        [XmlAttribute("Value")]
        public string Key { get; set; }
        [XmlAttribute("Name")]
        public string Value { get; set; }

        public SEnumPair() { }
        public SEnumPair(string k, string v)
        {
            Key = k;
            Value = v;
        }
        public override string ToString()
        {
            return Key + "=" + Value;
        }
    }

    /// <summary>
    /// Base class for all NMEA 2000 fields
    /// </summary>
    public abstract class msgField : IComparable
    {
        [XmlAttribute]
        [Description("Name of the field")]
        public virtual string Name { get; set; }
        [Description("Description of the field")]
        public virtual string Description { get; set; }
        public abstract int CompareTo(object o);
    }

    /// <summary>
    /// Base class for all NMEA 2000 fields
    /// </summary>
    public abstract class N2kField : msgField
    {
        [Description("Bit offset of start of data, counting left-to-right. e.g. ---XXXX- has a bit offset of 3")]
        public int BitOffset { get; set; } // counting left to right, eg ---XXXX- has a bit offset of 3
        [Description("Bit length of data, use 0 for 'all the data'")]
        public int BitLength { get; set; }  // 0 means "all the data"

        // The following get the offset and length in terms of bytes, if possible.
        // The following attributes do not show in the PropertyGrid.
        [BrowsableAttribute(false)]
        public int ByteOffset {
            get {
                if ((BitOffset & 7) != 0) {
                    throw new Exception("The field's bit offset is not on a byte boundary.");
                } else {
                    return BitOffset >> 3;
                }
            }
        }
        [BrowsableAttribute(false)]
        public int ByteLength {
            get {
                if ((BitLength & 7) != 0) {
                    throw new Exception("The field's bit length is not a byte multiple.");
                } else {
                    return BitLength >> 3;
                }
            }
        }
        [BrowsableAttribute(false)]
        public int BitOffsetWithinByte { get { return BitOffset & 7; } }

        // Default method for displaying any field; in this case as hex
        public virtual string ToString(byte[] d) {
            if (BitLength == 0) {
                // This dummy bit length means all the data
                return BitConverter.ToString(d, ByteOffset, d.Length - ByteOffset);
            } else if (BitLength < 8) {
                return Tools.Binary(d[BitOffset >> 3],BitOffsetWithinByte,BitLength);
            } else if (((BitLength & 7) == 0) && ((BitOffset & 7)==0)) {
                return BitConverter.ToString(d, ByteOffset, ByteLength);
            } else if (BitLength <= 16) {
                ushort ival = (ushort)(BitConverter.ToUInt16(d, BitOffset >> 3) >> (BitOffset & 7));
                ushort mask = (ushort)((1 << BitLength) - 1);
                ival &= mask;
                return ival.ToString();
            } else if (BitLength <= 32) {
                uint ival = (uint)(BitConverter.ToUInt32(d, BitOffset >> 3) >> (BitOffset & 7));
                uint mask = ((uint)1 << BitLength) - 1;
                ival &= mask;
                return ival.ToString();
            }
            else if (BitLength <= 64)
            {
                UInt64 ival = BitConverter.ToUInt64(d, BitOffset >> 3) >> (BitOffset & 7);
                UInt64 mask = ((UInt64)1 << BitLength)-1 ;
                ival &= mask;
                return ival.ToString();
            } else {
                return "Unhandled field data";
            }
        }
        // Default method for displaying any field; in this case as hex
        public virtual string DebugString(byte[] d) {
            if (BitLength == 0) {
                // This dummy bit length means all the data
                return Tools.Dump(d, BitOffset, d.Length *8);
            } else {
                return Tools.Dump(d, BitOffset, BitLength);
            }
        }

        // List of this field and all derived fields
        public static Type[] AllFieldTypes() {
            // var TypeNameList = new List<string>();
            var TypeList = new List<Type>();
            foreach (Type t in typeof(N2kField).Assembly.GetTypes()) {
                if (!t.IsAbstract && (t.IsSubclassOf(typeof(N2kField)) || t==typeof(N2kField))) {
                    // string theClass = t.FullName.Substring(t.FullName.LastIndexOf(".")+1);
                    // TypeNameList.Add(t.DisplayName);
                    TypeList.Add(t);
                }
            }
            return TypeList.ToArray();
        }
        //    string theNameSpace = t.FullName.Substring(0,t.FullName.LastIndexOf("."));
        //    string theClass = t.FullName.Substring(t.FullName.LastIndexOf(".")+1);

        //    if (colNamespaces.ContainsKey(theNameSpace))
        //    {
        //        colNamespaces[theNameSpace].Add(theClass);
        //    }
        //    else
        //    {
        //        List<string> classes = new List<string>();
        //        classes.Add( theClass );
        //        colNamespaces[theNameSpace] = classes;
        //    }
        //}

        //   AssemblyLoadEventArgs 
        //}

        [BrowsableAttribute(true)]
        // Returns the class tpye of the field
        public string FieldType {
            get {
                return this.GetType().Name;
            }
        }

        public static N2kField CreateNewField(Type fieldType) {
            //Define the type of the control you want to create an instance of using reflection. 
            try {
                return (N2kField)(Activator.CreateInstance(fieldType));
            }
            catch {
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
            if (Name != null) {
                if (f.Name != null) {
                    return Name.CompareTo(f.Name);
                } else {
                    return +1;  // DisplayName!=null > o.DisplayName==null
                }

            } else {  // DisplayName==null
                if (f.Name == null) {
                    return 0;
                } else { // o.DisplayName != null
                    return -1;
                }
            }
        }
    }
    public class N2kBinaryField : N2kField
    {
/*        public override string ToString(byte[] d)
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
    public class N2kASCIIField : N2kField {
        public static string ReadString(byte[] d, int byteOffset, int byteLength) {
            string s = System.Text.Encoding.UTF8.GetString(d, byteOffset, byteLength);
            int end = s.IndexOf('\0');
            if (end >= 0)
            {
                return s.Substring(0, end);
            }
            return s;
        }
        // Display the string as text
        public override string ToString(byte[] d) {
            return ReadString(d, ByteOffset, ByteLength);
        }
        public string StringValue(byte[] d) {
            return ToString(d);
        }
    }

    // Variable string. First byte tells length of string and next byte is 1 - don't know the mening yet
    public class N2kStringField : N2kField
    {
        // Display the string as text
        public static string ReadString(byte[] d, int byteOffset, ref int byteLength)
        {
            int offset = byteOffset + 2;
            int length = d[byteOffset] - 2;
            byteLength = d[byteOffset];
            string s = "";
            if (length>0) s=System.Text.Encoding.UTF8.GetString(d, offset, length);
            return s;
        }
        public override string ToString(byte[] d)
        {
            int byteLength=0;
            string s=ReadString(d, ByteOffset, ref byteLength);
            return s;
        }
        public string StringValue(byte[] d)
        {
            return ToString(d);
        }
    }

    /// <summary>
    /// Base class for all numeric PGN fields, including enum fields
    /// This treats the field as an unsigned int; derived classes change this behavour
    /// All numeric fields can be treated as doubles, regardless of their undelrying types
    /// </summary>
    public abstract class N2kNumericField : N2kField {
        
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
        public int MaxRawIntValue {
            get {
                switch (BitLength) {
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
                    default: return (int)((uint)1 << (BitLength-1))-1;
                }
            }
        }
        [Browsable(false)]
        public long MaxRawLongValue {
            get {
                if (BitLength == 64) {
                    return 0x7FFFFFFFFFFFFFFF;
                } else if (BitLength > 32) {
                    return (long)((ulong)1 << (BitLength-1))-1;
                } else {
                    return MaxRawIntValue;
                }
            }
        }
        [Browsable(false)] 
        public uint MaxRawUIntValue {
            get {
                switch (BitLength) {
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
        public ulong MaxRawULongValue {
            get {
                if (BitLength == 64) {
                    return 0xFFFFFFFFFFFFFFFF;
                } else if (BitLength > 32) {
                    return ((ulong)1 << BitLength) - 1;
                } else {
                    return MaxRawUIntValue;
                }
            }
        }
        
        protected int RawIntValue(byte[] d) {
            // Get the raw signed (up to 32 bit) value from the bits in m, least significant byte first
            if (!System.BitConverter.IsLittleEndian) {
                // Not yet implemented
                throw new NotImplementedException();
            }
            // Handle byte-aligned values
            if ((BitOffset & 7) == 0)  {
                int ByteOffset = this.ByteOffset;
                if (BitLength == 8) {
                    return d[ByteOffset] < 128 ? (int)d[ByteOffset] : (int)d[ByteOffset] - 256;
                } else if (BitLength == 16) {
                    return (int)System.BitConverter.ToInt16(d, ByteOffset);
                } else if (BitLength == 24) {
                    uint value = d[ByteOffset++] + (uint)(d[ByteOffset++] << 8) + (uint)(d[ByteOffset++] << 16);
                    return value < 0x800000 ? (int)value : (int)(value - 0x1000000);
                } else if (BitLength == 32) {
                    return (int)System.BitConverter.ToInt32(d, ByteOffset);
                }
            }
            // Handle single-byte values
            if ((BitLength > 3) && (BitLength < 8) && (BitOffset >> 3 == (BitOffset + BitLength - 1) >> 3)) {
                byte b = d[BitOffset >> 3];
                // Discard low order bits
                b = (byte)(b >> (BitOffset & 7));
                // And now mask out high bits;
                b = (byte)(b & ((byte)0xFF >> (7-((BitLength-1) & 7))));
                if (b >= (1 << BitLength-1)) {
                    return b - (1 << BitLength); //make a negative value
                } else {
                    return (int)b;
                }
            }
            // Handle remaining cases
            ulong v = RawUIntValue(d);
            return (int)(((v & ((ulong)1 << (BitLength - 1))) > 0) ? (((long)1 << BitLength) - (long)v) : (long)v); 
        }
        protected long RawLongValue(byte[] d) {
            if (BitLength == 64) {
                return System.BitConverter.ToInt64(d, ByteOffset);
            } else if (BitLength > 32) {
                throw new Exception("Long values between 33 and 63 bits long are not handled.");
            } else {
                return (long)RawIntValue(d);
            }
        }
        protected uint RawUIntValue(byte[] d) {
            if (BitLength == 8) {
                int ByteOffset = this.ByteOffset;
                return d[ByteOffset];
            } else if (BitLength == 16) {
                int ByteOffset = this.ByteOffset;
                return (uint)d[ByteOffset++] + (uint)(d[ByteOffset] << 8); // or use BitConverter.ToUInt16(m.Data, BitOffset >> 3) if we can be sure of little/big endian;
            } else if (BitLength == 24) {
                int ByteOffset = this.ByteOffset;
                return d[ByteOffset++] + (uint)(d[ByteOffset++] << 8) + (uint)(d[ByteOffset++] << 16);
            } else if (BitLength == 32) {
                int ByteOffset = this.ByteOffset;
                return d[ByteOffset++] + (uint)(d[ByteOffset++] << 8) + (uint)(d[ByteOffset++] << 16) + (uint)(d[ByteOffset] << 24);
            } else if (BitLength < 8 && (BitOffset >> 3 == (BitOffset + BitLength - 1) >> 3)) {
                // We are extracting part of a byte
                byte b = d[BitOffset >> 3];
                // Discard high order bits
                b = (byte)(b & (0xFF >> (7 - ((BitOffset+BitLength-1) & 7))));
                // And now place low order bits in the correct position;
                b = (byte)(b >> (BitOffset & 7));
                return (uint)b;
            } else {
                if (BitLength > 32) throw new Exception("Integers over 32 bits but not 64 bits are not implemented.");
                int iFirstByte = BitOffset >> 3;
                int iLastByte = (BitOffset + BitLength - 1) >> 3;
                if (iLastByte - iFirstByte + 1 > 8) throw new Exception("64 bit numbers spanning 9 bytes are not implemented.");
                ulong v = (ulong)(d[iFirstByte] >> (BitOffset & 7));
                int iShiftCount = 8 - (BitOffset & 7);
                for (int i = iFirstByte + 1; i < iLastByte; i++) {
                    v = v + ((ulong)d[i] << iShiftCount);
                    iShiftCount += 8;
                }
                if (iLastByte > iFirstByte) {
                    v = v + (((ulong)d[iLastByte] & ((ulong)0xFF >> (7 - ((BitOffset + BitLength - 1) & 7)))) << iShiftCount);
                } else {
                    v = v & ((ulong)0xFF >> (7-((BitLength-1) & 7)));
                }
                return (uint)v;
            }
        }
        protected ulong RawULongValue(byte[] d) {
            if (BitLength == 64) {
                return System.BitConverter.ToUInt64(d, ByteOffset);
            } else if (BitLength > 32) {
                throw new Exception("ULong values between 33 and 63 bits long are not handled.");
            } else {
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
        public override string ToString(byte[] d)
        {
            // This returns a string suitable for displaying to the user
            FieldValueState valueState;
            double value = GetValue(d, out valueState);

            return Formatter.Format(value, valueState);
            //return IsNotAvailable(d) ? "NotAvailable" : IsError(d) ? "IsError" : _ToString(d);
        }
    }
    public class N2kIntField : N2kNumericField
    {
        public N2kIntField()
        {
            FormatAs = FormatEnum.Number;
        }
        public override FieldValueState GetState(double v)
        {
            return (v == MaxRawLongValue) ? FieldValueState.NotAvailable :
                    (v == MaxRawLongValue - 1) ? FieldValueState.Error :
                    FieldValueState.Valid;
        }
        public override double GetValue(byte[] d, out FieldValueState state)
        {
            double raw = RawIntValue(d);
            state = GetState(raw);
            return raw;
        }
    }
    public class N2kDblField : N2kIntField
    {
        public double Offset { get; set; }
        public double Scale { get; set; }

        public virtual bool ShouldSerializeOffset() { return Offset != 0; }
        public virtual bool ShouldSerializeScale() { return Scale != 1; }

        public N2kDblField()
        {
            FormatAs = FormatEnum.Number;
            Scale = 1;
        }
        // Uses same state function as it's base, IntField.
        public override double GetValue(byte[] d, out FieldValueState state)
        {
            double raw = RawLongValue(d);
            state = GetState(raw); 
            return raw * Scale + Offset;
        }
    }
    public class N2kUDblField : N2kDblField
    {
        // A double whose value is calculated from an unsigned int

        public N2kUDblField()
        {
            FormatAs = FormatEnum.Number;
        }
        public override FieldValueState GetState(double v)
        {
            return (v == MaxRawULongValue) ? FieldValueState.NotAvailable :
                    (v == MaxRawULongValue - 1) ? FieldValueState.Error :
                    FieldValueState.Valid;
        }
        public override double GetValue(byte[] d, out FieldValueState state)
        {
            double raw = RawULongValue(d);
            state = GetState(raw); 
            return raw * Scale + Offset; ;
        }
    }
    public class N2kUIntField : N2kNumericField
    {
        public N2kUIntField()
        {
            FormatAs = FormatEnum.Number;
            Formatter.FormatString = "G";
        }
        public override FieldValueState GetState(double v)
        {
            if (BitLength == 1) return FieldValueState.Valid;

            return (v == MaxRawULongValue) ? FieldValueState.NotAvailable :
                    (v == MaxRawULongValue - 1) ? FieldValueState.Error :
                    FieldValueState.Valid;
        }
        public override double GetValue(byte[] d, out FieldValueState state)
        {
            double raw = RawUIntValue(d);
            state = GetState(raw);
            return raw;
        }
    }
    public class N2kInstanceField : N2kUIntField
    {
        public N2kInstanceField()
        {
        }
    }
    public class N2kEnumField : N2kUIntField
    {
        [XmlIgnore]
        [Browsable(false)]
        public Dictionary<uint, string> EnumLabels { get; set; }

        // FormatAs does not make sense in regard to an enumfield, so we hide it
        // The easiest way to do that is to redeclare it, applying browsable false attribute.
        // Note: Xml does not like new definition not having a set, so we put one here that does nothing
        [XmlIgnore]
        [Browsable(false)]
        public new FormatEnum FormatAs { get { return FormatEnum.Number; } set { } }

        public N2kEnumField() {
            EnumLabels = new Dictionary<uint, string>();
        }

        // Array accessors are used to enable easy XML serialization
        [XmlArray("EnumValues")]
        // [XmlArrayItem("EnumPair", typeof(EnumPair))]
        public EnumPair[] EnumPairArray
        {
            get {
                var Value = new EnumPair[EnumLabels.Count];
                int i = 0;
                foreach (var d in EnumLabels) {
                    Value[i++] = new EnumPair(d.Key, d.Value);
                }
                return Value;
            }
            set {
                EnumLabels = new Dictionary<uint, string>();
                if (value == null) return; // Kees file has some "lookup tables" without data!
                foreach (var d in value) {
                    EnumLabels[d.Key] = d.Value;
                }
            }
        }

        public override string ToString(byte[] d)
        {
            // Get the value from the bits in d
            // We always return one of the enum values if possible, otherwise we check for error or not available
            uint raw = RawUIntValue(d);
            string s;
            if (EnumLabels.TryGetValue(raw, out s))
            {
                return s + "(" + raw + ")";
            }
            else
            {
                // Check state
                FieldValueState state = GetState(raw);
                return (state == FieldValueState.NotAvailable) ? "NotAvailable" :
                    (state == FieldValueState.Error) ? "Error" :
                    "Unrecognised enum value " + raw.ToString();
            }
        }
    }

    /// <summary>
    /// Base class for all N0183 fields
    /// </summary>
    public abstract class N0183Field : msgField
    {
        // Static
        public static Type[] AllFieldTypes()
        {
            var TypeList = new List<Type>();
            foreach (Type t in typeof(N0183Field).Assembly.GetTypes())
            {
                if (!t.IsAbstract && (t.IsSubclassOf(typeof(N0183Field)) || t == typeof(N0183Field)))
                {
                    // string theClass = t.FullName.Substring(t.FullName.LastIndexOf(".")+1);
                    // TypeNameList.Add(t.DisplayName);
                    TypeList.Add(t);
                }
            }
            return TypeList.ToArray();
        }
        public static N0183Field CreateNewField(Type fieldType)
        {
            //Define the type of the control you want to create an instance of using reflection. 
            try
            {
                return (N0183Field)(Activator.CreateInstance(fieldType));
            }
            catch
            {
                //Set the control to null 
                return null;
            }
        }

        // Public
        //[XmlAttribute] AJM: This was preventing the segment index loading; it was showing as 0 all the time, breaking NMEA0183
        [Description("What index the field relates to")]
        public int SegmentIndex { get; set; }
        [BrowsableAttribute(true)]
        public string FieldType
        {
            get
            {
                return this.GetType().Name;
            }
        }
        
        // Methods
        public virtual string ToString(string[] segments)
        {
            return (SegmentIndex < segments.Length) ? segments[SegmentIndex] : "N/A";
        }
        public override int CompareTo(object o)
        {
            N0183Field o2 = o as N0183Field;
            if (o2 == null)
            {
                throw new Exception("cant compare N0183Field to " + o.GetType().ToString());
            }
            return SegmentIndex.CompareTo(o2.SegmentIndex);
        }
    }
    public class N0183TextField : N0183Field
    {
    }

    /// <summary>
    /// Base class for all numeric NMEA 0183 fields, all numeric values are returned as doules
    /// </summary>
    public class N0183NumericField : N0183Field
    {
        // Every numeric field has a formatter
        protected FormatEnum _FormatAs;
        [Description("The formatter currently used")]
        [XmlIgnore]
        public Formatter Formatter;
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

        protected virtual FieldValueState GetState(string value)
        {
            if (value == "")
                return FieldValueState.NotAvailable;
            else
            {
                double d;
                if (!double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                    return FieldValueState.Error;
                else
                    return FieldValueState.Valid;
            }
        }
        public virtual double GetValue(string[] segments, out FieldValueState state)
        {
            if (SegmentIndex >= segments.Length)
            {
                state = FieldValueState.NotAvailable;
                return 0.0;
            }

            string raw = segments[SegmentIndex];
            state = GetState(raw);
            return (state != FieldValueState.Valid) ? 0.0 : double.Parse(segments[SegmentIndex], CultureInfo.InvariantCulture);
        }
        public override string ToString(string[] segments)
        {
            if (SegmentIndex >= segments.Length)
                return "N/A";

            FieldValueState state;
            double value = GetValue(segments, out state);
            return Formatter.Format(value, state);
        }

        // Creator
        public N0183NumericField() {
            FormatAs = FormatEnum.Number;   // AJM: Create a default formatter; we don't serialise FormatAs, so it may never be set
        }
    }
    public class N0183LeftRightField : N0183NumericField
    {
        public override double GetValue(string[] segments, out FieldValueState state)
        {
            if (SegmentIndex + 1 >= segments.Length)
            {
                state = FieldValueState.NotAvailable;
                return 0.0;
            }

            double v;
            if (!double.TryParse(segments[SegmentIndex], NumberStyles.Number, CultureInfo.InvariantCulture, out v))
            {
                state = FieldValueState.Error;
                return 0.0;
            }

            string dir = segments[SegmentIndex + 1];

            if (dir == "R") { v = -v; }
            else if (dir != "L")
            {
                // North/south field malformed
                state = FieldValueState.Error;
                return 0.0;
            }

            state = FieldValueState.Valid;
            return v;
        }
    }
    public abstract class N0183Sexagesimal : N0183NumericField
    {
        public override double GetValue(string[] segments, out FieldValueState state)
        {
            if (SegmentIndex >= segments.Length)
            {
                state = FieldValueState.NotAvailable;
                return 0.0;
            }

            string raw = segments[SegmentIndex];

            if ((state = GetState(raw)) != FieldValueState.Valid)
                return 0.0;

            try
            {
                int decIndex = raw.IndexOf(".");
                int degrees = int.Parse(raw.Substring(0, decIndex - 2));
                double minutes = double.Parse(raw.Substring(decIndex - 2), CultureInfo.InvariantCulture);

                return degrees + minutes / 60.0;
            }
            catch
            {
                // Malformed field.
                state = FieldValueState.Error;
                return 0.0;
            }
        }
    }
    public class N0183LatitudeField : N0183Sexagesimal
    {
        public N0183LatitudeField()
        {
            FormatAs = FormatEnum.Latitude;
        }

        public override double GetValue(string[] segments, out FieldValueState state)
        {
            if (SegmentIndex + 1 >= segments.Length)
            {
                state = FieldValueState.NotAvailable;
                return 0.0;
            }

            double lat = base.GetValue(segments, out state);
            if (state != FieldValueState.Valid)
                return 0.0;

            if (segments.Length - 1 < SegmentIndex + 1)
            {
                state = FieldValueState.Error;
                return 0.0;
            }

            string NS = segments[SegmentIndex + 1];

            if (NS == "S") { lat = -lat; }
            else if (NS != "N")
            {
                // North/south field malformed
                state = FieldValueState.Error;
                return 0.0;
            }

            return lat;
        }
    }
    public class N0183LongitudeField : N0183Sexagesimal
    {
        public N0183LongitudeField()
        {
            FormatAs = FormatEnum.Longitude;
        }

        public override double GetValue(string[] segments, out FieldValueState state)
        {
            if (SegmentIndex + 1 >= segments.Length)
            {
                state = FieldValueState.NotAvailable;
                return 0.0;
            }

            double lat = base.GetValue(segments, out state);
            if (state != FieldValueState.Valid)
                return 0.0;

            if (segments.Length - 1 < SegmentIndex + 1)
            {
                state = FieldValueState.Error;
                return 0.0;
            }

            string NS = segments[SegmentIndex + 1];

            if (NS == "W") { lat = -lat; }
            else if (NS != "E")
            {
                // West/East field malformed
                state = FieldValueState.Error;
                return 0.0;
            }

            return lat;
        }
    }
    public class N0183TimeField : N0183Field
    {
        public override string ToString(string[] segments)
        {
            if (SegmentIndex >= segments.Length)
                return "N/A";

            string raw = segments[SegmentIndex];
            DateTime UTCTime = DateTime.ParseExact(raw, "HHmmss", null);
            return UTCTime.ToLongTimeString();
        }
    }
    public class N0183EnumField : N0183Field
    {
        [XmlIgnore]
        [Browsable(false)]
        private Dictionary<string, string> _enumDict { get; set; }

        // FormatAs does not make sense in regard to an enumfield, so we hide it
        // The easiest way to do that is to redeclare it, applying browsable false attribute.
        // Note: Xml does not like new definition not having a set, so we put one here that does nothing
        // [XmlIgnore]
        // [Browsable(false)]
        // public new FormatEnum FormatAs { get { return FormatEnum.Number; } set { } }

        public N0183EnumField()
        {
            _enumDict = new Dictionary<string, string> { };
        }

        // Array accessors are used to enable easy XML serialization
        [XmlArray("SEnumValues")]
        // [XmlArrayItem("EnumPair", typeof(EnumPair))]
        public SEnumPair[] EnumPairArray
        {
            get
            {
                var Value = new SEnumPair[_enumDict.Count];
                int i = 0;
                foreach (var d in _enumDict)
                {
                    Value[i++] = new SEnumPair(d.Key, d.Value);
                }
                return Value;
            }
            set
            {
                _enumDict = new Dictionary<string, string> { };
                foreach (var d in value)
                {
                    _enumDict[d.Key] = d.Value;
                }
            }
        }

        public override string ToString(string[] segments)
        {
            if (SegmentIndex >= segments.Length)
                return "N/A";

            // Get the value from the bits in d
            // We always return one of the enum values if possible, otherwise we check for error or not available
            string rawValue = segments[SegmentIndex];
//            if (!uint.TryParse(segments[SegmentIndex], out rawValue))
//            {
//                return "No value";
//            }

            string s;
            if (_enumDict.TryGetValue(rawValue, out s))
            {
                return s + "(" + rawValue + ")";
            }
            else
            {
                return "Unrecognised enum value " + rawValue.ToString();
            }
        }
    }

    /// <summary>
    /// Base class for all AIS fields
    /// </summary>
    public abstract class AISField : msgField,INotifyPropertyChanged
    {
        // Static
        public static Type[] AllFieldTypes()
        {
            var TypeList = new List<Type> { };
            foreach (Type t in typeof(AISField).Assembly.GetTypes())
                if (!t.IsAbstract && (t.IsSubclassOf(typeof(AISField)) || t == typeof(AISField)))
                    TypeList.Add(t);
            
            return TypeList.ToArray();
        }
        public static AISField CreateNewField(Type fieldType)
        {
            try
            {
                return (AISField)(Activator.CreateInstance(fieldType));
            }
            catch
            {
                return null;
            }
        }

        // Public
        [XmlAttribute]
        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public override string Description
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
                NotifyPropertyChanged("Description");
            }
        }
        public override int CompareTo(object o) { return -1; } // ToDO
        public int BitOffset
        {
            get
            {
                return _bitOffset;
            }
            set
            {
                _bitOffset = value;
                NotifyPropertyChanged("BitOffset");
            }
        }
        public int BitLength
        {
            get
            {
                return _bitLength;
            }
            set
            {
                _bitLength = value;
                NotifyPropertyChanged("BitLength");
            }
        }

        // Private
        private string _name = "";
        protected string _desc = "";
        protected int _bitOffset;
        protected int _bitLength;

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        // Public methods
        public abstract double GetValue(AISData aisData, out FieldValueState valueState);
        public virtual string ToString(AISData aisData)
        {
            FieldValueState valueState;
            double value = GetValue(aisData, out valueState);
            return (valueState == FieldValueState.Valid) ? value.ToString() :
                   ((valueState == FieldValueState.NotAvailable) ? "Not Available" : "Error");
        }
    }
    public class AISSigned : AISField
    {
        public override double GetValue(AISData aisData, out FieldValueState valueState)
        {
            if (BitOffset + BitLength > 8 * aisData.AISBytes.Length)
            {
                valueState = FieldValueState.NotAvailable;
                return 0.0;
            }
            else
            {
                valueState = FieldValueState.Valid;
                return AisEncoding.GetSigned(aisData.AISBytes, BitOffset, BitLength);
            }
        }
    }
    public class AISUnsigned : AISField
    {
        public override double GetValue(AISData aisData, out FieldValueState valueState)
        {
            if (BitOffset + BitLength > 8 * aisData.AISBytes.Length)
            {
                valueState = FieldValueState.NotAvailable;
                return 0.0;
            }
            else
            {
                valueState = FieldValueState.Valid;
                return AisEncoding.GetUnsigned(aisData.AISBytes, BitOffset, BitLength);
            }
        }
    }
    public class AISEnum : AISUnsigned
    {
        [XmlIgnore]
        [Browsable(false)]
        private Dictionary<uint, string> _enumDict { get; set; }

        // FormatAs does not make sense in regard to an enumfield, so we hide it
        // The easiest way to do that is to redeclare it, applying browsable false attribute.
        // Note: Xml does not like new definition not having a set, so we put one here that does nothing
        // [XmlIgnore]
        // [Browsable(false)]
        // public new FormatEnum FormatAs { get { return FormatEnum.Number; } set { } }

        public AISEnum()
        {
            _enumDict = new Dictionary<uint, string> { };
        }

        // Array accessors are used to enable easy XML serialization
        [XmlArray("EnumValues")]
        // [XmlArrayItem("EnumPair", typeof(EnumPair))]
        public EnumPair[] EnumPairArray
        {
            get
            {
                var Value = new EnumPair[_enumDict.Count];
                int i = 0;
                foreach (var d in _enumDict)
                {
                    Value[i++] = new EnumPair(d.Key, d.Value);
                }
                return Value;
            }
            set
            {
                _enumDict = new Dictionary<uint, string> { };
                foreach (var d in value)
                {
                    _enumDict[d.Key] = d.Value;
                }
            }
        }

        public override string ToString(AISData aisData)
        {
            // Get the value from the bits in d
            // We always return one of the enum values if possible, otherwise we check for error or not available
            uint rawValue = (uint)AisEncoding.GetUnsigned(aisData.AISBytes, BitOffset, BitLength);
            string s;
            if (_enumDict.TryGetValue(rawValue, out s))
            {
                return s + "(" + rawValue + ")";
            }
            else
            {
                return "Unrecognised enum value " + rawValue.ToString();
            }
        }
    }
    public class AISSignedDouble : AISField
    {
        public double Scale { get; set; }
        public double Offset { get; set; }

        public override double GetValue(AISData aisData, out FieldValueState valueState)
        {
            if (BitOffset + BitLength > 8 * aisData.AISBytes.Length)
            {
                valueState = FieldValueState.NotAvailable;
                return 0.0;
            }
            else
            {
                valueState = FieldValueState.Valid;
                return AisEncoding.GetSigned(aisData.AISBytes, BitOffset, BitLength) * Scale + Offset;
            }
        }
    }
    public class AISUnsignedDouble : AISField
    {
        public double Scale { get; set; }
        public double Offset { get; set; }

        public override double GetValue(AISData aisData, out FieldValueState valueState)
        {
            if (BitOffset + BitLength > 8 * aisData.AISBytes.Length)
            {
                valueState = FieldValueState.NotAvailable;
                return 0.0;
            }
            else
            {
                valueState = FieldValueState.Valid;
                return AisEncoding.GetUnsigned(aisData.AISBytes, BitOffset, BitLength) * Scale + Offset;
            }
        }
    }
    public class AISString : AISField
    {
        public override double GetValue(AISData aisData, out FieldValueState valueState)
        {
            valueState = FieldValueState.Error;
            return 0.0;
        }
        public override string ToString(AISData aisData)
        {
            if (BitOffset + BitLength > 8 * aisData.AISBytes.Length)
            {
                return "N/A";
            }
            else
            {
                return AisEncoding.GetString(aisData.AISBytes, BitOffset, BitLength);
            }
        }
    }
    public class AISRoT : AISField
    {
        public override double GetValue(AISData aisData, out FieldValueState valueState)
        {
            valueState = FieldValueState.Valid;
            long value = AisEncoding.GetSigned(aisData.AISBytes, BitOffset, BitLength);
            return (value * Math.Abs(value)) / (4.733 * 4.733);
        }
        public override string ToString(AISData aisData)
        {
            long value = AisEncoding.GetSigned(aisData.AISBytes, BitOffset, BitLength);
            switch (value)
            {
                case -128: return "Data not available";
                case -127: return "Turning left at >5deg/30s (No TI available)";
                case 127: return "Turning right at >5deg/30s (No TI available)";
                default:
                    return ((value * Math.Abs(value)) / (4.733 * 4.733)).ToString();
            }
        }
    }

    /// <summary>
    /// Provides methods for encoding byte arrays to AIS format, and the reverse.
    /// </summary>
    public static class AisEncoding
    {
        private static byte[] asciiToSixbit; // For decoding an ascii byte into the corresponding 6-bit value; aisByteToSixbit[ascii] = sixbit
        private static byte[] sixbitToAscii; // For encoding a 6-bit value into the corresponding ascii byte; sixbitToAisByte[sixbit] = ascii
        private static char[] sixbitToChar;  // For decoding a 6-bit value into the corresponding character; sixbitToChar[sixbit] = character
        
        static AisEncoding()
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

            // How many bytes to shift foward by
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

    /// <summary>
    /// Provides methods for packing an NMEA 2000 message into an NMEA 0183 message using AIS encoding of the bytes, and the reverse conversion.
    /// </summary>
    public static class FrameConversion
    {
        public const string N2kTypeCode = "$NTWOK";

        public static N0183Frame PackN2k(N2kFrame n2kFrame)
        {
            // Step 1 : Pack N2k frame into a byte array
            byte[] n2kBytes = new byte[4 + n2kFrame.Data.Length];
            n2kBytes[0] = n2kFrame.Header.Byte0;
            n2kBytes[1] = n2kFrame.Header.Byte1;
            n2kBytes[2] = n2kFrame.Header.Byte2;
            n2kBytes[3] = n2kFrame.Header.Byte3;
            Array.Copy(n2kFrame.Data, 0, n2kBytes, 4, n2kFrame.Data.Length);

            // Step 2 : Form N0183 string
            string message = N2kTypeCode + "," + AisEncoding.EncodeBytes(n2kBytes, 0, n2kBytes.Length);
            
            // Step 3 : Add checksum
            byte checksum = 0;
            for (int i = 1; i < message.Length; i++)
                checksum ^= Convert.ToByte(message[i]);
            message += "*" + checksum.ToString("X2");

            // Step 4 : Return new N0183 frame
            return new N0183Frame(message, DateTime.Now);
        }
        public static N2kFrame UnpackN2k(N0183Frame n0183Frame)
        {
            // Step 1 : Get N2k bytes
            byte[] n2kBytes = AisEncoding.GetBytes(n0183Frame.Segments[0]);
            byte[] n2kData = new byte[n2kBytes.Length - 4];
            Array.Copy(n2kBytes, 4, n2kData, 0, n2kData.Length);

            // Step 2 : Return new N2k frame
            N2kHeader n2kHeader = new N2kHeader(n2kBytes[0], n2kBytes[1], n2kBytes[2], n2kBytes[3]);
            return new N2kFrame(n2kHeader, n2kData, DateTime.Now);
        }

    }

    /// <summary>
    /// Formatters. Classes that define conversion from the raw double value to a string.
    /// Some of these classes provide interpretation (e.g. SID), and others just decoration (e.g. speed)
    /// </summary>
    public enum FormatEnum
    {
        Number,
        Latitude,
        Longitude,
        LatitudeDDMM,
        LongitudeDDDMM,
        Length,
        Speed,
        Heading,
        Temperature,
        Time,
        Date,
        SID,
        Custom
    }
    public abstract class Formatter
    {
        public static Formatter Create(FormatEnum formatType)
        {
            switch (formatType)
            {
                case FormatEnum.Number: return new NumberFormat();
                case FormatEnum.Latitude: return new LatitudeFormat();
                case FormatEnum.Longitude: return new LongitudeFormat();
                case FormatEnum.LatitudeDDMM: return new LatitudeDDMMFormat();
                case FormatEnum.LongitudeDDDMM: return new LongitudeDDDMMFormat();
                case FormatEnum.Length: return new LengthFormat();
                case FormatEnum.Speed: return new SpeedFormat();
                case FormatEnum.Heading: return new HeadingFormat();
                case FormatEnum.Temperature: return new TemperatureFormat();
                case FormatEnum.Date: return new DateFormat();
                case FormatEnum.Time: return new TimeFormat();
                case FormatEnum.SID: return new SIDFormat();
                default: return new NumberFormat();
            }
        }

        public static string DDtoDMS(double coordinate, string ValueFormat)
        {
            string[] ValueFormats = ValueFormat.Split(';');

            // For 0, simply return 0 format, if it exist.
            if ((coordinate==0) && (ValueFormats.Length>=3)) return ValueFormats[2];

            // Choose format to be used. 
            string UseFormat = ((coordinate < 0d) && (ValueFormats.Length >= 2) ? ValueFormats[1] : ValueFormats[0]);

            int DDPos = UseFormat.IndexOf("°");
            // If there is no degrees, use simple format
            if (DDPos<=0) return coordinate.ToString("#0.000°");

            // If we have negative format, work with a positive number
            double AbsCoord=(ValueFormats.Length >= 2?Math.Abs(coordinate):coordinate);

            int MMPos = UseFormat.IndexOf("'");
            // If there is no minutes, use UseFormat
            if (MMPos<0) return AbsCoord.ToString(UseFormat);
            // If there is no escape before ', add it
            if ((MMPos > 0) && (UseFormat.Substring(MMPos - 1, 1) != "\\"))
            {
                UseFormat = UseFormat.Insert(MMPos, "\\");
                MMPos++;
            }

            int SSPos = UseFormat.IndexOf("\"");
            // If there is no escape before ", add it
            if ((SSPos > 0) && (UseFormat.Substring(SSPos - 1, 1) != "\\"))
            {
                UseFormat = UseFormat.Insert(SSPos, "\\");
                SSPos++;
            }

            // MMPos<DDPos - stupid order, use simple format
            if ((MMPos<DDPos) || (SSPos>0) && (SSPos<MMPos)) return coordinate.ToString("#0.000°");

            string DDFormat=UseFormat.Substring(0,DDPos+1);
            string MMFormat = UseFormat.Substring(DDPos + 1);

            double d=Math.Truncate(AbsCoord);
            double m=Math.Abs((AbsCoord - d) * 60);

            if (SSPos<0) return d.ToString(DDFormat)+m.ToString(MMFormat);

            MMFormat = UseFormat.Substring(DDPos + 1, MMPos-DDPos);
            string SSFormat=UseFormat.Substring(MMPos+1);

            double im = Math.Floor(m);
            double s=(m-im)*60;
            return d.ToString(DDFormat) + m.ToString(MMFormat) + s.ToString(SSFormat);
        }

        public string FormatString { get; set; }

        protected virtual string Format(double v)
        {
            return v.ToString(FormatString);
        }
        public virtual string Format(double v, FieldValueState state)
        {
            return (state == FieldValueState.Error) ? "Error" :
                    (state == FieldValueState.NotAvailable) ? " NotAvailable" :
                    Format(v);
        }
        public virtual string Format(double v, Parameter.ParameterStateEnum state)
        {
            return (state == Parameter.ParameterStateEnum.IsError) ? "Error" :
                (state == Parameter.ParameterStateEnum.IsNotAvailable) ? "NotAvailable" :
                (state == Parameter.ParameterStateEnum.NoDataReceived) ? "No Data Received" :
                Format(v);
        }
        public virtual string FormatToN0183(double v, Parameter.ParameterStateEnum state)
        {
            if (state == Parameter.ParameterStateEnum.ValidValueReceived)
                return v.ToString();
            
            return "";
        }
    }
    public class NumberFormat : Formatter
    {
        public NumberFormat()
        {
            FormatString = "F";
        }
    }
    public class LatitudeFormat : Formatter
    {
        public LatitudeFormat()
        {
            FormatString = "#0.00 °N;#0.00 °S; '0' °N";
        }

        public override string FormatToN0183(double v, Parameter.ParameterStateEnum state)
        {
            if (state == Parameter.ParameterStateEnum.ValidValueReceived)
            {
                int degrees = (int)v;
                double minutes = (v * 60) % 60;

                string retStr = degrees.ToString() + minutes.ToString("00.0##");
                if (v >= 0)
                    return retStr + ",N";
                else
                    return retStr + ",S";
            }

            return ",";
        }
    }
    public class LongitudeFormat : Formatter
    {
        public LongitudeFormat()
        {
            FormatString = "#0.00 °E;#0.00 °W; '0' °E";
        }
    }
    public class LatitudeDDMMFormat : LatitudeFormat
    {
        public LatitudeDDMMFormat()
        {
            FormatString = "#00° 00.000' N;#00° 00.000' S;00° 00.000' N";
        }

        protected override string Format(double v)
        {
            return DDtoDMS(v, FormatString);
        }
    }
    public class LongitudeDDDMMFormat : LongitudeFormat
    {
        public LongitudeDDDMMFormat()
        {
            FormatString = "#000° 00.000' E;#000° 00.000' W;000° 00.000' E";
        }
        protected override string Format(double v)
        {
            return DDtoDMS(v, FormatString);
        }
    }
    public class LengthFormat : Formatter
    {
        public LengthFormat()
        {
            FormatString = "#0.00 m";
        }
    }
    public class SpeedFormat : Formatter
    {
        public SpeedFormat()
        {
            FormatString = "#0.00 m/s";
        }
    }
    public class HeadingFormat : Formatter
    {
        public HeadingFormat()
        {
            FormatString = "#0.00°";
        }
    }
    public class TemperatureFormat : Formatter
    {
        public TemperatureFormat()
        {
            FormatString = "#0.00 °C";
        }
    }
    public class TimeFormat : Formatter
    {
        public TimeFormat()
        {
            FormatString = "HH:mm:ss.fff";
        }

        protected override string Format(double v)
        {
            // v = Seconds since midnight, range allows for leap seconds, resolution 100 microseconds
            // Year/month/day don't matter.
            DateTime baseTime = new DateTime(1, 1, 1, 0, 0, 0);
            DateTime adjTime = baseTime.AddSeconds(v);
            return adjTime.ToString(FormatString);
        }
    }
    public class DateFormat : Formatter
    {
        public DateFormat()
        {
            FormatString = "yyyy-MM-dd";
        }

        protected override string Format(double v)
        {
            DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime adjTime = baseTime.AddDays(v);
            return adjTime.ToString(FormatString);
        }
    }
    public class SIDFormat : Formatter
    {
        public SIDFormat()
        {
            FormatString = "N";
        }

        public override string Format(double v, FieldValueState state)
        {
            return (state == FieldValueState.Error) ? "Error" :
                    (state == FieldValueState.NotAvailable) ? "No linked PGNs" :
                    v.ToString();
        }
    }

    public enum FieldValueState
    {
        NotAvailable,   // The field is not available, or no data has been provided
        Error,          // An error occured while translating the field
        Valid           // The field contains valid data
    }

    /// <summary>
    /// Base class for all frame headers
    /// </summary>
    public abstract class FrameHeader : IComparable
    {
        public const int GlobalDestination = 255;

        public abstract string Identifier { get; }
        public abstract string Source { get; }
        public abstract string Destination { get; }
        public abstract string Priority { get; }

        public FrameHeader() { }
        public abstract bool Equals(FrameHeader p);
        public override abstract string ToString();
        public abstract int CompareTo(object p);
    }

    /// <summary>
    /// Represents a header of an NMEA 2000 message, specifying the PGN, priority, destination, and source.
    /// </summary>
    public class N2kHeader : FrameHeader
    {
        public byte Byte0 { get; set; }
        public byte Byte1 { get; set; }
        public byte Byte2 { get; set; }
        public byte Byte3 { get; set; }

        public int PGN
        {
            get
            {
                return ((int)(Byte0 & 3) << 16)
                     + ((int)Byte1 << 8)
                     + ((int)(Byte1 > 239 ? Byte2 : 0));
            }
            private set
            {
                byte b0 = (byte)((value >> 16) & 3);
                byte b1 = (byte)((value >> 8) & 255);
                byte b2 = (byte)(value & 255);
                Byte0 = (byte)((Byte0 & (byte)252) | b0);
                Byte1 = b1;
                if (b1 > 239)
                {
                    Byte2 = b2;
                }
                else if (b2 != 0)
                {
                    throw new Exception("Illegal PGN code");
                }
            }
        }
        public byte PGNPriority
        {
            get { return (byte)((Byte0 >> 2) & 7); }
            private set { Byte0 = (byte)((Byte0 & 227) | ((value & 7) << 2)); }
        }
        public byte PGNDestination
        {
            get { return (Byte1 < 240) ? Byte2 : (byte)255; }   // 255 means global destination
            private set
            {
                if (Byte1 > 239)
                {
                    // Only valid destination is 255, which is implied anyway and not set in the Header
                    if (value != 255) throw new Exception("Cannot set non-global Destination in J1939 Header for this PGN");
                }
                else
                {
                    Byte2 = (byte)value;
                }
            }
        }
        public byte PGNSource
        {
            get { return Byte3; }
            private set { Byte3 = (byte)value; }
        }

        public override string Identifier { get { return PGN.ToString().PadLeft(6, '0'); } }
        public override string Source { get { return PGNSource.ToString().PadLeft(3); } }
        public override string Destination { get { return (PGNDestination == FrameHeader.GlobalDestination) ? "All" : PGNDestination.ToString().PadLeft(3); } }
        public override string Priority { get { return PGNPriority.ToString(); } }

        public N2kHeader(byte priority, int pgn, byte destination, byte source)
        {
            // This creator ensures the PGN is set before the Destination 
            PGNPriority = priority;
            PGN = pgn;
            PGNDestination = destination;
            PGNSource = source;
        }
        public N2kHeader(byte b0, byte b1, byte b2, byte b3) {
            // This creator ensures the PGN is set before the Destination 
            Byte0 = b0;
            Byte1 = b1;
            Byte2 = b2;
            Byte3 = b3;
        }

        public uint AsUInt
        {
            // Return the header in the form of a uint32
            get
            {
                return ((uint)Byte0 << 24) + ((uint)Byte1 << 16) + ((uint)Byte2 << 8) + (uint)Byte3;
            }
        }
        public override bool Equals(FrameHeader p)
        {
            // If parameter is null return false:
            N2kHeader p2 = p as N2kHeader;
            if (p2 == null)
            {
                return false;
            }
            return p2.AsUInt == AsUInt;
        }
        public override string ToString()
        {
            return "PGN=" + PGN + ", Source=" + PGNSource + ", Dest=" + PGNDestination + ", Priority=" + PGNPriority;
        }
        public override int CompareTo(object p)
        {
            N2kHeader p2 = p as N2kHeader;
            if (p2 == null)
            {
                return 1; // N2k comes after N0183
            }
            return AsUInt.CompareTo(p2.AsUInt);
        }
    }

    /// <summary>
    /// Represents a header of an NMEA 0183 message, specifying the talker ID and typecode
    /// </summary>
    public class N0183Header : FrameHeader
    {
        // Public properties
        public string HeaderText { get { return _headerText; } }
        public string TalkerID { get { return _talkerID; } }
        public string TypeCode { get { return _typeCode; } }

        public override string Identifier { get { return TypeCode; } }
        public override string Source { get { return TalkerID; } }
        public override string Destination { get { return "All"; } }
        public override string Priority { get { return "-"; } }

        // Private vars
        private readonly string _headerText;
        private readonly string _startCharacter;
        private readonly string _talkerID;
        private readonly string _typeCode;
        
        // Constructors
        public N0183Header(string headerText)
        {
            _headerText = headerText;
            _startCharacter = headerText.Substring(0, 1);
            _talkerID = headerText.Substring(1, 2);
            _typeCode = headerText.Substring(3, 3);
        }
        public N0183Header(string startChar, string talkerID, string typeCode)
        {
            _startCharacter = startChar;
            _talkerID = talkerID;
            _typeCode = typeCode;
            _headerText = startChar + talkerID + typeCode;
        }

        // Public methods
        public override bool Equals(FrameHeader p)
        {
            N0183Header p2 = p as N0183Header;
            if (p2 == null)
            {
                return false;
            }
            return HeaderText == p2.HeaderText;
        }
        public override string ToString()
        {
            return "TalkerID=" + TalkerID + ", Code=" + TypeCode;
        }
        public override int CompareTo(object p)
        {
            N0183Header p2 = p as N0183Header;
            if (p2 == null)
            {
                return -1; // N2k comes after N0183
            }
            return HeaderText.CompareTo(p2.HeaderText);
        }
    }

    /// <summary>
    /// Represents a piece data available in both byte[] and AIS format.
    /// </summary>
    public class AISData
    {
        public readonly string AISString;
        public readonly byte[] AISBytes;

        public AISData(string aisString)
        {
            AISString = aisString;
            AISBytes = AisEncoding.GetBytes(AISString);
        }
        public AISData(byte[] aisBytes)
        {
            AISString = AisEncoding.GetString(aisBytes, 0, aisBytes.Length);
            AISBytes = aisBytes;
        }
    }

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

    /// <summary>
    /// This class is the central unit of the application, it represents a fully-assembled message with header, it may or may not be decoded
    /// </summary>
    public abstract class Frame
    {
        // We serialize either a millisecond or a datetime.
        // SwitchToMilliSecondsTimeStamp is arbitrarily chosen as the breakpoint between timestamp-came-from-milliseconds and timestamp-is-actual.
        static DateTime SwitchToMilliSecondsTimeStamp = new DateTime(1, 12, 1);//new DateTime().AddMilliseconds(int.MaxValue);

        // Public properties
        [XmlIgnore]
        public int ArrivalID { get; set; }
        [XmlIgnore]
        public string OriginName { get; set; }
        [XmlAttribute(AttributeName = "ms")]
        public long ms
        {
            get
            {
                return (TimeStamp.Ticks / TimeSpan.TicksPerMillisecond); // (uint)(TimeStamp.TimeOfDay.TotalMilliseconds + 0.5); 
            }
            set
            {
                TimeStamp = new DateTime().AddMilliseconds(value); // Set the date/time to the milliseconds
            }
        }
        public bool ShouldSerializems() { return !ShouldSerializeTimeStamp(); }
        [XmlAttribute(DataType = "dateTime", AttributeName = "TimeStamp")]
        public DateTime TimeStamp { get; set; }
        public bool ShouldSerializeTimeStamp()
        {
            return TimeStamp > SwitchToMilliSecondsTimeStamp;
        }
        public string TimeStampString
        {
            get
            {
                if (TimeStamp > SwitchToMilliSecondsTimeStamp)
                {
                    return TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.ffff", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    return ms.ToString() + " ms";
                }
            }
        }

        [XmlIgnore]
        public FrameHeader Header { get; set; }

        [XmlIgnore]
        public abstract int Length { get; }

        [XmlIgnore]
        public Defn Defn { get; set; }

        public abstract double GetDouble(msgField field, out FieldValueState valueState);
        public abstract string GetString(msgField field, out FieldValueState valueState);
        public abstract bool IsSameValue(msgField field, string cmpTo);
        public abstract double GetDouble(string fieldName, out FieldValueState valueState);
        public abstract string GetString(string fieldName, out FieldValueState valueState);
        public abstract bool IsSameValue(string fieldName, string cmpTo);
        public abstract msgField GetField(string fieldName);
        // public abstract string GetString(int fieldIndex, out FieldValueState valueState);
    }
    
    /// <summary>
    /// Represents an NMEA 2000 message
    /// </summary>
    [XmlRoot("N2kMsg")]
    public class N2kFrame : Frame
    {
        // Builder
        public class Builder
        {
            private Dictionary<uint, FastPacketMessage> _fastPackets = new Dictionary<uint, FastPacketMessage>(); // We store a list of FastPacketMessage's, keyed by the header data (as a uint32)

            public N2kFrame AddFrame(N2kFrame n2kMsg)
            {
                N2kHeader n2kHeader = n2kMsg.Header;

                FastPacketMessage FastPacket;
                if (_fastPackets.TryGetValue(n2kHeader.AsUInt, out FastPacket))
                {
                    // This frame is the next in some existing sequence, so add it to the sequence.
                    try
                    {
                        if (FastPacket.AddNextFrame(n2kMsg))
                        {
                            // It was the last frame in the sequence, so the message is complete, and no further processing is needed. 
                            // Return the processed message as a decoded message
                            _fastPackets.Remove(n2kHeader.AsUInt);

                            FastPacket.Defn = FastPacket.pgnInfo;
                            //return (N2kFrame)FastPacket;
                            return new N2kFrame(FastPacket.Header, FastPacket.Data, FastPacket.TimeStamp) { Defn = FastPacket.Defn };
                        }
                        else
                        {
                            // More frames are needed, so cannot return any decoded message yet
                            return null;
                        }
                    }
                    catch
                    {
                        // There has been an error. Stop processing the incoming frames
                        _fastPackets.Remove(n2kHeader.AsUInt);
                        throw;
                    }
                }
                // Get the PGN that provides info about this frame.
                PGNDefn pgnInfo = Definitions.PGNDefnCol.GetPGNDefn(n2kMsg);
                if ((n2kMsg.Data.Length > 8) || (pgnInfo.ByteLength <= 8))
                {
                    if (pgnInfo.ByteLength > n2kMsg.Data.Length)
                        ReportHandler.LogWarning(string.Format("PGN {0} length ({1}) exceeds data provided ({2}); some fields won't show.", pgnInfo, pgnInfo.ByteLength, n2kMsg.Data.Length));

                    // This is a single frame message. Return it with the associated PGN
                    n2kMsg.Defn = pgnInfo;
                    return n2kMsg; // new N2kFrame(N2kMsg, pgnInfo);
                }
                else
                {
                    // This is the first in a sequence of fast packet frames. Add it for further processing, and return nothing

                    // First frame must start with 00, 02, 04, ...
                    // If it doesn't, we ignore the packet (We have missed the start of a multipacket message (Started recieving too late), => impossible to form it)
                    if ((n2kMsg.Data[0] & 0x1F) == 0)
                    {
                        FastPacket = new FastPacketMessage(n2kMsg, pgnInfo);
                        _fastPackets.Add(n2kHeader.AsUInt, FastPacket);

                        if (pgnInfo.ByteLength > FastPacket.ExpectedByteCount)
                            ReportHandler.LogWarning(string.Format("PGN {0} length ({1}) exceeds data provided ({2}); some fields won't show.", pgnInfo.PGN, pgnInfo.ByteLength, FastPacket.ExpectedByteCount));
                    }

                    return null;
                }
            }
            public void Reset()
            {
                _fastPackets.Clear();
            }
        }

        // Constants
        private const byte Escape = 0x10;
        private const byte StartOfText = 0x02;
        private const byte EndOfText = 0x03;
        private const byte N2kDataReceived = 0x93;

        public static XmlSerializer XmlSerializer = new XmlSerializer(typeof(N2kFrame));

        [XmlIgnore]
        public new N2kHeader Header { get { return (N2kHeader)base.Header; } set { base.Header = value; } }
        [XmlIgnore]
        public new PGNDefn Defn { get { return (PGNDefn)base.Defn; } set { base.Defn = value; } }

        [XmlText]
        public string AsHex
        {
            get
            {
                var x = new System.Text.StringBuilder();
                for (int i = 0; i < Bytes.Length; i++)
                    x.Append(Bytes[i].ToString("X2"));

                return x.ToString();
            }
            set
            {
                var x = new List<byte> { };
                for (int i = 0; i < value.Length; i += 2)
                    x.Add(Convert.ToByte(value.Substring(i, 2), 16));

                Bytes = x.ToArray();
            }
        }

        [XmlIgnore]
        public byte[] Data { get; set; } // Contains the data, minus header bytes
        
        [XmlIgnore] // [XmlText(DataType = "hexBinary")]
        public byte[] Bytes {
            get {
                var b = new byte[Data.Length + 4];
                N2kHeader header = (N2kHeader)Header;
                b[0] = header.Byte0;
                b[1] = header.Byte1;
                b[2] = header.Byte2;
                b[3] = header.Byte3;
                for (int i = 0; i < Data.Length; i++) {
                    b[i + 4] = Data[i];
                }
                return b;
            }
            set {
                Header = new N2kHeader(value[0], value[1], value[2], value[3]);
                Data = new byte[value.Length - 4];
                for (int i = 4; i < value.Length; i++) {
                    Data[i-4] = value[i];
                }
            }
        }

        [XmlIgnore]
        public override int Length { get { return Bytes.Length; } }

        // Constructors
        private N2kFrame()
        {
            // For XML deserializer
        }
        public N2kFrame(N2kHeader h, byte[] data, DateTime timeStamp)
        {
            this.Header = h;
            this.Data = data;
            this.TimeStamp = timeStamp;
        }
        
        // Public methods
        public override string ToString()
        {
            return System.BitConverter.ToString(Data);
        }
        public override double GetDouble(msgField field, out FieldValueState valueState)
        {
            N2kNumericField n2kField = field as N2kNumericField;
            if (n2kField == null)
            {
                valueState = FieldValueState.Error;
                return 0.0;
            }

            return n2kField.GetValue(Data, out valueState);
        }
        public override string GetString(msgField field, out FieldValueState valueState)
        {
            N2kField NamedField = field as N2kField;
            if (NamedField == null)
            {
                valueState = FieldValueState.Error;
                return "N/A";
            }

            valueState = FieldValueState.Valid;
            return NamedField.ToString(Data);
        }
        public override bool IsSameValue(msgField field, string cmpTo)
        {
            N2kField NamedField = field as N2kField; ;
            if (NamedField == null) return false;
            N2kNumericField NumField = NamedField as N2kNumericField;
            if (NumField != null)
            {
                try
                {
                    FieldValueState valueState;
                    double NumVal = Convert.ToDouble(cmpTo);
                    return (NumVal == NumField.GetValue(Data, out valueState));
                }
                catch
                {
                }
            }

            return (NamedField.ToString(Data) == cmpTo);
        }
        public override double GetDouble(string fieldName, out FieldValueState valueState)
        {
            return GetDouble(Defn.GetFieldByName(fieldName), out valueState);
        }
        public override string GetString(string fieldName, out FieldValueState valueState)
        {
            return GetString(Defn.GetFieldByName(fieldName), out valueState);
        }

        // This is used by hook filter. It should be improved so that we provide the filter 
        // class, which could be checked without every time convert.
        public override bool IsSameValue(string fieldName, string cmpTo)
        {
            return IsSameValue(Defn.GetFieldByName(fieldName), cmpTo);
        }

        public override msgField GetField(string fieldName)
        {
            return Defn.GetFieldByName(fieldName);
        }

    }

    /// <summary>
    /// Represents an NMEA 0183 message
    /// </summary>
    [XmlRoot("N0183Msg")]
    public class N0183Frame : Frame
    {
        // Static
        public static XmlSerializer XmlSerializer = new XmlSerializer(typeof(N0183Frame));
        public static N0183Frame TryCreate(string fullMessage, DateTime timeStamp)
        {
            try
            {
                N0183Frame newFrame = new N0183Frame(fullMessage, timeStamp);

                if (newFrame.IsCRCOk)
                    return newFrame;

                return null;
            }
            catch
            {
                Console.WriteLine(fullMessage);
                return null;
            }
        }

        // Public properties
        [XmlText] public string FullMessage
        {
            get
            {
                return _fullMessage;
            }
            set
            {
                // 'FullMessage' would be a readonly field, however that prevents XML serialization (Which sets properties AFTER constructor)
                // So instead we put this exception here, to give us the guarantee that it is not changed after construction.
                if (_messageSet)
                    throw new InvalidOperationException("Cannot change NMEA 0183 message after creation");
                _messageSet = true;

                // This code should, and currently does, support being called with or without trailing whitespace/LF/CR characters.

                _fullMessage = value;

                int asterixIndex = value.IndexOf('*'); // Asterix tells us where the data ends and checksum begins
                if (asterixIndex < 6)
                {
                    IsCRCOk = false;
                    ReportHandler.LogError("Malformed message: " + _fullMessage);
                    return;
                }

                this.Header = new N0183Header(value.Substring(0, 6));

                _dataString = value.Substring(7, asterixIndex - 7); // 7 to skip the first comma.
                _dataSegments = _dataString.Split(new char[] { ',' });

                int checksumChars = value.Length - asterixIndex - 1;

                if (checksumChars >= 2)
                    _checksum = Convert.ToByte(value.Substring(asterixIndex + 1, 2), 16);
                else
                    if (checksumChars == 1)
                        _checksum = Convert.ToByte(value.Substring(asterixIndex + 1, 1), 16);
                    else
                    {
                        IsCRCOk = false;
                        ReportHandler.LogError("No checksum on message: " + _fullMessage);
                        return;
                    }

                byte actualChecksum = 0;
                for (int i = 1; i < asterixIndex; i++)
                    actualChecksum ^= Convert.ToByte(_fullMessage[i]);

                IsCRCOk = (_checksum == actualChecksum);
                if (!IsCRCOk)
                {
                    ReportHandler.LogError("Bad checksum on message: " + _fullMessage);
                }
            }
        }
        [XmlIgnore] public bool IsCRCOk { get; set; }
        [XmlIgnore] public new N0183Header Header { get { return (N0183Header)base.Header; } set { base.Header = value; } }
        [XmlIgnore] public new N0183Defn Defn { get { return (N0183Defn)base.Defn; } set { base.Defn = value; } }
        [XmlIgnore] public string DataString { get { return _dataString; } }
        [XmlIgnore] public string[] Segments { get { return _dataSegments; } }
        [XmlIgnore] public override int Length { get { return _dataString.Length; } }

        // Private
        private byte _checksum;
        private string _fullMessage;
        private string _dataString;
        private string[] _dataSegments;
        private bool _messageSet = false;

        // Constructors
        private N0183Frame()
        {
            // For XML serializer (Can access private ctors)
        }
        public N0183Frame(string fullMessage, DateTime timeStamp)
        {
            this.FullMessage = fullMessage;
            this.TimeStamp = timeStamp;
        }
        public N0183Frame(string NMEA0183TypeCode, List<Parameter> Parameters) {
            // Form an NMEA 0183 string from a sequence of paramaters
            StringBuilder message = new StringBuilder(NMEA0183TypeCode);

            foreach (Parameter p in Parameters) {
                message.Append(",");
                if (p != null) message.Append(p.ToString());
            }

            // Add checksum
            byte checksum = 0;
            for (int i = 1; i < message.Length; i++)
                checksum ^= Convert.ToByte(message[i]);
            message.Append("*" + checksum.ToString("X2"));
            
            // Step 4 : Save the new N0183 frame
            this.FullMessage = message.ToString();
            this.TimeStamp = DateTime.Now;
        }

        // Public methods
        public override string ToString()
        {
            return _fullMessage;
        }

        public override double GetDouble(msgField field, out FieldValueState valueState)
        {
            N0183NumericField n0183Field = field as N0183NumericField;
            if (n0183Field == null)
            {
                valueState = FieldValueState.Error;
                return 0.0;
            }

            return n0183Field.GetValue(_dataSegments, out valueState);
        }
        public override string GetString(msgField field, out FieldValueState valueState)
        {
            N0183Field NamedField = field as N0183Field;
            if (NamedField == null)
            {
                valueState = FieldValueState.Error;
                return "N/A";
            }

            valueState = FieldValueState.Valid;
            return NamedField.ToString(_dataSegments);
        }
        public override bool IsSameValue(msgField field, string cmpTo)
        {
            N0183Field NamedField = field as N0183Field;
            if (NamedField == null) return false;
            return (NamedField.SegmentIndex < _dataSegments.Length) ?
                   (_dataSegments[NamedField.SegmentIndex] == cmpTo) :
                   false;
        }
        public override double GetDouble(string fieldName, out FieldValueState valueState)
        {
            return GetDouble(Defn.GetFieldByName(fieldName), out valueState);
        }
        public override string GetString(string fieldName, out FieldValueState valueState)
        {
            return GetString(Defn.GetFieldByName(fieldName), out valueState);
        }
        public override bool IsSameValue(string fieldName, string cmpTo)
        {
            return IsSameValue(Defn.GetFieldByName(fieldName), cmpTo);
        }
        public override msgField GetField(string fieldName)
        {
            return Defn.GetFieldByName(fieldName);
        }
    }

    /// <summary>
    /// Represents an AIS message
    /// </summary>
    [XmlRoot("AISMsg")]
    public class AISFrame : Frame
    {
        /// <summary>
        /// Acts as the factory for AIS frames. Accepts AIVDM/AIVDO NMEA 0183 frames and (may) return the AIS frame
        /// For multipart messages, it will hold onto the fragment until it recieves all fragments, at which point it will return the fully assembled frame.
        /// </summary>
        public class Builder
        {
            private const int FragmentSetTimeout = 30; // Fragment sets are reset if new fragment arrives >timeout /seconds/ since last (This aspect is controlled by the Builder, not the set itself)

            /// <summary>
            /// Holds a given number of fragments. When all fragments are recieved, it will return the completed AIS frame.
            /// </summary>
            private class AISFragmentSet
            {
                // Properties
                public DateTime LastReceivedTime
                {
                    get
                    {
                        return _lastReceivedTime;
                    }
                }

                // Private fields
                private int _fragmentCount;
                private bool[] _haveFragment;
                private string[] _fragmentData;
                private int _fragmentsRecieved;
                private DateTime _lastReceivedTime;

                // Constructor
                public AISFragmentSet(int fragmentCount)
                {
                    _fragmentCount = fragmentCount;
                    _fragmentData = new string[fragmentCount];
                    _haveFragment = new bool[fragmentCount];
                }

                // Public methods
                public AISFrame AddFragment(int fragmentNumber, string fragmentData)
                {
                    if (!_haveFragment[fragmentNumber - 1])
                    {
                        _fragmentData[fragmentNumber - 1] = fragmentData;
                        _haveFragment[fragmentNumber - 1] = true;
                        _fragmentsRecieved++;
                        _lastReceivedTime = DateTime.Now;

                        if (_fragmentsRecieved == _fragmentCount)
                        {
                            // Return completed frame !
                            return new AISFrame(string.Concat(_fragmentData), DateTime.Now);
                        }
                        else
                        {
                            // Frame not yet complete
                            return null;
                        }
                    }
                    else
                    {
                        // We already have this fragment, we simply ignore it
                        return null;
                    }
                }
            }

            // Dictionary mapping sequential message ID to its corresponding fragment set
            private Dictionary<int, AISFragmentSet> _fragmentSets;
            
            // Constructors
            public Builder()
            {
                _fragmentSets = new Dictionary<int, AISFragmentSet> { };
            }

            // Public methods
            public AISFrame AddFrame(N0183Frame n0183Frame)
            {
                // [0] = fragment count
                // [1] = fragment number
                // [2] = sequential message ID (for multipart)
                // [3] = radio channel (A or B)
                // [4] = ais data
                // [5] = number of padding bits

                int fragmentCount = int.Parse(n0183Frame.Segments[0]);
                if (fragmentCount == 1)
                {
                    AISFrame completedFrame = new AISFrame(n0183Frame.Segments[4], DateTime.Now);
                    completedFrame.Defn = Definitions.AISDefnCol.GetAISDefn(completedFrame);
                    return completedFrame;
                }
                else
                {
                    int fragmentNumber = int.Parse(n0183Frame.Segments[1]);
                    int sequenceID = int.Parse(n0183Frame.Segments[2]);
                    AISFragmentSet fragmentSet;

                    // Get corresponding fragment set, making sure it hasn't timed out
                    if (!_fragmentSets.TryGetValue(sequenceID, out fragmentSet) || ((DateTime.Now - fragmentSet.LastReceivedTime).TotalSeconds > FragmentSetTimeout))
                    {
                        fragmentSet = new AISFragmentSet(fragmentCount);
                        _fragmentSets[sequenceID] = fragmentSet;
                    }

                    AISFrame completedFrame = fragmentSet.AddFragment(fragmentNumber, n0183Frame.Segments[4]);
                    if (completedFrame != null)
                    {
                        _fragmentSets.Remove(sequenceID);
                        completedFrame.Defn = Definitions.AISDefnCol.GetAISDefn(completedFrame);
                    }
                    return completedFrame;
                }
            }
            public void Reset()
            {
                _fragmentSets.Clear();
            }
        }

        // Static
        public static XmlSerializer XmlSerializer = new XmlSerializer(typeof(AISFrame));

        // Public properties
        [XmlText]
        public string aisString
        {
            get
            {
                return _AISData.AISString;
            }
            set
            {
                // For xml serializer only !
                _AISData = new AISData(value);
                this.Header = new AISHeader((int)AisEncoding.GetUnsigned(_AISData.AISBytes, 0, 6));
            }
        }
        public override int Length
        {
            get
            {
                return _AISData.AISBytes.Length;
            }
        }
        [XmlIgnore]
        public new AISHeader Header { get { return (AISHeader)base.Header; } set { base.Header = value; } }
        [XmlIgnore]
        public new AISDefn Defn { get { return (AISDefn)base.Defn; } set { base.Defn = value; } }
        [XmlIgnore]
        public AISData AISData
        {
            get
            {
                return _AISData;
            }
            set
            {
                _AISData = value;
            }
        }

        private AISData _AISData;

        // Constructors
        private AISFrame()
        {
            // For XML serializer
        }
        private AISFrame(string aisString, DateTime timeStamp)
        {
            _AISData = new AISData(aisString);
            this.Header = new AISHeader((int)AisEncoding.GetUnsigned(_AISData.AISBytes, 0, 6));
            this.TimeStamp = timeStamp;
        }

        // Public methods
        public override double GetDouble(msgField field, out FieldValueState valueState)
        {
            AISField aisField = field as AISField;
            if (aisField == null)
            {
                valueState = FieldValueState.Error;
                return 0.0;
            }

            return aisField.GetValue(_AISData, out valueState);
        }
        public override string GetString(msgField field, out FieldValueState valueState)
        {
            AISField NamedField = field as AISField;
            if (NamedField == null)
            {
                valueState = FieldValueState.Error;
                return "N/A";
            }

            valueState = FieldValueState.Valid;
            return NamedField.ToString(_AISData);
        }
        public override bool IsSameValue(msgField field, string cmpTo)
        {
            AISField NamedField = field as AISField;
            if (NamedField == null) return false;
            return NamedField.ToString(_AISData) == cmpTo;
        }
        public override double GetDouble(string fieldName, out FieldValueState valueState)
        {
            AISField aisField = Defn.GetFieldByName(fieldName);
            if (aisField == null)
            {
                valueState = FieldValueState.Error;
                return 0.0;
            }

            return aisField.GetValue(_AISData, out valueState);
        }
        public override string GetString(string fieldName, out FieldValueState valueState)
        {
            AISField NamedField = Defn.GetFieldByName(fieldName);
            if (NamedField == null)
            {
                valueState = FieldValueState.Error;
                return "N/A";
            }

            valueState = FieldValueState.Valid;
            return NamedField.ToString(_AISData);
        }
        public override bool IsSameValue(string fieldName, string cmpTo)
        {
            AISField NamedField = Defn.GetFieldByName(fieldName);
            if (NamedField == null) return false;
            return NamedField.ToString(_AISData) == cmpTo;
        }
        public override msgField GetField(string fieldName)
        {
            return Defn.GetFieldByName(fieldName);
        }

    }

    /// <summary>
    /// Represents a partially completed N2kFrame, 
    /// </summary>
    public class FastPacketMessage : N2kFrame
    {
        // TODO : Remove inheritence. Add private N2kFrame member variable. Then move the logic from this class into 'N2kFrame.Builder'

        // This is the class that we use to receive multi-packet PGNs. It is called repeatedly to add successive frames.
        // It handles the same frame being repeated in the data (generating only a warning).
        public int LastFrameIdentifier = -1;
        public int ExpectedByteCount = -1;
        public int ReceivedByteCount = 0;
        public PGNDefn pgnInfo { get; set; }

         // Creator. Call with the first frame of the fast-Packet frame sequence
        public FastPacketMessage(N2kFrame FirstFrame, PGNDefn PgnInfo)
            : base(FirstFrame.Header, FirstFrame.Data, FirstFrame.TimeStamp)
        {
            ReportHandler.LogInfo("First Fast Packet Receieved: " + FirstFrame.Header + " : " + System.BitConverter.ToString(FirstFrame.Data));

            InitFastPacketSequence(FirstFrame);
            pgnInfo = PgnInfo;
        }

        public void InitFastPacketSequence(N2kFrame FirstFrame) {
            // Remember the header from the first frame; should match all the other frames
            Header = FirstFrame.Header;
            // Remember time stamp
            TimeStamp = FirstFrame.TimeStamp;
            // Get the frame identifier and the total number of bytes to read from the first frame in posns 0 and 1 respectively.
            LastFrameIdentifier = FirstFrame.Data[0];

            if ((LastFrameIdentifier & 0x1F) != 0) throw new Exception("First frame of fast packet sequence does not start with identifier 00, 20, 40, ...");
            
            ExpectedByteCount = FirstFrame.Data[1];
            if (ExpectedByteCount < 8) throw new Exception("Fast packet frame encountered with fewer than 8 bytes.");
            // And copy the remaining 6 bytes of data
            Data = new byte[ExpectedByteCount];
            ReceivedByteCount = 0;
            for (int i = 2; i < 8; i++) {
                Data[ReceivedByteCount++] = FirstFrame.Data[i];
            }
        }

        // Add the next received frame to the data. Returns true if the last packet has been received.
        public bool AddNextFrame(N2kFrame Frame)
        {
            ReportHandler.LogInfo("Fast Packet Receieved: " + Frame.Header + " : " + System.BitConverter.ToString(Frame.Data));

            if (!Frame.Header.Equals(Header))
                throw new Exception("Headers don't match in successive fast packet frames.");

            int FrameIdentifier = Frame.Data[0];
            // We specifically handle repeated frames, which we see in Kee's sample data
            if (FrameIdentifier == LastFrameIdentifier)
            {
                // Check that the data matches; note index goes from 1 to 7
                int FrameDiffsCount = 0;
                for (int i = 1; i <= Math.Min(7, ReceivedByteCount); i++)
                {
                    if (Frame.Data[8-i] != Data[ReceivedByteCount - i]) {
                        Data[ReceivedByteCount - i] = Frame.Data[8-i];
                        FrameDiffsCount++;
                    }
                }

                if (FrameDiffsCount == 0)
                    ReportHandler.LogWarning(string.Format("PGN {0} has fast packet frame {1} duplicated. ", ((N2kHeader)Frame.Header).PGN, FrameIdentifier));
                
                if (FrameDiffsCount > 0)
                    ReportHandler.LogWarning(string.Format("PGN {0} has fast packet frame {1} duplicated, but with {2} differences. ", ((N2kHeader)Frame.Header).PGN, FrameIdentifier, FrameDiffsCount));

                return false;
            }

            if ((FrameIdentifier & 0x1F) == 0)
            {
                // This appears to be the first frame in a fast packet sequence. Reset our sequence
                ReportHandler.LogWarning(string.Format("New fast packet sequence started before previous (with {0} bytes received) completed for PGN {1}.\n", ReceivedByteCount, ((N2kHeader)Frame.Header).PGN));
                
                if ((Frame.Data[1] != ExpectedByteCount) && pgnInfo.HasMultipleDefinitions)
                    throw new Exception("Code does not handle unexpected re-starting of fast packet sequence with a different length for a multiple-defn PGN.");
                
                InitFastPacketSequence(Frame);
                return false;
            }
            if (FrameIdentifier != ((LastFrameIdentifier + 1))) {
                throw new Exception("Frame identifiers out of order in successive fast packet frames, perhaps because of missing frames?");
            }
            int BytesToReceive = Math.Min(7, ExpectedByteCount - ReceivedByteCount);
            for (int i = 1; i <= BytesToReceive; i++) {
                Data[ReceivedByteCount++] = Frame.Data[i];
            }
            LastFrameIdentifier++;
            return ReceivedByteCount == ExpectedByteCount;
        }
    }

    /// <summary>
    /// Handles both building multi-part messages and applying definitions.
    /// </summary>
    public class IncomingMessageHandler
    {
        // Private fields
        private AISFrame.Builder _AISBuilder;
        private N2kFrame.Builder _N2kBuilder;

        // Constructors
        public IncomingMessageHandler()
        {
            _AISBuilder = new AISFrame.Builder();
            _N2kBuilder = new N2kFrame.Builder();
        }

        // Public methods
        public void Reset()
        {
            _N2kBuilder.Reset();
            _AISBuilder.Reset();
        }

        /// <summary>
        /// Builds and decodes the given frame, returning the completed frame on retrival of the last frame part.
        /// </summary>
        /// <param name="msg">The raw message</param>
        /// <returns>Fully-built and decoded frame</returns>
        public Frame DecodeMessage(Frame msg)
        {
            // Decoding depends on the type of frame
            if (msg is N2kFrame)
            {
                return _N2kBuilder.AddFrame((N2kFrame)msg);
            }
            else if (msg is N0183Frame)
            {
                N0183Frame n0183msg = (N0183Frame)msg;

                if (n0183msg.Header.HeaderText == "!AIVDO" || n0183msg.Header.HeaderText == "!AIVDM")
                {
                    return _AISBuilder.AddFrame(n0183msg);
                }
                else if (n0183msg.Header.HeaderText == FrameConversion.N2kTypeCode)
                {
                    N2kFrame unpackedFrame = FrameConversion.UnpackN2k(n0183msg);
                    unpackedFrame.Defn = Definitions.PGNDefnCol.GetPGNDefn(unpackedFrame);
                    return unpackedFrame;
                }

                msg.Defn = Definitions.N0183DefnCol.GetN0183Defn((N0183Frame)msg);
                return msg;
            }
            else if (msg is AISFrame)
            {
                msg.Defn = Definitions.AISDefnCol.GetAISDefn((AISFrame)msg);
                return msg;
            }
            else
            {
                // We don't know how to decode this frame
                throw new NotSupportedException("Cannot decode frame of type: '" + msg.GetType().Name + "'");
            }
        }
    }

}
