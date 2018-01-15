namespace OpenSkipperApplication.Forms
{
    partial class EditLinksForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditLinksForm));
            this.dgvLinks = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cANStreamerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLinks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLinks
            // 
            this.dgvLinks.AllowUserToAddRows = false;
            this.dgvLinks.AllowUserToDeleteRows = false;
            this.dgvLinks.AllowUserToResizeRows = false;
            this.dgvLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLinks.AutoGenerateColumns = false;
            this.dgvLinks.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvLinks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvLinks.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvLinks.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvLinks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLinks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn});
            this.dgvLinks.DataSource = this.cANStreamerBindingSource;
            this.dgvLinks.Location = new System.Drawing.Point(12, 12);
            this.dgvLinks.Name = "dgvLinks";
            this.dgvLinks.RowHeadersVisible = false;
            this.dgvLinks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLinks.Size = new System.Drawing.Size(435, 320);
            this.dgvLinks.TabIndex = 24;
            this.dgvLinks.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRouting_CellContentClick);
            this.dgvLinks.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRouting_CellContentDoubleClick);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // cANStreamerBindingSource
            // 
            this.cANStreamerBindingSource.DataSource = typeof(CANStreams.CANStreamer);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(372, 338);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 23;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // EditLinksForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 373);
            this.Controls.Add(this.dgvLinks);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditLinksForm";
            this.Text = "EditLinksForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLinks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLinks;
        private System.Windows.Forms.BindingSource cANStreamerBindingSource;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
//        private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;

    }
}