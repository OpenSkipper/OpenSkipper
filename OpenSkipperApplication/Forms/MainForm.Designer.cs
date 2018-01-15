namespace OpenSkipperApplication {
    partial class MainForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileLoadForm = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuComPort2000 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuComPort0183 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpenLogfile = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLogfileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuView = new System.Windows.Forms.ToolStripMenuItem();
            this.decodedStreamsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdvanced = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuCANDevices = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHTTP = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuDebugConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.pGNDefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAISDefns = new System.Windows.Forms.ToolStripMenuItem();
            this.paramExplorertoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.actisenseNGT1USBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.keesFileConverterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTabPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadDisplaysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTabPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.showGPSInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMenusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutDigitalBridgeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.contextMenu_Main = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topmostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMenusToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.displaysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.panelMain = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageGPSInfo = new System.Windows.Forms.TabPage();
            this.tabPageReadMe = new System.Windows.Forms.TabPage();
            this.richTextBoxReadMe = new System.Windows.Forms.RichTextBox();
            this.cANStreamerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.extender = new OpenSkipperApplication.ParameterExtender(this.components);
            this.menuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenu_Main.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageGPSInfo.SuspendLayout();
            this.tabPageReadMe.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Enabled = false;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileLoadForm,
            this.toolStripMenuView,
            this.toolStripMenuTools,
            this.toolStripMenuItemDisplay,
            this.toolStripMenuItemAbout});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(772, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            this.menuStrip.MenuActivate += new System.EventHandler(this.menuStrip_MenuActivate);
            this.menuStrip.MenuDeactivate += new System.EventHandler(this.menuStrip_MenuDeactivate);
            this.menuStrip.Leave += new System.EventHandler(this.menuStrip_Leave);
            this.menuStrip.MouseEnter += new System.EventHandler(this.menuStrip_MouseEnter);
            this.menuStrip.MouseLeave += new System.EventHandler(this.menuStrip_MouseLeave);
            // 
            // fileLoadForm
            // 
            this.fileLoadForm.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuComPort2000,
            this.mnuComPort0183,
            this.mnuOpenLogfile,
            this.viewLogfileMenuItem,
            this.toolStripSeparator1,
            this.settingsToolStripMenuItem1,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileLoadForm.Name = "fileLoadForm";
            this.fileLoadForm.Size = new System.Drawing.Size(37, 20);
            this.fileLoadForm.Text = "&File";
            this.fileLoadForm.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // mnuComPort2000
            // 
            this.mnuComPort2000.Name = "mnuComPort2000";
            this.mnuComPort2000.Size = new System.Drawing.Size(292, 22);
            this.mnuComPort2000.Text = "Open NMEA 2000 (Actisense NGT-1-USB)";
            this.mnuComPort2000.ToolTipText = "Open a COM port connected to an NGT-1";
            this.mnuComPort2000.Click += new System.EventHandler(this.mnuComPort2000_Click);
            // 
            // mnuComPort0183
            // 
            this.mnuComPort0183.Name = "mnuComPort0183";
            this.mnuComPort0183.Size = new System.Drawing.Size(292, 22);
            this.mnuComPort0183.Text = "Open NMEA 0183";
            this.mnuComPort0183.ToolTipText = "Open a COM port connected to an NMEA 0183 device";
            // 
            // mnuOpenLogfile
            // 
            this.mnuOpenLogfile.Name = "mnuOpenLogfile";
            this.mnuOpenLogfile.Size = new System.Drawing.Size(292, 22);
            this.mnuOpenLogfile.Text = "Replay Logfile...";
            this.mnuOpenLogfile.ToolTipText = "Read in and play back a file of previously logged instrument data";
            this.mnuOpenLogfile.Click += new System.EventHandler(this.mnuOpenLogfile_Click);
            // 
            // viewLogfileMenuItem
            // 
            this.viewLogfileMenuItem.Name = "viewLogfileMenuItem";
            this.viewLogfileMenuItem.Size = new System.Drawing.Size(292, 22);
            this.viewLogfileMenuItem.Text = "View Logfile...";
            this.viewLogfileMenuItem.ToolTipText = "Read in and decode a file of previously logged instrument data";
            this.viewLogfileMenuItem.Click += new System.EventHandler(this.viewLogfileMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(289, 6);
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(292, 22);
            this.settingsToolStripMenuItem1.Text = "Settings...";
            this.settingsToolStripMenuItem1.ToolTipText = "Change the files used to define how messages should be decoded";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(289, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(292, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripMenuView
            // 
            this.toolStripMenuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decodedStreamsToolStripMenuItem,
            this.mnuAdvanced,
            this.toolStripMenuCANDevices,
            this.toolStripSeparator5,
            this.mnuHTTP,
            this.toolStripSeparator3,
            this.mnuDebugConsole,
            this.toolStripMenuItem6,
            this.fullScreenToolStripMenuItem});
            this.toolStripMenuView.Name = "toolStripMenuView";
            this.toolStripMenuView.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuView.Text = "&View";
            // 
            // decodedStreamsToolStripMenuItem
            // 
            this.decodedStreamsToolStripMenuItem.Name = "decodedStreamsToolStripMenuItem";
            this.decodedStreamsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.decodedStreamsToolStripMenuItem.Text = "Decoded Messages...";
            this.decodedStreamsToolStripMenuItem.ToolTipText = "View and decode the messages arriving from one or more sources";
            this.decodedStreamsToolStripMenuItem.Click += new System.EventHandler(this.decodedStreamsToolStripMenuItem_Click);
            // 
            // mnuAdvanced
            // 
            this.mnuAdvanced.Name = "mnuAdvanced";
            this.mnuAdvanced.Size = new System.Drawing.Size(184, 22);
            this.mnuAdvanced.Text = "Data Streams...";
            this.mnuAdvanced.ToolTipText = "View the sources of messages, and where they are being sent";
            this.mnuAdvanced.Click += new System.EventHandler(this.mnuAdvanced_Click);
            // 
            // toolStripMenuCANDevices
            // 
            this.toolStripMenuCANDevices.Name = "toolStripMenuCANDevices";
            this.toolStripMenuCANDevices.Size = new System.Drawing.Size(184, 22);
            this.toolStripMenuCANDevices.Text = "NMEA2000 devices...";
            this.toolStripMenuCANDevices.ToolTipText = "View NMEA2000 bus device information";
            this.toolStripMenuCANDevices.Click += new System.EventHandler(this.toolStripMenuCANDevices_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(181, 6);
            // 
            // mnuHTTP
            // 
            this.mnuHTTP.Name = "mnuHTTP";
            this.mnuHTTP.Size = new System.Drawing.Size(184, 22);
            this.mnuHTTP.Text = "Web Server...";
            this.mnuHTTP.ToolTipText = "Configure the Open Skipper web server that can send instrument data to a browser";
            this.mnuHTTP.Click += new System.EventHandler(this.mnuHTTP_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(181, 6);
            // 
            // mnuDebugConsole
            // 
            this.mnuDebugConsole.Name = "mnuDebugConsole";
            this.mnuDebugConsole.Size = new System.Drawing.Size(184, 22);
            this.mnuDebugConsole.Text = "Debug Console...";
            this.mnuDebugConsole.ToolTipText = "View any debug or information messages produced by Open Skipper";
            this.mnuDebugConsole.Click += new System.EventHandler(this.mnuDebugConsole_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(181, 6);
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            this.fullScreenToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.fullScreenToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fullScreenToolStripMenuItem.Text = "Full Screen";
            this.fullScreenToolStripMenuItem.Click += new System.EventHandler(this.fullScreenToolStripMenuItem_Click);
            // 
            // toolStripMenuTools
            // 
            this.toolStripMenuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pGNDefinitionsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.mnuAISDefns,
            this.paramExplorertoolStripMenuItem1,
            this.actisenseNGT1USBToolStripMenuItem,
            this.toolStripSeparator4,
            this.keesFileConverterToolStripMenuItem});
            this.toolStripMenuTools.Name = "toolStripMenuTools";
            this.toolStripMenuTools.Size = new System.Drawing.Size(48, 20);
            this.toolStripMenuTools.Text = "&Tools";
            this.toolStripMenuTools.DropDownOpening += new System.EventHandler(this.toolStripMenuItem1_DropDownOpening);
            // 
            // pGNDefinitionsToolStripMenuItem
            // 
            this.pGNDefinitionsToolStripMenuItem.Name = "pGNDefinitionsToolStripMenuItem";
            this.pGNDefinitionsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.pGNDefinitionsToolStripMenuItem.Text = "PGN Definitions Explorer...";
            this.pGNDefinitionsToolStripMenuItem.Click += new System.EventHandler(this.pGNDefinitionsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(221, 22);
            this.toolStripMenuItem2.Text = "N0183 Definitions Explorer...";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // mnuAISDefns
            // 
            this.mnuAISDefns.Name = "mnuAISDefns";
            this.mnuAISDefns.Size = new System.Drawing.Size(221, 22);
            this.mnuAISDefns.Text = "AIS Definitions Explorer...";
            this.mnuAISDefns.Click += new System.EventHandler(this.mnuAISDefns_Click);
            // 
            // paramExplorertoolStripMenuItem1
            // 
            this.paramExplorertoolStripMenuItem1.Name = "paramExplorertoolStripMenuItem1";
            this.paramExplorertoolStripMenuItem1.Size = new System.Drawing.Size(221, 22);
            this.paramExplorertoolStripMenuItem1.Text = "Parameter Explorer...";
            this.paramExplorertoolStripMenuItem1.Click += new System.EventHandler(this.paramExplorertoolStripMenuItem1_Click);
            // 
            // actisenseNGT1USBToolStripMenuItem
            // 
            this.actisenseNGT1USBToolStripMenuItem.Name = "actisenseNGT1USBToolStripMenuItem";
            this.actisenseNGT1USBToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.actisenseNGT1USBToolStripMenuItem.Text = "Serial port listener...";
            this.actisenseNGT1USBToolStripMenuItem.ToolTipText = "Simply listens serial port data.";
            this.actisenseNGT1USBToolStripMenuItem.Click += new System.EventHandler(this.actisenseNGT1USBToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(218, 6);
            // 
            // keesFileConverterToolStripMenuItem
            // 
            this.keesFileConverterToolStripMenuItem.Name = "keesFileConverterToolStripMenuItem";
            this.keesFileConverterToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.keesFileConverterToolStripMenuItem.Text = "Kees File Converter...";
            this.keesFileConverterToolStripMenuItem.Click += new System.EventHandler(this.keesFileConverterToolStripMenuItem_Click);
            // 
            // toolStripMenuItemDisplay
            // 
            this.toolStripMenuItemDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFormToolStripMenuItem,
            this.loadTabPageToolStripMenuItem,
            this.reloadDisplaysToolStripMenuItem,
            this.removeTabPageToolStripMenuItem,
            this.toolStripMenuItem4,
            this.showGPSInfoToolStripMenuItem,
            this.showMenusToolStripMenuItem,
            this.toolStripSeparator6});
            this.toolStripMenuItemDisplay.Name = "toolStripMenuItemDisplay";
            this.toolStripMenuItemDisplay.Size = new System.Drawing.Size(57, 20);
            this.toolStripMenuItemDisplay.Text = "&Display";
            // 
            // loadFormToolStripMenuItem
            // 
            this.loadFormToolStripMenuItem.Name = "loadFormToolStripMenuItem";
            this.loadFormToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.loadFormToolStripMenuItem.Text = "Load Form...";
            this.loadFormToolStripMenuItem.ToolTipText = "Experimental: Load pre-built Open Skipper forms from their XML files";
            this.loadFormToolStripMenuItem.Click += new System.EventHandler(this.loadFormToolStripMenuItem_Click);
            // 
            // loadTabPageToolStripMenuItem
            // 
            this.loadTabPageToolStripMenuItem.Name = "loadTabPageToolStripMenuItem";
            this.loadTabPageToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.loadTabPageToolStripMenuItem.Text = "Load Tab Page...";
            this.loadTabPageToolStripMenuItem.ToolTipText = "Experimental: Load a pre-built Open Skipper form (saved as an XML file) and displ" +
    "ay it as a new tab page ";
            this.loadTabPageToolStripMenuItem.Click += new System.EventHandler(this.loadTabPageToolStripMenuItem_Click);
            // 
            // reloadDisplaysToolStripMenuItem
            // 
            this.reloadDisplaysToolStripMenuItem.Name = "reloadDisplaysToolStripMenuItem";
            this.reloadDisplaysToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.R)));
            this.reloadDisplaysToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.reloadDisplaysToolStripMenuItem.Text = "Reload displays";
            this.reloadDisplaysToolStripMenuItem.Click += new System.EventHandler(this.reloadDisplaysToolStripMenuItem_Click);
            // 
            // removeTabPageToolStripMenuItem
            // 
            this.removeTabPageToolStripMenuItem.Name = "removeTabPageToolStripMenuItem";
            this.removeTabPageToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.removeTabPageToolStripMenuItem.Text = "Remove Tab Page";
            this.removeTabPageToolStripMenuItem.ToolTipText = "Experimental: Remove the currently displayed  tab page from the main form";
            this.removeTabPageToolStripMenuItem.Click += new System.EventHandler(this.removeTabPageToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(225, 6);
            // 
            // showGPSInfoToolStripMenuItem
            // 
            this.showGPSInfoToolStripMenuItem.CheckOnClick = true;
            this.showGPSInfoToolStripMenuItem.Name = "showGPSInfoToolStripMenuItem";
            this.showGPSInfoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.showGPSInfoToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.showGPSInfoToolStripMenuItem.Text = "Show GPS Info";
            this.showGPSInfoToolStripMenuItem.Click += new System.EventHandler(this.showGPSInfoToolStripMenuItem_Click);
            // 
            // showMenusToolStripMenuItem
            // 
            this.showMenusToolStripMenuItem.Checked = true;
            this.showMenusToolStripMenuItem.CheckOnClick = true;
            this.showMenusToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showMenusToolStripMenuItem.Name = "showMenusToolStripMenuItem";
            this.showMenusToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.showMenusToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.showMenusToolStripMenuItem.Text = "Show menus";
            this.showMenusToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showMenusToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(225, 6);
            // 
            // toolStripMenuItemAbout
            // 
            this.toolStripMenuItemAbout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.toolStripMenuItem5,
            this.aboutDigitalBridgeToolStripMenuItem});
            this.toolStripMenuItemAbout.Name = "toolStripMenuItemAbout";
            this.toolStripMenuItemAbout.Size = new System.Drawing.Size(52, 20);
            this.toolStripMenuItemAbout.Text = "About";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.helpToolStripMenuItem.Text = "Help...";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(187, 6);
            // 
            // aboutDigitalBridgeToolStripMenuItem
            // 
            this.aboutDigitalBridgeToolStripMenuItem.Name = "aboutDigitalBridgeToolStripMenuItem";
            this.aboutDigitalBridgeToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.aboutDigitalBridgeToolStripMenuItem.Text = "About Open Skipper...";
            this.aboutDigitalBridgeToolStripMenuItem.Click += new System.EventHandler(this.aboutDigitalBridgeToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 431);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(772, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // helpProvider1
            // 
            this.helpProvider1.HelpNamespace = "Help.chm";
            // 
            // contextMenu_Main
            // 
            this.contextMenu_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mimizeToolStripMenuItem,
            this.topmostToolStripMenuItem,
            this.showMenusToolStripMenuItem1,
            this.displaysToolStripMenuItem,
            this.loggingToolStripMenuItem});
            this.contextMenu_Main.Name = "contextMenuStrip1";
            this.contextMenu_Main.Size = new System.Drawing.Size(188, 114);
            // 
            // mimizeToolStripMenuItem
            // 
            this.mimizeToolStripMenuItem.Name = "mimizeToolStripMenuItem";
            this.mimizeToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.mimizeToolStripMenuItem.Text = "Mimize";
            this.mimizeToolStripMenuItem.Click += new System.EventHandler(this.mimizeToolStripMenuItem_Click);
            // 
            // topmostToolStripMenuItem
            // 
            this.topmostToolStripMenuItem.CheckOnClick = true;
            this.topmostToolStripMenuItem.Name = "topmostToolStripMenuItem";
            this.topmostToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.topmostToolStripMenuItem.Text = "Show on top";
            this.topmostToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.TopMostChanged);
            // 
            // showMenusToolStripMenuItem1
            // 
            this.showMenusToolStripMenuItem1.Checked = true;
            this.showMenusToolStripMenuItem1.CheckOnClick = true;
            this.showMenusToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showMenusToolStripMenuItem1.Name = "showMenusToolStripMenuItem1";
            this.showMenusToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.showMenusToolStripMenuItem1.Size = new System.Drawing.Size(187, 22);
            this.showMenusToolStripMenuItem1.Text = "Show menus";
            this.showMenusToolStripMenuItem1.Click += new System.EventHandler(this.showMenusToolStripMenuItem_Click);
            // 
            // displaysToolStripMenuItem
            // 
            this.displaysToolStripMenuItem.Name = "displaysToolStripMenuItem";
            this.displaysToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.displaysToolStripMenuItem.Text = "Displays";
            // 
            // loggingToolStripMenuItem
            // 
            this.loggingToolStripMenuItem.Name = "loggingToolStripMenuItem";
            this.loggingToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loggingToolStripMenuItem.Text = "Logging";
            // 
            // richTextBox3
            // 
            this.richTextBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox3.Location = new System.Drawing.Point(10, 65);
            this.richTextBox3.Name = "richTextBox3";
            this.extender.SetParameterName(this.richTextBox3, "SatTest");
            this.extender.SetPropertyToSet(this.richTextBox3, "Text");
            this.richTextBox3.Size = new System.Drawing.Size(748, 316);
            this.extender.SetStreamsToMatch(this.richTextBox3, "Any");
            this.richTextBox3.TabIndex = 0;
            this.richTextBox3.Text = "This will display information on visible satellites. Test this using a GPS data f" +
    "eed.";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox2.Location = new System.Drawing.Point(8, 6);
            this.richTextBox2.Name = "richTextBox2";
            this.extender.SetParameterName(this.richTextBox2, "");
            this.richTextBox2.Size = new System.Drawing.Size(748, 47);
            this.extender.SetStreamsToMatch(this.richTextBox2, "");
            this.richTextBox2.TabIndex = 0;
            this.richTextBox2.Text = "This will display information on visible satellites. Test this using an NMEA 0183" +
    " or NMEA 2000  GPS data feed or log file (such as \'Kees NMEA2000 Sample.xml\')";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.tabControl1);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 24);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(772, 407);
            this.panelMain.TabIndex = 18;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageGPSInfo);
            this.tabControl1.Controls.Add(this.tabPageReadMe);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ItemSize = new System.Drawing.Size(42, 18);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(772, 407);
            this.tabControl1.TabIndex = 18;
            this.tabControl1.Visible = false;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabPageGPSInfo
            // 
            this.tabPageGPSInfo.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageGPSInfo.Controls.Add(this.richTextBox3);
            this.tabPageGPSInfo.Controls.Add(this.richTextBox2);
            this.tabPageGPSInfo.Location = new System.Drawing.Point(4, 22);
            this.tabPageGPSInfo.Name = "tabPageGPSInfo";
            this.tabPageGPSInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGPSInfo.Size = new System.Drawing.Size(764, 381);
            this.tabPageGPSInfo.TabIndex = 1;
            this.tabPageGPSInfo.Text = "GPS Info";
            this.tabPageGPSInfo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.StartMoveDisplay);
            // 
            // tabPageReadMe
            // 
            this.tabPageReadMe.Controls.Add(this.richTextBoxReadMe);
            this.tabPageReadMe.Location = new System.Drawing.Point(4, 22);
            this.tabPageReadMe.Name = "tabPageReadMe";
            this.tabPageReadMe.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageReadMe.Size = new System.Drawing.Size(764, 381);
            this.tabPageReadMe.TabIndex = 2;
            this.tabPageReadMe.Text = "ReadMe";
            this.tabPageReadMe.UseVisualStyleBackColor = true;
            // 
            // richTextBoxReadMe
            // 
            this.richTextBoxReadMe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxReadMe.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxReadMe.Name = "richTextBoxReadMe";
            this.richTextBoxReadMe.Size = new System.Drawing.Size(758, 375);
            this.richTextBoxReadMe.TabIndex = 0;
            this.richTextBoxReadMe.Text = resources.GetString("richTextBoxReadMe.Text");
            // 
            // cANStreamerBindingSource
            // 
            this.cANStreamerBindingSource.DataSource = typeof(CANStreams.CANStreamer);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(772, 453);
            this.ContextMenuStrip = this.contextMenu_Main;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Open Skipper";
            this.TransparencyKey = System.Drawing.Color.MistyRose;
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenu_Main.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageGPSInfo.ResumeLayout(false);
            this.tabPageReadMe.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cANStreamerBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileLoadForm;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAbout;
        private System.Windows.Forms.ToolStripMenuItem aboutDigitalBridgeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuTools;
        private System.Windows.Forms.ToolStripMenuItem keesFileConverterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pGNDefinitionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem paramExplorertoolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.BindingSource cANStreamerBindingSource;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuView;
        private System.Windows.Forms.ToolStripMenuItem decodedStreamsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuOpenLogfile;
        private System.Windows.Forms.ToolStripMenuItem mnuComPort0183;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem actisenseNGT1USBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuAdvanced;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem viewLogfileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuComPort2000;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem mnuHTTP;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mnuDebugConsole;
        private System.Windows.Forms.ToolStripMenuItem mnuAISDefns;
        private ParameterExtender extender;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDisplay;
        private System.Windows.Forms.ToolStripMenuItem loadFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTabPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem removeTabPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.ContextMenuStrip contextMenu_Main;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem fullScreenToolStripMenuItem;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageGPSInfo;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.TabPage tabPageReadMe;
        private System.Windows.Forms.RichTextBox richTextBoxReadMe;
        private System.Windows.Forms.ToolStripMenuItem showGPSInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem showMenusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadDisplaysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mimizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showMenusToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem displaysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topmostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuCANDevices;
    }
}