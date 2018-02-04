/*
	Copyright (C) 2018, Michael Lucas

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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CANStreams
{
    public class CANStreamer_YDWG02_2000 : CANStreamer_Logfile
    {
        // ****************************************************************************************
        // NOTE: 
        // Make sure to add this class to the XmlIncludes on the abstract class CANStreamer.
        // Otherwise when the settings files are saved, a serilization exception will occur!!
        // ****************************************************************************************

        // Properties
        public static string FileFilter = "YDWG-02 Log File (*.*)|*.*";

        // Variables for reading the frames
        private StreamReader fileReader;
        private N2kRawReader rawReader;

        protected override void Init()
        {
            base.Init();
            fileReader = new StreamReader(_fileStream);
            rawReader = new N2kRawReader();
        }
        protected override void DeInit()
        {
            base.DeInit();
            fileReader.Close();
        }

        public override Frame getNextPacket()
        {
            string line;
            int errorCount = 0;
            while ((line = fileReader.ReadLine()) != null)
            {
                try
                {
                    var data = Encoding.ASCII.GetBytes(line);
                    var frames = rawReader.GetFrames(data);
                    var builder = new N2kFrame.Builder();
                    foreach (var frame in frames)
                    {
                        //var built = builder.AddFrame(frame);
                        //if (built != null)
                        //{
                        return frame;
                        //}
                    }

                    return null;
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());

                    errorCount++;

                    if (errorCount > 10)
                    {
                        MessageBox.Show("A large number of errors occured while reading log file " + Name + ". Reading has been stopped.");
                        return null;
                    }
                }
            }

            // EOF
            return null;
        }

        public override void Rewind()
        {
            lock (packetLocker)
            {
                MessageHandler.Reset();

                _fileStream.Seek(0, SeekOrigin.Begin);
                fileReader = new StreamReader(_fileStream);

                nextPacket = getNextPacket();
                if (nextPacket == null)
                {
                    return; // Empty log file..
                }

                _logTime = nextPacket.TimeStamp;
                _lastTime = DateTime.Now;
            }
        }

    } // class

} // namespace
