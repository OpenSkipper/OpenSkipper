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
using OpenSkipperApplication;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CANStreams
{
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

                try
                {

                    if (nextPacket == null)
                        nextPacket = getNextPacket();

                    while (nextPacket != null)
                    {
                        OnRawReceived(nextPacket);
                        nextPacket = getNextPacket();
                    }
                }
                finally
                {
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
        public override void Disconnect()
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

} // namespace
