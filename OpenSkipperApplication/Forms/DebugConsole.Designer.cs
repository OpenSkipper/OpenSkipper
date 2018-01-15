namespace OpenSkipperApplication.Forms
{
    partial class DebugConsole
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
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.chkInfos = new System.Windows.Forms.CheckBox();
            this.chkWarnings = new System.Windows.Forms.CheckBox();
            this.chkErrors = new System.Windows.Forms.CheckBox();
            this.chkLog = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.Location = new System.Drawing.Point(12, 35);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsole.Size = new System.Drawing.Size(519, 342);
            this.txtConsole.TabIndex = 0;
            // 
            // chkInfos
            // 
            this.chkInfos.AutoSize = true;
            this.chkInfos.Location = new System.Drawing.Point(12, 12);
            this.chkInfos.Name = "chkInfos";
            this.chkInfos.Size = new System.Drawing.Size(44, 17);
            this.chkInfos.TabIndex = 1;
            this.chkInfos.Text = "Info";
            this.chkInfos.UseVisualStyleBackColor = true;
            this.chkInfos.CheckedChanged += new System.EventHandler(this.chkInfos_CheckedChanged);
            // 
            // chkWarnings
            // 
            this.chkWarnings.AutoSize = true;
            this.chkWarnings.Checked = true;
            this.chkWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWarnings.Location = new System.Drawing.Point(62, 12);
            this.chkWarnings.Name = "chkWarnings";
            this.chkWarnings.Size = new System.Drawing.Size(71, 17);
            this.chkWarnings.TabIndex = 2;
            this.chkWarnings.Text = "Warnings";
            this.chkWarnings.UseVisualStyleBackColor = true;
            this.chkWarnings.CheckedChanged += new System.EventHandler(this.chkWarnings_CheckedChanged);
            // 
            // chkErrors
            // 
            this.chkErrors.AutoSize = true;
            this.chkErrors.Checked = true;
            this.chkErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkErrors.Location = new System.Drawing.Point(139, 12);
            this.chkErrors.Name = "chkErrors";
            this.chkErrors.Size = new System.Drawing.Size(53, 17);
            this.chkErrors.TabIndex = 3;
            this.chkErrors.Text = "Errors";
            this.chkErrors.UseVisualStyleBackColor = true;
            this.chkErrors.CheckedChanged += new System.EventHandler(this.chkErrors_CheckedChanged);
            // 
            // chkLog
            // 
            this.chkLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkLog.AutoSize = true;
            this.chkLog.Location = new System.Drawing.Point(375, 12);
            this.chkLog.Name = "chkLog";
            this.chkLog.Size = new System.Drawing.Size(75, 17);
            this.chkLog.TabIndex = 4;
            this.chkLog.Text = "Log to File";
            this.chkLog.UseVisualStyleBackColor = true;
            this.chkLog.CheckedChanged += new System.EventHandler(this.chkLog_CheckedChanged);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(456, 8);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // DebugConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 389);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.chkLog);
            this.Controls.Add(this.chkErrors);
            this.Controls.Add(this.chkWarnings);
            this.Controls.Add(this.chkInfos);
            this.Controls.Add(this.txtConsole);
            this.Name = "DebugConsole";
            this.Text = "DebugConsole";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.CheckBox chkInfos;
        private System.Windows.Forms.CheckBox chkWarnings;
        private System.Windows.Forms.CheckBox chkErrors;
        private System.Windows.Forms.CheckBox chkLog;
        private System.Windows.Forms.Button btnClear;
    }
}