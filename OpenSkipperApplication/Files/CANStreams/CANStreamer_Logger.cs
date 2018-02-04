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
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace CANStreams
{
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

} // namespace
