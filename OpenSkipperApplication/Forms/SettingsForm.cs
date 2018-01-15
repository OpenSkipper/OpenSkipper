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
using Parameters;
using OpenSkipperApplication;
using OpenSkipperApplication.Properties;
using CANDefinitions;

namespace OpenSkipperApplication
{
    public partial class SettingsForm : Form
    {
        private bool formLoaded = false;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            if (Settings.Default.N2kPath != "")
            {
                optPGNCustom.Checked = true;
                txtN2kPath.Text = Settings.Default.N2kPath;
            }

            if (Settings.Default.N0183Path != "")
            {
                optN0183Custom.Checked = true;
                txtN0183Path.Text = Settings.Default.N0183Path;
            }

            if (Settings.Default.ParametersPath != "")
            {
                optParamCustom.Checked = true;
                txtParamPath.Text = Settings.Default.ParametersPath;
            }

            if (Settings.Default.AISPath != "")
            {
                optAISCustom.Checked = true;
                txtAISPath.Text = Settings.Default.AISPath;
            }

            txtDisplaysPath.Text = Settings.Default.Displays;
            cbHideMenusOnStart.Checked = Settings.Default.HideMenusOnStart;
            txtWWWRoot.Text = Settings.Default.WWWRoot;
            valueWWWPort.Value = Settings.Default.WWWPort;

            formLoaded = true;
        }

        private void btnChangeN2k_Click(object sender, EventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Title = "Open NMEA 2000 Definition File";
            fDialog.Filter = Constants.XMLPGNDefnFileFilter;
            if ((fDialog.ShowDialog() == DialogResult.OK) && (fDialog.CheckFileExists == true))
            {
                txtN2kPath.Text = fDialog.FileName;
                optPGNCustom.Checked = true;
            }
        }
        private void btnChangeN0183_Click(object sender, EventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Title = "Open NMEA 0183 Definition File";
            fDialog.Filter = Constants.XMLN0183DefnFileFilter;
            if ((fDialog.ShowDialog() == DialogResult.OK) && (fDialog.CheckFileExists == true))
            {
                txtN0183Path.Text = fDialog.FileName;
                optN0183Custom.Checked = true;
            }
        }
        private void btnChangeParam_Click(object sender, EventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Title = "Open Parameter Definition File";
            fDialog.Filter = Constants.XMLParameterFileFilter;
            if ((fDialog.ShowDialog() == DialogResult.OK) && (fDialog.CheckFileExists == true))
            {
                txtParamPath.Text = fDialog.FileName;
                optParamCustom.Checked = true;
            }
        }
        private void btnChangeAIS_Click(object sender, EventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Title = "Open AIS Definition File";
            fDialog.Filter = Constants.XMLAISDefnFileFilter;
            if ((fDialog.ShowDialog() == DialogResult.OK) && (fDialog.CheckFileExists == true))
            {
                txtAISPath.Text = fDialog.FileName;
                optAISCustom.Checked = true;
            }
        }

        private void btnChangeDisplays_Click(object sender, EventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Title = "Open Displays Definition File";
            fDialog.Filter = Constants.XMLDisplayDefnFileFilter;
            if ((fDialog.ShowDialog() == DialogResult.OK) && (fDialog.CheckFileExists == true))
            {
                txtDisplaysPath.Text = fDialog.FileName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtN2kPath.Text != Settings.Default.N2kPath) Definitions.LoadPGNDefns(txtN2kPath.Text);
            if (txtN0183Path.Text != Settings.Default.N0183Path) Definitions.LoadN0183Defns(txtN0183Path.Text);
            if (txtParamPath.Text != Settings.Default.ParametersPath) Definitions.LoadParameters(txtParamPath.Text);
            if (txtAISPath.Text != Settings.Default.AISPath) Definitions.LoadAISDefns(txtAISPath.Text);
            if (txtDisplaysPath.Text != Settings.Default.Displays)
            {
                // Load or note for restart!
                Settings.Default.Displays = txtDisplaysPath.Text; 
            }
            if (txtWWWRoot.Text!=Settings.Default.WWWRoot) 
            {
                // Load or note for restart!
                Settings.Default.WWWRoot = txtWWWRoot.Text;
            }
            if (valueWWWPort.Value != Settings.Default.WWWPort)
            {
                // Load or note for restart!
                Settings.Default.WWWPort = System.Convert.ToUInt32(valueWWWPort.Value);
            }
            Settings.Default.HideMenusOnStart = cbHideMenusOnStart.Checked;
            Settings.Default.Save();

            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void optPGNCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (!formLoaded) return;

            if (optPGNCustom.Checked == true)
            {
                if (txtN2kPath.Text == "")
                    btnChangeN2k_Click(null, null);
            }
            else
                txtN2kPath.Text = "";
        }
        private void optN0183Custom_CheckedChanged(object sender, EventArgs e)
        {
            if (!formLoaded) return;

            if (optN0183Custom.Checked == true)
            {
                if (txtN0183Path.Text == "")
                    btnChangeN0183_Click(null, null);
            }
            else
                txtN0183Path.Text = "";
        }
        private void optParamCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (!formLoaded) return;

            if (optParamCustom.Checked == true)
            {
                if (txtParamPath.Text == "")
                    btnChangeParam_Click(null, null);
            }
            else
                txtParamPath.Text = "";
        }
        private void optAISCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (!formLoaded) return;

            if (optAISCustom.Checked == true)
            {
                if (txtAISPath.Text == "")
                    btnChangeAIS_Click(null, null);
            }
            else
                txtAISPath.Text = "";
        }
    }
}
