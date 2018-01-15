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

namespace OpenSkipperApplication.Forms
{
    public partial class EditLinksForm : Form
    {
        private readonly CANStreamer thisStream;
        private readonly DataGridViewDisableCheckBoxColumn fromColumn;
        private readonly DataGridViewDisableCheckBoxColumn toColumn;

        public EditLinksForm(CANStreamer stream)
        {
            InitializeComponent();

            thisStream = stream;

            if (thisStream.CanWrite)
            {
                // Stream can take input from other streams
                fromColumn = new DataGridViewDisableCheckBoxColumn() { HeaderText = "Take From" };
                dgvLinks.Columns.Add(fromColumn);
            }

            if (thisStream.CanRead)
            {
                // Stream can send output to other streams
                toColumn = new DataGridViewDisableCheckBoxColumn() { HeaderText = "Send To" };
                dgvLinks.Columns.Add(toColumn);
            }

            this.Text = "Editing links for '" + thisStream.Name + "'";

            dgvLinks.RowsAdded += new DataGridViewRowsAddedEventHandler(dgvRouting_RowsAdded);
            dgvLinks.DataSource = StreamManager.Streams;
        }

        private void dgvRouting_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < e.RowCount; i++)
            {
                DataGridViewRow dgvRow = dgvLinks.Rows[i + e.RowIndex];
                CANStreamer otherStream = (CANStreamer)dgvRow.DataBoundItem;

                if (fromColumn != null)
                {
                    DataGridViewDisableCheckBoxCell fromCell = (DataGridViewDisableCheckBoxCell)dgvRow.Cells[fromColumn.Index];
                    fromCell.Value = StreamManager.ConnectionExists(otherStream, thisStream);
                    fromCell.Enabled = (otherStream == thisStream) ? false : otherStream.CanRead;
                }

                if (toColumn != null)
                {
                    DataGridViewDisableCheckBoxCell toCell = (DataGridViewDisableCheckBoxCell)dgvRow.Cells[toColumn.Index];
                    toCell.Value = StreamManager.ConnectionExists(thisStream, otherStream);
                    toCell.Enabled = (otherStream == thisStream) ? false : otherStream.CanWrite;
                }
            }
        }

        private void dgvRouting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewColumn col = dgvLinks.Columns[e.ColumnIndex];
            if ((col != fromColumn) && (col != toColumn))
                return;
            
            // Get the clicked row and associated stream
            DataGridViewRow row = dgvLinks.Rows[e.RowIndex];
            CANStreamer clickedStream = (CANStreamer)row.DataBoundItem;

            // Get the clicked cell
            DataGridViewCell cell = row.Cells[e.ColumnIndex];
            bool cellValue = (bool)cell.EditedFormattedValue;

            // Add/remove connector
            if (col == fromColumn)
                if (cellValue)
                    StreamManager.AddConnector(clickedStream, thisStream);
                else
                    StreamManager.RemoveConnector(clickedStream, thisStream);
            else
                if (cellValue)
                    StreamManager.AddConnector(thisStream, clickedStream);
                else
                    StreamManager.RemoveConnector(thisStream, clickedStream);
        }

        private void dgvRouting_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvRouting_CellContentClick(sender, e);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
