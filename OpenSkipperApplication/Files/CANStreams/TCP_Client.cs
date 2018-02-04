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
using CANReaders;
using CANSenders;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CANStreams
{
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
                try
                {
                    // Note: These lines regularly cause exceptions which we just ignore
                    _tcpClient.Client.Shutdown(SocketShutdown.Both);
                    _tcpClient.Client.Close(1000);
                }
                catch (Exception)
                {
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

} // namespace
