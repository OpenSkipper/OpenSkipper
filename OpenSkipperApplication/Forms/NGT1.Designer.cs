namespace OpenSkipperApplication {
    partial class NGT1 {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NGT1));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripCBPort = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripCBBaud = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripCBMode = new System.Windows.Forms.ToolStripComboBox();
            this.timerClose = new System.Windows.Forms.Timer(this.components);
            this.timerEventBytesAvg = new System.Windows.Forms.Timer(this.components);
            this.valueEventBytesAvg = new KaveExtControls.ValueBox();
            this.labelEventBytesAvg = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.DtrEnable = true;
            this.serialPort1.PortName = "COM4";
            this.serialPort1.RtsEnable = true;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(-2, 32);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(930, 396);
            this.textBox1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOpen,
            this.toolStripButtonClose,
            this.toolStripCBPort,
            this.toolStripCBBaud,
            this.toolStripCBMode});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(928, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonOpen.Enabled = false;
            this.toolStripButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpen.Image")));
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(40, 22);
            this.toolStripButtonOpen.Text = "Open";
            this.toolStripButtonOpen.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButtonClose
            // 
            this.toolStripButtonClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonClose.Enabled = false;
            this.toolStripButtonClose.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClose.Image")));
            this.toolStripButtonClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClose.Name = "toolStripButtonClose";
            this.toolStripButtonClose.Size = new System.Drawing.Size(40, 22);
            this.toolStripButtonClose.Text = "Close";
            this.toolStripButtonClose.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripCBPort
            // 
            this.toolStripCBPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripCBPort.Name = "toolStripCBPort";
            this.toolStripCBPort.Size = new System.Drawing.Size(80, 25);
            this.toolStripCBPort.ToolTipText = "Communication port";
            this.toolStripCBPort.DropDown += new System.EventHandler(this.toolStripCBPort_DropDown);
            this.toolStripCBPort.SelectedIndexChanged += new System.EventHandler(this.toolStripCBPort_SelectedIndexChanged);
            // 
            // toolStripCBBaud
            // 
            this.toolStripCBBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripCBBaud.Items.AddRange(new object[] {
            "4800",
            "38400",
            "115200"});
            this.toolStripCBBaud.Name = "toolStripCBBaud";
            this.toolStripCBBaud.Size = new System.Drawing.Size(80, 25);
            this.toolStripCBBaud.ToolTipText = "Communication speed";
            // 
            // toolStripCBMode
            // 
            this.toolStripCBMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripCBMode.Items.AddRange(new object[] {
            "NMEA2000",
            "NMEA0183",
            "HEX"});
            this.toolStripCBMode.Name = "toolStripCBMode";
            this.toolStripCBMode.Size = new System.Drawing.Size(90, 25);
            this.toolStripCBMode.ToolTipText = "Mode";
            this.toolStripCBMode.SelectedIndexChanged += new System.EventHandler(this.toolStripCBMode_SelectedIndexChanged);
            // 
            // timerClose
            // 
            this.timerClose.Tick += new System.EventHandler(this.timerClose_Tick);
            // 
            // timerEventBytesAvg
            // 
            this.timerEventBytesAvg.Interval = 200;
            this.timerEventBytesAvg.Tick += new System.EventHandler(this.timerEventBytesAvg_Tick);
            // 
            // valueEventBytesAvg
            // 
            this.valueEventBytesAvg.Format = "G";
            this.valueEventBytesAvg.Location = new System.Drawing.Point(540, 5);
            this.valueEventBytesAvg.Max = 1.7976931348623157E+308D;
            this.valueEventBytesAvg.Min = -1.7976931348623157E+308D;
            this.valueEventBytesAvg.Name = "valueEventBytesAvg";
            this.valueEventBytesAvg.ReadOnly = true;
            this.valueEventBytesAvg.Size = new System.Drawing.Size(69, 20);
            this.valueEventBytesAvg.TabIndex = 2;
            this.valueEventBytesAvg.Text = "0";
            this.valueEventBytesAvg.Value = 0D;
            // 
            // labelEventBytesAvg
            // 
            this.labelEventBytesAvg.AutoSize = true;
            this.labelEventBytesAvg.Location = new System.Drawing.Point(447, 8);
            this.labelEventBytesAvg.Name = "labelEventBytesAvg";
            this.labelEventBytesAvg.Size = new System.Drawing.Size(87, 13);
            this.labelEventBytesAvg.TabIndex = 3;
            this.labelEventBytesAvg.Text = "Event bytes avg.";
            // 
            // NGT1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 426);
            this.Controls.Add(this.labelEventBytesAvg);
            this.Controls.Add(this.valueEventBytesAvg);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NGT1";
            this.Text = "Serial port listener";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NGT1_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.ToolStripButton toolStripButtonClose;
        private System.Windows.Forms.ToolStripComboBox toolStripCBPort;
        private System.Windows.Forms.ToolStripComboBox toolStripCBBaud;
        private System.Windows.Forms.ToolStripComboBox toolStripCBMode;
        private System.Windows.Forms.Timer timerClose;
        private KaveExtControls.ValueBox valueEventBytesAvg;
        private System.Windows.Forms.Timer timerEventBytesAvg;
        private System.Windows.Forms.Label labelEventBytesAvg;
    }
}