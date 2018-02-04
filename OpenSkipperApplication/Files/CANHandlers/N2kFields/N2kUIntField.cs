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

using OpenSkipperApplication;
using System;

namespace CANHandler
{
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

        #region Get Value

        public override double GetValue(byte[] d, out FieldValueState state)
        {
            return GetRawValue(d, out state);
        }

        public override double GetRawValue(byte[] d, out FieldValueState state)
        {
            double raw = RawUIntValue(d);
            state = GetState(raw);
            return raw;
        }

        #endregion

        #region SetValue

        public virtual byte[] SetValue(string v, out FieldValueState state)
        {
            state = FieldValueState.NotAvailable;

            int iVal = 0;
            if (int.TryParse(v, out iVal))
            {
                return SetValue(iVal, out state);
            }
            else
            {
                state = FieldValueState.Error;
                return new byte[0];
            }
        }

        public byte[] SetValue(int v, out FieldValueState state)
        {
            state = FieldValueState.NotAvailable;

            if (v == IntNA || v == double.NaN)
            {
                // Convert 255 to FF x number of required bytes
                state = FieldValueState.Valid;
                return FieldConverter.SetNaBytes(BitLength);
            }

            if (GetState(v) == FieldValueState.Valid)
            {
                byte[] chkbytes;
                byte[] bytes = FieldConverter.SetBytes(v, BitLength, ByteOffset, out chkbytes);

                var chk = GetValue(chkbytes, out state);

                if (v == chk && state == FieldValueState.Valid)
                {
                    return bytes;
                }
            }

            state = FieldValueState.Error;
            return new byte[0];
        }

        #endregion

    } // class

} // namespace
