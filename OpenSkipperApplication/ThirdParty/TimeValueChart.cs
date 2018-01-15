using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Runtime.CompilerServices;

namespace KaveExtControls
{
    [Description("Value chart with time on x-axis")]
    class TimeValueChart : System.Windows.Forms.DataVisualization.Charting.Chart
    {
#region enum, var
        private bool m_InitDone = false;
        private double m_ForceUpdate = Double.NaN;
        private double m_AutoScrollMultiplier = 0.9;
        private double m_Value = 0;
        private double m_DefSize = 20;
        private double m_AutoScrollSize = 20/86400;
        private string m_ExtChartDef = "";
        private string m_MenuText = "Chart";
        private string m_StrSynchronizeXAxisWith = "";
        private List<TimeValueChart> m_SynchronizeXAxisWith = new List<TimeValueChart>();
        private TimeValueChart m_SynchronizeXAxisParent = null;
        private string m_StrUseMultiAreaChartSeries = "";
        private Series DataSeries = null; // Series we use for setting data
        private ContextMenuStrip m_CM = null;
        private ToolStripMenuItem m_autoscrollToolStripMenuItem;
        private ToolStripMenuItem m_ChartMenu;
        private const string ChartDefaultDefinition= @"
            <Chart Size=""945, 405"">
              <Series>
                <Series Name=""Value"" Legend=""Value"" XValueType=""Time"" YValueType=""Double"" ChartType=""Line"" ChartArea=""Area1"">
                </Series>
              </Series>
              <Legends>
                <Legend Name=""Value"" Enabled=""False"">
                </Legend>
              </Legends>
              <ChartAreas>
                <ChartArea Name=""Area1"">
                  <AxisX IsLabelAutoFit=""False"" LabelAutoFitStyle=""None"">
                    <LabelStyle Interval=""10"" IntervalType=""Seconds"" Format=""HH:mm:ss"" />
                    <ScaleView Position=""0"" Size=""20"" SizeType=""Seconds"" MinSize=""20"" />
                  </AxisX>
                </ChartArea>
              </ChartAreas>
            </Chart>";
#endregion

        public TimeValueChart()
        {
            //            this.Series[0].XValueType = ChartValueType.Time;
            this.AxisScrollBarClicked += chart1_AxisScrollBarClicked;
            this.AxisViewChanged += chart1_AxisViewChanged;
            LoadChartDefaultDefinitions();
            DataSeries = this.Series[0];
        }

        private void LoadChartDefaultDefinitions()
        {
            MemoryStream memStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memStream);
            writer.Write(ChartDefaultDefinition);
            writer.Flush();
            memStream.Position = 0;
            this.Serializer.Load(memStream);
        }

        private void CalculateAutoScrollSize()
        {
            if (Double.IsNaN(this.ChartAreas[0].AxisX.ScaleView.Size)) return;

            m_AutoScrollSize = this.ChartAreas[0].AxisX.ScaleView.Size;
            switch (this.ChartAreas[0].AxisX.ScaleView.SizeType)
            {
                case DateTimeIntervalType.Seconds: m_AutoScrollSize /= (24*60*60); break;
                case DateTimeIntervalType.Minutes: m_AutoScrollSize /= (24*60); break;
                case DateTimeIntervalType.Hours: m_AutoScrollSize /= (24); break;
                case DateTimeIntervalType.Weeks: m_AutoScrollSize *= 7; break;
                case DateTimeIntervalType.Months: m_AutoScrollSize *= 30; break;
                case DateTimeIntervalType.Years: m_AutoScrollSize *= 365; break;
            }
        }

        private void SetSizeAndPosition(double size, double position)
        {
            this.ChartAreas[0].AxisX.ScaleView.Size = size;
            this.ChartAreas[0].AxisX.ScaleView.Position = position;
            CalculateAutoScrollSize();

            foreach (TimeValueChart SyncControl in m_SynchronizeXAxisWith) {
                SyncControl.SetSizeAndPosition(this.ChartAreas[0].AxisX.ScaleView.Size, this.ChartAreas[0].AxisX.ScaleView.Position);
            }
        }

        private void ZoomChanged(object sender, EventArgs e)
        {
            if (Double.IsNaN(m_DefSize)) return;
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem cSender = (ToolStripMenuItem)sender;
                SetSizeAndPosition(m_DefSize / (double)(cSender.Tag), this.ChartAreas[0].AxisX.ScaleView.Position);
            }
        }

        private void CheckScrollPosition(double xVal)
        {
            if (XAxisAutoscroll() && this.ChartAreas[0].AxisX.ScaleView.Position < xVal - m_AutoScrollSize)
            {
                SetSizeAndPosition(this.ChartAreas[0].AxisX.ScaleView.Size, xVal - m_AutoScrollMultiplier * m_AutoScrollSize);
            }
        }

        private bool XAxisAutoscroll()
        {
            return (m_autoscrollToolStripMenuItem == null || m_autoscrollToolStripMenuItem.Checked);
        }

#region events
        private void chart1_AxisScrollBarClicked(object sender, ScrollBarEventArgs e)
        {
            // Stop auto scroll, if used clicks axis scrollbar
            if (m_autoscrollToolStripMenuItem!=null) m_autoscrollToolStripMenuItem.Checked = false;
        }

        private void chart1_AxisViewChanged(object sender, ViewEventArgs e)
        {
            if (m_SynchronizeXAxisParent == null)
            {
                SetSizeAndPosition(this.ChartAreas[0].AxisX.ScaleView.Size, this.ChartAreas[0].AxisX.ScaleView.Position);
            } else
            {
                m_SynchronizeXAxisParent.SetSizeAndPosition(this.ChartAreas[0].AxisX.ScaleView.Size, this.ChartAreas[0].AxisX.ScaleView.Position);
            } 
        }

#endregion

#region Context menu building
        private ContextMenuStrip GetContextMenuStrip()
        {
            if (m_CM == null)
            {
                m_CM = this.ContextMenuStrip;
                for (Control cParent = this.Parent; 
                    m_CM == null && cParent != null; 
                    m_CM = cParent.ContextMenuStrip, cParent=cParent.Parent) ;

                if (m_CM == null)
                {
                    this.ContextMenuStrip = new ContextMenuStrip();
                    m_CM = this.ContextMenuStrip;
                }
            }

            return m_CM;
        }

        private void AddZoomMenuItem(ToolStripMenuItem ParentMenu, int zoom)
        {
            ToolStripMenuItem ZoomItem;

            ZoomItem = new ToolStripMenuItem(this.Name + "_zoom_"+zoom.ToString());
            ZoomItem.Text = "Zoom "+zoom.ToString()+"%";
            ZoomItem.Tag = zoom/100.0;
            ZoomItem.Click += new System.EventHandler(ZoomChanged);
            ParentMenu.DropDownItems.Add(ZoomItem);
        }

        private TimeValueChart FindChartByName(string chartName)
        {
            Control cParent = this.Parent;
            for (; cParent != null && cParent.Parent != null; cParent = cParent.Parent) ;

            if (cParent == null) return null;

            Control[] c = cParent.Controls.Find(chartName, true);
            if (c.Count() > 0 && c[0] is TimeValueChart)
            {
                return (TimeValueChart)c[0];
            }
            else
            {
                return null;
            }
        }

        private void SynchronizeXAxisToControl(string cChartName)
        {
            TimeValueChart cSyncChart=FindChartByName(cChartName);
            if (cSyncChart != null)
            {
                if (cSyncChart.m_CM!=null) // Clean context menu created by chart itself
                {
                    cSyncChart.m_CM.Items.Remove(cSyncChart.m_ChartMenu);
                    cSyncChart.m_autoscrollToolStripMenuItem = null;
                    cSyncChart.m_ChartMenu = null;
                    cSyncChart.m_CM = null;
                }
                cSyncChart.m_autoscrollToolStripMenuItem = m_autoscrollToolStripMenuItem;
                cSyncChart.m_SynchronizeXAxisParent = this;
                m_SynchronizeXAxisWith.Add(cSyncChart);
            }
        }

        private void AddToolStrip()
        {
            if (m_StrUseMultiAreaChartSeries == "" && m_SynchronizeXAxisParent==null && m_CM == null)
            {
                GetContextMenuStrip();
                if (!Double.IsNaN(m_DefSize))
                {
                    m_ChartMenu = new ToolStripMenuItem(this.Name + "_Menu");
                    m_ChartMenu.Text = this.MenuText;
                    m_CM.Items.Add(m_ChartMenu);

                    m_autoscrollToolStripMenuItem = new ToolStripMenuItem(this.Name + "_autoscrollToolStripMenuItem");
                    m_autoscrollToolStripMenuItem.Text = "Auto scroll";
                    m_autoscrollToolStripMenuItem.CheckOnClick = true;
                    m_autoscrollToolStripMenuItem.Checked = true;

                    m_ChartMenu.DropDownItems.Add(m_autoscrollToolStripMenuItem);

                    AddZoomMenuItem(m_ChartMenu, 25);
                    AddZoomMenuItem(m_ChartMenu, 50);
                    AddZoomMenuItem(m_ChartMenu, 100);
                    AddZoomMenuItem(m_ChartMenu, 150);
                }

                if (m_StrSynchronizeXAxisWith != "")
                {
                    string[] SyncCharts = m_StrSynchronizeXAxisWith.Split(new string[] { ";" }, StringSplitOptions.None);
                    foreach (string item in SyncCharts) SynchronizeXAxisToControl(item);
                }
            }
        }

        private void Init()
        {
            if (m_InitDone) return;
            m_InitDone = true;

            if (m_StrUseMultiAreaChartSeries != "")
            {
                string[] LinkStrs = m_StrUseMultiAreaChartSeries.Split(new string[] { ":" }, StringSplitOptions.None);
                TimeValueChart cLinkChart = FindChartByName(LinkStrs[0]);
                if (cLinkChart != null)
                {
                    DataSeries = cLinkChart.DataSeries; // as default use first
                    if (LinkStrs.Count() > 1)
                    {
                        foreach (Series item in cLinkChart.Series)
                        {
                            if (item.Name == LinkStrs[1]) DataSeries = item;
                        }
                        //                        DataSeries = cLinkChart.Series[LinkStrs[1]];
                    }
                }
            }

            AddToolStrip();
        }

#endregion

#region properties
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Value"),
        System.ComponentModel.Description("The value.")]
        public double Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                // For charts, which data has been linked and chart area is hidden behind other chart,
                // I could not find better way to do all initialization. For those, Paint will be never
                // called.
                Init();

                Double xVal = DateTime.Now.ToOADate();
                m_Value = value;
                DataSeries.Points.AddXY(xVal, value);
                CheckScrollPosition(xVal);
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Value"),
        System.ComponentModel.Description("Series value")]
        [IndexerNameAttribute("ValueOfSeries")]
        public double this[int key]    
        {
            get
            {
                if (key >= 0 && key < this.Series.Count && this.Series[key].Points.Count > 0) return this.Series[key].Points.Last().YValues[0];
                return 0;
            }
            set
            {
                // For charts, which data has been linked and chart area is hidden behind other chart,
                // I could not find better way to do all initialization. For those, Paint will be never
                // called.
                Init();

                if (key>=0 && key < this.Series.Count)
                {
                    Double xVal = DateTime.Now.ToOADate();
                    this.Series[key].Points.AddXY(xVal, value);
                    CheckScrollPosition(xVal);
                }
                if (key==0) m_Value = value;
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Chart"),
        System.ComponentModel.Description("Time between updates even no change in value")]
        public double ForceUpdate
        {
            get
            {
                return m_ForceUpdate;
            }
            set
            {
                m_ForceUpdate=value;
            }
        }
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Chart"),
        System.ComponentModel.Description("Defines how much (percentage of size) we scroll axis, when x-axis end will bea reched")]
        public double AutoScrollSize
        {
            get
            {
                return Math.Round((1 - m_AutoScrollMultiplier) * 100,2);
            }
            set
            {
                if ((value > 1) && (value<100)) { m_AutoScrollMultiplier=1-value/100; }
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Chart"),
        System.ComponentModel.Description("")]
        public string ChartDefinitionFile
        {
            get
            {
                return m_ExtChartDef;
            }
            set
            {
                int wCur = this.Width;
                int hCur = this.Height;
                m_ExtChartDef = value;
                this.Serializer.Load(m_ExtChartDef);
                DataSeries = this.Series[0];
                DataSeries.XValueType = ChartValueType.Time; // Currently works only with time axis

                this.Width = wCur;
                this.Height = hCur;
                m_DefSize = this.ChartAreas[0].AxisX.ScaleView.Size;
                CalculateAutoScrollSize();
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Chart"),
        System.ComponentModel.Description("Synchronizes chart X-Axis with named chart on same form.")]
        public string SynchronizeXAxisWith
        {
            get
            {
                return m_StrSynchronizeXAxisWith;
            }
            set
            {
                m_StrSynchronizeXAxisWith=value; //We need to search syncronize components later.
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Chart"),
        System.ComponentModel.Description("Use multiarea chart are for displaying data")]
        public string UseMultiAreaChartSeries
        {
            get
            {
                return m_StrUseMultiAreaChartSeries;
            }
            set
            {
                m_StrUseMultiAreaChartSeries = value; //We need to search syncronize components later.
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Chart"),
        System.ComponentModel.Description("Menu text in context menu")]
        public string MenuText
        {
            get
            {
                return m_MenuText;
            }
            set
            {
                m_MenuText = value; //We need to search syncronize components later.
            }
        }
#endregion
    }
}
