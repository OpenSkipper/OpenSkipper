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
using System.Xml;
using System.Xml.Serialization;

namespace CANDefinitions
{
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

} // namespace
