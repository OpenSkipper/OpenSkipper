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
using CANHandler;

namespace OpenSkipperApplication.Forms
{
    public partial class StreamSelecterForm : Form
    {
        public delegate void StreamSelectionHandler(object Sender, EventArgs e, CANStreamer stream, bool selected);
        public event StreamSelectionHandler StreamSelectionChanged;

        public StreamSelecterForm(StreamSelectionHandler handler)
        {
            InitializeComponent();
            dgvStreams.DataSource = StreamManager.Streams;

            if (handler != null)
                StreamSelectionChanged += handler;
        }

        private void StreamSelecter_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Only hide the form
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void dgvStreams_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Only for column with checkbox in
            if (dgvStreams.Columns[e.ColumnIndex].Name != "colDisplay")
                return;

            // Get the clicked row & associated stream
            DataGridViewRow row = dgvStreams.Rows[e.RowIndex];
            CANStreamer stream = (CANStreamer)row.DataBoundItem;

            // Get the clicked cell
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            // Fire event
            if (StreamSelectionChanged != null)
                StreamSelectionChanged(this, null, stream, (bool)cell.EditedFormattedValue);
        }

        private void dgvStreams_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Treat double click as 2 single clicks (The first click of the double-click fired the click event once already)
            dgvStreams_CellContentClick(sender, e);
        }
    }
}
