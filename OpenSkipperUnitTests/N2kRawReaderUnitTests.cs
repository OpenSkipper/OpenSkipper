using CANHandler;
using CANReaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace OpenSkipperUnitTests
{
    [TestClass]
    public class N2kRawReaderUnitTests
    {
        // PGN Lookup: https://www.isobus.net/isobus/pGNAndSPN/index?PGNAndSPN_page=28&PGNAndSPN_sort=pgn_description.desc
        // NMEA 2000 0183 to PGN: https://www.fugawi.com/knowledge_base/document/HD25056

        // Message ID to PGN for SAE J1939 extended ID (CAN 2.0B standard)
        // http://alumni.cs.ucsb.edu/~savior/convert-j1939-id-to-pgn.php
        // https://www.csselectronics.com/screen/page/j1939-pgn-conversion-tool/language/en

        [TestMethod]
        public void N2kRawReaderTest()
        {
            var reader = new N2kRawReader();

            // Single Part Message
            byte[] data = Encoding.ASCII.GetBytes("12:40:03.316 R 09F80203 FF FC 20 AE 02 00 FF FF");
            Assert.IsTrue(reader.ProcessBytes(data, 0, 1) == 1, "N2kRawReader failed to parse single part message");

            // Multi Part Message
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("12:40:03.316 R 0DFE1103 80 0E C0 8D 20 AE 02 00");
            sb.AppendLine("12:40:03.317 R 0DFE1103 81 FF FF FF FF FF FF FF");
            sb.AppendLine("12:40:03.318 R 0DFE1103 82 FF FF FF FF FF FF FF");
            sb.AppendLine("12:40:03.319 R 09F80103 6B B6 80 1A C8 8B 11 D0");
            byte[] data2 = Encoding.ASCII.GetBytes(sb.ToString());
            Assert.IsTrue(reader.ProcessBytes(data2, 0, 1) == 4, "N2kRawReader failed to parse multi part message");
        }

        [TestMethod]
        public void N2kRawReaderIdParseTest()
        {
            // Parses a NMEA 2000 Raw Message ID and checks if it is the correct PGN
            var idHex = "DF50B01";
            var msgID = new N2kRawMessage(idHex);
            msgID.Parse();

            Assert.IsTrue(msgID.Pgn == 128267, "PGN does not match");
        }

        [TestMethod]
        public void N2kRawReaderParseTest()
        {
            // Parses a NMEA 2000 Raw Message and checks if it is the correct data
            byte[] data = Encoding.ASCII.GetBytes("20:27:12.740 T DF50B01 00 0C 01 00 00 00 00 00");
            var reader = new N2kRawReader();
            var msgs = reader.ParseBytes(data);

            foreach(var msg in msgs)
            {
                Assert.IsTrue(msg.Pgn == 128267, "PGN does not match");
                Assert.IsTrue(Convert.ToDouble(msg.Data[1]) == 12, "Water Depth does not match");
                Assert.IsTrue(Convert.ToDouble(msg.Data[2]) == 1, "Transducer Depth does not match");
            }
        }


        [TestMethod]
        public void N2kRawReaderFastMessageTest()
        {
            // PGN: 129540 - GNSS Sats in View (219 bytes)
            // Looks like the Raymarine GPS is only retuning 171 bytes for this message
            // It is possible this is because the GPS is only seeing x out of the available satellites
            // while sitting on the development desk
            //var testFileName = "PGN129540.txt";
            //var hexPGN = "19FA0403";

            // PGN: 128275 - Distance Log (14 bytes)
            var testFileName = "PGN128275.txt";
            var hexPGN = "15F51301";

            byte[] data = Encoding.ASCII.GetBytes(TestHelper.LoadTestFile(testFileName));
            var msg = new N2kRawMessage(hexPGN);
            var reader = new N2kRawReader();

            var frames = reader.GetFrames(data);
            var builder = new N2kFrame.Builder();
            var done = false;
            foreach(var frame in frames)
            {
                var built = builder.AddFrame(frame);
                if (built != null)
                {
                    Assert.IsTrue(built.Data.Length == built.Defn.ByteLength, "Data and PGN lengths don't match");
                    done = true;
                }
            }

            Assert.IsTrue(done, "Did not successfully process " + testFileName);
        }



    } // class

} // namespace
