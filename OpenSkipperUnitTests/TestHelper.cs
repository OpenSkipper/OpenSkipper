using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSkipperUnitTests
{
    internal class TestHelper
    {
        private const string TEST_FILE_SUB_FOLDER = "TestFiles";

        /// <summary>
        /// Loads a file from the test directory
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string LoadTestFile(string fileName)
        {
            fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TEST_FILE_SUB_FOLDER, fileName);

            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }

            return string.Empty;
        } 

    } // class

} // namespace
