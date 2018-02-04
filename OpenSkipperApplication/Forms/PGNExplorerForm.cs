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

using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CANHandler;
using CANDefinitions;

namespace OpenSkipperApplication
{
    public partial class PGNExplorerForm : Form
    {
        public PGNDefnCollection PGNDefnCol;
        public bool AnyChanges;
        public string FormTitleText;    // The original name of the form to which we append the name of any loaded PGN file

        // We remember values for the properties of the field so we change the fiueld type and retain values
        Dictionary<string, object> FieldPropertyValues = new Dictionary<string, object>();

        // Creator
        public PGNExplorerForm(string fileName)
        {
            InitializeComponent();

            PGNDefnCol = PGNDefnCollection.LoadFromFile(fileName); // Creates our own copy

            FormTitleText = this.Text;
            if (fileName == "")
                this.Text += " - <Internal>";
            else
                this.Text += " - " + fileName;

            // We have a combo-box thatallowsd the type (class) of a field to be changed; populate this with the list of all CAN Field Types
            FieldClassTypeListComboBox.Items.AddRange(N2kField.AllFieldTypes());  // We display the DisplayName field of these
            UpdateGridDisplays(GridViewsUpdateType.RebindPGNs);
            AnyChanges = false;

            savePGNDefinitionFileToolStripMenuItem.Enabled = (fileName != "");
        }

        private void PGNExplorerForm_Load(object sender, EventArgs e) {
            UpdateGridDisplays(GridViewsUpdateType.RebindPGNs);
        }

        // The different ways we can request the screen GridViews and PropertyGrid to be updated.
        public enum GridViewsUpdateType {
            RebindFieldProperties,
            RebindFields, RebindFieldsAndSelectLast,
            RebindPGNs, RebindPGNsAndSelectLast
        };

        // Request a screen update of the GridViews and ProopertyGrid
        private void UpdateGridDisplays(GridViewsUpdateType UpdateType) {
            if (PGNDefnCol == null)
            {
                dataGridViewPGNs.DataSource = null;
                dataGridViewFields.DataSource = null;
                propertyGridField.SelectedObject = null;
                toolStripStatusLabelPGNCount.Text = "";
            }
            if (UpdateType >= GridViewsUpdateType.RebindPGNs) {
                if (PGNDefnCol == null)
                {
                    dataGridViewPGNs.DataSource = null;
                    dataGridViewPGNs.Enabled = false;
                    toolStripStatusLabelPGNCount.Text = "";
                } else {
                    dataGridViewPGNs.DataSource = PGNDefnCol.PGNDefns;
                    dataGridViewPGNs.Enabled = true;
                    toolStripStatusLabelPGNCount.Text = PGNDefnCol.PGNDefns.Count() + " PGNs.";
                    if (UpdateType == GridViewsUpdateType.RebindPGNsAndSelectLast) {
                        dataGridViewPGNs.FirstDisplayedScrollingRowIndex = dataGridViewPGNs.RowCount-1;
                        dataGridViewPGNs.Rows[dataGridViewPGNs.RowCount-1].Selected = true;
                    }
                }
            }
            if (UpdateType >= GridViewsUpdateType.RebindFields) {
                if ((dataGridViewPGNs.DataSource != null) &&
                    (dataGridViewPGNs.SelectedRows.Count == 1) &&
                    (dataGridViewPGNs.SelectedRows.Count <= PGNDefnCol.PGNDefns.Length))
                {
                    PGNDefn pgnDefn = PGNDefnCol.PGNDefns[dataGridViewPGNs.SelectedRows[0].Index];
                    dataGridViewFields.DataSource = pgnDefn.Fields;

                    dataGridViewFields.Enabled = true;
                    dataGridViewFields.Refresh();
                    // dataGridViewFields_SelectionChanged(sender, null);
                    FieldDataGridTitle.Text = "PGN "
                        + pgnDefn.PGN
                        + (((pgnDefn.Name != null && pgnDefn.Name != "") ? " (" + PGNDefnCol.PGNDefns[dataGridViewPGNs.SelectedRows[0].Index].Name + ")" : ""))
                        + " Fields";
                    if (UpdateType == GridViewsUpdateType.RebindFieldsAndSelectLast) {
                        dataGridViewFields.FirstDisplayedScrollingRowIndex = dataGridViewFields.RowCount-1;
                        dataGridViewFields.Rows[dataGridViewFields.RowCount - 1].Selected = true;
                    }

                } else {
                    dataGridViewFields.DataSource = null;
                    dataGridViewFields.Enabled = false;
                    FieldDataGridTitle.Text = "(No PGN selected)";
                    // dataGridViewFields_SelectionChanged(sender, null);
                }
            }
            if (UpdateType >= GridViewsUpdateType.RebindFieldProperties) {
                if ((dataGridViewFields.DataSource != null) && (dataGridViewFields.SelectedRows.Count == 1)) {
                    propertyGridField.SelectedObject = ((N2kField[])dataGridViewFields.DataSource)[dataGridViewFields.SelectedRows[0].Index];
                    propertyGridField.Enabled = true;
                    propertyGridField.Refresh();
                    PropertyGridTitle.Text = "Definition of Field '" + ((N2kField[])dataGridViewFields.DataSource)[dataGridViewFields.SelectedRows[0].Index].Name + "'";
                    FieldClassTypeListComboBox.Enabled = true;
                    FieldClassTypeListComboBox.Text = (((N2kField[])dataGridViewFields.DataSource)[dataGridViewFields.SelectedRows[0].Index]).GetType().Name;
                    // We remember the values of the properties in each PGNField
                    UpdatePropertyValuesDict(ref FieldPropertyValues, propertyGridField.SelectedObject);
                } else {
                    propertyGridField.SelectedObject = null;
                    propertyGridField.Enabled = false;
                    PropertyGridTitle.Text = "(No field selected)";
                    FieldClassTypeListComboBox.Text = "";
                    FieldClassTypeListComboBox.Enabled = false;
                }
            }
        }

        private void openPGNDefinitionFileToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                OpenFileDialog fDialog = new OpenFileDialog();
                fDialog.Title = "Open PGN Definition File";
                fDialog.Filter = Constants.XMLPGNDefnFileFilter;
                // fDialog.InitialDirectory = @"C:\";
                if ((fDialog.ShowDialog() == DialogResult.OK) && (fDialog.CheckFileExists == true))
                    PGNDefnCol = PGNDefnCollection.LoadFromFile(fDialog.FileName);
            }
            catch {
                PGNDefnCol = null;
                throw;
            }
            finally {
                PGNForm2_VisibleChanged(null, null); // force the form title to update
                UpdateGridDisplays(GridViewsUpdateType.RebindPGNs);
            }
        }

        /*        private void importKeesPGNDeinitionFileToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                OpenFileDialog fDialog = new OpenFileDialog();
                fDialog.Title = "Open Kees PGN Definition File";
                fDialog.Filter = Constants.KeesXMLPGNDefnFileFilter;
                // fDialog.InitialDirectory = @"C:\";
                if ((fDialog.ShowDialog() == DialogResult.OK) && (fDialog.CheckFileExists == true)) {
                    using (new WaitCursor()) {
                        using (StreamReader r = new StreamReader(fDialog.FileName)) {
                            // Read in the KEEs PGN Definitions
                            // Deserialising from XML file for creating PGNDefnCollection Class
                            KeesPGNDefnCollection KeesDefinitions;
                            // TextReader reader = new StreamReader(@"D:\Documents\Projects\NMEA 2000 Interfacing\Kees Software\packetlogger_20090806_explain.xml");
                            // Create an instance of the XmlSerializer class.
                            XmlSerializer KeesDefnsSerializer = new XmlSerializer(typeof(KeesPGNDefnCollection));

                            KeesDefinitions = (KeesPGNDefnCollection)KeesDefnsSerializer.Deserialize(r);
                            r.Close();
                            // Console.Out.WriteLine(Collect2.PGNDefns[1].ToString(TestData) + '\n');

                            // Create the PGN Definition
                            PGNDefnCol = new PGNDefnCollection {
                                Version = KeesDefinitions.Version,
                                Date = KeesDefinitions.Date,
                                Comment = KeesDefinitions.Comment,
                                CreatorCode = KeesDefinitions.CreatorCode,
                                PGNDefns = KeesDefinitions.GetPGNInfos(),
                                FileName = fDialog.FileName,
                                FileType = PGNDefnCollection.FileTypeEnum.KeesXMLFile
                            };
                        }
                    }
                }
            }
            catch {
                PGNDefnCol = null;
                throw;
            }
            finally {
                PGNForm2_VisibleChanged(null, null); // force the form title to update
                dataGridViewPGNs.Enabled = PGNDefnCol != null;
            }
        }
        */

        private void PGNForm2_VisibleChanged(object sender, EventArgs e) {
            // Update the title of the form to reflect the name of the loaded PGN definitions
            this.Text = FormTitleText + ((PGNDefnCol == null) ? "" : " - " + PGNDefnCol.FileName);
            //this.dataGridViewPGNs.DataSource = MainForm.PGNDefns.PGNDefns;
            UpdateGridDisplays(GridViewsUpdateType.RebindPGNs);
        }


        private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e) {
            UpdateGridDisplays(GridViewsUpdateType.RebindFields);
            //if ((MainForm.PGNDefns != null) && (MainForm.PGNDefns.PGNDefns != null) &&
            //    (dataGridViewPGNs.SelectedRows.Count == 1) &&
            //    (dataGridViewPGNs.SelectedRows.Count <= MainForm.PGNDefns.PGNDefns.Length)) {
            //    PGNDefn pgnDefn = MainForm.PGNDefns.PGNDefns[dataGridViewPGNs.SelectedRows[0].Index];
            //    dataGridViewFields.DataSource = pgnDefn.Fields;

            //    dataGridViewFields.Enabled = true;
            //    dataGridViewFields.Refresh();
            //    dataGridViewFields_SelectionChanged(sender, null);
            //    FieldDataGridTitle.Text = "PGN "
            //        + pgnDefn.PGN 
            //        + (((pgnDefn.DisplayName != null && pgnDefn.DisplayName!="") ? " (" + MainForm.PGNDefns.PGNDefns[dataGridViewPGNs.SelectedRows[0].Index].DisplayName + ")" : ""))
            //        +" Fields";
            //} else {
            //    dataGridViewFields.DataSource = null;
            //    dataGridViewFields.Enabled = false;
            //    FieldDataGridTitle.Text = "(No PGN selected)";
            //    dataGridViewFields_SelectionChanged(sender, null);
            //}
        }

        private void dataGridViewFields_SelectionChanged(object sender, EventArgs e) {
            UpdateGridDisplays(GridViewsUpdateType.RebindFieldProperties);
            //if ((dataGridViewFields.DataSource != null) && (dataGridViewFields.SelectedRows.Count == 1)) {
            //    propertyGridField.SelectedObject = ((Field[])dataGridViewFields.DataSource)[dataGridViewFields.SelectedRows[0].Index];
            //    propertyGridField.Enabled = true;
            //    propertyGridField.Refresh();
            //    PropertyGridTitle.Text = "Definition of Field '" + ((Field[])dataGridViewFields.DataSource)[dataGridViewFields.SelectedRows[0].Index].DisplayName + "'";
            //    FieldClassTypeListComboBox.Enabled = true;
            //    FieldClassTypeListComboBox.Text = (((Field[])dataGridViewFields.DataSource)[dataGridViewFields.SelectedRows[0].Index]).GetType().DisplayName;
            //    // We remember the values of the properties in each PGNField
            //    UpdatePropertyValuesDict(ref FieldPropertyValues, propertyGridField.SelectedObject);
            //} else {
            //    propertyGridField.SelectedObject = null;
            //    propertyGridField.Enabled = false;
            //    PropertyGridTitle.Text = "(No field selected)";
            //    FieldClassTypeListComboBox.Text = "";
            //    FieldClassTypeListComboBox.Enabled = false;
            //}
        }

        // User has editted a value in the fields grid; redraw the property grid as properties are also listed there
        private void dataGridViewFields_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateGridDisplays(GridViewsUpdateType.RebindFieldProperties);
        }

        private void addNewFieldToolStripMenuItem_Click(object sender, EventArgs e) {
            if (dataGridViewFields.DataSource != null) {
                // Create a new object with one more element (which is new because we are dealing with arrays)
                PGNDefnCol.PGNDefns[dataGridViewPGNs.SelectedRows[0].Index].AddField(new N2kBinaryField() { Name = "New Field" });
                UpdateGridDisplays(GridViewsUpdateType.RebindFieldsAndSelectLast);
                AnyChanges = true;
                // Bind to the new object
                // dataGridViewFields.DataSource = MainForm.PGNDefns.PGNDefns[dataGridViewPGNs.SelectedRows[0].Index].Fields;

                // dataGridViewFields.Refresh();
            }
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            addPGNToolStripMenuItem1.Enabled = PGNDefnCol != null;
            addNewFieldToolStripMenuItem.Enabled = dataGridViewFields.DataSource != null;
            deletePGNsToolStripMenuItem.Enabled = (PGNDefnCol != null) && (dataGridViewPGNs.SelectedRows.Count > 0) && dataGridViewPGNs.Focused;
            deleteFieldsToolStripMenuItem.Enabled = (dataGridViewFields.DataSource != null) && (dataGridViewFields.SelectedRows.Count > 0) && dataGridViewFields.Focused;
        }

        private void pGNDefinitionFilePropertiesToolStripMenuItem_Click(object sender, EventArgs e) {
            PGNDefinitionPropertiesForm frm = new PGNDefinitionPropertiesForm(PGNDefnCol);
            frm.ShowDialog();
        }

        ////////////////////// Tools /////////////////////////

        // This copies property values out of an object and adds them to a dictionary of properties and values
        public static void UpdatePropertyValuesDict(ref Dictionary<string, object> properties, object sourceObject) {
            object value = new object();
            object[] param = new object[0];

            foreach (PropertyInfo propertyInfo in sourceObject.GetType().GetProperties()) {
                if (propertyInfo.CanRead && propertyInfo.CanWrite) {
                    MethodInfo getMethod = propertyInfo.GetGetMethod();
                    // Ensure that there is a get method for the source object. 
                    if (getMethod != null) {
                        // Ensure that there is a set method in the target object. 
                        properties[propertyInfo.Name] = getMethod.Invoke(sourceObject, param);
                    }
                }
            }
        }

        // This copies property values from the given dictionary of proerty/value pairs into an object
        public static void CopyTo(Dictionary<string, object> properties, object targetObject) {
            object value = new object();
            object[] param = new object[0];

            foreach (KeyValuePair<string, object> kvp in properties) {
                // MethodInfo getMethod = propertyInfo.GetGetMethod(); 
                // Ensure that there is a get method for the source object. 
                System.Reflection.PropertyInfo targetPropertyInfo = targetObject.GetType().GetProperty(kvp.Key);
                if (targetPropertyInfo != null) {
                    // Ensure that there is a set method in the target object. 
                    if (targetPropertyInfo.CanWrite) {
                        // value = getMethod.Invoke(sourceObject, param); 
                        targetPropertyInfo.SetValue(targetObject, kvp.Value, null);
                    }
                }
            }
        }

        public static void CopyTo(object sourceObject, object targetObject) {
            // This copies all the matching property values of one object to another
            object value = new object();
            object[] param = new object[0];

            foreach (PropertyInfo propertyInfo in sourceObject.GetType().GetProperties()) {
                MethodInfo getMethod = propertyInfo.GetGetMethod();
                // Ensure that there is a get method for the source object. 
                var targetPropertyInfo = targetObject.GetType().GetProperty(propertyInfo.Name);
                if ((getMethod != null) && (targetPropertyInfo != null)) {
                    // Ensure that there is a set method in the target object. 
                    if (targetPropertyInfo.CanWrite) {
                        value = getMethod.Invoke(sourceObject, param);
                        targetPropertyInfo.SetValue(targetObject, value, null);
                    }
                }
            }
        }

        // This allows the user to change the class of a field
        private void FieldClassTypeListComboBox_SelectionChangeCommitted(object sender, EventArgs e) {
            if (propertyGridField.SelectedObject != null) {
                N2kField f = ((N2kField[])dataGridViewFields.DataSource)[dataGridViewFields.SelectedRows[0].Index];
                if (FieldClassTypeListComboBox.Text != f.GetType().Name) {
                    UpdatePropertyValuesDict(ref FieldPropertyValues, propertyGridField.SelectedObject);
                    ((N2kField[])dataGridViewFields.DataSource)[dataGridViewFields.SelectedRows[0].Index] = N2kField.CreateNewField((Type)FieldClassTypeListComboBox.SelectedItem);
                    CopyTo(FieldPropertyValues, ((N2kField[])dataGridViewFields.DataSource)[dataGridViewFields.SelectedRows[0].Index]);
                    UpdateGridDisplays(GridViewsUpdateType.RebindFields);
                    AnyChanges = true;
                }
            }
        }

        private void sortAndValidateDataToolStripMenuItem_Click(object sender, EventArgs e) {
            string errs = PGNDefnCol.SortAndValidate(false);
            UpdateGridDisplays(GridViewsUpdateType.RebindPGNs);
            MessageBox.Show(errs, "Validation Report", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void deletePGNsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (dataGridViewPGNs.Focused) {
                // We set all selected items to null, and then rebuild the array of PGNDefns
                for (int i = 0; i < dataGridViewPGNs.Rows.Count; i++) {
                    if (dataGridViewPGNs.Rows[i].Selected) {
                        PGNDefnCol.PGNDefns[i] = null;
                    }
                }
                var newPGNDefns = new PGNDefn[PGNDefnCol.PGNDefns.Length - dataGridViewPGNs.SelectedRows.Count];
                int j = 0;
                for (int i = 0; i < PGNDefnCol.PGNDefns.Length; i++)
                {
                    if (PGNDefnCol.PGNDefns[i] != null)
                    {
                        newPGNDefns[j++] = PGNDefnCol.PGNDefns[i];
                    }
                }
                PGNDefnCol.PGNDefns = newPGNDefns;
                UpdateGridDisplays(GridViewsUpdateType.RebindPGNs);
            }
        }

        private void deleteFieldsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (dataGridViewFields.Focused) {
                var fields = (N2kField[])dataGridViewFields.DataSource;
                // We set all selected items to null, and then rebuild the array of Fields
                for (int i = 0; i < dataGridViewFields.Rows.Count; i++) {
                    if (dataGridViewFields.Rows[i].Selected) {
                        fields[i] = null;
                    }
                }
                //foreach (var r in dataGridViewFields.SelectedRows) {
                //    MainForm.PGNDefns.PGNDefns[r.Index] = null;
                //}
                var newFields = new N2kField[fields.Length - dataGridViewFields.SelectedRows.Count];
                int j = 0;
                for (int i = 0; i < fields.Length; i++) {
                    if (fields[i] != null) {
                        newFields[j++] = fields[i];
                    }
                }
                PGNDefnCol.PGNDefns[dataGridViewPGNs.SelectedRows[0].Index].Fields = newFields;
                UpdateGridDisplays(GridViewsUpdateType.RebindFields);
                // Delete a field

            }
        }

        private void addPGNToolStripMenuItem1_Click(object sender, EventArgs e) {
            PGNDefnCol.AddPGNDefn();
            UpdateGridDisplays(GridViewsUpdateType.RebindPGNsAndSelectLast);
        }

        private void PGNExplorerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if they want to keep changes
            if (AnyChanges == true)
            {
                switch (MessageBox.Show("You have made changes to the definitions. Do you want to save these changes?", "OpenSeas", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Yes:
                        if (PGNDefnCol.FileName != "")
                            savePGNDefinitionFileToolStripMenuItem_Click(null, null);
                        else
                            savePGNDefinitionFileAsToolStripMenuItem_Click(null, null);
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

        private void savePGNDefinitionFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PGNDefnCol.SaveToFile(PGNDefnCol.FileName))
                return;

            AnyChanges = false;

            if (MessageBox.Show("Use these definitions immediately?", "Update definitions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Definitions.LoadPGNDefns(PGNDefnCol.FileName);
            }
        }

        private void savePGNDefinitionFileAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.Filter = Constants.XMLPGNDefnFileFilter;
            if (sDialog.ShowDialog() == DialogResult.OK)
            {
                // Set filename and fire normal save.
                PGNDefnCol.FileName = sDialog.FileName;
                savePGNDefinitionFileToolStripMenuItem.Enabled = true; // Save as => Save becomes available
                savePGNDefinitionFileToolStripMenuItem_Click(null, null);
            }
        }

        private void dataGridViewPGNs_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            AnyChanges = true;
        }

        private void dataGridViewFields_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            AnyChanges = true;
        }
        private void propertyGridField_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            AnyChanges = true;
            dataGridViewFields.ResetBindings();
            dataGridViewFields.Refresh();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    } // end of form class defintion

    // define class for showing a wait cursor; this class must occur after the form for the designer to work
    public class WaitCursor : IDisposable
    {
        private static int callCount = 0;
        private static readonly object cursorLocker = new object();

        public WaitCursor()
        {
            lock (cursorLocker)
            {
                callCount++;
                Cursor.Current = Cursors.WaitCursor;
            }
        }
        public void Dispose()
        {
            lock (cursorLocker)
            {
                callCount--;
                if (callCount == 0)
                    Cursor.Current = Cursors.Default;
            }
        }
    }
}
