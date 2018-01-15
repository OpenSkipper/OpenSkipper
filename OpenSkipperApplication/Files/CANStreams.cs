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
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using CANHandler;
using OpenSkipperApplication;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using CANReaders;
using CANSenders;
using Parameters;
using CANDefinitions;

namespace CANStreams
{
    // Enums
    public enum ConnectionStateEnum
    {
        Disconnected,
        Connected
    }

    // EventArgs
    public class FrameReceivedEventArgs : EventArgs
    {
        public Frame ReceivedFrame
        {
            get
            {
                return _receivedFrame;
            }
        }
        public CANStreamer Stream
        {
            get
            {
                return _stream;
            }
        }

        private Frame _receivedFrame;
        private CANStreamer _stream;

        public FrameReceivedEventArgs(Frame receivedFrame, CANStreamer originatingStream)
        {
            _receivedFrame = receivedFrame;
            _stream = originatingStream;
        }
    }

    /// <summary>
    /// This is the base class for processing a stream of incoming raw NMEA data and forwarding the processed messages
    /// It is used to feed consumers of the the NMEA messages.
    /// Messages are delivered as they arrive via FrameReceived events, and the more specific PGNReceived events
    /// </summary>
    [XmlInclude(typeof(CANStreamer_KeesLogfile))]
    [XmlInclude(typeof(CANStreamer_XMLLogfile))]
    [XmlInclude(typeof(CANStreamer_WeatherLogfile))]
    [XmlInclude(typeof(CANStreamer_N0183Logfile))]
    [XmlInclude(typeof(CANStreamer_NGT1_2000))]
    [XmlInclude(typeof(CANStreamer_N0183))]
    [XmlInclude(typeof(CANStreamer_Network))]
    [XmlInclude(typeof(CANStreamer_Logger))]
    [XmlInclude(typeof(CANStreamer_Generator))]
    public abstract class CANStreamer : INotifyPropertyChanged
    {
        // Static
        private static int _nextArrivalID = 0;

        // Public properties
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public abstract string Type { get; } // Human readable type, i.e. "Log file", "COM port", "IP", etc.

        [XmlIgnore]
        public ConnectionStateEnum ConnectionState
        // Warning: _connectionState is manipulated directly when sending all the frames in a stream
        {
            get
            {
                return _connectionState;
            }
            protected set
            {
                LastState = value;
                _connectionState = value;
                NotifyPropertyChanged("ConnectionState");
            }
        }
        public ConnectionStateEnum LastState { get; set; } // Public set due to xmlserializer... No need to/shouldnt alter from outside class
        public virtual bool CanRead { get { return true; } }
        public virtual bool CanWrite { get { return false; } }
        public int ID { get { return _ID; } set { _ID = value; } }

        // Private vars
        private int _ID;
        private string _Name;
        protected volatile ConnectionStateEnum _connectionState = ConnectionStateEnum.Disconnected;
        protected readonly IncomingMessageHandler MessageHandler;
        protected readonly object connectionLocker = new object();

        // Events
        public event EventHandler<FrameReceivedEventArgs> RawReceived;
        public event EventHandler<FrameReceivedEventArgs> FrameReceived;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ParametersNotified;

        // Event dictionaries (for PGN/Typecode specific handling)
        [XmlIgnore] public EventDictionary<int, FrameReceivedEventArgs> PGNReceived;
        [XmlIgnore] public EventDictionary<string, FrameReceivedEventArgs> TypeCodeReceived;
        [XmlIgnore] public EventDictionary<int, FrameReceivedEventArgs> AISReceived;

        // Constructor
        protected CANStreamer()
        {
            MessageHandler = new IncomingMessageHandler();
            PGNReceived = new EventDictionary<int, FrameReceivedEventArgs>();
            TypeCodeReceived = new EventDictionary<string, FrameReceivedEventArgs>();
            AISReceived = new EventDictionary<int, FrameReceivedEventArgs>();
        }

        // Methods
        protected void OnRawReceived(Frame msg)
        {
            if (ConnectionState == ConnectionStateEnum.Disconnected) return;    // Multi-threading protection

            var e = RawReceived;    // Multi-threading protection 
            if (e != null)
                e(this, new FrameReceivedEventArgs(msg, this));

            // Tag the message.
            msg.ArrivalID = Interlocked.Increment(ref _nextArrivalID);
            msg.OriginName = _Name;

            Frame decodedPacket = MessageHandler.DecodeMessage(msg);

            if (decodedPacket != null)
                OnFrameReceived(decodedPacket);
        }
        private void OnFrameReceived(Frame msg)
        {
            if (ConnectionState == ConnectionStateEnum.Disconnected) return;    // Multi-threading protection

            // Fire handler for generic message received
            var e = FrameReceived;
            if (e != null)
                e(this, new FrameReceivedEventArgs(msg, this));

            // Lock parameter locker (Forces parameters to update from only 1 message at a time)
            lock (Definitions.ParamCol.UpdateLocker)
            {
                // Look at more specific handlers
                N2kFrame n2kMsg = msg as N2kFrame;
                if (n2kMsg != null)
                {
                    PGNReceived.RaiseEvent(n2kMsg.Header.PGN, this, new FrameReceivedEventArgs(n2kMsg, this));
                }
                else
                {
                    N0183Frame n0183Msg = msg as N0183Frame;
                    if (n0183Msg != null)
                    {
                        TypeCodeReceived.RaiseEvent(n0183Msg.Header.TypeCode, this, new FrameReceivedEventArgs(n0183Msg, this));
                    }
                    else
                    {
                        AISFrame aisMsg = msg as AISFrame;
                        if (aisMsg != null)
                        {
                            AISReceived.RaiseEvent(aisMsg.Header.MessageID, this, new FrameReceivedEventArgs(aisMsg, this));
                        }
                    }
                }

                // Finished notifying parameters
                if (ParametersNotified != null)
                    ParametersNotified(this, EventArgs.Empty);
            }
        }
        protected void NotifyPropertyChanged(string propertyName)
        {
            var e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void SendFrame(Frame msg)
        {
            throw new NotSupportedException();
        }
        public abstract void ConnectStream();
        public abstract void Disconnect();
    }
    
    /// <summary>
    /// Base class for all file-based streams. Derived classes handle file processing
    /// </summary>
    public abstract class CANStreamer_Logfile : CANStreamer
    {
        public static CANStreamer_Logfile OpenNew()
        {
            string[] filters = new string[] {CANStreamer_XMLLogfile.FileFilter,
                                                CANStreamer_KeesLogfile.FileFilter,
                                                CANStreamer_N0183Logfile.FileFilter,
                                                CANStreamer_WeatherLogfile.FileFilter};
            
            OpenFileDialog oDialog = new OpenFileDialog();
            oDialog.Filter = string.Join("|", filters);
            if ((oDialog.ShowDialog() == DialogResult.OK) && (oDialog.CheckFileExists))
            {
                switch (oDialog.FilterIndex)
                {
                    case 1:
                        return new CANStreamer_XMLLogfile() { FileName = oDialog.FileName, Name = Path.GetFileNameWithoutExtension(oDialog.FileName) };

                    case 2:
                        return new CANStreamer_KeesLogfile() { FileName = oDialog.FileName, Name = Path.GetFileNameWithoutExtension(oDialog.FileName) };

                    case 3:
                        return new CANStreamer_N0183Logfile() { FileName = oDialog.FileName, Name = Path.GetFileNameWithoutExtension(oDialog.FileName) };

                    case 4:
                        return new CANStreamer_WeatherLogfile() { FileName = oDialog.FileName, Name = Path.GetFileNameWithoutExtension(oDialog.FileName) };

                    default:
                        throw new Exception("Invalid selection");
                }
            }

            return null;
        }

        // Properties
        public virtual string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                NotifyPropertyChanged("FileName");
            }
        }
        public override string Type
        {
            get { return "Logfile"; }
        }
        [XmlIgnore]
        public float PlaySpeed
        {
            get { return _playSpeed; }
            set
            {
                _playSpeed = value;
                NotifyPropertyChanged("PlaySpeed");
            }
        }
        public DateTime LogTime
        {
            get
            {
                return _logTime;
            }
        }
        [XmlIgnore]
        public bool Paused
        {
            get
            {
                return _paused;
            }
            set
            {
                lock (pauseLocker)
                {
                    _paused = value;

                    if (!_paused)
                        Monitor.PulseAll(pauseLocker);
                }

                NotifyPropertyChanged("Paused");
                NotifyPropertyChanged("Status");
            }
        }
        public string Status
        {
            get
            {
                if (ConnectionState == ConnectionStateEnum.Disconnected)
                {
                    return "Closed";
                }
                else
                {
                    if (_paused)
                    {
                        return "Paused";
                    }
                    else
                    {
                        return "Playing";
                    }
                }
            }
        }

        // Vars
        protected string _fileName = null;
        protected FileStream _fileStream = null;
        protected LogControlForm _controlPanel = null;
        protected volatile float _playSpeed = 1.0f;
        protected volatile bool _paused = false;

        // Variables for streaming frames
        protected DateTime _logTime;
        protected DateTime _lastTime;
        protected Frame nextPacket; // Holds the next packet that will be sent
        protected AutoResetEvent disconnectFlag = new AutoResetEvent(false);
        protected AutoResetEvent sendNextFlag = new AutoResetEvent(false);
        protected readonly object packetLocker = new object();
        protected readonly object pauseLocker = new object();

        // Constructors
        public CANStreamer_Logfile()
            : base()
        {
            _controlPanel = new LogControlForm(this);
        }

        // Stream simulator thread
        private Thread messageStreamer;
        private void streamMessages()
        {
            // Infinite loop, exits when getNextPacket is null
            while (true)
            {
                // Increase log time
                DateTime currentTime = DateTime.Now;
                _logTime = _logTime.AddTicks((long)((currentTime - _lastTime).Ticks * _playSpeed));
                _lastTime = currentTime;

                lock (pauseLocker)
                {
                    if (_paused)
                    {
                        do
                        {
                            // Check disconnect before we block
                            if (disconnectFlag.WaitOne(0))
                                return;

                            // Wait for pulse
                            Monitor.Wait(pauseLocker);

                        } while (_paused);

                        _lastTime = DateTime.Now;
                    }
                }

                long msToNext;

                // Outer lock 
                lock (packetLocker)
                {
                    // Loop while the next packet should have been simulated
                    while ((msToNext = (nextPacket.TimeStamp - _logTime).Milliseconds) <= 0)
                    {
                        OnRawReceived(nextPacket);

                        nextPacket = getNextPacket();
                        if (nextPacket == null)
                            return;
                    }
                }
                
                // Sanity check on time to wait
                if (msToNext < 0)
                {
                    // TODO : Inform user that logfile timestamps suddenly go backwards, recommend breaking up logfile.
                    return;
                }
                else if (msToNext > Int32.MaxValue)
                {
                    // TODO : Inform user that logfile timestamps suddenly just foward over 25 days, recommend breaking up logfile.
                    // Note: Attempting to wait over int32.maxvalue milliseconds will throw an exception.
                    return;
                }

                // Wait for disconnect, until it is time to send next packet
                // WaitOne returns true if it recieves signal before timeout.
                if (disconnectFlag.WaitOne((int)msToNext))
                    return;
            }
        }

        /// <summary>
        /// Sends the next packet (i.e. stepping through packets when paused)
        /// </summary>
        public void SendNextPacket()
        {
            lock (packetLocker)
            {
                if (nextPacket == null)
                    return;

                OnRawReceived(nextPacket);

                nextPacket = getNextPacket();
                if (nextPacket == null)
                    Disconnect();
            }
        }
        public void SendAllFrames()
        {
            if (_fileStream == null)
            {
                // Attempt to open the file
                try
                {
                    _fileStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading file '" + FileName + "': " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Init();

            lock (packetLocker)
            {
                // We pretend that this stream is connected by setting _connectionState = true; Otherwise, no subscribers are notified of the messages
                var old_connectionState = _connectionState;
                _connectionState = ConnectionStateEnum.Connected;    // WARNING: Direct access to an internal private variable. TODO: Do this differently

                try {

                    if (nextPacket == null)
                        nextPacket = getNextPacket();

                    while (nextPacket != null) {
                        OnRawReceived(nextPacket);
                        nextPacket = getNextPacket();
                    }
                }
                finally {
                    _connectionState = old_connectionState;
                }
            }
        }

        // Grabs next packet from stream, returns null if no packets left
        public abstract Frame getNextPacket();

        // Rewinds to start of file, continues playing.
        public abstract void Rewind();

        // Starting/stopping stream - mainly setting up message thread
        public override void ConnectStream()
        {
            if (ConnectionState == ConnectionStateEnum.Connected)
                return;

            // Attempt to open the file
            try
            {
                _fileStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
            }
            catch (IOException e)
            {
                if (MessageBox.Show("Error opening file: " + e.Message + "\n\n" + "Remove stream from list?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    StreamManager.RemoveStream(this);
                }
                return;
            }

            // Connecting...
            Init();

            // Only one logfile is allowed to be connected at a time
            StreamManager.DisconnectLogfiles();

            // Reinitialize
            MessageHandler.Reset();
            disconnectFlag.Reset();

            // Grab the first packet from file
            nextPacket = getNextPacket();
            if (nextPacket == null)
            {
                MessageBox.Show("Logfile contains no packets", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            _logTime = nextPacket.TimeStamp;
            _lastTime = DateTime.Now;

            // Start up streaming thread
            messageStreamer = new Thread(new ThreadStart(streamMessages));
            messageStreamer.Name = "LogStreamer: " + _fileName;
            messageStreamer.Start();

            // Done
            ConnectionState = ConnectionStateEnum.Connected;
            NotifyPropertyChanged("Status");
        }
        public override void  Disconnect()
        {
            if (ConnectionState == ConnectionStateEnum.Disconnected)
                return;

            _fileStream.Dispose();

            // Due to lock. Streamer is either: a) sending packets, b) waiting for disconnect, c) waiting for unpause.
            // Setting the flag covers a) and b)
            // Unpausing moves it out of c) and onto hitting either a) or b)
            // Lock also ensures both of these operations complete before any other code continues (Other code is waiting for lock)
            lock (pauseLocker)
            {
                disconnectFlag.Set();
                Monitor.PulseAll(pauseLocker);
            }

            messageStreamer.Join();

            ConnectionState = ConnectionStateEnum.Disconnected;
            NotifyPropertyChanged("Status");
        }

        protected virtual void Init()
        {
        }
        protected virtual void DeInit()
        {
            _fileStream.Close();
        }
    }
    public class CANStreamer_XMLLogfile : CANStreamer_Logfile
    {
        /*public static List<Frame> GetAllFrames(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                XmlReader xmlr = XmlReader.Create(fs, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment });
                List<Frame> frameList = new List<Frame> { };

                while (xmlr.Read())
                {
                    if (xmlr.Name == "N2kMsg")
                    {
                        frameList.Add((N2kFrame)N2kFrame.XmlSerializer.Deserialize(new StringReader(xmlr.ReadOuterXml())));
                    }
                    else if (xmlr.Name == "N0183Msg")
                    {
                        frameList.Add((N0183Frame)N0183Frame.XmlSerializer.Deserialize(new StringReader(xmlr.ReadOuterXml())));
                    }
                }
            }

            return frameList;
        }
        */

        // Properties
        public static string FileFilter = "XML Log File (*.xml)|*.xml";

        // Variables for reading the frames
        private System.Xml.XmlReader xmlReader;
        
        // Constructors
        public CANStreamer_XMLLogfile()
            : base()
        {
        }

        // Gets next packet from file
        public override Frame getNextPacket()
        {
            while ( true )
            {
                try
                {
                    if (!xmlReader.Read())
                        return null;
                }
                catch
                {
                    return null;
                }

                Frame newPacket;
                if (xmlReader.Name == "N2kMsg")
                {
                    try
                    {
                        var outerXml = xmlReader.ReadOuterXml();
                        var outerReader = new StringReader(outerXml);
                        newPacket = (N2kFrame)N2kFrame.XmlSerializer.Deserialize(outerReader);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error reading logfile: " + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                        return null;
                    }
                    return newPacket;
                }
                if (xmlReader.Name == "N0183Msg")
                {
                    try
                    {
                        string outerXml = xmlReader.ReadOuterXml();
                        StringReader outerReader = new StringReader(outerXml);
                        newPacket = (N0183Frame)N0183Frame.XmlSerializer.Deserialize(outerReader);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error reading logfile: " + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                        return null;
                    }
                    return newPacket;
                }
                if (xmlReader.Name == "AISMsg")
                {
                    try
                    {
                        string outerXml = xmlReader.ReadOuterXml();
                        StringReader outerReader = new StringReader(outerXml);
                        newPacket = (AISFrame)AISFrame.XmlSerializer.Deserialize(outerReader);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error reading logfile: " + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                        return null;
                    }
                    return newPacket;
                }
            }
        }
        protected override void Init()
        {
            base.Init();
            xmlReader = System.Xml.XmlReader.Create(_fileStream, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment });
        }
        protected override void DeInit()
        {
            base.DeInit();
            xmlReader.Close();
        }
        public override void Rewind()
        {
            lock (packetLocker)
            {
                MessageHandler.Reset();
                _fileStream.Seek(0, SeekOrigin.Begin);
                xmlReader = System.Xml.XmlReader.Create(_fileStream, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment });

                nextPacket = getNextPacket();
                if (nextPacket == null)
                    return; // Empty log file..

                _logTime = nextPacket.TimeStamp;
                _lastTime = DateTime.Now;
            }
        }
    }
    public class CANStreamer_KeesLogfile : CANStreamer_Logfile
    {
        // Properties
        public static string FileFilter = "Kees Log File (*.*)|*.*";

        // Variables for reading the frames
        private StreamReader fileReader;
        
        // Constructors
        public CANStreamer_KeesLogfile()
            : base()
        {
        }

        // Gets next packet from file
        public override Frame getNextPacket()
        {
            string line;
            int errorCount = 0;
            while ((line = fileReader.ReadLine()) != null)
            {
                try
                {
                    char[] charSeparators = new char[] { ',' };
                    var elements = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    // DateTime Date = DateTime.ParseExact(elements[0], "yyyy-MM-dd'Z'HH':'mm':'ss'.'FFF", System.Globalization.CultureInfo.InvariantCulture);
                    if (elements[0][10] == 'Z') elements[0] = elements[0].Replace('Z', 'T');  // Get rid of local time designation so we write back out as came in
                    DateTime Date = DateTime.Parse(elements[0]);
                    //DateTime Date = DateTime.SpecifyKind(DateTime.Parse(elements[0]), DateTimeKind.Unspecified);
                    byte Priority = Convert.ToByte(elements[1]);
                    int PGN = Convert.ToInt32(elements[2]);
                    byte Source = Convert.ToByte(elements[3]);
                    byte Destination = Convert.ToByte(elements[4]);
                    int ByteCount = Convert.ToInt32(elements[5]);
                    N2kHeader hdr = new N2kHeader(Priority, PGN, Destination, Source);

                    if ((hdr.PGN != PGN) || (hdr.PGNSource != Source) || (hdr.PGNDestination != Destination) || (hdr.PGNPriority != Priority))
                    {
                        N2kHeader hdr2 = new N2kHeader(Priority, PGN, Destination, Source);
                    }

                    Byte[] Bytes = new Byte[ByteCount];
                    for (int i = 0; i < ByteCount; i++)
                    {
                        Bytes[i] = byte.Parse(elements[6 + i], System.Globalization.NumberStyles.HexNumber);
                    }

                    // Return it
                    var N2kMsg = new N2kFrame(hdr, Bytes, new DateTime());// { ms = 0, Data = Bytes, Header = hdr, TimeStamp = Date };
                    return N2kMsg;
                }
                catch
                {
                    // Exception... Move on, unless we have had too many
                    errorCount++;
                    if (errorCount > 10) {
                        MessageBox.Show("A large number of errors occured while reading log file " + Name + ". Reading has been stopped.");
                        return null;
                    }
                }
            }

            // EOF
            return null;
        }

        protected override void Init()
        {
            base.Init();
            fileReader = new StreamReader(_fileStream);
        }
        protected override void DeInit()
        {
            base.DeInit();
            fileReader.Close();
        }
        public override void Rewind()
        {
            lock (packetLocker)
            {
                MessageHandler.Reset();
                _fileStream.Seek(0, SeekOrigin.Begin);
                fileReader = new StreamReader(_fileStream);

                nextPacket = getNextPacket();
                if (nextPacket == null)
                    return; // Empty log file..

                _logTime = nextPacket.TimeStamp;
                _lastTime = DateTime.Now;
            }
        }
    }
    public class CANStreamer_WeatherLogfile : CANStreamer_Logfile
    {
        // Properties
        public static string FileFilter = "Weather Log File (*.*)|*.*";
        
        // Public
        public override string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;

                string fileName = Path.GetFileName(_fileName);
                if (fileName.Length >= 11)
                    DateTime.TryParseExact(fileName.Substring(0, 11), "MMM_dd_yyyy", null, System.Globalization.DateTimeStyles.None, out _baseDate);
                else
                    _baseDate = new DateTime();

                NotifyPropertyChanged("FileName");
            }
        }

        // Variables for reading the frames
        private StreamReader _fileReader;
        private DateTime _baseDate;

        // Constructors
        public CANStreamer_WeatherLogfile()
            : base()
        {
        }

        // Gets next packet from file
        public override Frame getNextPacket()
        {
            int errorCount = 0;
            string line;
            while ((line = _fileReader.ReadLine()) != null)
            {
                try
                {
                    // Msg: "22:22:10.628: -> 130306 09FD023A FF030354F4FAFFFF"
                    // 0 : 22:22:10.628:
                    // 1 : ->
                    // 2 : 130306
                    // 3 : 09FD023A
                    // 4 : FF030354F4FAFFFF

                    string[] fields = line.Split(new char[] { ' ' });

                    DateTime msgTime = DateTime.ParseExact(fields[0], "HH:mm:ss.fff:", null);
                    DateTime finalTime = _baseDate.Add(msgTime.TimeOfDay);

                    byte b0 = Convert.ToByte(fields[3].Substring(0, 2), 16);
                    byte b1 = Convert.ToByte(fields[3].Substring(2, 2), 16);
                    byte b2 = Convert.ToByte(fields[3].Substring(4, 2), 16);
                    byte b3 = Convert.ToByte(fields[3].Substring(6, 2), 16);

                    byte[] data = new byte[fields[4].Length / 2];
                    for (int i = 0; i < data.Length; i++)
                        data[i] = Convert.ToByte(fields[4].Substring(2 * i, 2), 16);

                    N2kFrame N2kMsg = new N2kFrame(new N2kHeader(b0, b1, b2, b3), data, finalTime);
                    return N2kMsg;
                }
                catch
                {
                    // Exception.. move on if not too many errors
                    errorCount++;
                    if (errorCount > 10) {
                        MessageBox.Show("A large number of errors occured while reading log file " + Name + ". Reading has been stopped.");
                        return null;
                    }

                }
            }

            // EOF
            return null;
        }

        protected override void Init()
        {
            base.Init();
            _fileReader = new StreamReader(_fileStream);
        }
        protected override void DeInit()
        {
            base.DeInit();
            _fileReader.Close();
        }
        public override void Rewind()
        {
            lock (packetLocker)
            {
                MessageHandler.Reset();
                _fileStream.Seek(0, SeekOrigin.Begin);
                _fileReader = new StreamReader(_fileStream);

                nextPacket = getNextPacket();
                if (nextPacket == null)
                    return; // Empty log file..

                _logTime = nextPacket.TimeStamp;
                _lastTime = DateTime.Now;
            }
        }
    }
    public class CANStreamer_N0183Logfile : CANStreamer_Logfile
    {
        // Properties
        public static string FileFilter = "NMEA 0183 Log File (*.txt)|*.txt";

        // Variables for reading the frames
        private FrameReader _N0183Builder;
        private Queue<Frame> _frames;
        private bool setDate;
        private DateTime _lastDate;

        // Constructors
        public CANStreamer_N0183Logfile()
            : base()
        {
            _N0183Builder = new N0183Reader();
            _N0183Builder.FrameCreated += N0183Builder_FrameCreated;
            _frames = new Queue<Frame> { };
        }

        private void N0183Builder_FrameCreated(Frame createdFrame)
        {
            if (setDate)
            {
                _lastDate = createdFrame.TimeStamp = _lastDate.AddSeconds(0.1);
            }
            else
            {
                createdFrame.TimeStamp = DateTime.Now;
                _lastDate = createdFrame.TimeStamp;
                setDate = true;
            }

            _frames.Enqueue(createdFrame);
        }

        // Gets next packet from file
        public override Frame getNextPacket()
        {
            if (_frames.Count > 0)
            {
                return _frames.Dequeue();
            }
            else
            {
                byte[] buffer = new byte[256];
                int readBytes;

                while (_frames.Count == 0)
                {
                    readBytes = _fileStream.Read(buffer, 0, 256);
                    if (readBytes == 0)
                        return null;

                    for (int i = 0; i < readBytes; i++)
                        i += _N0183Builder.ProcessBytes(buffer, i, readBytes - i);
                }

                return _frames.Dequeue();
            }
        }
        public override void Rewind()
        {
            lock (packetLocker)
            {
                MessageHandler.Reset();
                _fileStream.Seek(0, SeekOrigin.Begin);

                nextPacket = getNextPacket();
                if (nextPacket == null)
                    return; // Empty log file..

                _logTime = nextPacket.TimeStamp;
                _lastTime = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Base class for all COM port-based streams. Derived classes handle decoding byte stream
    /// </summary>
    public abstract class CANStreamer_COMPort : CANStreamer
    {
        // Vars
        protected SerialPort serialPort;
        protected string _PortName;
        protected int _Speed = 4800;

        // Properties
        public string PortName
        {
            get
            {
                return _PortName;
            }
            set
            {
                if (value != _PortName) // Don't do anything if port is the same
                {
                    _PortName = value;
                    serialPort.PortName = _PortName;
                    NotifyPropertyChanged("PortName");
                }
            }
        }
        public int Speed
        {
            get
            {
                return _Speed;
            }
            set
            {
                _Speed = value;
                serialPort.BaudRate = value;
            }
        }

        // Constructor
        public CANStreamer_COMPort()
            : base()
        {
        }

        // Methods
        public override void ConnectStream()
        {
            lock (connectionLocker)
            {
                lock (serialPort)
                {
                    try
                    {
                        if (!serialPort.IsOpen)
                        {
                            serialPort.Open();
                            serialPort.DiscardInBuffer();
                        }
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show("An error occurred when opening the COM port '" + serialPort.PortName +"': " + e.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                // Done
                ConnectionState = ConnectionStateEnum.Connected;
            }
        }
        public override void Disconnect()
        {
            lock (connectionLocker)
            {
                lock (serialPort)
                {
                    try
                    {
                        if (serialPort.IsOpen)
                            serialPort.Close();
                    }
                    catch
                    {
                        // Nothing more we can do.
                    }
                }

                // Done
                ConnectionState = ConnectionStateEnum.Disconnected;
            }
        }
    }

    /// <summary>
    /// Handles reading from an Actisense USB device emulating a serial port
    /// </summary>
    public class CANStreamer_NGT1_2000 : CANStreamer_COMPort
    {
        // Public properties
        public override string Type
        {
            get { return "COM Port (N2000)"; }
        }

        // Private vars
        private ActisenseInterface _actisenseInterface;

        // Constructor
        public CANStreamer_NGT1_2000()
            : base()
        {
            _actisenseInterface = new ActisenseInterface();
            _actisenseInterface.FrameCreated += new Action<Frame>(_actisenseInterface_FrameCreated);

            // Change from the default 'CANStreamer_COMPort' 4,800 baud to 115.2k baud
            _Speed = 115200;

            // Initialize serial port
            serialPort = new SerialPort();
            serialPort.BaudRate = _Speed;
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.Handshake = System.IO.Ports.Handshake.None;
            serialPort.Encoding = System.Text.Encoding.UTF8;   // We don't actually use this
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
        }

        void _actisenseInterface_FrameCreated(Frame obj)
        {
            OnRawReceived(obj);
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (serialPort)
            {
                try
                {
                    int bytesRead = serialPort.BytesToRead;
                    byte[] buffer = new byte[bytesRead];
                    serialPort.Read(buffer, 0, bytesRead);

                    for (int i = 0; i < bytesRead; i++)
                        i += _actisenseInterface.ProcessBytes(buffer, i, bytesRead - i);
                }
                catch (Exception ex)
                {
                    // Failure to read from serial port
                    // => Disconnect stream
                    MessageBox.Show("Error reading from COM port '" + PortName + "': " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Disconnect();
                }
            }
        }
    }
    public class CANStreamer_N0183 : CANStreamer_COMPort
    {
        // Public properties
        public override string Type
        {
            get { return "COM Port (N0183)"; }
        }
        public override bool CanWrite { get { return true; } }

        // Private vars
        private FrameReader _N0183Builder;
        private FrameSender _N0183Sender;

        // Constructor
        public CANStreamer_N0183()
            : base()
        {
            _N0183Sender = new N0183Sender();
            _N0183Builder = new N0183Reader();
            _N0183Builder.FrameCreated += new Action<Frame>(_N0183Builder_FrameCreated);

            // Initialize serial port
            serialPort = new SerialPort();
            serialPort.BaudRate = _Speed;
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.Handshake = System.IO.Ports.Handshake.None;
            serialPort.Encoding = System.Text.Encoding.UTF8;   // We don't actually use this
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived); 
        }

        void _N0183Builder_FrameCreated(Frame msg)
        {
            msg.TimeStamp = DateTime.Now;
            OnRawReceived(msg);
        }

        public override void SendFrame(Frame msg)
        {
            lock (connectionLocker)
            {
                if (ConnectionState == ConnectionStateEnum.Disconnected)
                    return;

                lock (serialPort)
                {
                    byte[] msgBytes = _N0183Sender.GetBytes(msg);
                    serialPort.Write(msgBytes, 0, msgBytes.Length);
                }
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (serialPort)
            {
                try
                {
                    int bytesRead = serialPort.BytesToRead;
                    byte[] buffer = new byte[bytesRead];

                    serialPort.Read(buffer, 0, bytesRead);

                    for (int i = 0; i < bytesRead; i++)
                        i += _N0183Builder.ProcessBytes(buffer, i, bytesRead - i);
                }
                catch (Exception ex)
                {
                    // Failure to read from serial port
                    // => Disconnect stream
                    MessageBox.Show("Error reading from COM port '" + PortName + "': " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Disconnect();
                }
            }
        }
    }

    /// <summary>
    /// Wrapper classes around TCP/UDP clients
    /// </summary>
    public class UDP_Client
    {
        public string Hostname
        {
            get
            {
                return _hostname;
            }
            set
            {
                _hostname = value;
            }
        }
        public int Port { get; set; }
        public OutgoingProtocolEnum OutgoingProtocol
        {
            get
            {
                return _sendingProtocol;
            }
            set
            {
                _sendingProtocol = value;
                _frameSender = FrameSender.Create(value);
            }
        }

        private string _hostname;
        private UdpClient _udpClient = new UdpClient();
        private OutgoingProtocolEnum _sendingProtocol;
        private FrameSender _frameSender;

        private UDP_Client()
        {
            // Xml serializer requires parameterless constructor
        }
        public UDP_Client(string hostname, int remotePort)
        {
            Hostname = hostname;
            Port = remotePort;
            OutgoingProtocol = OutgoingProtocolEnum.NMEA0183;
        }

        public void SendFrame(Frame frame)
        {
            byte[] datagram = _frameSender.GetBytes(frame);
            if (datagram != null)
                _udpClient.Send(datagram, datagram.Length, _hostname, Port);
        }
    }
    public class TCP_Client : INotifyPropertyChanged
    {
        // Consts
        private const int bufferSize = 1024;

        public class ConnectionStateChangedEventArgs : EventArgs
        {
            public ConnectionStateEnum OldState { get { return _oldState; } }
            public ConnectionStateEnum NewState { get { return _newState; } }

            private ConnectionStateEnum _oldState;
            private ConnectionStateEnum _newState;

            public ConnectionStateChangedEventArgs(ConnectionStateEnum oldState, ConnectionStateEnum newState)
            {
                _oldState = oldState;
                _newState = newState;
            }
        }
        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        private void OnConnectionStateChanged(ConnectionStateChangedEventArgs a)
        {
            var e = ConnectionStateChanged;
            if (e != null)
                e(this, a);
        }

        // Public properties
        [XmlIgnore]
        public ConnectionStateEnum ConnectionState
        {
            get { return _connectionState; }
            set
            {
                OnConnectionStateChanged(new ConnectionStateChangedEventArgs(_connectionState, value));
                _connectionState = value;
            }
        }
        [XmlAttribute]
        public string Hostname
        {
            get
            {
                return _hostname;
            }
            set
            {
                _hostname = value;
                OnPropertyChanged("Hostname");
            }
        }
        [XmlAttribute]
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }
        [XmlIgnore]
        public string Status
        {
            get
            {
                return _status;
            }
            private set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public IncomingProtocolEnum IncomingProtocol
        {
            get
            {
                return _incomingProtocol;
            }
            set
            {
                _incomingProtocol = value;
                _frameReader = FrameReader.Create(value);
                _frameReader.FrameCreated += new Action<Frame>(_frameReader_FrameCreated);
                OnPropertyChanged("IncomingProtocol");
            }
        }
        public OutgoingProtocolEnum OutgoingProtocol
        {
            get
            {
                return _outgoingProtocol;
            }
            set
            {
                _outgoingProtocol = value;
                _frameSender = FrameSender.Create(value);
                OnPropertyChanged("OutgoingProtocol");
            }
        }

        public event Action<Frame> RawCreated;
        public event PropertyChangedEventHandler PropertyChanged;

        // Private vars
        private volatile ConnectionStateEnum _connectionState;
        private IncomingProtocolEnum _incomingProtocol;
        private OutgoingProtocolEnum _outgoingProtocol;
        private string _status;
        private string _hostname;
        private int _port;
        private TcpClient _tcpClient;
        private FrameReader _frameReader;
        private FrameSender _frameSender;
        private volatile bool _isConnecting = false;
        private readonly object connectionLocker = new object(); // Lock for anything involving connecting/disconnecting

        // Constructors
        private TCP_Client()
        {
            // Private ctor for XmlSerializer (req's parameterless constructor)

            _tcpClient = new TcpClient();
            Status = "Disconnected";
        }
        public TCP_Client(TcpClient connectedClient)
        {
            _tcpClient = connectedClient;
            _hostname = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address.ToString();
            _port = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Port;
            Status = "Connected";
            ConnectionState = ConnectionStateEnum.Connected;
            IncomingProtocol = IncomingProtocolEnum.NMEA0183;
            OutgoingProtocol = OutgoingProtocolEnum.NMEA0183;
        }
        public TCP_Client(string hostname, int port)
        {
            _tcpClient = new TcpClient();
            _hostname = hostname;
            _port = port;
            Status = "Disconnected";
            IncomingProtocol = IncomingProtocolEnum.NMEA0183;
            OutgoingProtocol = OutgoingProtocolEnum.NMEA0183;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                try
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                catch
                {

                }
            }
                
        }

        // Public methods
        public void SendFrame(Frame msg)
        {
            if (!_tcpClient.Connected || (ConnectionState == ConnectionStateEnum.Disconnected))
                return;

            byte[] data = _frameSender.GetBytes(msg);
            if (data == null)
                return;

            try
            {
                NetworkStream tcpStream = _tcpClient.GetStream();

                // We use the asynchronous BeginWrite to write to the network stream
                // It /appears/ that beginwrite calls result in data being sent in the same order as the calls
                tcpStream.BeginWrite(data, 0, data.Length, writeCallback, tcpStream);
            }
            catch (Exception e)
            {
                lock (connectionLocker)
                {
                    ConnectionState = ConnectionStateEnum.Disconnected;
                    Status = "Error: " + e.Message;
                    return;
                }
            }
        }
        
        // Methods
        public void Connect()
        {
            lock (connectionLocker)
            {
                if (ConnectionState == ConnectionStateEnum.Connected || _isConnecting)
                    return;

                // Tcp client may not be connected.
                if (!_tcpClient.Connected)
                {
                    try
                    {
                        Status = "Connecting";
                        _isConnecting = true;

                        _tcpClient = new TcpClient();
                        _tcpClient.BeginConnect(_hostname, _port, connectCallback, _tcpClient);
                    }
                    catch (Exception e)
                    {
                        Status = "Failed to initiate connection: " + e.Message;
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }
        public void Disconnect()
        {
            lock (connectionLocker)
            {
                try {
                    // Note: These lines regularly cause exceptions which we just ignore
                    _tcpClient.Client.Shutdown(SocketShutdown.Both);
                    _tcpClient.Client.Close(1000);
                }
                catch (Exception) {
                    // AJM We just ignore errors when attempting to close
                }

                // TODO : Disconnect should block until we have actually disconnected.
                // So we need to set some flag & wait.
                // Again, problems with quick disconnect-reconnect resulting in it not ending. If we do it the easy way of not waiting
                ConnectionState = ConnectionStateEnum.Disconnected;
                Status = "Disconnected";
                _isConnecting = false;
            }
        }

        // TCP
        private byte[] buffer = new byte[bufferSize];

        // Async callbacks
        private void connectCallback(IAsyncResult ar)
        {
            lock (connectionLocker)
            {
                try
                {
                    TcpClient tcpClient = (TcpClient)ar.AsyncState;
                    tcpClient.EndConnect(ar);   // This regularly causes an exception which we catch
                }
                catch (Exception e)
                {
                    _isConnecting = false;
                    ConnectionState = ConnectionStateEnum.Disconnected;
                    Status = "Failed to connect: " + e.Message;
                    return;
                }

                // Connected now
                ConnectionState = ConnectionStateEnum.Connected;
                Status = "Connected";
                _isConnecting = false;

                // Set up reading
                try
                {
                    NetworkStream tcpStream = _tcpClient.GetStream();
                    tcpStream.BeginRead(buffer, 0, bufferSize, readCallback, tcpStream);
                }
                catch (Exception e)
                {
                    ConnectionState = ConnectionStateEnum.Disconnected;
                    Status = "Failed to connect: " + e.Message;
                    return;
                }
            }
        }
        private void readCallback(IAsyncResult ar)
        {
            try
            {
                NetworkStream tcpStream = (NetworkStream)ar.AsyncState;
                int readBytes = tcpStream.EndRead(ar);

                _frameReader.ProcessBytes(buffer, 0, readBytes);
                
                tcpStream.BeginRead(buffer, 0, bufferSize, readCallback, tcpStream);
            }
            catch (Exception e)
            {
                lock (connectionLocker)
                {
                    ConnectionState = ConnectionStateEnum.Disconnected;
                    Status = "Read error: " + e.Message;
                    return;
                }
            }
        }
        private void writeCallback(IAsyncResult ar)
        {
            NetworkStream tcpStream = (NetworkStream)ar.AsyncState;
            try
            {
                tcpStream.EndWrite(ar);
            }
            catch (Exception e)
            {
                lock (connectionLocker)
                {
                    ConnectionState = ConnectionStateEnum.Disconnected;
                    Status = "Write error: " + e.Message;
                    return;
                }
            }
        }

        private void _frameReader_FrameCreated(Frame msg)
        {
            var e = RawCreated;
            if (e != null)
                e(msg);
        }
    }

    /// <summary>
    /// Main network stream object, handles TCP and UDP connections, as well as UDP multicasting / listening.
    /// </summary>
    public class CANStreamer_Network : CANStreamer
    {
        // Public properties
        public override string Type
        {
            get { return "Internet"; }
        }
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public BindingList<TCP_Client> OwnTCPClients { get { return _ownTcpClients; } }
        [XmlIgnore]
        public BindingList<TCP_Client> OtherTCPClients { get { return _otherTcpClients; } }
        public BindingList<UDP_Client> OwnUDPClients { get { return _ownUdpClients; } }

        // This stores the port to listen on for TCP. If we do want any listening to occur, we store a negative (or zero) value.
        public int TCPListenPort
        {
            get
            {
                return _tcpListenPort;
            }
            set
            {
                if (value == _tcpListenPort)
                    return;

                _tcpListenPort = value;

                // Shut down any current listener (because it is being turned off, or its port has changed)
                if (_tcpListener != null) {
                    _tcpListener.Server.Close();
                    _tcpListener.Stop();
                }
                _tcpListener = null;
                // If the user wants a TCP listener running, then set one up and start it up if this stream is connected
                if (_tcpListenPort > 0) {
                    // Set up a new listener
                    _tcpListener = new TcpListener(IPAddress.Any, _tcpListenPort);
                    if (ConnectionState == ConnectionStateEnum.Connected) {
                        _tcpListener.Start();
                        _tcpListener.BeginAcceptTcpClient(acceptCallback, _tcpListener);
                    }
                }
            }
        }

        // This stores the port to listen on for UDP. If we do want any listening to occur, we store a negative (or zero) value.
        public int UDPReceivePort
        {
            get
            {
                return _udpReceivePort;
            }
            set
            {
                if (value == _udpReceivePort)
                    return;

                _udpReceivePort = value;

                // Either port has changed (but user still wants a UdpClient), or the user wants to shut down the UdpClient.
                // Either way, we shut down the current UdpClient.
                if (_udpReceiver != null) {
                    _udpReceiver.Close();
                }
                _udpReceiver = null;

                // Create, and if the stream is connected, also start, a new UDP Client.
                if (_udpReceivePort > 0)
                {
                    _udpReceiver = new UdpClient(value);

                    if (ConnectionState == ConnectionStateEnum.Connected)
                        _udpReceiver.BeginReceive(receiveCallback, _udpReceiver);
                }
            }
        }

        public bool ListeningToBroadcast
        {
            get
            {
                return _isListeningToBroadcast;
            }
            set
            {
                _isListeningToBroadcast = value;
                NotifyPropertyChanged("ListeningToBroadcast");
            }
        } // Set is only public for XmlSerializer
        public string BroadcastIP { get { return _broadcastIP; } set { _broadcastIP = value; } } // Set is only public for XmlSerializer
        public int BroadcastPort { get { return _broadcastPort; } set { _broadcastPort = value; } } // Set is only public for XmlSerializer

        public IncomingProtocolEnum IncomingProtocol
        {
            get
            {
                return _incomingProtocol;
            }
            set
            {
                _incomingProtocol = value;

                lock (_otherTcpClients)
                {
                    foreach (TCP_Client stream in _otherTcpClients)
                        stream.IncomingProtocol = value;
                }

                foreach (TCP_Client stream in _ownTcpClients)
                    stream.IncomingProtocol = value;

                _udpReader = FrameReader.Create(value);
                _udpReader.FrameCreated += new Action<Frame>(_udpReader_FrameCreated);

                NotifyPropertyChanged("IncomingProtocol");
            }
        }

        void _udpReader_FrameCreated(Frame obj)
        {
            OnRawReceived(obj);
        }
        public OutgoingProtocolEnum OutgoingProtocol
        {
            get
            {
                return _outgoingProtocol;
            }
            set
            {
                _outgoingProtocol = value;

                lock (_otherTcpClients)
                {
                    foreach (TCP_Client client in _otherTcpClients)
                        client.OutgoingProtocol = value;
                }

                foreach (TCP_Client client in _ownTcpClients)
                    client.OutgoingProtocol = value;

                NotifyPropertyChanged("OutgoingProtocol");
            }
        }

        // Default ports and broadcast addresses.
        public const int DefaultPort = 5037;
        public const string DefaultBroadcastIP = "233.252.123.112"; // Part of AD-HOC Block III; see http://www.iana.org/assignments/multicast-addresses/

        // Private vars
        private readonly object _otherTcpLocker = new object(); // 'OtherClients' list can be modified from non-UI thread, so we require a locker

        private int _tcpListenPort = -DefaultPort; // meaning inactive; a set must be used to make it active
        private TcpListener _tcpListener;

        private int _udpReceivePort = -DefaultPort; // meaning inactive; a set must be used to make it active
        private UdpClient _udpReceiver;

        private bool _isListeningToBroadcast;
        private string _broadcastIP = DefaultBroadcastIP;
        private int _broadcastPort = DefaultPort;
        private UdpClient _broadcastReceiver;

        private IncomingProtocolEnum _incomingProtocol;
        private OutgoingProtocolEnum _outgoingProtocol;
        private FrameReader _udpReader;

        private BindingList<UDP_Client> _ownUdpClients = new BindingList<UDP_Client> { };
        private BindingList<TCP_Client> _ownTcpClients = new BindingList<TCP_Client> { };
        private ThreadedBindingList<TCP_Client> _otherTcpClients = new ThreadedBindingList<TCP_Client> { };
        
        // Constructors
        public CANStreamer_Network()
            : base()
        {
            _ownTcpClients.ListChanged += new ListChangedEventHandler(_ownClients_ListChanged);

            IncomingProtocol = IncomingProtocolEnum.NMEA0183;
            OutgoingProtocol = OutgoingProtocolEnum.NMEA0183;
        }

        // Public methods
        public bool StartListeningToBroadcast(IPAddress ipAddress, int port)
        {
            if (_broadcastReceiver != null)
            {
                // Need to stop old receiver. Close socket => Raises exception on it's BeginRead
                _broadcastReceiver.Client.Close();
            }

            _broadcastIP = ipAddress.ToString();
            _broadcastPort = port;
            ListeningToBroadcast = true;

            try
            {
                _broadcastReceiver = new UdpClient(port);
                _broadcastReceiver.JoinMulticastGroup(ipAddress);
                _broadcastReceiver.BeginReceive(receiveCallback, _broadcastReceiver);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Error attempting to listen to broadcast: " + ex.Message);
                return false;
            }

            return true;
        }
        public void StopListeningToBroadcast()
        {
            if (_broadcastReceiver != null)
            {
                // Need to stop old receiver. Close socket => Raises exception on it's BeginRead
                _broadcastReceiver.Client.Close();
            }

            ListeningToBroadcast = false;
        }

        public void ConnectViaUDP(IPAddress ipAddress, int port)
        {
            UDP_Client client = new UDP_Client(ipAddress.ToString(), port);
            _ownUdpClients.Add(client);
        }
        public void ConnectTo(string hostname, int port)
        {
            TCP_Client stream = new TCP_Client(hostname, port);
            
            if (ConnectionState == ConnectionStateEnum.Connected)
                stream.Connect();

            _ownTcpClients.Add(stream);
        }

        public void RemoveOwnClient(UDP_Client udpClient)
        {
            // UDP has nothing to disconnect.
            _ownUdpClients.Remove(udpClient);
        }
        public void RemoveOwnClient(TCP_Client tcpClient)
        {
            tcpClient.Disconnect();
            _ownTcpClients.Remove(tcpClient);
        }
        public void RemoveOtherClient(TCP_Client client)
        {
            lock (_otherTcpClients)
            {
                client.Disconnect();
                _otherTcpClients.Remove(client);
            }
        }

        public override void SendFrame(Frame msg)
        {
            TCP_Client[] otherTCP = OtherTCP_SafeCopy();
            foreach (TCP_Client client in otherTCP)
                client.SendFrame(msg);

            foreach (UDP_Client client in _ownUdpClients)
                client.SendFrame(msg);
        }

        public override void ConnectStream()
        {
            if (ConnectionState == ConnectionStateEnum.Connected)
                return;
            ConnectionState = ConnectionStateEnum.Connected;

            // Start listenenr 
            if (_tcpListener != null)
            {
                _tcpListener.Start();
                _tcpListener.BeginAcceptTcpClient(acceptCallback, _tcpListener);
            }

            if (_udpReceiver != null)
            {
                _udpReceiver.BeginReceive(receiveCallback, _udpReceiver);
            }

            if (ListeningToBroadcast)
            {
                StartListeningToBroadcast(IPAddress.Parse(BroadcastIP), BroadcastPort);
            }

            // Start all clients.
            foreach (TCP_Client stream in _ownTcpClients)
                stream.Connect();

         //   TCP_Client[] otherTCP = OtherTCP_SafeCopy();
         //   foreach (TCP_Client stream in otherTCP)
          //      stream.Connect();
        }
        public override void Disconnect()
        {
            if (ConnectionState == ConnectionStateEnum.Disconnected)
                return;

            if (_tcpListener != null)
                _tcpListener.Stop();

            if (_udpReceiver != null)
                _udpReceiver.Client.Close();

            if (_broadcastReceiver != null)
                _broadcastReceiver.Client.Close();

            // Stop all clients as well.
            TCP_Client[] ownCopy = new TCP_Client[_ownTcpClients.Count];
            _ownTcpClients.CopyTo(ownCopy, 0);
            foreach (TCP_Client stream in ownCopy)
                stream.Disconnect();

            // On disconnect, remote connections will be dropped?
            TCP_Client[] otherTCP = OtherTCP_SafeCopy();
            foreach (TCP_Client stream in otherTCP)
                stream.Disconnect();
            
            ConnectionState = ConnectionStateEnum.Disconnected;
        }

        // Private methods
        void _ownClients_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:

                    TCP_Client addedTCPClient = _ownTcpClients[e.NewIndex];

                    addedTCPClient.IncomingProtocol = IncomingProtocol;
                    addedTCPClient.RawCreated += addedTCPClient_RawCreated;
                    break;
            }
        }

        void addedTCPClient_RawCreated(Frame msg)
        {
            OnRawReceived(msg);
        }

        void OtherTCP_Client_ConnectionStateChanged(object sender, TCP_Client.ConnectionStateChangedEventArgs e)
        {
            TCP_Client client = (TCP_Client)sender;
            if (e.NewState == ConnectionStateEnum.Disconnected)
            {
                lock (_otherTcpClients)
                {
                    _otherTcpClients.Remove(client);
                }
            }
        }

        private TCP_Client[] OtherTCP_SafeCopy()
        {
            TCP_Client[] otherTCP;
            lock (_otherTcpClients)
            {
                otherTCP = new TCP_Client[_otherTcpClients.Count];
                _otherTcpClients.CopyTo(otherTCP, 0);
            }
            return otherTCP;
        }

        // TCP
        private void acceptCallback(IAsyncResult ar)
        {
            TcpListener tcpListener;
            TcpClient tcpClient;
            
            try
            {
                tcpListener = (TcpListener)ar.AsyncState;
                tcpClient = tcpListener.EndAcceptTcpClient(ar);
            }
            catch (ObjectDisposedException)
            {
                // ObjectDisposedException happens when the listener is stopped.
                // Note: There is no nicer way to do this
                return;
            }

            TCP_Client stream = new TCP_Client(tcpClient);
            stream.OutgoingProtocol = OutgoingProtocol;
            stream.IncomingProtocol = IncomingProtocol;
            stream.Connect();
            stream.ConnectionStateChanged += OtherTCP_Client_ConnectionStateChanged;

            lock (_otherTcpClients)
            {
                _otherTcpClients.Add(stream);
            }

            tcpListener.BeginAcceptTcpClient(acceptCallback, tcpListener);
        }

        // UDP
        private void receiveCallback(IAsyncResult ar)
        {
            // End async call
            UdpClient udpReceiver = (UdpClient)ar.AsyncState;
            IPEndPoint remoteEndpoint = new IPEndPoint(0, 0);

            try
            {
                byte[] datagram = udpReceiver.EndReceive(ar, ref remoteEndpoint);

                // Process datagram and restart receiving.
                _udpReader.ProcessBytes(datagram, 0, datagram.Length);
                udpReceiver.BeginReceive(receiveCallback, udpReceiver);
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }

    /// <summary>
    /// Write-only stream for logging to a file
    /// </summary>
    public class CANStreamer_Logger : CANStreamer
    {
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                ResolvedFileName();
                NotifyPropertyChanged("FileName");
            }
        }
        public bool Overwrite
        {
            get
            {
                return _overwrite;
            }
            set
            {
                _overwrite = value;
                NotifyPropertyChanged("Overwrite");
            }
        }
        public bool ShowOnMenu
        {
            get
            {
                return _showonmenu;
            }
            set
            {
                _showonmenu = value;
                NotifyPropertyChanged("ShowOnMenu");
            }
        }
        public override bool CanRead { get { return false; } }
        public override bool CanWrite { get { return true; } }
        public override string Type { get { return "Logger"; } }

        private string _fileName;
        private bool _overwrite;
        private bool _showonmenu;
        private FileStream _fileStream;
        private StreamWriter _fileWriter;
        private XmlWriterSettings _xmlSettings;
        private XmlSerializerNamespaces _blankNamespace;

        public CANStreamer_Logger()
        {
            _xmlSettings = new XmlWriterSettings() { OmitXmlDeclaration = true };
            _blankNamespace = new XmlSerializerNamespaces();
            _blankNamespace.Add("", "");
        }

        public override void SendFrame(Frame msg)
        {
            lock (connectionLocker)
            {
                if (ConnectionState != ConnectionStateEnum.Connected)
                    return;

                StringWriter xmlStringWriter = new StringWriter();
                using (XmlWriter xmlWriter = XmlWriter.Create(xmlStringWriter, _xmlSettings))
                {
                    if (msg is N2kFrame)
                        N2kFrame.XmlSerializer.Serialize(xmlWriter, msg, _blankNamespace);
                    else if (msg is N0183Frame)
                        N0183Frame.XmlSerializer.Serialize(xmlWriter, msg, _blankNamespace); // NotifyingSerializer uses blank namespace by default
                    else if (msg is AISFrame)
                        AISFrame.XmlSerializer.Serialize(xmlWriter, msg, _blankNamespace);
                    else
                        throw new Exception("Cannot serialize frame of type: '" + msg.GetType().Name + "'");
                }

                try
                {
                    _fileWriter.WriteLine(xmlStringWriter.ToString());
                    _fileWriter.Flush();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error writing to logfile '" + _fileName + "': " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Disconnect();
                }
            }
        }

        private string ResolvedFileName()
        {
            string[] Parts = _fileName.Split(new string[] { "##" }, StringSplitOptions.None);
            string ResolvedFileName = "";
            bool AddSeparator = false; // For first we do not add any separator
            for (int i = 0; i < Parts.Length; i++)
            {
                if (Parts[i] == "date")
                {
                    ResolvedFileName += DateTime.Now.ToString("yyyyMMdd");
                    AddSeparator = false;
                }
                else if (Parts[i] == "time")
                {
                    ResolvedFileName += DateTime.Now.ToString("HHmmss");
                    AddSeparator = false;
                }
                else
                {
                    if (AddSeparator) ResolvedFileName += "##";
                    AddSeparator = true;
                    ResolvedFileName += Parts[i];
                }
            }
            return ResolvedFileName;
        }

        public override void ConnectStream()
        {
            lock (connectionLocker)
            {
                if (ConnectionState == ConnectionStateEnum.Connected)
                    return;

                try
                {
                    if (_overwrite)
                        _fileStream = new FileStream(ResolvedFileName(), FileMode.Create, FileAccess.Write);
                    else
                        _fileStream = new FileStream(ResolvedFileName(), FileMode.Append, FileAccess.Write);
                }
                catch (Exception ex)
                {
                    ConnectionState = ConnectionStateEnum.Disconnected; // This updates LastState, which ensures that we will not attempt to connect this stream when we next start up the application
                    MessageBox.Show("Error opening output logfile '" + _fileName + "': " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                _fileWriter = new StreamWriter(_fileStream);
                ConnectionState = ConnectionStateEnum.Connected;
            }
        }
        public override void Disconnect()
        {
            lock (connectionLocker)
            {
                if (ConnectionState == ConnectionStateEnum.Disconnected)
                    return;

                _fileWriter.Close();
                _fileStream.Close();

                ConnectionState = ConnectionStateEnum.Disconnected;
            }
        }
    }

    /// <summary>
    /// Generates NMEA 0183 messages from parameters
    /// </summary>
    public class CANStreamer_Generator : CANStreamer
    {
        public override string Type
        {
            get { return "Generator"; }
        }
        public int Interval
        {
            get
            {
                return _Interval;
            }
            set
            {
                _Interval = value;
            }
        }
        public string MessagePrefix
        {
            get { return _MessagePrefix; }
            set { _MessagePrefix = value; }
        }
        public string MessageBody
        {
            get { return _MessageBody; }
            set {
                _MessageBody = value;
                _SplitBody = _MessageBody.Split(',');
            }
        }

        private readonly DateTime gpsEpochStart = new DateTime(1980, 1, 6);
        private string _MessagePrefix;
        private string _MessageBody;
        private string[] _SplitBody = new string[0];
        private int _Interval = 1000;
        private System.Threading.Timer _Timer;

        public CANStreamer_Generator()
        {
            _Timer = new System.Threading.Timer(_Timer_Tick);
        }

        private void _Timer_Tick(object state)
        {
            StringBuilder message = new StringBuilder(_MessagePrefix);

            lock (Definitions.ParamCol.UpdateLocker)
            {
                for (int i = 0; i < _SplitBody.Length; i++)
                {
                    string bodyPart = _SplitBody[i].Trim();
                    if (bodyPart == "[Date]")
                    {
                        message.Append("," + (int)((DateTime.Now - gpsEpochStart).TotalDays / 7));
                        continue;
                    }
                    else if (bodyPart == "[Time]")
                    {
                        int dayOfWeek = (int)((DateTime.Now - gpsEpochStart).TotalDays % 7);
                        message.Append("," + (dayOfWeek * 24 * 60 * 60 + (int)DateTime.Now.TimeOfDay.TotalSeconds));
                        continue;
                    }
                    else if (bodyPart != "")
                    {
                        bool found = false;
                        foreach (Parameter p in Definitions.ParamCol.ClonedParameters)
                        {
                            NumericParameter np = p as NumericParameter;
                            if ((np != null) && (np.InternalName == bodyPart))
                            {
                                Parameter.ParameterStateEnum pstate;
                                DateTime dt;
                                double v;

                                v = np.GetValueAndState(out pstate, out dt);

                                found = true;
                                message.Append("," + np.Formatter.FormatToN0183(v, pstate));
                                break;
                            }
                        }

                        if (!found)
                        {
                            message.Append(",");
                            ReportHandler.LogWarning("[Generator] Parameter '" + bodyPart + "' not found");
                        }
                    }
                    else
                    {
                        message.Append(",");
                    }
                }
            }

            // Add checksum
            byte checksum = 0;
            for (int i = 1; i < message.Length; i++)
                checksum ^= Convert.ToByte(message[i]);
            message.Append("*" + checksum.ToString("X2"));

            N0183Frame generatedFrame = new N0183Frame(message.ToString(), DateTime.Now);
            OnRawReceived(generatedFrame);
        }
        
        public override void ConnectStream()
        {
            if (ConnectionState == ConnectionStateEnum.Connected)
                return;

            _Timer.Change(_Interval, _Interval);
            ConnectionState = ConnectionStateEnum.Connected;
        }
        public override void Disconnect()
        {
            if (ConnectionState == ConnectionStateEnum.Disconnected)
                return;

            _Timer.Change(Timeout.Infinite, Timeout.Infinite);
            ConnectionState = ConnectionStateEnum.Disconnected;
        }
    }

    /// <summary>
    /// The StreamManager static class manages all the streams in the program, including but not limited to serialization and managing connectors.
    /// </summary>
    public static class StreamManager
    {
        // Constants
        public static readonly string StreamsFileName = Application.StartupPath + "\\Streams.xml";
        public static readonly string ConnectorsFileName = Application.StartupPath + "\\Connectors.xml";

        // Public
        public static BindingList<CANStreamer> Streams { get { return _streams; } }
        public static BindingList<Connector> Connectors { get { return _connectors; } }

        public static event Action<CANStreamer> NewStream;

        // Private
        private static Dictionary<int, bool> _ids;
        private static BindingList<CANStreamer> _streams;
        private static BindingList<Connector> _connectors;
        private static Dictionary<KeyValuePair<CANStreamer, CANStreamer>, Connector> _connectorMap;
        private static Random _random;
        private static XmlFileSerializer<BindingList<CANStreamer>> _streamsSerializer = new XmlFileSerializer<BindingList<CANStreamer>>();
        private static XmlFileSerializer<BindingList<Connector>> _connectorsSerializer = new XmlFileSerializer<BindingList<Connector>>();

        // Constructor
        static StreamManager()
        {
            _ids = new Dictionary<int, bool> { };
            _streams = new BindingList<CANStreamer> { };
            _connectors = new BindingList<Connector> { };
            _connectorMap = new Dictionary<KeyValuePair<CANStreamer, CANStreamer>, Connector> { };
            _random = new Random();
        }
        
        // Public methods
        public static void AddStream(CANStreamer stream)
        {
            int id;
            do
            {
                id = _random.Next(1000, int.MaxValue); // Range lets us know if an ID has not been set
            } while (_ids.ContainsKey(id));

            stream.ID = id;
            _ids.Add(id, true);
            _streams.Add(stream);

            // Fire event
            var e = NewStream;
            if (e != null)
                e(stream);
        }
        public static void RemoveStream(CANStreamer stream)
        {
            _ids.Remove(stream.ID);
            _streams.Remove(stream);

            List<Connector> toRemove = new List<Connector> { };
            foreach (Connector c in _connectors)
                if (c.fromID == stream.ID || c.toID == stream.ID)
                    toRemove.Add(c);

            foreach (Connector c in toRemove)
                RemoveConnector(c._fromStream, c._toStream);
        }

        public static void AddConnector(CANStreamer fromStream, CANStreamer toStream)
        {
            var kvp = new KeyValuePair<CANStreamer, CANStreamer>(fromStream, toStream);
            if (!_connectorMap.ContainsKey(kvp))
            {
                Connector newConnector = new Connector(fromStream, toStream);
                _connectors.Add(newConnector);
                _connectorMap.Add(kvp, newConnector);
            }
        }
        public static void RemoveConnector(CANStreamer fromStream, CANStreamer toStream)
        {
            var kvp = new KeyValuePair<CANStreamer, CANStreamer>(fromStream, toStream);
            Connector connector;
            if (_connectorMap.TryGetValue(kvp, out connector))
            {
                connector.Disconnect();
                _connectorMap.Remove(kvp);
                _connectors.Remove(connector);
            }
        }
        public static bool ConnectionExists(CANStreamer fromStream, CANStreamer toStream)
        {
            return _connectorMap.ContainsKey(new KeyValuePair<CANStreamer,CANStreamer>(fromStream, toStream));
        }

        public static void DisconnectLogfiles()
        {
            foreach (CANStreamer stream in _streams)
                if (stream is CANStreamer_Logfile && stream.ConnectionState == ConnectionStateEnum.Connected)
                    stream.Disconnect();
        }

        public static void SaveToFiles()
        {
            _streamsSerializer.Serialize(StreamsFileName, _streams);
            _connectorsSerializer.Serialize(ConnectorsFileName, _connectors);
        }
        public static void LoadFromFiles()
        {
            // Deserialize streams/connectors to temp arrays, due to need for proper initialization/validation
            BindingList<CANStreamer> streamsTemp = _streamsSerializer.Deserialize(StreamsFileName) ?? new BindingList<CANStreamer> { };
            BindingList<Connector> connectorsTemp = _connectorsSerializer.Deserialize(ConnectorsFileName) ?? new BindingList<Connector> { };

            // Step 1 : Add streams (Must be done before connectors can be done)
            foreach (CANStreamer stream in streamsTemp)
            {
                _streams.Add(stream);
                
                var e = NewStream;
                if (e != null)
                    e(stream);
            }

            // Step 2 : Do connector work (Now that we have our streams/ids)
            foreach (Connector connector in connectorsTemp)
            {
                CANStreamer fromStream = null;
                CANStreamer toStream = null;

                foreach (CANStreamer stream in _streams)
                {
                    if (stream.ID == connector.fromID)
                        fromStream = stream;

                    if (stream.ID == connector.toID)
                        toStream = stream;
                }

                if (fromStream != null && toStream != null)
                    AddConnector(fromStream, toStream);
            }

            // Step 3 : Start up streams
            foreach (CANStreamer stream in streamsTemp)
            {
                if (stream.LastState == ConnectionStateEnum.Connected)
                    stream.ConnectStream();
            }
        }
    }

    /// <summary>
    /// The Connector class links to output of one stream to the input of the other.
    /// It transfers the frame via itself, so it has the ability to filter the frames it passes on (Not currently in use)
    /// </summary>
    public class Connector
    {
        public int fromID
        {
            get
            {
                return _fromID;
            }
            set
            {
                if (_fromID != default(int))
                    throw new Exception("Cannot change connector ID !");
                _fromID = value;
            }
        }
        public int toID
        {
            get
            {
                return _toID;
            }
            set
            {
                if (_toID != default(int))
                    throw new Exception("Cannot change connector ID !");
                _toID = value;
            }
        }

        private int _fromID;
        private int _toID;
        public readonly CANStreamer _fromStream;
        public readonly CANStreamer _toStream;

        private Connector()
        {
            // For xml serializer only
        }
        public Connector(CANStreamer fromStream, CANStreamer toStream)
        {
            _fromStream = fromStream;
            _toStream = toStream;

            fromID = _fromStream.ID;
            toID = _toStream.ID;

            _fromStream.RawReceived += Filter;
        }

        public void Disconnect()
        {
            _fromStream.RawReceived -= Filter;
        }

        public void Filter(object sender, FrameReceivedEventArgs e)
        {
            _toStream.SendFrame(e.ReceivedFrame);
        }
    }
}
