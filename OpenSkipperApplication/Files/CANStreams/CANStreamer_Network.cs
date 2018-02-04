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
                if (_tcpListener != null)
                {
                    _tcpListener.Server.Close();
                    _tcpListener.Stop();
                }
                _tcpListener = null;
                // If the user wants a TCP listener running, then set one up and start it up if this stream is connected
                if (_tcpListenPort > 0)
                {
                    // Set up a new listener
                    _tcpListener = new TcpListener(IPAddress.Any, _tcpListenPort);
                    if (ConnectionState == ConnectionStateEnum.Connected)
                    {
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
                if (_udpReceiver != null)
                {
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

            // Start listener 
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

} // namespace
