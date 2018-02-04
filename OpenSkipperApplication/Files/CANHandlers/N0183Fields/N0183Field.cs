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
using System.ComponentModel;
using System.Xml.Serialization;

namespace CANHandler
{
    /// <summary>
    /// Base class for all N0183 fields
    /// </summary>
    public abstract class N0183Field : msgField
    {
        // Static
        public static Type[] AllFieldTypes()
        {
            var TypeList = new List<Type>();
            foreach (Type t in typeof(N0183Field).Assembly.GetTypes())
            {
                if (!t.IsAbstract && (t.IsSubclassOf(typeof(N0183Field)) || t == typeof(N0183Field)))
                {
                    // string theClass = t.FullName.Substring(t.FullName.LastIndexOf(".")+1);
                    // TypeNameList.Add(t.DisplayName);
                    TypeList.Add(t);
                }
            }
            return TypeList.ToArray();
        }

        public static N0183Field CreateNewField(Type fieldType)
        {
            //Define the type of the control you want to create an instance of using reflection. 
            try
            {
                return (N0183Field)(Activator.CreateInstance(fieldType));
            }
            catch
            {
                //Set the control to null 
                return null;
            }
        }

        // Public
        [XmlAttribute]
        [Description("What index the field relates to")]
        public int SegmentIndex { get; set; }

        [BrowsableAttribute(true)]
        public string FieldType
        {
            get
            {
                return this.GetType().Name;
            }
        }

        // Methods
        public virtual string ToString(string[] segments)
        {
            return (SegmentIndex < segments.Length) ? segments[SegmentIndex] : "N/A";
        }

        public override int CompareTo(object o)
        {
            N0183Field o2 = o as N0183Field;
            if (o2 == null)
            {
                throw new Exception("cant compare N0183Field to " + o.GetType().ToString());
            }
            return SegmentIndex.CompareTo(o2.SegmentIndex);
        }
    }

} // namespace
