namespace OpenSkipperApplication
{
    partial class PGNExplorerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "123",
            "Desc",
            "Fields"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "456",
            "Desc2",
            "Fields2"}, -1);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPGNDefinitionFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.savePGNDefinitionFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePGNDefinitionFileAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.pGNDefinitionFilePropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPGNToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addFieldToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.deleteFieldsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePGNsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.sortAndValidateDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewPGNs = new System.Windows.Forms.DataGridView();
            this.pGNDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.byteLengthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hasMultipleDefinitionsDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pGNDefnBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.statusStripPGNs = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelPGNCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.FieldDataGridTitle = new System.Windows.Forms.Label();
            this.dataGridViewFields = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bitOffsetDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bitLengthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fieldTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fieldBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.FieldClassTypeListComboBox = new System.Windows.Forms.ComboBox();
            this.PropertyGridTitle = new System.Windows.Forms.Label();
            this.propertyGridField = new System.Windows.Forms.PropertyGrid();
            this.listViewPGNs = new System.Windows.Forms.ListView();
            this.columnHeaderPGN = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDesc = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderFields = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPGNs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pGNDefnBindingSource)).BeginInit();
            this.statusStripPGNs.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fieldBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(725, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openPGNDefinitionFileToolStripMenuItem1,
            this.savePGNDefinitionFileToolStripMenuItem,
            this.savePGNDefinitionFileAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.pGNDefinitionFilePropertiesToolStripMenuItem,
            this.toolStripMenuItem4,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openPGNDefinitionFileToolStripMenuItem1
            // 
            this.openPGNDefinitionFileToolStripMenuItem1.Name = "openPGNDefinitionFileToolStripMenuItem1";
            this.openPGNDefinitionFileToolStripMenuItem1.Size = new System.Drawing.Size(236, 22);
            this.openPGNDefinitionFileToolStripMenuItem1.Text = "Open PGN Definition File...";
            this.openPGNDefinitionFileToolStripMenuItem1.Click += new System.EventHandler(this.openPGNDefinitionFileToolStripMenuItem_Click);
            // 
            // savePGNDefinitionFileToolStripMenuItem
            // 
            this.savePGNDefinitionFileToolStripMenuItem.Name = "savePGNDefinitionFileToolStripMenuItem";
            this.savePGNDefinitionFileToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.savePGNDefinitionFileToolStripMenuItem.Text = "Save PGN Definition File";
            this.savePGNDefinitionFileToolStripMenuItem.Click += new System.EventHandler(this.savePGNDefinitionFileToolStripMenuItem_Click);
            // 
            // savePGNDefinitionFileAsToolStripMenuItem
            // 
            this.savePGNDefinitionFileAsToolStripMenuItem.Name = "savePGNDefinitionFileAsToolStripMenuItem";
            this.savePGNDefinitionFileAsToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.savePGNDefinitionFileAsToolStripMenuItem.Text = "Save PGN Definition File As...";
            this.savePGNDefinitionFileAsToolStripMenuItem.Click += new System.EventHandler(this.savePGNDefinitionFileAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(233, 6);
            // 
            // pGNDefinitionFilePropertiesToolStripMenuItem
            // 
            this.pGNDefinitionFilePropertiesToolStripMenuItem.Name = "pGNDefinitionFilePropertiesToolStripMenuItem";
            this.pGNDefinitionFilePropertiesToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.pGNDefinitionFilePropertiesToolStripMenuItem.Text = "PGN Definition File Properties...";
            this.pGNDefinitionFilePropertiesToolStripMenuItem.Click += new System.EventHandler(this.pGNDefinitionFilePropertiesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(233, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewFieldToolStripMenuItem,
            this.addPGNToolStripMenuItem1,
            this.addFieldToolStripMenuItem,
            this.deleteFieldsToolStripMenuItem,
            this.deletePGNsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.sortAndValidateDataToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // addNewFieldToolStripMenuItem
            // 
            this.addNewFieldToolStripMenuItem.Name = "addNewFieldToolStripMenuItem";
            this.addNewFieldToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.addNewFieldToolStripMenuItem.Text = "Add Field";
            this.addNewFieldToolStripMenuItem.Click += new System.EventHandler(this.addNewFieldToolStripMenuItem_Click);
            // 
            // addPGNToolStripMenuItem1
            // 
            this.addPGNToolStripMenuItem1.Name = "addPGNToolStripMenuItem1";
            this.addPGNToolStripMenuItem1.Size = new System.Drawing.Size(193, 22);
            this.addPGNToolStripMenuItem1.Text = "Add Definition";
            this.addPGNToolStripMenuItem1.Click += new System.EventHandler(this.addPGNToolStripMenuItem1_Click);
            // 
            // addFieldToolStripMenuItem
            // 
            this.addFieldToolStripMenuItem.Name = "addFieldToolStripMenuItem";
            this.addFieldToolStripMenuItem.Size = new System.Drawing.Size(190, 6);
            // 
            // deleteFieldsToolStripMenuItem
            // 
            this.deleteFieldsToolStripMenuItem.Name = "deleteFieldsToolStripMenuItem";
            this.deleteFieldsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteFieldsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.deleteFieldsToolStripMenuItem.Text = "Delete Field";
            this.deleteFieldsToolStripMenuItem.Click += new System.EventHandler(this.deleteFieldsToolStripMenuItem_Click);
            // 
            // deletePGNsToolStripMenuItem
            // 
            this.deletePGNsToolStripMenuItem.Name = "deletePGNsToolStripMenuItem";
            this.deletePGNsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deletePGNsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.deletePGNsToolStripMenuItem.Text = "Delete Definition";
            this.deletePGNsToolStripMenuItem.Click += new System.EventHandler(this.deletePGNsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(190, 6);
            // 
            // sortAndValidateDataToolStripMenuItem
            // 
            this.sortAndValidateDataToolStripMenuItem.Name = "sortAndValidateDataToolStripMenuItem";
            this.sortAndValidateDataToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.sortAndValidateDataToolStripMenuItem.Text = "Sort and Validate Data";
            this.sortAndValidateDataToolStripMenuItem.Click += new System.EventHandler(this.sortAndValidateDataToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewPGNs);
            this.splitContainer1.Panel1.Controls.Add(this.statusStripPGNs);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.listViewPGNs);
            this.splitContainer1.Size = new System.Drawing.Size(725, 430);
            this.splitContainer1.SplitterDistance = 301;
            this.splitContainer1.TabIndex = 1;
            // 
            // dataGridViewPGNs
            // 
            this.dataGridViewPGNs.AllowUserToOrderColumns = true;
            this.dataGridViewPGNs.AutoGenerateColumns = false;
            this.dataGridViewPGNs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPGNs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.pGNDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.byteLengthDataGridViewTextBoxColumn,
            this.hasMultipleDefinitionsDataGridViewCheckBoxColumn});
            this.dataGridViewPGNs.DataSource = this.pGNDefnBindingSource;
            this.dataGridViewPGNs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPGNs.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewPGNs.MultiSelect = false;
            this.dataGridViewPGNs.Name = "dataGridViewPGNs";
            this.dataGridViewPGNs.RowHeadersVisible = false;
            this.dataGridViewPGNs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPGNs.Size = new System.Drawing.Size(301, 408);
            this.dataGridViewPGNs.TabIndex = 0;
            this.dataGridViewPGNs.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPGNs_CellValueChanged);
            this.dataGridViewPGNs.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // pGNDataGridViewTextBoxColumn
            // 
            this.pGNDataGridViewTextBoxColumn.DataPropertyName = "PGN";
            this.pGNDataGridViewTextBoxColumn.HeaderText = "PGN";
            this.pGNDataGridViewTextBoxColumn.Name = "pGNDataGridViewTextBoxColumn";
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            // 
            // byteLengthDataGridViewTextBoxColumn
            // 
            this.byteLengthDataGridViewTextBoxColumn.DataPropertyName = "ByteLength";
            this.byteLengthDataGridViewTextBoxColumn.HeaderText = "ByteLength";
            this.byteLengthDataGridViewTextBoxColumn.Name = "byteLengthDataGridViewTextBoxColumn";
            // 
            // hasMultipleDefinitionsDataGridViewCheckBoxColumn
            // 
            this.hasMultipleDefinitionsDataGridViewCheckBoxColumn.DataPropertyName = "HasMultipleDefinitions";
            this.hasMultipleDefinitionsDataGridViewCheckBoxColumn.HeaderText = "HasMultipleDefinitions";
            this.hasMultipleDefinitionsDataGridViewCheckBoxColumn.Name = "hasMultipleDefinitionsDataGridViewCheckBoxColumn";
            // 
            // pGNDefnBindingSource
            // 
            this.pGNDefnBindingSource.DataSource = typeof(CANDefinitions.PGNDefn);
            // 
            // statusStripPGNs
            // 
            this.statusStripPGNs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelPGNCount});
            this.statusStripPGNs.Location = new System.Drawing.Point(0, 408);
            this.statusStripPGNs.Name = "statusStripPGNs";
            this.statusStripPGNs.Size = new System.Drawing.Size(301, 22);
            this.statusStripPGNs.TabIndex = 1;
            this.statusStripPGNs.Text = "statusStrip1";
            // 
            // toolStripStatusLabelPGNCount
            // 
            this.toolStripStatusLabelPGNCount.Name = "toolStripStatusLabelPGNCount";
            this.toolStripStatusLabelPGNCount.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabelPGNCount.Text = "toolStripStatusLabel1";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.FieldDataGridTitle);
            this.splitContainer2.Panel1.Controls.Add(this.dataGridViewFields);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.FieldClassTypeListComboBox);
            this.splitContainer2.Panel2.Controls.Add(this.PropertyGridTitle);
            this.splitContainer2.Panel2.Controls.Add(this.propertyGridField);
            this.splitContainer2.Size = new System.Drawing.Size(420, 430);
            this.splitContainer2.SplitterDistance = 165;
            this.splitContainer2.TabIndex = 1;
            // 
            // FieldDataGridTitle
            // 
            this.FieldDataGridTitle.AutoSize = true;
            this.FieldDataGridTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FieldDataGridTitle.Location = new System.Drawing.Point(6, 7);
            this.FieldDataGridTitle.Name = "FieldDataGridTitle";
            this.FieldDataGridTitle.Size = new System.Drawing.Size(113, 13);
            this.FieldDataGridTitle.TabIndex = 1;
            this.FieldDataGridTitle.Text = "(No PGN selected)";
            // 
            // dataGridViewFields
            // 
            this.dataGridViewFields.AllowUserToOrderColumns = true;
            this.dataGridViewFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewFields.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewFields.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn1,
            this.descriptionDataGridViewTextBoxColumn1,
            this.bitOffsetDataGridViewTextBoxColumn,
            this.bitLengthDataGridViewTextBoxColumn,
            this.fieldTypeDataGridViewTextBoxColumn});
            this.dataGridViewFields.DataSource = this.fieldBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewFields.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewFields.Location = new System.Drawing.Point(0, 28);
            this.dataGridViewFields.Name = "dataGridViewFields";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewFields.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewFields.RowHeadersVisible = false;
            this.dataGridViewFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewFields.Size = new System.Drawing.Size(419, 137);
            this.dataGridViewFields.TabIndex = 0;
            this.dataGridViewFields.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFields_CellValueChanged);
            this.dataGridViewFields.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFields_CellEndEdit);
            this.dataGridViewFields.SelectionChanged += new System.EventHandler(this.dataGridViewFields_SelectionChanged);
            // 
            // nameDataGridViewTextBoxColumn1
            // 
            this.nameDataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn1.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn1.Name = "nameDataGridViewTextBoxColumn1";
            this.nameDataGridViewTextBoxColumn1.Width = 60;
            // 
            // descriptionDataGridViewTextBoxColumn1
            // 
            this.descriptionDataGridViewTextBoxColumn1.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn1.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn1.Name = "descriptionDataGridViewTextBoxColumn1";
            this.descriptionDataGridViewTextBoxColumn1.Width = 120;
            // 
            // bitOffsetDataGridViewTextBoxColumn
            // 
            this.bitOffsetDataGridViewTextBoxColumn.DataPropertyName = "BitOffset";
            this.bitOffsetDataGridViewTextBoxColumn.HeaderText = "BitOffset";
            this.bitOffsetDataGridViewTextBoxColumn.Name = "bitOffsetDataGridViewTextBoxColumn";
            this.bitOffsetDataGridViewTextBoxColumn.Width = 50;
            // 
            // bitLengthDataGridViewTextBoxColumn
            // 
            this.bitLengthDataGridViewTextBoxColumn.DataPropertyName = "BitLength";
            this.bitLengthDataGridViewTextBoxColumn.HeaderText = "BitLength";
            this.bitLengthDataGridViewTextBoxColumn.Name = "bitLengthDataGridViewTextBoxColumn";
            this.bitLengthDataGridViewTextBoxColumn.Width = 50;
            // 
            // fieldTypeDataGridViewTextBoxColumn
            // 
            this.fieldTypeDataGridViewTextBoxColumn.DataPropertyName = "FieldType";
            this.fieldTypeDataGridViewTextBoxColumn.HeaderText = "FieldType";
            this.fieldTypeDataGridViewTextBoxColumn.Name = "fieldTypeDataGridViewTextBoxColumn";
            this.fieldTypeDataGridViewTextBoxColumn.ReadOnly = true;
            this.fieldTypeDataGridViewTextBoxColumn.Width = 78;
            // 
            // fieldBindingSource
            // 
            this.fieldBindingSource.DataSource = typeof(CANHandler.N2kField);
            // 
            // FieldClassTypeListComboBox
            // 
            this.FieldClassTypeListComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FieldClassTypeListComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.FieldClassTypeListComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.FieldClassTypeListComboBox.DisplayMember = "Name";
            this.FieldClassTypeListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FieldClassTypeListComboBox.FormattingEnabled = true;
            this.FieldClassTypeListComboBox.Location = new System.Drawing.Point(309, 0);
            this.FieldClassTypeListComboBox.MaxDropDownItems = 20;
            this.FieldClassTypeListComboBox.Name = "FieldClassTypeListComboBox";
            this.FieldClassTypeListComboBox.Size = new System.Drawing.Size(104, 21);
            this.FieldClassTypeListComboBox.Sorted = true;
            this.FieldClassTypeListComboBox.TabIndex = 3;
            this.FieldClassTypeListComboBox.SelectionChangeCommitted += new System.EventHandler(this.FieldClassTypeListComboBox_SelectionChangeCommitted);
            // 
            // PropertyGridTitle
            // 
            this.PropertyGridTitle.AutoSize = true;
            this.PropertyGridTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PropertyGridTitle.Location = new System.Drawing.Point(6, 3);
            this.PropertyGridTitle.Name = "PropertyGridTitle";
            this.PropertyGridTitle.Size = new System.Drawing.Size(111, 13);
            this.PropertyGridTitle.TabIndex = 2;
            this.PropertyGridTitle.Text = "(No field selected)";
            // 
            // propertyGridField
            // 
            this.propertyGridField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridField.Location = new System.Drawing.Point(1, 26);
            this.propertyGridField.Name = "propertyGridField";
            this.propertyGridField.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridField.Size = new System.Drawing.Size(419, 225);
            this.propertyGridField.TabIndex = 0;
            this.propertyGridField.ToolbarVisible = false;
            this.propertyGridField.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridField_PropertyValueChanged);
            // 
            // listViewPGNs
            // 
            this.listViewPGNs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPGNs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderPGN,
            this.columnHeaderName,
            this.columnHeaderDesc,
            this.columnHeaderFields});
            this.listViewPGNs.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.listViewPGNs.Location = new System.Drawing.Point(343, 432);
            this.listViewPGNs.MultiSelect = false;
            this.listViewPGNs.Name = "listViewPGNs";
            this.listViewPGNs.Size = new System.Drawing.Size(233, 128);
            this.listViewPGNs.TabIndex = 0;
            this.listViewPGNs.UseCompatibleStateImageBehavior = false;
            this.listViewPGNs.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderPGN
            // 
            this.columnHeaderPGN.Text = "PGN";
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            // 
            // columnHeaderDesc
            // 
            this.columnHeaderDesc.Text = "Description";
            // 
            // columnHeaderFields
            // 
            this.columnHeaderFields.Text = "Fields";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
            // 
            // PGNExplorerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 454);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PGNExplorerForm";
            this.Text = "PGN Explorer";
            this.Load += new System.EventHandler(this.PGNExplorerForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PGNExplorerForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPGNs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pGNDefnBindingSource)).EndInit();
            this.statusStripPGNs.ResumeLayout(false);
            this.statusStripPGNs.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fieldBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePGNDefinitionFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePGNDefinitionFileAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listViewPGNs;
        private System.Windows.Forms.ColumnHeader columnHeaderPGN;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderDesc;
        private System.Windows.Forms.ColumnHeader columnHeaderFields;
        private System.Windows.Forms.DataGridView dataGridViewPGNs;
        private System.Windows.Forms.DataGridViewTextBoxColumn pGNDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn byteLengthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn hasMultipleDefinitionsDataGridViewCheckBoxColumn;
        private System.Windows.Forms.BindingSource pGNDefnBindingSource;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dataGridViewFields;
        private System.Windows.Forms.BindingSource fieldBindingSource;
        private System.Windows.Forms.PropertyGrid propertyGridField;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePGNsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator addFieldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPGNToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addNewFieldToolStripMenuItem;
        private System.Windows.Forms.Label FieldDataGridTitle;
        private System.Windows.Forms.Label PropertyGridTitle;
        private System.Windows.Forms.ToolStripMenuItem pGNDefinitionFilePropertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem openPGNDefinitionFileToolStripMenuItem1;
        private System.Windows.Forms.ComboBox FieldClassTypeListComboBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem sortAndValidateDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteFieldsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStripPGNs;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelPGNCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn bitOffsetDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn bitLengthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fieldTypeDataGridViewTextBoxColumn;
    }
}