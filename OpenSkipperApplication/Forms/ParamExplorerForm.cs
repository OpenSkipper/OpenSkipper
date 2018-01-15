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

using Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CANDefinitions;

namespace OpenSkipperApplication
{
    public partial class ParamExplorerForm : Form
    {
        public ParameterCollection ParamCol;
        public string originalFormText;
        private bool AnyChanges;

        public ParamExplorerForm(string fileName)
        {
            InitializeComponent();

            ParamCol = ParameterCollection.LoadFromFile(fileName);

            originalFormText = this.Text;

            if (fileName != "")
                this.Text += " - " + fileName;

            saveParameterFileToolStripMenuItem.Enabled = (fileName != "");

            gridParameters.DataSource = ParamCol.Parameters;

            // We have a combo-box thats allowed the type (class) of a parameter to be changed
            // populate this with the list of all parameter types
            comboType.Items.AddRange(Parameter.AllParameterTypes());

            AnyChanges = false;
        }

        private void openParamerFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fDialog = new OpenFileDialog();
                fDialog.Title = "Open Parameter File";
                fDialog.Filter = Constants.XMLParameterFileFilter;
                if ((fDialog.ShowDialog() == DialogResult.OK) && (fDialog.CheckFileExists == true))
                {
                    ParamCol = ParameterCollection.LoadFromFile(fDialog.FileName);

                    // Display parameters
                    gridParameters.DataSource = ParamCol.Parameters;

                    // Update menu
                    saveParameterFileToolStripMenuItem.Enabled = true;

                    this.Text = originalFormText + " - " + fDialog.FileName;

                    AnyChanges = false;
                }
            }
            catch
            {
                ParamCol = null;
                throw;
            }
        }

        private void gridParameters_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= ParamCol.Parameters.Length))
                return;

            Parameter selParam = ParamCol.Parameters[e.RowIndex];
            propParam.SelectedObject = selParam;
            comboType.Text = selParam.GetType().Name;
            comboType.Enabled = true;
        }

        private void saveParameterFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ParamCol.SaveToFile(ParamCol.FileName))
                return;

            this.Text = originalFormText + " - " + ParamCol.FileName;
            AnyChanges = false;

            if (MessageBox.Show("Use these definitions immediately?", "Update definitions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Definitions.LoadParameters(ParamCol.FileName);
            }
        }

        private void saveParameterFileAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog fDialog = new SaveFileDialog();
            fDialog.Title = "Save Parameter File";
            fDialog.Filter = Constants.XMLParameterFileFilter;
            fDialog.AddExtension = true;
            if ((fDialog.ShowDialog() == DialogResult.OK))
            {
                ParamCol.FileName = fDialog.FileName;
                saveParameterFileToolStripMenuItem.Enabled = true;
                saveParameterFileToolStripMenuItem_Click(null, null);
            }
        }

        private void addParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // As default we add the simplest parameter, numeric parameter with single source
            // ParamCol.Parameters.Add();
            ParamCol.AddParameter(new MultipleSourceNumeric() { DisplayName = "<New>", InternalName = "<New>" });

            gridParameters.DataSource = null; // Required to refresh correctly. Assumption is that it doesn't realise change as change is in property rather than object itself ?
            gridParameters.DataSource = ParamCol.Parameters;
            gridParameters.Refresh();

            // Select the newly added row
            gridParameters.FirstDisplayedScrollingRowIndex = gridParameters.Rows.Count - 1;
            gridParameters.Rows[gridParameters.Rows.Count - 1].Selected = true;

            // Refresh property grid to reflect new selection
            propParam.SelectedObject = gridParameters.SelectedRows[0].DataBoundItem; // ParamCol.Parameters[gridParameters.SelectedRows[0].Index];
            propParam.Refresh();
        }

        private void comboType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int selIdx = (gridParameters.SelectedRows.Count == 0) ? -1 : gridParameters.SelectedRows[0].Index;
            if (selIdx >= 0 && (selIdx < ParamCol.Parameters.Length))
            {
                if (ParamCol.Parameters[selIdx].GetType().Name != ((Type)comboType.SelectedItem).Name)
                {
                    // Backup properties
                    var tempDict = new Dictionary<string, object> {};
                    PGNExplorerForm.UpdatePropertyValuesDict(ref tempDict, ParamCol.Parameters[selIdx]);

                    // Change object type
                    ParamCol.Parameters[selIdx] = (Parameter)Activator.CreateInstance((Type)comboType.SelectedItem);
                    
                    // Reload properties
                    PGNExplorerForm.CopyTo(tempDict, ParamCol.Parameters[selIdx]);

                    // Refresh
                    gridParameters.Refresh();
                    propParam.SelectedObject = ParamCol.Parameters[selIdx];
                    propParam.Refresh();
                }
            }
        }

        private void deleteParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selIdx = (gridParameters.SelectedRows.Count == 0) ? -1 : gridParameters.SelectedRows[0].Index;
            if (selIdx >= 0 && (selIdx < ParamCol.Parameters.Length))
            {
                // ParamCol.Parameters.RemoveAt(selIdx);
                ParamCol.DeleteParameter(selIdx);

                gridParameters.DataSource = null; // Required to refresh correctly. Assumption is that it doesn't realise change as change is in property rather than object itself ?
                gridParameters.DataSource = ParamCol.Parameters;
                gridParameters.Refresh();

                if (ParamCol.Parameters.Length > 0)
                {
                    propParam.Refresh();
                }
                else
                {
                    propParam.SelectedObject = null;
                    comboType.Enabled = false;
                    comboType.Text = "";
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ParamExplorerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if they want to keep changes
            if (AnyChanges == true)
            {
                switch (MessageBox.Show("You have made changes to the definitions. Do you want to save these changes?", "OpenSeas", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Yes:
                        if (ParamCol.FileName != "")
                            saveParameterFileToolStripMenuItem_Click(null, null);
                        else
                            saveParameterFileAsToolStripMenuItem_Click(null, null);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    default:
                        // User selected 'No'
                        break;
                }
            }

            /*if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }*/
        }

        private void propParam_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propParam.Refresh();
            gridParameters.Refresh();
            AnyChanges = true;
        }

        private void gridParameters_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ParamExplorerForm_Load(object sender, EventArgs e)
        {

        }

        private void gridParameters_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            AnyChanges = true;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
