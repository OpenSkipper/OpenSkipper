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
using System.Linq;
using System.Xml.Serialization;

namespace CANHandler
{
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

        public N2kEnumField()
        {
            EnumLabels = new Dictionary<uint, string>();
        }

        // Array accessors are used to enable easy XML serialization
        [XmlArray("EnumValues")]
        public EnumPair[] EnumPairArray
        {
            get
            {
                var Value = new EnumPair[EnumLabels.Count];
                int i = 0;
                foreach (var d in EnumLabels)
                {
                    Value[i++] = new EnumPair(d.Key, d.Value);
                }
                return Value;
            }
            set
            {
                EnumLabels = new Dictionary<uint, string>();
                if (value == null) return; // Kees file has some "lookup tables" without data!
                foreach (var d in value)
                {
                    EnumLabels[d.Key] = d.Value;
                }
            }
        }

        #region Set Value

        public override byte[] SetValue(string v, out FieldValueState state)
        {
            uint input;
            if (uint.TryParse(v, out input))
            {
                return base.SetValue(v, out state);
            }

            input = FindKeyByValue(v);
            return base.SetValue(input.ToString(), out state);
        }

        #endregion

        public uint FindKeyByValue(string value)
        {
            return EnumPairArray.FirstOrDefault(x => x.Value == value).Key;
        }

        public string FindValueByKey(uint value)
        {
            return EnumPairArray.FirstOrDefault(x => x.Key == value).Value;
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

    } // class

} // namespace
