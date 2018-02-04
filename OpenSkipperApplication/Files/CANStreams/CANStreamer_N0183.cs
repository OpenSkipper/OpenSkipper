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
using System.IO.Ports;
using System.Windows.Forms;

namespace CANStreams
{
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

} // namespace
