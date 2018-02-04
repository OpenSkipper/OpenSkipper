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
using System.Xml;

namespace CANStreams
{
    public class CANStreamer_XMLLogfile : CANStreamer_Logfile
    {
        /*public static List<Frame> GetAllFrames(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                XmlReader xmlr = XmlReader.Create(fs, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment });
                List<Frame> frameList = new List<Frame> { };

                while (xmlr.Read())
                {
                    if (xmlr.Name == "N2kMsg")
                    {
                        frameList.Add((N2kFrame)N2kFrame.XmlSerializer.Deserialize(new StringReader(xmlr.ReadOuterXml())));
                    }
                    else if (xmlr.Name == "N0183Msg")
                    {
                        frameList.Add((N0183Frame)N0183Frame.XmlSerializer.Deserialize(new StringReader(xmlr.ReadOuterXml())));
                    }
                }
            }

            return frameList;
        }
        */

        // Properties
        public static string FileFilter = "XML Log File (*.xml)|*.xml";

        // Variables for reading the frames
        private System.Xml.XmlReader xmlReader;

        // Constructors
        public CANStreamer_XMLLogfile()
            : base()
        {
        }

        // Gets next packet from file
        public override Frame getNextPacket()
        {
            while (true)
            {
                try
                {
                    if (!xmlReader.Read())
                        return null;
                }
                catch
                {
                    return null;
                }

                Frame newPacket;
                if (xmlReader.Name == "N2kMsg")
                {
                    try
                    {
                        var outerXml = xmlReader.ReadOuterXml();
                        var outerReader = new StringReader(outerXml);
                        newPacket = (N2kFrame)N2kFrame.XmlSerializer.Deserialize(outerReader);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error reading logfile: " + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                        return null;
                    }
                    return newPacket;
                }
                if (xmlReader.Name == "N0183Msg")
                {
                    try
                    {
                        string outerXml = xmlReader.ReadOuterXml();
                        StringReader outerReader = new StringReader(outerXml);
                        newPacket = (N0183Frame)N0183Frame.XmlSerializer.Deserialize(outerReader);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error reading logfile: " + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                        return null;
                    }
                    return newPacket;
                }
                if (xmlReader.Name == "AISMsg")
                {
                    try
                    {
                        string outerXml = xmlReader.ReadOuterXml();
                        StringReader outerReader = new StringReader(outerXml);
                        newPacket = (AISFrame)AISFrame.XmlSerializer.Deserialize(outerReader);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error reading logfile: " + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                        return null;
                    }
                    return newPacket;
                }
            }
        }
        protected override void Init()
        {
            base.Init();
            xmlReader = System.Xml.XmlReader.Create(_fileStream, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment });
        }
        protected override void DeInit()
        {
            base.DeInit();
            xmlReader.Close();
        }
        public override void Rewind()
        {
            lock (packetLocker)
            {
                MessageHandler.Reset();
                _fileStream.Seek(0, SeekOrigin.Begin);
                xmlReader = System.Xml.XmlReader.Create(_fileStream, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment });

                nextPacket = getNextPacket();
                if (nextPacket == null)
                    return; // Empty log file..

                _logTime = nextPacket.TimeStamp;
                _lastTime = DateTime.Now;
            }
        }
    }

} // namespace
