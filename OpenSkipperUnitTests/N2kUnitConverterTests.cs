using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSkipperApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSkipperUnitTests
{
    [TestClass]
    public class N2kUnitConverter
    {
        [TestMethod]
        public void N2kUnitConvertTest()
        {
            #region Temperature

            double celsius = 18.1;
            double fahrenheit = 64.58;
            double kelvin = 291.25;

            // https://www.rapidtables.com/convert/temperature/celsius-to-kelvin.html
            Assert.IsTrue(UnitConverter.CelsiusToKelvin(celsius) == kelvin, "CelsiusToKelvin failed");

            // https://www.rapidtables.com/convert/temperature/kelvin-to-celsius.html
            Assert.IsTrue(UnitConverter.KelvinToCelsius(kelvin, 2) == celsius, "KelvinToCelsius failed");

            // https://www.rapidtables.com/convert/temperature/fahrenheit-to-kelvin.html
            Assert.IsTrue(UnitConverter.FahrenheitToKelvin(fahrenheit) == kelvin, "FahrenheitToKelvin failed");

            // https://www.rapidtables.com/convert/temperature/kelvin-to-fahrenheit.html
            Assert.IsTrue(UnitConverter.KelvinToFahrenheit(kelvin, 2) == fahrenheit, "KelvinToFahrenheit failed");

            #endregion

            #region Date/Time

            //  Test Date/time = Jan 1, 2018 at 10:00 am
            DateTime testDate = new DateTime(2018, 01, 01, 10, 0, 0);
            DateTime fromSecTestDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 10, 0, 0);
            DateTime fromDaysTestDate = new DateTime(testDate.Year, testDate.Month, testDate.Day, 0, 0, 0);
            int numSeconds = 60 * 60 * 10;
            int numDays = 17532;

            // Second Since Midnight = 60 seconds * 60 minutes * 10 hours
            Assert.IsTrue(UnitConverter.ConvertToNumSeconds(testDate) == numSeconds, "ConvertToNumSeconds failed");
            Assert.IsTrue(UnitConverter.ConvertFromNumSeconds(numSeconds) == fromSecTestDate, "ConvertFromNumSeconds failed");

            // https://www.convertunits.com/dates/from/Jan+1,+1970/to/Jan+1,+2018
            Assert.IsTrue(UnitConverter.ConvertToNumDays(testDate) == numDays, "ConvertToNumDays failed");
            Assert.IsTrue(UnitConverter.ConvertFromNumDays(numDays) == fromDaysTestDate, "ConvertFromNumDays failed");

            #endregion
        }

    } // class
    
} // namespace
