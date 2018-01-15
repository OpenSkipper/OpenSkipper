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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CANHandler;
using CANStreams;
using Parameters;
using CANDefinitions;

namespace OpenSkipperApplication
{
    public partial class N0183ExplorerForm : Form
    {
        public N0183DefnCollection N0183DefnCol;
        private string originalFormText;
        private bool AnyChanges;

        public N0183ExplorerForm(string fileName)
        {
            InitializeComponent();
            N0183DefnCol = N0183DefnCollection.LoadFromFile(fileName); // Creates our own copy.
            comboTypes.Items.AddRange(N0183Field.AllFieldTypes());

            originalFormText = this.Text;

            if (fileName != "")
                this.Text = originalFormText + " - " + fileName;

            saveDefinitionsToolStripMenuItem.Enabled = (fileName != "");

            AnyChanges = false;
        }

        private void N0183ExplorerForm_Load(object sender, EventArgs e)
        {
            DefnsChanged();
        }

        public void DefnsChanged()
        {
            if (N0183DefnCol == null)
            {
                dgvDefns.DataSource = null;
                dgvFields.DataSource = null;
                pgField.SelectedObject = null;
            }
            else
            {
                dgvDefns.DataSource = N0183DefnCol.N0183Defns;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void N0183ExplorerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if they want to keep changes
            if (AnyChanges == true)
            {
                switch (MessageBox.Show("You have made changes to the definitions. Do you want to save these changes?", "OpenSeas", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Yes:
                        if (N0183DefnCol.FileName != "")
                            saveDefinitionsToolStripMenuItem_Click(null, null);
                        else
                            saveDefnsAstoolStripMenuItem_Click(null, null);
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

        private void dgvDefns_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDefns.DataSource != null && dgvDefns.SelectedRows.Count == 1)
            {
                N0183Defn defn = (N0183Defn)dgvDefns.SelectedRows[0].DataBoundItem ;//((N0183Defn[])dgvDefns.DataSource)[dgvDefns.SelectedRows[0].Index];
                dgvFields.DataSource = defn.Fields;
                lblFields.Text = "Fields of code '" + defn.Code + "'" + ((defn.Name != "") ? " (" + defn.Name + ")" : "");
            }
            else
            {
                lblFields.Text = "No Code selected";
            }
        }

        private void dgvFields_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFields.DataSource != null && dgvFields.SelectedRows.Count == 1)
            {
                N0183Field field = (N0183Field)dgvFields.SelectedRows[0].DataBoundItem;// ((N0183Field[])dgvFields.DataSource)[dgvFields.SelectedRows[0].Index];
                pgField.SelectedObject = field;
                lblProperties.Text = "Properties of field '" + field.Name + "'";
                comboTypes.Text = field.GetType().Name;
            }
            else
            {
                lblProperties.Text = "No Field selected";
            }
        }

        private void openN0183DefinitionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fDialog = new OpenFileDialog();
                fDialog.Title = "Open PGN Definition File";
                fDialog.Filter = Constants.XMLN0183DefnFileFilter;
                if ((fDialog.ShowDialog() == DialogResult.OK) && (fDialog.CheckFileExists == true))
                {
                    N0183DefnCol = N0183DefnCollection.LoadFromFile(fDialog.FileName);

                    saveDefinitionsToolStripMenuItem.Enabled = true;
                    this.Text = originalFormText + " - " + fDialog.FileName;
                }
            }
            catch
            {
                N0183DefnCol = null;
                throw;
            }
            finally
            {
                DefnsChanged();
            }
        }

        private void comboTypes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dgvFields.DataSource != null && dgvFields.SelectedRows.Count == 1)
            {
                N0183Field[] fields = ((N0183Field[])dgvFields.DataSource);
                int idx = dgvFields.SelectedRows[0].Index;

                if (fields[idx].GetType().Name != ((Type)comboTypes.SelectedItem).Name)
                {
                    // Backup properties
                    var tempDict = new Dictionary<string, object> { };
                    PGNExplorerForm.UpdatePropertyValuesDict(ref tempDict, fields[idx]);

                    // Change object type
                    fields[idx] = (N0183Field)Activator.CreateInstance((Type)comboTypes.SelectedItem);

                    // Reload properties
                    PGNExplorerForm.CopyTo(tempDict, fields[idx]);

                    // Refresh
                    dgvDefns.Refresh();
                    dgvFields.Refresh();
                    pgField.SelectedObject = fields[idx];
                    pgField.Refresh();
                }
            }
        }

        private void pgField_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            dgvFields.Refresh();
            AnyChanges = true;
        }

        private void dgvFields_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            pgField.Refresh();
            AnyChanges = true;
        }

        private void saveDefinitionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!N0183DefnCol.SaveToFile(N0183DefnCol.FileName))
                return;

            this.Text = originalFormText + " - " + N0183DefnCol.FileName;

            if (MessageBox.Show("Use these definitions immediately?", "Update definitions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Definitions.LoadN0183Defns(N0183DefnCol.FileName);
            }
        }

        private void saveDefnsAstoolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.Filter = Constants.XMLN0183DefnFileFilter;
            if (sDialog.ShowDialog() == DialogResult.OK)
            {
                // Set filename and fire normal save.
                N0183DefnCol.FileName = sDialog.FileName;
                saveDefinitionsToolStripMenuItem.Enabled = true; // Save as => Save becomes available
                saveDefinitionsToolStripMenuItem_Click(null, null);
            }
        }

        private void dgvDefns_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            AnyChanges = true;
        }

        private void addDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            N0183DefnCol.AddN0183Defn();
            dgvDefns.DataSource = null;
            dgvDefns.DataSource = N0183DefnCol.N0183Defns;

            dgvDefns.FirstDisplayedScrollingRowIndex = dgvDefns.RowCount - 1;
            dgvDefns.Rows[dgvDefns.RowCount - 1].Selected = true;
            AnyChanges = true;
        }

        private void deleteDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // We set all selected items to null, and then rebuild the array of PGNDefns
            for (int i = 0; i < dgvDefns.Rows.Count; i++)
            {
                if (dgvDefns.Rows[i].Selected)
                {
                    N0183DefnCol.N0183Defns[i] = null;
                }
            }

            var newPGNDefns = new N0183Defn[N0183DefnCol.N0183Defns.Length - dgvDefns.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < N0183DefnCol.N0183Defns.Length; i++)
            {
                if (N0183DefnCol.N0183Defns[i] != null)
                {
                    newPGNDefns[j++] = N0183DefnCol.N0183Defns[i];
                }
            }
            N0183DefnCol.N0183Defns = newPGNDefns;

            dgvDefns.DataSource = null;
            dgvDefns.DataSource = N0183DefnCol.N0183Defns;
        }

        private void addFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvFields.DataSource != null)
            {
                // Create a new object with one more element (which is new because we are dealing with arrays)
                N0183Defn dfn = N0183DefnCol.N0183Defns[dgvDefns.SelectedRows[0].Index];
                dfn.AddField(new N0183TextField() { Name = "New Field" });

                dgvFields.DataSource = null;
                dgvFields.DataSource = dfn.Fields;

                dgvFields.FirstDisplayedScrollingRowIndex = dgvFields.RowCount - 1;
                dgvFields.Rows[dgvFields.RowCount - 1].Selected = true;

                AnyChanges = true;
                // Bind to the new object
                // dataGridViewFields.DataSource = MainForm.PGNDefns.PGNDefns[dataGridViewPGNs.SelectedRows[0].Index].Fields;
                // dataGridViewFields.Refresh();
            }
        }

        private void deleteFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fields = (N0183Field[])dgvFields.DataSource;

            // We set all selected items to null, and then rebuild the array of Fields
            for (int i = 0; i < dgvFields.Rows.Count; i++)
                if (dgvFields.Rows[i].Selected)
                    fields[i] = null;

            var newFields = new N0183Field[fields.Length - dgvFields.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i] != null)
                {
                    newFields[j++] = fields[i];
                }
            }

            N0183Defn dfn = N0183DefnCol.N0183Defns[dgvDefns.SelectedRows[0].Index];
            
            dfn.Fields = newFields;

            dgvFields.DataSource = null;
            dgvFields.DataSource = dfn.Fields;
           
            // Delete a field

        }

        private void filePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void comboTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
