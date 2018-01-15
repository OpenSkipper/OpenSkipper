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
using CANDefinitions;
using CANHandler;

namespace OpenSkipperApplication.Forms
{
    public partial class AISExplorerForm : Form
    {
        private AISDefnCollection aisDefnCol;
        private bool anyChanges;

        public AISExplorerForm(string fileName)
        {
            InitializeComponent();

            aisDefnCol = AISDefnCollection.LoadFromFile(fileName);
            dgvDefns.DataSource = aisDefnCol.AisDefns;

            if (aisDefnCol.FileName != "")
                this.Text = "AIS Explorer - " + aisDefnCol.FileName;

            saveToolStripMenuItem.Enabled = (aisDefnCol.FileName != "");

            comboTypes.Items.AddRange(AISField.AllFieldTypes());

            anyChanges = false;
        }

        private void dgvDefns_SelectionChanged(object sender, EventArgs e)
        {
            dgvFields.DataSource = (dgvDefns.SelectedRows.Count > 0) ? ((AISDefn)dgvDefns.SelectedRows[0].DataBoundItem).Fields : null;
        }
        private void dgvFields_SelectionChanged(object sender, EventArgs e)
        {
            AISField selField = (dgvFields.SelectedRows.Count > 0) ? (AISField)dgvFields.SelectedRows[0].DataBoundItem : null;
            propField.SelectedObject = selField;

            if (selField != null)
            {
                comboTypes.Enabled = true;
                comboTypes.Text = selField.GetType().Name;
            }
            else
                comboTypes.Enabled = false;
        }

        private void addDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aisDefnCol.AisDefns.Add(new AISDefn() { Description = "<New>" });

            dgvDefns.FirstDisplayedScrollingRowIndex = dgvDefns.RowCount - 1;
            dgvDefns.Rows[dgvDefns.RowCount - 1].Selected = true;
            anyChanges = true;
        }
        private void deleteDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvDefns.SelectedRows.Count > 0)
            {
                aisDefnCol.AisDefns.Remove((AISDefn)dgvDefns.SelectedRows[0].DataBoundItem);
            }
            else
            {
                MessageBox.Show("No definition selected !");
            }
        }
        private void addFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvDefns.SelectedRows.Count > 0)
            {
                ((AISDefn)dgvDefns.SelectedRows[0].DataBoundItem).Fields.Add(new AISUnsigned() { Description = "<New>" });

                dgvFields.FirstDisplayedScrollingRowIndex = dgvFields.RowCount - 1;
                dgvFields.Rows[dgvFields.RowCount - 1].Selected = true;
                anyChanges = true;
            }
            else
            {
                MessageBox.Show("No definition selected !");
            }
        }
        private void deleteFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvFields.SelectedRows.Count > 0)
            {
                ((AISDefn)dgvDefns.SelectedRows[0].DataBoundItem).Fields.Remove((AISField)dgvFields.SelectedRows[0].DataBoundItem);
            }
            else
            {
                MessageBox.Show("No field selected !");
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Title = "Open AIS Definition File";
            fDialog.Filter = Constants.XMLAISDefnFileFilter;
            if ((fDialog.ShowDialog() == DialogResult.OK) && (fDialog.CheckFileExists == true))
            {
                aisDefnCol = AISDefnCollection.LoadFromFile(fDialog.FileName);

                saveToolStripMenuItem.Enabled = true;
                this.Text = "AIS Explorer - " + fDialog.FileName;
                anyChanges = false;
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!aisDefnCol.SaveToFile(aisDefnCol.FileName))
                return;

            anyChanges = false;
            this.Text = "AIS Explorer - " + aisDefnCol.FileName;

            if (MessageBox.Show("Use these definitions immediately?", "Update definitions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Definitions.LoadAISDefns(aisDefnCol.FileName);
            }
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.Filter = Constants.XMLAISDefnFileFilter;
            if (sDialog.ShowDialog() == DialogResult.OK)
            {
                // Set filename and fire normal save.
                aisDefnCol.FileName = sDialog.FileName;
                saveToolStripMenuItem.Enabled = true; // Save as => Save becomes available
                saveToolStripMenuItem_Click(null, null);
            }
        }

        private void AISExplorerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if they want to keep changes
            if (anyChanges == true)
            {
                switch (MessageBox.Show("You have made changes to the definitions. Do you want to save these changes?", "OpenSeas", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Yes:
                        if (aisDefnCol.FileName != "")
                            saveToolStripMenuItem_Click(null, null);
                        else
                            saveAsToolStripMenuItem_Click(null, null);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    default:
                        // User selected 'No'
                        break;
                }
            }
        }

        private void dgvDefns_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            anyChanges = true;
        }
        private void dgvFields_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            anyChanges = true;
        }
        private void propField_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            anyChanges = true;
        }

        private void comboTypes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            BindingList<AISField> Fields = ((AISDefn)dgvDefns.SelectedRows[0].DataBoundItem).Fields;
            int fieldIndex = dgvFields.SelectedRows[0].Index;

            if (Fields[fieldIndex].GetType().Name != ((Type)comboTypes.SelectedItem).Name)
            {
                // Backup properties
                var tempDict = new Dictionary<string, object> { };
                PGNExplorerForm.UpdatePropertyValuesDict(ref tempDict, Fields[fieldIndex]);

                // Change object type
                Fields[fieldIndex] = AISField.CreateNewField((Type)comboTypes.SelectedItem);

                // Reload properties
                PGNExplorerForm.CopyTo(tempDict, Fields[fieldIndex]);

                // Update grids
                propField.SelectedObject = Fields[fieldIndex];
            }
        }
    }
}
