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

using CANHandler;
using OpenSkipperApplication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CANDefinitions
{
    /// <summary>
    /// Provides a definition to apply to NMEA 2000 messages.
    /// </summary>
    public class PGNDefn : Defn
    {
        [XmlAttribute]
        public uint PGN { get; set; }
        public uint ByteLength { get; set; } // The expected length in bytes, used in joining of fast packets

        [XmlAttribute]
        public bool CanBeShortened { get; set; } // (Not in use) True if actual packets can be shorter than ByteLength

        [XmlIgnore]
        public bool HasMultipleDefinitions { get; set; } // Used to indicate that there are mutiple definitions with the same PGN number

        // Serialization
        public bool ShouldSerializeByteLength() { return ByteLength != 0; }
        public bool ShouldSerializeCanBeShortened() { return CanBeShortened == true; }
        public bool ShouldSerializeFields() { return (Fields != null) && (Fields.Length > 0); }

        // Field array. Serializer must be told what type the names correspond to.
        [XmlArray("Fields")]
        [XmlArrayItem("Field", typeof(N2kBinaryField))]
        [XmlArrayItem("NumericField", typeof(N2kNumericField))]
        [XmlArrayItem("DblField", typeof(N2kDblField))]
        [XmlArrayItem("UDblField", typeof(N2kUDblField))]
        [XmlArrayItem("IntField", typeof(N2kIntField))]
        [XmlArrayItem("UIntField", typeof(N2kUIntField))]
        [XmlArrayItem("InstanceField", typeof(N2kInstanceField))]
        [XmlArrayItem("EnumField", typeof(N2kEnumField))]
        [XmlArrayItem("ASCIIField", typeof(N2kASCIIField))]
        [XmlArrayItem("StringField", typeof(N2kStringField))]
        public N2kField[] Fields
        {
            get { return _Fields; }
            set { _Fields = value ?? new N2kField[0]; RebuildFieldDict(); } // Prevent null fields.
        }
        private N2kField[] _Fields;

        // Jan 22, 2018 - mlucas
        // Change the dictionary to be case insensitive to allow TryGetValue to fiend
        // a key regardless of the case.
        //private Dictionary<string, N2kField> _fieldDict = new Dictionary<string, N2kField> { };
        private Dictionary<string, N2kField> _fieldDict = new Dictionary<string, N2kField>(StringComparer.OrdinalIgnoreCase) { };

        // Creators. These ensure we never have null references.
        public PGNDefn()
        {
            Fields = new N2kField[0];
        }
        public PGNDefn(uint pgn, string name, string description,
                        uint dataLength, N2kField[] fields, uint byteLength)
        {
            PGN = pgn;
            Name = name;
            Description = description;
            ByteLength = dataLength;
            Fields = fields;
            ByteLength = byteLength;
        }

        // Overrides
        public override string FieldsString(Frame fmsg)
        {
            var msg = (N2kFrame)fmsg;
            string s = "";
            foreach (N2kField f in Fields)
            {
                // We ignore fields that have not been transmitted, eg see PGN 127250
                if (f.BitOffset + f.BitLength <= (msg.Data.Length) << 3)
                {
                    string fStr = f.ToString(msg.Data);
                    int end = fStr.IndexOf('\0');
                    if (end >= 0)
                    {
                        s = s + "  " + f.Name + "=" + fStr.Substring(0, end); //  +"{" + (f as Field).ToString(msg.Data) + "}" + "\n";
                    }
                    else
                    {
                        s = s + "  " + f.Name + "=" + fStr; //  +"{" + (f as Field).ToString(msg.Data) + "}" + "\n";
                    }
                }
            }
            return s;
        }
        public override string EnumFieldsString(Frame fmsg)
        {
            var msg = (N2kFrame)fmsg;
            string s = "";
            foreach (N2kField f in Fields)
            {
                // We ignore fields that have not been transmitted, eg see PGN 127250
                if ((f is N2kEnumField) && (f.BitOffset + f.BitLength <= (msg.Data.Length) << 3))
                {
                    s = s + "  " + f.Name + "=" + f.ToString(msg.Data); //  +"{" + (f as Field).ToString(msg.Data) + "}" + "\n";
                }
            }
            return s;
        }
        public override string InstanceFieldsString(Frame fmsg)
        {
            var msg = (N2kFrame)fmsg;
            string s = "";
            foreach (N2kField f in Fields)
            {
                // We ignore fields that have not been transmitted, eg see PGN 127250
                if ((f is N2kInstanceField) && (f.BitOffset + f.BitLength <= (msg.Data.Length) << 3))
                {
                    s = s + "  " + f.Name + "=" + f.ToString(msg.Data); //  +"{" + (f as Field).ToString(msg.Data) + "}" + "\n";
                }
            }
            return s;
        }
        public override string ToString(Frame fmsg)
        {
            var msg = (N2kFrame)fmsg;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(((N2kHeader)msg.Header).PGN.ToString() + ": " + (Name ?? ""));
            sb.AppendFormat("Source = {0}\n", msg.Header.PGNSource);
            sb.AppendFormat("Destination = {0}\n", msg.Header.PGNDestination);
            sb.AppendFormat("Priority = {0}\n", msg.Header.PGNPriority);
            sb.AppendFormat("Expected Length = {0} bytes\n", ByteLength.ToString());
            sb.AppendFormat("Actual Length = {0} bytes\n", msg.Data.Length);

            foreach (N2kField f in Fields)
            {
                // We ignore fields that have not been transmitted, eg see PGN 127250
                if (f.BitOffset + f.BitLength <= (msg.Data.Length) << 3)
                {
                    sb.AppendFormat("     {0} = {1}\n", f.Name, f.ToString(msg.Data));
                }
            }

            return sb.ToString();
        }
        public override string ToDebugString(Frame fmsg)
        {
            var msg = (N2kFrame)fmsg;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(((N2kHeader)msg.Header).PGN.ToString() + ": " + (Name ?? ""));
            sb.AppendFormat("Source          = {0}\n", msg.Header.PGNSource);
            sb.AppendFormat("Destination     = {0}\n", msg.Header.PGNDestination);
            sb.AppendFormat("Priority        = {0}\n", msg.Header.PGNPriority);
            sb.AppendFormat("Expected Length = {0} bytes\n", ByteLength.ToString());
            sb.AppendFormat("Actual Length   = {0} bytes\n", msg.Data.Length);

            if ((msg.Defn == null) || (msg.Defn is PGNDefnUnknown))
            {
                // We output each byte with its binary representation
                for (int i = 0; i < msg.Data.Length; i++)
                {
                    sb.AppendFormat("{0} Byte {1}", Tools.Dump(msg.Data, 8 * i, 8), i.ToString());
                }
            }
            else
            {
                foreach (N2kField f in Fields)
                {
                    var BitOffset = f.BitOffset;
                    // We ignore fields that have not been transmitted, eg see PGN 127250
                    if (f.BitOffset + f.BitLength <= (msg.Data.Length) << 3)
                    {
                        sb.Append(f.DebugString(msg.Data));
                        sb.AppendFormat("  {0} = {1}\n", f.Name, f.ToString(msg.Data));
                    }
                }
            }

            return sb.ToString();
        }
        public override int CompareTo(object obj)
        {
            PGNDefn oDefn = obj as PGNDefn;
            return (oDefn == null) ? -1 : this.PGN.CompareTo(oDefn.PGN);
        }

        // Field operations
        public void AddField(N2kField f)
        {
            N2kField[] fields = Fields;
            int length = fields.Length;
            Array.Resize(ref fields, length + 1);
            Fields = fields;
            Fields[length] = f;
            RebuildFieldDict();
        }
        public void SortFields()
        {
            // This sorts the fields into ascending order
            Array.Sort(Fields);
        }
        public string ValidateFields(bool Fix)
        {
            // This assumes the fields have been sorted. It checks that the PGN byte length corresponds with the end of the
            // last field.
            //foreach (var f in Fields) {
            //    Fields2.Add(f);
            //}
            if (Fields.Length == 0)
            {
                if (ByteLength == 0)
                {
                    return "";
                }
                else
                {
                    if (Fix)
                    {
                        string s = "PGN " + PGN + " has a length (" + ByteLength / 8.0 + " 8-byte frames, " + ByteLength + " bytes, " + ByteLength * 8 + " bits) that differs from the length (0) of the fields. ByteLength set to 0.";
                        ByteLength = 0;
                        return s;
                    }
                    else
                    {
                        return "PGN " + PGN + " has a length (" + ByteLength / 8.0 + " 8-byte frames, " + ByteLength + " bytes, " + ByteLength * 8 + " bits) that differs from the length (0) of the fields.";
                    }
                }
            }
            else
            {
                int l = (Fields[Fields.Length - 1].BitOffset + Fields[Fields.Length - 1].BitLength);
                if (l != ByteLength * 8)
                {
                    if (Fix)
                    {
                        string s = "PGN " + PGN + " has a length (" + ByteLength / 8.0 + " 8-byte frames, " + ByteLength + " bytes, " + ByteLength * 8 + " bits) that differs from the total length (" + l / 8.0 + " bytes, " + l + " bits) of the fields. Bytelength set to ";
                        ByteLength = (uint)((l + 7) / 8);   // round up if the length is 8 or greater   
                        if (ByteLength >= 8)
                        {
                            ByteLength = (uint)((ByteLength + 7) / 8);    // A multiple of 8 byte packets
                        }
                        // Note: We may need to add a Reserved field to get to a multiple of 8 bytes
                        return s + ByteLength;
                    }
                    else
                    {
                        return "PGN " + PGN + " has a length (" + ByteLength / 8.0 + " 8-byte frames, " + ByteLength + " bytes, " + ByteLength * 8 + " bits) that differs from the total length (" + l / 8.0 + " bytes, " + l + " bits) of the fields.";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        public N2kField GetFieldByName(string fieldName)
        {
            N2kField f;
            _fieldDict.TryGetValue(fieldName, out f);
            return f;
        }

        // Jan 24, 2018 - mlucas
        // Adding support for creating a new frame manually
        public N2kFrame CreateMsg(byte priority, byte source, byte destination, PGNDefn pgnDef)
        {
            var data = new byte[pgnDef.ByteLength];

            N2kHeader header = new N2kHeader(priority, Convert.ToInt32(PGN), destination, source);
            N2kFrame frame = new N2kFrame(header, data, DateTime.UtcNow);
            frame.Defn = pgnDef;

            return frame;
        }
        public N2kFrame CreateMsg(int priority, int source, int destination)
        {
            N2kHeader header = new N2kHeader(priority.ToByte(), Convert.ToInt32(PGN), destination.ToByte(), source.ToByte());
            N2kFrame frame = new N2kFrame(header, null, DateTime.UtcNow);
            frame.Defn = this;

            return frame;
        }

        // Rebuilds field dictionary for GetFieldByName
        private void RebuildFieldDict()
        {
            _fieldDict.Clear();
            foreach (N2kField f in _Fields)
                _fieldDict[f.Name] = f;
        }
    }
    public class PGNDefnUnknown : PGNDefn
    {
        public PGNDefnUnknown()
        // This PGn definition is used whenever we receive an unrecognised PGN
        // It has a single field which does a binary dump of the data
        {
            PGN = 0;
            ByteLength = 0;
            Name = "Unknown PGN";
            Fields = new N2kField[1] { new N2kBinaryField { BitLength = 0, BitOffset = 0, Description = "Unknown", Name = "Unknown" } };
        }
    }

    /// <summary>
    /// Holds full set of NMEA 2000 message definitions, with associated access methods
    /// </summary>
    public class PGNDefnCollection
    {
        public const string DefDefnFile = "PGNDefns.N2kDfn.xml";
        [XmlAttribute]
        public string Version { get; set; }

        // We serlialise a date. But, if DateSpecified is false, the Data is not serialised
        public string Date { get; set; }
        [XmlIgnore]
        public bool DateSpecified { get { return Date != ""; } }

        // We serlialise an optional Comment. But, if DateSpecified is false, the Data is not serialised
        public string Comment { get; set; }
        [XmlIgnore]
        public bool CommentSpecified { get { return Comment != ""; } }

        // We serialize an optional CreatorCode, to identify who authored the definition file
        public string CreatorCode { get; set; }
        [XmlIgnore]
        public bool CreatorCodeSpecified { get { return CreatorCode != ""; } }

        // The file we loaded the definitions from
        [XmlIgnore]
        [ReadOnly(true)]
        public string FileName { get; set; }
        [XmlIgnore]
        [ReadOnly(true)]
        // The file we loaded the definitions from
        public DateTime FileDateTime { get; set; }

        // The type of file we loaded the definitions from, being either native or Kees.
        [XmlIgnore]
        public FileTypeEnum FileType { get; set; }

        // The array of pgnDefns
        [XmlIgnore]
        private PGNDefn[] pgnDefns;

        // Dictionary mapping each PGN to its associated definition
        [XmlIgnore]
        public Dictionary<uint, int> PGNDictionary { get; set; }

        // Accessor to array of pgn definitions, which ensures non-null definition set and up-to-date internal structures
        [XmlArray("PGNDefns")]
        [XmlArrayItem("PGNDefn", typeof(PGNDefn))]
        public PGNDefn[] PGNDefns
        {
            get { return pgnDefns; }
            set
            {
                pgnDefns = value ?? new PGNDefn[0]; // Don't allow null for PGNDefns.
                SortAndBuildInternalStructures(); // Sort PGNs, sort the fields, and build a dictionary for fast lookup of PGNs
            }
        }

        // This PGNDefn handles the decoding of all unknown PGNs
        PGNDefnUnknown UnknownPGNDefn = new PGNDefnUnknown();

        // Constructor
        public PGNDefnCollection()
        {
            // Make sure a new collection has no null references.
            PGNDictionary = new Dictionary<uint, int> { };
            PGNDefns = new PGNDefn[0];
            FileName = "";
            FileDateTime = new DateTime();
        }

        public bool IsChanged(string newFileName)
        {
            newFileName = CommonRoutines.ResolveFileNameIfEmpty(newFileName, DefDefnFile);
            return ((newFileName != FileName) || (FileName == "") || (File.GetLastWriteTime(FileName) != FileDateTime));
        }

        // Given a (possibly partially) completed PGN message, 
        // described by a PGN number, and the apparent 'byte length'
        // for the message (which is only relevant if the message is a 
        // multiple packet message), return the
        // PGNDefn that allows this message to be decoded
        public PGNDefn GetPGNDefn(N2kFrame msg)
        {
            uint PGN = (uint)msg.Header.PGN;
            int pgnIndex;
            if (!PGNDictionary.TryGetValue(PGN, out pgnIndex))
            {
                // No matching PGN definition; return UnknownPGNDefn
                return UnknownPGNDefn;
            }
            else
            {
                PGNDefn pgn = PGNDefns[pgnIndex];
                // Is this a multiple definition PGN? (If so, it must be a fast packet PGN?)
                if (!pgn.HasMultipleDefinitions)
                {
                    // Only one matching definition available; return it
                    //                    int iBytesTransmitted = pgn.ByteLength <= 8 ? msg.Data.Length : msg.Data[1];
                    return PGNDefns[pgnIndex];
                }
                else
                {
                    // Find the matching definition
                    while ((pgnIndex < PGNDefns.Length) && (PGNDefns[pgnIndex].PGN == pgn.PGN))
                    {
                        if (PGNDefns[pgnIndex].ByteLength == msg.Data[1])
                        {
                            return PGNDefns[pgnIndex];
                        }
                        else
                        {
                            // Find any PGN that is not too long, just on case we cannot find any with an exact match
                            if (pgn.ByteLength < msg.Data[1]) pgn = PGNDefns[pgnIndex];
                            pgnIndex++;
                        }
                    }
                    return pgn;
                }
            }
        }

        private void SortAndBuildInternalStructures()
        {
            if (PGNDefns != null)
            {
                Array.Sort(pgnDefns);

                // Identify any duplicate PGNs in the data
                for (int i = 0; i < pgnDefns.Length; i++)
                {
                    pgnDefns[i].HasMultipleDefinitions = false;
                }
                for (int i = 0; i < pgnDefns.Length - 1; i++)
                {
                    if (pgnDefns[i].PGN == pgnDefns[i + 1].PGN)
                    {
                        pgnDefns[i].HasMultipleDefinitions = true;
                        pgnDefns[i + 1].HasMultipleDefinitions = true;
                    }
                }

                // Now (re)build up a dictionary of the PGN indices, recording the *first* instance of each PGN
                PGNDictionary.Clear();
                for (int i = pgnDefns.Length - 1; i >= 0; i--)
                {
                    PGNDictionary[pgnDefns[i].PGN] = i;
                }

                // Finally we sort all the fields
                foreach (PGNDefn p in PGNDefns)
                {
                    p.SortFields();
                }
                // ReBuildPGNDictionary();
            }
        }

        // This should be called whenever any editting has been done to the PGNs
        // It returns a string of errors 
        public string SortAndValidate(bool Fix)
        {
            SortAndBuildInternalStructures();
            string Errors = "";
            foreach (PGNDefn p in PGNDefns)
            {
                string err = p.ValidateFields(Fix);
                if (err != "")
                {
                    if (Errors == "")
                    {
                        Errors = err;
                    }
                    else
                    {
                        if (!Errors.EndsWith("\n")) Errors += "\n";
                        Errors += err;
                    }
                }
            }
            return Errors;
        }

        public void AddPGNDefn(PGNDefn pgn)
        {
            if (PGNDefns == null)
            {
                PGNDefns = new PGNDefn[1] { pgn };
            }
            else
            {
                Array.Resize(ref pgnDefns, PGNDefns.Length + 1);
                PGNDefns[PGNDefns.Length - 1] = pgn;
            }
            SortAndBuildInternalStructures();
        }

        public void AddPGNDefn()
        {
            // This should be called by a manual editor to add a blank PGN; it does NOT call SortAndBuildInternalStructures
            if (PGNDefns == null)
            {
                PGNDefns = new PGNDefn[1] { new PGNDefn() { Name = "New PGN" } };
            }
            else
            {
                Array.Resize(ref pgnDefns, PGNDefns.Length + 1);
                PGNDefns[PGNDefns.Length - 1] = new PGNDefn() { Name = "New PGN" };
            }
        }

        // Serialization
        private static XmlFileSerializer<PGNDefnCollection> XmlFileSerializer = new XmlFileSerializer<PGNDefnCollection>();

        // File IO
        public static PGNDefnCollection LoadFromFile(string fileName)
        {
            fileName = CommonRoutines.ResolveFileNameIfEmpty(fileName, DefDefnFile);
            if (fileName == "") return LoadInternal();

            PGNDefnCollection pgnDefnCol = XmlFileSerializer.Deserialize(fileName);
            if (pgnDefnCol != null)
            {
                pgnDefnCol.FileName = fileName;
                pgnDefnCol.FileDateTime = File.GetLastWriteTime(fileName);
                pgnDefnCol.FileType = FileTypeEnum.NativeXMLFile;
                return pgnDefnCol;
            }

            return LoadInternal();
        }
        public static PGNDefnCollection LoadInternal()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("OpenSkipperApplication.Resources.PGNDefns.N2kDfn.xml");

            PGNDefnCollection newDefns = XmlFileSerializer.Deserialize(stream);
            newDefns.FileType = FileTypeEnum.Internal;
            return newDefns;
        }
        public bool SaveToFile(string fileName)
        {
            using (new WaitCursor())
            {
                if (XmlFileSerializer.Serialize(fileName, this))
                {
                    FileName = fileName;
                    FileType = FileTypeEnum.NativeXMLFile;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

} // namespace
