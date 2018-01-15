namespace OpenSkipperApplication
{
    partial class ParamExplorerForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle100 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle101 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle102 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openParamerFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveParameterFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveParameterFileAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridParameters = new System.Windows.Forms.DataGridView();
            this.displayNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.internalNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParameterType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.paramBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.propParam = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.sourceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paramBindingSource)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sourceBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(681, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openParamerFileToolStripMenuItem,
            this.saveParameterFileToolStripMenuItem,
            this.saveParameterFileAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openParamerFileToolStripMenuItem
            // 
            this.openParamerFileToolStripMenuItem.Name = "openParamerFileToolStripMenuItem";
            this.openParamerFileToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.openParamerFileToolStripMenuItem.Text = "Open Parameter File...";
            this.openParamerFileToolStripMenuItem.Click += new System.EventHandler(this.openParamerFileToolStripMenuItem_Click);
            // 
            // saveParameterFileToolStripMenuItem
            // 
            this.saveParameterFileToolStripMenuItem.Name = "saveParameterFileToolStripMenuItem";
            this.saveParameterFileToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.saveParameterFileToolStripMenuItem.Text = "Save Parameter File...";
            this.saveParameterFileToolStripMenuItem.Click += new System.EventHandler(this.saveParameterFileToolStripMenuItem_Click);
            // 
            // saveParameterFileAsToolStripMenuItem
            // 
            this.saveParameterFileAsToolStripMenuItem.Name = "saveParameterFileAsToolStripMenuItem";
            this.saveParameterFileAsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.saveParameterFileAsToolStripMenuItem.Text = "Save Parameter File As...";
            this.saveParameterFileAsToolStripMenuItem.Click += new System.EventHandler(this.saveParameterFileAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(198, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.exitToolStripMenuItem.Text = "Close";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addParameterToolStripMenuItem,
            this.deleteParameterToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // addParameterToolStripMenuItem
            // 
            this.addParameterToolStripMenuItem.Name = "addParameterToolStripMenuItem";
            this.addParameterToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.addParameterToolStripMenuItem.Text = "Add Parameter...";
            this.addParameterToolStripMenuItem.Click += new System.EventHandler(this.addParameterToolStripMenuItem_Click);
            // 
            // deleteParameterToolStripMenuItem
            // 
            this.deleteParameterToolStripMenuItem.Name = "deleteParameterToolStripMenuItem";
            this.deleteParameterToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.deleteParameterToolStripMenuItem.Text = "Delete Parameter...";
            this.deleteParameterToolStripMenuItem.Click += new System.EventHandler(this.deleteParameterToolStripMenuItem_Click);
            // 
            // gridParameters
            // 
            this.gridParameters.AllowUserToOrderColumns = true;
            this.gridParameters.AutoGenerateColumns = false;
            dataGridViewCellStyle100.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle100.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle100.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle100.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle100.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle100.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle100.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridParameters.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle100;
            this.gridParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.displayNameDataGridViewTextBoxColumn,
            this.internalNameDataGridViewTextBoxColumn,
            this.ParameterType});
            this.gridParameters.DataSource = this.paramBindingSource;
            dataGridViewCellStyle101.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle101.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle101.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle101.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle101.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle101.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle101.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridParameters.DefaultCellStyle = dataGridViewCellStyle101;
            this.gridParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridParameters.Location = new System.Drawing.Point(0, 0);
            this.gridParameters.MultiSelect = false;
            this.gridParameters.Name = "gridParameters";
            dataGridViewCellStyle102.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle102.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle102.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle102.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle102.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle102.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle102.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridParameters.RowHeadersDefaultCellStyle = dataGridViewCellStyle102;
            this.gridParameters.RowHeadersVisible = false;
            this.gridParameters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridParameters.Size = new System.Drawing.Size(326, 442);
            this.gridParameters.TabIndex = 2;
            this.gridParameters.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridParameters_CellContentClick);
            this.gridParameters.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridParameters_CellValueChanged);
            this.gridParameters.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridParameters_RowEnter);
            // 
            // displayNameDataGridViewTextBoxColumn
            // 
            this.displayNameDataGridViewTextBoxColumn.DataPropertyName = "DisplayName";
            this.displayNameDataGridViewTextBoxColumn.HeaderText = "DisplayName";
            this.displayNameDataGridViewTextBoxColumn.Name = "displayNameDataGridViewTextBoxColumn";
            // 
            // internalNameDataGridViewTextBoxColumn
            // 
            this.internalNameDataGridViewTextBoxColumn.DataPropertyName = "InternalName";
            this.internalNameDataGridViewTextBoxColumn.HeaderText = "InternalName";
            this.internalNameDataGridViewTextBoxColumn.Name = "internalNameDataGridViewTextBoxColumn";
            // 
            // ParameterType
            // 
            this.ParameterType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ParameterType.DataPropertyName = "ParameterType";
            this.ParameterType.HeaderText = "ParameterType";
            this.ParameterType.Name = "ParameterType";
            this.ParameterType.ReadOnly = true;
            // 
            // paramBindingSource
            // 
            this.paramBindingSource.DataSource = typeof(Parameters.Parameter);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.propParam, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(351, 442);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // propParam
            // 
            this.propParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propParam.Location = new System.Drawing.Point(0, 30);
            this.propParam.Margin = new System.Windows.Forms.Padding(0);
            this.propParam.Name = "propParam";
            this.propParam.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propParam.Size = new System.Drawing.Size(351, 420);
            this.propParam.TabIndex = 2;
            this.propParam.ToolbarVisible = false;
            this.propParam.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propParam_PropertyValueChanged);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.comboType, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(345, 24);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // comboType
            // 
            this.comboType.DisplayMember = "Name";
            this.comboType.Dock = System.Windows.Forms.DockStyle.Right;
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.Enabled = false;
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(176, 3);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(166, 21);
            this.comboType.TabIndex = 5;
            this.comboType.SelectionChangeCommitted += new System.EventHandler(this.comboType_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Properties of Parameter";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gridParameters);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(681, 442);
            this.splitContainer1.SplitterDistance = 326;
            this.splitContainer1.TabIndex = 3;
            // 
            // sourceBindingSource
            // 
            this.sourceBindingSource.AllowNew = false;
            this.sourceBindingSource.DataSource = typeof(Parameters.MsgHook);
            // 
            // ParamExplorerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 466);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ParamExplorerForm";
            this.Text = "Parameter Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ParamExplorerForm_FormClosing);
            this.Load += new System.EventHandler(this.ParamExplorerForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paramBindingSource)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sourceBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.BindingSource paramBindingSource;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.BindingSource sourceBindingSource;
        private System.Windows.Forms.ToolStripMenuItem openParamerFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveParameterFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveParameterFileAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addParameterToolStripMenuItem;
        private System.Windows.Forms.DataGridView gridParameters;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PropertyGrid propParam;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem deleteParameterToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn displayNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn internalNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterType;
    }
}