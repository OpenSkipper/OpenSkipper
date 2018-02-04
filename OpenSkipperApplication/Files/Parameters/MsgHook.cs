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

using CANDevices;
using CANHandler;
using CANStreams;
using System;
using System.ComponentModel;

namespace Parameters
{
    public enum MsgTypeEnum
    {
        NMEA2000,
        NMEA0183,
        AIS
    }

    /// <summary>
    /// Message Hook Filter
    /// </summary>
    /// Hook filter should contain real field value. In this way
    /// checking would not need conversion every time.
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MsgHookFilter
    {
        public string FieldName { get; set; }

        public string Value { get; set; }

        public MsgHookFilter Clone()
        {
            return (MsgHookFilter)this.MemberwiseClone();
        }
    }

    /// <summary>
    /// Supports hooking onto a stream and extracting messages which meet specified criteria.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MsgHook
    {
        public const int AnyDestination = -1;  // Used to indicate an N2k message that was sent to any destination device
        public const int AnySource = -1;       // Used to indicate an N2k message received from any device
        public const int RuleValue = -2;       // Used to indicate an N2k message received by rule device

        private int AisID;

        private int PGN;
        private string PGNSourceRule;
        private string PGNDestinationRule;
        private int PGNSource;
        private int PGNDestination;
        private msgField _msgField;

        private string TypeCode;    // The typically 3 letter NMEA 0183 message type
        private string TalkerID;    // The typically 2 letter NMEA 0183 sending device type ID

        private string _FieldName = "";
        private double _Multiplier;

        // all browsable - Set methods change internals
        //        [DefaultValue(1)]
        [Description("Set multiplier for numerical message")]
        public double Multiplier
        {
            get { return _Multiplier; }
            set { _Multiplier = value; }
        }
        public MsgHookFilter Filter { get; set; }

        private bool ShouldSerializeMultiplier()
        {
            //RETURN:
            //      = true if the property value should be displayed in bold, or "treated as different from a default one"
            return !(_Multiplier == 1);
        }
        private void ResetMultiplier()
        {
            //This method is called after a call to 'PropertyGrid.ResetSelectedProperty()' method on this property
            _Multiplier = 1;
        }


        //[DefaultValue("")]
        [Description("Choose the type of message that is used to set the value for your parameter.")]
        public MsgTypeEnum Protocol { get; set; }

        //[DefaultValue("")]
        [Description("Enter the NMEA2000 PGN, NMEA 0183 message type (typically 3-letter), or the AIS message ID of the message used to update this parameter.")]
        public string Identifier
        {
            get
            {
                switch (Protocol)
                {
                    case MsgTypeEnum.NMEA2000:
                        return PGN.ToString();

                    case MsgTypeEnum.NMEA0183:
                        return TypeCode;

                    case MsgTypeEnum.AIS:
                        return AisID.ToString();

                    default:
                        throw new Exception("Invalid parameter Protocol: '" + Protocol.GetType().Name + "'");
                }
            }
            set
            {
                int v;
                if (int.TryParse(value, out v))
                {
                    switch (Protocol)
                    {
                        case MsgTypeEnum.NMEA2000:
                            PGN = v;
                            break;

                        case MsgTypeEnum.AIS:
                            AisID = v;
                            break;

                        case MsgTypeEnum.NMEA0183:
                            TypeCode = v.ToString();
                            break;
                    }
                }
                else
                {
                    Protocol = MsgTypeEnum.NMEA0183;
                    TypeCode = value;
                }
            }
        }
        //[DefaultValue("Any")]
        [Description("Enter the device bus address (0..254) or ID:<device unique ID> that the NMEA 2000 message must have been sent from. For NMEA 0183 device enter device (typically 2-letter) type. 'Any' accepts messsages sent by any device.")]
        public string Source
        {
            get
            {
                switch (Protocol)
                {
                    case MsgTypeEnum.NMEA2000:
                        return (PGNSourceRule == null ? (PGNSource == AnySource ? "Any" : PGNSource.ToString()) : PGNSourceRule);

                    case MsgTypeEnum.NMEA0183:
                        return TalkerID;

                    case MsgTypeEnum.AIS:
                        return "N/A";

                    default:
                        throw new Exception("Invalid parameter Protocol: '" + Protocol.GetType().Name + "'");
                }
            }
            set
            {
                int v;
                bool ValueIsNumber = (int.TryParse(value, out v));
                switch (Protocol)
                {
                    case MsgTypeEnum.NMEA2000:
                        if (value.Equals("Any", StringComparison.OrdinalIgnoreCase))
                        {
                            PGNSource = AnySource;
                        }
                        else if (ValueIsNumber)
                        {
                            PGNSource = v;
                        }
                        else
                        {
                            PGNSource = RuleValue;
                            PGNSourceRule = value;
                        }
                        break;

                    case MsgTypeEnum.NMEA0183:
                        if (value.Equals("Any", StringComparison.OrdinalIgnoreCase))
                        {
                            TalkerID = "Any";   // Force a consistent case
                        }
                        else
                        {
                            TalkerID = value;
                        }
                        break;

                    default:
                        // Don't do anything
                        break;
                }
            }
        }
        //[DefaultValue("Any")]
        [Description("Enter the device bus address (0..254) or ID:<device unique ID> that the NMEA 2000 message must have been sent to. Entering 'Any' accepts messages sent to any (or all) devices.")]
        public string Destination
        {
            get
            {
                switch (Protocol)
                {
                    case MsgTypeEnum.NMEA2000:
                        return (PGNDestinationRule == null ? (PGNDestination == AnyDestination ? "Any" : PGNDestination.ToString()) : PGNDestinationRule);

                    case MsgTypeEnum.NMEA0183:
                        return "N/A";

                    case MsgTypeEnum.AIS:
                        return "N/A";
                }
                return "N/A";
            }
            set
            {
                switch (Protocol)
                {
                    case MsgTypeEnum.NMEA2000:
                        if (value.Equals("Any", StringComparison.OrdinalIgnoreCase))
                        {
                            PGNDestination = AnyDestination;
                        }
                        else
                        {
                            int v;
                            if (int.TryParse(value, out v))
                            {
                                PGNDestination = v;
                            }
                            else
                            {
                                PGNDestination = RuleValue;
                                PGNDestinationRule = value;
                            }
                        }
                        break;

                    default:
                        // Don't do anything
                        break;
                }
            }
        }

        //[DefaultValue("")]
        [Description("Enter the name of the field in the incoming message that contains the parameter value.")]
        public string FieldName { get { return _FieldName; } set { _FieldName = value; } }

        [BrowsableAttribute(false)]
        public msgField MsgField { get { return _msgField; } set { _msgField = value; } }

        public event EventHandler<FrameReceivedEventArgs> FrameReceived;

        public void N2kSourceChanged()
        {
            CANDevice dev = null;
            if (PGNSourceRule != null)
            {
                PGNSource = RuleValue;
                dev = CANDeviceList.FindDeviceByRule(PGNSourceRule);
                if (dev != null) PGNSource = dev.Source;
            }
            if (PGNDestinationRule != null)
            {
                PGNDestination = RuleValue;
                dev = CANDeviceList.FindDeviceByRule(PGNDestinationRule);
                if (dev != null) PGNDestination = dev.Source;
            }
        }

        public void MessageCallback(object sender, FrameReceivedEventArgs e)
        {
            switch (Protocol)
            {
                case MsgTypeEnum.NMEA2000:
                    //                    PGNSource = 101;
                    N2kFrame n2kMsg = (N2kFrame)e.ReceivedFrame;
                    if (((PGNSource == AnySource) || (n2kMsg.Header.PGNSource == PGNSource)) &&
                         ((PGNDestination == AnyDestination) || (n2kMsg.Header.PGNDestination == PGNDestination)))
                        FrameReceived(this, e);
                    break;

                case MsgTypeEnum.NMEA0183:
                    N0183Frame n0183Msg = (N0183Frame)e.ReceivedFrame;
                    if ((TalkerID == "Any") || (n0183Msg.Header.TalkerID == TalkerID))
                        FrameReceived(this, e);
                    break;

                case MsgTypeEnum.AIS:
                    AISFrame aisMsg = (AISFrame)e.ReceivedFrame;
                    // if (aisMsg.Header.MessageID == AisID) TODO Confirm we don't need this
                    FrameReceived(this, e);
                    break;
            }
        }

        // Hooks onto/off stream
        public void HookOnto(CANStreamer stream)
        {
            switch (Protocol)
            {
                case MsgTypeEnum.NMEA2000:
                    stream.PGNReceived[PGN] += MessageCallback;
                    if (PGNDestinationRule != null || PGNSourceRule != null) CANDeviceList.SourceChange += N2kSourceChanged;
                    break;

                case MsgTypeEnum.NMEA0183:
                    stream.TypeCodeReceived[TypeCode] += MessageCallback;
                    break;

                case MsgTypeEnum.AIS:
                    stream.AISReceived[AisID] += MessageCallback;
                    break;
            }
        }

        public void UnhookFrom(CANStreamer stream)
        {
            if (Protocol == MsgTypeEnum.NMEA2000)
            {
                stream.PGNReceived[PGN] -= MessageCallback;
                if (PGNDestinationRule != null || PGNSourceRule != null) CANDeviceList.SourceChange -= N2kSourceChanged;
            }
            else
            {
                stream.TypeCodeReceived[TypeCode] -= MessageCallback;
            }
        }

        public MsgHook Clone()
        {
            MsgHook clone = (MsgHook)this.MemberwiseClone();
            if (this.Filter != null) clone.Filter = this.Filter.Clone();
            return (MsgHook)clone;
        }

        // See about resetting default values http://www.codeproject.com/Articles/66073/DefaultValue-Attribute-Based-Approach-to-Property
        public MsgHook()
        {
            ResetMultiplier();
            _msgField = null;
        }

        public Boolean CheckFilter(FrameReceivedEventArgs e)
        {
            if (Filter == null) return true;
            Frame msg = e.ReceivedFrame;

            return (msg.IsSameValue(this.Filter.FieldName, Filter.Value));
        }
    }

} // namespace
