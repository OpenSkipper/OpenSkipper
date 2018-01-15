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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parameters;
using CANHandler;
using OpenSkipperApplication;
using OpenSkipperApplication.Properties;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using CANStreams;
using CANDefinitions;

/// <summary>
/// 
/// </summary>
namespace CANDefinitions
{
    /// <summary>
    /// Holds the application-wide definition collections
    /// </summary>
    public static class Definitions
    {
        // Public static definition collections, modifyable externally only via the Load__Defns(fileName) functions. 
        public static PGNDefnCollection PGNDefnCol { get; private set; }
        public static N0183DefnCollection N0183DefnCol { get; private set; }
        public static ParameterCollection ParamCol { get; private set; }
        public static AISDefnCollection AISDefnCol { get; private set; }

        // Some elements of the program (i.e the extender) are interested in when the parameters are reloaded, in order to rebind controls etc.
        public static event Action ParametersReloaded;

        // Public methods for changing the definition collections in use, will revert to internal definitions if provided with a fileName it cannot load from
        // Returns true, if new file has been loaded
        public static bool LoadPGNDefns(string fileName)
        {
            if ((PGNDefnCol != null) && !PGNDefnCol.IsChanged(fileName)) return false;
            PGNDefnCol = PGNDefnCollection.LoadFromFile(fileName);
            Settings.Default.N2kPath = fileName;
            Settings.Default.Save();
            return true;
        }
        public static bool LoadN0183Defns(string fileName)
        {
            if ((N0183DefnCol != null) && !N0183DefnCol.IsChanged(fileName)) return false;
            N0183DefnCol = N0183DefnCollection.LoadFromFile(fileName);
            Settings.Default.N0183Path = fileName;
            Settings.Default.Save();
            return true;
        }
        public static bool LoadParameters(string fileName)
        {
            if ((ParamCol != null) && !ParamCol.IsChanged(fileName)) return false;
            ParamCol = ParameterCollection.LoadFromFile(fileName);

            if (ParametersReloaded != null)
                ParametersReloaded();

            Settings.Default.ParametersPath = fileName;
            Settings.Default.Save();
            return true;
        }
        public static bool LoadAISDefns(string fileName)
        {
            if ((AISDefnCol != null) && !AISDefnCol.IsChanged(fileName)) return false;
            AISDefnCol = AISDefnCollection.LoadFromFile(fileName);
            Settings.Default.AISPath = fileName;
            Settings.Default.Save();
            return true;
        }
    }

    /// <summary>
    /// Provides a common base for all definitions, allowing them to be compared and databound to
    /// </summary>
    public abstract class Defn : IComparable, INotifyPropertyChanged
    {
        // Public
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        [XmlAttribute]
        public string Description
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
                NotifyPropertyChanged("Description");
            }
        }
        
        // Private
        protected string _name = "";
        protected string _desc = "";

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        // Public methods
        // Serialization
        public bool ShouldSerializeName() { return Name != ""; }
        public bool ShouldSerializeDescription() { return Description != ""; }

        // These methods are used to display data about the definition
        public abstract string FieldsString(Frame fmsg);    // Gives the names and values of each field
        public abstract string EnumFieldsString(Frame fmsg);// Gives the names and values of the enum fields
        public virtual string InstanceFieldsString(Frame fmsg) { return ""; }// Gives the names and values of the inctance fields
        public abstract string ToString(Frame fmsg);        // Returns complete information about the frame across multiple lines, including header and field information
        public abstract string ToDebugString(Frame fmsg);   // Returns complete information about the frame across multiple lines, including header and field debug/detailed information
        public abstract int CompareTo(object obj);          // Implements IComparable
    }

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
            set { _Fields = value ?? new N2kField[0]; RebuildFieldDict();  } // Prevent null fields.
        }
        private N2kField[] _Fields;
        private Dictionary<string, N2kField> _fieldDict = new Dictionary<string, N2kField> { };

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
                    string fStr=f.ToString(msg.Data);
                    int end = fStr.IndexOf('\0');
                    if (end >= 0)
                    {
                        s = s + "  " + f.Name + "=" + fStr.Substring(0,end); //  +"{" + (f as Field).ToString(msg.Data) + "}" + "\n";
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
            string s = ((N2kHeader)msg.Header).PGN.ToString() + ": " + (Name ?? "") + "\nSource=" + msg.Header.PGNSource + "\nDestination=" + msg.Header.PGNDestination + "\nPriority=" + msg.Header.PGNPriority + "\nExpected Length=" + ByteLength.ToString() + " bytes" + "\nActual Length=" + msg.Data.Length + " bytes\n";

            foreach (N2kField f in Fields)
            {
                // We ignore fields that have not been transmitted, eg see PGN 127250
                if (f.BitOffset + f.BitLength <= (msg.Data.Length) << 3)
                {
                    s = s + "  " + f.Name + "=" + f.ToString(msg.Data) + "\n"; //  +"{" + (f as Field).ToString(msg.Data) + "}" + "\n";
                }
            }
            return s;
        }
        public override string ToDebugString(Frame fmsg)
        {
            var msg = (N2kFrame)fmsg;
            string s = ((N2kHeader)msg.Header).PGN.ToString() + ": " + (Name ?? "") + "\nSource=" + msg.Header.PGNSource + "\nDestination=" + msg.Header.PGNDestination + "\nPriority=" + msg.Header.PGNPriority + "\nExpected Length=" + ByteLength.ToString() + " bytes" + "\nActual Length=" + msg.Data.Length + " bytes\n";
            // string s = ((N2kHeader)msg.Header).PGN.ToString() + ": " + (Name ?? "") + ", Source=" + msg.Header.PGNSource + ", Destination=" + msg.Header.PGNDestination + ", Priority=" + msg.Header.PGNPriority + ", Length=" + ByteLength.ToString() + " bytes\n";
            if ((msg.Defn == null) || (msg.Defn is PGNDefnUnknown)) {
                // We output each byte with its binary representation
                for (int i=0; i<msg.Data.Length; i++) {
                    s = s + Tools.Dump(msg.Data, 8*i, 8);
                    s = s+ "  " + "Byte " + i.ToString() + "\n";
                }
            } else {
                foreach (N2kField f in Fields)
                {
                    var BitOffset = f.BitOffset;
                    // We ignore fields that have not been transmitted, eg see PGN 127250
                    if (f.BitOffset + f.BitLength <= (msg.Data.Length) << 3)
                    {
                        s = s + f.DebugString(msg.Data);
                        s = s + "  " + f.Name + "=" + f.ToString(msg.Data) + "\n"; //  +"{" + (f as Field).ToString(msg.Data) + "}" + "\n";
                    }
                }
            }
            return s;
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

    /// <summary>
    /// Provides a definition to apply to NMEA 0183 messages.
    /// </summary>
    public class N0183Defn : Defn
    {
        [XmlAttribute]
        public string Code { get; set; } // The type code the definition corresponds to, i.e. 'ALM', 'GLL'

        // The defined fields, note the XML serializer must be explicitly told what types to expect
        [XmlArray("Fields")]
        [XmlArrayItem("Field", typeof(N0183TextField))]
        [XmlArrayItem("NumericField", typeof(N0183NumericField))]
        [XmlArrayItem("LatitudeField", typeof(N0183LatitudeField))]
        [XmlArrayItem("LongitudeField", typeof(N0183LongitudeField))]
        [XmlArrayItem("TimeField", typeof(N0183TimeField))]
        [XmlArrayItem("EnumField", typeof(N0183EnumField))]
        [XmlArrayItem("LeftRightField", typeof(N0183LeftRightField))]
        public N0183Field[] Fields
        {
            get { return _Fields; }
            set { _Fields = value ?? new N0183Field[0]; RebuildFieldDict(); } // Prevent null Fields
        }
        private N0183Field[] _Fields;
        private Dictionary<string, N0183Field> _fieldDict = new Dictionary<string, N0183Field> { };

        // Constructors. Ensure no null references.
        private N0183Defn()
        {
            // For XML serializer
            Fields = new N0183Field[0];
        }
        public N0183Defn(string code)
        {
            Code = code;
            Fields = new N0183Field[0];
        }

        // Methods for interacting with field array
        public void SortFields()
        {
            Array.Sort(Fields);
        }
        public void AddField(N0183Field f)
        {
            N0183Field[] temp = Fields;
            Array.Resize(ref temp, temp.Length + 1);
            _Fields = temp;
            _Fields[Fields.Length - 1] = f;
            RebuildFieldDict();
        }
        public void DeleteField(int index)
        {
            for (int i = index; i + 1 < _Fields.Length; i++)
                _Fields[i] = _Fields[i + 1];
            Array.Resize(ref _Fields, _Fields.Length - 1);
        }

        // Methods for displaying data, refer to base Defn class
        public override string FieldsString(Frame fmsg)
        {
            var msg = (N0183Frame)fmsg;
            string s = "";
            foreach (N0183Field f in Fields)
            {
                s = s + " " + f.Name + "=" + f.ToString(msg.Segments);
            }
            return s;
        }
        public override string EnumFieldsString(Frame fmsg)
        {
            var msg = (N0183Frame)fmsg;
            string s = "";
            foreach (N0183Field f in Fields)
            {
                if (f is N0183EnumField)
                    s += " " + f.Name + "=" + f.ToString(msg.Segments);
            }
            return s;
        }
        public override string ToString(Frame fmsg)
        {
            var msg = (N0183Frame)fmsg;
            string s = msg.Header.TypeCode + ": " + Name + "\nSource=" + msg.Header.TalkerID + "\nLength=" + msg.Length + " bytes\n";
            foreach (N0183Field f in Fields)
            {
                s += " " + f.Name + "=" + f.ToString(msg.Segments) + "\n";
            }
            return s;
        }
        public override string ToDebugString(Frame fmsg)
        {
            // Loop over fields, printing values
            var msg = (N0183Frame)fmsg;
            string s = msg.FullMessage + "\n\n" + (Name ?? "") + ", TalkerID=" + msg.Header.TalkerID + ", Length=" + msg.Length + " bytes\n";
            foreach (N0183Field f in Fields)
            {
                s = s + "  " + ((f.SegmentIndex < msg.Segments.Length) ? msg.Segments[f.SegmentIndex].PadRight(10) : "N/A");
                s = s + "  " + f.Name + " = " + f.ToString(msg.Segments) + "\n";
            }
            return s;
        }
        public override int CompareTo(object obj)
        {
            N0183Defn oDefn = obj as N0183Defn;
            return (oDefn == null) ? -1 : this.Code.CompareTo(oDefn.Code);
        }

        public N0183Field GetFieldByName(string fieldName)
        {
            N0183Field f;
            _fieldDict.TryGetValue(fieldName, out f);
            return f;
        }
        // Rebuilds field dictionary for GetFieldByName
        private void RebuildFieldDict()
        {
            _fieldDict.Clear();
            foreach (N0183Field f in _Fields)
                _fieldDict[f.Name] = f;
        }
    }
    public class UnknownN0183Defn : N0183Defn
    {
        public UnknownN0183Defn()
            : base("???")
        {
            Name = "Unknown TypeCode";
            Fields = new N0183Field[0];
        }
    }

    /// <summary>
    /// Holds full set of NMEA 0183 message definitions, with associated access methods
    /// </summary>
    public class N0183DefnCollection
    {
        public const string DefDefnFile = "N0183Defns.N0183Dfn.xml";
        [XmlAttribute]
        public string Version { get; set; }

        // We serlialise a date. But, if DateSpecified is false, the Data is not serialised
        public string Date { get; set; }
        public bool ShouldSerializeDate() { return Date != ""; }

        // We serlialise an optional Comment. But, if DateSpecified is false, the Data is not serialised
        public string Comment { get; set; }
        public bool ShouldSerializeComment() { return Comment != ""; }

        // We serialize creator code
        public string CreatorCode { get; set; }
        public bool ShouldSerializeCreatorCode() { return CreatorCode != ""; }

        // The file we loaded the definitions from
        [XmlIgnore]
        [ReadOnly(true)]
        public string FileName { get; set; }
        [XmlIgnore]
        [ReadOnly(true)]
        // The file we loaded the definitions from
        public DateTime FileDateTime { get; set; }

        // The type of the file, internal or native ml
        [XmlIgnore]
        public FileTypeEnum FileType { get; set; }

        [XmlIgnore]
        private Dictionary<string, int> N0183Dictionary { get; set; }
        private Dictionary<string, int> _largeTypeCodes;

        // Array of N0183 definitions, note that the xml serializer must be explicitly told what types to expect 
        [XmlArray("N0183Defns")]
        [XmlArrayItem("N0183Defn", typeof(N0183Defn))]
        public N0183Defn[] N0183Defns
        {
            get { return _N0183Defns; }
            set
            {
                _N0183Defns = value ?? new N0183Defn[0]; // Don't allow null references
                SortAndBuildInternalStructures(); // Sort PGNs, sort the fields, and build a dictionary for fast lookup of PGNs
            }
        }
        private N0183Defn[] _N0183Defns;

        // Constructor
        public N0183DefnCollection()
        {
            // Ensure no null references
            _largeTypeCodes = new Dictionary<string, int> { };
            N0183Dictionary = new Dictionary<string, int> { };
            N0183Defns = new N0183Defn[0];
            FileName = "";
            FileDateTime = new DateTime();
        }

        public bool IsChanged(string newFileName)
        {
            newFileName = CommonRoutines.ResolveFileNameIfEmpty(newFileName, DefDefnFile);
            return ((newFileName != FileName) || (FileName == "") || (File.GetLastWriteTime(FileName) != FileDateTime));
        }

        // Methods for accessing definitions
        public void AddN0183Defn()
        {
            Array.Resize(ref _N0183Defns, _N0183Defns.Length + 1);
            _N0183Defns[_N0183Defns.Length - 1] = new N0183Defn("NEW") { Name = "New", Description = "New" };
            SortAndBuildInternalStructures();
        }
        public N0183Defn GetN0183Defn(N0183Frame msg)
        {
            string headerText = msg.Header.HeaderText;
            int defnIndex;
            if (_largeTypeCodes.TryGetValue(headerText.Substring(1), out defnIndex))
            {
                msg.Header = new N0183Header(headerText.Substring(0, 1), "", headerText.Substring(1));
                return N0183Defns[defnIndex];
            }
            else
            {
                if (N0183Dictionary.TryGetValue(msg.Header.TypeCode, out defnIndex))
                {
                    return N0183Defns[defnIndex];
                }
                else
                {
                    return new UnknownN0183Defn();
                }
            }
        }
        private void SortAndBuildInternalStructures()
        {
            if (N0183Defns != null)
            {
              //  Array.Sort(N0183Defns);

                // Now (re)build up a dictionary of the PGN indices, recording the *first* instance of each PGN
                N0183Dictionary.Clear();
                for (int i = N0183Defns.Length - 1; i >= 0; i--)
                {
                    string typeCode = N0183Defns[i].Code;
                    if (typeCode.Length > 3)
                    {
                        _largeTypeCodes[typeCode] = i;
                    }
                    else
                    {
                        N0183Dictionary[typeCode] = i;
                    }
                }
                // Finally we sort all the fields
            //    foreach (N0183Defn p in N0183Defns)
           //     {
         //           p.SortFields();
         //       }
            }
        }

        // Serialization
        private static XmlFileSerializer<N0183DefnCollection> XmlFileSerializer = new XmlFileSerializer<N0183DefnCollection>();

        // File IO
        public static N0183DefnCollection LoadFromFile(string fileName)
        {
            fileName = CommonRoutines.ResolveFileNameIfEmpty(fileName, DefDefnFile);
            if (fileName == "") return LoadInternal();

            N0183DefnCollection n0183DefnCol = XmlFileSerializer.Deserialize(fileName);
            if (n0183DefnCol != null)
            {
                n0183DefnCol.FileName = fileName;
                n0183DefnCol.FileType = FileTypeEnum.NativeXMLFile;
                return n0183DefnCol;
            }

            return LoadInternal();
        }
        public static N0183DefnCollection LoadInternal()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("OpenSkipperApplication.Resources.N0183Defns.N0183Dfn.xml");

            N0183DefnCollection newDefns = XmlFileSerializer.Deserialize(stream);
            newDefns.FileType = FileTypeEnum.Internal; // No filename set, instead we set type to internal
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

    /// <summary>
    /// Provides a definition to apply to AIS messages.
    /// </summary>
    public class AISDefn : Defn
    {
        // The message ID that this definition applies to
        [XmlAttribute]
        public int MessageID
        {
            get
            {
                return _messageID;
            }
            set
            {
                _messageID = value;
                NotifyPropertyChanged("MessageID");
            }
        }

        // The defined fields, note the XML serializer must be explicitly told what types to expect
        [XmlArray("Fields")]
        [XmlArrayItem("SignedField", typeof(AISSigned))]
        [XmlArrayItem("UnsignedField", typeof(AISUnsigned))]
        [XmlArrayItem("SignedDoubleField", typeof(AISSignedDouble))]
        [XmlArrayItem("UnsignedDoubleField", typeof(AISUnsignedDouble))]
        [XmlArrayItem("StringField", typeof(AISString))]
        [XmlArrayItem("RateOfTurnField", typeof(AISRoT))]
        [XmlArrayItem("EnumField", typeof(AISEnum))]
        public BindingList<AISField> Fields
        {
            get
            {
                return _fields;
            }
            set
            {
                _fields = value ?? new BindingList<AISField> { };
                RebuildFieldDict();
            }
        }

        // Private vars
        private BindingList<AISField> _fields;
        private Dictionary<string, AISField> _fieldDict;
        private int _messageID;

        // Constructors
        public AISDefn()
        {
            _fieldDict = new Dictionary<string, AISField> { };
            _fields = new BindingList<AISField> { };
            _fields.ListChanged += new ListChangedEventHandler(_fields_ListChanged);
        }

        // Methods for displaying data, refer to base Defn class
        public override string EnumFieldsString(Frame fmsg)
        {
            AISFrame aisMsg = (AISFrame)fmsg;
            string s = "";
            foreach (AISField f in Fields)
                if (f is AISEnum)
                    s += f.Name + "=" + f.ToString(aisMsg.AISData);

            return s;
        }
        public override string FieldsString(Frame fmsg)
        {
            AISFrame aisMsg = (AISFrame)fmsg;
            string s = "";
            foreach (AISField f in Fields)
                s += f.Name + "=" + f.ToString(aisMsg.AISData);

            return s;
        }
        public override string ToString(Frame fmsg)
        {
            AISFrame aisMsg = (AISFrame)fmsg;
            string s = "";
            foreach (AISField f in Fields)
                s += f.Name + " = " + f.ToString(aisMsg.AISData) + "\n";
            
            return s;
        }
        public override string ToDebugString(Frame fmsg)
        {
            AISFrame aisMsg = (AISFrame)fmsg;
            string bitstr = "";
            foreach (byte b in aisMsg.AISData.AISBytes)
                bitstr += Convert.ToString(b, 2).PadLeft(8, '0');
            string retstr = "";

            foreach (AISField f in Fields)
            {
                string fbitstr = (f.BitOffset < bitstr.Length) ? bitstr.Substring(f.BitOffset, Math.Min(f.BitLength, bitstr.Length - f.BitOffset)).PadRight(f.BitLength, 'x') : new string('x', f.BitLength);
                retstr += fbitstr + " -> " + f.Name + " = " + f.ToString(aisMsg.AISData) + "\n";
            }
            return retstr;
        }
        public override int CompareTo(object obj)
        {
            AISDefn oDefn = obj as AISDefn;
            return (oDefn == null) ? -1 : this.MessageID.CompareTo(oDefn.MessageID);
        }

        private void _fields_ListChanged(object sender, ListChangedEventArgs e)
        {
            RebuildFieldDict();
        }
        public AISField GetFieldByName(string fieldName)
        {
            AISField f;
            if (_fieldDict.TryGetValue(fieldName, out f))
                return f;
            else
                return null;
        }
        // Rebuilds field dictionary for GetFieldByName
        private void RebuildFieldDict()
        {
            _fieldDict.Clear();
            foreach (AISField field in _fields)
                _fieldDict[field.Name] = field;
        }
    }
    public class UnknownAISDefn : AISDefn
    {
        public UnknownAISDefn()
        {
            Name = "Unknown MessageID";
            Fields = new BindingList<AISField> { };
        }
    }

    /// <summary>
    /// Holds full set of AIS message definitions, with associated access methods
    /// </summary>
    public class AISDefnCollection
    {
        public const string DefDefnFile="AIS.AISDfn.xml";
        // The filename of the file the definitions were loaded from
        // Internal definitions use "" for the filename
        [XmlIgnore]
        public string FileName { get; set; }
        [XmlIgnore]
        [ReadOnly(true)]
        // The file we loaded the definitions from
        public DateTime FileDateTime { get; set; }

        // The type of the file, internal or native xml
        [XmlIgnore]
        public FileTypeEnum FileType { get; set; }

        // The list of all AIS definitions, can be bound to
        public BindingList<AISDefn> AisDefns
        {
            get
            {
                return _aisDefns;
            }
            set
            {
                _aisDefns = value ?? new BindingList<AISDefn> { };
                RebuildDict();
            }
        }

        // Private vars
        private BindingList<AISDefn> _aisDefns;
        private Dictionary<int, AISDefn> _aisDictionary;

        // Constructors
        public AISDefnCollection()
        {
            _aisDictionary = new Dictionary<int, AISDefn> { };
            _aisDefns = new BindingList<AISDefn> { };
            _aisDefns.ListChanged += new ListChangedEventHandler(_aisDefns_ListChanged);
            FileName = "";
            FileDateTime = new DateTime();
        }

        public bool IsChanged(string newFileName)
        {
            newFileName = CommonRoutines.ResolveFileNameIfEmpty(newFileName, DefDefnFile);
            return ((newFileName != FileName) || (FileName == "") || (File.GetLastWriteTime(FileName) != FileDateTime));
        }

        // Methods
        public AISDefn GetAISDefn(AISFrame msg)
        {
            AISDefn defn;
            if (_aisDictionary.TryGetValue(msg.Header.MessageID, out defn))
                return defn;
            else
                return new UnknownAISDefn();
        }
        private void _aisDefns_ListChanged(object sender, ListChangedEventArgs e)
        {
            RebuildDict();
        }
        private void RebuildDict()
        {
            _aisDictionary.Clear();
            foreach (AISDefn aisDefn in _aisDefns)
                _aisDictionary.Add(aisDefn.MessageID, aisDefn);
        }

        // Serialization
        private static XmlFileSerializer<AISDefnCollection> XmlFileSerializer = new XmlFileSerializer<AISDefnCollection>();

        // File IO
        public static AISDefnCollection LoadFromFile(string fileName)
        {
            if (fileName == "") return LoadInternal();

            AISDefnCollection aisDefnCol = XmlFileSerializer.Deserialize(fileName);
            if (aisDefnCol != null)
            {
                aisDefnCol.FileName = fileName;
                aisDefnCol.FileType = FileTypeEnum.NativeXMLFile;
                return aisDefnCol;
            }

            return LoadInternal();
        }
        public static AISDefnCollection LoadInternal()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("OpenSkipperApplication.Resources.AIS.AISDfn.xml");

            AISDefnCollection newDefns = XmlFileSerializer.Deserialize(stream);
            newDefns.FileType = FileTypeEnum.Internal; // No filename set, instead we set type to internal
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
}
