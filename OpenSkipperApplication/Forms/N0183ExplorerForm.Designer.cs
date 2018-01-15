namespace OpenSkipperApplication
{
    partial class N0183ExplorerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(N0183ExplorerForm));
            this.dgvDefns = new System.Windows.Forms.DataGridView();
            this.codeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.n0183DefnBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.pgField = new System.Windows.Forms.PropertyGrid();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openN0183DefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDefnsAstoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvFields = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.segmentIndexDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FieldType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.n0183FieldBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblFields = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblProperties = new System.Windows.Forms.Label();
            this.comboTypes = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.n0183DefnBindingSource)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.n0183FieldBindingSource)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvDefns
            // 
            this.dgvDefns.AllowUserToOrderColumns = true;
            this.dgvDefns.AllowUserToResizeRows = false;
            this.dgvDefns.AutoGenerateColumns = false;
            this.dgvDefns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDefns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.codeDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn});
            this.dgvDefns.DataSource = this.n0183DefnBindingSource;
            this.dgvDefns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDefns.Location = new System.Drawing.Point(0, 0);
            this.dgvDefns.MultiSelect = false;
            this.dgvDefns.Name = "dgvDefns";
            this.dgvDefns.RowHeadersVisible = false;
            this.dgvDefns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDefns.Size = new System.Drawing.Size(347, 458);
            this.dgvDefns.TabIndex = 0;
            this.dgvDefns.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDefns_CellValueChanged);
            this.dgvDefns.SelectionChanged += new System.EventHandler(this.dgvDefns_SelectionChanged);
            // 
            // codeDataGridViewTextBoxColumn
            // 
            this.codeDataGridViewTextBoxColumn.DataPropertyName = "Code";
            this.codeDataGridViewTextBoxColumn.HeaderText = "Code";
            this.codeDataGridViewTextBoxColumn.Name = "codeDataGridViewTextBoxColumn";
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            // 
            // n0183DefnBindingSource
            // 
            this.n0183DefnBindingSource.DataSource = typeof(CANDefinitions.N0183Defn);
            // 
            // pgField
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.pgField, 2);
            this.pgField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgField.Location = new System.Drawing.Point(3, 33);
            this.pgField.Name = "pgField";
            this.pgField.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.pgField.Size = new System.Drawing.Size(368, 203);
            this.pgField.TabIndex = 2;
            this.pgField.ToolbarVisible = false;
            this.pgField.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgField_PropertyValueChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(725, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openN0183DefinitionsToolStripMenuItem,
            this.saveDefinitionsToolStripMenuItem,
            this.saveDefnsAstoolStripMenuItem,
            this.toolStripSeparator2,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openN0183DefinitionsToolStripMenuItem
            // 
            this.openN0183DefinitionsToolStripMenuItem.Name = "openN0183DefinitionsToolStripMenuItem";
            this.openN0183DefinitionsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.openN0183DefinitionsToolStripMenuItem.Text = "Open Definitions...";
            this.openN0183DefinitionsToolStripMenuItem.Click += new System.EventHandler(this.openN0183DefinitionsToolStripMenuItem_Click);
            // 
            // saveDefinitionsToolStripMenuItem
            // 
            this.saveDefinitionsToolStripMenuItem.Name = "saveDefinitionsToolStripMenuItem";
            this.saveDefinitionsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.saveDefinitionsToolStripMenuItem.Text = "Save Definitions";
            this.saveDefinitionsToolStripMenuItem.Click += new System.EventHandler(this.saveDefinitionsToolStripMenuItem_Click);
            // 
            // saveDefnsAstoolStripMenuItem
            // 
            this.saveDefnsAstoolStripMenuItem.Name = "saveDefnsAstoolStripMenuItem";
            this.saveDefnsAstoolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.saveDefnsAstoolStripMenuItem.Text = "Save Definitions As...";
            this.saveDefnsAstoolStripMenuItem.Click += new System.EventHandler(this.saveDefnsAstoolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(180, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFieldToolStripMenuItem,
            this.addDefinitionToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteFieldToolStripMenuItem,
            this.deleteDefinitionToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // addFieldToolStripMenuItem
            // 
            this.addFieldToolStripMenuItem.Name = "addFieldToolStripMenuItem";
            this.addFieldToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.addFieldToolStripMenuItem.Text = "Add Field";
            this.addFieldToolStripMenuItem.Click += new System.EventHandler(this.addFieldToolStripMenuItem_Click);
            // 
            // addDefinitionToolStripMenuItem
            // 
            this.addDefinitionToolStripMenuItem.Name = "addDefinitionToolStripMenuItem";
            this.addDefinitionToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.addDefinitionToolStripMenuItem.Text = "Add Definition";
            this.addDefinitionToolStripMenuItem.Click += new System.EventHandler(this.addDefinitionToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(159, 6);
            // 
            // deleteFieldToolStripMenuItem
            // 
            this.deleteFieldToolStripMenuItem.Name = "deleteFieldToolStripMenuItem";
            this.deleteFieldToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.deleteFieldToolStripMenuItem.Text = "Delete Field";
            this.deleteFieldToolStripMenuItem.Click += new System.EventHandler(this.deleteFieldToolStripMenuItem_Click);
            // 
            // deleteDefinitionToolStripMenuItem
            // 
            this.deleteDefinitionToolStripMenuItem.Name = "deleteDefinitionToolStripMenuItem";
            this.deleteDefinitionToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.deleteDefinitionToolStripMenuItem.Text = "Delete Definition";
            this.deleteDefinitionToolStripMenuItem.Click += new System.EventHandler(this.deleteDefinitionToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvDefns);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(725, 458);
            this.splitContainer1.SplitterDistance = 347;
            this.splitContainer1.TabIndex = 3;
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
            this.splitContainer2.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.splitContainer2.Size = new System.Drawing.Size(374, 458);
            this.splitContainer2.SplitterDistance = 215;
            this.splitContainer2.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dgvFields, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblFields, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(374, 215);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // dgvFields
            // 
            this.dgvFields.AllowUserToOrderColumns = true;
            this.dgvFields.AllowUserToResizeRows = false;
            this.dgvFields.AutoGenerateColumns = false;
            this.dgvFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn1,
            this.descriptionDataGridViewTextBoxColumn1,
            this.segmentIndexDataGridViewTextBoxColumn,
            this.FieldType});
            this.dgvFields.DataSource = this.n0183FieldBindingSource;
            this.dgvFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFields.Location = new System.Drawing.Point(3, 33);
            this.dgvFields.MultiSelect = false;
            this.dgvFields.Name = "dgvFields";
            this.dgvFields.RowHeadersVisible = false;
            this.dgvFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFields.Size = new System.Drawing.Size(368, 179);
            this.dgvFields.TabIndex = 2;
            this.dgvFields.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFields_CellValueChanged);
            this.dgvFields.SelectionChanged += new System.EventHandler(this.dgvFields_SelectionChanged);
            // 
            // nameDataGridViewTextBoxColumn1
            // 
            this.nameDataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn1.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn1.Name = "nameDataGridViewTextBoxColumn1";
            // 
            // descriptionDataGridViewTextBoxColumn1
            // 
            this.descriptionDataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.descriptionDataGridViewTextBoxColumn1.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn1.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn1.Name = "descriptionDataGridViewTextBoxColumn1";
            // 
            // segmentIndexDataGridViewTextBoxColumn
            // 
            this.segmentIndexDataGridViewTextBoxColumn.DataPropertyName = "SegmentIndex";
            this.segmentIndexDataGridViewTextBoxColumn.HeaderText = "SegmentIndex";
            this.segmentIndexDataGridViewTextBoxColumn.Name = "segmentIndexDataGridViewTextBoxColumn";
            // 
            // FieldType
            // 
            this.FieldType.DataPropertyName = "FieldType";
            this.FieldType.HeaderText = "FieldType";
            this.FieldType.Name = "FieldType";
            this.FieldType.ReadOnly = true;
            // 
            // n0183FieldBindingSource
            // 
            this.n0183FieldBindingSource.DataSource = typeof(CANHandler.N0183Field);
            // 
            // lblFields
            // 
            this.lblFields.AutoSize = true;
            this.lblFields.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblFields.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFields.Location = new System.Drawing.Point(3, 3);
            this.lblFields.Margin = new System.Windows.Forms.Padding(3);
            this.lblFields.Name = "lblFields";
            this.lblFields.Size = new System.Drawing.Size(108, 24);
            this.lblFields.TabIndex = 3;
            this.lblFields.Text = "No Code selected";
            this.lblFields.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel2.Controls.Add(this.pgField, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblProperties, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboTypes, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(374, 239);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // lblProperties
            // 
            this.lblProperties.AutoSize = true;
            this.lblProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProperties.Location = new System.Drawing.Point(3, 3);
            this.lblProperties.Margin = new System.Windows.Forms.Padding(3);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(218, 24);
            this.lblProperties.TabIndex = 3;
            this.lblProperties.Text = "No Field selected";
            this.lblProperties.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboTypes
            // 
            this.comboTypes.DisplayMember = "Name";
            this.comboTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTypes.FormattingEnabled = true;
            this.comboTypes.Location = new System.Drawing.Point(227, 3);
            this.comboTypes.Name = "comboTypes";
            this.comboTypes.Size = new System.Drawing.Size(144, 21);
            this.comboTypes.TabIndex = 4;
            this.comboTypes.SelectionChangeCommitted += new System.EventHandler(this.comboTypes_SelectionChangeCommitted);
            this.comboTypes.SelectedIndexChanged += new System.EventHandler(this.comboTypes_SelectedIndexChanged);
            // 
            // N0183ExplorerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 482);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "N0183ExplorerForm";
            this.Text = "NMEA 0183 Explorer";
            this.Load += new System.EventHandler(this.N0183ExplorerForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.N0183ExplorerForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.n0183DefnBindingSource)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.n0183FieldBindingSource)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDefns;
        private System.Windows.Forms.PropertyGrid pgField;
        private System.Windows.Forms.BindingSource n0183DefnBindingSource;
        private System.Windows.Forms.BindingSource n0183FieldBindingSource;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openN0183DefinitionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveDefinitionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addDefinitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteDefinitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFieldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteFieldToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridViewTextBoxColumn codeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgvFields;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblFields;
        private System.Windows.Forms.Label lblProperties;
        private System.Windows.Forms.ComboBox comboTypes;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn segmentIndexDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldType;
        private System.Windows.Forms.ToolStripMenuItem saveDefnsAstoolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}