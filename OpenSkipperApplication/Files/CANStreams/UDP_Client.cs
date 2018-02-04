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
using CANSenders;
using System;
using System.Net;
using System.Net.Sockets;

namespace CANStreams
{
    /// <summary>
    /// Wrapper classes around TCP/UDP clients
    /// </summary>
    public class UDP_Client : IDisposable
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
            {
                // Jan 21, 2018 - mlucas
                // Check if this send is a UPD broadcast (send to anyone listening).
                // When no host is provided also assume sending UDP broadcast.
                if (string.IsNullOrEmpty(_hostname)
                    || _hostname == IPAddress.Any.ToString()
                    || _hostname == IPAddress.Broadcast.ToString())
                {
                    // Create a broadcast endpoint on the specified port
                    var ip = new IPEndPoint(IPAddress.Broadcast, Port);

                    // Send the data via broadcast on the port
                    _udpClient.Send(datagram, datagram.Length, ip);
                }
                else
                {
                    // Send the data to the host and port
                    _udpClient.Send(datagram, datagram.Length, _hostname, Port);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (_udpClient != null)
                {
                    // Clean up our connections
                    _udpClient.Close();
                }
            }
            // free native resources if there are any.
        }

    } // class

} // namespace
