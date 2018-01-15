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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using CANHandler;
using CANStreams;
using KeesFileHandler;
using OpenSkipperApplication;
using System.Reflection;
using System.Collections.Concurrent;
using CANDevices;

namespace Parameters
{
    public enum MsgTypeEnum
    {
        NMEA2000,
        NMEA0183,
        AIS
    }


    /// <summary>
    /// .
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
                        if (value.Equals("Any",StringComparison.OrdinalIgnoreCase)) {
                            PGNSource = AnySource;
                        } else if (ValueIsNumber) {
                            PGNSource = v;
                        }
                        else
                        {
                            PGNSource = RuleValue;
                            PGNSourceRule = value;
                        }
                        break;

                    case MsgTypeEnum.NMEA0183:
                        if (value.Equals("Any", StringComparison.OrdinalIgnoreCase)) {
                            TalkerID = "Any";   // Force a consistent case
                        } else {
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
                switch (Protocol) {
                    case MsgTypeEnum.NMEA2000:
                        if (value.Equals("Any", StringComparison.OrdinalIgnoreCase)) {
                            PGNDestination = AnyDestination;
                        } else {
                            int v;
                            if (int.TryParse(value, out v)) {
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
                    if ( ((PGNSource==AnySource) || (n2kMsg.Header.PGNSource == PGNSource)) &&
                         ((PGNDestination == AnyDestination) || (n2kMsg.Header.PGNDestination == PGNDestination)) )
                        FrameReceived(this, e);
                    break;

                case MsgTypeEnum.NMEA0183:
                    N0183Frame n0183Msg = (N0183Frame)e.ReceivedFrame;
                    if ( (TalkerID=="Any") || (n0183Msg.Header.TalkerID == TalkerID))
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
            if (this.Filter!=null) clone.Filter = this.Filter.Clone();
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

            return (msg.IsSameValue(this.Filter.FieldName,Filter.Value));
        }
    }

//    public interface IToString
//    {
//        string ToString();
//    }
    public interface IToDouble
    {
        double ToDouble();
    }

    /// <summary>
    /// A named value that is derived from messages. e.g. Speed
    /// </summary>
    public abstract class Parameter
    {
        // Static
        public static Type[] AllParameterTypes()
        {
            var TypeList = new List<Type>();
            foreach (Type t in typeof(Parameter).Assembly.GetTypes())
            {
                if (!t.IsAbstract && (t.IsSubclassOf(typeof(Parameter)) || t == typeof(Parameter)))
                {
                    // string theClass = t.FullName.Substring(t.FullName.LastIndexOf(".")+1);
                    // TypeNameList.Add(t.DisplayName);
                    TypeList.Add(t);
                }
            }
            return TypeList.ToArray();
        } // List of this field and all derived fields

        public enum ParameterStateEnum
        {
            NoDataReceived,
            IsNotAvailable,
            IsError,
            ValidValueReceived,
            Lost
        }
        
        // Properties
        public string DisplayName { get; set; }  // The name we display to the user
        public string InternalName { get; set; } // The name we use internally

        [DefaultValue(true)]
        [Description("Lock onto the first of possbly many streams providing messages that update this parameter? Otherwise, accept possibly contradictory messages over all streams.")]
        public bool LockFirstStream { get; set; }

        [DefaultValue(true)]
        [Description("Lock onto the first protocol (NMEA 2000 or 0183 or AIS) providing messages that update this parameter? Otherwise, accept possibly contradictory messages over multiple protocols.")]
        public bool LockFirstProtocol { get; set; }

        // Private
        protected readonly object parameterLocker = new object();
        protected ParameterStateEnum _unsafeState;
        protected DateTime _unsafeTimeStamp;
        protected DateTime _LastTimeStamp;
        protected bool _lockedOntoStream;
        protected bool _lockedOntoFormat;
        protected CANStreamer _lockedStream;
        protected MsgTypeEnum _lockedFormat;
        protected readonly object hookLocker = new object();

        // Methods for connecting
        public abstract void Connect(CANStreamer stream);
        public abstract void Disconnect(CANStreamer stream);

        // Hook filtering callback
        protected void HookFilter(object sender, FrameReceivedEventArgs e)
        {
            MsgHook hook = (MsgHook)sender;

            lock (hookLocker)
            {
                // Check stream lock
                if (LockFirstStream)
                {
                    if (_lockedOntoStream)
                    {
                        if (e.Stream != _lockedStream)
                            return;
                    }
                    else
                    {
                        _lockedStream = e.Stream;
                        _lockedOntoStream = true;
                    }
                }

                // Check format lock
                if (LockFirstProtocol)
                {
                    if (_lockedOntoFormat)
                    {
                        if (hook.Protocol != _lockedFormat)
                            return;
                    }
                    else
                    {
                        _lockedFormat = hook.Protocol;
                        _lockedOntoFormat = true;
                    }
                }

                if (!hook.CheckFilter(e)) return;
                // Hook passed filtering, fire callback
                HookCallback(sender, e);
            }
        }

        // Hook accepted callback
        protected virtual void HookCallback(object sender, FrameReceivedEventArgs e)
        {
        }

        public abstract Parameter Clone();

        public virtual ParameterStateEnum State()
        {
            if (_unsafeState!=ParameterStateEnum.NoDataReceived)
            {
                TimeSpan sinceLast = DateTime.Now-_LastTimeStamp;
                if (sinceLast.Seconds > 4) _unsafeState = ParameterStateEnum.Lost;
            }
            return _unsafeState; 
        }

        public DateTime LastTimeStamp() { return _LastTimeStamp;  }
    }

    public class DummyParameter : Parameter//, IToString
    {
        public DummyParameter()
        {
            _unsafeState = Parameter.ParameterStateEnum.IsNotAvailable;
        }

        public override Parameter Clone()
        {
            Parameter clone = (Parameter)this.MemberwiseClone();
            return clone;
        }
        public override void Connect(CANStreamer stream)
        {
        }
        public override void Disconnect(CANStreamer stream)
        {
        }
    }

    // TODO : Complete SatelliteParameter class
    /// <summary>
    /// Hooks into and decodes satellite information messages.
    /// </summary>
    public class SatelliteParameter : Parameter//, IToString
    {
        private class SatelliteInfo
        {
            public int PRN;
            public double Elevation;
            public double Azimuth;
            public double SNR;
            public double Resid;
        }

        private int _fragmentCount;
        private bool[] _haveFragment;
        private string[] _fragmentData;
        private int _fragmentsRecieved;

        private List<SatelliteInfo> _satellites = new List<SatelliteInfo> { };
        
        public override Parameter Clone()
        {
            // TODO : Confirm this copies correctly
            Parameter clone = (Parameter)this.MemberwiseClone();
            return clone;
        }
        public override void Connect(CANStreamer stream)
        {
            stream.PGNReceived[129540] += On129540Receieved;
            stream.TypeCodeReceived["GSV"] += OnGSVReceieved;
        }
        public override void Disconnect(CANStreamer stream)
        {
            stream.PGNReceived[129540] -= On129540Receieved;
            stream.TypeCodeReceived["GSV"] -= OnGSVReceieved;
        }

        public override string ToString()
        {
            lock (parameterLocker)
            {
                string retStr = "";
                foreach (SatelliteInfo sI in _satellites)
                {
                    retStr += "PRN=" + sI.PRN + "   Ev/Az=" + sI.Elevation + "/" + sI.Azimuth + "   SNR=" + sI.SNR + "\r\n";
                }
                return retStr;
            }
        }

        private void On129540Receieved(object sender, FrameReceivedEventArgs e)
        {
            N2kFrame n2kFrame = (N2kFrame)e.ReceivedFrame;
            N2kField[] fields = n2kFrame.Defn.Fields;

            // It is possible that no definition was loaded for 129540
            // This means it will have no fields defined, and hence no data to retrieve
            // We could hardcode the decoding here, but it would make a mess of the code (and de-isolate decoding)
            if (fields.Length < 4)
            {
                lock (parameterLocker)
                    _satellites.Clear();

                return;
            }

            FieldValueState fieldState;
            double satInViewFieldValue = ((N2kNumericField)fields[3]).GetValue(n2kFrame.Data, out fieldState);
            if (fieldState != FieldValueState.Valid)
            {
                lock (parameterLocker)
                    _satellites.Clear();

                return;
            }

            lock (parameterLocker)
            {
                _satellites.Clear();

                int satInView = (int)satInViewFieldValue;
                for (int satNumber = 0; satNumber < satInView; satNumber++)
                {
                    int fieldIndex = 4 + 7 * satNumber;
                    int satPRN = (int)((N2kNumericField)fields[fieldIndex]).GetValue(n2kFrame.Data, out fieldState);
                    double satElevation = ((N2kNumericField)fields[fieldIndex + 1]).GetValue(n2kFrame.Data, out fieldState);
                    double satAzimuth = ((N2kNumericField)fields[fieldIndex + 2]).GetValue(n2kFrame.Data, out fieldState);
                    double satSNR = ((N2kNumericField)fields[fieldIndex + 3]).GetValue(n2kFrame.Data, out fieldState);
                    double satResid = ((N2kNumericField)fields[fieldIndex + 4]).GetValue(n2kFrame.Data, out fieldState);
                    // The last 2 fields in this 7-block are spare and reserved respectively.

                    SatelliteInfo satInfo = new SatelliteInfo();
                    satInfo.PRN = satPRN;
                    satInfo.Elevation = satElevation;
                    satInfo.Azimuth = satAzimuth;
                    satInfo.SNR = satSNR;
                    satInfo.Resid = satResid;

                    _satellites.Add(satInfo);
                }
            }
        }
        private void OnGSVReceieved(object sender, FrameReceivedEventArgs e)
        {
            N0183Frame msg = (N0183Frame)e.ReceivedFrame;

            int fragmentCount = int.Parse(msg.Segments[0]);
            int fragmentNumber = int.Parse(msg.Segments[1]) - 1;

            if (fragmentCount != _fragmentCount || _haveFragment[fragmentNumber])
            {
                // Start new
                _fragmentCount = fragmentCount;
                _fragmentData = new string[_fragmentCount];
                _haveFragment = new bool[_fragmentCount];
                _fragmentsRecieved = 0;
            }

            _fragmentData[fragmentNumber] = string.Join(",", msg.Segments, 3, msg.Segments.Length - 3);
            _haveFragment[fragmentNumber] = true;
            _fragmentsRecieved++;

            if (_fragmentsRecieved == _fragmentCount)
            {
                // Done !
                string[] fields = string.Join(",", _fragmentData).Split(',');

                lock (parameterLocker)
                {
                    _satellites.Clear();
                    for (int i = 0; i <= fields.Length - 4; i += 4)
                    {
                        SatelliteInfo sI = new SatelliteInfo();
                        try
                        {
                            sI.PRN = -1;
                            sI.Elevation = -1;
                            sI.Azimuth = -1;
                            sI.SNR = -1;

                            if (fields[i] != "") sI.PRN = int.Parse(fields[i]);
                            if (fields[i + 1] != "") sI.Elevation = double.Parse(fields[i + 1]);
                            if (fields[i + 1] != "") sI.Azimuth = double.Parse(fields[i + 2]);
                            if (fields[i + 3] != "") sI.SNR = double.Parse(fields[i + 3]);
                        }
                        catch
                        {
                            // TODO: Report errors somehow
                        }
                        finally
                        {
                            _satellites.Add(sI);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Represents a parameter with a numeric (double) value.
    /// </summary>
    public class StringParameter : Parameter//, IToString
    {
        [Description("You can specify one or more NMEA 2000, 0183 or AIS messages that are used to set the value for your parameter.")]
        public MsgHook[] Hooks { get; set; }

        // Constructor
        public StringParameter()
        {
            Hooks = new MsgHook[0];
        }

        // Private
        private string _unsafeValue;
        private bool _updateRequired;
        private bool _valueChanged;

        // Delegates / events
        public delegate void ValueReceivedHandler(object sender, EventArgs e, ParameterStateEnum parameterState, string value, DateTime timeStamp);
            // Consumers of the messages need to implement this handler. Note that the values are passed to ensure the receiver
            // gets a clean copy of the values even in a multi-threaded environment.

        public event ValueReceivedHandler ValueReceived;    // A value has arrived
        public event ValueReceivedHandler ValueChanged;     // A new value has arrived that differs from the old value

        // Public methods
        public virtual bool ShouldSerializeFormatAs() { return true; } // FormatAs != FormatEnum.Number; }
        public override string ToString()
        {
            lock (parameterLocker)
            {
                return _unsafeValue;
            }
        }
        public string GetValueAndState(out ParameterStateEnum state, out DateTime timeStamp)
        {
            lock (parameterLocker)
            {
                state = _unsafeState;
                timeStamp = _unsafeTimeStamp;
                return _unsafeValue;
            }
        }

        // Private methods
        protected void OnValueReceived(object sender, EventArgs e, ParameterStateEnum newParameterState, string newValue, DateTime newTimeStamp)
        {
            // Begin updating, enter lock (if we havn't already)
            if (!_updateRequired)
            {
                Monitor.Enter(parameterLocker);
                _updateRequired = true;
            }

            _valueChanged = (newParameterState != _unsafeState) || (newValue != _unsafeValue);
            
            _unsafeState = newParameterState;
            _unsafeValue = newValue;
            _unsafeTimeStamp = newTimeStamp;
            _LastTimeStamp = DateTime.Now;
        }
        protected void OnParametersNotified(object sender, EventArgs e)
        {
            if (!_updateRequired) return;
            _updateRequired = false;

            if (ValueReceived != null)
                ValueReceived(this, EventArgs.Empty, _unsafeState, _unsafeValue, _unsafeTimeStamp);

            if (_valueChanged && (ValueChanged != null))
                ValueChanged(this, EventArgs.Empty, _unsafeState, _unsafeValue, _unsafeTimeStamp);

            // Finished updating, release lock
            Monitor.Exit(parameterLocker);
        }
        protected override void HookCallback(object sender, FrameReceivedEventArgs e)
        {
            MsgHook hook = (MsgHook)sender;
            Frame msg = e.ReceivedFrame;

            FieldValueState newState;
            if (hook.MsgField == null) { hook.MsgField = msg.GetField(hook.FieldName); }
            string newValue = msg.GetString(hook.MsgField, out newState);

            ParameterStateEnum newPState;
            if (newState == FieldValueState.Valid) newPState = ParameterStateEnum.ValidValueReceived;
            else if (newState == FieldValueState.NotAvailable) newPState = ParameterStateEnum.IsNotAvailable;
            else newPState = ParameterStateEnum.IsError;

            OnValueReceived(null, null, newPState, newValue, msg.TimeStamp);
        }

        public override void Connect(CANStreamer stream)
        {
            foreach (MsgHook hook in Hooks)
            {
                hook.FrameReceived += HookFilter;
                hook.HookOnto(stream);
            }

            stream.ParametersNotified += OnParametersNotified;
        }
        public override void Disconnect(CANStreamer stream)
        {
            foreach (MsgHook hook in Hooks)
            {
                hook.FrameReceived -= HookFilter;
                hook.UnhookFrom(stream);
            }

            stream.ParametersNotified -= OnParametersNotified;
        }

        public override Parameter Clone()
        {
            StringParameter clone = (StringParameter)this.MemberwiseClone();
            for (int i = 0; i < clone.Hooks.Length; i++)
                clone.Hooks[i] = this.Hooks[i].Clone();

            return (Parameter)clone;
        }
    }

    /*
    public class HistoryData
    {
        private DateTime[] _timeStamps;
        private double[] _values;
        private int _count;
        private int _last;

        public HistoryData(int size)
        {
            _timeStamps = new DateTime[size];
            _values = new double[size];
            _count = 0;
            _last = 0;
        }

        public void Add(DateTime timeStamp, double value)
        {
            if (_count < _timeStamps.Length) _count++;
        }

        public int First()
        {
            return 0;
        }

        public int Last()
        {
            return _last;
        }

        public bool Next(ref int i, out DateTime timeStamp, out double value)
        {
            return false;
        }
    }
     */
    /// <summary>
    /// Represents a parameter with a numeric (double) value.
    /// </summary>
    public abstract class NumericParameter : Parameter, IToDouble//, IToString
    {
        private DateTime[] _timeStamps;
        private double[] _values;
        private int _countHistory=0;
        private int _nextHistory=0;

        // Constructor
        public NumericParameter()
        {
        }

        public virtual double ToDouble()
        {
            lock (parameterLocker)
            {
                return _unsafeValue;
            }       
        }
        // Public
        public string Unit { get; set; } // The name we use internally
        public int KeepHistory {
            get
            {
                return HistorySize();
            }
            set 
            {
                _timeStamps = new DateTime[value];
                _values = new double[value];
                _countHistory = 0;
                _nextHistory = 0;
            }
        } // How many seconds of history data parameter will keep in memory.
        [XmlIgnore]
        [Browsable(false)]
        public Formatter Formatter
        {
            get
            {
                return _formatter;
            }
            set
            {
                _formatter = value;
            }
        }
        public FormatEnum FormatAs
        {
            get
            {
                return _FormatAs;
            }
            set
            {
                _FormatAs = value;
                Formatter = Formatter.Create(_FormatAs);
                _formatString = Formatter.FormatString;
            }
        }
        public string FormatString
        {
            get
            {
                return _formatString;
            }
            set
            {
                if (_formatString == value)
                    return;
                // Commented 4.10.2015. Why this should be changed? If formatter will be changed to
                // Custom, it does not allow user to change e.g. date or time format.
                //                FormatAs = FormatEnum.Custom; 
                _formatString = value;
                Formatter.FormatString = _formatString;
            }
        }

        // Private
        private Formatter _formatter = Formatter.Create(FormatEnum.Number);
        private double _unsafeValue;
        protected FormatEnum _FormatAs;
        private string _formatString;
        private bool _updateRequired;
        private bool _valueChanged;

        // Delegates / events
        public delegate void ValueReceivedHandler(object sender, EventArgs e, ParameterStateEnum parameterState, double value, DateTime timeStamp);
        // Consumers of the messages need to implement this handler. Note that the values are passed to ensure the receiver
        // gets a clean copy of the values even in a multi-threaded environment.

        public event ValueReceivedHandler ValueReceived;    // A value has arrived
        public event ValueReceivedHandler ValueChanged;     // A new value has arrived that differs from the old value

        // Public methods
        public virtual bool ShouldSerializeFormatAs() { return true; } // FormatAs != FormatEnum.Number; }
        public override string ToString()
        {
            lock (parameterLocker)
            {
                return Formatter.Format(_unsafeValue, _unsafeState);
            }
        }
        public double GetValueAndState(out ParameterStateEnum state, out DateTime timeStamp)
        {
            lock (parameterLocker)
            {
                state = _unsafeState;
                timeStamp = _unsafeTimeStamp;
                return _unsafeValue;
            }
        }

        public string GetHistory()
        {
            string history = "\"";
            int counter;
            int i;
            int endHistory;
            bool FirstPair = true;

            lock (parameterLocker)
            {
                counter = (_nextHistory - _countHistory);
                endHistory = Math.Min(_nextHistory, counter + 10);
                // Take first 10 within this lock. Since the buffer is ring buffer, writer could write
                // new values, while we are reading them. So we first read further, so writer won't reach us.
                // The max speed for incoming values is about 20Hz.
                for (; counter < endHistory; counter++)
                {
                    i = ( counter+HistorySize() ) % HistorySize();
                    if (!FirstPair) history += ",";
                    history += new DateTime(_timeStamps[i].Ticks, DateTimeKind.Local).ToString("o") + ',' + _values[i].ToString(CultureInfo.InvariantCulture);
                    FirstPair = false;
                }
                endHistory = _nextHistory;
            }
            // Then we take rest in loop so that lock will be realeased on each value
            for (; counter < endHistory; counter++)
            {
                lock (parameterLocker)
                {
                    i = (counter + HistorySize()) % HistorySize();
                    history += ",";
                    history += new DateTime(_timeStamps[i].Ticks, DateTimeKind.Local).ToString("o") + ',' + _values[i].ToString(CultureInfo.InvariantCulture);
                }
                Thread.Sleep(0);
            }
            // Maybe we should recheck here that there is no new data, while we read old.

            history += "\"";
            return history;
        }

        // Private methods
        protected int HistorySize()
        {
            return _values.Length;
        }

        protected void AddHistory(DateTime timeStamp, double value) 
        {
            lock (parameterLocker)
            {
                _timeStamps[_nextHistory]=timeStamp;
                _values[_nextHistory]=value;
                _nextHistory++;
                if (_nextHistory == _values.Length) _nextHistory = 0;
                if (_countHistory < _values.Length) _countHistory++;
            }
        }

        protected void OnValueReceived(object sender, EventArgs e, ParameterStateEnum newParameterState, double newValue, DateTime newTimeStamp)
        {
            // Begin updating, enter lock (if we havn't already)
            if (!_updateRequired)
            {
                Monitor.Enter(parameterLocker);
                _updateRequired = true;
            }

            _valueChanged = (newParameterState != _unsafeState) || (newValue != _unsafeValue);

            _unsafeState = newParameterState;
            _unsafeValue = newValue;
            _unsafeTimeStamp = newTimeStamp;
            _LastTimeStamp = DateTime.Now;
            if (HistorySize() > 0) AddHistory(_LastTimeStamp, _unsafeValue);
        }

        protected void OnParametersNotified(object sender, EventArgs e)
        {
            if (!_updateRequired) return;
            _updateRequired = false;

            if (ValueReceived != null)
                ValueReceived(this, EventArgs.Empty, _unsafeState, _unsafeValue, _unsafeTimeStamp);

            if (_valueChanged && (ValueChanged != null))
                ValueChanged(this, EventArgs.Empty, _unsafeState, _unsafeValue, _unsafeTimeStamp);

            // Finished updating, release lock
            Monitor.Exit(parameterLocker);
        }
    }

    /// <summary>
    /// Represents a parameter which updates from one or more hooks (i.e. sources)
    /// </summary>
    public class MultipleSourceNumeric : NumericParameter
    {
        [Description("You can specify one or more NMEA 2000, 0183 or AIS messages that are used to set the value for your parameter.")]
        public MsgHook[] Hooks { get; set; }

        public MultipleSourceNumeric()
        {
            Hooks = new MsgHook[0];
            FormatAs = FormatEnum.Number;
        }

        protected override void HookCallback(object sender, FrameReceivedEventArgs e)
        {
            MsgHook hook = (MsgHook)sender;
            Frame msg = e.ReceivedFrame;

            FieldValueState newState;
            if (hook.MsgField == null) { hook.MsgField = msg.GetField(hook.FieldName);  }
            double newValue = msg.GetDouble(hook.MsgField, out newState) * hook.Multiplier;

            ParameterStateEnum newPState;
            if (newState == FieldValueState.Valid) newPState = ParameterStateEnum.ValidValueReceived;
            else if (newState == FieldValueState.NotAvailable) newPState = ParameterStateEnum.IsNotAvailable;
            else newPState = ParameterStateEnum.IsError;

            OnValueReceived(null, null, newPState, newValue, msg.TimeStamp);
        }

        public override void Connect(CANStreamer stream)
        {
            foreach (MsgHook hook in Hooks)
            {
                hook.FrameReceived += HookFilter;
                hook.HookOnto(stream);
            }

            stream.ParametersNotified += OnParametersNotified;
        }
        public override void Disconnect(CANStreamer stream)
        {
            foreach (MsgHook hook in Hooks)
            {
                hook.FrameReceived -= HookFilter;
                hook.UnhookFrom(stream);
            }

            stream.ParametersNotified -= OnParametersNotified;
        }

        public override Parameter Clone()
        {
            MultipleSourceNumeric clone = (MultipleSourceNumeric)this.MemberwiseClone();
            for (int i = 0; i < clone.Hooks.Length; i++)
                clone.Hooks[i] = this.Hooks[i].Clone();

            return (Parameter)clone;
        }
    }

    /// <summary>
    /// Represents a parameter which has a value determined by the calculation in two hooks (i.e. sources)
    /// </summary>
    public abstract class NumericCalc:NumericParameter
    {
        public MsgHook Source1 { get; set; }
        public MsgHook Source2 { get; set; }

        private double _Value1;
        private double _Value2;

        private ParameterStateEnum _State1 = ParameterStateEnum.NoDataReceived;
        private ParameterStateEnum _State2 = ParameterStateEnum.NoDataReceived;

        public NumericCalc()
        {
            Source1 = new MsgHook();
            Source2 = new MsgHook();
            FormatAs = FormatEnum.Number;
        }

        public void Source1Callback(Frame msg, MsgHook hook)
        {
            FieldValueState newState;
            _Value1 = msg.GetDouble(hook.FieldName, out newState) * hook.Multiplier;

            switch (newState)
            {
                case FieldValueState.Valid:
                    _State1 = ParameterStateEnum.ValidValueReceived;
                    break;

                case FieldValueState.NotAvailable:
                    _State1 = ParameterStateEnum.IsNotAvailable;
                    break;

                case FieldValueState.Error:
                    _State1 = ParameterStateEnum.IsError;
                    break;
            }

            UpdateValue();
        }
        public void Source2Callback(Frame msg, MsgHook hook)
        {
            FieldValueState newState;
            _Value2 = msg.GetDouble(hook.FieldName, out newState) * hook.Multiplier;

            switch (newState)
            {
                case FieldValueState.Valid:
                    _State2 = ParameterStateEnum.ValidValueReceived;
                    break;

                case FieldValueState.NotAvailable:
                    _State2 = ParameterStateEnum.IsNotAvailable;
                    break;

                case FieldValueState.Error:
                    _State2 = ParameterStateEnum.IsError;
                    break;
            }

            UpdateValue();
        }

        public void UpdateValue()
        {
            ParameterStateEnum finalState;
            double finalValue = 0.0;

            if (_State1 == ParameterStateEnum.IsError || _State2 == ParameterStateEnum.IsError)
            {
                finalValue = 0.0;
                finalState = ParameterStateEnum.IsError;
            }
            else if (_State1 == ParameterStateEnum.ValidValueReceived && _State2 == ParameterStateEnum.ValidValueReceived)
            {
                finalValue = Calc(_Value1,_Value2);
                finalState = ParameterStateEnum.ValidValueReceived;
            }
            else
            {
                finalValue = 0.0;
                finalState = ParameterStateEnum.IsNotAvailable;
            }

            OnValueReceived(null, null, finalState, finalValue, DateTime.Now);
        }

        protected override void HookCallback(object sender, FrameReceivedEventArgs e)
        {
            MsgHook hook = (MsgHook)sender;
            if (hook == Source1)
            {
                Source1Callback(e.ReceivedFrame, hook);
            }
            else if (hook == Source2)
            {
                Source2Callback(e.ReceivedFrame, hook);
            }
        }

        public override void Connect(CANStreamer stream)
        {
            Source1.FrameReceived += base.HookFilter;
            Source2.FrameReceived += base.HookFilter;

            Source1.HookOnto(stream);
            Source2.HookOnto(stream);

            stream.ParametersNotified += OnParametersNotified;
        }
        public override void Disconnect(CANStreamer stream)
        {
            Source1.FrameReceived -= base.HookFilter;
            Source2.FrameReceived -= base.HookFilter;

            Source1.UnhookFrom(stream);
            Source2.UnhookFrom(stream);

            stream.ParametersNotified -= OnParametersNotified;
        }

        public override Parameter Clone()
        {
            NumericCalc clone = (NumericCalc)this.MemberwiseClone();
            clone.Source1 = this.Source1.Clone();
            clone.Source2 = this.Source2.Clone();
            
            return (Parameter)clone;
        }

        public abstract double Calc(double val1, double val2);
    }

    /// <summary>
    /// Represents a parameter which has a value determined by the difference in two hooks (i.e. sources)
    /// </summary>
    public class NumericDifference:NumericCalc
    {
        public NumericDifference():base()
        {
        }

        public override double Calc(double val1, double val2)
        {
            return val1-val2;
        }

        public override Parameter Clone()
        {
            NumericDifference clone = (NumericDifference)this.MemberwiseClone();
            clone.Source1 = this.Source1.Clone();
            clone.Source2 = this.Source2.Clone();

            return (Parameter)clone;
        }

    }

    /// <summary>
    /// Represents a parameter which has a value determined by the multiplication of two hooks (i.e. sources)
    /// </summary>
    public class NumericMultiplication : NumericCalc
    {
        public NumericMultiplication()
            : base()
        {
        }

        public override double Calc(double val1, double val2)
        {
            return val1*val2;
        }

        public override Parameter Clone()
        {
            NumericMultiplication clone = (NumericMultiplication)this.MemberwiseClone();
            clone.Source1 = this.Source1.Clone();
            clone.Source2 = this.Source2.Clone();

            return (Parameter)clone;
        }
    }

    /*public abstract class VectorParameter : Parameter
    {
        private List<double> _Value;

        [BrowsableAttribute(false)]
        public List<double> Value
        {
            get { return _Value; }
        }

        public override string ToString()
        {
            switch (_ParameterState)
            {
                case ParameterStateEnum.IsError:
                    return "Error";
                case ParameterStateEnum.IsNotAvailable:
                    return "IsNotAvailable";
                case ParameterStateEnum.NoDataReceived:
                    return "No Data Received";
                default:
                    string retStr = "";
                    for (int i = 0; i < _Value.Count; i++)
                    {
                        retStr += _Value[i].ToString() + ", ";
                    }
                    return retStr.Substring(0, retStr.Length - 2);
            }
        }

        public delegate void ValueReceivedHandler(object sender, EventArgs e, ParameterStateEnum parameterState, List<double> value, DateTime timeStamp);
        // Consumers of the messages need to implement this handler. Note that the values are passed to ensure the receiver
        // gets a clean copy of the values even in a multi-threaded environment.

        public event ValueReceivedHandler ValueReceived;    // A value has arrived
        public event ValueReceivedHandler ValueChanged;     // A new value has arrived that differs from the old value
        // Consumers of the messages should connect to this
        
        // Creator
        public VectorParameter()
        {
        }

        // Call this to inform our listeners of a value changed
        protected void OnValueReceived(object sender, EventArgs e, ParameterStateEnum newParameterState, List<double> newValue, DateTime newTimeStamp)
        {
            bool valueIsChanged = (newParameterState != _ParameterState) || (newValue[0] != _Value[0]) || (newValue[1] != _Value[1]);
            _ParameterState = newParameterState;
            _Value = newValue;
            _ValueTimeStamp = newTimeStamp;
            if (ValueReceived != null)
            {
                ValueReceived(this, e, _ParameterState, Value, _ValueTimeStamp);
            }
            if (valueIsChanged && (ValueChanged != null))
            {
                ValueChanged(this, e, _ParameterState, Value, _ValueTimeStamp);
            }
        }

        // Default received handler
        /*
        public override void PGNReceivedHandler(object sender, EventArgs e, Frame fmsg)
        {
            var msg = (N2kFrame)fmsg;
            foreach (var dataSource in GetHooks)
            {
                if ((dataSource.PGN == ((N2kHeader)msg.Header).PGN) && ((dataSource.Source == SourceAny) || (dataSource.Source == msg.Header.Source)))
                {
                    NumericField field = (NumericField)((PGNDefn)msg.Defn).Fields[dataSource.FieldIndex];
                    ParameterStateEnum NewParameterState;
                    if (field.IsError(msg.Data))
                    {
                        NewParameterState = ParameterStateEnum.IsError;
                    }
                    else if (field.IsNotAvailable(msg.Data))
                    {
                        NewParameterState = ParameterStateEnum.IsNotAvailable;
                    }
                    else
                    {
                        NewParameterState = ParameterStateEnum.ValidValueReceived;
                    }

                    double degrees = field.DblValue(msg.Data); // TODO Only try convert if not error and available ?
                    double radians = degrees * (Math.PI / 180);

                    // Value in degrees, convert to 2d vector
                    double vx = Math.Sin(radians);
                    double vy = Math.Cos(radians);
                    double vl = Math.Sqrt(vx * vx + vy * vy);

                    OnValueReceived(sender, e, NewParameterState, new List<double> { vx / vl, vy / vl } , msg.TimeStamp);
                }
            }
        }
    }
    */

    /*public class TwoDVector : VectorParameter
    {
        public MsgHook Source { get; set; }

        public TwoDVector()
        {
            Source = new N2kHook();
        }

        public override MsgHook[] GetHooks()
        {
            return new MsgHook[] { Source };
        }
    }

    public class ScaledVector : VectorParameter
    {
        public MsgHook Magnitude { get; set; }
        public MsgHook Direction { get; set; }

        private ParameterStateEnum _State1;
        private ParameterStateEnum _State2;

        private double _Magnitude;
        private double _Vx;
        private double _Vy;

        public ScaledVector()
        {
            Magnitude = new N2kHook();
            Direction = new N2kHook();
        }

        public override MsgHook[] GetHooks()
        {
            return new MsgHook[] { Magnitude, Direction };
        }

        /*
        public override void PGNReceivedHandler(object sender, EventArgs e, Frame fmsg)
        {
            var msg = (N2kFrame)fmsg;
            if ((((N2kHeader)msg.Header).PGN == Magnitude.PGN) && (Magnitude.Source == SourceAny || Magnitude.Source == msg.Header.Source))
            {
                NumericField field = (NumericField)((PGNDefn)msg.Defn).Fields[Magnitude.FieldIndex];
                if (field.IsError(msg.Data))
                {
                    _State1 = ParameterStateEnum.IsError;
                }
                else if (field.IsNotAvailable(msg.Data))
                {
                    _State1 = ParameterStateEnum.IsNotAvailable;
                }
                else
                {
                    _State1 = ParameterStateEnum.ValidValueReceived;
                    _Magnitude = field.DblValue(msg.Data);
                }
            }

            if ((((N2kHeader)msg.Header).PGN == Direction.PGN) && (Direction.Source == SourceAny || Direction.Source == msg.Header.Source))
            {
                NumericField field = (NumericField)((PGNDefn)msg.Defn).Fields[Direction.FieldIndex];
                if (field.IsError(msg.Data))
                {
                    _State2 = ParameterStateEnum.IsError;
                }
                else if (field.IsNotAvailable(msg.Data))
                {
                    _State2 = ParameterStateEnum.IsNotAvailable;
                }
                else
                {
                    _State2 = ParameterStateEnum.ValidValueReceived;
                    
                    double degrees = field.DblValue(msg.Data);
                    double radians = degrees * (Math.PI / 180);
                    double vx = Math.Sin(radians);
                    double vy = Math.Cos(radians);
                    double vl = Math.Sqrt(vx * vx + vy * vy);

                    _Vx = vx / vl;
                    _Vy = vy / vl;
                }
            }

            ParameterStateEnum finalState;

            // Are both values valid ? If so, set final value
            if ((_State1 == ParameterStateEnum.ValidValueReceived) && (_State2 == ParameterStateEnum.ValidValueReceived))
            {
                finalState = ParameterStateEnum.ValidValueReceived;
            }
            else
            {
                // If one of the fields is an error, this parameter is an error
                if ((_State1 == ParameterStateEnum.IsError) || (_State2 == ParameterStateEnum.IsError))
                {
                    finalState = ParameterStateEnum.IsError;
                }
                else
                {
                    // Either of the fields is not available (Or not received), so this parameter is not available
                    finalState = ParameterStateEnum.IsNotAvailable;
                }
            }

            // Fire received with updated info (Which will change the base value)
            OnValueReceived(this, e, finalState, new List<double> { _Magnitude * _Vx, _Magnitude * _Vy }, msg.TimeStamp);
        }
     * 
    }
    */

    /// <summary>
    /// A collection of parameter objects. Which are cloned out to use.
    /// </summary>
    public class ParameterCollection
    {
        public const string DefDefnFile = "Parameters.N2kParams.xml";
        public readonly object UpdateLocker = new object();

        [XmlIgnore]
        public BindingList<Parameter> ClonedParameters = new BindingList<Parameter> { };
        [XmlIgnore]
        public Dictionary<KeyValuePair<string, string>, Parameter> CopyDict = new Dictionary<KeyValuePair<string, string>, Parameter> { };

        [XmlIgnore]
        [ReadOnly(true)]
        // The file we loaded the definitions from
        public string FileName { get; set; }
        [XmlIgnore]
        [ReadOnly(true)]
        // The file we loaded the definitions from
        public DateTime FileDateTime { get; set; }

        [XmlIgnore]
        public FileTypeEnum FileType { get; set; }

        private Parameter[] _Parameters;
        [XmlArray]
        [XmlArrayItem("StringParameter", typeof(StringParameter))]
        [XmlArrayItem("MultiSourceNumeric", typeof(MultipleSourceNumeric))]
        [XmlArrayItem("NumericDifference", typeof(NumericDifference))]
        [XmlArrayItem("NumericMultiplication", typeof(NumericMultiplication))]
        [XmlArrayItem("SatelliteInfo", typeof(SatelliteParameter))]
        public Parameter[] Parameters
        {
            get
            {
                return _Parameters;
            }
            set
            {
                _Parameters = value ?? new Parameter[0];
                BuildInternalStructures();
            }
        }

        [XmlIgnore]
        public Dictionary<string, Parameter> ParameterFromName;

        // Constructor
        public ParameterCollection()
        {
            ParameterFromName = new Dictionary<string, Parameter> { }; 
            Parameters = new Parameter[] { };
            FileName = "";
            FileDateTime = new DateTime();
        }

        public bool IsChanged(string newFileName)
        {
            newFileName = CommonRoutines.ResolveFileNameIfEmpty(newFileName, DefDefnFile);
            return ((newFileName != FileName) || (FileName == "") || (File.GetLastWriteTime(FileName) != FileDateTime));
        }

        /// <summary>
        /// Factory method for retrieving copies of parameters
        /// </summary>
        /// <param name="parameterName">The internal name of the parameter</param>
        /// <param name="streamsToMatch">Comma deliminated streams to match</param>
        /// <returns>Working copy of parameter</returns>
        public Parameter GetCopy(string parameterName, string streamsToMatch)
        {
            Parameter copy;
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(parameterName, streamsToMatch);
            if (!CopyDict.TryGetValue(kvp, out copy))
            {
                Parameter orig;
                if (!ParameterFromName.TryGetValue(parameterName, out orig))
                    return null;

                copy = orig.Clone();
                // copy.InternalName += " (" + streamsToMatch + ")";
                CopyDict.Add(kvp, copy);
                ClonedParameters.Add(copy);
            }

            return copy;
        }
        public void AddParameter(Parameter p)
        {
            Array.Resize(ref _Parameters, _Parameters.Length + 1);
            _Parameters[_Parameters.Length - 1] = p;
        }
        public void DeleteParameter(int idx)
        {
            for (int i = idx; i < _Parameters.Length - 1; i++)
                _Parameters[i] = _Parameters[i + 1];

            Array.Resize(ref _Parameters, _Parameters.Length - 1);
        }

        // Serialization
        private static XmlFileSerializer<ParameterCollection> XmlFileSerializer = new XmlFileSerializer<ParameterCollection>();

        // File IO
        public static ParameterCollection LoadFromFile(string fileName)
        {
            fileName = CommonRoutines.ResolveFileNameIfEmpty(fileName, DefDefnFile);
            if (fileName == "") return LoadInternal();

            ParameterCollection paramCol = XmlFileSerializer.Deserialize(fileName);
            if (paramCol != null)
            {
                paramCol.FileName = fileName;
                paramCol.FileType = FileTypeEnum.NativeXMLFile;
                return paramCol;
            }

            return LoadInternal();
        }
        public static ParameterCollection LoadInternal()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("OpenSkipperApplication.Resources.Parameters.N2kParams.xml");

            ParameterCollection newDefns = XmlFileSerializer.Deserialize(stream);
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

        // Internal structs
        private void BuildInternalStructures()
        {
            ParameterFromName.Clear();
            foreach (Parameter p in Parameters)
            {
                if (ParameterFromName.ContainsKey(p.InternalName))
                {
                    // Duplicate internal names, give warning (Duplicate is overwritten)
                    MessageBox.Show("Duplicate parameter internal name '" + p.InternalName + "', internal names must be unique", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                ParameterFromName[p.InternalName] = p;
            }
        }
    }
}
