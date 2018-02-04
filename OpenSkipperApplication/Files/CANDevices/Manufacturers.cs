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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace CANDevices
{
    public class Manufacturer
    {
        public string Name { get; set; }
        public UInt32 Code { get; set; }
    }

    public static class Manufacturers
    {
        public static ManufacturerCollection _Manufacturers;

        static Manufacturers()
        {
            _Manufacturers = ManufacturerCollection.LoadFromFile();
        }

        public static string AsString(UInt32 key)
        {
            Manufacturer m;
            string str;
            if (_Manufacturers.ManufacturerByCode.TryGetValue(key, out m))
            {
                str = key.ToString() + " (" + m.Name + ")";
            }
            else
            {
                str = key.ToString() + " (unknown)";
            }
            return str;
        }

    }

    public class ManufacturerCollection
    {
        public const string DefDefnFile = "Manufacturers.N2kDfn.xml";
        private Manufacturer[] _Manufacturers;

        [XmlIgnore]
        // The file we loaded the definitions from
        public string FileName { get; set; }

        [XmlIgnore]
        public FileTypeEnum FileType { get; set; }

        [XmlIgnore]
        public Dictionary<UInt32, Manufacturer> ManufacturerByCode;

        public ManufacturerCollection()
        {
            ManufacturerByCode = new Dictionary<UInt32, Manufacturer> { };
            Manufacturers = new Manufacturer[0] { };
            FileName = "";
        }

        public Manufacturer[] Manufacturers
        {
            get
            {
                return _Manufacturers;
            }
            set
            {
                _Manufacturers = value ?? new Manufacturer[0];
                BuildInternalStructures();
            }
        }

        // Serialization
        private static XmlFileSerializer<ManufacturerCollection> XmlFileSerializer = new XmlFileSerializer<ManufacturerCollection>();

        // File IO
        public static ManufacturerCollection LoadFromFile()
        {
            string fileName = DefDefnFile;
            if (fileName == "") return LoadInternal();

            ManufacturerCollection manufCol = XmlFileSerializer.Deserialize(fileName);
            if (manufCol != null)
            {
                manufCol.FileName = fileName;
                manufCol.FileType = FileTypeEnum.NativeXMLFile;
                return manufCol;
            }

            return LoadInternal();
        }
        public static ManufacturerCollection LoadInternal()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("OpenSkipperApplication.Resources.Manufacturers.N2kDfn.xml");

            ManufacturerCollection newDefns = XmlFileSerializer.Deserialize(stream);
            newDefns.FileType = FileTypeEnum.Internal; // No filename set, instead we set type to internal
            return newDefns;
        }

        // Internal structs
        private void BuildInternalStructures()
        {
            ManufacturerByCode.Clear();
            foreach (Manufacturer m in Manufacturers)
            {
                if (!ManufacturerByCode.ContainsKey(m.Code))
                {
                    ManufacturerByCode[m.Code] = m;
                }
            }
        }
    }

} // namespace
