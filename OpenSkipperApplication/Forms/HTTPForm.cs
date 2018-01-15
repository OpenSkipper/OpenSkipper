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
using System.Net;
using CANDefinitions;
using CANStreams;
using System.IO;
using Parameters;

namespace OpenSkipperApplication.Forms
{
    public partial class HTTPForm : Form
    {
        private List<Parameter> _selectedParameters;
     //   private Dictionary<Parameter, Parameter> _clonedParameter;

        public HTTPForm()
        {
            InitializeComponent();

            StreamManager.NewStream += new Action<CANStreamer>(StreamManager_NewStream);

       //     _clonedParameter = new Dictionary<Parameter, Parameter> { };
            _selectedParameters = new List<Parameter> { };
            dgvParameters.DataSource = Definitions.ParamCol.ClonedParameters;
        }

        void StreamManager_NewStream(CANStreamer stream)
        {
            foreach (Parameter param in _selectedParameters)
                param.Connect(stream);
        }

        private void dgvParameters_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvParameters.Columns[e.ColumnIndex].Name != "colReport")
                return;

            // Get the clicked row and associated stream
            DataGridViewRow row = dgvParameters.Rows[e.RowIndex];
            Parameter clickedParameter = (Parameter)row.DataBoundItem;

            // Get the clicked cell
            DataGridViewCell cell = row.Cells[e.ColumnIndex];
            bool cellValue = (bool)cell.EditedFormattedValue;

            if (cellValue)
            {
                _selectedParameters.Add(clickedParameter);

                foreach (CANStreamer stream in StreamManager.Streams)
                    clickedParameter.Connect(stream);
            }
            else
            {
                foreach (CANStreamer stream in StreamManager.Streams)
                    clickedParameter.Disconnect(stream);

                _selectedParameters.Remove(clickedParameter);
            }
        }
        private void dgvParameters_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvParameters_CellContentClick(sender, e);
        }

        // HTTP
        private HttpListener httpListener = new HttpListener();
        private void chkHTTP_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHTTP.CheckState == CheckState.Checked)
            {
                if (httpListener == null)
                    httpListener = new HttpListener();

                if (httpListener.Prefixes.Count == 0)
                    httpListener.Prefixes.Add("http://localhost:8080/"); // Works
                    //httpListener.Prefixes.Add("http://130.216.209.84:8080/"); // Doesn't work
                    //httpListener.Prefixes.Add("http://*:8080/"); // Doesn't work

                try
                {
                    httpListener.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to start HTTP listener: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    chkHTTP.CheckState = CheckState.Unchecked;
                    httpListener = new HttpListener();
                    return;
                }

                httpListener.BeginGetContext(httpCallback, null);
            }
            else
            {
                if (httpListener.IsListening)
                    httpListener.Stop();
            }
        }

        private void httpCallback(IAsyncResult ar)
        {
            try
            {
                HttpListenerContext httpContext = httpListener.EndGetContext(ar);
                HttpListenerResponse httpResponse = httpContext.Response;

                // Construct a response.
                string responseString = @"<HTML><HEAD><meta http-equiv=""refresh"" content=""1""></HEAD><BODY>";
                foreach (Parameter param in _selectedParameters)
                    responseString += param.DisplayName + "=" + param.ToString() + "<br />";
                responseString += "</BODY></HTML>";
                byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(responseString);

                // Send the response
                httpResponse.ContentLength64 = responseBytes.Length;

                Stream output = httpResponse.OutputStream;
                output.Write(responseBytes, 0, responseBytes.Length);
                output.Close();

                httpListener.BeginGetContext(httpCallback, null);
            }
            catch
            {
                // Occurs when listener is stopped.
                return;
            }
        }

        private void HTTPForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void HTTPForm_Shown(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dgvRow in dgvParameters.Rows)
            {
                Parameter boundParam = (Parameter)dgvRow.DataBoundItem;

                if (_selectedParameters.Contains(boundParam))
                {
                    DataGridViewCell cell = dgvRow.Cells["colReport"];
                    cell.Value = true;
                }
            }
        }

        private void HTTPForm_Load(object sender, EventArgs e)
        {

        }
    }
}
