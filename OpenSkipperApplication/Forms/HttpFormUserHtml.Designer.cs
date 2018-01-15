namespace OpenSkipperApplication.Forms
{
    partial class HttpFormUserHtml
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
                listener.Close();
                feedListener.Close();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HttpFormUserHtml));
            this.chkHTTP = new System.Windows.Forms.CheckBox();
            this.groupBoxStatus = new System.Windows.Forms.GroupBox();
            this.richTextStatus = new System.Windows.Forms.RichTextBox();
            this.panelDescription = new System.Windows.Forms.Panel();
            this.labelDescription = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxWebServerSettings = new System.Windows.Forms.GroupBox();
            this.checkBoxStartLastState = new System.Windows.Forms.CheckBox();
            this.checkBoxLocalOnly = new System.Windows.Forms.CheckBox();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.groupBoxStatus.SuspendLayout();
            this.panelDescription.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBoxWebServerSettings.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkHTTP
            // 
            this.chkHTTP.AutoSize = true;
            this.chkHTTP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHTTP.Location = new System.Drawing.Point(20, 53);
            this.chkHTTP.Name = "chkHTTP";
            this.chkHTTP.Size = new System.Drawing.Size(129, 19);
            this.chkHTTP.TabIndex = 5;
            this.chkHTTP.Text = "Enable Web server";
            this.chkHTTP.UseVisualStyleBackColor = true;
            this.chkHTTP.CheckedChanged += new System.EventHandler(this.chkHTTP_CheckedChanged);
            // 
            // groupBoxStatus
            // 
            this.groupBoxStatus.Controls.Add(this.richTextStatus);
            this.groupBoxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxStatus.Location = new System.Drawing.Point(5, 5);
            this.groupBoxStatus.Name = "groupBoxStatus";
            this.groupBoxStatus.Size = new System.Drawing.Size(415, 122);
            this.groupBoxStatus.TabIndex = 9;
            this.groupBoxStatus.TabStop = false;
            this.groupBoxStatus.Text = "Web server status";
            // 
            // richTextStatus
            // 
            this.richTextStatus.BackColor = System.Drawing.SystemColors.Control;
            this.richTextStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextStatus.ForeColor = System.Drawing.SystemColors.WindowText;
            this.richTextStatus.Location = new System.Drawing.Point(3, 16);
            this.richTextStatus.Name = "richTextStatus";
            this.richTextStatus.Size = new System.Drawing.Size(409, 103);
            this.richTextStatus.TabIndex = 7;
            this.richTextStatus.Text = "Web server disabled.";
            this.richTextStatus.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextStatus_LinkClicked);
            // 
            // panelDescription
            // 
            this.panelDescription.Controls.Add(this.labelDescription);
            this.panelDescription.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDescription.Location = new System.Drawing.Point(0, 0);
            this.panelDescription.Name = "panelDescription";
            this.panelDescription.Padding = new System.Windows.Forms.Padding(5);
            this.panelDescription.Size = new System.Drawing.Size(425, 108);
            this.panelDescription.TabIndex = 10;
            // 
            // labelDescription
            // 
            this.labelDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription.Location = new System.Drawing.Point(5, 5);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(415, 98);
            this.labelDescription.TabIndex = 8;
            this.labelDescription.Text = "Open Skipper has a built in web server that can deliver parameter values to a web" +
    " page.\r\n\r\nWhen the web server is listening, you can connect to the server using " +
    "one of the IP listed below.";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBoxWebServerSettings);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 108);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(425, 134);
            this.panel1.TabIndex = 11;
            // 
            // groupBoxWebServerSettings
            // 
            this.groupBoxWebServerSettings.Controls.Add(this.checkBoxStartLastState);
            this.groupBoxWebServerSettings.Controls.Add(this.checkBoxLocalOnly);
            this.groupBoxWebServerSettings.Controls.Add(this.chkHTTP);
            this.groupBoxWebServerSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxWebServerSettings.Location = new System.Drawing.Point(5, 5);
            this.groupBoxWebServerSettings.Name = "groupBoxWebServerSettings";
            this.groupBoxWebServerSettings.Size = new System.Drawing.Size(415, 124);
            this.groupBoxWebServerSettings.TabIndex = 7;
            this.groupBoxWebServerSettings.TabStop = false;
            this.groupBoxWebServerSettings.Text = "Web server settings";
            // 
            // checkBoxStartLastState
            // 
            this.checkBoxStartLastState.AutoSize = true;
            this.checkBoxStartLastState.Checked = true;
            this.checkBoxStartLastState.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStartLastState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxStartLastState.Location = new System.Drawing.Point(20, 28);
            this.checkBoxStartLastState.Name = "checkBoxStartLastState";
            this.checkBoxStartLastState.Size = new System.Drawing.Size(179, 19);
            this.checkBoxStartLastState.TabIndex = 7;
            this.checkBoxStartLastState.Text = "Start Web server to last state";
            this.checkBoxStartLastState.UseVisualStyleBackColor = true;
            // 
            // checkBoxLocalOnly
            // 
            this.checkBoxLocalOnly.AutoSize = true;
            this.checkBoxLocalOnly.Checked = true;
            this.checkBoxLocalOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLocalOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxLocalOnly.Location = new System.Drawing.Point(52, 77);
            this.checkBoxLocalOnly.Name = "checkBoxLocalOnly";
            this.checkBoxLocalOnly.Size = new System.Drawing.Size(107, 19);
            this.checkBoxLocalOnly.TabIndex = 6;
            this.checkBoxLocalOnly.Text = "Local host only";
            this.checkBoxLocalOnly.UseVisualStyleBackColor = true;
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.groupBoxStatus);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatus.Location = new System.Drawing.Point(0, 242);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Padding = new System.Windows.Forms.Padding(5);
            this.panelStatus.Size = new System.Drawing.Size(425, 132);
            this.panelStatus.TabIndex = 12;
            // 
            // HttpFormUserHtml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 374);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelDescription);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HttpFormUserHtml";
            this.Text = "Http Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HttpFormUserHtml_FormClosing);
            this.Load += new System.EventHandler(this.HttpFormUserHtml_Load);
            this.groupBoxStatus.ResumeLayout(false);
            this.panelDescription.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBoxWebServerSettings.ResumeLayout(false);
            this.groupBoxWebServerSettings.PerformLayout();
            this.panelStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkHTTP;
        private System.Windows.Forms.GroupBox groupBoxStatus;
        private System.Windows.Forms.RichTextBox richTextStatus;
        private System.Windows.Forms.Panel panelDescription;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.CheckBox checkBoxLocalOnly;
        private System.Windows.Forms.GroupBox groupBoxWebServerSettings;
        private System.Windows.Forms.CheckBox checkBoxStartLastState;
    }
}