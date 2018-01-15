namespace OpenSkipperApplication.Forms
{
    partial class StreamViewForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StreamViewForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.tpComPort2000 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.dgvPort2000 = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Speed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Port = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cANStreamerNGT1BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tpLogfile = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.dgvLogfile = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConnectionState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChangeFile = new System.Windows.Forms.DataGridViewButtonColumn();
            this.LogfileBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpComPort0183 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.dgvPort0183 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Port0183 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tpInternet = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.dgvTCP = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.incomingProtocolColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.outgoingProtocolColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.connectionStateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cANStreamerNetworkBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tpLogging = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvLogging = new System.Windows.Forms.DataGridView();
            this.cANStreamerLoggerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tpGen = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvGen = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Interval = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MessageBody = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cANStreamerGeneratorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnEdit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Overwrite = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ShowOnMenu = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChangeLogfile = new System.Windows.Forms.DataGridViewButtonColumn();
            this.tpComPort2000.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPort2000)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerNGT1BindingSource)).BeginInit();
            this.tpLogfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogfile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LogfileBindingSource)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tpComPort0183.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPort0183)).BeginInit();
            this.tpInternet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTCP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerNetworkBindingSource)).BeginInit();
            this.tpLogging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogging)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerLoggerBindingSource)).BeginInit();
            this.tpGen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerGeneratorBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(677, 366);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(8, 366);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(150, 25);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add New...";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConnect.Enabled = false;
            this.btnConnect.Location = new System.Drawing.Point(301, 366);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(115, 25);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(164, 366);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(131, 25);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // tpComPort2000
            // 
            this.tpComPort2000.Controls.Add(this.label6);
            this.tpComPort2000.Controls.Add(this.dgvPort2000);
            this.tpComPort2000.Location = new System.Drawing.Point(4, 22);
            this.tpComPort2000.Name = "tpComPort2000";
            this.tpComPort2000.Padding = new System.Windows.Forms.Padding(3);
            this.tpComPort2000.Size = new System.Drawing.Size(736, 293);
            this.tpComPort2000.TabIndex = 1;
            this.tpComPort2000.Text = "NMEA 2000 (Actisense NGT-1-USB)";
            this.tpComPort2000.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(631, 26);
            this.label6.TabIndex = 4;
            this.label6.Text = "You can receive (and transmit) NMEA 2000 messages from an NMEA 2000 network using" +
    " an Actisense NGT-1USB (or serial) device\r\nby opening its associated COM port.";
            // 
            // dgvPort2000
            // 
            this.dgvPort2000.AllowUserToAddRows = false;
            this.dgvPort2000.AllowUserToDeleteRows = false;
            this.dgvPort2000.AllowUserToResizeRows = false;
            this.dgvPort2000.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPort2000.AutoGenerateColumns = false;
            this.dgvPort2000.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvPort2000.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvPort2000.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvPort2000.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvPort2000.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPort2000.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn1,
            this.Speed,
            this.Port});
            this.dgvPort2000.DataSource = this.cANStreamerNGT1BindingSource;
            this.dgvPort2000.Location = new System.Drawing.Point(3, 32);
            this.dgvPort2000.MultiSelect = false;
            this.dgvPort2000.Name = "dgvPort2000";
            this.dgvPort2000.RowHeadersVisible = false;
            this.dgvPort2000.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPort2000.Size = new System.Drawing.Size(730, 258);
            this.dgvPort2000.TabIndex = 0;
            this.dgvPort2000.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvPort2000_DataError);
            this.dgvPort2000.SelectionChanged += new System.EventHandler(this.dgvPort2000_SelectionChanged);
            // 
            // nameDataGridViewTextBoxColumn1
            // 
            this.nameDataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn1.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn1.Name = "nameDataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "ConnectionState";
            this.dataGridViewTextBoxColumn1.HeaderText = "State";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // Speed
            // 
            this.Speed.DataPropertyName = "Speed";
            this.Speed.HeaderText = "Speed";
            this.Speed.Name = "Speed";
            // 
            // Port
            // 
            this.Port.DataPropertyName = "PortName";
            this.Port.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Port.HeaderText = "Port";
            this.Port.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4"});
            this.Port.Name = "Port";
            this.Port.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Port.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // cANStreamerNGT1BindingSource
            // 
            this.cANStreamerNGT1BindingSource.DataSource = typeof(CANStreams.CANStreamer_NGT1_2000);
            // 
            // tpLogfile
            // 
            this.tpLogfile.Controls.Add(this.label7);
            this.tpLogfile.Controls.Add(this.dgvLogfile);
            this.tpLogfile.Location = new System.Drawing.Point(4, 22);
            this.tpLogfile.Name = "tpLogfile";
            this.tpLogfile.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogfile.Size = new System.Drawing.Size(736, 293);
            this.tpLogfile.TabIndex = 0;
            this.tpLogfile.Text = "Logfile";
            this.tpLogfile.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(671, 26);
            this.label7.TabIndex = 4;
            this.label7.Text = "Open Skipper can read and replay log files generated by itself, www.KeverSoft.com" +
    "\'s PacketLogger, airmartechnology.com\'s Weathercaster, \r\nor plain text NMEA 0183" +
    " files.\r\n";
            // 
            // dgvLogfile
            // 
            this.dgvLogfile.AllowUserToAddRows = false;
            this.dgvLogfile.AllowUserToDeleteRows = false;
            this.dgvLogfile.AllowUserToResizeRows = false;
            this.dgvLogfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLogfile.AutoGenerateColumns = false;
            this.dgvLogfile.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvLogfile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvLogfile.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvLogfile.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvLogfile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLogfile.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.ConnectionState,
            this.Column1,
            this.ChangeFile});
            this.dgvLogfile.DataSource = this.LogfileBindingSource;
            this.dgvLogfile.Location = new System.Drawing.Point(3, 32);
            this.dgvLogfile.MultiSelect = false;
            this.dgvLogfile.Name = "dgvLogfile";
            this.dgvLogfile.RowHeadersVisible = false;
            this.dgvLogfile.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLogfile.Size = new System.Drawing.Size(730, 258);
            this.dgvLogfile.TabIndex = 0;
            this.dgvLogfile.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLogfile_CellContentClick);
            this.dgvLogfile.SelectionChanged += new System.EventHandler(this.dgvLogfile_SelectionChanged);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.Width = 150;
            // 
            // ConnectionState
            // 
            this.ConnectionState.DataPropertyName = "Status";
            this.ConnectionState.HeaderText = "State";
            this.ConnectionState.Name = "ConnectionState";
            this.ConnectionState.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.DataPropertyName = "FileName";
            this.Column1.HeaderText = "File";
            this.Column1.MinimumWidth = 200;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // ChangeFile
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "...";
            this.ChangeFile.DefaultCellStyle = dataGridViewCellStyle1;
            this.ChangeFile.HeaderText = "";
            this.ChangeFile.MinimumWidth = 30;
            this.ChangeFile.Name = "ChangeFile";
            this.ChangeFile.Text = "Asd";
            this.ChangeFile.Width = 30;
            // 
            // LogfileBindingSource
            // 
            this.LogfileBindingSource.DataSource = typeof(CANStreams.CANStreamer_Logfile);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tpLogfile);
            this.tabControl.Controls.Add(this.tpComPort2000);
            this.tabControl.Controls.Add(this.tpComPort0183);
            this.tabControl.Controls.Add(this.tpInternet);
            this.tabControl.Controls.Add(this.tpLogging);
            this.tabControl.Controls.Add(this.tpGen);
            this.tabControl.Location = new System.Drawing.Point(8, 41);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(20, 3);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(744, 319);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tpComPort0183
            // 
            this.tpComPort0183.Controls.Add(this.label5);
            this.tpComPort0183.Controls.Add(this.dgvPort0183);
            this.tpComPort0183.Location = new System.Drawing.Point(4, 22);
            this.tpComPort0183.Name = "tpComPort0183";
            this.tpComPort0183.Padding = new System.Windows.Forms.Padding(3);
            this.tpComPort0183.Size = new System.Drawing.Size(736, 293);
            this.tpComPort0183.TabIndex = 2;
            this.tpComPort0183.Text = "NMEA 0183 (COM ports)";
            this.tpComPort0183.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(756, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "NMEA 0183 messages (including AIS messages) can be received from (or sent to) an " +
    "NMEA 0183 device by opening the COM port the device is connected to.";
            // 
            // dgvPort0183
            // 
            this.dgvPort0183.AllowUserToAddRows = false;
            this.dgvPort0183.AllowUserToDeleteRows = false;
            this.dgvPort0183.AllowUserToResizeRows = false;
            this.dgvPort0183.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPort0183.AutoGenerateColumns = false;
            this.dgvPort0183.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvPort0183.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvPort0183.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvPort0183.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvPort0183.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPort0183.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.Port0183});
            this.dgvPort0183.DataSource = this.cANStreamerNGT1BindingSource;
            this.dgvPort0183.Location = new System.Drawing.Point(3, 32);
            this.dgvPort0183.MultiSelect = false;
            this.dgvPort0183.Name = "dgvPort0183";
            this.dgvPort0183.RowHeadersVisible = false;
            this.dgvPort0183.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPort0183.Size = new System.Drawing.Size(730, 258);
            this.dgvPort0183.TabIndex = 1;
            this.dgvPort0183.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvPort0183_DataError);
            this.dgvPort0183.SelectionChanged += new System.EventHandler(this.dgvPort0183_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Name";
            this.dataGridViewTextBoxColumn2.HeaderText = "Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "ConnectionState";
            this.dataGridViewTextBoxColumn3.HeaderText = "State";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Speed";
            this.dataGridViewTextBoxColumn4.HeaderText = "Speed";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // Port0183
            // 
            this.Port0183.DataPropertyName = "PortName";
            this.Port0183.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Port0183.HeaderText = "Port";
            this.Port0183.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4"});
            this.Port0183.Name = "Port0183";
            this.Port0183.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Port0183.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // tpInternet
            // 
            this.tpInternet.Controls.Add(this.label4);
            this.tpInternet.Controls.Add(this.dgvTCP);
            this.tpInternet.Location = new System.Drawing.Point(4, 22);
            this.tpInternet.Name = "tpInternet";
            this.tpInternet.Padding = new System.Windows.Forms.Padding(3);
            this.tpInternet.Size = new System.Drawing.Size(736, 293);
            this.tpInternet.TabIndex = 3;
            this.tpInternet.Text = "Internet";
            this.tpInternet.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(669, 26);
            this.label4.TabIndex = 12;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // dgvTCP
            // 
            this.dgvTCP.AllowUserToAddRows = false;
            this.dgvTCP.AllowUserToDeleteRows = false;
            this.dgvTCP.AllowUserToResizeRows = false;
            this.dgvTCP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTCP.AutoGenerateColumns = false;
            this.dgvTCP.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvTCP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvTCP.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvTCP.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvTCP.ColumnHeadersHeight = 18;
            this.dgvTCP.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvTCP.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn3,
            this.incomingProtocolColumn,
            this.outgoingProtocolColumn,
            this.connectionStateDataGridViewTextBoxColumn});
            this.dgvTCP.DataSource = this.cANStreamerNetworkBindingSource;
            this.dgvTCP.Location = new System.Drawing.Point(3, 32);
            this.dgvTCP.MultiSelect = false;
            this.dgvTCP.Name = "dgvTCP";
            this.dgvTCP.RowHeadersVisible = false;
            this.dgvTCP.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTCP.Size = new System.Drawing.Size(730, 258);
            this.dgvTCP.TabIndex = 11;
            this.dgvTCP.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTCP_CellContentClick);
            this.dgvTCP.SelectionChanged += new System.EventHandler(this.dgvTCP_SelectionChanged);
            // 
            // nameDataGridViewTextBoxColumn3
            // 
            this.nameDataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn3.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn3.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn3.Name = "nameDataGridViewTextBoxColumn3";
            // 
            // incomingProtocolColumn
            // 
            this.incomingProtocolColumn.DataPropertyName = "IncomingProtocol";
            this.incomingProtocolColumn.HeaderText = "Incoming";
            this.incomingProtocolColumn.Name = "incomingProtocolColumn";
            this.incomingProtocolColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.incomingProtocolColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // outgoingProtocolColumn
            // 
            this.outgoingProtocolColumn.DataPropertyName = "OutgoingProtocol";
            this.outgoingProtocolColumn.HeaderText = "Outgoing";
            this.outgoingProtocolColumn.Name = "outgoingProtocolColumn";
            this.outgoingProtocolColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.outgoingProtocolColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // connectionStateDataGridViewTextBoxColumn
            // 
            this.connectionStateDataGridViewTextBoxColumn.DataPropertyName = "ConnectionState";
            this.connectionStateDataGridViewTextBoxColumn.HeaderText = "State";
            this.connectionStateDataGridViewTextBoxColumn.Name = "connectionStateDataGridViewTextBoxColumn";
            this.connectionStateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // cANStreamerNetworkBindingSource
            // 
            this.cANStreamerNetworkBindingSource.DataSource = typeof(CANStreams.CANStreamer_Network);
            // 
            // tpLogging
            // 
            this.tpLogging.Controls.Add(this.label3);
            this.tpLogging.Controls.Add(this.dgvLogging);
            this.tpLogging.Location = new System.Drawing.Point(4, 22);
            this.tpLogging.Name = "tpLogging";
            this.tpLogging.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogging.Size = new System.Drawing.Size(736, 293);
            this.tpLogging.TabIndex = 4;
            this.tpLogging.Text = "Logging";
            this.tpLogging.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(520, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Messages arriving on any stream(s) can be written (logged) to an XML file for lat" +
    "er play back in Open Skipper.";
            // 
            // dgvLogging
            // 
            this.dgvLogging.AllowUserToAddRows = false;
            this.dgvLogging.AllowUserToDeleteRows = false;
            this.dgvLogging.AllowUserToResizeRows = false;
            this.dgvLogging.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLogging.AutoGenerateColumns = false;
            this.dgvLogging.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvLogging.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvLogging.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvLogging.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvLogging.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLogging.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.Overwrite,
            this.ShowOnMenu,
            this.dataGridViewTextBoxColumn7,
            this.ChangeLogfile});
            this.dgvLogging.DataSource = this.cANStreamerLoggerBindingSource;
            this.dgvLogging.Location = new System.Drawing.Point(3, 32);
            this.dgvLogging.MultiSelect = false;
            this.dgvLogging.Name = "dgvLogging";
            this.dgvLogging.RowHeadersVisible = false;
            this.dgvLogging.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLogging.Size = new System.Drawing.Size(730, 258);
            this.dgvLogging.TabIndex = 1;
            this.dgvLogging.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLogging_CellContentClick);
            // 
            // cANStreamerLoggerBindingSource
            // 
            this.cANStreamerLoggerBindingSource.DataSource = typeof(CANStreams.CANStreamer_Logger);
            // 
            // tpGen
            // 
            this.tpGen.Controls.Add(this.label2);
            this.tpGen.Controls.Add(this.dgvGen);
            this.tpGen.Location = new System.Drawing.Point(4, 22);
            this.tpGen.Name = "tpGen";
            this.tpGen.Padding = new System.Windows.Forms.Padding(3);
            this.tpGen.Size = new System.Drawing.Size(736, 293);
            this.tpGen.TabIndex = 5;
            this.tpGen.Text = "Generating";
            this.tpGen.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(689, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // dgvGen
            // 
            this.dgvGen.AllowUserToAddRows = false;
            this.dgvGen.AllowUserToDeleteRows = false;
            this.dgvGen.AllowUserToResizeRows = false;
            this.dgvGen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvGen.AutoGenerateColumns = false;
            this.dgvGen.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvGen.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvGen.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvGen.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvGen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGen.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9,
            this.Interval,
            this.dataGridViewTextBoxColumn10,
            this.MessageBody});
            this.dgvGen.DataSource = this.cANStreamerGeneratorBindingSource;
            this.dgvGen.Location = new System.Drawing.Point(3, 32);
            this.dgvGen.MultiSelect = false;
            this.dgvGen.Name = "dgvGen";
            this.dgvGen.RowHeadersVisible = false;
            this.dgvGen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGen.Size = new System.Drawing.Size(730, 258);
            this.dgvGen.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn8.DataPropertyName = "Name";
            this.dataGridViewTextBoxColumn8.HeaderText = "Name";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.DataPropertyName = "ConnectionState";
            this.dataGridViewTextBoxColumn9.HeaderText = "State";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // Interval
            // 
            this.Interval.DataPropertyName = "Interval";
            this.Interval.HeaderText = "Interval";
            this.Interval.Name = "Interval";
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.DataPropertyName = "MessagePrefix";
            this.dataGridViewTextBoxColumn10.HeaderText = "Prefix";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            // 
            // MessageBody
            // 
            this.MessageBody.DataPropertyName = "MessageBody";
            this.MessageBody.HeaderText = "Body";
            this.MessageBody.Name = "MessageBody";
            this.MessageBody.Width = 300;
            // 
            // cANStreamerGeneratorBindingSource
            // 
            this.cANStreamerGeneratorBindingSource.DataSource = typeof(CANStreams.CANStreamer_Generator);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEdit.Enabled = false;
            this.btnEdit.Location = new System.Drawing.Point(422, 366);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(115, 25);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(11, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(644, 32);
            this.label1.TabIndex = 6;
            this.label1.Text = "A stream is a source of messages received from one or more instruments, a log fil" +
    "e or from another computer. It is also possible to send messages to some streams" +
    ".";
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "Name";
            this.dataGridViewTextBoxColumn5.HeaderText = "Name";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 150;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "ConnectionState";
            this.dataGridViewTextBoxColumn6.HeaderText = "State";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // Overwrite
            // 
            this.Overwrite.DataPropertyName = "Overwrite";
            this.Overwrite.HeaderText = "Overwrite";
            this.Overwrite.Name = "Overwrite";
            this.Overwrite.Width = 75;
            // 
            // ShowOnMenu
            // 
            this.ShowOnMenu.DataPropertyName = "ShowOnMenu";
            this.ShowOnMenu.HeaderText = "ShowOnMenu";
            this.ShowOnMenu.Name = "ShowOnMenu";
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn7.DataPropertyName = "FileName";
            this.dataGridViewTextBoxColumn7.HeaderText = "File";
            this.dataGridViewTextBoxColumn7.MinimumWidth = 200;
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // ChangeLogfile
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "...";
            this.ChangeLogfile.DefaultCellStyle = dataGridViewCellStyle2;
            this.ChangeLogfile.HeaderText = "";
            this.ChangeLogfile.MinimumWidth = 30;
            this.ChangeLogfile.Name = "ChangeLogfile";
            this.ChangeLogfile.Text = "Asd";
            this.ChangeLogfile.Width = 30;
            // 
            // StreamViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 397);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabControl);
            this.Name = "StreamViewForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Data Streams";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StreamManagerForm_FormClosing);
            this.Load += new System.EventHandler(this.StreamManagerForm_Load);
            this.tpComPort2000.ResumeLayout(false);
            this.tpComPort2000.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPort2000)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerNGT1BindingSource)).EndInit();
            this.tpLogfile.ResumeLayout(false);
            this.tpLogfile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogfile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LogfileBindingSource)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tpComPort0183.ResumeLayout(false);
            this.tpComPort0183.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPort0183)).EndInit();
            this.tpInternet.ResumeLayout(false);
            this.tpInternet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTCP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerNetworkBindingSource)).EndInit();
            this.tpLogging.ResumeLayout(false);
            this.tpLogging.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogging)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerLoggerBindingSource)).EndInit();
            this.tpGen.ResumeLayout(false);
            this.tpGen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerGeneratorBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.BindingSource LogfileBindingSource;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.BindingSource cANStreamerNGT1BindingSource;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TabPage tpComPort2000;
        private System.Windows.Forms.DataGridView dgvPort2000;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Speed;
        private System.Windows.Forms.DataGridViewComboBoxColumn Port;
        private System.Windows.Forms.TabPage tpLogfile;
        private System.Windows.Forms.DataGridView dgvLogfile;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tpComPort0183;
        private System.Windows.Forms.DataGridView dgvPort0183;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewComboBoxColumn Port0183;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConnectionState;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewButtonColumn ChangeFile;
        private System.Windows.Forms.TabPage tpInternet;
        private System.Windows.Forms.DataGridView dgvTCP;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.TabPage tpLogging;
        private System.Windows.Forms.DataGridView dgvLogging;
        private System.Windows.Forms.BindingSource cANStreamerLoggerBindingSource;
        private System.Windows.Forms.BindingSource cANStreamerNetworkBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewComboBoxColumn incomingProtocolColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn outgoingProtocolColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn connectionStateDataGridViewTextBoxColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tpGen;
        private System.Windows.Forms.DataGridView dgvGen;
        private System.Windows.Forms.BindingSource cANStreamerGeneratorBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Interval;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn MessageBody;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Overwrite;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ShowOnMenu;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewButtonColumn ChangeLogfile;
    }
}