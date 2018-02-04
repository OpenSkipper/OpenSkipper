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

using CANDefinitions;
using System;
using System.Xml.Serialization;

namespace CANHandler
{
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

} // namespace
