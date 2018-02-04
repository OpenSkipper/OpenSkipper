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
using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace CANStreams
{
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
        private ActisenseReader _actisenseInterface;

        // Constructor
        public CANStreamer_NGT1_2000()
            : base()
        {
            _actisenseInterface = new ActisenseReader();
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

} // namespace
