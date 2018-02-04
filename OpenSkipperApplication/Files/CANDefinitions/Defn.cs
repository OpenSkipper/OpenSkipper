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
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// 
/// </summary>
namespace CANDefinitions
{
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

} // namespace
