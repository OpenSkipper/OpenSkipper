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
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CANHandler
{
    /// <summary>
    /// Represents an AIS message
    /// </summary>
    [XmlRoot("AISMsg")]
    public class AISFrame : Frame
    {
        /// <summary>
        /// Acts as the factory for AIS frames. Accepts AIVDM/AIVDO NMEA 0183 frames and (may) return the AIS frame
        /// For multi-part messages, it will hold onto the fragment until it receives all fragments, at which point it will return the fully assembled frame.
        /// </summary>
        public class Builder
        {
            private const int FragmentSetTimeout = 30; // Fragment sets are reset if new fragment arrives >timeout /seconds/ since last (This aspect is controlled by the Builder, not the set itself)

            /// <summary>
            /// Holds a given number of fragments. When all fragments are received, it will return the completed AIS frame.
            /// </summary>
            private class AISFragmentSet
            {
                // Properties
                public DateTime LastReceivedTime
                {
                    get
                    {
                        return _lastReceivedTime;
                    }
                }

                // Private fields
                private int _fragmentCount;
                private bool[] _haveFragment;
                private string[] _fragmentData;
                private int _fragmentsRecieved;
                private DateTime _lastReceivedTime;

                // Constructor
                public AISFragmentSet(int fragmentCount)
                {
                    _fragmentCount = fragmentCount;
                    _fragmentData = new string[fragmentCount];
                    _haveFragment = new bool[fragmentCount];
                }

                // Public methods
                public AISFrame AddFragment(int fragmentNumber, string fragmentData)
                {
                    if (!_haveFragment[fragmentNumber - 1])
                    {
                        _fragmentData[fragmentNumber - 1] = fragmentData;
                        _haveFragment[fragmentNumber - 1] = true;
                        _fragmentsRecieved++;
                        _lastReceivedTime = DateTime.Now;

                        if (_fragmentsRecieved == _fragmentCount)
                        {
                            // Return completed frame !
                            return new AISFrame(string.Concat(_fragmentData), DateTime.Now);
                        }
                        else
                        {
                            // Frame not yet complete
                            return null;
                        }
                    }
                    else
                    {
                        // We already have this fragment, we simply ignore it
                        return null;
                    }
                }
            }

            // Dictionary mapping sequential message ID to its corresponding fragment set
            private Dictionary<int, AISFragmentSet> _fragmentSets;

            // Constructors
            public Builder()
            {
                _fragmentSets = new Dictionary<int, AISFragmentSet> { };
            }

            // Public methods
            public AISFrame AddFrame(N0183Frame n0183Frame)
            {
                // [0] = fragment count
                // [1] = fragment number
                // [2] = sequential message ID (for multipart)
                // [3] = radio channel (A or B)
                // [4] = ais data
                // [5] = number of padding bits

                int fragmentCount = int.Parse(n0183Frame.Segments[0]);
                if (fragmentCount == 1)
                {
                    AISFrame completedFrame = new AISFrame(n0183Frame.Segments[4], DateTime.Now);
                    completedFrame.Defn = Definitions.AISDefnCol.GetAISDefn(completedFrame);
                    return completedFrame;
                }
                else
                {
                    int fragmentNumber = int.Parse(n0183Frame.Segments[1]);
                    int sequenceID = int.Parse(n0183Frame.Segments[2]);
                    AISFragmentSet fragmentSet;

                    // Get corresponding fragment set, making sure it hasn't timed out
                    if (!_fragmentSets.TryGetValue(sequenceID, out fragmentSet) || ((DateTime.Now - fragmentSet.LastReceivedTime).TotalSeconds > FragmentSetTimeout))
                    {
                        fragmentSet = new AISFragmentSet(fragmentCount);
                        _fragmentSets[sequenceID] = fragmentSet;
                    }

                    AISFrame completedFrame = fragmentSet.AddFragment(fragmentNumber, n0183Frame.Segments[4]);
                    if (completedFrame != null)
                    {
                        _fragmentSets.Remove(sequenceID);
                        completedFrame.Defn = Definitions.AISDefnCol.GetAISDefn(completedFrame);
                    }
                    return completedFrame;
                }
            }

            public void Reset()
            {
                _fragmentSets.Clear();
            }
        }

        // Static
        public static XmlSerializer XmlSerializer = new XmlSerializer(typeof(AISFrame));

        // Public properties
        [XmlText]
        public string aisString
        {
            get
            {
                return _AISData.AISString;
            }
            set
            {
                // For xml serializer only !
                _AISData = new AISData(value);
                this.Header = new AISHeader((int)AISEncoding.GetUnsigned(_AISData.AISBytes, 0, 6));
            }
        }

        public override int Length
        {
            get
            {
                return _AISData.AISBytes.Length;
            }
        }

        [XmlIgnore]
        public new AISHeader Header { get { return (AISHeader)base.Header; } set { base.Header = value; } }

        [XmlIgnore]
        public new AISDefn Defn { get { return (AISDefn)base.Defn; } set { base.Defn = value; } }

        [XmlIgnore]
        public AISData AISData
        {
            get
            {
                return _AISData;
            }
            set
            {
                _AISData = value;
            }
        }

        private AISData _AISData;

        // Constructors
        private AISFrame()
        {
            // For XML serializer
        }

        private AISFrame(string aisString, DateTime timeStamp)
        {
            _AISData = new AISData(aisString);
            this.Header = new AISHeader((int)AISEncoding.GetUnsigned(_AISData.AISBytes, 0, 6));
            this.TimeStamp = timeStamp;
        }

        // Public methods
        public override double GetDouble(msgField field, out FieldValueState valueState)
        {
            AISField aisField = field as AISField;
            if (aisField == null)
            {
                valueState = FieldValueState.Error;
                return 0.0;
            }

            return aisField.GetValue(_AISData, out valueState);
        }

        public override string GetString(msgField field, out FieldValueState valueState)
        {
            AISField NamedField = field as AISField;
            if (NamedField == null)
            {
                valueState = FieldValueState.Error;
                return "N/A";
            }

            valueState = FieldValueState.Valid;
            return NamedField.ToString(_AISData);
        }

        public override bool IsSameValue(msgField field, string cmpTo)
        {
            AISField NamedField = field as AISField;
            if (NamedField == null) return false;
            return NamedField.ToString(_AISData) == cmpTo;
        }

        public override double GetDouble(string fieldName, out FieldValueState valueState)
        {
            AISField aisField = Defn.GetFieldByName(fieldName);
            if (aisField == null)
            {
                valueState = FieldValueState.Error;
                return 0.0;
            }

            return aisField.GetValue(_AISData, out valueState);
        }

        public override string GetString(string fieldName, out FieldValueState valueState)
        {
            AISField NamedField = Defn.GetFieldByName(fieldName);
            if (NamedField == null)
            {
                valueState = FieldValueState.Error;
                return "N/A";
            }

            valueState = FieldValueState.Valid;
            return NamedField.ToString(_AISData);
        }

        public override bool IsSameValue(string fieldName, string cmpTo)
        {
            AISField NamedField = Defn.GetFieldByName(fieldName);
            if (NamedField == null) return false;
            return NamedField.ToString(_AISData) == cmpTo;
        }

        public override msgField GetField(string fieldName)
        {
            return Defn.GetFieldByName(fieldName);
        }

    }

} // namespace
