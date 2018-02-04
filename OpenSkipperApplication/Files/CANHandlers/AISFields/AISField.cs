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
    /// Base class for all AIS fields
    /// </summary>
    public abstract class AISField : msgField, INotifyPropertyChanged
    {
        // Static
        public static Type[] AllFieldTypes()
        {
            var TypeList = new List<Type> { };
            foreach (Type t in typeof(AISField).Assembly.GetTypes())
                if (!t.IsAbstract && (t.IsSubclassOf(typeof(AISField)) || t == typeof(AISField)))
                    TypeList.Add(t);

            return TypeList.ToArray();
        }

        public static AISField CreateNewField(Type fieldType)
        {
            try
            {
                return (AISField)(Activator.CreateInstance(fieldType));
            }
            catch
            {
                return null;
            }
        }

        // Public
        [XmlAttribute]
        public override string Name
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

        public override string Description
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

        public override int CompareTo(object o) { return -1; } // ToDO

        public int BitOffset
        {
            get
            {
                return _bitOffset;
            }
            set
            {
                _bitOffset = value;
                NotifyPropertyChanged("BitOffset");
            }
        }

        public int BitLength
        {
            get
            {
                return _bitLength;
            }
            set
            {
                _bitLength = value;
                NotifyPropertyChanged("BitLength");
            }
        }

        // Private
        private string _name = "";
        protected string _desc = "";
        protected int _bitOffset;
        protected int _bitLength;

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        // Public methods
        public abstract double GetValue(AISData aisData, out FieldValueState valueState);

        public virtual string ToString(AISData aisData)
        {
            FieldValueState valueState;
            double value = GetValue(aisData, out valueState);
            return (valueState == FieldValueState.Valid) ? value.ToString() :
                   ((valueState == FieldValueState.NotAvailable) ? "Not Available" : "Error");
        }
    }

} // namespace
