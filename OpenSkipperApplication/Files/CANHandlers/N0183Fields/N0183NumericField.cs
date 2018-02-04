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

using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace CANHandler
{
    /// <summary>
    /// Base class for all numeric NMEA 0183 fields, all numeric values are returned as doubles
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
        public N0183NumericField()
        {
            FormatAs = FormatEnum.Number;   // AJM: Create a default formatter; we don't serialise FormatAs, so it may never be set
        }
    }

} // namespace
