/*
	Copyright (C) 2009-2010, Andrew Mason <amas008@users.sourceforge.net>
	Copyright (C) 2009-2010, Jason Drake <jdra@users.sourceforge.net>

	This file is part of Open Skipper.
	
	Open Skipper is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Open Skipper is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using Parameters;
using System.Collections;
using System;
using System.IO;
using System.Windows.Forms;
using CANHandler;
using CANStreams;
using CANDevices;
using OpenSkipperApplication.Forms;
using OpenSkipperApplication.Properties;
using System.Reflection;
using System.IO.Ports;
using CANDefinitions;
using System.Linq;
using AJMLoaderCode;
using System.Drawing.Drawing2D;
using System.Drawing;
using DisplayDefinitions;

namespace OpenSkipperApplication
{
    public partial class MainForm : Form
    {
        // Forms
        Form frmPGNForm; // PGN Explorer
        Form frmN0183Form; // N0183 Explorer
        Form frmParamForm; // Parameter Explorer
        Form frmSettingsForm; // Settings form
        Form frmDecodedLogForm; // Viewer
        Form frmStreamManagerForm; // Manager
        Form frmCANDevices;
        Form frmHTTP; // HTTP Form
        Form frmDebugConsole;
        Form frmAisExplorer;

        bool _doFullScreen = false;
        Size FullViewSize;
        DisplaysCollection Displays;
        bool StartMinimized = false;

        // Constructor
        public MainForm()
        {
            InitializeComponent();
            CANDeviceList.Initialize(); // How to initialize this automatically
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // return;

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            // Add version number to title bar
            this.Text += " (v" + Application.ProductVersion + ")";

            bool changeWindowRegion = false;
            if (changeWindowRegion) {
                // http://ekotc96.blogspot.com/2006/05/create-rounded-corner-form-in-cnet.html
                GraphicsPath p = new GraphicsPath();
                p.StartFigure();
                p.AddArc(new Rectangle(0, 0, 40, 40), 180, 90);
                p.AddLine(40, 0, this.Width - 40, 0);
                p.AddArc(new Rectangle(this.Width - 40, 0, 40, 40), -90, 90);
                p.AddLine(this.Width, 40, this.Width, this.Height - 40);
                p.AddArc(new Rectangle(this.Width - 40, this.Height - 40, 40, 40), 0, 90);
                p.AddLine(this.Width - 40, this.Height, 40, this.Height);
                p.AddArc(new Rectangle(0, this.Height - 40, 40, 40), 90, 90);
                p.CloseFigure();
                this.Region = new Region(p);
                //SolidBrush oBlackBrush = new SolidBrush(System.Drawing.Color.Black);
                //System.Drawing.Pen borderPen = new System.Drawing.Pen(oBlackBrush, 4);
                //e.Graphics.DrawPath(borderPen, p);
                //borderPen.Dispose();
                p.Dispose();
            }

            bool MakeWindowTransparent = false;
            if (MakeWindowTransparent) {
                // http://www.codeproject.com/KB/cs/Free_Form_Window.aspx
                this.ForeColor = SystemColors.ControlText;
                this.FormBorderStyle = FormBorderStyle.None;
                this.HelpButton = false;
                this.TransparencyKey = SystemColors.ControlText;
            }

            bool FullScreen = false;
            if (FullScreen) {
                // Can also try:
                // Bounds = Screen.GetBounds(this);
                // Bounds = Screen.GetWorkingArea(this);

                // http://www.codeproject.com/KB/cs/Free_Form_Window.aspx
                this.FormBorderStyle = FormBorderStyle.None;
                this.HelpButton = false;
                // this.TopMost = true;
                this.WindowState = System.Windows.Forms.FormWindowState.Maximized;  // Must follow FormBorderStyle = FormBorderStyle.None;
                this.menuStrip.AllowItemReorder = true;
                this.menuStrip.Dock = DockStyle.None;
                // this.menuStrip.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
                this.menuStrip.Anchor = AnchorStyles.None;
                this.menuStrip.GripStyle = ToolStripGripStyle.Visible;
                this.MainMenuStrip = null;
                menuStrip.Height = 1;
                //this.menuStrip.Visible = false;
                // Create a panel at the top of the form to pop-up the main menu
                // this.panelMenuPopup.Location = new System.Drawing.Point(0,0);
                // this.panelMenuPopup.Dock = DockStyle.Top;
                //this.panelMenuPopup.Height = 1;
                //this.panelMenuPopup.Name = "panelMenuPopup";
                //this.panelMenuPopup.Size = new System.Drawing.Size(269, 8);
                //this.panelMenuPopup.TabIndex = 16;
                //this.panelMenuPopup.MouseEnter += new System.EventHandler(this.panelMenuPopup_MouseEnter);
                //this.panelMenuPopup.Visible = true;

                this.tabControl1.Top = 1; // panelMenuPopup.Top + panelMenuPopup.Height;
            }

            if (Settings.Default.Size.Height > 50) this.Size = Settings.Default.Size;
            FullViewSize = this.Size;
            if (Settings.Default.Position.X >= 0) this.Location = Settings.Default.Position;

            // With debugger forms stays under IDE, if we start main form minimized.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            this.Show();
            Application.DoEvents();

            var about = new About();
            // about.ShowDialog(this);

            LoadDefinitions();
            CreateLoggingMenus();

            /*
            AISDefnCollection defnCol = new AISDefnCollection();
            string aivdm = File.ReadAllText("U:\\AIVDM.txt");
            Regex regex = new Regex(@"=== Types? (.*?): (.*?) ===.*?topbot.*?\|=(.*?)\|=", RegexOptions.Compiled | RegexOptions.Singleline);
            MatchCollection matches = regex.Matches(aivdm);
            foreach (Match m in matches)
            {
                string typeNum = m.Groups[1].Value;
                string desc = m.Groups[2].Value;
                string fields = m.Groups[3].Value;

                int imin = 0;
                int imax = 0;

                if (typeNum == "1, 2 and 3")
                {
                    imin = 1;
                    imax = 3;
                }
                else
                {
                    imin = imax = int.Parse(typeNum);
                }

                for (int i = imin; i <= imax; i++)
                {
                    AISDefn defn = new AISDefn() { MessageID = (ulong)i, Description = desc, Fields = new AISField[0] };

                    string[] fieldSplit = fields.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < fieldSplit.Length; j++)
                    {
                        string[] colSplit = fieldSplit[j].Split(new char[] { '|' });
                        if (colSplit.Length < 5)
                            continue;

                        int bitoffset = -1;
                        int bitlength = -1;

                        Match mm = Regex.Match(colSplit[1], "([0-9]*) *- *([0-9]*)");
                        if (mm.Success)
                        {
                            bitoffset = int.Parse(mm.Groups[1].Value);
                            int bitoffset2 = int.Parse(mm.Groups[2].Value);
                            bitlength = bitoffset2 - bitoffset + 1;
                        }
                        else
                        {
                            if (int.TryParse(colSplit[1].Trim(), out bitoffset))
                            {
                                if (int.TryParse(colSplit[2].Trim(), out bitlength))
                                {

                                }
                            }
                        }

                        if (bitlength > 0)
                        {
                            AISField newField;

                            if (colSplit[5].ToLower().Contains("character") || colSplit[5].ToLower().Contains("chars"))
                                newField = new AISString();
                            else if (colSplit[5].ToLower().Contains("lati") || 
                                    colSplit[5].ToLower().Contains("longi") ||
                                    colSplit[3].ToLower().Contains("lati") ||
                                    colSplit[3].ToLower().Contains("longi"))
                                newField = new AISDouble() { Scale = 1.0 / 600000.0, Offset = 0.0 };
                            else
                                newField = new AISUnsigned();

                            newField.Name = colSplit[3].Trim();
                            newField.Description = colSplit[5].Trim();
                            newField.BitOffset = bitoffset;
                            newField.BitLength = bitlength;

                            defn.AddField(newField);
                        }
                        else
                        {
                            Console.WriteLine(i + " - '" + colSplit[1] + "' - '" + colSplit[2] + "'");
                        }
                    }

                    defnCol.AddAISDefn(defn);
                }
            }

            defnCol.SaveToFile(@"U:\Test.xml");
            */

            /*
            byte[] testdata = new byte[] { 0, 1, 5, 10, 20, 40, 80, 160, 250, 255, 255 };
            string testres = AisEncoding.EncodeBytes(testdata, 0, testdata.Length);
            byte[] recovdata = AisEncoding.GetBytes(testres);
            */

            /*
            var b = new AISFrame.Builder();
            string[] tests = new string[]
            {
                "!AIVDM,2,1,3,B,55P5TL01VIaAL@7WKO@mBplU@<PDhh000000001S;AJ::4A80?4i@E53,0*3E",
                "!AIVDM,2,2,3,B,1@0000000000000,2*55",
                "!AIVDM,1,1,,B,177KQJ5000G?tO`K>RA1wUbN0TKH,0*5C",
                "!AIVDM,1,1,,A,13u?etPv2;0n:dDPwUM1U1Cb069D,0*24",
                "!AIVDM,1,1,,A,14eG;o@034o8sd<L9i:a;WF>062D,0*7D",
                "!AIVDM,1,1,,A,152JPuOP00JvJ8nHvW`0j?v020Sn,0*55,d-053,S2239,t235959.00,T59.70994503,r01SSSP1,1263945603",
                "!AIVDM,2,1,3,A,59NS7F@2=bJl7PHC:204i85:222222222222221AE`P,0*46,d-107,S2093,t235955.00,T55.82982737,r003669959,1263945597",
                "!AIVDM,2,2,3,A,G94N@0E@DVQEp;hC1iDR@H888880,2*4E,d-107,S2093,t235955.00,T55.82982737,r003669959,1263945597",
                "!AIVDM,1,1,,A,19NRpnh00SJq:;<GkkNm?4Al0@QF,0*34,d-108,S2096,t235955.00,T55.90987913,r003669959,1263945597"
            };

            foreach (string t in tests)
            {
                var f = b.AddFrame(new N0183Frame(t, DateTime.Now));
                if (f != null)
                {
                    Console.WriteLine("---------- AISFrame: " + f.Defn.Name + " -----------------");
                    Console.WriteLine("-> " + t);
                    foreach (AISField fi in f.Defn.Fields)
                    {
                        Console.WriteLine(fi.Name + " = " + fi.ToString(f.AISData));
                    }
                }
            }
            */

            // Finished
            toolStripStatusLabel.Text = "Ready.";
            menuStrip.Enabled = true;

            tabControl1.TabPages.Remove(tabPageGPSInfo);
            if (Displays != null)
            {
                tabControl1.TabPages.Remove(tabPageReadMe);
                showMenusToolStripMenuItem.Checked = !Settings.Default.HideMenusOnStart;
            }

            tabControl1.Visible = true;

            if (System.Net.HttpListener.IsSupported&&Settings.Default.StartWebServerLastState)
            {
                if (frmHTTP == null) frmHTTP = new HttpFormUserHtml();
            }

            if (!StartMinimized)
            {
                this.WindowState = FormWindowState.Normal;
                // We have to set form TopMost for while to get it visible with Visual Studio
                this.TopMost = true;
                this.TopMost = topmostToolStripMenuItem.Checked;
            }
        }

        void CreateLoggingMenus()
        {
            int ItemCount = 0;
            loggingToolStripMenuItem.DropDownItems.Clear();
            foreach (CANStreamer stream in StreamManager.Streams)
            {
                if ((stream is CANStreamer_Logger) && (((CANStreamer_Logger)stream).ShowOnMenu))
                {
                    ToolStripMenuItem NewDisplayItem = new ToolStripMenuItem(stream.Name);
                    loggingToolStripMenuItem.DropDownItems.Add(NewDisplayItem);
                    NewDisplayItem.Click += new System.EventHandler(this.mnuLoggingItem_Click);
                    NewDisplayItem.Checked = (stream.ConnectionState == ConnectionStateEnum.Connected);
                    ItemCount++;
                }
            }
            loggingToolStripMenuItem.Visible = (ItemCount > 0);
        }

        void LoadDefinitions()
        {
            using (new WaitCursor())
            {
                // First load all display definitions. This has to be done before loading parameter definition
                // or otherwise parameter definition has to be reloaded after loading displays.
                toolStripStatusLabel.Text = "Loading display definitions..."; statusStrip1.Refresh();
                LoadDisplays();

                // Load definitions
                toolStripStatusLabel.Text = "Loading NMEA 2000 Definitions..."; statusStrip1.Refresh();
                Definitions.LoadPGNDefns(Settings.Default.N2kPath);

                toolStripStatusLabel.Text = "Loading NMEA 0183 Definitions..."; statusStrip1.Refresh();
                Definitions.LoadN0183Defns(Settings.Default.N0183Path);

                toolStripStatusLabel.Text = "Loading AIS Definitions..."; statusStrip1.Refresh();
                Definitions.LoadAISDefns(Settings.Default.AISPath);

                toolStripStatusLabel.Text = "Loading Parameter Definitions..."; statusStrip1.Refresh();
                // Note : The extender is initialized on LoadFromFile.
                Definitions.LoadParameters(Settings.Default.ParametersPath);

                toolStripStatusLabel.Text = "Loading Streams from " + StreamManager.StreamsFileName; statusStrip1.Refresh();

                // Load streams
                StreamManager.LoadFromFiles(); // This handles reconnecting disconnected streams.
            }
        }

        void r_FrameCreated(Frame obj)
        {
            MessageBox.Show(obj.ToString() + obj.Header.ToString());
        }

        /*
        void Definitions_ParametersReloaded()
        {
            extender.Initialize(Definitions.ParamCol);
            lock (StreamManager.Streams)
            {
                foreach (CANStreamer stream in StreamManager.Streams)
                    extender.OnNewStream(stream);
            }
        }
        */

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Save
            StreamManager.SaveToFiles();
            
            // Time to exit the application
            // Streams that use threads can cause the application to stay alive
            // So we disconnect all streams (Which then will allow the application to exit cleanly)
            lock (StreamManager.Streams)
            {
                foreach (CANStreamer stream in StreamManager.Streams)
                {
                    if (stream.ConnectionState == ConnectionStateEnum.Connected)
                    {
                        stream.Disconnect();
                    }
                }
            }
        }

        private void ShowMenusAndStatus(Boolean show)
        {
            if (show)
            {
//                menuStrip.Dock = DockStyle.Top;
//                statusStrip1.Dock = DockStyle.Bottom;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Size = FullViewSize;
                menuStrip.Visible = true;
                statusStrip1.Visible = true;
            }
            else
            {
//                menuStrip.Dock = DockStyle.None;
//                statusStrip1.Dock = DockStyle.None;
                menuStrip.Visible = false;
                statusStrip1.Visible = false;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Size = CommonRoutines.FindMinSize(tabControl1.SelectedTab)
                         + new Size(tabControl1.Width - tabControl1.DisplayRectangle.Width, tabControl1.Height - tabControl1.DisplayRectangle.Height);
//                this.Height  = this.Height - menuStrip.Height - statusStrip1.Height;
            }
        }

        private void LoadDisplays()
        {
            // Load Display definitions
            ArrayList errors=new ArrayList();
            string DefDisplays="";
            string DisplayPath;

            try
            {
                DefDisplays = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.Displays);
                if (!File.Exists(DefDisplays)) return;

                if (Displays != null)
                {
                    if (!Displays.IsChanged(DefDisplays)) return;
                    Displays.ClearDisplays();
                    Displays = null;
                }
                Displays = DisplaysCollection.LoadFromFile(DefDisplays);
                if (Displays == null) return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not read display definition file " + DefDisplays+ ": "+ex.Message);
                return;
            }

//            KeysConverter converter = new KeysConverter();
//            ToolStripMenuItem NewDisplayItem = null; ;
            var loader = new AJMLoaderCode.AJMLoader();
            TabPage tabPage;

            int DisplayTag = 15001;
            FormWithComponents lastF = null ;
            loader.Assemblies = Displays.Assemblies;
            foreach (DisplayInfo display in Displays.Displays)
            {
                DisplayPath = display.DefinitionFileFullPath();
                if (!File.Exists(DisplayPath)) continue;

                bool success=false;
                switch (display.Type)
                {

                    case DisplayInfo.DisplayType.TabPage:
                      success = loader.ReadFileAsNewTabPage(DisplayPath, tabControl1, this.components, out errors, out tabPage);
                      display.Display = tabPage;
                      CommonRoutines.SetMouseDownHandlers(tabPage, StartMoveDisplay);
                      topmostToolStripMenuItem.Checked |= display.TopMost;
                      StartMinimized |= display.StartMinimized;
                      break;
                
                    case DisplayInfo.DisplayType.Form:
                        FormWithComponents f;
                        success = loader.ReadFile(DisplayPath, this.components, out errors, out f);
                        ActivateExtenders(f);
                        if (f != null)
                        {
                            f.DefLocation = display.DefLocation;
                            f.StartMinimized = display.StartMinimized;
                            f.Show();
                            f.displaysToolStripMenuItem=displaysToolStripMenuItem;
                            f.Icon = Icon;
                            f.TopMostState(display.TopMost);
                            lastF = f;
                        }
                        display.Display = f;
                        break;
                }
                if (!success) continue;

                display.AddToToolStrip(toolStripMenuItemDisplay, DisplayTag, this.mnuChangeDisplay_Click);
                display.AddToToolStrip(displaysToolStripMenuItem, DisplayTag, this.mnuChangeDisplay_Click);
//                if (lastF!=null) display.AddToToolStrip(lastF.MenuDisplaysToolStripMenuItem, DisplayTag, this.mnuChangeDisplay_Click);
                DisplayTag++;
            }

//            Displays.AddToToolStrip(displaysToolStripMenuItem);
//            Displays.AddToToolStrip(((FormWithComponents)lastF).displaysToolStripMenuItem);
        }

        private CANStreamer_Logger FindLoggerStreamByName(string Name)
        {
            foreach (CANStreamer stream in StreamManager.Streams)
            {
                if ((stream is CANStreamer_Logger) &&  (stream.Name == Name)) return ((CANStreamer_Logger)stream);
            }
            return null;
        }

        private void mnuLoggingItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CANStreamer_Logger stream = FindLoggerStreamByName(item.Text);
            if (stream != null)
            {
                if (item.Checked)
                {
                    stream.Disconnect();
                    item.Checked = false;
                }
                else
                {
                    stream.ConnectStream();
                    item.Checked = true;
                }
            }
        }

        private void ExtractResourceToFile(string resourceName, string filename)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            System.IO.Stream resource = asm.GetManifestResourceStream(resourceName);
            System.IO.Stream output = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            byte[] bytes = new byte[4096]; // 4K chunks  
            int bytesread = 0;
            while (0 < (bytesread = resource.Read(bytes, 0, bytes.Length))) {
                output.Write(bytes, 0, bytesread);
            }
            output.Close();
        }

        private void mnuChangeDisplay_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            Displays.ActivateByTag((int)item.Tag);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {
            // We first extract the help file from the resource
            try {
                string resourceName = "OpenSkipperApplication.Resources.Help.chm";
                string fileName = helpProvider1.HelpNamespace;
                if (!File.Exists(fileName)) {
                    ExtractResourceToFile(resourceName, fileName);
                }
                // See http://www.codeproject.com/KB/dotnet/HelpIntegrationInDotNet.aspx
                //Help.ShowHelp(this, helpProvider1.HelpNamespace, HelpNavigator.Index); //, HelpNavigator.TableOfContents);
                Help.ShowHelp(this, helpProvider1.HelpNamespace, HelpNavigator.TableOfContents, "Open Skipper Summary"); //, HelpNavigator.TableOfContents);
                //Help.ShowHelpIndex(this, helpProvider1.HelpNamespace); //, HelpNavigator.TableOfContents);
            }
            catch (Exception ex) {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void pGNDefinitionsToolStripMenuItem_Click(object sender, EventArgs e) {
            //Display frmPGNForm as a modal dialog
            //this.Hide();
            // frmPGNForm.ShowDialog();
            if (frmPGNForm == null) frmPGNForm = new PGNExplorerForm(Definitions.PGNDefnCol.FileName);
            frmPGNForm.ShowDialog(this);    // show as a modal form
            //this.Show();
        }

        /*private void ReadLogFile() {
            if (Packets == null) {
                try
                {
                    using (new WaitCursor())
                    {
                        Packets = new FrameCollection();
                        using (var LogFileStream = new StreamReader(LogFilePath))
                        {
                            if (LogFileType == LogFileTypeEnum.NativeXMLLogFile)
                            {
                                // Extremely slow, 1 hour log = many seconds to deserialize
                                Packets = FrameCollection.XmlDeserialize(LogFileStream);
                            }
                            else if (LogFileType == LogFileTypeEnum.KeesLogFile)
                            {
                                // Load the raw data from a Kees log file
                                Packets = KeesLogReader.ReadFrameCollection(LogFileStream);
                            }
                        }
                    }
                }
                catch
                {
                    Packets = null;
                    throw;
                }
            }
        }
        */

        /*private void ReadLogFileIntoTextBox()
        {
            // See http://stackoverflow.com/questions/265639/net-c-random-access-in-text-files-no-easy-way
            const int MaxLines = 1001;
            const int LinesFromHeader = 500;
            const int LinesFromTail = 100;
            const double SafetyFraction = 0.75; // Only aim to read in 75% of the last lines so we ensure file fits in MaxLines
            using (var LogFileStream = new StreamReader(LogFilePath)) {
                textBoxLogFile.Clear();
                // We start by reading in up to 1000 lines from the file
                var lines = new String[MaxLines];
                int i;
                for (i = 0; i < MaxLines; i++) {
                    if (LogFileStream.EndOfStream) break;
                    string s = LogFileStream.ReadLine();
                    lines[i] = s;
                }
                if (!LogFileStream.EndOfStream) {
                    // File not entirely read. Try to read in the end of the file
                    double AverageLineLength = (double)LogFileStream.BaseStream.Position / MaxLines;
                    LogFileStream.DiscardBufferedData();
                    long TailFilePosition = (long)(LogFileStream.BaseStream.Length - LinesFromTail * AverageLineLength * SafetyFraction);

                    // Count the number of lines from here to the end of the file
                    int TailLines = 3;
                    LogFileStream.BaseStream.Position = TailFilePosition;
                    LogFileStream.ReadLine(); // We skip the next line as it is probably a partial line
                    while (!LogFileStream.EndOfStream) {
                        LogFileStream.ReadLine();
                        TailLines++;
                    }
                    // Now read in the remainder of the file and put it into our lines
                    i = MaxLines - TailLines - 1;
                    if (i < LinesFromHeader) i = LinesFromHeader;   // We cannot fit all the tail... too bad
                    lines[i++] = "";
                    lines[i++] = "... (lines skipped) ...";
                    lines[i++] = "";
                    LogFileStream.BaseStream.Position = TailFilePosition;
                    LogFileStream.ReadLine(); // We skip the next line as it is probably a partial line
                    for (; i < MaxLines; i++) {
                        if (LogFileStream.EndOfStream) {
                            lines[i] = "";  // this should never happen
                        } else {
                            string s = LogFileStream.ReadLine();
                            lines[i] = s;
                        }
                    }
                    if (!LogFileStream.EndOfStream) {
                        lines[MaxLines - 1] = "File not completely displayed. (Full file will still be processed.)";
                    }
                }
                textBoxLogFile.Lines = lines;
            }
        }
        */

        private void toolStripButtonExplorePGNs_Click(object sender, EventArgs e) {
                pGNDefinitionsToolStripMenuItem_Click(sender, e);
        }

        private void actisenseNGT1USBSubItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnuSender = (ToolStripMenuItem)sender;
            Form actisenseForm = new NGT1(mnuSender.Text);
            actisenseForm.ShowDialog(this);
        }

        private void aboutDigitalBridgeToolStripMenuItem_Click(object sender, EventArgs e) {
            var about = new About();
            about.ShowDialog(this);
        }

        private void paramExplorertoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (frmParamForm == null)
                frmParamForm = new ParamExplorerForm(Definitions.ParamCol.FileName);

            frmParamForm.ShowDialog(this);
        }

        // Handler for menu COM port sub-items for NMEA 0183 menu
        void portItem_0183_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnuSender = (ToolStripMenuItem)sender;
            string portName = mnuSender.Text;
            
            // Item is checked/unchecked before this function fires
            if (mnuSender.Checked == false)
            {
                // Find stream to disconnect (So port is available again)
                CANStreamer stream = StreamManager.Streams.FirstOrDefault(str => (str is CANStreamer_N0183 && ((CANStreamer_COMPort)str).PortName == portName && str.ConnectionState == ConnectionStateEnum.Connected));
                if (stream != null)
                    stream.Disconnect();
            }
            else
            {
                // Check for existing disconnected stream with same name, which we will reconnect
                CANStreamer stream = StreamManager.Streams.FirstOrDefault(str => (str is CANStreamer_N0183 && ((CANStreamer_COMPort)str).PortName == portName && str.ConnectionState == ConnectionStateEnum.Disconnected));
                if (stream == null)
                {
                    // Adding new stream
                    stream = new CANStreamer_N0183() { PortName = portName, Name = portName };
                    StreamManager.AddStream(stream);
                }
                
                stream.ConnectStream();
            }
        }

        // Handler for menu COM port sub-items for Actisense NGT1-USB menu
        void portItem_NGT1_2000_Click(object sender, EventArgs e) {
            ToolStripMenuItem mnuSender = (ToolStripMenuItem)sender;
            string portName = mnuSender.Text;

            // Item is checked/unchecked before this function fires
            if (mnuSender.Checked == false) {
                // Find stream to disconnect (So port is available again)
                CANStreamer stream = StreamManager.Streams.FirstOrDefault(str => (str is CANStreamer_NGT1_2000 && ((CANStreamer_COMPort)str).PortName == portName && str.ConnectionState == ConnectionStateEnum.Connected));
                if (stream != null)
                    stream.Disconnect();
            } else {
                // Check for existing disconnected stream with same name, which we will reconnect
                CANStreamer stream = StreamManager.Streams.FirstOrDefault(str => (str is CANStreamer_NGT1_2000 && ((CANStreamer_COMPort)str).PortName == portName && str.ConnectionState == ConnectionStateEnum.Disconnected));
                if (stream == null) {
                    stream = new CANStreamer_NGT1_2000() { PortName = portName, Name = portName };
                    StreamManager.AddStream(stream);
                }

                stream.ConnectStream();
            }
        }

        /*
        this.Show();

        var col = new N0183Defncollection() {Version = "1.0", Comment = "Test", Date = DateTime.Now.ToString(), CreatorCode = "Jason" };
        var fs = new StreamReader(@"U:\Current\Boats\NMEA.txt");
        var fullText = fs.ReadToEnd();
        fs.Close();

        var regex = new Regex(@"=== (...) - ([^=\n]*) ===.+?\n ?(\$[^\n]*)\n-*(.+?)\nQQ", RegexOptions.Compiled | RegexOptions.Singleline);
        var fregex = new Regex(@"[0-9][0-9]?\. ([^\n]*)\n", RegexOptions.Compiled | RegexOptions.Singleline);
        
        MatchCollection Matches = regex.Matches(fullText);

        foreach (Match m in Matches)
        {
            MatchCollection FieldMatches = fregex.Matches(m.Groups[4].Value);

            string Code = m.Groups[1].Value;
            string Desc = m.Groups[2].Value;
            string Format = m.Groups[3].Value;
            string[] Fields = Format.Split(",*".ToCharArray());

            N0183Defn newDefn = new N0183Defn();
            newDefn.Code = Code;
            newDefn.Description = Desc;
            
            for (int i = 1; i < Fields.Length - 1; i++)
            {
                newDefn.AddField(new N0183Field() { DisplayName = FieldMatches[i-1].Groups[1].Value, Description = Fields[i], FieldIndex = i - 1});
            }

            col.AddN0183Defn(newDefn);
        }

        var xmls = new XmlSerializer(typeof(N0183Defncollection));
        var fr = new StreamWriter(@"U:\Test2.xml");
        xmls.Serialize(fr, col);
        fr.Close();
        */

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (frmN0183Form == null)
                frmN0183Form = new N0183ExplorerForm(Definitions.N0183DefnCol.FileName);

            frmN0183Form.ShowDialog(this);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmSettingsForm == null)
                frmSettingsForm = new SettingsForm();

            frmSettingsForm.ShowDialog(this);
        }

        private void keesFileConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog oDialog = new OpenFileDialog();
            oDialog.Filter = Constants.KeesXMLPGNDefnFileFilter;
            oDialog.Title = "Select Kees PGN definition file";
            if ((oDialog.ShowDialog() == DialogResult.OK) && (oDialog.CheckFileExists == true))
            {
                // Vars
                KeesPGNDefnCollection KeesDefinitions;
                PGNDefnCollection newPGNDefnCol;

                // Could take a while, use wait cursor
                using (new WaitCursor())
                {
                    // Open 
                    using (StreamReader r = new StreamReader(oDialog.FileName))
                    {
                        // Deserialising from Kee's XML file for creating PGNDefnCollection Class
                        KeesDefinitions = KeesPGNDefnCollection.XmlDeserialize(r);
                        newPGNDefnCol = new PGNDefnCollection()
                                            {
                                                Version = KeesDefinitions.Version,
                                                Date = KeesDefinitions.Date,
                                                Comment = KeesDefinitions.Comment,
                                                CreatorCode = KeesDefinitions.CreatorCode,
                                                PGNDefns = KeesDefinitions.GetPGNInfos(), // Does the decoding
                                                FileName = Path.ChangeExtension(oDialog.FileName, null),  // We remove both extensions
                                            };
                    }
                }

                SaveFileDialog sDialog = new SaveFileDialog();
                sDialog.Filter = Constants.XMLPGNDefnFileFilter;
                sDialog.Title = "Save converted PGN definitions";
                if (sDialog.ShowDialog() == DialogResult.OK)
                {
                    newPGNDefnCol.SaveToFile(sDialog.FileName);
                }
            }
        }

        private void decodedStreamsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmDecodedLogForm == null)
                frmDecodedLogForm = new DecodedLogForm();

            frmDecodedLogForm.Show(this);
        }

        private void mnuAdvanced_Click(object sender, EventArgs e)
        {
            if (frmStreamManagerForm == null)
                frmStreamManagerForm = new StreamViewForm();

            frmStreamManagerForm.ShowDialog(this);
            CreateLoggingMenus();
        }

        private void toolStripMenuCANDevices_Click(object sender, EventArgs e)
        {
            if (frmCANDevices == null)
                frmCANDevices = new CANDevicesForm();

            frmCANDevices.ShowDialog(this);
        }

        private void mnuOpenLogfile_Click(object sender, EventArgs e)
        {
            CANStreamer_Logfile newStream = CANStreamer_Logfile.OpenNew();
            if (newStream != null)
            {
                StreamManager.AddStream(newStream);
                newStream.ConnectStream();
            }
        }

        private void viewLogfileMenuItem_Click(object sender, EventArgs e)
        {
            CANStreamer_Logfile newStream = CANStreamer_Logfile.OpenNew();
            if (newStream != null)
            {
                DecodedLogForm decodedLogForm = new DecodedLogForm(true);
                decodedLogForm.Show(this);

                newStream.FrameReceived += decodedLogForm.FrameReceived;
                newStream.SendAllFrames();

                /*
               IncomingMessageHandler messageHandler = new IncomingMessageHandler();
               Frame nextPacket;

               while ((nextPacket = newStream.getNextPacket()) != null)
               {
                   Frame decodedPacket = messageHandler.DecodeMessage(nextPacket);

                   // If it was part of a fastpacket message, returned message is null, check for this.
                   if (decodedPacket != null)
                       decodedLogForm.FrameReceived(decodedPacket);
               }
               */
            }
        }

        /*private void mnuStopLogging_Click(object sender, EventArgs e)
        {
            logger.StopLogging();
            logger = null;
            mnuStopLogging.Enabled = false;
        }*/

        private void mnuHTTP_Click(object sender, EventArgs e)
        {
            if (!System.Net.HttpListener.IsSupported)
            {
                MessageBox.Show("Operating system not supported, Windows XP SP2 or Server 2003 or later required", "HTTP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (frmHTTP == null)
                frmHTTP = new HttpFormUserHtml();
            //frmHTTP = new HttpGenForm();
            //frmHTTP = new HTTPForm();

            frmHTTP.ShowDialog();
        }

        private void actisenseNGT1USBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form actisenseForm = new NGT1("");
            actisenseForm.Show();// Dialog(this);
        }

        private void mnuDebugConsole_Click(object sender, EventArgs e)
        {
            if (frmDebugConsole == null || frmDebugConsole.IsDisposed || frmDebugConsole.Disposing)
                frmDebugConsole = new DebugConsole();

            frmDebugConsole.Show();
        }

        private void mnuAISDefns_Click(object sender, EventArgs e)
        {
            if (frmAisExplorer == null)
                frmAisExplorer = new AISExplorerForm(Definitions.AISDefnCol.FileName);

            frmAisExplorer.ShowDialog();
        }

        private void mnuComPort2000_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Form frmHttpGenForm = new HttpFormUserHtml();
            frmHttpGenForm.ShowDialog();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        // We always build this list when required to allow newly added COM ports to show up
        {
            mnuComPort2000.DropDownItems.Clear();
            mnuComPort0183.DropDownItems.Clear();

            // Set up menus here
            string[] portNames = SerialPort.GetPortNames();
            if (portNames.Length > 0)
            {
                foreach (string portName in portNames)
                {
                    // We show only ports, which are not in use.
                    if (!CommonRoutines.TestIsPortFree(portName)) continue;

                    ToolStripMenuItem portItem1 = new ToolStripMenuItem() { Text = portName, CheckOnClick = true };
                    portItem1.Click += new EventHandler(portItem_NGT1_2000_Click);
                    mnuComPort2000.DropDownItems.Add(portItem1);

                    ToolStripMenuItem portItem2 = new ToolStripMenuItem() { Text = portName, CheckOnClick = true };
                    portItem2.Click += new EventHandler(portItem_0183_Click);
                    mnuComPort0183.DropDownItems.Add(portItem2);

                    if (StreamManager.Streams.FirstOrDefault(s => (s is CANStreamer_NGT1_2000 && ((CANStreamer_COMPort)s).PortName == portName) && (s.ConnectionState == ConnectionStateEnum.Connected)) != null)
                    {
                        portItem1.Checked = true;
                    }
                    if (StreamManager.Streams.FirstOrDefault(s => (s is CANStreamer_N0183 && ((CANStreamer_COMPort)s).PortName == portName) && (s.ConnectionState == ConnectionStateEnum.Connected)) != null) {
                        portItem2.Checked = true;
                    }
                }
            }
            else
            {
                // No ports, disable menu entries
                mnuComPort2000.Enabled = false;
                mnuComPort0183.Enabled = false;
            }
        }

        private void toolStripMenuItem1_DropDownOpening(object sender, EventArgs e)
        {
            return;
            // Set up menus here
/*            string[] portNames = SerialPort.GetPortNames();
            if (portNames.Length > 0)
            {
                actisenseNGT1USBToolStripMenuItem.DropDownItems.Clear();

                foreach (string portName in portNames)
                {
                    ToolStripItem actisenseItem = new ToolStripMenuItem() { Text = portName };
                    actisenseItem.Click += new EventHandler(actisenseNGT1USBSubItem_Click);
                    actisenseNGT1USBToolStripMenuItem.DropDownItems.Add(actisenseItem);
                }
            }
            else
            {
                // No ports, disable menu entries
                actisenseNGT1USBToolStripMenuItem.Enabled = false;
            }
*/
        }

        private void ActivateExtenders(Form f)
        {
            // We need to activate the extender after the load has completed
            var f2 = f as FormWithComponents;
            if (f2 != null && f2.Components != null)
            {
                foreach (var c in f2.Components.Components)
                {
                    if (c is ParameterExtender)
                    {
                        (c as ParameterExtender).InitialiseExtender();
                    }
                }
            }
        }

        private void loadFormToolStripMenuItem_Click(object sender, EventArgs e) {
            string fileName = null;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "xml";
            dlg.Filter = "Xml Files|*.xml";
            if (dlg.ShowDialog() == DialogResult.OK)
                fileName = dlg.FileName;

            if (fileName == null)
                return;
            var loader = new AJMLoaderCode.AJMLoader();
            ArrayList errors;
            FormWithComponents f;
            loader.ReadFile(fileName,  this.components, out errors, out f);
            ActivateExtenders(f);
            if (f != null) f.Show();
            return;
        }

        private void loadTabPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = null;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "xml";
            dlg.Filter = "Xml Files|*.xml";
            if (dlg.ShowDialog() == DialogResult.OK)
                fileName = dlg.FileName;

            if (fileName == null)
                return;
            var loader = new AJMLoaderCode.AJMLoader();
            ArrayList errors;
            bool success = loader.ReadFileAsNewTabPage(fileName, tabControl1, this.components, out errors);
            return;
        }

        private void removeTabPageToolStripMenuItem_Click(object sender, EventArgs e) {
            this.tabControl1.TabPages.Remove(this.tabControl1.SelectedTab);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close(); // Application.Exit();
        }

        // Following methods add support for running full screen, in which case we show or hide the menu 

//        private void panelMenuPopup_MouseEnter(object sender, EventArgs e) {
//           //tabControl1.Top = menuStrip.Top + menuStrip.Height;
////            menuStrip.Show();
//            menuStrip.Height = 24;
//            //tabControl1.Dock = DockStyle.Fill;
//        }

        bool _menuActivated = false;
        bool _mouseInMenu = false;
//        bool _mouseInMenuPopupLabel = false;

        private void menuStrip_MenuDeactivate(object sender, EventArgs e) {
            if (!_doFullScreen) return;
            if (!_mouseInMenu) menuStrip.Height = 1; //  menuStrip.Visible = false;
            _menuActivated = false;
        }

        private void menuStrip_MouseLeave(object sender, EventArgs e) {
            _mouseInMenu = false;
            if (!_menuActivated) menuStrip_MenuDeactivate(sender, e);
        }

        private void menuStrip_Leave(object sender, EventArgs e) {
            menuStrip_MenuDeactivate(sender, e);
        }

        private void menuStrip_MenuActivate(object sender, EventArgs e) {
            menuStrip.Height = 24;
            _menuActivated = true;
            _mouseInMenu = true;
        }

        private void menuStrip_MouseEnter(object sender, EventArgs e) {
            menuStrip.Height = 24;
            _mouseInMenu = true;
        }

        int _oldFullScreenHeight;
        FormBorderStyle _oldFormBorderStyle;
        int _oldTabControlHeight;
        int _oldFormHeight, _oldFormWidth;

        private void SetFullScreen(bool doFullScreen) {
            if (doFullScreen == _doFullScreen) return;
            _doFullScreen = doFullScreen;
            if (doFullScreen) {
                _oldFullScreenHeight = menuStrip.Height;
                _oldFormBorderStyle  = this.FormBorderStyle;
                _oldTabControlHeight = this.tabControl1.Height;
                _oldFormHeight = this.Height;
                _oldFormWidth = this.Width;
                menuStrip.Height = 1;
                this.FormBorderStyle = FormBorderStyle.None;
                this.HelpButton = false;
                int oldTop = tabControl1.Top;
                this.tabControl1.Top = 1;
                tabControl1.Height += oldTop - 1;
                // this.TopMost = true;
                this.WindowState = FormWindowState.Maximized;  // Must follow FormBorderStyle = FormBorderStyle.None;
            } else {
                menuStrip.Height = _oldFullScreenHeight;
                this.FormBorderStyle = _oldFormBorderStyle;
                this.HelpButton = true;
                // this.TopMost = true;
                this.WindowState = FormWindowState.Normal;  // Must follow FormBorderStyle = FormBorderStyle.None;
                this.Height = _oldFormHeight;
                this.Width = _oldFormWidth;
                this.tabControl1.Height = _oldTabControlHeight;
                this.tabControl1.Top = menuStrip.Height;
            }
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e) {
            SetFullScreen(!_doFullScreen);
        }

        private void showGPSInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showGPSInfoToolStripMenuItem.Checked)
            {
                tabControl1.TabPages.Insert(0,tabPageGPSInfo);
            }
            else
            {
                tabControl1.TabPages.Remove(tabPageGPSInfo);
            }
        }

        private void showMenusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            ShowMenusAndStatus(mi.Checked);
            showMenusToolStripMenuItem1.CheckState  = mi.CheckState;
            showMenusToolStripMenuItem.CheckState = mi.CheckState;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (menuStrip.Visible) FullViewSize = (this.WindowState == FormWindowState.Normal ? this.Size : this.RestoreBounds.Size);
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (!menuStrip.Visible) {
                this.Size = CommonRoutines.FindMinSize(tabControl1.SelectedTab)
                         + new Size(tabControl1.Width - tabControl1.DisplayRectangle.Width, tabControl1.Height - tabControl1.DisplayRectangle.Height);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Displays!=null) Displays.ClearDisplays(); // we need to force user forms to be cleared, so that positions will be saved.
            Settings.Default.Position = this.Location;
            Settings.Default.Size = FullViewSize;
            Settings.Default.Save();
        }

        private void reloadDisplaysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (new WaitCursor())
            {
                tabControl1.Visible = false;
                LoadDisplays();
                ShowMenusAndStatus(showMenusToolStripMenuItem.Checked);
                tabControl1.Visible = true;
            }
        }

        private void mimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ti = (ToolStripMenuItem)sender;
            ContextMenuStrip cm=(ContextMenuStrip)(ti.Owner);
            Form caller=(Form)(cm.SourceControl);
            caller.WindowState = FormWindowState.Minimized;
        }

        private void StartMoveDisplay(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                CommonRoutines.ReleaseCapture();
                CommonRoutines.SendMessage(Handle, CommonRoutines.WM_NCLBUTTONDOWN, CommonRoutines.HT_CAPTION, 0);
            }
        }

        private void TopMostChanged(object sender, EventArgs e)
        {
            this.TopMost = topmostToolStripMenuItem.Checked;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {

        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            contextMenu_Main.Items.Add(displaysToolStripMenuItem);
        }

    }

    public class ParameterLink
    {
        private DateTime _LastTimeStamp;
        private Parameter.ParameterStateEnum _LastState;
        private double _LastValue;

        public readonly Control control;
        public readonly Parameter parameter;
        public readonly PropertyInfo propertyInfo;
        public readonly object[] index;
        public readonly double ForceUpdate;
        public readonly string[] StreamsToMatch;

        public ParameterLink(Control control, Parameter parameter, PropertyInfo propertyInfo, object[] index, string StreamsToMatch)
        {
            this.control = control;
            this.parameter = parameter;
            this.propertyInfo = propertyInfo;
            this.index = index;
            this.StreamsToMatch = StreamsToMatch.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            _LastTimeStamp = DateTime.Now;
            _LastState = Parameter.ParameterStateEnum.IsError;
            _LastValue = 0;
            PropertyInfo fuProp = control.GetType().GetProperty("ForceUpdate");
            ForceUpdate = ( (fuProp == null) ? Double.NaN : (double)fuProp.GetValue(control));
        }

        public bool MatchesStream(CANStreamer stream)
        {
            return StreamsToMatch.Contains("Any") || StreamsToMatch.Contains(stream.Name); 
            //foreach (string streamMatch in StreamsToMatch)
            //    if (streamMatch == "Any" || streamMatch == stream.Name)
            //        return true;
            //return false;
        }

        public void UpdateControl()
        {
            // The casting code here is because SetValue will throw an exception if the value cannot be implicitly converted.
            // Note: Dynamic casting is possible, but problematic (+ property type is limited to these types anyway (via extender checking))
            object valueToSet;

            NumericParameter NumParam = parameter as NumericParameter;

            if ((parameter.State() == _LastState) &&
                (Double.IsNaN(ForceUpdate) || (DateTime.Now < _LastTimeStamp.AddSeconds(ForceUpdate)) ) &&
                ((NumParam!=null) && (_LastValue==NumParam.ToDouble()) || (parameter.LastTimeStamp() == _LastTimeStamp))
                ) return;

            _LastState = parameter.State();
            _LastTimeStamp = parameter.LastTimeStamp();
            if (NumParam != null) _LastValue = NumParam.ToDouble();

//            return;
            switch (parameter.State())
            {
                case Parameter.ParameterStateEnum.NoDataReceived:
                case Parameter.ParameterStateEnum.IsNotAvailable:
                    control.Enabled = false;
                    control.Text = parameter.InternalName; // control.GetType().ToString();
                    break;
                case Parameter.ParameterStateEnum.ValidValueReceived:
                    control.Enabled = true;
                    control.BackColor = default(Color);
                    //control.UseVisualStyleBackColor = true;
                    if (propertyInfo.PropertyType == typeof(String)) valueToSet = parameter.ToString();
                    else if (propertyInfo.PropertyType == typeof(Decimal)) valueToSet = (Decimal)((IToDouble)parameter).ToDouble();
                    else if (propertyInfo.PropertyType == typeof(Single)) valueToSet = (Single)((IToDouble)parameter).ToDouble();
                    else if (propertyInfo.PropertyType == typeof(int)) valueToSet = (int)((IToDouble)parameter).ToDouble();
                    else valueToSet = ((IToDouble)parameter).ToDouble();

                    propertyInfo.SetValue(control, valueToSet, index);
                    break;
                case Parameter.ParameterStateEnum.IsError:
                    //control
                    control.BackColor = Color.Red;
                    //propertyInfo.SetValue(control, valueToSet, null);
                    break;
                case Parameter.ParameterStateEnum.Lost:
                    //control
                    control.BackColor = Color.Yellow;
                    //propertyInfo.SetValue(control, valueToSet, null);
                    break;
            }
        }
    }

}
