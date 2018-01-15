namespace OpenSkipperApplication.Forms
{
    partial class StreamSelecterForm
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
            this.dgvStreams = new System.Windows.Forms.DataGridView();
            this.btnOK = new System.Windows.Forms.Button();
            this.cANStreamerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConnectionState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisplay = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStreams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvStreams
            // 
            this.dgvStreams.AllowUserToAddRows = false;
            this.dgvStreams.AllowUserToDeleteRows = false;
            this.dgvStreams.AllowUserToResizeColumns = false;
            this.dgvStreams.AllowUserToResizeRows = false;
            this.dgvStreams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStreams.AutoGenerateColumns = false;
            this.dgvStreams.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvStreams.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvStreams.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvStreams.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvStreams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStreams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.Type,
            this.ConnectionState,
            this.colDisplay});
            this.dgvStreams.DataSource = this.cANStreamerBindingSource;
            this.dgvStreams.Location = new System.Drawing.Point(6, 6);
            this.dgvStreams.Name = "dgvStreams";
            this.dgvStreams.RowHeadersVisible = false;
            this.dgvStreams.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStreams.Size = new System.Drawing.Size(388, 282);
            this.dgvStreams.TabIndex = 1;
            this.dgvStreams.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStreams_CellContentDoubleClick);
            this.dgvStreams.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStreams_CellContentClick);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(319, 294);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cANStreamerBindingSource
            // 
            this.cANStreamerBindingSource.DataSource = typeof(CANStreams.CANStreamer);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // Protocol
            // 
            this.Type.DataPropertyName = "Protocol";
            this.Type.HeaderText = "Protocol";
            this.Type.Name = "Protocol";
            this.Type.ReadOnly = true;
            // 
            // ConnectionState
            // 
            this.ConnectionState.DataPropertyName = "ConnectionState";
            this.ConnectionState.HeaderText = "State";
            this.ConnectionState.Name = "ConnectionState";
            this.ConnectionState.ReadOnly = true;
            // 
            // colDisplay
            // 
            this.colDisplay.HeaderText = "";
            this.colDisplay.MinimumWidth = 75;
            this.colDisplay.Name = "colDisplay";
            this.colDisplay.Width = 75;
            // 
            // StreamSelecterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 325);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dgvStreams);
            this.Name = "StreamSelecterForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Select Data Streams...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StreamSelecter_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStreams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvStreams;
        private System.Windows.Forms.BindingSource cANStreamerBindingSource;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConnectionState;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colDisplay;
    }
}