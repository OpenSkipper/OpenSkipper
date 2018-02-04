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

namespace CANStreams
{
    public class CANStreamer_WeatherLogfile : CANStreamer_Logfile
    {
        // Properties
        public static string FileFilter = "Weather Log File (*.*)|*.*";

        // Public
        public override string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;

                string fileName = Path.GetFileName(_fileName);
                if (fileName.Length >= 11)
                    DateTime.TryParseExact(fileName.Substring(0, 11), "MMM_dd_yyyy", null, System.Globalization.DateTimeStyles.None, out _baseDate);
                else
                    _baseDate = new DateTime();

                NotifyPropertyChanged("FileName");
            }
        }

        // Variables for reading the frames
        private StreamReader _fileReader;
        private DateTime _baseDate;

        // Constructors
        public CANStreamer_WeatherLogfile()
            : base()
        {
        }

        // Gets next packet from file
        public override Frame getNextPacket()
        {
            int errorCount = 0;
            string line;
            while ((line = _fileReader.ReadLine()) != null)
            {
                try
                {
                    // Msg: "22:22:10.628: -> 130306 09FD023A FF030354F4FAFFFF"
                    // 0 : 22:22:10.628:
                    // 1 : ->
                    // 2 : 130306
                    // 3 : 09FD023A
                    // 4 : FF030354F4FAFFFF

                    string[] fields = line.Split(new char[] { ' ' });

                    DateTime msgTime = DateTime.ParseExact(fields[0], "HH:mm:ss.fff:", null);
                    DateTime finalTime = _baseDate.Add(msgTime.TimeOfDay);

                    byte b0 = Convert.ToByte(fields[3].Substring(0, 2), 16);
                    byte b1 = Convert.ToByte(fields[3].Substring(2, 2), 16);
                    byte b2 = Convert.ToByte(fields[3].Substring(4, 2), 16);
                    byte b3 = Convert.ToByte(fields[3].Substring(6, 2), 16);

                    byte[] data = new byte[fields[4].Length / 2];
                    for (int i = 0; i < data.Length; i++)
                        data[i] = Convert.ToByte(fields[4].Substring(2 * i, 2), 16);

                    N2kFrame N2kMsg = new N2kFrame(new N2kHeader(b0, b1, b2, b3), data, finalTime);
                    return N2kMsg;
                }
                catch
                {
                    // Exception.. move on if not too many errors
                    errorCount++;
                    if (errorCount > 10)
                    {
                        MessageBox.Show("A large number of errors occurred while reading log file " + Name + ". Reading has been stopped.");
                        return null;
                    }

                }
            }

            // EOF
            return null;
        }

        protected override void Init()
        {
            base.Init();
            _fileReader = new StreamReader(_fileStream);
        }

        protected override void DeInit()
        {
            base.DeInit();
            _fileReader.Close();
        }

        public override void Rewind()
        {
            lock (packetLocker)
            {
                MessageHandler.Reset();
                _fileStream.Seek(0, SeekOrigin.Begin);
                _fileReader = new StreamReader(_fileStream);

                nextPacket = getNextPacket();
                if (nextPacket == null)
                    return; // Empty log file..

                _logTime = nextPacket.TimeStamp;
                _lastTime = DateTime.Now;
            }
        }
    }

} // namespace
