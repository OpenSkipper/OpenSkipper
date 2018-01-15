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
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms.VisualStyles;

namespace OpenSkipperApplication.Forms
{
    public partial class AddNetForm : Form
    {
        public bool resultOK = false;
        public bool initializing = true;

        private CANStreamer_Network _tcpServer;

        public AddNetForm(CANStreamer_Network tcpServer)
        {
            InitializeComponent();

            _tcpServer = tcpServer;

            dgvOwn.DataSource = _tcpServer.OwnTCPClients;
            dgvOther.DataSource = _tcpServer.OtherTCPClients;
            dgvUDP.DataSource = _tcpServer.OwnUDPClients;

            txtName.DataBindings.Add("Text", _tcpServer, "Name");

            // The TCPListenPort can be negative (or zero), indicating we are not currently listening
            txtListenTCP.Text = Math.Abs(tcpServer.TCPListenPort).ToString();   
            chkListenTCP.CheckState = (tcpServer.TCPListenPort > 0) ? CheckState.Checked : CheckState.Unchecked;

            // The UDPListenPort can be negative (or zero), indicating we are not currently listening
            txtListenUDP.Text = Math.Abs(tcpServer.UDPReceivePort).ToString();
            chkListenUDP.CheckState = (tcpServer.UDPReceivePort > 0) ? CheckState.Checked : CheckState.Unchecked;

            txtRemotePort.Text = CANStreamer_Network.DefaultPort.ToString();
            txtUDPPort.Text = CANStreamer_Network.DefaultPort.ToString();
            txtBroadcastIP.Text = CANStreamer_Network.DefaultBroadcastIP;

            txtBroadcastIP.Text = tcpServer.BroadcastIP;
            txtBroadcastPort.Text = tcpServer.BroadcastPort.ToString();
            chkBroadcastListen.Checked = tcpServer.ListeningToBroadcast;
            txtBroadcastIP.Enabled =
                txtBroadcastPort.Enabled =
                !tcpServer.ListeningToBroadcast;

            initializing = false;
        }

        private void chkListen_CheckedChanged(object sender, EventArgs e)
        {
            if (chkListenTCP.CheckState == CheckState.Checked)
            {
                int listenPort = 0;
                if (!int.TryParse(txtListenTCP.Text, out listenPort) || (listenPort <= 0) || (listenPort < IPEndPoint.MinPort) || (listenPort > IPEndPoint.MaxPort))
                {
                    MessageBox.Show("Invalid port");
                    txtListenTCP.Focus();
                    chkListenTCP.CheckState = CheckState.Unchecked;
                    return;
                }

                _tcpServer.TCPListenPort = listenPort;
                txtListenTCP.Enabled = false;
            }
            else
            {
                txtListenTCP.Enabled = true;
                _tcpServer.TCPListenPort = -Math.Abs(_tcpServer.TCPListenPort); // A negative (or zero) port means turn off listening
            }
        }

        private void txtListenTCP_TextChanged(object sender, EventArgs e) {
            // When the _tcpServer is not listening for TCP messages, we can change the TCP port. We remember the new port (if the number is valid).
            // The port is stored as a negative number to show that we are not listening to the TCP port.
            int listenPort = 0;
            if (int.TryParse(txtListenTCP.Text, out listenPort)) {
                if (listenPort > 0) {
                    _tcpServer.TCPListenPort = -listenPort;
                }
            }
        }

        private void btnRemoteConnect_Click(object sender, EventArgs e)
        {
            int port;
            if (!int.TryParse(txtRemotePort.Text, out port))
            {
                MessageBox.Show("Invalid port");
                return;
            }

            _tcpServer.ConnectTo(txtRemoteIP.Text, port);
        }

        private void chkListenUDP_CheckedChanged(object sender, EventArgs e)
        {
            if (chkListenUDP.CheckState == CheckState.Checked)
            {
                int listenPort = 0;
                if (!int.TryParse(txtListenUDP.Text, out listenPort) || (listenPort <= 0)  || (listenPort < IPEndPoint.MinPort) || (listenPort > IPEndPoint.MaxPort))
                {
                    MessageBox.Show("Invalid port");
                    txtListenUDP.Focus();
                    chkListenUDP.CheckState = CheckState.Unchecked;
                    return;
                }

                _tcpServer.UDPReceivePort = int.Parse(txtListenUDP.Text);

                txtListenUDP.Enabled = false;
            }
            else
            {
                txtListenUDP.Enabled = true;
                _tcpServer.UDPReceivePort = -Math.Abs(_tcpServer.UDPReceivePort); // A negative (or zero) port means turn off listening
            }
        }

        private void txtListenUDP_TextChanged(object sender, EventArgs e) {
            // When the _tcpServer is not listening for TCP messages, we can change the TCP port. We remember the new port (if the number is valid).
            // The port is stored as a negative number to show that we are not listening to the TCP port.
            int listenPort = 0;
            if (int.TryParse(txtListenUDP.Text, out listenPort)) {
                if (listenPort > 0) {
                    _tcpServer.UDPReceivePort = -listenPort;
                }
            }
        }

        private void btnAddUDP_Click(object sender, EventArgs e)
        {
            IPAddress ipAddress;
            int port;

            if (cmbIP.SelectedIndex < 0)
            {
                if (!IPAddress.TryParse(cmbIP.Text, out ipAddress))
                {
                    try
                    {
                        IPAddress[] resolvedIPs = Dns.GetHostAddresses(cmbIP.Text);
                        if (resolvedIPs.Length == 0)
                        {
                            MessageBox.Show("Could not resolve hostname '" + cmbIP.Text + "'");
                            return;
                        }

                        ipAddress = resolvedIPs[0]; // Note: Can return IPv6 addresses (in their corresponding format)
                    }
                    catch
                    {
                        MessageBox.Show("Could not resolve hostname '" + cmbIP.Text + "'");
                        return;
                    }
                }
            }
            else
            {
                switch (cmbIP.SelectedIndex)
                {
                    case 0:
                        ipAddress = IPAddress.Parse("127.0.0.1");
                        break;

                    case 1:
                        ipAddress = IPAddress.Parse(CANStreamer_Network.DefaultBroadcastIP);
                        break;

                    default:
                        MessageBox.Show("Invalid IP selection");
                        return;
                }
            }

            if (!int.TryParse(txtUDPPort.Text, out port))
            {
                MessageBox.Show("Invalid port");
                return;
            }

            _tcpServer.ConnectViaUDP(ipAddress, port);
        }

        private void btnLinks_Click(object sender, EventArgs e)
        {
            (new EditLinksForm(_tcpServer)).ShowDialog();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            resultOK = true;
            this.Close();
        }

        private void btnDropOwn_Click(object sender, EventArgs e)
        {
            TCP_Client selClient = (TCP_Client)dgvOwn.Rows[dgvOwn.SelectedRows[0].Index].DataBoundItem;
            _tcpServer.RemoveOwnClient(selClient);
        }
        private void btnDropOther_Click(object sender, EventArgs e)
        {
            TCP_Client selClient = (TCP_Client)dgvOther.Rows[dgvOther.SelectedRows[0].Index].DataBoundItem;
            _tcpServer.RemoveOtherClient(selClient);
        }
        private void btnDropUDP_Click(object sender, EventArgs e)
        {
            UDP_Client selClient = (UDP_Client)dgvUDP.Rows[dgvUDP.SelectedRows[0].Index].DataBoundItem;
            _tcpServer.RemoveOwnClient(selClient);
        }

        private void dgvOwn_SelectionChanged(object sender, EventArgs e)
        {
            btnDropOwn.Enabled = (dgvOwn.SelectedRows.Count > 0);
        }
        private void dgvOther_SelectionChanged(object sender, EventArgs e)
        {
            btnDropOther.Enabled = (dgvOther.SelectedRows.Count > 0);
        }
        private void dgvUDP_SelectionChanged(object sender, EventArgs e)
        {
            btnDropUDP.Enabled = (dgvUDP.SelectedRows.Count > 0);
        }

        private void chkBroadcastListen_CheckedChanged(object sender, EventArgs e)
        {
            if (initializing)
                return;

            if (chkBroadcastListen.Checked)
            {
                IPAddress ipAddress;
                int port;
                if (!IPAddress.TryParse(txtBroadcastIP.Text, out ipAddress))
                {
                    chkBroadcastListen.Checked = false;
                    MessageBox.Show("Invalid IP");
                    return;
                }
                if (ipAddress.GetAddressBytes()[0] < 224 || ipAddress.GetAddressBytes()[0] >= 240)
                {
                    chkBroadcastListen.Checked = false;
                    MessageBox.Show("IP must be in the range 224.0.0.0 to 239.255.255.255");
                    return;
                }
                if (!int.TryParse(txtBroadcastPort.Text, out port))
                {
                    chkBroadcastListen.Checked = false;
                    MessageBox.Show("Invalid port");
                    return;
                }
                if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                {
                    chkBroadcastListen.Checked = false;
                    MessageBox.Show("Port must be in the range " + IPEndPoint.MinPort + " to " + IPEndPoint.MaxPort);
                    return;
                }

                if (_tcpServer.ConnectionState == ConnectionStateEnum.Connected)
                {
                    _tcpServer.StartListeningToBroadcast(ipAddress, port);
                }
                else
                {
                    _tcpServer.BroadcastIP = ipAddress.ToString();
                    _tcpServer.BroadcastPort = port;
                    _tcpServer.ListeningToBroadcast = true;
                }

                txtBroadcastIP.Enabled = false;
                txtBroadcastPort.Enabled = false;
            }
            else
            {
                _tcpServer.StopListeningToBroadcast();

                txtBroadcastIP.Enabled = true;
                txtBroadcastPort.Enabled = true;
            }
        }

        private void AddNetForm_Load(object sender, EventArgs e)
        {

        }


    }

    // Code for DataGridViewDisableCheckBoxColumn / DataGridViewDisableCheckBoxCell classes taken from : http://social.msdn.microsoft.com/Forums/en-US/winformsdatacontrols/thread/988c7e3f-c172-467d-89b7-b80a60b7f24f
    public class DataGridViewDisableCheckBoxColumn : DataGridViewCheckBoxColumn
    {
        public DataGridViewDisableCheckBoxColumn()
        {
            this.CellTemplate = new DataGridViewDisableCheckBoxCell();
        }
    }
    public class DataGridViewDisableCheckBoxCell : DataGridViewCheckBoxCell
    {
        bool enabledValue;

        public bool Enabled
        {
            get
            {
                return enabledValue;
            }
            set
            {
                enabledValue = value;
                this.ReadOnly = !value;
            }
        }

        // Override the Clone method so that the Enabled property is copied.
        public override object Clone()
        {
            DataGridViewDisableCheckBoxCell cell = (DataGridViewDisableCheckBoxCell)base.Clone();
            cell.Enabled = this.Enabled;
            return cell;
        }

        // By default, enable the CheckBox cell.
        public DataGridViewDisableCheckBoxCell()
        {
            this.enabledValue = true;
        }

        protected override void Paint(Graphics graphics,
                                        Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                                        DataGridViewElementStates elementState, object value,
                                        object formattedValue, string errorText,
                                        DataGridViewCellStyle cellStyle,
                                        DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                        DataGridViewPaintParts paintParts)
        {
            // The checkBox cell is disabled, so paint the border,
            // background, and disabled checkBox for the cell.
            if (!this.enabledValue)
            {
                // Draw the cell background, if specified.
                if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
                {
                    Brush cellBackground = new SolidBrush(this.Selected ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                    graphics.FillRectangle(cellBackground, cellBounds);
                    cellBackground.Dispose();
                }

                // Draw the cell borders, if specified.
                if ((paintParts & DataGridViewPaintParts.Border) == DataGridViewPaintParts.Border)
                {
                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
                }

                CheckState checkState = CheckState.Unchecked;

                if (formattedValue != null)
                {
                    if (formattedValue is CheckState)
                    {
                        checkState = (CheckState)formattedValue;
                    }
                    else if (formattedValue is bool)
                    {
                        if ((bool)formattedValue)
                        {
                            checkState = CheckState.Checked;
                        }
                    }
                }

                CheckBoxState state = checkState == CheckState.Checked ? CheckBoxState.CheckedDisabled : CheckBoxState.UncheckedDisabled;

                // Calculate the area in which to draw the checkBox.

                // force to unchecked!!
                Size size = CheckBoxRenderer.GetGlyphSize(graphics, state);
                Point center = new Point(cellBounds.X, cellBounds.Y);
                center.X += (cellBounds.Width - size.Width) / 2;
                center.Y += (cellBounds.Height - size.Height) / 2;

                // Draw the disabled checkBox.
                // We prevent painting of the checkbox if the Width,
                // plus a little padding, is too small.
                if (size.Width + 4 < cellBounds.Width)
                {
                    CheckBoxRenderer.DrawCheckBox(graphics, center, state);
                }
            }
            else
            {
                // The checkBox cell is enabled, so let the base class
                // handle the painting.
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }
    }
}
