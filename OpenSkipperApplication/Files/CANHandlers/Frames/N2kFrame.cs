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
using OpenSkipperApplication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace CANHandler
{
    /// <summary>
    /// Represents an NMEA 2000 message
    /// </summary>
    [XmlRoot("N2kMsg")]
    public class N2kFrame : Frame
    {
        // Builder
        public class Builder
        {
            // We store a list of FastPacketMessage's, keyed by the header data (as a uint32)
            private Dictionary<uint, FastPacketMessage> _fastPackets = new Dictionary<uint, FastPacketMessage>();

            public N2kFrame AddFrame(N2kFrame n2kMsg)
            {
                N2kHeader n2kHeader = n2kMsg.Header;

                FastPacketMessage FastPacket;
                if (_fastPackets.TryGetValue(n2kHeader.AsUInt, out FastPacket))
                {
                    // This frame is the next in some existing sequence, so add it to the sequence.
                    try
                    {
                        if (FastPacket.AddNextFrame(n2kMsg))
                        {
                            // It was the last frame in the sequence, so the message is complete, and no further processing is needed. 
                            // Return the processed message as a decoded message
                            _fastPackets.Remove(n2kHeader.AsUInt);

                            FastPacket.Defn = FastPacket.pgnInfo;
                            
                            return new N2kFrame(FastPacket.Header, FastPacket.Data, FastPacket.TimeStamp) { Defn = FastPacket.Defn };
                        }
                        else
                        {
                            // More frames are needed, so cannot return any decoded message yet
                            return null;
                        }
                    }
                    catch(Exception ex)
                    {
                        // There has been an error. Stop processing the incoming frames
                        _fastPackets.Remove(n2kHeader.AsUInt);

                        Debug.WriteLine(ex.ToString());

                        throw ex;
                    }
                }

                // Get the PGN that provides info about this frame.
                PGNDefn pgnInfo = Definitions.PGNDefnCol.GetPGNDefn(n2kMsg);
                if ((n2kMsg.Data.Length > 8) || (pgnInfo.ByteLength <= 8))
                {
                    if (pgnInfo.ByteLength > n2kMsg.Data.Length)
                    {
                        ReportHandler.LogWarning(string.Format("PGN {0} length ({1}) exceeds data provided ({2}); some fields won't show.", pgnInfo, pgnInfo.ByteLength, n2kMsg.Data.Length));
                    }

                    // This is a single frame message. Return it with the associated PGN
                    n2kMsg.Defn = pgnInfo;
                    return n2kMsg;
                }
                else
                {
                    // This is the first in a sequence of fast packet frames. Add it for further processing, and return nothing

                    // First frame must start with 00, 02, 04, ...
                    // If it doesn't, we ignore the packet (We have missed the start of a multi-packet message (Started receiving too late), => impossible to form it)
                    if ((n2kMsg.Data[0] & 0x1F) == 0)
                    {
                        FastPacket = new FastPacketMessage(n2kMsg, pgnInfo);
                        _fastPackets.Add(n2kHeader.AsUInt, FastPacket);

                        if (pgnInfo.ByteLength > FastPacket.ExpectedByteCount)
                        {
                            ReportHandler.LogWarning(string.Format("PGN {0} length ({1}) exceeds data provided ({2}); some fields won't show.", pgnInfo.PGN, pgnInfo.ByteLength, FastPacket.ExpectedByteCount));
                        }
                    }

                    return null;
                }
            }

            public void Reset()
            {
                _fastPackets.Clear();
            }

        }

        // Constants
        private const byte Escape = 0x10;
        private const byte StartOfText = 0x02;
        private const byte EndOfText = 0x03;
        private const byte N2kDataReceived = 0x93;

        // Private Members
        private int DataLen = 0;
        private readonly object Lock = new object();

        public static XmlSerializer XmlSerializer = new XmlSerializer(typeof(N2kFrame));

        [XmlIgnore]
        public new N2kHeader Header { get { return (N2kHeader)base.Header; } set { base.Header = value; } }

        [XmlIgnore]
        public new PGNDefn Defn { get { return (PGNDefn)base.Defn; } set { base.Defn = value; } }

        [XmlText]
        public string AsHex
        {
            get
            {
                var x = new System.Text.StringBuilder();
                for (int i = 0; i < Bytes.Length; i++)
                    x.Append(Bytes[i].ToString("X2"));

                return x.ToString();
            }
            set
            {
                var x = new List<byte> { };
                for (int i = 0; i < value.Length; i += 2)
                    x.Add(Convert.ToByte(value.Substring(i, 2), 16));

                Bytes = x.ToArray();
            }
        }

        [XmlIgnore]
        public byte[] Data { get; set; } // Contains the data, minus header bytes

        [XmlIgnore] // [XmlText(DataType = "hexBinary")]
        public byte[] Bytes
        {
            get
            {
                var b = new byte[Data.Length + 4];
                N2kHeader header = (N2kHeader)Header;
                b[0] = header.Byte0;
                b[1] = header.Byte1;
                b[2] = header.Byte2;
                b[3] = header.Byte3;
                for (int i = 0; i < Data.Length; i++)
                {
                    b[i + 4] = Data[i];
                }
                return b;
            }
            set
            {
                Header = new N2kHeader(value[0], value[1], value[2], value[3]);
                Data = new byte[value.Length - 4];
                for (int i = 4; i < value.Length; i++)
                {
                    Data[i - 4] = value[i];
                }
            }
        }

        [XmlIgnore]
        public override int Length { get { return Bytes.Length; } }

        // Constructors
        private N2kFrame()
        {
            // For XML deserializer
            this.DataLen = this.Length;
        }

        public N2kFrame(N2kHeader h, byte[] data, DateTime timeStamp)
        {
            this.Header = h;
            this.Data = data;
            this.TimeStamp = timeStamp;

            if (this.Data != null)
            {
                this.DataLen = this.Data.Length;
            }
        }

        // Public methods
        public override string ToString()
        {
            return BitConverter.ToString(Data);
        }

        public override double GetDouble(msgField field, out FieldValueState valueState)
        {
            N2kNumericField n2kField = field as N2kNumericField;
            if (n2kField == null)
            {
                valueState = FieldValueState.Error;
                return 0.0;
            }

            return n2kField.GetValue(Data, out valueState);
        }

        public override string GetString(msgField field, out FieldValueState valueState)
        {
            N2kField NamedField = field as N2kField;
            if (NamedField == null)
            {
                valueState = FieldValueState.Error;
                return "N/A";
            }

            valueState = FieldValueState.Valid;
            return NamedField.ToString(Data);
        }

        public override double GetDouble(string fieldName, out FieldValueState valueState)
        {
            return GetDouble(Defn.GetFieldByName(fieldName), out valueState);
        }

        public override string GetString(string fieldName, out FieldValueState valueState)
        {
            return GetString(Defn.GetFieldByName(fieldName), out valueState);
        }

        public override bool IsSameValue(msgField field, string cmpTo)
        {
            N2kField NamedField = field as N2kField; ;
            if (NamedField == null) return false;
            N2kNumericField NumField = NamedField as N2kNumericField;
            if (NumField != null)
            {
                try
                {
                    FieldValueState valueState;
                    double NumVal = Convert.ToDouble(cmpTo);
                    return (NumVal == NumField.GetValue(Data, out valueState));
                }
                catch
                {
                }
            }

            return (NamedField.ToString(Data) == cmpTo);
        }

        // This is used by hook filter. It should be improved so that we provide the filter 
        // class, which could be checked without every time convert.
        public override bool IsSameValue(string fieldName, string cmpTo)
        {
            return IsSameValue(Defn.GetFieldByName(fieldName), cmpTo);
        }

        public override msgField GetField(string fieldName)
        {
            return Defn.GetFieldByName(fieldName);
        }

        public void SetValue(string fieldName, string value)
        {
            var field = Defn.GetFieldByName(fieldName);
            if (field == null)
            {
                throw new ArgumentException("Field name not found in the PGNDefn.");
            }

            if (this.Data == null)
            {
                this.Data = new byte[Defn.ByteLength];
            }

            lock (Lock)
            {
                BitArray bits = null;
                byte[] bytes = null;
                FieldValueState state = FieldValueState.NotAvailable;

                if (string.IsNullOrEmpty(value))
                {
                    value = "0";
                }

                switch (field.FieldType)
                {
                    case "N2kInstanceField":
                        var instField = field as N2kInstanceField;
                        bytes = instField.SetValue(value, out state);
                        break;

                    case "N2kUIntField":
                        var intfield = field as N2kUIntField;
                        bytes = intfield.SetValue(value, out state);
                        break;

                    case "N2kUDblField":
                        var udblField = field as N2kUDblField;
                        bytes = udblField.SetValue(value, out state);
                        break;

                    case "N2kDblField":
                        var dblField = field as N2kDblField;
                        bytes = dblField.SetValue(value, out state);
                        break;

                    case "N2kEnumField":
                        var enmField = field as N2kEnumField;
                        bytes = enmField.SetValue(value, out state);
                        break;
                }

                bits = new BitArray(bytes);

                // Sanity check
                //if (chkVal != value || state != FieldValueState.Valid)
                if (state != FieldValueState.Valid)
                {
                    // TODO: This should be an error
                    string error = "Values are not equal";
                }

                if (bits.Length > field.BitLength)
                {
                    // TODO: Should this truncate or return an error?
                }
                else if (bits.Length < field.BitLength)
                {
                    // TODO: Should this pad left with zeros?
                    bytes.LeftPad(0, field.BitLength);
                }

                bytes.CopyTo(this.Data, field.ByteOffset);
            }
        }

    } // class

} // namespace
