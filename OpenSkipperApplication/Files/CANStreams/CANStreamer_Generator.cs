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

using CANDefinitions;
using CANHandler;
using Parameters;
using System;
using System.Text;
using System.Threading;

namespace CANStreams
{
    /// <summary>
    /// Generates NMEA 0183 messages from parameters
    /// </summary>
    public class CANStreamer_Generator : CANStreamer
    {
        public override string Type
        {
            get { return "Generator"; }
        }
        public int Interval
        {
            get
            {
                return _Interval;
            }
            set
            {
                _Interval = value;
            }
        }

        public string MessagePrefix
        {
            get { return _MessagePrefix; }
            set { _MessagePrefix = value; }
        }

        public string MessageBody
        {
            get { return _MessageBody; }
            set
            {
                _MessageBody = value;
                _SplitBody = _MessageBody.Split(',');
            }
        }

        private readonly DateTime gpsEpochStart = new DateTime(1980, 1, 6);
        private string _MessagePrefix;
        private string _MessageBody;
        private string[] _SplitBody = new string[0];
        private int _Interval = 1000;
        private System.Threading.Timer _Timer;

        public CANStreamer_Generator()
        {
            _Timer = new System.Threading.Timer(_Timer_Tick);
        }

        private void _Timer_Tick(object state)
        {
            StringBuilder message = new StringBuilder(_MessagePrefix);

            lock (Definitions.ParamCol.UpdateLocker)
            {
                for (int i = 0; i < _SplitBody.Length; i++)
                {
                    string bodyPart = _SplitBody[i].Trim();
                    if (bodyPart == "[Date]")
                    {
                        message.Append("," + (int)((DateTime.Now - gpsEpochStart).TotalDays / 7));
                        continue;
                    }
                    else if (bodyPart == "[Time]")
                    {
                        int dayOfWeek = (int)((DateTime.Now - gpsEpochStart).TotalDays % 7);
                        message.Append("," + (dayOfWeek * 24 * 60 * 60 + (int)DateTime.Now.TimeOfDay.TotalSeconds));
                        continue;
                    }
                    else if (bodyPart != "")
                    {
                        bool found = false;
                        foreach (Parameter p in Definitions.ParamCol.ClonedParameters)
                        {
                            NumericParameter np = p as NumericParameter;
                            if ((np != null) && (np.InternalName == bodyPart))
                            {
                                Parameter.ParameterStateEnum pstate;
                                DateTime dt;
                                double v;

                                v = np.GetValueAndState(out pstate, out dt);

                                found = true;
                                message.Append("," + np.Formatter.FormatToN0183(v, pstate));
                                break;
                            }
                        }

                        if (!found)
                        {
                            message.Append(",");
                            ReportHandler.LogWarning("[Generator] Parameter '" + bodyPart + "' not found");
                        }
                    }
                    else
                    {
                        message.Append(",");
                    }
                }
            }

            // Add checksum
            byte checksum = 0;
            for (int i = 1; i < message.Length; i++)
                checksum ^= Convert.ToByte(message[i]);
            message.Append("*" + checksum.ToString("X2"));

            N0183Frame generatedFrame = new N0183Frame(message.ToString(), DateTime.Now);
            OnRawReceived(generatedFrame);
        }

        public override void ConnectStream()
        {
            if (ConnectionState == ConnectionStateEnum.Connected)
                return;

            _Timer.Change(_Interval, _Interval);
            ConnectionState = ConnectionStateEnum.Connected;
        }
        public override void Disconnect()
        {
            if (ConnectionState == ConnectionStateEnum.Disconnected)
                return;

            _Timer.Change(Timeout.Infinite, Timeout.Infinite);
            ConnectionState = ConnectionStateEnum.Disconnected;
        }
    }

} // namespace
