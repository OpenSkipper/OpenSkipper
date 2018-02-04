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

using CANDefinitions;
using Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CANHandler
{
    /// <summary>
    /// Represents an NMEA 0183 message
    /// </summary>
    [XmlRoot("N0183Msg")]
    public class N0183Frame : Frame
    {
        // Static
        public static XmlSerializer XmlSerializer = new XmlSerializer(typeof(N0183Frame));

        public static N0183Frame TryCreate(string fullMessage, DateTime timeStamp)
        {
            try
            {
                N0183Frame newFrame = new N0183Frame(fullMessage, timeStamp);

                if (newFrame.IsCRCOk)
                    return newFrame;

                return null;
            }
            catch
            {
                Console.WriteLine(fullMessage);
                return null;
            }
        }

        // Public properties
        [XmlText]
        public string FullMessage
        {
            get
            {
                return _fullMessage;
            }
            set
            {
                // 'FullMessage' would be a readonly field, however that prevents XML serialization (Which sets properties AFTER constructor)
                // So instead we put this exception here, to give us the guarantee that it is not changed after construction.
                if (_messageSet)
                    throw new InvalidOperationException("Cannot change NMEA 0183 message after creation");
                _messageSet = true;

                // This code should, and currently does, support being called with or without trailing whitespace/LF/CR characters.

                _fullMessage = value;

                int asterixIndex = value.IndexOf('*'); // Asterix tells us where the data ends and checksum begins
                if (asterixIndex < 6)
                {
                    IsCRCOk = false;
                    ReportHandler.LogError("Malformed message: " + _fullMessage);
                    return;
                }

                this.Header = new N0183Header(value.Substring(0, 6));

                _dataString = value.Substring(7, asterixIndex - 7); // 7 to skip the first comma.
                _dataSegments = _dataString.Split(new char[] { ',' });

                int checksumChars = value.Length - asterixIndex - 1;

                if (checksumChars >= 2)
                    _checksum = Convert.ToByte(value.Substring(asterixIndex + 1, 2), 16);
                else
                    if (checksumChars == 1)
                    _checksum = Convert.ToByte(value.Substring(asterixIndex + 1, 1), 16);
                else
                {
                    IsCRCOk = false;
                    ReportHandler.LogError("No checksum on message: " + _fullMessage);
                    return;
                }

                byte actualChecksum = 0;
                for (int i = 1; i < asterixIndex; i++)
                    actualChecksum ^= Convert.ToByte(_fullMessage[i]);

                IsCRCOk = (_checksum == actualChecksum);
                if (!IsCRCOk)
                {
                    ReportHandler.LogError("Bad checksum on message: " + _fullMessage);
                }
            }
        }
        [XmlIgnore] public bool IsCRCOk { get; set; }
        [XmlIgnore] public new N0183Header Header { get { return (N0183Header)base.Header; } set { base.Header = value; } }
        [XmlIgnore] public new N0183Defn Defn { get { return (N0183Defn)base.Defn; } set { base.Defn = value; } }
        [XmlIgnore] public string DataString { get { return _dataString; } }
        [XmlIgnore] public string[] Segments { get { return _dataSegments; } }
        [XmlIgnore] public override int Length { get { return _dataString.Length; } }

        // Private
        private byte _checksum;
        private string _fullMessage;
        private string _dataString;
        private string[] _dataSegments;
        private bool _messageSet = false;

        // Constructors
        private N0183Frame()
        {
            // For XML serializer (Can access private ctors)
        }

        public N0183Frame(string fullMessage, DateTime timeStamp)
        {
            this.FullMessage = fullMessage;
            this.TimeStamp = timeStamp;
        }

        public N0183Frame(string NMEA0183TypeCode, List<Parameter> Parameters)
        {
            // Form an NMEA 0183 string from a sequence of parameters
            StringBuilder message = new StringBuilder(NMEA0183TypeCode);

            foreach (Parameter p in Parameters)
            {
                message.Append(",");
                if (p != null) message.Append(p.ToString());
            }

            // Add checksum
            byte checksum = 0;
            for (int i = 1; i < message.Length; i++)
                checksum ^= Convert.ToByte(message[i]);
            message.Append("*" + checksum.ToString("X2"));

            // Step 4 : Save the new N0183 frame
            this.FullMessage = message.ToString();
            this.TimeStamp = DateTime.Now;
        }

        // Public methods
        public override string ToString()
        {
            return _fullMessage;
        }

        public override double GetDouble(msgField field, out FieldValueState valueState)
        {
            N0183NumericField n0183Field = field as N0183NumericField;
            if (n0183Field == null)
            {
                valueState = FieldValueState.Error;
                return 0.0;
            }

            return n0183Field.GetValue(_dataSegments, out valueState);
        }

        public override string GetString(msgField field, out FieldValueState valueState)
        {
            N0183Field NamedField = field as N0183Field;
            if (NamedField == null)
            {
                valueState = FieldValueState.Error;
                return "N/A";
            }

            valueState = FieldValueState.Valid;
            return NamedField.ToString(_dataSegments);
        }

        public override bool IsSameValue(msgField field, string cmpTo)
        {
            N0183Field NamedField = field as N0183Field;
            if (NamedField == null) return false;
            return (NamedField.SegmentIndex < _dataSegments.Length) ?
                   (_dataSegments[NamedField.SegmentIndex] == cmpTo) :
                   false;
        }

        public override double GetDouble(string fieldName, out FieldValueState valueState)
        {
            return GetDouble(Defn.GetFieldByName(fieldName), out valueState);
        }

        public override string GetString(string fieldName, out FieldValueState valueState)
        {
            return GetString(Defn.GetFieldByName(fieldName), out valueState);
        }

        public override bool IsSameValue(string fieldName, string cmpTo)
        {
            return IsSameValue(Defn.GetFieldByName(fieldName), cmpTo);
        }

        public override msgField GetField(string fieldName)
        {
            return Defn.GetFieldByName(fieldName);
        }
    }

} // namespace
