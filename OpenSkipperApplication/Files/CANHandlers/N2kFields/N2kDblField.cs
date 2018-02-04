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
using System.Linq;

namespace CANHandler
{
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

        #region Get Value

        // Uses same state function as it's base, IntField.
        public override double GetValue(byte[] d, out FieldValueState state)
        {
            return GetRawValue(d, out state) + Offset;
        }

        public override double GetRawValue(byte[] d, out FieldValueState state)
        {
            double raw = RawLongValue(d);
            state = GetState(raw);
            return raw * Scale;
        }

        #endregion

        #region Set Value

        public virtual byte[] SetValue(string v, out FieldValueState state)
        {
            state = FieldValueState.NotAvailable;

            double val = 0;
            if (double.TryParse(v, out val))
            {
                return SetValue(val, out state);
            }

            state = FieldValueState.Error;
            return new byte[0];
        }

        public virtual byte[] SetValue(double v, out FieldValueState state)
        {
            state = FieldValueState.NotAvailable;

            if (v == 255 || v == double.NaN)
            {
                // Convert 255 to FF x number of required bytes
                state = FieldValueState.Valid;
                return FieldConverter.SetNaBytes(BitLength);
            }

            if (GetState(v) == FieldValueState.Valid)
            {
                // Apply scale to input (do not apply Offset)
                int vv = (int)Math.Round(v / Scale, 0);
                byte[] chkbytes;
                byte[] bytes = FieldConverter.SetBytes(vv, BitLength, ByteOffset, out chkbytes);

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
