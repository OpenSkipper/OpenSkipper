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
using System.IO;
using System.IO.Ports;
using CANHandler;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CANDefinitions;
using CANReaders;
using CANSenders;

namespace OpenSkipperApplication.Forms
{
    public partial class StreamViewForm : Form
    {
        private FilteredStreamList<CANStreamer_Logfile> logStreams;
        private FilteredStreamList<CANStreamer_NGT1_2000> N2000portStreams;
        private FilteredStreamList<CANStreamer_N0183> N0183portStreams;
        private FilteredStreamList<CANStreamer_Network> TCPStreams;
        private FilteredStreamList<CANStreamer_Logger> LoggerStreams;
        private FilteredStreamList<CANStreamer_Generator> GenStreams;

        private CANStreamer SelectedStream()
        {
            switch (tabControl.SelectedTab.Name)
            {
                case "tpLogfile":
                    return (dgvLogfile.SelectedRows.Count > 0) ? (CANStreamer)dgvLogfile.SelectedRows[0].DataBoundItem : null;

                case "tpComPort2000":
                    return (dgvPort2000.SelectedRows.Count > 0) ? (CANStreamer)dgvPort2000.SelectedRows[0].DataBoundItem : null;

                case "tpComPort0183":
                    return (dgvPort0183.SelectedRows.Count > 0) ? (CANStreamer)dgvPort0183.SelectedRows[0].DataBoundItem : null;

                case "tpInternet":
                    return (dgvTCP.SelectedRows.Count > 0) ? (CANStreamer)dgvTCP.SelectedRows[0].DataBoundItem : null;

                case "tpLogging":
                    return (dgvLogging.SelectedRows.Count > 0) ? (CANStreamer)dgvLogging.SelectedRows[0].DataBoundItem : null;

                case "tpGen":
                    return (dgvGen.SelectedRows.Count > 0) ? (CANStreamer)dgvGen.SelectedRows[0].DataBoundItem : null;

                default:
                    return null;
            }
        }

        public StreamViewForm()
        {
            InitializeComponent();

            // Set up protocol type columns. This is the simplest way to bind to an enum with a dropdown list
            DataGridViewComboBoxColumn incProtocolColumn = (DataGridViewComboBoxColumn)dgvTCP.Columns["incomingProtocolColumn"];
            DataGridViewComboBoxColumn outProtocolColumn = (DataGridViewComboBoxColumn)dgvTCP.Columns["outgoingProtocolColumn"];
            incProtocolColumn.DataSource = Enum.GetValues(typeof(IncomingProtocolEnum));
            outProtocolColumn.DataSource = Enum.GetValues(typeof(OutgoingProtocolEnum));
        }

        private void StreamManagerForm_Load(object sender, EventArgs e)
        {
            logStreams = new FilteredStreamList<CANStreamer_Logfile>(StreamManager.Streams);
            N2000portStreams = new FilteredStreamList<CANStreamer_NGT1_2000>(StreamManager.Streams);
            N0183portStreams = new FilteredStreamList<CANStreamer_N0183>(StreamManager.Streams);
            TCPStreams = new FilteredStreamList<CANStreamer_Network>(StreamManager.Streams);
            LoggerStreams = new FilteredStreamList<CANStreamer_Logger>(StreamManager.Streams);
            GenStreams = new FilteredStreamList<CANStreamer_Generator>(StreamManager.Streams);

            dgvLogfile.DataSource = logStreams;
            dgvPort2000.DataSource = N2000portStreams;
            dgvPort0183.DataSource = N0183portStreams;
            dgvTCP.DataSource = TCPStreams;
            dgvLogging.DataSource = LoggerStreams;
            dgvGen.DataSource = GenStreams;

            string[] portNames = SerialPort.GetPortNames();

            var portColumn = dgvPort2000.Columns["Port"] as DataGridViewComboBoxColumn;
            portColumn.Items.Clear();
            foreach (string portName in portNames)
                portColumn.Items.Add(portName);
            // We add any port names that exist in the current streams
            foreach (var stream in N2000portStreams) {
                if (!portColumn.Items.Contains(stream.PortName)) {
                    portColumn.Items.Add(stream.PortName);
                }
            }

            portColumn = dgvPort0183.Columns["Port0183"] as DataGridViewComboBoxColumn;
            portColumn.Items.Clear();
            foreach (string portName in portNames)
                portColumn.Items.Add(portName);
            // We add any port names that exist in the current streams
            foreach (var stream in N0183portStreams) {
                if (!portColumn.Items.Contains(stream.PortName)) {
                    portColumn.Items.Add(stream.PortName);
                }
            }
        }

        private void dgvLogfile_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Only button column
            if (dgvLogfile.Columns[e.ColumnIndex].Name != "ChangeFile")
                return;

            // Get selected stream
            CANStreamer_Logfile selStream = logStreams[e.RowIndex];

            // Open file dialog
            OpenFileDialog oDialog = new OpenFileDialog();

            if (selStream is CANStreamer_XMLLogfile)
                oDialog.Filter = CANStreamer_XMLLogfile.FileFilter;
            else if (selStream is CANStreamer_KeesLogfile)
                oDialog.Filter = CANStreamer_KeesLogfile.FileFilter;
            else if (selStream is CANStreamer_N0183Logfile)
                oDialog.Filter = CANStreamer_N0183Logfile.FileFilter;
            else if (selStream is CANStreamer_WeatherLogfile)
                oDialog.Filter = CANStreamer_WeatherLogfile.FileFilter;
            else if (selStream is CANStreamer_YDWG02_2000)
                oDialog.Filter = CANStreamer_YDWG02_2000.FileFilter;
            else
                throw new Exception("No file filter defined for type '" + selStream.GetType().Name + "'");

            oDialog.FileName = selStream.FileName ?? "";
            if ((oDialog.ShowDialog() == DialogResult.OK) && (oDialog.CheckFileExists))
            {
                selStream.FileName = oDialog.FileName;

                if (selStream.ConnectionState == ConnectionStateEnum.Connected)
                {
                    selStream.Disconnect();
                    selStream.ConnectStream();
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab.Name == "tpLogfile")
            {
                CANStreamer_Logfile newStream = CANStreamer_Logfile.OpenNew();
                
                if (newStream != null)
                    StreamManager.AddStream(newStream);
            }
            else if (tabControl.SelectedTab.Name == "tpComPort2000")
            {
                StreamManager.AddStream(new CANStreamer_NGT1_2000() { PortName = (SerialPort.GetPortNames())[0], Name = "<New NMEA 2000 Stream>" });
            }
            else if (tabControl.SelectedTab.Name == "tpComPort0183")
            {
                StreamManager.AddStream(new CANStreamer_N0183() { PortName = (SerialPort.GetPortNames())[0], Name = "<New NMEA 0183 Stream>" });
            }
            else if (tabControl.SelectedTab.Name == "tpInternet")
            {
                CANStreamer_Network tcpServer = new CANStreamer_Network();
                AddNetForm addForm = new AddNetForm(tcpServer);
                addForm.ShowDialog();
                if (addForm.resultOK == true)
                {
                    StreamManager.AddStream(tcpServer);
                }
            }
            else if (tabControl.SelectedTab.Name == "tpLogging")
            {
                SaveFileDialog oDialog = new SaveFileDialog();
                oDialog.Filter = "XML Log File (*.xml)|*.xml";
                if (oDialog.ShowDialog() == DialogResult.OK)
                {
                    CANStreamer_Logger newStream = new CANStreamer_Logger() { FileName = oDialog.FileName, Name = Path.GetFileNameWithoutExtension(oDialog.FileName) };
                    StreamManager.AddStream(newStream);
                }
            }
            else if (tabControl.SelectedTab.Name == "tpGen")
            {
                StreamManager.AddStream(new CANStreamer_Generator() { Name = "<New Generator>", MessagePrefix = "$NEW" });
            }

            UpdateButtons();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void UpdateButtons()
        {
            CANStreamer stream = SelectedStream();

            if (stream != null)
            {
                if (stream.ConnectionState == ConnectionStateEnum.Connected)
                    btnConnect.Text = "Disconnect";
                else
                    btnConnect.Text = "Connect";

                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                btnConnect.Enabled = true;
            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnConnect.Enabled = false;
            }

            btnAdd.Enabled = ((tabControl.SelectedTab.Name != "tpComPort2000") && (tabControl.SelectedTab.Name != "tpComPort0183")) || (SerialPort.GetPortNames().Length > 0);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            CANStreamer stream = SelectedStream();

            if (stream.ConnectionState == ConnectionStateEnum.Connected)
                stream.Disconnect();
            else if (stream.ConnectionState == ConnectionStateEnum.Disconnected)
                stream.ConnectStream();

            UpdateButtons();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            CANStreamer stream = SelectedStream();

            if (stream.ConnectionState == ConnectionStateEnum.Connected)
                stream.Disconnect();

            StreamManager.RemoveStream(stream);

            UpdateButtons();
        }

        private void dgvLogfile_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void dgvPort2000_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void dgvPort0183_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void dgvTCP_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
            btnAdd.Enabled = !(((tabControl.SelectedTab.Name == "tpComPort2000") || (tabControl.SelectedTab.Name == "tpComPort0183")) && (SerialPort.GetPortNames().Length == 0));
        }

        private void StreamManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void dgvTCP_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab.Name == "tpInternet")
            {
                if (dgvTCP.SelectedRows.Count > 0)
                    (new AddNetForm((CANStreamer_Network)dgvTCP.SelectedRows[0].DataBoundItem)).ShowDialog();
      
            }
            else
            {
                CANStreamer selStream = SelectedStream();
                if (selStream != null)
                    (new EditLinksForm(selStream)).ShowDialog();
            }   
        }

        private void dgvLogging_FileDialog_FileOk(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
        }

        private void dgvLogging_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            // Only button column
            if (dgvLogging.Columns[e.ColumnIndex].Name != "ChangeLogfile")
                return;

            // Get selected stream
            CANStreamer_Logger selStream = LoggerStreams[e.RowIndex];

            // Open file dialog
            OpenFileDialog oDialog = new OpenFileDialog();
            oDialog.Filter = "XML Log File (*.xml)|*.xml";
            oDialog.FileName = selStream.FileName ?? "";
            oDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.dgvLogging_FileDialog_FileOk);
            oDialog.CheckFileExists = false;
            oDialog.ValidateNames = false;
            if ((oDialog.ShowDialog() == DialogResult.OK)) // && (oDialog.CheckFileExists))
            {
                selStream.FileName = oDialog.FileName;
                if (selStream.ConnectionState == ConnectionStateEnum.Connected)
                {
                    selStream.Disconnect();
                    selStream.ConnectStream();
                }
            }
        }

        private void dgvPort2000_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            // This error can occur when one of the serial ports listed in the system disappears (eg a USB to Serial is removed)
            var x = e;
        }

        private void dgvPort0183_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            var x = e;
        }

    }

    // Produces subset of a stream list where the subset items are of the given type.
    public class FilteredStreamList<T> : BindingList<T> where T : CANStreamer
    {
        public BindingList<CANStreamer> _masterList;

        public FilteredStreamList(BindingList<CANStreamer> masterList)
        {
            _masterList = masterList;
            _masterList.ListChanged += new ListChangedEventHandler(masterListChangedHandler);

            this.ListChanged += new ListChangedEventHandler(ownListChangedHandler);

            masterListChangedHandler(null, null);
        }

        public void ownListChangedHandler(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                _masterList.Add(this.Items[e.NewIndex]);
            }
        }

        public void masterListChangedHandler(object sender, ListChangedEventArgs e)
        {
            // This can be done much more efficiently, by using the eventargs, but speed is not an issue.
            this.Items.Clear();
            foreach (CANStreamer stream in _masterList)
            {
                if (stream is T)
                {
                    this.Items.Add((T)stream);
                }
            }

            // We changed the list, fire the event.
            // TODO This has raised an exception saying that we using a NULL object; could this be a threading issue?
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }
}
