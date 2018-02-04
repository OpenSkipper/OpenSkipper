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

namespace CANHandler
{
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

        #region Get Value

        public override double GetValue(byte[] d, out FieldValueState state)
        {
            return GetRawValue(d, out state);
        }

        public override double GetRawValue(byte[] d, out FieldValueState state)
        {
            double raw = RawIntValue(d);
            state = GetState(raw);
            return raw;
        }

        #endregion

    } // class

} // namespace
