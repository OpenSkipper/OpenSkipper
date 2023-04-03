namespace OpenSkipperApplication
{
    partial class SettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtN2kPath = new System.Windows.Forms.TextBox();
            this.txtN0183Path = new System.Windows.Forms.TextBox();
            this.txtParamPath = new System.Windows.Forms.TextBox();
            this.btnChangeN2k = new System.Windows.Forms.Button();
            this.btnChangeN0183 = new System.Windows.Forms.Button();
            this.btnChangeParam = new System.Windows.Forms.Button();
            this.optPGNCustom = new System.Windows.Forms.RadioButton();
            this.optPGNInternal = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelDisplays = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.optAISCustom = new System.Windows.Forms.RadioButton();
            this.optAISInternal = new System.Windows.Forms.RadioButton();
            this.btnChangeDisplays = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDisplaysPath = new System.Windows.Forms.TextBox();
            this.btnChangeAIS = new System.Windows.Forms.Button();
            this.txtAISPath = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.optParamCustom = new System.Windows.Forms.RadioButton();
            this.optParamInternal = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.optN0183Custom = new System.Windows.Forms.RadioButton();
            this.optN0183Internal = new System.Windows.Forms.RadioButton();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbHideMenusOnStart = new System.Windows.Forms.CheckBox();
            this.panelWWW = new System.Windows.Forms.Panel();
            this.valueWWWPort = new KaveExtControls.ValueBox();
            this.labelWWWPort = new System.Windows.Forms.Label();
            this.labelWWWRoot = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtWWWRoot = new System.Windows.Forms.TextBox();
            this.groupBoxOther = new System.Windows.Forms.GroupBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelWWW.SuspendLayout();
            this.groupBoxOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "NMEA 2000 Definitions:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "NMEA 0183 Definitions:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 22);
            this.label3.TabIndex = 2;
            this.label3.Text = "Parameter Definitions:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtN2kPath
            // 
            this.txtN2kPath.Location = new System.Drawing.Point(295, 20);
            this.txtN2kPath.Name = "txtN2kPath";
            this.txtN2kPath.ReadOnly = true;
            this.txtN2kPath.Size = new System.Drawing.Size(264, 20);
            this.txtN2kPath.TabIndex = 3;
            // 
            // txtN0183Path
            // 
            this.txtN0183Path.Location = new System.Drawing.Point(295, 47);
            this.txtN0183Path.Name = "txtN0183Path";
            this.txtN0183Path.ReadOnly = true;
            this.txtN0183Path.Size = new System.Drawing.Size(264, 20);
            this.txtN0183Path.TabIndex = 4;
            // 
            // txtParamPath
            // 
            this.txtParamPath.Location = new System.Drawing.Point(295, 75);
            this.txtParamPath.Name = "txtParamPath";
            this.txtParamPath.ReadOnly = true;
            this.txtParamPath.Size = new System.Drawing.Size(264, 20);
            this.txtParamPath.TabIndex = 5;
            // 
            // btnChangeN2k
            // 
            this.btnChangeN2k.Location = new System.Drawing.Point(565, 19);
            this.btnChangeN2k.Name = "btnChangeN2k";
            this.btnChangeN2k.Size = new System.Drawing.Size(24, 21);
            this.btnChangeN2k.TabIndex = 6;
            this.btnChangeN2k.Text = "...";
            this.btnChangeN2k.UseVisualStyleBackColor = true;
            this.btnChangeN2k.Click += new System.EventHandler(this.btnChangeN2k_Click);
            // 
            // btnChangeN0183
            // 
            this.btnChangeN0183.Location = new System.Drawing.Point(565, 47);
            this.btnChangeN0183.Name = "btnChangeN0183";
            this.btnChangeN0183.Size = new System.Drawing.Size(24, 21);
            this.btnChangeN0183.TabIndex = 7;
            this.btnChangeN0183.Text = "...";
            this.btnChangeN0183.UseVisualStyleBackColor = true;
            this.btnChangeN0183.Click += new System.EventHandler(this.btnChangeN0183_Click);
            // 
            // btnChangeParam
            // 
            this.btnChangeParam.Location = new System.Drawing.Point(565, 75);
            this.btnChangeParam.Name = "btnChangeParam";
            this.btnChangeParam.Size = new System.Drawing.Size(24, 22);
            this.btnChangeParam.TabIndex = 8;
            this.btnChangeParam.Text = "...";
            this.btnChangeParam.UseVisualStyleBackColor = true;
            this.btnChangeParam.Click += new System.EventHandler(this.btnChangeParam_Click);
            // 
            // optPGNCustom
            // 
            this.optPGNCustom.AutoSize = true;
            this.optPGNCustom.Location = new System.Drawing.Point(69, 2);
            this.optPGNCustom.Name = "optPGNCustom";
            this.optPGNCustom.Size = new System.Drawing.Size(60, 17);
            this.optPGNCustom.TabIndex = 4;
            this.optPGNCustom.Text = "Custom";
            this.optPGNCustom.UseVisualStyleBackColor = true;
            this.optPGNCustom.CheckedChanged += new System.EventHandler(this.optPGNCustom_CheckedChanged);
            // 
            // optPGNInternal
            // 
            this.optPGNInternal.AutoSize = true;
            this.optPGNInternal.Checked = true;
            this.optPGNInternal.Location = new System.Drawing.Point(3, 2);
            this.optPGNInternal.Name = "optPGNInternal";
            this.optPGNInternal.Size = new System.Drawing.Size(60, 17);
            this.optPGNInternal.TabIndex = 5;
            this.optPGNInternal.TabStop = true;
            this.optPGNInternal.Text = "Internal";
            this.optPGNInternal.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.optPGNCustom);
            this.panel1.Controls.Add(this.optPGNInternal);
            this.panel1.Location = new System.Drawing.Point(156, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(133, 21);
            this.panel1.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelDisplays);
            this.groupBox1.Controls.Add(this.panel4);
            this.groupBox1.Controls.Add(this.btnChangeDisplays);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtDisplaysPath);
            this.groupBox1.Controls.Add(this.btnChangeAIS);
            this.groupBox1.Controls.Add(this.txtAISPath);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtN2kPath);
            this.groupBox1.Controls.Add(this.btnChangeParam);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtN0183Path);
            this.groupBox1.Controls.Add(this.btnChangeN2k);
            this.groupBox1.Controls.Add(this.btnChangeN0183);
            this.groupBox1.Controls.Add(this.txtParamPath);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(595, 158);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Definition files";
            // 
            // labelDisplays
            // 
            this.labelDisplays.Location = new System.Drawing.Point(6, 125);
            this.labelDisplays.Margin = new System.Windows.Forms.Padding(3);
            this.labelDisplays.Name = "labelDisplays";
            this.labelDisplays.Size = new System.Drawing.Size(144, 22);
            this.labelDisplays.TabIndex = 14;
            this.labelDisplays.Text = "Displays definition:";
            this.labelDisplays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.optAISCustom);
            this.panel4.Controls.Add(this.optAISInternal);
            this.panel4.Location = new System.Drawing.Point(156, 100);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(133, 21);
            this.panel4.TabIndex = 14;
            // 
            // optAISCustom
            // 
            this.optAISCustom.AutoSize = true;
            this.optAISCustom.Location = new System.Drawing.Point(69, 2);
            this.optAISCustom.Name = "optAISCustom";
            this.optAISCustom.Size = new System.Drawing.Size(60, 17);
            this.optAISCustom.TabIndex = 4;
            this.optAISCustom.Text = "Custom";
            this.optAISCustom.UseVisualStyleBackColor = true;
            this.optAISCustom.CheckedChanged += new System.EventHandler(this.optAISCustom_CheckedChanged);
            // 
            // optAISInternal
            // 
            this.optAISInternal.AutoSize = true;
            this.optAISInternal.Checked = true;
            this.optAISInternal.Location = new System.Drawing.Point(3, 2);
            this.optAISInternal.Name = "optAISInternal";
            this.optAISInternal.Size = new System.Drawing.Size(60, 17);
            this.optAISInternal.TabIndex = 5;
            this.optAISInternal.TabStop = true;
            this.optAISInternal.Text = "Internal";
            this.optAISInternal.UseVisualStyleBackColor = true;
            // 
            // btnChangeDisplays
            // 
            this.btnChangeDisplays.Location = new System.Drawing.Point(565, 127);
            this.btnChangeDisplays.Name = "btnChangeDisplays";
            this.btnChangeDisplays.Size = new System.Drawing.Size(24, 22);
            this.btnChangeDisplays.TabIndex = 16;
            this.btnChangeDisplays.Text = "...";
            this.btnChangeDisplays.UseVisualStyleBackColor = true;
            this.btnChangeDisplays.Click += new System.EventHandler(this.btnChangeDisplays_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 99);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 22);
            this.label4.TabIndex = 11;
            this.label4.Text = "AIS Definitions:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDisplaysPath
            // 
            this.txtDisplaysPath.Location = new System.Drawing.Point(295, 127);
            this.txtDisplaysPath.Name = "txtDisplaysPath";
            this.txtDisplaysPath.ReadOnly = true;
            this.txtDisplaysPath.Size = new System.Drawing.Size(264, 20);
            this.txtDisplaysPath.TabIndex = 15;
            // 
            // btnChangeAIS
            // 
            this.btnChangeAIS.Location = new System.Drawing.Point(565, 101);
            this.btnChangeAIS.Name = "btnChangeAIS";
            this.btnChangeAIS.Size = new System.Drawing.Size(24, 22);
            this.btnChangeAIS.TabIndex = 13;
            this.btnChangeAIS.Text = "...";
            this.btnChangeAIS.UseVisualStyleBackColor = true;
            this.btnChangeAIS.Click += new System.EventHandler(this.btnChangeAIS_Click);
            // 
            // txtAISPath
            // 
            this.txtAISPath.Location = new System.Drawing.Point(295, 101);
            this.txtAISPath.Name = "txtAISPath";
            this.txtAISPath.ReadOnly = true;
            this.txtAISPath.Size = new System.Drawing.Size(264, 20);
            this.txtAISPath.TabIndex = 12;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.optParamCustom);
            this.panel3.Controls.Add(this.optParamInternal);
            this.panel3.Location = new System.Drawing.Point(156, 74);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(133, 21);
            this.panel3.TabIndex = 10;
            // 
            // optParamCustom
            // 
            this.optParamCustom.AutoSize = true;
            this.optParamCustom.Location = new System.Drawing.Point(69, 2);
            this.optParamCustom.Name = "optParamCustom";
            this.optParamCustom.Size = new System.Drawing.Size(60, 17);
            this.optParamCustom.TabIndex = 4;
            this.optParamCustom.Text = "Custom";
            this.optParamCustom.UseVisualStyleBackColor = true;
            this.optParamCustom.CheckedChanged += new System.EventHandler(this.optParamCustom_CheckedChanged);
            // 
            // optParamInternal
            // 
            this.optParamInternal.AutoSize = true;
            this.optParamInternal.Checked = true;
            this.optParamInternal.Location = new System.Drawing.Point(3, 2);
            this.optParamInternal.Name = "optParamInternal";
            this.optParamInternal.Size = new System.Drawing.Size(60, 17);
            this.optParamInternal.TabIndex = 5;
            this.optParamInternal.TabStop = true;
            this.optParamInternal.Text = "Internal";
            this.optParamInternal.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.optN0183Custom);
            this.panel2.Controls.Add(this.optN0183Internal);
            this.panel2.Location = new System.Drawing.Point(156, 46);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(133, 21);
            this.panel2.TabIndex = 9;
            // 
            // optN0183Custom
            // 
            this.optN0183Custom.AutoSize = true;
            this.optN0183Custom.Location = new System.Drawing.Point(69, 2);
            this.optN0183Custom.Name = "optN0183Custom";
            this.optN0183Custom.Size = new System.Drawing.Size(60, 17);
            this.optN0183Custom.TabIndex = 4;
            this.optN0183Custom.Text = "Custom";
            this.optN0183Custom.UseVisualStyleBackColor = true;
            this.optN0183Custom.CheckedChanged += new System.EventHandler(this.optN0183Custom_CheckedChanged);
            // 
            // optN0183Internal
            // 
            this.optN0183Internal.AutoSize = true;
            this.optN0183Internal.Checked = true;
            this.optN0183Internal.Location = new System.Drawing.Point(3, 2);
            this.optN0183Internal.Name = "optN0183Internal";
            this.optN0183Internal.Size = new System.Drawing.Size(60, 17);
            this.optN0183Internal.TabIndex = 5;
            this.optN0183Internal.TabStop = true;
            this.optN0183Internal.Text = "Internal";
            this.optN0183Internal.UseVisualStyleBackColor = true;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnOK);
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(10, 299);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(595, 29);
            this.panelButtons.TabIndex = 10;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(439, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(520, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbHideMenusOnStart
            // 
            this.cbHideMenusOnStart.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbHideMenusOnStart.Location = new System.Drawing.Point(9, 78);
            this.cbHideMenusOnStart.Name = "cbHideMenusOnStart";
            this.cbHideMenusOnStart.Size = new System.Drawing.Size(164, 24);
            this.cbHideMenusOnStart.TabIndex = 0;
            this.cbHideMenusOnStart.Text = "Hide menus on start";
            this.cbHideMenusOnStart.UseVisualStyleBackColor = true;
            // 
            // panelWWW
            // 
            this.panelWWW.Controls.Add(this.valueWWWPort);
            this.panelWWW.Controls.Add(this.labelWWWPort);
            this.panelWWW.Controls.Add(this.labelWWWRoot);
            this.panelWWW.Controls.Add(this.button1);
            this.panelWWW.Controls.Add(this.txtWWWRoot);
            this.panelWWW.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelWWW.Location = new System.Drawing.Point(3, 16);
            this.panelWWW.Name = "panelWWW";
            this.panelWWW.Size = new System.Drawing.Size(589, 57);
            this.panelWWW.TabIndex = 13;
            // 
            // valueWWWPort
            // 
            this.valueWWWPort.Format = "G";
            this.valueWWWPort.Location = new System.Drawing.Point(292, 32);
            this.valueWWWPort.Max = 65535D;
            this.valueWWWPort.Min = 80D;
            this.valueWWWPort.Name = "valueWWWPort";
            this.valueWWWPort.Size = new System.Drawing.Size(100, 20);
            this.valueWWWPort.TabIndex = 18;
            this.valueWWWPort.Text = "80";
            this.valueWWWPort.Value = 80D;
            // 
            // labelWWWPort
            // 
            this.labelWWWPort.Location = new System.Drawing.Point(6, 31);
            this.labelWWWPort.Margin = new System.Windows.Forms.Padding(3);
            this.labelWWWPort.Name = "labelWWWPort";
            this.labelWWWPort.Size = new System.Drawing.Size(144, 22);
            this.labelWWWPort.TabIndex = 17;
            this.labelWWWPort.Text = "Web Server Port:";
            this.labelWWWPort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelWWWRoot
            // 
            this.labelWWWRoot.Location = new System.Drawing.Point(6, 5);
            this.labelWWWRoot.Margin = new System.Windows.Forms.Padding(3);
            this.labelWWWRoot.Name = "labelWWWRoot";
            this.labelWWWRoot.Size = new System.Drawing.Size(144, 22);
            this.labelWWWRoot.TabIndex = 14;
            this.labelWWWRoot.Text = "Web Server Files Directory:";
            this.labelWWWRoot.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(562, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 22);
            this.button1.TabIndex = 16;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnChangeWebServerDirectory_Click);
            // 
            // txtWWWRoot
            // 
            this.txtWWWRoot.Location = new System.Drawing.Point(292, 7);
            this.txtWWWRoot.Name = "txtWWWRoot";
            this.txtWWWRoot.ReadOnly = true;
            this.txtWWWRoot.Size = new System.Drawing.Size(264, 20);
            this.txtWWWRoot.TabIndex = 15;
            // 
            // groupBoxOther
            // 
            this.groupBoxOther.Controls.Add(this.cbHideMenusOnStart);
            this.groupBoxOther.Controls.Add(this.panelWWW);
            this.groupBoxOther.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxOther.Location = new System.Drawing.Point(10, 168);
            this.groupBoxOther.Name = "groupBoxOther";
            this.groupBoxOther.Size = new System.Drawing.Size(595, 107);
            this.groupBoxOther.TabIndex = 14;
            this.groupBoxOther.TabStop = false;
            this.groupBoxOther.Text = "Other";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 338);
            this.Controls.Add(this.groupBoxOther);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelWWW.ResumeLayout(false);
            this.panelWWW.PerformLayout();
            this.groupBoxOther.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtN2kPath;
        private System.Windows.Forms.TextBox txtN0183Path;
        private System.Windows.Forms.TextBox txtParamPath;
        private System.Windows.Forms.Button btnChangeN2k;
        private System.Windows.Forms.Button btnChangeN0183;
        private System.Windows.Forms.Button btnChangeParam;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton optPGNCustom;
        private System.Windows.Forms.RadioButton optPGNInternal;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton optParamCustom;
        private System.Windows.Forms.RadioButton optParamInternal;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton optN0183Custom;
        private System.Windows.Forms.RadioButton optN0183Internal;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.RadioButton optAISCustom;
        private System.Windows.Forms.RadioButton optAISInternal;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnChangeAIS;
        private System.Windows.Forms.TextBox txtAISPath;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelDisplays;
        private System.Windows.Forms.Button btnChangeDisplays;
        private System.Windows.Forms.TextBox txtDisplaysPath;
        private System.Windows.Forms.CheckBox cbHideMenusOnStart;
        private System.Windows.Forms.Panel panelWWW;
        private System.Windows.Forms.Label labelWWWRoot;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtWWWRoot;
        private System.Windows.Forms.GroupBox groupBoxOther;
        private System.Windows.Forms.Label labelWWWPort;
        private KaveExtControls.ValueBox valueWWWPort;
    }
}