using CANHandler;
using CANStreams;
using OpenSkipperApplication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Parameters
{
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
        [Description("Lock onto the first of possibly many streams providing messages that update this parameter? Otherwise, accept possibly contradictory messages over all streams.")]
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
            if (_unsafeState != ParameterStateEnum.NoDataReceived)
            {
                TimeSpan sinceLast = DateTime.Now - _LastTimeStamp;
                if (sinceLast.Seconds > 4) _unsafeState = ParameterStateEnum.Lost;
            }
            return _unsafeState;
        }

        public DateTime LastTimeStamp() { return _LastTimeStamp; }
    }

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

} // namespace
