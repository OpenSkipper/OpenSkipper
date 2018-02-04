/*
	Copyright (C) 2018 Timo Lappalainen, Michael Lucas

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

namespace OpenSkipperApplication
{
    public class UnitConverter
    {
        private const double KELVIN_OFFSET = 273.15;
        private const double HECTOSECONDS_OFFSET = 3600;
        private const double FEET_OFFSET = 3.28084;

        private static DateTime UNIX_START_DATE = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        #region Angle

        /// <summary>
        /// Convert Radians to Degrees (°)
        /// </summary>
        /// <param name="value">Angle in Radians</param>
        /// <returns></returns>
        public static double RadiansToDegrees(double value)
        {
            return value * 180.0 / 3.1415926535897932384626433832795;
        }

        /// <summary>
        /// Convert Degrees (°) to Radians
        /// </summary>
        /// <param name="value">Angle in Degrees</param>
        /// <returns></returns>
        public static double DegreesToRadians(double value)
        {
            return value / 180.0 * 3.1415926535897932384626433832795;
        }

        #endregion

        #region Pressure

        /// <summary>
        /// Convert Millibar (mbar) To Pascal (Pa)
        /// </summary>
        /// <param name="value">Pressure in  mbar</param>
        /// <returns></returns>
        public static double MillibarToPascal(double value)
        {
            return value * 100;
        }

        /// <summary>
        /// Convert Pascal (Pa) To Millibar (mbar)
        /// </summary>
        /// <param name="value">Pressure in Pa</param>
        /// <returns></returns>
        public static double PascalToMillibar(double value)
        {
            return value / 100;
        }

        /// <summary>
        /// Convert Hectopascal (hPa) To Pascal (Pa)
        /// </summary>
        /// <param name="value">Pressure in hPa</param>
        /// <returns></returns>
        public static double HectopascaToPascal(double value)
        {
            return value * 100;
        }

        /// <summary>
        /// Convert Pascal (Pa) To Hectopascal (hPa)
        /// </summary>
        /// <param name="value">Pressure in Pa</param>
        /// <returns></returns>
        public static double PascalToHectopascal(double value)
        {
            return value / 100;
        }

        #endregion

        #region Power

        /// <summary>
        /// Convert Ampere-Hours (Ah) To Coulomb (C)
        /// </summary>
        /// <param name="value">Power in Ah</param>
        /// <returns></returns>
        public static double AmpereHoursToCoulomb(double value)
        {
            return value * 3600;
        }

        /// <summary>
        /// Convert Coulomb (C) To Ampere-Hours (Ah)
        /// </summary>
        /// <param name="value">Power in C</param>
        /// <returns></returns>
        public static double CoulombToAmpereHours(double value)
        {
            return value / 3600;
        }

        #endregion

        #region Temperature

        /// <summary>
        /// Converts a Celsius (C) value to Kelvin (K)
        /// </summary>
        /// <param name="value">Temperature in C</param>
        /// <returns></returns>
        public static double CelsiusToKelvin(double value)
        {
            return value + KELVIN_OFFSET;
        }
        public static double CelsiusToKelvin(double value, int roundTo)
        {
            return Math.Round(CelsiusToKelvin(value), roundTo);
        }
        /// <summary>
        /// Converts a Kelvin (K) value to Celsius (C)
        /// </summary>
        /// <param name="value">Temperature in K</param>
        /// <returns></returns>
        public static double KelvinToCelsius(double value)
        {
            return value - KELVIN_OFFSET;
        }
        public static double KelvinToCelsius(double value, int roundTo)
        {
            return Math.Round(KelvinToCelsius(value), roundTo);
        }

        /// <summary>
        /// Converts a Fahrenheit (F) value to Kelvin (K)
        /// </summary>
        /// <param name="value">Temperature in F</param>
        /// <returns></returns>
        public static double FahrenheitToKelvin(double value)
        {
            return (value - 32) * 5.0 / 9.0 + KELVIN_OFFSET;
        }
        public static double FahrenheitToKelvin(double value, int roundTo)
        {
            return Math.Round(FahrenheitToKelvin(value), roundTo);
        }

        /// <summary>
        /// Converts a Kelvin (K) value to Fahrenheit (F)
        /// </summary>
        /// <param name="value">Temperature in K</param>
        /// <returns></returns>
        public static double KelvinToFahrenheit(double value)
        {
            return (value - KELVIN_OFFSET) * 9.0 / 5.0 + 32;
        }
        public static double KelvinToFahrenheit(double value, int roundTo)
        {
            return Math.Round(KelvinToFahrenheit(value), roundTo);
        }

        #endregion

        #region Speed

        /// <summary>
        /// Convert Meters per second (ms) to Knots (k)
        /// </summary>
        /// <param name="value">Speed in ms</param>
        /// <returns></returns>
        public static double MetersPerSecondToKnots(double value)
        {
            return value * 3600 / 1852.0;
        }

        #endregion

        #region Time

        /// <summary>
        /// Convert Hectoseconds (hs) To Seconds (s)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double HectosecondsToSeconds(double value)
        {
            return value * HECTOSECONDS_OFFSET;
        }

        /// <summary>
        /// Convert Seconds (s) to Hectoseconds (hs) 
        /// </summary>
        /// <param name="value">Time in s</param>
        /// <returns></returns>
        public static double SecondsToHectoseconds(double value)
        {
            return value / HECTOSECONDS_OFFSET;
        }

        #endregion

        #region Date/Time

        /// <summary>
        /// Converts a Unix Time to a DateTime in UTC
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime ConvertFromUnixTimestamp(double unixTime)
        {
            // TODO: Use .Net 4.6 built in functions?
            // See: https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(UNIX_START_DATE.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }

        /// <summary>
        /// Converts a UTC DateTime to a Unix Time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double ConvertToUnixTimestamp(DateTime dateTime)
        {
            // TODO: Use .Net 4.6 built in functions?
            // See: https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - UNIX_START_DATE).Ticks;
            return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// Calculates the number of days since January 1, 1970 for the provided UTC date.
        /// </summary>
        /// <param name="currentDateTime in UTC"></param>
        /// <returns>Days since January 1, 1970</returns>
        public static int ConvertToNumDays(DateTime dateTime)
        {
            // See: https://www.epochconverter.com/
            return (int)(dateTime - UNIX_START_DATE).TotalDays;
        }

        /// <summary>
        /// Calculates the date for the number of days January 1, 1970.
        /// </summary>
        /// <param name="numDays">Number of days since January 1, 1970</param>
        /// <returns></returns>
        public static DateTime ConvertFromNumDays(int numDays)
        {
            return UNIX_START_DATE.AddDays(numDays);
        }

        /// <summary>
        /// Calculates the number of Seconds since midnight
        /// </summary>
        /// <param name="dateTime">Current date/time</param>
        /// <returns>Seconds since midnight</returns>
        public static int ConvertToNumSeconds(DateTime dateTime)
        {
            TimeSpan sinceMidnight = dateTime - new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            return (int)sinceMidnight.TotalSeconds;
        }

        /// <summary>
        /// Calculates today's UTC date with the time since midnight
        /// </summary>
        /// <param name="seconds">Seconds since midnight</param>
        /// <returns></returns>
        public static DateTime ConvertFromNumSeconds(int seconds)
        {
            var dateTime = DateTime.UtcNow;
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddSeconds(seconds);
        }

        #endregion

        #region Measurement

        /// <summary>
        /// Converts Meters (m) to Feet (ft)
        /// </summary>
        /// <param name="value">Distance or Depth in m</param>
        /// <returns></returns>
        public static double MetersToFeet(double value)
        {
            return value * FEET_OFFSET;
        }
        public static double MetersToFeet(double value, int roundTo)
        {
            return Math.Round(MetersToFeet(value), roundTo);
        }
        /// <summary>
        /// Converts Feet (ft) to Meters (C)
        /// </summary>
        /// <param name="value">Distance or Depth in ft</param>
        /// <returns></returns>
        public static double FeetToMeters(double value)
        {
            return value / FEET_OFFSET;
        }
        public static double FeetToMeters(double value, int roundTo)
        {
            return Math.Round(FeetToMeters(value), roundTo);
        }

        #endregion

    } // class

} // namespace
