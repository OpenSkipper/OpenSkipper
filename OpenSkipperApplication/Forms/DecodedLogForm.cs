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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CANHandler;
using System.Collections;
using System.IO;
using CANStreams;
using OpenSkipperApplication.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;

namespace OpenSkipperApplication {

    public partial class DecodedLogForm : Form
    {
        // Our form for selecting which streams to show
        private Form frmSelectStreams;

        // List of frames at each 'stage'
        private List<Frame> PendingMsgs; // Holds messages until we are ready to process
        private Queue<Frame> DecodedMsgs; // Holds every processed message
        private List<Frame> FilteredMsgs; // Holds filtered messages (Subset of DecodedMsgs)
        private List<Frame> DisplayedMsgs; // Holds messages currently displayed (Contains all items in FilteredMsgs, but rearranged)
        
        private List<CompareBase> _Filters;
        private List<CompareBase> Filters
        {
            get
            {
                return _Filters;
            }
            set
            {
                _Filters = value;
                Filterer = new FrameComparer(value);
            }
        }
        private FrameComparer Filterer;

        private List<CompareBase> _Sorters;
        private List<CompareBase> Sorters
        {
            get
            {
                return _Sorters;
            }
            set
            {
                _Sorters = value;
                Sorter = new FrameComparer(value);
            }
        }
        private FrameComparer Sorter;

     //   private Frame SelectedFrame;

        private uint maxFrames;
        private bool ignoreSelectionChanges = false;
        private bool firstShow = true;
        private bool fadeToShowMessageAge = true;
        private bool useFilter;

        // Constructor
        public DecodedLogForm()
            : this(false)
        {
        }
        public DecodedLogForm(bool singleStreamMode)
        {
            InitializeComponent();

            maxFrames = OpenSkipperApplication.Properties.Settings.Default.MaxFrames;
            txtMaxFrames.Text = maxFrames.ToString();

            PendingMsgs = new List<Frame> { };
            DecodedMsgs = new Queue<Frame> { };
            FilteredMsgs = new List<Frame> { };
            DisplayedMsgs = new List<Frame> { };

            if (!singleStreamMode)
            {
                useFilter = false;
            }
            else
            {
                // Disable stream selection, we only have 1 stream in this mode.
                selectStreamsToolStripMenuItem.Enabled = false;

                firstShow = false;
                fadeToShowMessageAge = false;
            }

            Filter_Click(this,null);
            // By default we filter by header and instance
//            uniqueHeadersAndEnumFieldsToolStripMenuItem.Checked = true;
//            useFilter = true;
//            Filters = new List<CompareBase> { CompareBase.Create("Header"), CompareBase.Create("EnumFields"), CompareBase.Create("InstanceFields") };

            // By default we sort by PGN
            Sorters = new List<CompareBase> { CompareBase.Create("ID") };
        }

        private void DecodedLogForm_Load(object sender, EventArgs e)
        {
            // Enable virtual mode.
            this.dataGridView1.VirtualMode = true;
            this.dataGridView1.AllowUserToAddRows = false;  // This seems to be staying as true even tho it is false in the designer
            
            // Disable any menus not relevant for this playing mode
            fadeToShowMessageAgeToolStripMenuItem.Checked = fadeToShowMessageAge;

            UpdateDisplay();

            // For first showing of form, we should pop up the stream selector before continuing (Showing the decoded stream form)
            if (firstShow)
            {
                if (frmSelectStreams == null)
                    frmSelectStreams = new StreamSelecterForm(new StreamSelecterForm.StreamSelectionHandler(StreamSelectionChanged));

                frmSelectStreams.Show(this);
                firstShow = false;
            }
        }

        private void StreamSelectionChanged(object sender, EventArgs e, CANStreamer stream, bool selected)
        {
            if (selected == true)
                stream.FrameReceived += FrameReceived;
            else
                stream.FrameReceived -= FrameReceived;
        }

        private void UpdateDisplay() {
            // dataGridView1.Rows.Clear();
            dataGridView1.RowCount = DisplayedMsgs.Count + 1; // there is an extra "add a new row" that I cannot remove
            dataGridView1.Refresh();
            toolStripStatusLabel1.Text = DisplayedMsgs.Count + " records.";
            UpdateDetailedTextViews();
            if (fadeToShowMessageAge) UpdateFading();
        }

        void UpdateFading()
        {
            if (fadeToShowMessageAge && (DisplayedMsgs.Count > 0))
            {
                DateTime recentDate = DisplayedMsgs[0].TimeStamp;
                for (int i = 1; i < DisplayedMsgs.Count; i++)
                    if (DisplayedMsgs[i].TimeStamp > recentDate)
                        recentDate = DisplayedMsgs[i].TimeStamp;

                for (int i = 0; i < DisplayedMsgs.Count; i++)
                {
                    TimeSpan age = recentDate - DisplayedMsgs[i].TimeStamp;
                    int shade = (age.TotalSeconds > 5) ? 230 : (int)((age.TotalSeconds / 5) * 230);
                    this.dataGridView1.Rows[i].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(shade, shade, shade);
                }
            }
        }

        // Request a screen update of the GridViews and ProopertyGrid
        private void UpdateDetailedTextViews()
        {
            int selectedIndex = (dataGridView1.SelectedRows.Count > 0) ? dataGridView1.SelectedRows[0].Index : -1;
            if (selectedIndex >= 0 && selectedIndex < DisplayedMsgs.Count)
            {
                Frame SelectedFrame = DisplayedMsgs[selectedIndex];

                richTextBoxPGNDebugView.Text = SelectedFrame.Defn.ToDebugString(SelectedFrame);
                richTextBoxPGNDebugView.Refresh();

                richTextBoxPGNView.Text = SelectedFrame.Defn.ToString(SelectedFrame);
            }
            else
            {
                richTextBoxPGNView.Text = "";
                richTextBoxPGNDebugView.Text = "";
            }
        }

        private readonly object pendingLock = new object();
        public void FrameReceived(object sender, FrameReceivedEventArgs e)
        {
            lock (pendingLock)
            {
                PendingMsgs.Add(e.ReceivedFrame);
            }
        }

        public void ProcessPendingFrames()
        {
            // Get the new messages - The lock ensures that there won't be any threads that write to the old list, after the switch has been done (If switch occurred mid-add)
            List<Frame> MsgsToDisplay;
            lock (pendingLock)
            {
                // Check if there are any pending, if not then don't touch anything, just return.
                if (PendingMsgs.Count == 0)
                    return;

                // Switch out pending messages for an empty list. Pending messages become our local messages to process
                MsgsToDisplay = PendingMsgs;
                PendingMsgs = new List<Frame> { };
            }

            // Remember selection
            int oldIdx = (dataGridView1.SelectedRows.Count > 0) ? dataGridView1.SelectedRows[0].Index : -1;
            Frame selFrame = (oldIdx >= 0 && oldIdx < DisplayedMsgs.Count) ? DisplayedMsgs[oldIdx] : null;
            ignoreSelectionChanges = true;

            // Process all pending frames
            foreach (Frame decodedFrame in MsgsToDisplay)
            {
                // Store in decoded messages.
                DecodedMsgs.Enqueue(decodedFrame);

                if (DecodedMsgs.Count > maxFrames)
                {
                    Frame droppedFrame = DecodedMsgs.Dequeue();
                    FilteredMsgs.Remove(droppedFrame);
                    DisplayedMsgs.Remove(droppedFrame);
                }

                // Processing handles the filtering/sorting/displaying.
                ProcessMsg(decodedFrame);
            }

            // Update grid row count
            if (dataGridView1.RowCount != DisplayedMsgs.Count)
                dataGridView1.RowCount = DisplayedMsgs.Count;

            // Reselect
            if (selFrame != null)
            {
                Frame newSelFrame = (dataGridView1.SelectedRows.Count > 0) ? DisplayedMsgs[dataGridView1.SelectedRows[0].Index] : null;
                if (!object.ReferenceEquals(newSelFrame, selFrame))
                {
                    // int newIdx = DisplayedMsgs.IndexOf(selFrame);
                    int newIdx = DisplayedMsgs.FindIndex(f => (Filterer.Compare(f, selFrame) == 0));
                    if (newIdx >= 0)
                    {
                        
                        dataGridView1.Rows[oldIdx].Selected = false;
                        dataGridView1.Rows[newIdx].Selected = true;
                        
                    }
                }
            }

            ignoreSelectionChanges = false;
            UpdateDisplay();
        }

        private void ProcessMsg(Frame msg)
        {
            if (useFilter)
            {
                int index = FilteredMsgs.BinarySearch(msg, Filterer);
                if (index < 0)
                {
                    // Item is a new. Add to filtered
                    FilteredMsgs.Insert(~index, msg);

                    // Also add item to displayed.
                    int index2 = DisplayedMsgs.BinarySearch(msg, Sorter);
                    DisplayedMsgs.Insert(~index, msg);
                }
                else
                {
                    // Item was already in filter. Replace it.
                    Frame oldFrame = FilteredMsgs[index];
                    FilteredMsgs[index] = msg;

                    // Search for it in displayedMsgs.

                    // TODO : Fix this. Binary search doesnt work for some strange reason....
                    //int index2 = DisplayedMsgs.BinarySearch(oldFrame, Sorter);
                    int index2 = DisplayedMsgs.IndexOf(oldFrame);

                    // Replace it
                    DisplayedMsgs.RemoveAt(index2);
                    DisplayedMsgs.Insert(index2, msg);
                }
            }
            else
            {
                int index = DisplayedMsgs.BinarySearch(msg, Sorter);
                int index2 = index < 0 ? ~index : index;

                DisplayedMsgs.Insert(index2, msg);
            }
        }

        /// <summary>
        /// Refreshes filtered and displayed messsages from scratch, based on current filters/sorters.
        /// </summary>
        private void ResetFiltered()
        {
            // Remember selection
            int oldIdx = (dataGridView1.SelectedRows.Count > 0) ? dataGridView1.SelectedRows[0].Index : -1;
            Frame selFrame = (oldIdx >= 0 && oldIdx < DisplayedMsgs.Count) ? DisplayedMsgs[oldIdx] : null;
            ignoreSelectionChanges = true;

            FilteredMsgs = new List<Frame> { };
            DisplayedMsgs = new List<Frame> { };
            dataGridView1.RowCount = 0;

            foreach (Frame decodedFrame in DecodedMsgs)
            {
                ProcessMsg(decodedFrame);
            }

            // Update grid row count
            if (dataGridView1.RowCount != DisplayedMsgs.Count)
                dataGridView1.RowCount = DisplayedMsgs.Count;

            // Reselect
            if (selFrame != null)
            {
                // int newIdx = DisplayedMsgs.IndexOf(selFrame);
                int newIdx = DisplayedMsgs.FindIndex(f => (Filterer.Compare(f, selFrame) == 0));
                if (newIdx >= 0)
                    dataGridView1.Rows[newIdx].Selected = true;
            }

            ignoreSelectionChanges = false;
            UpdateDisplay();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (!ignoreSelectionChanges)
                UpdateDetailedTextViews();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ProcessPendingFrames();
        }

        private void dataGridView1_CellValueNeeded(object sender, System.Windows.Forms.DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            if (e.RowIndex == this.dataGridView1.RowCount - 1) return;

            // Set the cell value to paint using the Customer object retrieved.
            if (e.RowIndex >= 0 && e.RowIndex < DisplayedMsgs.Count) {

                Frame msg = DisplayedMsgs[e.RowIndex];
                switch (this.dataGridView1.Columns[e.ColumnIndex].Name)
                {
                    case "ID": e.Value = msg.Header.Identifier; break;
                    case "NameColumn": e.Value = msg.Defn.Name; break;
                    case "Description": e.Value = msg.Defn.Description; break;
                    case "Source": e.Value = msg.Header.Source; break;
                    case "Time": e.Value = msg.TimeStamp.ToString(); break;
                    case "Destination": e.Value = msg.Header.Destination; break;
                    case "Priority": e.Value = msg.Header.Priority; break;
                    case "Length": e.Value = msg.Length; break;
                    case "Fields": e.Value = msg.Defn.FieldsString(msg); break;
                    case "EnumFields": e.Value = msg.Defn.EnumFieldsString(msg); break;
                }
            }
        }

        private void mnuShowAll_Click(object sender, EventArgs e)
        {
            if (!mnuShowAll.Checked)
            {
                mnuShowAll.Checked = true;
                return;
            }

            foreach (ToolStripItem child in viewToolStripMenuItem.DropDownItems)
            {
                string childTag = (string)child.Tag;
                if ((childTag != null) &&
                    (child.GetType() == typeof(ToolStripMenuItem))
                   )
                {
                    ((ToolStripMenuItem)child).Checked = false;
                }
            }

            useFilter = false;

            ResetFiltered();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            string ColumnName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (Sorters.Count == 0) {
                // First click on any column; we simplky sort by this column
                Sorters.Add(CompareBase.Create(ColumnName));
            } else if (Sorters[0].Name==ColumnName) {
                // This is already main sort column; reverse its order
                Sorters[0].Sign *= -1;
            } else {
                // Delete any existing sorting on this column
                CompareBase comparer = Sorters.Find(delegate(CompareBase p) { return p.Name == ColumnName;} );
                if (comparer != null) Sorters.Remove(comparer); 
                Sorters.Insert(0,CompareBase.Create(ColumnName));
            }

            using (new WaitCursor())
            {
                int oldIdx = (dataGridView1.SelectedRows.Count > 0) ? dataGridView1.SelectedRows[0].Index : -1;
                Frame selFrame = (oldIdx >= 0 && oldIdx < DisplayedMsgs.Count) ? DisplayedMsgs[oldIdx] : null;
                dataGridView1.Rows[oldIdx].Selected = false;

                DisplayedMsgs.Sort(Sorter);

                if (selFrame != null)
                {
                    // Update grid row count
                    if (dataGridView1.RowCount != DisplayedMsgs.Count)
                        dataGridView1.RowCount = DisplayedMsgs.Count;

                    dataGridView1.Rows[DisplayedMsgs.IndexOf(selFrame)].Selected = true;
                }

                UpdateDisplay();
            }
        }

        private void DecodedLogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void fadeToShowMessageAgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fadeToShowMessageAge = !fadeToShowMessageAge;
            fadeToShowMessageAgeToolStripMenuItem.Checked = fadeToShowMessageAge;

            if (fadeToShowMessageAge)
            {
                UpdateFading();
            }
            else
            {
                // Fading was turned off, then set everything back to black
                for (int i = 0; i < DisplayedMsgs.Count; i++)
                {
                    this.dataGridView1.Rows[i].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        private void selectStreamsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmSelectStreams == null)
                frmSelectStreams = new StreamSelecterForm(new StreamSelecterForm.StreamSelectionHandler(StreamSelectionChanged));

            frmSelectStreams.Show(this);
        }

        private void viewLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CANStreamer_Logfile newStream = CANStreamer_Logfile.OpenNew();
            if (newStream != null)
            {
                DecodedLogForm decodedLogForm = new DecodedLogForm(true);
                decodedLogForm.Show(this);

                newStream.FrameReceived += decodedLogForm.FrameReceived;
                newStream.SendAllFrames();
            }
        }

        private void DecodedLogForm_VisibleChanged(object sender, EventArgs e)
        {
            timer1.Enabled = this.Visible;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog oDialog = new SaveFileDialog();
            oDialog.Filter = "XML Log File (*.xml)|*.xml";
            if (oDialog.ShowDialog() == DialogResult.OK)
            {
                CANStreamer_Logger logger = new CANStreamer_Logger() { FileName = oDialog.FileName, Overwrite = true };

                logger.ConnectStream();

                foreach (Frame f in DecodedMsgs)
                    logger.SendFrame(f);

                logger.Disconnect();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void txtMaxFrames_Validating(object sender, CancelEventArgs e)
        {
            uint v;
            if (!uint.TryParse(txtMaxFrames.Text, out v))
                e.Cancel = true;
        }

        private void txtMaxFrames_Validated(object sender, EventArgs e)
        {
            maxFrames = uint.Parse(txtMaxFrames.Text);
            OpenSkipperApplication.Properties.Settings.Default.MaxFrames = maxFrames;
            OpenSkipperApplication.Properties.Settings.Default.Save();

            long framesToRemove = DecodedMsgs.Count - maxFrames;
            for (long i = 0; i < framesToRemove; i++)
            {
                Frame droppedFrame = DecodedMsgs.Dequeue();
                FilteredMsgs.Remove(droppedFrame);
                DisplayedMsgs.Remove(droppedFrame);
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            mnuPause.Text = timer1.Enabled ? "Pause" : "Resume";
        }

        private void Filter_Click(object sender, EventArgs e)
        {
            useFilter = false;
            Filters = new List<CompareBase> { };
            foreach (ToolStripItem child in viewToolStripMenuItem.DropDownItems)
            {
                string childTag = (string)child.Tag;
                if ((childTag != null) && 
                    (child.GetType()==typeof(ToolStripMenuItem)) && 
                    (((ToolStripMenuItem)child).Checked)
                   ) 
                { 
                    useFilter |= true; Filters.Add(CompareBase.Create(childTag)); 
                }
            }
//            if (pGNsIDToolStripMenuItem.Checked) { useFilter |= true; Filters.Add(CompareBase.Create("ID")); }
//            if (sourceToolStripMenuItem.Checked) { useFilter |= true; Filters.Add(CompareBase.Create("Source")); }
//            if (instanceFieldsToolStripMenuItem.Checked) { useFilter |= true; Filters.Add(CompareBase.Create("InstanceFields")); }
//            if (enumFieldsToolStripMenuItem.Checked) { useFilter |= true; Filters.Add(CompareBase.Create("EnumFields")); }

            mnuShowAll.Checked = !useFilter;

            ResetFiltered();
        }
    }

    public abstract class CompareBase : IComparer<Frame>, IComparable
    {
        public int Sign = 1;
        public string Name = "";
        public abstract int Compare(Frame x, Frame y);
        public static CompareBase Create(string s)
        {
            switch (s)
            {
                case "ID": return new ComparePGN() { Name = "ID" };
                case "NameColumn": return new CompareName() { Name = "NameColumn" };   // We don't use DisplayName as it caused problems for C#
                case "Description": return new CompareDescription() { Name = "Descripton" };
                case "Source": return new CompareSource() { Name = "Source" };
                case "Destination": return new CompareDestination() { Name = "Destination" };
                case "Priority": return new ComparePriority() { Name = "Priority" };
                case "Length": return new CompareLength() { Name = "Length" };
                case "Time": return new CompareTime() { Name = "Time" };
                case "Fields": return new CompareFields() { Name = "Fields" };
                case "EnumFields": return new CompareEnumFields() { Name = "EnumFields" };
                case "InstanceFields": return new CompareInstanceFields() { Name = "InstanceFields" };
                case "Header": return new CompareHeader() { Name = "Header" };
                default: throw new Exception("Missing switch value.");
            }
        }
        public int CompareTo(object o)
        {
            return Name.CompareTo((o as CompareBase).Name);
        }
    }
    public class ComparePGN : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Header.Identifier.CompareTo(y.Header.Identifier); } //Sign*x.Header.PGN.CompareTo(y.Header.PGN); }
    }
    public class CompareName : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Defn.Name.CompareTo(y.Defn.Name); }
    }
    public class CompareDescription : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Defn.Description.CompareTo(y.Defn.Description); }
    }
    public class CompareSource : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Header.Source.CompareTo(y.Header.Source); }
    }
    public class CompareDestination : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Header.Destination.CompareTo(y.Header.Destination); }
    }
    public class ComparePriority : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Header.Priority.CompareTo(y.Header.Priority); }
    }
    public class CompareLength : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Length.CompareTo(y.Length); }
    }
    public class CompareFields : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Defn.FieldsString(x).CompareTo(y.Defn.FieldsString(y)); }
    }
    public class CompareEnumFields : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Defn.EnumFieldsString(x).CompareTo(y.Defn.EnumFieldsString(y)); }
    }
    public class CompareInstanceFields : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Defn.InstanceFieldsString(x).CompareTo(y.Defn.InstanceFieldsString(y)); }
    }
    public class CompareTime : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.TimeStamp.CompareTo(y.TimeStamp); }
    }
    public class CompareHeader : CompareBase
    {
        public override int Compare(Frame x, Frame y) { return Sign * x.Header.CompareTo(y.Header); }
    }

    public class FrameComparer : IComparer<Frame>
    {
        public List<CompareBase> Comparers;

        public FrameComparer(List<CompareBase> Comparers)
        {
            this.Comparers = Comparers;
        }

        public int Compare(Frame x, Frame y)
        {
            foreach (var c in Comparers)
            {
                int r = c.Compare(x, y);
                if (r != 0) return r;
            }
            return 0;
        }
    }
}
