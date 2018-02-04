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
    public class DeviceClass
    {
        private DeviceFunction[] _DeviceFunctions;

        [XmlIgnore]
        public Dictionary<UInt32, DeviceFunction> DeviceFunctionByCode;

        public string Name { get; set; }
        public UInt32 Code { get; set; }
        public override string ToString() { return Code + " (" + Name + ")"; }

        public DeviceClass()
        {
            DeviceFunctionByCode = new Dictionary<UInt32, DeviceFunction> { };
            _DeviceFunctions = new DeviceFunction[0] { };
        }

        public DeviceFunction[] DeviceFunctions
        {
            get
            {
                return _DeviceFunctions;
            }
            set
            {
                _DeviceFunctions = value ?? new DeviceFunction[0];
                BuildInternalStructures();
            }
        }

        // Internal structs
        private void BuildInternalStructures()
        {
            DeviceFunctionByCode.Clear();
            foreach (DeviceFunction c in DeviceFunctions)
            {
                if (!DeviceFunctionByCode.ContainsKey(c.Code))
                {
                    DeviceFunctionByCode[c.Code] = c;
                }
            }
        }
    }

    public class DeviceFunction
    {
        public string Name { get; set; }
        public UInt32 Code { get; set; }
        public override string ToString() { return Code + " (" + Name + ")"; }
    }

    public static class DeviceClasses
    {
        public static DeviceClassCollection _DeviceClasses;

        static DeviceClasses()
        {
            _DeviceClasses = DeviceClassCollection.LoadFromFile();
        }

        public static string AsString(UInt32 classCode)
        {
            DeviceClass c;
            string str;

            if (_DeviceClasses.DeviceClassByCode.TryGetValue(classCode, out c))
            {
                str = c.ToString();
            }
            else
            {
                str = classCode.ToString() + " (unknown)";
            }
            return str;
        }

        public static string FunctionAsString(UInt32 classCode, UInt32 functionCode)
        {
            DeviceClass c;
            DeviceFunction f;
            string str;

            if (_DeviceClasses.DeviceClassByCode.TryGetValue(classCode, out c) &&
                c.DeviceFunctionByCode.TryGetValue(functionCode, out f))
            {
                str = f.ToString();
            }
            else
            {
                str = functionCode.ToString() + " (unknown)";
            }
            return str;
        }

    }

    public class DeviceClassCollection
    {
        public const string DefDefnFile = "ClassAndFunction.N2kDfn.xml";
        private DeviceClass[] _DeviceClasses;

        [XmlIgnore]
        // The file we loaded the definitions from
        public string FileName { get; set; }
        [XmlIgnore]
        public FileTypeEnum FileType { get; set; }
        [XmlIgnore]
        public Dictionary<UInt32, DeviceClass> DeviceClassByCode;

        public DeviceClassCollection()
        {
            DeviceClassByCode = new Dictionary<UInt32, DeviceClass> { };
            DeviceClasses = new DeviceClass[0] { };
            FileName = "";
        }

        public DeviceClass[] DeviceClasses
        {
            get
            {
                return _DeviceClasses;
            }
            set
            {
                _DeviceClasses = value ?? new DeviceClass[0];
                BuildInternalStructures();
            }
        }

        // Serialization
        private static XmlFileSerializer<DeviceClassCollection> XmlFileSerializer = new XmlFileSerializer<DeviceClassCollection>();

        // File IO
        public static DeviceClassCollection LoadFromFile()
        {
            string fileName = DefDefnFile;
            if (fileName == "") return LoadInternal();

            DeviceClassCollection classCol = XmlFileSerializer.Deserialize(fileName);
            if (classCol != null)
            {
                classCol.FileName = fileName;
                classCol.FileType = FileTypeEnum.NativeXMLFile;
                return classCol;
            }

            return LoadInternal();
        }
        public static DeviceClassCollection LoadInternal()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("OpenSkipperApplication.Resources.ClassAndFunction.N2kDfn.xml");

            DeviceClassCollection newDefns = XmlFileSerializer.Deserialize(stream);
            newDefns.FileType = FileTypeEnum.Internal; // No filename set, instead we set type to internal
            return newDefns;
        }

        // Internal structs
        private void BuildInternalStructures()
        {
            DeviceClassByCode.Clear();
            foreach (DeviceClass c in DeviceClasses)
            {
                if (!DeviceClassByCode.ContainsKey(c.Code))
                {
                    DeviceClassByCode[c.Code] = c;
                }
            }
        }
    }

} // namespace
