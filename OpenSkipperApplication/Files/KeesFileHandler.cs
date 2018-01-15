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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CANHandler;
using CANDefinitions;
using System.Globalization;

namespace KeesFileHandler
{
    /// <summary>
    /// Represents a field of a Kees format definition
    /// </summary>
    [XmlRoot("Field")]
    public class KeesField
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public int BitOffset { get; set; } // counting left to right, eg ---XXXX- has a bit offset of 3
        public int BitLength { get; set; }  // 0 means "all the data"
        public int BitStart { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Resolution { get; set; }
        public string Units { get; set; }
        public bool Signed { get; set; }

        // Array accessors are used to enable easy XML serialization
        [XmlArray("EnumValues")]
        // [XmlArrayItem("EnumPair", typeof(EnumPair))]
        public EnumPair[] EnumPairArray { get; set; }


        // The following attributes do not show in the PropertyGrid
        [BrowsableAttribute(false)]
        public int ByteOffset { get { return BitOffset >> 3; } }
        [BrowsableAttribute(false)]
        public int ByteLength { get { return BitLength >> 3; } }

        [BrowsableAttribute(false)]
        public int BitOffsetWithinByte { get { return BitOffset - ByteOffset * 8; } }

        // Default method for displaying any field; in this case as hex
        public virtual string ToString(byte[] d) {
            int Length = BitLength > 0 ? ByteLength : d.Length - ByteOffset;
            return BitConverter.ToString(d, ByteOffset, Length);
        }
    }

    /// <summary>
    /// Represents a definition for a PGN in Kees format
    /// </summary>
    [XmlRoot("PGNInfo")]
    public class KeesPGNDefn
    {
        public uint PGN { get; set; }
        public string Description { get; set; }
        public bool Complete { get; set; }

        [XmlArray("Fields")]
        // Explicitly tell the serializer to expect the Item class
        // so it can be properly written to XML from the collection:
        [XmlArrayItem("Field", typeof(KeesField))]
        public KeesField[] Fields { get; set; }

        // Creators
        public KeesPGNDefn() {
        }

        public string ToString(byte[] d) {
            string msg = PGN.ToString();
            foreach (KeesField f in Fields) {
                msg = msg + '\n' + f.Name + '\n' + f.ToString(d);
            }
            return msg;
        }
    }

    /// <summary>
    /// Represents a collection of PGN definitions in Kees format, and supports translating that into Open Skipper XML format
    /// This definition collection cannot be used directly by open skipper while in Kees format
    /// </summary>
    [XmlRoot("PGNDefinitions")]
    public class KeesPGNDefnCollection
    {
        [XmlAttribute]
        public string Version = ""; // { get; set; }

        // We serlialise a date. But, if DateSpecified is false, the Data is not serialised
        public string Date = "";

        public bool ShouldSerializeDate() { return Date != ""; }

        // We serlialise an optional Comment. But, if DateSpecified is false, the Data is not serialised
        public string Comment = "Test Run";

        public bool ShouldSerializeComment() { return Comment != ""; }

        public string CreatorCode = "PGN Tester";
        public bool ShouldSerializeCreatorCode() { return CreatorCode != ""; }

        [XmlArray("PGNs")]
        [XmlArrayItem("PGNInfo", typeof(KeesPGNDefn))]
        public KeesPGNDefn[] KeesPGNDefns { get; set; }

        // Constructor
        public KeesPGNDefnCollection() {
            KeesPGNDefns = null;
        }

        public static KeesPGNDefnCollection XmlDeserialize(StreamReader stream) {
            return (KeesPGNDefnCollection)XmlSerializer.Deserialize(stream);
        }

        // Create a static instance of the XmlSerializer class.
        public static System.Xml.Serialization.XmlSerializer XmlSerializer = new XmlSerializer(typeof(KeesPGNDefnCollection));

        public PGNDefn[] GetPGNInfos()
            // Get an array of PGNDefn records, effectively converting from KeesDefinitions 
            // into our own PGNDefn array
        {
            PGNDefn[] result = new PGNDefn[KeesPGNDefns.Count()];
            int j = 0;
            foreach (KeesPGNDefn kPGN in KeesPGNDefns)
            {
                N2kField[] f = new N2kField[kPGN.Fields != null ? kPGN.Fields.Count() : 0];

                if (kPGN.Fields != null)
                {
                    int i = 0;
                    foreach (KeesField kField in kPGN.Fields) 
                    {
                        double scale;
                        if (!double.TryParse(kField.Resolution, NumberStyles.Number, CultureInfo.InvariantCulture, out scale))
                            scale = 1.0;

                        /* // Old method for working out scale...
                        switch (kField.Resolution)
                        {
                            case "0.1":
                                scale = 0.1;
                                break;
                            case "0.01":
                                scale = 0.01;
                                break;
                            case "0.001":
                                scale = 0.001;
                                break;
                            case "0.004":
                                scale = 0.004;
                                break;
                            case "1e-006":
                                scale = 1e-6;
                                break;
                            case "0.0001":
                                scale = 0.001;
                                break;
                            default:
                                scale = 1;
                                break;
                        }
                        */

                        if ((kPGN.PGN == 130842) && (kField.Order == 17) && (kField.Name == "Spare")) {
                            // AJM: I think this is probably an unsigned integer based on Description="0=unavailable"
                            f[i++] = new N2kUIntField { Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                        }
                        else if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(kField.Name, "Instance", CompareOptions.IgnoreCase)>=0)
                        {
                            f[i++] = new N2kInstanceField { Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                        }
                        else if ((kField.Name == "SID"))
                        {
                            f[i++] = new N2kUIntField { FormatAs = FormatEnum.SID, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                        } else if ((kField.Name == "PGN")) {
                            // We force all PGNBs to be UInt; Kees has one defined as an Integer in 59904, but no such 'Integer' defn in 59392
                            f[i++] = new N2kUIntField { Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                        } else if ((kField.Name == "Speed") || (kField.Name == "SOG")) {
                            //f[i++] = new SpeedField { DisplayName = kField.DisplayName, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description, Scale = scale };
                            f[i++] = new N2kUDblField { FormatAs = FormatEnum.Speed, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description, Scale = scale };
                        } else if ((kField.Name == "Depth") || (kField.Name == "SOG")) {
                            // This is probably unsigned based on FFFFFFFF appearing to mean 'unavailable'
                            f[i++] = new N2kUDblField { FormatAs = FormatEnum.Length, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description, Scale = scale };
                        } else {
                            switch (kField.Type) {
                                case null:
                                    if ((kField.Resolution == null) || (kField.Resolution == "")) {
                                        f[i++] = new N2kUIntField { Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                                    } else {
                                        f[i++] = new N2kDblField { Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description, Scale = scale };
                                    }
                                    break;
                                case "ASCII text":
                                    f[i++] = new N2kASCIIField { Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                                    break;
                                case "Binary data":
                                case "String with start/stop byte":  // We need new type for this
                                    f[i++] = new N2kBinaryField { Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                                    break;
                                case "Manufacturer code":
                                    f[i++] = new N2kBinaryField { Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                                    break;
                                case "Date":
                                    //f[i++] = new DateField { DisplayName = kField.DisplayName, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                                    f[i++] = new N2kUIntField { FormatAs = FormatEnum.Date, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                                    break;
                                case "Degrees":  // Degrees changed to angle on Kees?
                                case "Angle":
                                    if (((kField.Resolution == "0.0001 rad") || (kField.Resolution == "0.0001")) ) {
                                        // We convert the units from Radians to Degrees
                                        if (kField.Signed)
                                        {
                                            f[i++] = new N2kDblField { FormatAs = FormatEnum.Heading, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description, Scale = 0.0001 * 180 / Math.PI };
                                        } 
                                        else
                                        {
                                            f[i++] = new N2kUDblField { FormatAs = FormatEnum.Heading, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description, Scale = 0.0001 * 180 / Math.PI };
                                        }
                                    } else {
                                        Console.WriteLine("Unknown Kees Degrees field data");
                                    }
                                    break;
                                case "IEEE Float":
                                        f[i++] = new N2kDblField { FormatAs = FormatEnum.Number, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = 32, Description = kField.Description, Scale=scale };
                                    break;
                                case "Integer":
                                    f[i++] = new N2kIntField { Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                                    break;
                                case "Latitude":
                                    if (kField.BitLength == 32) {
                                        // LatField
                                        f[i++] = new N2kDblField { FormatAs = FormatEnum.Latitude, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = 32, Description = kField.Description, Scale = 1e-7 };
                                    } else if (kField.BitLength == 64) {
                                        // LatField
                                        f[i++] = new N2kDblField { FormatAs = FormatEnum.Latitude, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = 64, Description = kField.Description, Scale = 1e-16 };
                                    } else {
                                        Console.WriteLine("Unknown Kees Latitude field data");
                                    }
                                    break;
                                case "Longitude":
                                    if (kField.BitLength == 32) {
                                        // LonField
                                        f[i++] = new N2kDblField { FormatAs = FormatEnum.Longitude, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = 32, Description = kField.Description, Scale = 1e-7 };
                                    } else if (kField.BitLength == 64) {
                                        // LonField
                                        f[i++] = new N2kDblField { FormatAs = FormatEnum.Longitude, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = 64, Description = kField.Description, Scale = 1e-16 };
                                    } else {
                                        Console.WriteLine("Unknown Kees Longitude field data");
                                    }
                                    break;
                                case "Lookup table":
                                    f[i++] = new N2kEnumField { Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description, EnumPairArray = kField.EnumPairArray };
                                    break;
                                case "Temperature":
                                    if ((kField.Units == null) || (kField.Units == "K")) {
                                        // We convert this to Celcius
                                        // f[i++] = new TemperatureField { DisplayName = kField.DisplayName, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description };
                                        f[i++] = new N2kUDblField { FormatAs = FormatEnum.Temperature, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description, Scale = 0.01, Offset = -273.15};
                                    } else {
                                        Console.WriteLine("Unknown Kees Temperature field data");
                                    }
                                    break;
                                case "Time":
                                    // TimeField
                                    f[i++] = new N2kUDblField { FormatAs = FormatEnum.Time, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description, Scale = 0.0001 };
                                    break;
                                case "Speed":
                                    f[i++] = new N2kUDblField { FormatAs = FormatEnum.Speed, Name = kField.Name, BitOffset = kField.BitOffset, BitLength = kField.BitLength, Description = kField.Description, Scale = scale };
                                    break;
                                default:
                                    Console.WriteLine("Unknown Kees Field");
                                    break;
                            }
                        }
                    }
                }
                uint ByteLength = 0;
                foreach (N2kField field in f) {
                    ByteLength = (uint)Math.Max(ByteLength, (int)(((uint)((uint)field.BitOffset + (uint)field.BitLength + 7)) >> 3));
                }
                if (kPGN.PGN == 129029) {
                    ByteLength = 43;   // Add 3 extra bytes missing in Kees data
                }
                result[j++] = new PGNDefn { PGN = kPGN.PGN, Name = kPGN.Description, Description = "", ByteLength = ByteLength, Fields = f };
            }
            return result;
        }
    }

    /*public class KeesPGNDefnTester
    {
        static public void TestPGNInfo()
        {

            //Field f = new Field { DisplayName = "Test", BitLength = 8, BitOffset = 0 };
            //Console.Out.WriteLine(f);

            //// Create an instance of the XmlSerializer class.
            //XmlSerializer mySerializer1 = new XmlSerializer(typeof(Field));
            //TextWriter writer1 = new StreamWriter(@"C:\temp\N2k\field.xml");
            //mySerializer1.Serialize(writer1, f);
            //writer1.Close();


            //Field[] fields = {
            //                new Field{DisplayName = "Test", BitLength=8, BitOffset=0 },
            //                new Field{DisplayName = "Test2", BitLength=8, BitOffset=0 },
            //                new Field{DisplayName = "Test3", BitLength=8, BitOffset=0 }
            //                        };
            //// PGNDefn info = new PGNDefn { PGN = 1, Fields = fields };
            //var PGNinfo1 = new PGNDefn(124575, "NMEA PGN", 8, fields);

            //EnumField fEnum = new EnumField { DisplayName = "Enum", BitLength = 8 };
            //fEnum.EnumLabels = new Dictionary<uint,string>();
            //fEnum.EnumLabels[0] = "Value0";
            //fEnum.EnumLabels[1] = "Value1";
            //fEnum.EnumLabels[2] = "Value2";

            //var PGNinfo2 = new PGNDefn(157477, "NMEA", 16, new Field[] {
            //                fEnum,
            //                new Field{DisplayName = "Field", BitLength=8, BitOffset=0 },
            //                new NumericField{DisplayName = "Numeric", BitLength=16, BitOffset=0 },
            //                new IntField{DisplayName = "Int", BitLength=32, BitOffset=0 },
            //                new DblField{DisplayName = "Dbl", BitLength=32, BitOffset=8, Offset=100, Scale = 0.01 },
            //                new UnknownField(),
            //                new Field{DisplayName = "Field", BitLength=8, BitOffset=0 }
            //                        });
            //byte[] TestData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            //Console.Out.WriteLine(f.ToString(TestData));
            //Console.Out.WriteLine("PGN={0}", PGNinfo2.PGN);
            //Console.Out.WriteLine("{0}={1}", PGNinfo2.Fields[0].DisplayName, PGNinfo2.Fields[0].ToString(TestData));
            //Console.Out.WriteLine("{0}={1}", PGNinfo2.Fields[1].DisplayName, PGNinfo2.Fields[1].ToString(TestData));
            //Console.Out.WriteLine("{0}={1}", PGNinfo2.Fields[2].DisplayName, PGNinfo2.Fields[2].ToString(TestData));
            //Console.Out.WriteLine("{0}={1}", PGNinfo2.Fields[3].DisplayName, PGNinfo2.Fields[3].ToString(TestData));
            //Console.Out.WriteLine("{0}={1}", PGNinfo2.Fields[4].DisplayName, PGNinfo2.Fields[4].ToString(TestData));
            //Console.Out.WriteLine("{0}={1}", PGNinfo2.Fields[5].DisplayName, PGNinfo2.Fields[5].ToString(TestData));
            //Console.Out.WriteLine("Space");

            //PGNDefnCollection Collect = new PGNDefnCollection();
            //Collect.PGNDefns = new PGNDefn[] {PGNinfo1,PGNinfo2 };

            // Create an instance of the XmlSerializer class.
            XmlSerializer mySerializer2 = new XmlSerializer(typeof(KeesPGNDefnCollection));

            //// Writing the file requires a TextWriter.
            //TextWriter writer2 = new StreamWriter(@"C:\temp\N2k\PGNDefn.xml");

            //// Serialize the class, and close the TextWriter.
            //mySerializer2.Serialize(writer2, Collect);
            //writer2.Close();


            // Deserialising from XML file for creating PGNDefnCollection Class
            KeesPGNDefnCollection KeesDefinitions;
            TextReader r = new StreamReader(@"D:\Documents\Projects\NMEA 2000 Interfacing\Kees Software\packetlogger_20090806_explain.xml");
            KeesDefinitions = (KeesPGNDefnCollection)mySerializer2.Deserialize(r);
            r.Close();
            // Console.Out.WriteLine(Collect2.PGNDefns[1].ToString(TestData) + '\n');

            var PGNDefnCol = new PGNDefnCollection
            {
                Version = KeesDefinitions.Version,
                Date = KeesDefinitions.Date,
                Comment = KeesDefinitions.Comment,
                CreatorCode = KeesDefinitions.CreatorCode,
                PGNDefns = KeesDefinitions.GetPGNInfos()
            };

            // Create an instance of the XmlSerializer class.
            XmlSerializer mySerializer3 = new XmlSerializer(typeof(PGNDefnCollection));

            // Writing the file requires a TextWriter.
            TextWriter writer3 = new StreamWriter(@"C:\temp\N2k\KeesPGNInfo.xml");

            // Serialize the class, and close the TextWriter.
            mySerializer3.Serialize(writer3, PGNDefnCol);
            writer3.Close();

            // My Testing Section
            // Testing Data
            /*byte[] myTestData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            // Testing Field Class
            Field fTest1 = new Field { DisplayName = "Tested Field", BitLength = 21, BitOffset = 10};
            Console.Out.WriteLine( "Field DisplayName" + fTest1.DisplayName + '\n'
                                   + "BitOffset: " + fTest1.BitOffset + '\n'
                                   + "BitLength: " + fTest1.BitLength + '\n'
                                   + "ByteOffset: " + fTest1.ByteOffset + '\n'
                                   + "ByteLength: " + fTest1.ByteLength + '\n'
                                   + "BitOffsetWithinByte: " + fTest1.BitOffsetWithinByte + '\n'
                                   + "ToString: " + fTest1.ToString(myTestData));
            
            // Testing String Field Class
            byte[] myTestData2 = new byte[] { 65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80 };
            StringField stringTest1 = new StringField {DisplayName = "String Field", BitOffset = 16, BitLength = 64};
            Console.Out.WriteLine("Field DisplayName" + stringTest1.DisplayName + '\n'
                                   + "BitOffset: " + stringTest1.BitOffset + '\n'
                                   + "BitLength: " + stringTest1.BitLength + '\n'
                                   + "ByteOffset: " + stringTest1.ByteOffset + '\n'
                                   + "ByteLength: " + stringTest1.ByteLength + '\n'
                                   + "BitOffsetWithinByte: " + stringTest1.BitOffsetWithinByte + '\n'
                                   + "ToString: " + stringTest1.ToString(myTestData2) + '\n'
                                   + "StringValue: " + stringTest1.StringValue(myTestData2));

            // Testing NumericField
            NumericField nTest1 = new NumericField {DisplayName = "Numeric Field", BitOffset = 8, BitLength = 32 };
            Console.Out.WriteLine("Field DisplayName" + nTest1.DisplayName + '\n'
                                   + "BitOffset: " + nTest1.BitOffset + '\n'
                                   + "BitLength: " + nTest1.BitLength + '\n'
                                   + "ByteOffset: " + nTest1.ByteOffset + '\n'
                                   + "ByteLength: " + nTest1.ByteLength + '\n'
                                   + "BitOffsetWithinByte: " + nTest1.BitOffsetWithinByte + '\n'
                                   + "ToString: " + nTest1.ToString(myTestData) + '\n'
                                   + "StringValue: " + nTest1.RawValue(myTestData));
            

            //            var PGNs = new PGNFieldCollection();
            //            PGNs.PGNInfoArray = new PGNDefn[2] { PGNinfo1, PGNinfo2 };

            // Create an instance of the XmlSerializer class.
            //            XmlSerializer mySerializer3 = new XmlSerializer(typeof(PGNFieldCollection));

            // Writing the file requires a TextWriter.
            //            TextWriter writer3 = new StreamWriter(@"c:\PGNFieldCollection.xml");

            // Serialize the class, and close the TextWriter.
            //            mySerializer3.Serialize(writer3, PGNs);
            //            writer3.Close();
        }
    }
    */

    /*public class KeesLogReader
    {
        public static FrameCollection ReadFrameCollection(StreamReader sr)
        {
            var FrameCol = new FrameCollection();
            try {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                int LargestByteCount = 0;
                int LinesWithErrors = 0;
                string ErrorLines = "";
                int LineCount = 0;
                // Frames = new List<Frame>();
                String line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null) {
                    LineCount++;
                    try {
                        char[] charSeparators = new char[] { ',' };
                        var elements = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                        // DateTime Date = DateTime.ParseExact(elements[0], "yyyy-MM-dd'Z'HH':'mm':'ss'.'FFF", System.Globalization.CultureInfo.InvariantCulture);
                        if (elements[0][10] == 'Z') elements[0] = elements[0].Replace('Z', 'T');  // Get rid of local time designation so we write back out as came in
                        DateTime Date = DateTime.Parse(elements[0]);
                        //DateTime Date = DateTime.SpecifyKind(DateTime.Parse(elements[0]), DateTimeKind.Unspecified);
                        int Priority = Convert.ToInt32(elements[1]);
                        int PGN = Convert.ToInt32(elements[2]);
                        int Source = Convert.ToInt32(elements[3]);
                        int Destination = Convert.ToInt32(elements[4]);
                        int ByteCount = Convert.ToInt32(elements[5]);
                        N2kHeader hdr = new N2kHeader(Priority, PGN, Destination, Source);
                        if ((hdr.PGN != PGN) || (hdr.PGNSource != Source) || (hdr.PGNDestination != Destination) || (hdr.PGNPriority != Priority)) {
                            N2kHeader hdr2 = new N2kHeader(Priority, PGN, Destination, Source);
                        }
                        Byte[] Bytes = new Byte[ByteCount];
                        for (int i = 0; i < ByteCount; i++) {
                            Bytes[i] = byte.Parse(elements[6 + i], System.Globalization.NumberStyles.HexNumber);
                        }
                        var N2kMsg = new N2kFrame { ms = 0, Data = Bytes, Header = hdr, TimeStamp = Date };
                        FrameCol.Frames.Add(N2kMsg);
                        if (ByteCount > LargestByteCount) LargestByteCount = ByteCount;
                    }
                    catch
                    {
                        LinesWithErrors++;
                        if (LinesWithErrors < 100) ErrorLines = ErrorLines + ((ErrorLines != "" ? ", " : "") + LineCount.ToString());
                    }
                    Console.WriteLine("{0} N2k Data Frames (or messages) read from file. \nThe largest frame (or message) contained {1} bytes. \n{2} lines were skipped, being lines {3}",
                                       FrameCol.Frames.Count, LargestByteCount, LinesWithErrors, ErrorLines);

                    // Now we serliaize the data
                    XmlSerializer mySerializer2 = new XmlSerializer(typeof(FrameCollection));
                    TextWriter writer2 = new StreamWriter(@"C:\temp\N2k\N2kFramesFromKees.xml");
                    mySerializer2.Serialize(writer2, FrameCol);
                    writer2.Close();

                }
            }
            catch (Exception e) {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return FrameCol;

        }
    }
    */
}


