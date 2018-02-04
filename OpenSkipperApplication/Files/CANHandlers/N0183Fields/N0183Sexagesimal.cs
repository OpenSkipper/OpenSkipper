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

using System.Globalization;

namespace CANHandler
{
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

} // namespace
