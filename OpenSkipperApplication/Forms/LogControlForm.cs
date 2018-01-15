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
using CANStreams;

namespace OpenSkipperApplication
{
    public partial class LogControlForm : Form
    {
        private CANStreamer_Logfile _logStream;

        public LogControlForm(CANStreamer_Logfile logStream)
        {
            InitializeComponent();
            _logStream = logStream;
            cANStreamerLogfileBindingSource.DataSource = _logStream;

            _logStream.PropertyChanged += new PropertyChangedEventHandler(_logStream_PropertyChanged);

            // This form is owned by the main form
            var mainForm = Application.OpenForms["MainForm"];
            if (mainForm != null) {
                mainForm.AddOwnedForm(this);
            }
        }

        private void LogControlForm_VisibleChanged(object sender, EventArgs e) {
            // We start up the timer if, and only if, the form is visible
            // The timer updates the date/time display
            timer1.Enabled = this.Visible;
        }

        void _logStream_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ConnectionState")
            {
                if (_logStream.ConnectionState == ConnectionStateEnum.Connected)
                {
                    this.Show();
                }
                else
                {
                    this.Hide();
                }
            }
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            _logStream.Rewind();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //lblLogTime.Text = _logStream.LogTime.ToLongTimeString();
            lblLogTime.Text = _logStream.LogTime.ToShortDateString() + " " + _logStream.LogTime.ToString("hh:mm:ss");
            lblLogTime.Refresh();
        }

        private void LogControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                _logStream.Disconnect();
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            _logStream.Paused = false;
            btnNext.Enabled = false;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _logStream.Paused = true;
            btnNext.Enabled = true;
        }

        private void btnPlay_EnabledChanged(object sender, EventArgs e)
        {
            btnPause.Enabled = !btnPlay.Enabled;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _logStream.SendNextPacket();
        }

        private void numPlayspeed_ValueChanged(object sender, EventArgs e) {
            // Although the speed is data bound, it does not update until it loses focus
            // Hence, we force more frequent updates
            _logStream.PlaySpeed = (float)numPlayspeed.Value;
        }

    }
}
