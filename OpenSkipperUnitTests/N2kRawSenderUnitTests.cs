using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using CANDefinitions;
using CANHandler;
using CANMessages;
using CANReaders;
using CANSenders;
using CANStreams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSkipperApplication;

namespace OpenSkipperUnitTests
{
    [TestClass]
    public class N2kRawSenderUnitTests
    {
        private const string HOST = "192.168.10.105";
        private const int PORT = 2000;

        //[TestMethod]
        public void N2kRawSendHeartbeat()
        {
            byte priority = 3.ToByte();
            int pgn = 126993; // Heartbeat
            byte destination = 255.ToByte();
            byte source = 1.ToByte();

            //60 EA 49 CF FF FF
            byte[] data = new byte[8];
            data[0] = "60".HexToByte();
            data[1] = "EA".HexToByte();
            data[2] = "49".HexToByte();
            data[3] = "CF".HexToByte();
            data[4] = "FF".HexToByte();
            data[5] = "FF".HexToByte();

            var header = new N2kHeader(priority, pgn, destination, source);
            var frame = new N2kFrame(header, data, DateTime.UtcNow);

            //SendFrame(frame);
        }

        [TestMethod]
        public void N2kRawSendWaterDepth()
        {
            /*
            128267: Water Depth
            Source=3
            Destination=255
            Priority=3
            Expected Length=8 bytes
            Actual Length=8 bytes
                0: 1B -- -- -- -- -- -- -- 00011011  SID=27
                0: -- E5 01 00 00 -- -- -- ........  Depth=4.85 m
                0: -- -- -- -- -- 00 00 -- ........  Offset=0.00
                0: -- -- -- -- -- -- -- FF 11111111  Reserved= NotAvailable
            */

            N2kMessage msg = new N2kMessage(128267, 3, 1, 255);
            msg.Frame.SetValue("SID", "27");
            msg.Frame.SetValue("Depth","4.85");
            msg.Frame.SetValue("Offset", "1.50");
            msg.Frame.SetValue("Reserved", N2kField.IntNA.ToString());   // Hex 0xFF

            FieldValueState state;

            // Check Frame Bytes
            //Assert.IsTrue(frame.ToString() == "1B-E5-01-00-00-00-00-FF"); // Offset = 0.00
            Assert.IsTrue(msg.Frame.ToString() == "1B-E5-01-00-00-DC-05-FF", "Hex did not match");   // Offset = 1.50

            // Check Depth
            var depthField = msg.Definition.GetFieldByName("Depth") as N2kUDblField;
            var depth = depthField.GetValue(msg.Frame.Data, out state);
            Assert.IsTrue(state == FieldValueState.Valid, "Depth is not valid");
            Assert.IsTrue(4.85 == Math.Round(depth, 2), "Depths do not match");

            // Check Offset
            var offsetField = msg.Definition.GetFieldByName("Offset") as N2kDblField;
            var offset = offsetField.GetValue(msg.Frame.Data, out state);
            Assert.IsTrue(state == FieldValueState.Valid, "Offset is not valid");
            Assert.IsTrue(1.50 == Math.Round(offset, 2), "Offsets do not match");

            //SendFrame(frame);
        }

        [TestMethod]
        public void N2kRawSendWaterTemperature()
        {
            /*
            130310: Environmental Parameters
            Source          = 3
            Destination     = 255
            Priority        = 5
            Expected Length = 7 bytes
            Actual Length   = 8 bytes
                0: 20 -- -- -- -- -- -- -- 00100000  SID = 32
                0: -- CD 71 -- -- -- -- -- ........  Water Temperature = 18.18 °C
                0: -- -- -- FF FF -- -- -- ........  Outside Ambient Air Temperature =  NotAvailable
                0: -- -- -- -- -- FF FF -- ........  Atmospheric Pressure =  NotAvailable
                0: -- -- -- -- -- -- -- FF 11111111  Reserved =  NotAvailable
            */

            #region Other Environmental Parameters
            /*
            130311: Environmental Parameters
            Source          = 35
            Destination     = 255
            Priority        = 5
            Expected Length = 8 bytes
            Actual Length   = 8 bytes
                0: 19 -- -- -- -- -- -- -- 00011001  SID = 25
                0: -- C0 -- -- -- -- -- -- ..000000  Temperature Instance = Sea Temperature(0)
                0: -- C0 -- -- -- -- -- -- 11......  Humidity Instance = NotAvailable
                0: -- -- 6B 76 -- -- -- -- ........  Temperature = 30.00 °C
                0: -- -- -- -- FF 7F -- -- ........  Humidity =  NotAvailable
                0: -- -- -- -- -- -- FF FF ........  Atmospheric Pressure =  NotAvailable

            130312: Temperature
            Source          = 3
            Destination     = 255
            Priority        = 5
            Expected Length = 8 bytes
            Actual Length   = 8 bytes
                0: 1F -- -- -- -- -- -- -- 00011111  SID = 31
                0: -- 00 -- -- -- -- -- -- 00000000  Temperature Instance = 0
                0: -- -- 00 -- -- -- -- -- 00000000  Temperature Source = Sea Temperature(0)
                0: -- -- -- CD 71 -- -- -- ........  Actual Temperature = 18.18 °C
                0: -- -- -- -- -- FF FF -- ........  Set Temperature =  NotAvailable
                0: -- -- -- -- -- -- -- FF 11111111  Reserved =  NotAvailable

            */
            #endregion

            N2kMessage msg = new N2kMessage(130310, 3, 1, 255);
            var waterTemp = UnitConverter.CelsiusToKelvin(18.18).ToString();
            var airTemp = UnitConverter.CelsiusToKelvin(21.8).ToString();  //255.ToString();
            var airPres = "1003";  //255.ToString();  // 1 millibar = 1 hPa 

            msg.Frame.SetValue("SID", "32");                                // N2kUIntField
            msg.Frame.SetValue("Water Temperature", waterTemp);             // N2kUDblField
            msg.Frame.SetValue("Outside Ambient Air Temperature", airTemp); // N2kUDblField
            msg.Frame.SetValue("Atmospheric Pressure", airPres);            // N2kUIntField
            msg.Frame.SetValue("Reserved", N2kField.IntNA.ToString());      // N2kUIntField

            // Check Frame Bytes
            //Assert.IsTrue(msg.Frame.ToString() == "20-CD-71-FF-FF-FF-FF-FF"); // airTemp and airPres = 255/Nan
            Assert.IsTrue(msg.Frame.ToString() == "20-CD-71-37-73-EB-03-FF", "Hex did not match");

            //SendFrameTwice(frame);
        }

        [TestMethod]
        public void N2kCreatePGN128275Test()
        {
            /*
            128275: Distance Log
            Source          = 35
            Destination     = 255
            Priority        = 6
            Expected Length = 14 bytes
            Actual Length   = 14 bytes
                0: FF FF -- -- -- -- -- -- ........  Date =  NotAvailable
                0: -- -- FF FF FF FF -- -- ........  Time =  NotAvailable
                0: -- -- -- -- -- -- 3A 04 
                8: 00 00 -- -- -- --       ........  Log = 1082
                8: -- -- 3A 04 00 00       ........  Trip Log = 1082
            */

            var log = 1082;
            var trip = 1082;
            var date = DateTime.Parse("2018-02-02 2:07:17 PM");

            // Test 1: No date/time set
            var frame = N2kMessages.CreatePGN128275(DateTime.MinValue, log, trip);
            Assert.IsTrue(frame.ToString() == "FF-FF-FF-FF-FF-FF-3A-04-00-00-3A-04-00-00", "Hex did not match");  // No date/time provided

            // Test 2: Check the RawSender output for the Fast Message
            var sender = new N2kRawSender();
            var toSend = sender.GetBytes(frame);
            var sb = new StringBuilder();
            sb.AppendLine("15F51301 00 0E FF FF FF FF FF FF");
            sb.AppendLine("15F51301 01 3A 04 00 00 3A 04 00");
            sb.AppendLine("15F51301 02 00");
            var checkBytes = Encoding.ASCII.GetBytes(sb.ToString());

            Assert.IsTrue(toSend.SequenceEqual(checkBytes), "N2kRawSender test 1 bytes did not match");

            // Test 3: Date/time set
            frame = N2kMessages.CreatePGN128275(date, log, trip);
            Assert.IsTrue(frame.ToString() == "9C-44-50-1C-4D-1E-3A-04-00-00-3A-04-00-00", "Hex did not match");

            // Test 4: Check the RawSender output for the Fast Message
            toSend = sender.GetBytes(frame);
            sb = new StringBuilder();
            sb.AppendLine("15F51301 00 0E 9C 44 50 1C 4D 1E");
            sb.AppendLine("15F51301 01 3A 04 00 00 3A 04 00");
            sb.AppendLine("15F51301 02 00");
            checkBytes = Encoding.ASCII.GetBytes(sb.ToString());

            Assert.IsTrue(toSend.SequenceEqual(checkBytes), "N2kRawSender test 4 bytes did not match");

            //SendFrame(frame);
        }

        [TestMethod]
        public void N2kParsePGN128275Test()
        {
            DateTime date;
            int log;
            int trip;

            // Raw string created using N2kCreatePGN128275Test()
            var sb = new StringBuilder();
            var receive = DateTime.UtcNow.ToString("HH:mm:ss.fff") + " R ";
            sb.AppendLine(receive + "15F51301 00 0E 9C 44 50 1C 4D 1E ");
            sb.AppendLine(receive + "15F51301 01 3A 04 00 00 3A 04 00 ");
            sb.AppendLine(receive + "15F51301 02 00 ");

            byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());
            var reader = new N2kRawReader();
            var frame = reader.GetAsSingleFrame(bytes);

            var msg2 = N2kMessages.ParsePGN128275(frame, out date, out log, out trip);

            Assert.IsTrue(date == DateTime.Parse("2018-02-02 2:07:17 PM"), "Date did not match");
            Assert.IsTrue(log == 1082, "Log did not match");
            Assert.IsTrue(trip == 1082, "Trip did not match");
        }

        [TestMethod]
        public void N2kCreatePGN130310Test()
        {
            /*
            130310: Environmental Parameters
            Source          = 3
            Destination     = 255
            Priority        = 5
            Expected Length = 7 bytes
            Actual Length   = 8 bytes
                0: 20 -- -- -- -- -- -- -- 00100000  SID = 32
                0: -- CD 71 -- -- -- -- -- ........  Water Temperature = 18.18 °C
                0: -- -- -- FF FF -- -- -- ........  Outside Ambient Air Temperature =  NotAvailable
                0: -- -- -- -- -- FF FF -- ........  Atmospheric Pressure =  NotAvailable
                0: -- -- -- -- -- -- -- FF 11111111  Reserved =  NotAvailable
            */
            byte sid = 32.ToByte(); // Sequence ID (0 - 252)
            var waterTemp = UnitConverter.CelsiusToKelvin(18.18);
            var airTemp = UnitConverter.CelsiusToKelvin(21.8);
            var airPres = 1003;  //255.ToString();  // 1 millibar = 1 hPa 

            var frame = N2kMessages.CreatePGN130310(sid, waterTemp, airTemp, airPres);

            Assert.IsTrue(frame.ToString() == "20-CD-71-37-73-EB-03-FF", "Hex did not match");

            SendFrameTwice(frame);
        }

        private N2kFrame.Builder builder;

        [TestMethod]
        public void N2kConvertKeensToRaw()
        {
            Definitions.LoadPGNDefns("");

            var fileName = TestHelper.GetTestFilePath("KeesN2kLog.xml");
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment });

            builder = new N2kFrame.Builder();

            var reader = new N2kDatagramReader();
            reader.FrameCreated += Reader_FrameCreated;

            while (xmlReader.Read())
            {
                // <N2kMsg TimeStamp="2009-06-18T09:46:01.129">0DF80502A4FFFFFFFF10FC0B</N2kMsg>
                var timeStamp = xmlReader.GetAttribute("TimeStamp");
                var data = xmlReader.ReadElementContentAsString();

                Hex hex = new Hex(data);
                Byte[] Bytes = hex.ToBytes();

                reader.ProcessBytes(Bytes, 0, Bytes.Length);
            }
        }

        private void Reader_FrameCreated(Frame frame)
        {
            if (frame is N2kFrame)
            {
                var outFrame = builder.AddFrame((N2kFrame)frame);

                if (outFrame != null)
                {
                    N2kRawSender sender = new N2kRawSender();
                    File.AppendAllText(TestHelper.GetTestFilePath("RawN2kLog.log"), sender.GetLines(outFrame));
                }
            }
        }

        private void SendFrame(N2kFrame frame)
        {
            using (var client = new UDP_Client(HOST, PORT))
            {
                client.OutgoingProtocol = CANSenders.OutgoingProtocolEnum.N2KRaw;
                client.SendFrame(frame);
            }
        }

        private void SendFrameTwice(N2kFrame frame)
        {
            SendFrame(frame);
            Thread.Sleep(1000);
            SendFrame(frame);
        }

        private void WriteLog(string name, string value)
        {
            Debug.WriteLine("{0} = {1}", name, value);
        }

        private void WriteLog(string value)
        {
            Debug.WriteLine(value);
        }

    } // class

} // namespace
