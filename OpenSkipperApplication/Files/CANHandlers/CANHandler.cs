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
using System.Globalization;
using System.Xml.Serialization;

namespace CANHandler
{
    public enum FileTypeEnum
    {
        NativeXMLFile = 0,
        Internal = 2
    }

    public enum FieldValueState
    {
        NotAvailable,   // The field is not available, or no data has been provided
        Error,          // An error occurred while translating the field
        Valid           // The field contains valid data
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
        public static string Binary(byte b)
        {
            string s = "";
            for (int i = 0; i < 8; i++)
            {
                if ((b & 0x80) > 0)
                {
                    s = s + '1';
                }
                else
                {
                    s = s + '0';
                }
                b = (byte)(b << 1);
            }
            return s;
        }
        public static string Binary(byte b, int iFirst, int iLength)
        {
            return Binary(b).Substring(8 - iFirst - iLength, iLength);
        }
        public static string BinaryDump(byte[] data, int iFirst, int iLength)
        {
            string s = "";
            byte b = data[iFirst >> 3];
            int iFirstBit = iFirst & 7;
            int iLastBit = (iFirst + iLength - 1) & 7;
            for (int i = 0; i < 8; i++)
            {
                if (((7 - i) < iFirstBit) || ((7 - i) > iLastBit))
                {
                    s = s + '.';
                }
                else if ((b & 0x80) > 0)
                {
                    s = s + '1';
                }
                else
                {
                    s = s + '0';
                }
                b = (byte)(b << 1);
            }
            return s;
        }
        public static string Dump(byte[] b, int iFirst, int iLength)
        {
            string s = "";
            int iFirstByte = iFirst >> 3;
            int iLastByte = ((iFirst + iLength + 7) >> 3) - 1;
            int iFirstByteToOutput = iFirstByte & 0xFFFFF8;
            int iEndByteToOutput = ((iLastByte + 8) & 0xFFFFF8);
            s = String.Format("  {0,3}: ", iFirstByteToOutput);
            for (int i = iFirstByteToOutput; i < iEndByteToOutput; i++)
            {
                if (i > iFirstByteToOutput && (i & 7) == 0)
                {
                    s = s + "\n  ";
                    s = s + String.Format("{0,3}: ", i);
                }
                if (i >= b.Length)
                {
                    s = s + "   ";
                }
                else if ((i < iFirstByte) || (i > iLastByte))
                {
                    s = s + "-- ";
                }
                else
                {
                    s = s + System.BitConverter.ToString(b, i, 1) + " ";
                }
            }
            if (iLength > 8)
            {
                return s + "........";
            }
            else
            {
                return s + BinaryDump(b, iFirst, iLength);
            }
        }
    }

} // namespace
