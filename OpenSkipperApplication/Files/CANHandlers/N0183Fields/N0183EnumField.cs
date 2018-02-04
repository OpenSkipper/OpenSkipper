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

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CANHandler
{

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
            //if (!uint.TryParse(segments[SegmentIndex], out rawValue))
            //{
            //    return "No value";
            //}

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
