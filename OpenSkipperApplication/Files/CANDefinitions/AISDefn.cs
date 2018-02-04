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
        public const string DefDefnFile = "AIS.AISDfn.xml";
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

} // namespace
