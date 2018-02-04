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

using Parameters;
using System;

namespace CANHandler
{
    /// <summary>
    /// Formatters. Classes that define conversion from the raw double value to a string.
    /// Some of these classes provide interpretation (e.g. SID), and others just decoration (e.g. speed)
    /// </summary>
    public enum FormatEnum
    {
        Number,
        Latitude,
        Longitude,
        LatitudeDDMM,
        LongitudeDDDMM,
        Length,
        Speed,
        Heading,
        Temperature,
        Time,
        Date,
        SID,
        Custom
    }

    public abstract class Formatter
    {
        public static Formatter Create(FormatEnum formatType)
        {
            switch (formatType)
            {
                case FormatEnum.Number: return new NumberFormat();
                case FormatEnum.Latitude: return new LatitudeFormat();
                case FormatEnum.Longitude: return new LongitudeFormat();
                case FormatEnum.LatitudeDDMM: return new LatitudeDDMMFormat();
                case FormatEnum.LongitudeDDDMM: return new LongitudeDDDMMFormat();
                case FormatEnum.Length: return new LengthFormat();
                case FormatEnum.Speed: return new SpeedFormat();
                case FormatEnum.Heading: return new HeadingFormat();
                case FormatEnum.Temperature: return new TemperatureFormat();
                case FormatEnum.Date: return new DateFormat();
                case FormatEnum.Time: return new TimeFormat();
                case FormatEnum.SID: return new SIDFormat();
                default: return new NumberFormat();
            }
        }

        public static string DDtoDMS(double coordinate, string ValueFormat)
        {
            string[] ValueFormats = ValueFormat.Split(';');

            // For 0, simply return 0 format, if it exist.
            if ((coordinate == 0) && (ValueFormats.Length >= 3)) return ValueFormats[2];

            // Choose format to be used. 
            string UseFormat = ((coordinate < 0d) && (ValueFormats.Length >= 2) ? ValueFormats[1] : ValueFormats[0]);

            int DDPos = UseFormat.IndexOf("°");
            // If there is no degrees, use simple format
            if (DDPos <= 0) return coordinate.ToString("#0.000°");

            // If we have negative format, work with a positive number
            double AbsCoord = (ValueFormats.Length >= 2 ? Math.Abs(coordinate) : coordinate);

            int MMPos = UseFormat.IndexOf("'");
            // If there is no minutes, use UseFormat
            if (MMPos < 0) return AbsCoord.ToString(UseFormat);
            // If there is no escape before ', add it
            if ((MMPos > 0) && (UseFormat.Substring(MMPos - 1, 1) != "\\"))
            {
                UseFormat = UseFormat.Insert(MMPos, "\\");
                MMPos++;
            }

            int SSPos = UseFormat.IndexOf("\"");
            // If there is no escape before ", add it
            if ((SSPos > 0) && (UseFormat.Substring(SSPos - 1, 1) != "\\"))
            {
                UseFormat = UseFormat.Insert(SSPos, "\\");
                SSPos++;
            }

            // MMPos<DDPos - stupid order, use simple format
            if ((MMPos < DDPos) || (SSPos > 0) && (SSPos < MMPos)) return coordinate.ToString("#0.000°");

            string DDFormat = UseFormat.Substring(0, DDPos + 1);
            string MMFormat = UseFormat.Substring(DDPos + 1);

            double d = Math.Truncate(AbsCoord);
            double m = Math.Abs((AbsCoord - d) * 60);

            if (SSPos < 0) return d.ToString(DDFormat) + m.ToString(MMFormat);

            MMFormat = UseFormat.Substring(DDPos + 1, MMPos - DDPos);
            string SSFormat = UseFormat.Substring(MMPos + 1);

            double im = Math.Floor(m);
            double s = (m - im) * 60;
            return d.ToString(DDFormat) + m.ToString(MMFormat) + s.ToString(SSFormat);
        }

        public string FormatString { get; set; }

        protected virtual string Format(double v)
        {
            return v.ToString(FormatString);
        }
        public virtual string Format(double v, FieldValueState state)
        {
            return (state == FieldValueState.Error) ? "Error" :
                    (state == FieldValueState.NotAvailable) ? " NotAvailable" :
                    Format(v);
        }
        public virtual string Format(double v, Parameter.ParameterStateEnum state)
        {
            return (state == Parameter.ParameterStateEnum.IsError) ? "Error" :
                (state == Parameter.ParameterStateEnum.IsNotAvailable) ? "NotAvailable" :
                (state == Parameter.ParameterStateEnum.NoDataReceived) ? "No Data Received" :
                Format(v);
        }
        public virtual string FormatToN0183(double v, Parameter.ParameterStateEnum state)
        {
            if (state == Parameter.ParameterStateEnum.ValidValueReceived)
                return v.ToString();

            return "";
        }
    }

    public class NumberFormat : Formatter
    {
        public NumberFormat()
        {
            FormatString = "F";
        }
    }

    public class LatitudeFormat : Formatter
    {
        public LatitudeFormat()
        {
            FormatString = "#0.00 °N;#0.00 °S; '0' °N";
        }

        public override string FormatToN0183(double v, Parameter.ParameterStateEnum state)
        {
            if (state == Parameter.ParameterStateEnum.ValidValueReceived)
            {
                int degrees = (int)v;
                double minutes = (v * 60) % 60;

                string retStr = degrees.ToString() + minutes.ToString("00.0##");
                if (v >= 0)
                    return retStr + ",N";
                else
                    return retStr + ",S";
            }

            return ",";
        }
    }

    public class LongitudeFormat : Formatter
    {
        public LongitudeFormat()
        {
            FormatString = "#0.00 °E;#0.00 °W; '0' °E";
        }
    }

    public class LatitudeDDMMFormat : LatitudeFormat
    {
        public LatitudeDDMMFormat()
        {
            FormatString = "#00° 00.000' N;#00° 00.000' S;00° 00.000' N";
        }

        protected override string Format(double v)
        {
            return DDtoDMS(v, FormatString);
        }
    }

    public class LongitudeDDDMMFormat : LongitudeFormat
    {
        public LongitudeDDDMMFormat()
        {
            FormatString = "#000° 00.000' E;#000° 00.000' W;000° 00.000' E";
        }
        protected override string Format(double v)
        {
            return DDtoDMS(v, FormatString);
        }
    }

    public class LengthFormat : Formatter
    {
        public LengthFormat()
        {
            FormatString = "#0.00 m";
        }
    }

    public class SpeedFormat : Formatter
    {
        public SpeedFormat()
        {
            FormatString = "#0.00 m/s";
        }
    }

    public class HeadingFormat : Formatter
    {
        public HeadingFormat()
        {
            FormatString = "#0.00°";
        }
    }

    public class TemperatureFormat : Formatter
    {
        public TemperatureFormat()
        {
            FormatString = "#0.00 °C";
        }
    }

    public class TimeFormat : Formatter
    {
        public TimeFormat()
        {
            FormatString = "HH:mm:ss.fff";
        }

        protected override string Format(double v)
        {
            // v = Seconds since midnight, range allows for leap seconds, resolution 100 microseconds
            // Year/month/day don't matter.
            DateTime baseTime = new DateTime(1, 1, 1, 0, 0, 0);
            DateTime adjTime = baseTime.AddSeconds(v);
            return adjTime.ToString(FormatString);
        }
    }

    public class DateFormat : Formatter
    {
        public DateFormat()
        {
            FormatString = "yyyy-MM-dd";
        }

        protected override string Format(double v)
        {
            DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime adjTime = baseTime.AddDays(v);
            return adjTime.ToString(FormatString);
        }
    }

    public class SIDFormat : Formatter
    {
        public SIDFormat()
        {
            FormatString = "N";
        }

        public override string Format(double v, FieldValueState state)
        {
            return (state == FieldValueState.Error) ? "Error" :
                    (state == FieldValueState.NotAvailable) ? "No linked PGNs" :
                    v.ToString();
        }
    }

} // namespace
