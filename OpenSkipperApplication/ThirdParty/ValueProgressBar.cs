using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace KaveExtControls
{
    [Description("Progress bar to different directions and range colors")]
    public class ValueProgressBar : ProgressBar
    {
#region enum, var, delegate, event
        public enum BarDirectionEnum : byte
        {
            LeftToRight,
            RightToLeft,
            DownToUp,
            UpToDown
        }
        private int m_Margin = 1;
        private double m_Value = 0;
        private Double m_RangeStart = 0;
        private Double m_RangeMaxEnd = 10;
        private const Byte NUMOFRANGES = 10;
        private Byte m_RangeIndex=0;
        private Boolean[] m_RangeEnabled = { true,        false,       false,       false,       false,       false,       false,       false,       false,       false };
        private Color[] m_RangeColor =     { Color.Green, Color.Green, Color.Green, Color.Green, Color.Green, Color.Green, Color.Green, Color.Green, Color.Green, Color.Green };
        private Color[] m_RangeMarkerColor = { Color.Red, Color.Red, Color.Red, Color.Red, Color.Red, Color.Red, Color.Red, Color.Red, Color.Red, Color.Red };
        private Double[] m_RangeEnd = { 10.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, };
#endregion
#region properties
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Value"),
        System.ComponentModel.Description("The value.")]
        public double DValue
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
                Refresh();
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Range"),
        System.ComponentModel.RefreshProperties(RefreshProperties.All),
        System.ComponentModel.Description("The range index. set this to a value of 0 up to 9 to change the corresponding range's properties.")]
        public Byte RangeIndex
        {
            get
            {
                return m_RangeIndex;
            }
            set
            {
                if ((m_RangeIndex != value)
                && (0 <= value)
                && (value < NUMOFRANGES))
                {
                    m_RangeIndex = value;
                    Refresh();
                }
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Range"),
        System.ComponentModel.RefreshProperties(RefreshProperties.All),
        System.ComponentModel.Description("The range start value.")]
        public Double RangeStart
        {
            get
            {
                return m_RangeStart;
            }
            set
            {
                m_RangeStart = value;
                Refresh();
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Range"),
        System.ComponentModel.RefreshProperties(RefreshProperties.All),
        System.ComponentModel.Description("The range end value for selected RangeIndex.")]
        public Double RangeEnd
        {
            get
            {
                return m_RangeEnd[m_RangeIndex];
            }
            set
            {
                m_RangeEnd[m_RangeIndex] = value;
                m_RangeEnabled[m_RangeIndex] = true;
                CalculateRangeMaxEnd();
                Refresh();
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Range"),
        System.ComponentModel.RefreshProperties(RefreshProperties.All),
        System.ComponentModel.Description("The range color for selected RangeIndex.")]
        public Color RangeColor
        {
            get
            {
                return m_RangeColor[m_RangeIndex];
            }
            set
            {
                m_RangeColor[m_RangeIndex] = value;
                Refresh();
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Range"),
        System.ComponentModel.RefreshProperties(RefreshProperties.All),
        System.ComponentModel.Description("The range marker color for selected RangeIndex.")]
        public Color RangeMarkerColor
        {
            get
            {
                return m_RangeMarkerColor[m_RangeIndex];
            }
            set
            {
                m_RangeMarkerColor[m_RangeIndex] = value;
                Refresh();
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Range"),
        System.ComponentModel.RefreshProperties(RefreshProperties.All),
        System.ComponentModel.Description("Is range for selected RangeIndex use.")]
        public Boolean RangeEnabled
        {
            get
            {
                return m_RangeEnabled[m_RangeIndex];
            }
            set
            {
                if (m_RangeIndex == 0) return;  // Can not change first range Enabled
                m_RangeEnabled[m_RangeIndex] = value;
                CalculateRangeMaxEnd();
                Refresh();
            }
        }

        [System.ComponentModel.Browsable(true),
         System.ComponentModel.Category("Layout"),
         System.ComponentModel.Description("Direction for progress bar")]
        public BarDirectionEnum Direction { get; set; }

        [System.ComponentModel.Browsable(true),
         System.ComponentModel.Category("Layout"),
         System.ComponentModel.Description("Define should range end markers to be drawn or not")]
        public Boolean DrawRangeEndMarkers { get; set; }

        // Properties for save all values to Designer.cs
        [System.ComponentModel.Browsable(false)]
        public Boolean[] RangesEnabled
        {
            get
            {
                return m_RangeEnabled;
            }
            set
            {
                m_RangeEnabled = value;
                CalculateRangeMaxEnd();
            }
        }

        // Properties for save all values to Designer.cs
        [System.ComponentModel.Browsable(false)]//,
//        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Double[] RangesEnd
        {
            get
            {
                return m_RangeEnd;
            }
            set
            {
                m_RangeEnd = value;
                CalculateRangeMaxEnd();
            }
        }

        [System.ComponentModel.Browsable(false)]
        public Color[] RangesColor
        {
            get
            {
                return m_RangeColor;
            }
            set
            {
                m_RangeColor = value;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public Color[] RangesMarkerColor
        {
            get
            {
                return m_RangeMarkerColor;
            }
            set
            {
                m_RangeMarkerColor = value;
            }
        }
#endregion

        public ValueProgressBar()
        {
            Direction = BarDirectionEnum.LeftToRight;
            DrawRangeEndMarkers = true;
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected Boolean IsBetween(Double val, Double start, Double end)
        {
            if (((start <= val) && (val < end)) || ((end <= val) && (val < start)))
            {
                return true;
            }
            else
            {
                return false;
            };
        }

        protected Color GetColor()
        {
            int iRange = 0;

            for (; ((iRange < NUMOFRANGES) && (m_RangeEnabled[iRange]) && !(IsBetween(DValue, RangeStart, m_RangeEnd[iRange]))); iRange++) ;
            if (iRange == NUMOFRANGES) iRange = 0;
            return m_RangeColor[iRange];
        }

        protected void CalculateRangeMaxEnd()
        {
            int iRange = 0;

            for (m_RangeMaxEnd=0; (
                    (iRange < NUMOFRANGES) && 
                    (m_RangeEnabled[iRange]) && 
                    (Math.Abs(m_RangeMaxEnd) < Math.Abs(m_RangeEnd[iRange]))
                   ); iRange++) m_RangeMaxEnd = m_RangeEnd[iRange];
        }

        protected int GetBarLength(Double val)
        {
            int BarMax = (IsHorizontal() ? this.Width - 2 * m_Margin : this.Height - 2 * m_Margin);
            if (m_RangeMaxEnd - m_RangeStart == 0) return 0;
            int ScaledValue = (int)(BarMax * val / (m_RangeMaxEnd - m_RangeStart));
            if (ScaledValue < 0) ScaledValue = 0;
            return ScaledValue;
        }

        protected int GetBarLengthForCurrentValue()
        {
            return GetBarLength(DValue);
        }

        protected Boolean IsHorizontal()
        {
            return ((Direction == BarDirectionEnum.LeftToRight) || (Direction == BarDirectionEnum.RightToLeft));
        }

        // See http://stackoverflow.com/questions/778678/how-to-change-the-color-of-progressbar-in-c-sharp-net-3-5
        protected override void OnPaint(PaintEventArgs e)
        {
            LinearGradientBrush brush = null;
            Rectangle rec = new Rectangle(0, 0, this.Width, this.Height);
            CalculateRangeMaxEnd(); // Need some better here!
            int BarLength = GetBarLengthForCurrentValue();

            if (IsHorizontal())
            {
                // First draw background
                if (ProgressBarRenderer.IsSupported) ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rec);

                int startX;

                if (BarLength > 0)
                {
                    rec.Width = BarLength;
                    rec.Height -= 2 * m_Margin;
                    startX = (Direction == BarDirectionEnum.LeftToRight ? m_Margin : this.Width-m_Margin-rec.Width);

                    brush = new LinearGradientBrush(rec, GetColor(), this.BackColor, LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(brush, startX, m_Margin, rec.Width, rec.Height);
                }

                if (DrawRangeEndMarkers)
                for (byte i = 0; ((i + 1 < NUMOFRANGES) && m_RangeEnabled[i + 1]); i++) // Not we do not draw range max line
                {
                    Pen pen = new Pen(new SolidBrush(m_RangeMarkerColor[i]));
                    // Draw range lines
                    startX = (Direction == BarDirectionEnum.LeftToRight ? m_Margin+GetBarLength(m_RangeEnd[i]) : this.Width - m_Margin - GetBarLength(m_RangeEnd[i]));
                    e.Graphics.DrawLine(pen, new Point(startX, m_Margin),
                                             new Point(startX, this.Height - m_Margin));
                }
            }
            else
            {
                // First draw background
                if (ProgressBarRenderer.IsSupported) ProgressBarRenderer.DrawVerticalBar(e.Graphics, rec);

                int startY;

                if (BarLength > 0)
                {
                    rec.Height = BarLength;
                    rec.Width -= 2*m_Margin;
                    startY = (Direction == BarDirectionEnum.DownToUp ? this.Height - m_Margin - rec.Height : m_Margin);

                    brush = new LinearGradientBrush(rec, GetColor(), this.BackColor, LinearGradientMode.Horizontal);
                    e.Graphics.FillRectangle(brush, m_Margin, startY, rec.Width, rec.Height);
                }

                if (DrawRangeEndMarkers)
                for (byte i=0; ((i+1<NUMOFRANGES) && m_RangeEnabled[i+1]);i++) // Not we do not draw range max line
                {
                    Pen pen = new Pen(new SolidBrush(m_RangeMarkerColor[i]));
                    // Draw range lines
                    startY = (Direction == BarDirectionEnum.DownToUp ? this.Height - m_Margin - GetBarLength(m_RangeEnd[i]) : m_Margin + GetBarLength(m_RangeEnd[i]));
                    e.Graphics.DrawLine(pen, new Point(m_Margin, startY),
                                             new Point(this.Width - m_Margin, startY));
                }
            }
        }
    }
}
