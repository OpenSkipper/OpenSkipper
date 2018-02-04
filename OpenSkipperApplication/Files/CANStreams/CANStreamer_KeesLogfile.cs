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
    public class CANStreamer_KeesLogfile : CANStreamer_Logfile
    {
        // Properties
        public static string FileFilter = "Kees Log File (*.*)|*.*";

        // Variables for reading the frames
        private StreamReader fileReader;

        // Constructors
        public CANStreamer_KeesLogfile()
            : base()
        {
        }

        // Gets next packet from file
        public override Frame getNextPacket()
        {
            string line;
            int errorCount = 0;
            while ((line = fileReader.ReadLine()) != null)
            {
                try
                {
                    char[] charSeparators = new char[] { ',' };
                    var elements = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    // DateTime Date = DateTime.ParseExact(elements[0], "yyyy-MM-dd'Z'HH':'mm':'ss'.'FFF", System.Globalization.CultureInfo.InvariantCulture);
                    if (elements[0][10] == 'Z') elements[0] = elements[0].Replace('Z', 'T');  // Get rid of local time designation so we write back out as came in
                    DateTime Date = DateTime.Parse(elements[0]);
                    //DateTime Date = DateTime.SpecifyKind(DateTime.Parse(elements[0]), DateTimeKind.Unspecified);
                    byte Priority = Convert.ToByte(elements[1]);
                    int PGN = Convert.ToInt32(elements[2]);
                    byte Source = Convert.ToByte(elements[3]);
                    byte Destination = Convert.ToByte(elements[4]);
                    int ByteCount = Convert.ToInt32(elements[5]);
                    N2kHeader hdr = new N2kHeader(Priority, PGN, Destination, Source);

                    if ((hdr.PGN != PGN) || (hdr.PGNSource != Source) || (hdr.PGNDestination != Destination) || (hdr.PGNPriority != Priority))
                    {
                        N2kHeader hdr2 = new N2kHeader(Priority, PGN, Destination, Source);
                    }

                    Byte[] Bytes = new Byte[ByteCount];
                    for (int i = 0; i < ByteCount; i++)
                    {
                        Bytes[i] = byte.Parse(elements[6 + i], System.Globalization.NumberStyles.HexNumber);
                    }

                    // Return it
                    var N2kMsg = new N2kFrame(hdr, Bytes, new DateTime());// { ms = 0, Data = Bytes, Header = hdr, TimeStamp = Date };
                    return N2kMsg;
                }
                catch
                {
                    // Exception... Move on, unless we have had too many
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

        protected override void Init()
        {
            base.Init();
            fileReader = new StreamReader(_fileStream);
        }
        protected override void DeInit()
        {
            base.DeInit();
            fileReader.Close();
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
                    return; // Empty log file..

                _logTime = nextPacket.TimeStamp;
                _lastTime = DateTime.Now;
            }
        }
    }

} // namespace
