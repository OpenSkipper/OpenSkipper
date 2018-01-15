namespace OpenSkipperApplication.Forms
{
    partial class HTTPForm
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
                httpListener.Close();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HTTPForm));
            this.chkHTTP = new System.Windows.Forms.CheckBox();
            this.dgvParameters = new System.Windows.Forms.DataGridView();
            this.colReport = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.InternalName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.parameterBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParameters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.parameterBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // chkHTTP
            // 
            this.chkHTTP.AutoSize = true;
            this.chkHTTP.Location = new System.Drawing.Point(12, 12);
            this.chkHTTP.Name = "chkHTTP";
            this.chkHTTP.Size = new System.Drawing.Size(91, 17);
            this.chkHTTP.TabIndex = 0;
            this.chkHTTP.Text = "Enable HTTP";
            this.chkHTTP.UseVisualStyleBackColor = true;
            this.chkHTTP.CheckedChanged += new System.EventHandler(this.chkHTTP_CheckedChanged);
            // 
            // dgvParameters
            // 
            this.dgvParameters.AllowUserToAddRows = false;
            this.dgvParameters.AllowUserToDeleteRows = false;
            this.dgvParameters.AllowUserToResizeRows = false;
            this.dgvParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvParameters.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvParameters.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvParameters.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvParameters.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colReport,
            this.InternalName,
            this.Column1});
            this.dgvParameters.Location = new System.Drawing.Point(12, 35);
            this.dgvParameters.MultiSelect = false;
            this.dgvParameters.Name = "dgvParameters";
            this.dgvParameters.RowHeadersVisible = false;
            this.dgvParameters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvParameters.Size = new System.Drawing.Size(400, 302);
            this.dgvParameters.TabIndex = 2;
            this.dgvParameters.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParameters_CellContentDoubleClick);
            this.dgvParameters.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParameters_CellContentClick);
            // 
            // colReport
            // 
            this.colReport.HeaderText = "Report";
            this.colReport.Name = "colReport";
            this.colReport.Width = 50;
            // 
            // InternalName
            // 
            this.InternalName.DataPropertyName = "InternalName";
            this.InternalName.HeaderText = "Internal Name";
            this.InternalName.Name = "InternalName";
            this.InternalName.Width = 150;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "Streams";
            this.Column1.Name = "Column1";
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.HeaderText = "Report";
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.ReadOnly = true;
            this.dataGridViewCheckBoxColumn1.Width = 50;
            // 
            // parameterBindingSource
            // 
            this.parameterBindingSource.DataSource = typeof(Parameters.Parameter);
            // 
            // HTTPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 349);
            this.Controls.Add(this.dgvParameters);
            this.Controls.Add(this.chkHTTP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HTTPForm";
            this.Text = "HTTP";
            this.Load += new System.EventHandler(this.HTTPForm_Load);
            this.Shown += new System.EventHandler(this.HTTPForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HTTPForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParameters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.parameterBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkHTTP;
        private System.Windows.Forms.BindingSource parameterBindingSource;
        private System.Windows.Forms.DataGridView dgvParameters;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colReport;
        private System.Windows.Forms.DataGridViewTextBoxColumn InternalName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    }
}