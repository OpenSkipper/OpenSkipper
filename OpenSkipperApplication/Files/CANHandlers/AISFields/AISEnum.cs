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
using System.Xml.Serialization;

namespace CANHandler
{
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
            uint rawValue = (uint)AISEncoding.GetUnsigned(aisData.AISBytes, BitOffset, BitLength);
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

} // namespace
