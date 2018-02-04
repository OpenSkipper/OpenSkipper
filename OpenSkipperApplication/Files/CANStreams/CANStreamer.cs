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
using CANHandler;
using System;
using System.ComponentModel;
using System.Threading;
using System.Xml.Serialization;

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
    /// It is used to feed consumers of the NMEA messages.
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
            {
                OnFrameReceived(decodedPacket);
            }
        }

        private void OnFrameReceived(Frame msg)
        {
            if (ConnectionState == ConnectionStateEnum.Disconnected) return;    // Multi-threading protection

            // Fire handler for generic message received
            FrameReceived?.Invoke(this, new FrameReceivedEventArgs(msg, this));

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
                ParametersNotified?.Invoke(this, EventArgs.Empty);
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void SendFrame(Frame msg)
        {
            throw new NotSupportedException();
        }
        public abstract void ConnectStream();
        public abstract void Disconnect();
    }

} // namespace
