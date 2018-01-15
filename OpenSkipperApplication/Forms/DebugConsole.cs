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
using CANHandler;

namespace OpenSkipperApplication.Forms
{
    public partial class DebugConsole : Form
    {
        public DebugConsole()
        {
            InitializeComponent();

            ReportHandler.WarningReceived += WarningCallback;
            ReportHandler.ErrorReceived += ErrorCallback;
        }

        private void AddLine(string s)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AddLine), s);
            }
            else
            {
                txtConsole.Text += "\r\n" + s;
            }
        }

        private void InfoCallback(string s)
        {
            AddLine("Info: " + s);
        }
        private void WarningCallback(string s)
        {
            AddLine("Warning: " + s);
        }
        private void ErrorCallback(string s)
        {
            AddLine("Error: " + s);
        }

        private void chkInfos_CheckedChanged(object sender, EventArgs e)
        {
            if (chkInfos.Checked)
                ReportHandler.InfoReceived += InfoCallback;
            else
                ReportHandler.InfoReceived -= InfoCallback;
        }

        private void chkWarnings_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWarnings.Checked)
                ReportHandler.WarningReceived += WarningCallback;
            else
                ReportHandler.WarningReceived -= WarningCallback;
        }

        private void chkErrors_CheckedChanged(object sender, EventArgs e)
        {
            if (chkErrors.Checked)
                ReportHandler.ErrorReceived += ErrorCallback;
            else
                ReportHandler.ErrorReceived -= ErrorCallback;
        }

        private void chkLog_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtConsole.Text = "";
        }
    }
}
