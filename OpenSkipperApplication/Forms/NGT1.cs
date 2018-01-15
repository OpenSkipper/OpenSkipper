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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace OpenSkipperApplication
{
    public partial class NGT1 : Form
    {
        public enum ListenMode
        {
            NMEA2000,
            NMEA0183,
            HEX
        }
        
        const int Escape = 0x10;
        const int StartOfText = 0x02;
        const int EndOfText = 0x03;
        const int N2kData = 0x93;
        const int BEMCMD = 0xA0;
        const char EscapeChar = (char)Escape;
        const int StartOfTextChar = (char)StartOfText;
        const int EndOfTextChar = (char)EndOfText;
        const int packetBufferSize = 1024;
        byte[] packetBuffer = new byte[packetBufferSize];
        byte[] buffer = new byte[5000];
        private System.Object lockThis = new System.Object();
        int packetBytes = 0;
        /*
        const int ReadBufferSize = 1024;
        byte[] ReadBuffer = new byte[ReadBufferSize];
        int ReadBufferPosn = 0;
        bool MessageHeaderRead = false;
        bool EscapeJustRead = false;
        int ErrorCount; // Count of number of errors that have occured
        */

        string CommPortName;
        ListenMode listenMode = ListenMode.NMEA2000;
        bool escFound = false;
        bool startFound = false;
        int EventCount=0;
        int EventBytes = 0;

        public NGT1(string commPortName)
        {
            InitializeComponent();
            serialPort1.Encoding = System.Text.Encoding.UTF8;
            CommPortName = commPortName;
            

            string[] portNames = SerialPort.GetPortNames();
            if (portNames.Length > 0)
            {
                foreach (string portName in portNames)
                {
                    ToolStripItem actisenseItem = new ToolStripMenuItem() { Text = portName };
                    //                    actisenseItem.Click += new EventHandler(actisenseNGT1USBSubItem_Click);
                    if (CommonRoutines.TestIsPortFree(portName)) toolStripCBPort.Items.Add(portName);
                }
            }

            toolStripCBMode.SelectedIndex = 0;
            toolStripCBBaud.SelectedIndex = 2;
        }

        private void serialPort1_N2kDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // This event is called from a seperate thread to main thread
            // We need to invoke on main thread so that we can update controls
            this.Invoke(new EventHandler(N2kDataReceived));
            //lock (lockThis)
            //{
            //  EventCount++;
            //  int bytesToRead = serialPort1.BytesToRead;
            //  EventBytes += bytesToRead;
            //  serialPort1.Read(buffer, 0, bytesToRead);
            //}
        }

        private void serialPort1_NMEA0183DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // This event is called from a seperate thread to main thread
            // We need to invoke on main thread so that we can update controls
            this.Invoke(new EventHandler(NMEA0183DataReceived));
        }

        private void serialPort1_HEXDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // This event is called from a seperate thread to main thread
            // We need to invoke on main thread so that we can update controls
            this.Invoke(new EventHandler(HEXDataReceived));
        }

        private string AddUncompleteData(byte[] buffer, int len)
        {
            string HexString = "";
            for (int i = 0; i < len; i++) HexString += buffer[i].ToString("X2");
            return HexString;
        }

        private string MakeCompletePacket(byte[] buffer, int len)
        {
            string HexString = "\r\n<SOP>";
            string HexStringRaw = "\r\n " + Escape.ToString("X2") + StartOfText.ToString("X2");
            int i = 0;
            int PGN = 0;

            if (len >= 6)
            {
                switch (buffer[0])
                {
                    case N2kData:
                        HexString += "<N2k>, len=" + buffer[1].ToString("D3") + ", priority=" + buffer[2].ToString("D2");
                        HexStringRaw += ",  " + buffer[0].ToString("X2") + ",      " + buffer[1].ToString("X2") + ",          " + buffer[2].ToString("X2");
                        i += 3;
                        PGN = buffer[i] + (buffer[i + 1] << 8) + (buffer[i + 2] << 16);
                        HexString += ", PGN=" + PGN.ToString();
                        HexStringRaw += ",     "+buffer[i].ToString("X2")+buffer[i+1].ToString("X2")+buffer[i+2].ToString("X2");
                        i += 3;

                        HexString += ", dst=" + buffer[i].ToString("D3");
                        HexStringRaw += ",      " + buffer[i++].ToString("X2");

                        HexString    += ", src=" + buffer[i].ToString("D3");
                        HexStringRaw += ",      " + buffer[i++].ToString("X2");

                        HexString += ", time=" + buffer[i].ToString("X2") + buffer[i + 1].ToString("X2") + buffer[i + 2].ToString("X2") + buffer[i + 3].ToString("X2");
                        HexStringRaw += ",      " + buffer[i].ToString("X2") + buffer[i + 1].ToString("X2") + buffer[i + 2].ToString("X2") + buffer[i + 3].ToString("X2");
                        i += 4;

                        HexString    += ", datalen=" + buffer[i].ToString("D3");
                        HexStringRaw += ",          " + buffer[i++].ToString("X2");
                        HexString    += ", data=";
                        HexStringRaw += ",      ";
                        break;
                    case BEMCMD:
                        HexString += "<BEMCMD>, " + buffer[0].ToString("X2") + buffer[2].ToString("X2")
                                   + " len=" + buffer[1].ToString() + ", data=";
                        i += 3;
                        break;
                }
            }

            for (; i < len - 1; i++)
            {
                HexString += buffer[i].ToString("X2");
                HexStringRaw += buffer[i].ToString("X2");
            }
            HexString += ", crc="+buffer[i].ToString("X2");
            HexStringRaw += ",     " + buffer[i].ToString("X2") + ", " + Escape.ToString("X2") + EndOfText.ToString("X2");
            return HexStringRaw+HexString + " <EOP>";
        }

        private void HEXDataReceived(object sender, EventArgs e)
        {
            EventCount++;
            while (true)
            {
                int bytesToRead = serialPort1.BytesToRead;
                EventBytes += bytesToRead;
                if (!(bytesToRead > 0))
                    return;

                byte[] buffer = new byte[bytesToRead];
                int readBytes = serialPort1.Read(buffer, 0, bytesToRead);

                string HexString="";
                for (int i = 0; i < readBytes; i++) HexString += buffer[i].ToString("X2");
                textBox1.AppendText(HexString);
            }
        }

        private void NMEA0183DataReceived(object sender, EventArgs e)
        {
            EventCount++;
            while (true)
            {
                int bytesToRead = serialPort1.BytesToRead;
                EventBytes += bytesToRead;
                if (!(bytesToRead > 0))
                    return;
             
                byte[] buffer = new byte[bytesToRead];
                int readBytes = serialPort1.Read(buffer, 0, bytesToRead);

                string DataString = "";
                for (int i = 0; i < readBytes; i++) DataString += Convert.ToChar(buffer[i]);
                textBox1.AppendText(DataString);
            }
        }

        private void N2kDataReceived(object sender, EventArgs e)
        {
            // Decode the Actisence low level messages
            // Each message goes <10><02><length-7><data1>...<datan><10><03> (in hex)
            // A <10> in the raw data in sent in escaped from '<10><10>'; this counts as 1 byte for the length
            // The full message length (<10><02>...<10><03>) is 7 bytes longer than the third byte.

            EventCount++;
            while (true)
            {
                int bytesToRead = serialPort1.BytesToRead;
                EventBytes += bytesToRead;
                if (!(bytesToRead > 0))
                    return;

//                byte[] buffer = new byte[bytesToRead];
                int readBytes = serialPort1.Read(buffer, 0, bytesToRead);

                string HexString="";
                for (int i = 0; i < readBytes; i++)
                {
                    if (escFound)
                    {
                        if (buffer[i] == StartOfText)
                        {
                            // If we already have bytes in packet, data is uncomplete
                            if (packetBytes>0) HexString = (startFound?"\r\n<ERR><SOP>":"\r\n") + AddUncompleteData(packetBuffer, packetBytes);
                            textBox1.AppendText(HexString);
                            packetBytes = 0;
                            startFound = true;
                        }
                        else if (buffer[i] == EndOfText)
                        {
                            if (startFound)
                            {
                                textBox1.AppendText(MakeCompletePacket(packetBuffer, packetBytes));
                                //MakeCompletePacket(packetBuffer, packetBytes);
                                packetBytes = 0;
                            }
                            else
                            {
                                textBox1.AppendText("\r\n<ERR>"+AddUncompleteData(packetBuffer, packetBytes)+"<EOP>");
                                packetBytes = 0;
                            }
                            startFound = false;
                        }
                        else // was not SOP or EOP so just data
                        {
//                            packetBuffer[packetBytes++] = Escape;
                            packetBuffer[packetBytes++] = buffer[i];
                        }
                        escFound = false;
                    }
                    else
                    {
                        if ((buffer[i] == Escape))
                        {
                            escFound = true;
                        }
                        else
                        {
                            packetBuffer[packetBytes++] = buffer[i];
                        }
                    }
                }

//                textBox1.AppendText(Encoding.ASCII.GetString(buffer, 0, readBytes) + "\r\n");
                // print out bufeer, if we have reached the max size
                if (packetBytes == packetBufferSize)
                {
                    textBox1.AppendText("\r\n"+AddUncompleteData(packetBuffer, packetBytes));
                    packetBytes=0;
                }
            }
            
            /*
            int iByteRead;
            while ( serialPort1.BytesToRead>0) {
                iByteRead = serialPort1.ReadByte();
                if (EscapeJustRead) {
                    // Process second char in escape sequence
                    if (iByteRead == Escape) {
                        ReadBuffer[ReadBufferPosn++] = Escape;
                    } else if (iByteRead == StartOfTextChar) {
                        // Found the start of a new message
                        if (ReadBufferPosn>0) {
                            ErrorCount++; // there was a partial message that we are discarding
                        } else {
                            ReadBufferPosn = 0;
                        }
                        MessageHeaderRead = true;
                    } else if (iByteRead == EndOfTextChar) {
                        if (!MessageHeaderRead) {
                            ErrorCount++;   // no previous start-of-message
                        } else {
                            // We have completed a message
                            MessageHeaderRead=false;
                            // this.Invoke(new EventHandler(NewMessageReceived));
                            // Output message to the text box
                            textBox1.AppendText(System.BitConverter.ToString(ReadBuffer,0,ReadBufferPosn)+"\n");
                            // Clear buffer
                            ReadBufferPosn = 0;
                        }
                    } else {
                        // unrecognised escape sequence; we ignore it
                        ErrorCount++;
                    }
                    EscapeJustRead = false;
                } else if (iByteRead==EscapeChar) {
                    EscapeJustRead = true;
                } else if (MessageHeaderRead){
                    // More data after a valid message header
                    if (ReadBufferPosn==ReadBufferSize) throw new Exception("Buffer full.");
                    ReadBuffer[ReadBufferPosn++] = (byte)iByteRead;
                } else {
                    // Data found, but no previous MessageHeaderRead
                    ReadBufferPosn++;   // We just count the bad characters read
                }
            }
            //string s = serialPort.ReadExisting();
            //for (int i = 0; i < s.Length; i++) {
            //    if (EscapeJustRead) {
            //        // Process second char in escape sequence
            //        if (s[i] == EscapeChar) {
            //            ReadBuffer[ReadBufferPosn++] = Escape;
            //        } else if (s[i] == StartOfTextChar) {
            //            // Found the start of a new message
            //            if (ReadBufferPosn > 0) {
            //                ErrorCount++; // there was a partial message that we are discarding
            //            } else {
            //                ReadBufferPosn = 0;
            //            }
            //            MessageHeaderRead = true;
            //        } else if (s[i] == EndOfTextChar) {
            //            if (!MessageHeaderRead) {
            //                ErrorCount++;   // no previous start-of-message
            //            } else {
            //                // We have completed a message
            //                MessageHeaderRead = false;
            //                // this.Invoke(new EventHandler(NewMessageReceived));
            //                // Output message to the text box
            //                textBox1.AppendText(System.BitConverter.ToString(ReadBuffer, 0, ReadBufferPosn) + "\n");
            //                // Clear buffer
            //                ReadBufferPosn = 0;
            //            }
            //        } else {
            //            // unrecognised escape sequence; we ignore it
            //            ErrorCount++;
            //        }
            //        EscapeJustRead = false;
            //    } else if (s[i] == EscapeChar) {
            //        EscapeJustRead = true;
            //    } else if (MessageHeaderRead) {
            //        // More data after a valid message header
            //        if (ReadBufferPosn == ReadBufferSize) throw new Exception("Buffer full.");
            //        ReadBuffer[ReadBufferPosn++] = (byte)s[i];
            //    } else {
            //        // Data found, but no previous MessageHeaderRead
            //        ReadBufferPosn++;   // We just count the bad characters read
            //    }
            //}
             * */
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            try
            {
                serialPort1.PortName = CommPortName;
                serialPort1.BaudRate = Int32.Parse(toolStripCBBaud.Text);

                // We need to clear old events. Clear all known events was easiest way I found.
                serialPort1.DataReceived -= new SerialDataReceivedEventHandler(serialPort1_N2kDataReceived);
                serialPort1.DataReceived -= new SerialDataReceivedEventHandler(serialPort1_NMEA0183DataReceived);
                serialPort1.DataReceived -= new SerialDataReceivedEventHandler(serialPort1_HEXDataReceived);
                EventCount = 0;
                EventBytes = 0;
                serialPort1.Open();
                timerEventBytesAvg.Enabled = true;
                //                string Dummy = serialPort1.ReadExisting();
                serialPort1.DiscardInBuffer();
                switch (listenMode)
                { 
                    case ListenMode.NMEA2000:
                        serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_N2kDataReceived);
                        break;
                    case ListenMode.NMEA0183:
                        serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_NMEA0183DataReceived);
                        break;
                    case ListenMode.HEX:
                        serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_HEXDataReceived);
                        break;
                }

                toolStripButtonOpen.Enabled = false;
                toolStripButtonClose.Enabled = true;
                toolStripCBPort.Enabled = false;
                toolStripCBBaud.Enabled = false;
                toolStripCBMode.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open serial port connection. Exception: " + ex.ToString()); 

            }
        }

        private void NGT1_FormClosing(object sender, FormClosingEventArgs e) {
            if (serialPort1.IsOpen)
            {
                e.Cancel = true;
                // With modeless form there was problem with closing port here, so we use timer for closing.
                timerClose.Enabled = true;
//                MessageBox.Show("Can not close form, when port is open.");
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e) {
            timerEventBytesAvg.Enabled = false;
            serialPort1.Close();
            toolStripButtonOpen.Enabled = true;
            toolStripButtonClose.Enabled = false;
            toolStripCBPort.Enabled = true;
            toolStripCBBaud.Enabled = true;
            toolStripCBMode.Enabled = true;
        }

        private void com1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void toolStripDropDownButton1_DropDownOpening(object sender, EventArgs e)
        {

        }

        private void toolStripCBPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommPortName = toolStripCBPort.Text;
            toolStripButtonOpen.Enabled = true;
        }

        private void toolStripCBMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            listenMode = (ListenMode)(toolStripCBMode.SelectedIndex);
        }

        private void timerClose_Tick(object sender, EventArgs e)
        {
            timerClose.Enabled = false;
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                timerClose.Enabled = true;
            }
            else
            {
                Close();
            }
        }

        private void toolStripCBPort_DropDown(object sender, EventArgs e)
        {

        }

        private void timerEventBytesAvg_Tick(object sender, EventArgs e)
        {
            lock (lockThis)
            {
                if (EventCount > 0)
                {
                    valueEventBytesAvg.Value = EventBytes / EventCount;
                }
                else
                {
                    valueEventBytesAvg.Value = 0;
                }
            }
        }
    }
}
