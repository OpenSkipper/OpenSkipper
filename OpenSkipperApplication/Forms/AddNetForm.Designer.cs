namespace OpenSkipperApplication.Forms
{
    partial class AddNetForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddNetForm));
            this.chkListenTCP = new System.Windows.Forms.CheckBox();
            this.txtListenTCP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRemoteIP = new System.Windows.Forms.TextBox();
            this.btnAddTCP = new System.Windows.Forms.Button();
            this.txtRemotePort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDropOther = new System.Windows.Forms.Button();
            this.dgvOther = new System.Windows.Forms.DataGridView();
            this.Hostname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tCPClientBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnLinks = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtListenUDP = new System.Windows.Forms.TextBox();
            this.chkListenUDP = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnDropOwn = new System.Windows.Forms.Button();
            this.dgvOwn = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.portDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnDropUDP = new System.Windows.Forms.Button();
            this.dgvUDP = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uDPClientBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txtUDPPort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnAddUDP = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.chkBroadcastListen = new System.Windows.Forms.CheckBox();
            this.txtBroadcastIP = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtBroadcastPort = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.dataGridViewDisableCheckBoxColumn1 = new OpenSkipperApplication.Forms.DataGridViewDisableCheckBoxColumn();
            this.dataGridViewDisableCheckBoxColumn2 = new OpenSkipperApplication.Forms.DataGridViewDisableCheckBoxColumn();
            this.cANStreamerTCPBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cmbIP = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOther)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tCPClientBindingSource)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOwn)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUDP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uDPClientBindingSource)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerTCPBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // chkListenTCP
            // 
            this.chkListenTCP.AutoSize = true;
            this.chkListenTCP.Location = new System.Drawing.Point(9, 20);
            this.chkListenTCP.Name = "chkListenTCP";
            this.chkListenTCP.Size = new System.Drawing.Size(220, 17);
            this.chkListenTCP.TabIndex = 7;
            this.chkListenTCP.Text = "Accept TCP connection requests on Port";
            this.chkListenTCP.UseVisualStyleBackColor = true;
            this.chkListenTCP.CheckedChanged += new System.EventHandler(this.chkListen_CheckedChanged);
            // 
            // txtListenTCP
            // 
            this.txtListenTCP.Location = new System.Drawing.Point(231, 18);
            this.txtListenTCP.Name = "txtListenTCP";
            this.txtListenTCP.Size = new System.Drawing.Size(65, 20);
            this.txtListenTCP.TabIndex = 8;
            this.txtListenTCP.Text = "5037";
            this.txtListenTCP.TextChanged += new System.EventHandler(this.txtListenTCP_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Name/IP";
            // 
            // txtRemoteIP
            // 
            this.txtRemoteIP.Location = new System.Drawing.Point(65, 20);
            this.txtRemoteIP.Name = "txtRemoteIP";
            this.txtRemoteIP.Size = new System.Drawing.Size(179, 20);
            this.txtRemoteIP.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtRemoteIP, "Enter the name or IP number of the computer you wish to connect to. ");
            // 
            // btnAddTCP
            // 
            this.btnAddTCP.Location = new System.Drawing.Point(350, 20);
            this.btnAddTCP.Name = "btnAddTCP";
            this.btnAddTCP.Size = new System.Drawing.Size(75, 21);
            this.btnAddTCP.TabIndex = 2;
            this.btnAddTCP.Text = "Add";
            this.toolTip1.SetToolTip(this.btnAddTCP, "Add this new computer to the list that Open Skipper makes connections to");
            this.btnAddTCP.UseVisualStyleBackColor = true;
            this.btnAddTCP.Click += new System.EventHandler(this.btnRemoteConnect_Click);
            // 
            // txtRemotePort
            // 
            this.txtRemotePort.Location = new System.Drawing.Point(282, 20);
            this.txtRemotePort.Name = "txtRemotePort";
            this.txtRemotePort.Size = new System.Drawing.Size(47, 20);
            this.txtRemotePort.TabIndex = 1;
            this.txtRemotePort.Text = "5037";
            this.toolTip1.SetToolTip(this.txtRemotePort, "Enter the port to connect on, usually 5037 for Open Skipper");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(250, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Port";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtListenTCP);
            this.groupBox2.Controls.Add(this.btnDropOther);
            this.groupBox2.Controls.Add(this.dgvOther);
            this.groupBox2.Controls.Add(this.chkListenTCP);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(429, 260);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(465, 158);
            this.groupBox2.TabIndex = 39;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "This stream can accept TCP connection requests issued by other computers";
            // 
            // btnDropOther
            // 
            this.btnDropOther.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDropOther.Enabled = false;
            this.btnDropOther.Location = new System.Drawing.Point(363, 125);
            this.btnDropOther.Name = "btnDropOther";
            this.btnDropOther.Size = new System.Drawing.Size(90, 21);
            this.btnDropOther.TabIndex = 47;
            this.btnDropOther.Text = "Disconnect";
            this.btnDropOther.UseVisualStyleBackColor = true;
            this.btnDropOther.Click += new System.EventHandler(this.btnDropOther_Click);
            // 
            // dgvOther
            // 
            this.dgvOther.AllowUserToAddRows = false;
            this.dgvOther.AllowUserToDeleteRows = false;
            this.dgvOther.AllowUserToResizeRows = false;
            this.dgvOther.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOther.AutoGenerateColumns = false;
            this.dgvOther.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvOther.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvOther.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvOther.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvOther.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOther.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Hostname,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.dgvOther.DataSource = this.tCPClientBindingSource;
            this.dgvOther.Location = new System.Drawing.Point(9, 64);
            this.dgvOther.MultiSelect = false;
            this.dgvOther.Name = "dgvOther";
            this.dgvOther.ReadOnly = true;
            this.dgvOther.RowHeadersVisible = false;
            this.dgvOther.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOther.Size = new System.Drawing.Size(444, 61);
            this.dgvOther.TabIndex = 46;
            this.dgvOther.SelectionChanged += new System.EventHandler(this.dgvOther_SelectionChanged);
            // 
            // Hostname
            // 
            this.Hostname.DataPropertyName = "Hostname";
            this.Hostname.HeaderText = "Hostname";
            this.Hostname.Name = "Hostname";
            this.Hostname.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Port";
            this.dataGridViewTextBoxColumn3.HeaderText = "Port";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Status";
            this.dataGridViewTextBoxColumn4.HeaderText = "Status";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // tCPClientBindingSource
            // 
            this.tCPClientBindingSource.DataSource = typeof(CANStreams.TCP_Client);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(301, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Connections have been requested by the following computers:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(747, 424);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(149, 23);
            this.btnOK.TabIndex = 40;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnLinks
            // 
            this.btnLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinks.Location = new System.Drawing.Point(592, 424);
            this.btnLinks.Name = "btnLinks";
            this.btnLinks.Size = new System.Drawing.Size(149, 23);
            this.btnLinks.TabIndex = 41;
            this.btnLinks.Text = "Edit Links";
            this.btnLinks.UseVisualStyleBackColor = true;
            this.btnLinks.Click += new System.EventHandler(this.btnLinks_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "on port";
            // 
            // txtListenUDP
            // 
            this.txtListenUDP.Location = new System.Drawing.Point(154, 18);
            this.txtListenUDP.Name = "txtListenUDP";
            this.txtListenUDP.Size = new System.Drawing.Size(119, 20);
            this.txtListenUDP.TabIndex = 23;
            this.txtListenUDP.Text = "5037";
            this.toolTip1.SetToolTip(this.txtListenUDP, "Enter the port that UDP data is being to this computer on.");
            this.txtListenUDP.TextChanged += new System.EventHandler(this.txtListenUDP_TextChanged);
            // 
            // chkListenUDP
            // 
            this.chkListenUDP.AutoSize = true;
            this.chkListenUDP.Location = new System.Drawing.Point(11, 20);
            this.chkListenUDP.Name = "chkListenUDP";
            this.chkListenUDP.Size = new System.Drawing.Size(146, 17);
            this.chkListenUDP.TabIndex = 22;
            this.chkListenUDP.Text = "Accept data on UDP port";
            this.chkListenUDP.UseVisualStyleBackColor = true;
            this.chkListenUDP.CheckedChanged += new System.EventHandler(this.chkListenUDP_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.btnDropOwn);
            this.groupBox4.Controls.Add(this.dgvOwn);
            this.groupBox4.Controls.Add(this.groupBox1);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Location = new System.Drawing.Point(429, 9);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(465, 238);
            this.groupBox4.TabIndex = 28;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "This stream can make TCP connections to other computers";
            // 
            // btnDropOwn
            // 
            this.btnDropOwn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDropOwn.Enabled = false;
            this.btnDropOwn.Location = new System.Drawing.Point(384, 211);
            this.btnDropOwn.Name = "btnDropOwn";
            this.btnDropOwn.Size = new System.Drawing.Size(75, 21);
            this.btnDropOwn.TabIndex = 48;
            this.btnDropOwn.Text = "Delete";
            this.toolTip1.SetToolTip(this.btnDropOwn, "Delete the selected computer from the list that Open Skipper makes connections to" +
                    ".");
            this.btnDropOwn.UseVisualStyleBackColor = true;
            this.btnDropOwn.Click += new System.EventHandler(this.btnDropOwn_Click);
            // 
            // dgvOwn
            // 
            this.dgvOwn.AllowUserToAddRows = false;
            this.dgvOwn.AllowUserToDeleteRows = false;
            this.dgvOwn.AllowUserToResizeRows = false;
            this.dgvOwn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOwn.AutoGenerateColumns = false;
            this.dgvOwn.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvOwn.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvOwn.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvOwn.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvOwn.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOwn.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn6,
            this.portDataGridViewTextBoxColumn,
            this.statusDataGridViewTextBoxColumn});
            this.dgvOwn.DataSource = this.tCPClientBindingSource;
            this.dgvOwn.Location = new System.Drawing.Point(13, 93);
            this.dgvOwn.MultiSelect = false;
            this.dgvOwn.Name = "dgvOwn";
            this.dgvOwn.ReadOnly = true;
            this.dgvOwn.RowHeadersVisible = false;
            this.dgvOwn.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOwn.Size = new System.Drawing.Size(443, 114);
            this.dgvOwn.TabIndex = 45;
            this.dgvOwn.SelectionChanged += new System.EventHandler(this.dgvOwn_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "Hostname";
            this.dataGridViewTextBoxColumn6.HeaderText = "Name/IP";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // portDataGridViewTextBoxColumn
            // 
            this.portDataGridViewTextBoxColumn.DataPropertyName = "Port";
            this.portDataGridViewTextBoxColumn.HeaderText = "Port";
            this.portDataGridViewTextBoxColumn.Name = "portDataGridViewTextBoxColumn";
            this.portDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtRemoteIP);
            this.groupBox1.Controls.Add(this.txtRemotePort);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnAddTCP);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(13, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(443, 49);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Specify a computer for this stream to connect to:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 75);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(299, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "This stream initiates connections with the following computers:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnDropUDP);
            this.groupBox5.Controls.Add(this.dgvUDP);
            this.groupBox5.Controls.Add(this.groupBox6);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Cursor = System.Windows.Forms.Cursors.Default;
            this.groupBox5.Location = new System.Drawing.Point(12, 131);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(396, 187);
            this.groupBox5.TabIndex = 43;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "This stream can send messages to other computers over UDP";
            // 
            // btnDropUDP
            // 
            this.btnDropUDP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDropUDP.Enabled = false;
            this.btnDropUDP.Location = new System.Drawing.Point(309, 160);
            this.btnDropUDP.Name = "btnDropUDP";
            this.btnDropUDP.Size = new System.Drawing.Size(75, 21);
            this.btnDropUDP.TabIndex = 48;
            this.btnDropUDP.Text = "Delete";
            this.btnDropUDP.UseVisualStyleBackColor = true;
            this.btnDropUDP.Click += new System.EventHandler(this.btnDropUDP_Click);
            // 
            // dgvUDP
            // 
            this.dgvUDP.AllowUserToAddRows = false;
            this.dgvUDP.AllowUserToDeleteRows = false;
            this.dgvUDP.AllowUserToResizeRows = false;
            this.dgvUDP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUDP.AutoGenerateColumns = false;
            this.dgvUDP.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvUDP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvUDP.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvUDP.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvUDP.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUDP.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.dataGridViewTextBoxColumn5});
            this.dgvUDP.DataSource = this.uDPClientBindingSource;
            this.dgvUDP.Location = new System.Drawing.Point(9, 92);
            this.dgvUDP.MultiSelect = false;
            this.dgvUDP.Name = "dgvUDP";
            this.dgvUDP.ReadOnly = true;
            this.dgvUDP.RowHeadersVisible = false;
            this.dgvUDP.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUDP.Size = new System.Drawing.Size(375, 68);
            this.dgvUDP.TabIndex = 47;
            this.dgvUDP.SelectionChanged += new System.EventHandler(this.dgvUDP_SelectionChanged);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.DataPropertyName = "Hostname";
            this.Column1.HeaderText = "Name/IP";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "Port";
            this.dataGridViewTextBoxColumn5.HeaderText = "Port";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 75;
            // 
            // uDPClientBindingSource
            // 
            this.uDPClientBindingSource.DataSource = typeof(CANStreams.UDP_Client);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cmbIP);
            this.groupBox6.Controls.Add(this.txtUDPPort);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.btnAddUDP);
            this.groupBox6.Location = new System.Drawing.Point(9, 19);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(375, 50);
            this.groupBox6.TabIndex = 44;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Specify a computer or a \'multicast\' addresss to send UDP message to:";
            // 
            // txtUDPPort
            // 
            this.txtUDPPort.Location = new System.Drawing.Point(218, 19);
            this.txtUDPPort.Name = "txtUDPPort";
            this.txtUDPPort.Size = new System.Drawing.Size(68, 20);
            this.txtUDPPort.TabIndex = 1;
            this.txtUDPPort.Text = "5037";
            this.toolTip1.SetToolTip(this.txtUDPPort, "Enter the port to use on the destination computer (or the multicast address)");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(191, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 13);
            this.label7.TabIndex = 36;
            this.label7.Text = "Port";
            // 
            // btnAddUDP
            // 
            this.btnAddUDP.Location = new System.Drawing.Point(292, 19);
            this.btnAddUDP.Name = "btnAddUDP";
            this.btnAddUDP.Size = new System.Drawing.Size(75, 21);
            this.btnAddUDP.TabIndex = 2;
            this.btnAddUDP.Text = "Add";
            this.btnAddUDP.UseVisualStyleBackColor = true;
            this.btnAddUDP.Click += new System.EventHandler(this.btnAddUDP_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(321, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "This stream sends messages over UDP to the following computers:";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.chkBroadcastListen);
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Controls.Add(this.txtBroadcastIP);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Controls.Add(this.txtBroadcastPort);
            this.groupBox7.Location = new System.Drawing.Point(12, 379);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(396, 67);
            this.groupBox7.TabIndex = 45;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "This stream can receive UDP data broadcast to many computers";
            // 
            // chkBroadcastListen
            // 
            this.chkBroadcastListen.AutoSize = true;
            this.chkBroadcastListen.Location = new System.Drawing.Point(6, 19);
            this.chkBroadcastListen.Name = "chkBroadcastListen";
            this.chkBroadcastListen.Size = new System.Drawing.Size(246, 17);
            this.chkBroadcastListen.TabIndex = 41;
            this.chkBroadcastListen.Text = "Accept data broadcast to multicast IP address ";
            this.chkBroadcastListen.UseVisualStyleBackColor = true;
            this.chkBroadcastListen.CheckedChanged += new System.EventHandler(this.chkBroadcastListen_CheckedChanged);
            // 
            // txtBroadcastIP
            // 
            this.txtBroadcastIP.Location = new System.Drawing.Point(248, 17);
            this.txtBroadcastIP.Name = "txtBroadcastIP";
            this.txtBroadcastIP.Size = new System.Drawing.Size(136, 20);
            this.txtBroadcastIP.TabIndex = 37;
            this.txtBroadcastIP.Text = "251.39.123.111";
            this.toolTip1.SetToolTip(this.txtBroadcastIP, "Enter a multicast IP number between 224.0.0.0 and 239.255.255.255");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(68, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 39;
            this.label8.Text = "IP";
            // 
            // txtBroadcastPort
            // 
            this.txtBroadcastPort.Location = new System.Drawing.Point(70, 38);
            this.txtBroadcastPort.Name = "txtBroadcastPort";
            this.txtBroadcastPort.Size = new System.Drawing.Size(53, 20);
            this.txtBroadcastPort.TabIndex = 38;
            this.txtBroadcastPort.Text = "5037";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.txtListenUDP);
            this.groupBox8.Controls.Add(this.chkListenUDP);
            this.groupBox8.Location = new System.Drawing.Point(12, 324);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(396, 49);
            this.groupBox8.TabIndex = 46;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "This  stream can receive data sent to this computer over UDP";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(86, 9);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(129, 20);
            this.txtName.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Stream Name";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(15, 35);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(393, 97);
            this.richTextBox1.TabIndex = 47;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // dataGridViewDisableCheckBoxColumn1
            // 
            this.dataGridViewDisableCheckBoxColumn1.HeaderText = "Take From";
            this.dataGridViewDisableCheckBoxColumn1.Name = "dataGridViewDisableCheckBoxColumn1";
            // 
            // dataGridViewDisableCheckBoxColumn2
            // 
            this.dataGridViewDisableCheckBoxColumn2.HeaderText = "Send To";
            this.dataGridViewDisableCheckBoxColumn2.Name = "dataGridViewDisableCheckBoxColumn2";
            // 
            // cANStreamerTCPBindingSource
            // 
            this.cANStreamerTCPBindingSource.DataSource = typeof(CANStreams.TCP_Client);
            // 
            // cmbIP
            // 
            this.cmbIP.FormattingEnabled = true;
            this.cmbIP.Items.AddRange(new object[] {
            "This Computer",
            "Default Multicast"});
            this.cmbIP.Location = new System.Drawing.Point(6, 20);
            this.cmbIP.Name = "cmbIP";
            this.cmbIP.Size = new System.Drawing.Size(179, 21);
            this.cmbIP.TabIndex = 37;
            this.cmbIP.Text = "251.39.123.111";
            // 
            // AddNetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 459);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnLinks);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddNetForm";
            this.Text = "Editing Network Stream";
            this.Load += new System.EventHandler(this.AddNetForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOther)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tCPClientBindingSource)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOwn)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUDP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uDPClientBindingSource)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerTCPBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkListenTCP;
        private System.Windows.Forms.TextBox txtListenTCP;
        private DataGridViewDisableCheckBoxColumn dataGridViewDisableCheckBoxColumn1;
        private DataGridViewDisableCheckBoxColumn dataGridViewDisableCheckBoxColumn2;
        private System.Windows.Forms.BindingSource cANStreamerTCPBindingSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRemoteIP;
        private System.Windows.Forms.Button btnAddTCP;
        private System.Windows.Forms.TextBox txtRemotePort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnLinks;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkListenUDP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtListenUDP;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnAddUDP;
        private System.Windows.Forms.TextBox txtUDPPort;
        private System.Windows.Forms.DataGridView dgvOwn;
        private System.Windows.Forms.BindingSource tCPClientBindingSource;
        private System.Windows.Forms.DataGridView dgvOther;
        private System.Windows.Forms.DataGridView dgvUDP;
        private System.Windows.Forms.BindingSource uDPClientBindingSource;
        private System.Windows.Forms.Button btnDropOther;
        private System.Windows.Forms.Button btnDropOwn;
        private System.Windows.Forms.Button btnDropUDP;
        private System.Windows.Forms.DataGridViewTextBoxColumn Hostname;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox chkBroadcastListen;
        private System.Windows.Forms.TextBox txtBroadcastIP;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtBroadcastPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn portDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.ComboBox cmbIP;
    }
}