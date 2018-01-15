namespace OpenSkipperApplication
{
    partial class LogControlForm
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
            this.btnPause = new System.Windows.Forms.Button();
            this.btnRewind = new System.Windows.Forms.Button();
            this.lblLogTime = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnPlay = new System.Windows.Forms.Button();
            this.numPlayspeed = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cANStreamerLogfileBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numPlayspeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerLogfileBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.SystemColors.Control;
            this.btnPause.FlatAppearance.BorderSize = 0;
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.Image = global::OpenSkipperApplication.Properties.Resources.Pause1;
            this.btnPause.Location = new System.Drawing.Point(31, 23);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(23, 23);
            this.btnPause.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnPause, "Pause");
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnRewind
            // 
            this.btnRewind.BackColor = System.Drawing.SystemColors.Control;
            this.btnRewind.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnRewind.FlatAppearance.BorderSize = 0;
            this.btnRewind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRewind.Image = global::OpenSkipperApplication.Properties.Resources.Rewind1;
            this.btnRewind.Location = new System.Drawing.Point(9, 23);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(23, 23);
            this.btnRewind.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnRewind, "Rewind");
            this.btnRewind.UseVisualStyleBackColor = false;
            this.btnRewind.Click += new System.EventHandler(this.btnRewind_Click);
            // 
            // lblLogTime
            // 
            this.lblLogTime.Location = new System.Drawing.Point(0, 1);
            this.lblLogTime.Name = "lblLogTime";
            this.lblLogTime.Size = new System.Drawing.Size(111, 20);
            this.lblLogTime.TabIndex = 6;
            this.lblLogTime.Text = "00:00:00 a.m";
            this.lblLogTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lblLogTime, "Current play-back time within log file");
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnPlay
            // 
            this.btnPlay.BackColor = System.Drawing.SystemColors.Control;
            this.btnPlay.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.cANStreamerLogfileBindingSource, "Paused", true));
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Image = global::OpenSkipperApplication.Properties.Resources.Play;
            this.btnPlay.Location = new System.Drawing.Point(53, 23);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(23, 23);
            this.btnPlay.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnPlay, "Play");
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            this.btnPlay.EnabledChanged += new System.EventHandler(this.btnPlay_EnabledChanged);
            // 
            // numPlayspeed
            // 
            this.numPlayspeed.BackColor = System.Drawing.SystemColors.Window;
            this.numPlayspeed.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.cANStreamerLogfileBindingSource, "PlaySpeed", true));
            this.numPlayspeed.DecimalPlaces = 2;
            this.numPlayspeed.Location = new System.Drawing.Point(47, 50);
            this.numPlayspeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numPlayspeed.Name = "numPlayspeed";
            this.numPlayspeed.Size = new System.Drawing.Size(50, 20);
            this.numPlayspeed.TabIndex = 5;
            this.toolTip1.SetToolTip(this.numPlayspeed, "Play-back speed (relative to original speed)");
            this.numPlayspeed.Value = new decimal(new int[] {
            100,
            0,
            0,
            131072});
            this.numPlayspeed.ValueChanged += new System.EventHandler(this.numPlayspeed_ValueChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 20);
            this.label1.TabIndex = 9;
            this.label1.Text = "Speed";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNext
            // 
            this.btnNext.Enabled = false;
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Image = global::OpenSkipperApplication.Properties.Resources.Step;
            this.btnNext.Location = new System.Drawing.Point(75, 23);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(23, 23);
            this.btnNext.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnNext, "Step forward one message");
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // cANStreamerLogfileBindingSource
            // 
            this.cANStreamerLogfileBindingSource.DataSource = typeof(CANStreams.CANStreamer_Logfile);
            // 
            // LogControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(111, 78);
            this.Controls.Add(this.lblLogTime);
            this.Controls.Add(this.numPlayspeed);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.btnRewind);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.label1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cANStreamerLogfileBindingSource, "Name", true));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LogControlForm";
            this.ShowInTaskbar = false;
            this.Text = "LogControlForm";
            this.VisibleChanged += new System.EventHandler(this.LogControlForm_VisibleChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogControlForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numPlayspeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerLogfileBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnRewind;
        private System.Windows.Forms.BindingSource cANStreamerLogfileBindingSource;
        private System.Windows.Forms.Label lblLogTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.NumericUpDown numPlayspeed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}