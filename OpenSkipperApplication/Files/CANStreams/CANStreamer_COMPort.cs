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

using CANStreams;
using System;
using System.IO.Ports;

namespace CANStreams
{
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
                        System.Windows.Forms.MessageBox.Show("An error occurred when opening the COM port '" + serialPort.PortName + "': " + e.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
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

} // namespace
