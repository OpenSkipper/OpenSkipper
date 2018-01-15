/*
 * Created by SharpDevelop.
 * User: Tefik Becirovic
 * Date: 15.09.2008
 * Time: 19:32
 * 
 * Sourced from http://www.codeproject.com/KB/miscctrl/SlidingScale.aspx
 * Distributed under the The Code Project Open License (CPOL); see http://www.codeproject.com/info/cpol10.aspx
 * 
 * Modified by Andrew Mason 2010.3.17 to add wrapping, making it suitable for a heading display. 
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TB.Instruments
{
	/// <summary>
	/// SlidingScale UserControl.
	/// </summary>
	public partial class SlidingScale : UserControl
	{
		#region [ Constructor ... ]
		
		/// <summary>
		/// SclidingScale default constructor.
		/// </summary>
		public SlidingScale()
		{
			InitializeComponent();
			
			SetStyle(ControlStyles.UserPaint |
			         ControlStyles.DoubleBuffer |
			         ControlStyles.ResizeRedraw |
			         ControlStyles.AllPaintingInWmPaint |
			         ControlStyles.SupportsTransparentBackColor,
			         true );
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// SlidingScale
			// 
			this.ForeColor = System.Drawing.Color.Black;
			this.BackColor = System.Drawing.Color.White;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Name = "SlidingScale";
			this.Size = new System.Drawing.Size(254, 45);
			this.ResumeLayout(false);
		}

		#endregion [ Constructor ]
		
		#region [ Properties ... ]
		
		private double curValue = 0.0;
		/// <summary>
		/// The current position of the scale.
		/// </summary>
		[
			Browsable(true),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			Category("Behavior"),
			Description("The current position of the scale."),
			DefaultValue(typeof(double), "0.0")
		]
		public double Value
		{
			get { return curValue; }
			set
			{
				double oldValue = curValue;
				
				curValue = value;
				
				// Refresh only if the curValue is significant changed.
				if (Math.Abs(oldValue - curValue) > 0.0001)
					this.Refresh();
			}
		}

		private double scaleRange = 100.0;
		/// <summary>
		/// The visible range of the scale.
		/// </summary>
		[
			Browsable(true),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			Category("Appearance"),
			Description("The visible range of the scale."),
			DefaultValue(typeof(double), "100.0")
		]
		public double ScaleRange
		{
			get { return scaleRange; }
			set
			{
				if (value>0.001)
				{
					scaleRange = value;
					this.Invalidate();
				}
			}
		}

        private bool wrap = false;
        /// <summary>
        /// if true, the scale range is 0 to ScaleRange and then back to zero (as on a compass).
        /// </summary>
        [
            Browsable(true),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            Category("Appearance"),
            Description("Set true to wrap back to zero."),
            DefaultValue(typeof(bool), "false")
        ]
        public bool Wrap {
            get { return wrap; }
            set {
                if (value != wrap) {
                    wrap = value;
                    this.Invalidate();
                }
            }
        }

        private int largeTicksCount = 5;
		/// <summary>
		/// The number of large ticks.
		/// </summary>
		[
			Browsable(true),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			Category("Appearance"),
			Description("The number of large ticks."),
			DefaultValue(typeof(int), "5")
		]
		public int LargeTicksCount
		{
			get { return largeTicksCount; }
			set
			{
				if (value!=largeTicksCount && value>0)
				{
					largeTicksCount = value;
					this.Invalidate();
				}
			}
		}
		
		private int largeTicksLength = 15;
		/// <summary>
		/// The length of large ticks.
		/// </summary>
		[
			Browsable(true),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			Category("Appearance"),
			Description("The length of large ticks."),
			DefaultValue(typeof(int), "15")
		]
		public int LargeTicksLength
		{
			get { return largeTicksLength; }
			set
			{
				if (value!=largeTicksLength && value>-1)
				{
					largeTicksLength = value;
					this.Invalidate();
				}
			}
		}
		
		private int smallTicksCount = 4;
		/// <summary>
		/// The number of small ticks.
		/// </summary>
		[
			Browsable(true),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			Category("Appearance"),
			Description("The number of small ticks."),
			DefaultValue(typeof(int), "4")
		]
		public int SmallTickCount
		{
			get { return smallTicksCount; }
			set	{
				if (value!=smallTicksCount && value>-1 && value<10)
				{
					smallTicksCount = value;
					this.Invalidate();
				}
			}
		}
		
		private int smallTicksLength = 10;
		/// <summary>
		/// The length of small ticks.
		/// </summary>
		[
			Browsable(true),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			Category("Appearance"),
			Description("The length of small ticks."),
			DefaultValue(typeof(int), "10")
		]
		public int SmallTickLength
		{
			get { return smallTicksLength; }
			set	{
				if (value!=smallTicksLength && value>-1)
				{
					smallTicksLength = value;
					this.Invalidate();
				}
			}
		}

		private bool shadowEnabled = true;
		/// <summary>
		/// The shadow color of the component.
		/// </summary>
		[
			Browsable(true),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			Category("Appearance"),
			Description("Is the shadow enabled?"),
			DefaultValue(typeof(bool), "true")
		]
		public bool ShadowEnabled
		{
			get { return shadowEnabled; }
			set
			{
				if (value!=shadowEnabled)
				{
					shadowEnabled = value;
					this.Invalidate();
				}
			}
		}
		
		private Color shadowColor = Color.Black;
		/// <summary>
		/// The shadow color of the component.
		/// </summary>
		[
			Browsable(true),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			Category("Appearance"),
			Description("The shadow color of the component."),
			DefaultValue(typeof(Color), "Black")
		]
		public Color ShadowColor
		{
			get { return shadowColor; }
			set
			{
				if (value!=shadowColor)
				{
					shadowColor = value;
					this.Invalidate();
				}
			}
		}
		
		private Color needleColor = Color.Blue;
		/// <summary>
		/// The color of the scala needle.
		/// </summary>
		[
			Browsable(true),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			Category("Appearance"),
			Description("The color of the scala needle."),
			DefaultValue(typeof(Color), "Blue")
		]
		public Color NeedleColor
		{
			get { return needleColor; }
			set
			{
				if (value!=needleColor)
				{
					needleColor = value;
					this.Invalidate();
				}
			}
		}

		private Orientation orientation = Orientation.Horizontal;
		/// <summary>
		/// The orientation of the control.
		/// </summary>
		[
			Browsable(true),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			Category("Appearance"),
			Description("The orientation of the control."),
			DefaultValue(typeof(Orientation), "Horizontal")
		]
		public Orientation Orientation
		{
			get { return orientation; }
			set
			{
				if (value!=orientation)
				{
					orientation = value;
					this.Invalidate();
				}
			}
		}
		
		#endregion [ Properties ]
		
		#region [ Private ... ]
		
		private int W,H, Wm,Hm = 0;
		private double largeTicksDistance = 0.0;
		private double smallTicksDistance = 0.0;
		private float smallTicksPixels = 0f;
		
		private void CalculateLocals()
		{
			// Calculate help variables
			W = this.ClientRectangle.Width;
			H = this.ClientRectangle.Height;

			Wm = W / 2;
			Hm = H / 2;
			
			// Calculate distances between ticks
			largeTicksDistance = scaleRange / largeTicksCount;
			smallTicksDistance = largeTicksDistance / (smallTicksCount+1);
			
			// Calculate number of pixel between small ticks
			if (Orientation == Orientation.Horizontal)
				smallTicksPixels = (float)(W/scaleRange*smallTicksDistance);
			else
				smallTicksPixels = (float)(H/scaleRange*smallTicksDistance);
		}

        private double ValueToDisplay(double value) {
            if (!wrap) {
                return value;
            } else {
                value = Math.Round(value, 6);   // Remove any small rounding problems
                double result = value - Math.Floor(value / scaleRange) * scaleRange;
                return result;
            } 
        }

		private void DrawHorizontal(Graphics g)
		{
			// Calculate first large tick value and position
			double tickValue = Math.Floor((curValue-scaleRange/2) /
			                              largeTicksDistance) * largeTicksDistance;
			float  tickPosition = (float)Math.Floor(Wm - W/scaleRange*(curValue-tickValue));
			
			// Create drawing resources
			Pen pen = new Pen(ForeColor);
			Brush brush = new SolidBrush(ForeColor);
			
			// For all large ticks
			for (int L=0; L<=largeTicksCount; L++)
			{
				// Draw large tick
				g.DrawLine(pen, tickPosition-0,0, tickPosition-0,largeTicksLength);
				g.DrawLine(pen, tickPosition-1,0, tickPosition-1,largeTicksLength);

				// Draw large tick numerical value
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
                g.DrawString(Math.Round(ValueToDisplay(tickValue), 2).ToString(),
				             Font, brush, new PointF(tickPosition, (H+largeTicksLength)/2), sf);
				
				// For all small ticks
				for (int S=1; S<=smallTicksCount; S++)
				{
					// Update tick value and position
					tickValue += smallTicksDistance;
					tickPosition += smallTicksPixels;
					
					// Draw small tick
					g.DrawLine(pen, tickPosition,0,tickPosition,smallTicksLength);
				}
				
				// Update tick value and position
				tickValue += smallTicksDistance;
				tickPosition += smallTicksPixels;
			}
			
			// Dispose drawing resources
			brush.Dispose();
			pen.Dispose();

			if (ShadowEnabled)
			{
				LinearGradientBrush LGBrush = null;
				Rectangle LGRect;
				
				// Draw left side shadow
				LGRect = new Rectangle(0, 0, Wm, H);
				LGBrush = new LinearGradientBrush(LGRect,
				                                  Color.FromArgb(255, ShadowColor),
				                                  Color.FromArgb(0, BackColor),
				                                  000, true);
				g.FillRectangle(LGBrush, LGRect);
				
				// Draw right/bottom side shadow
				LGRect = new Rectangle(Wm+1, 0, Wm, H);
				LGBrush = new LinearGradientBrush(LGRect,
				                                  Color.FromArgb(255, ShadowColor),
				                                  Color.FromArgb(0, BackColor),
				                                  180, true);
				g.FillRectangle(LGBrush, LGRect);

				LGBrush.Dispose();
			}

			// Draw scale needle
			g.DrawLine(new Pen(NeedleColor), Wm-0,0, Wm-0,H);
			g.DrawLine(new Pen(NeedleColor), Wm-1,0, Wm-1,H);
		}
		
		private void DrawVertical(Graphics g)
		{
			// Calculate first large tick value and position
			double tickValue = Math.Ceiling((curValue-scaleRange/2) /
			                                largeTicksDistance) * largeTicksDistance;
			float  tickPosition = (float)Math.Ceiling(Hm + H/scaleRange*(curValue-tickValue));
			
			// Create drawing resources
			Pen pen = new Pen(ForeColor);
			Brush brush = new SolidBrush(ForeColor);
			
			// For all large ticks
			for (int L=0; L<=largeTicksCount; L++)
			{
				// Draw large tick
				g.DrawLine(pen, 0,tickPosition+0, largeTicksLength,tickPosition+0);
				g.DrawLine(pen, 0,tickPosition+1, largeTicksLength,tickPosition+1);

				// Draw large tick numerical value
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.DrawString(Math.Round(ValueToDisplay(tickValue),2).ToString(),
				             Font, brush, new PointF((W+largeTicksLength)/2, tickPosition), sf);
				
				// For all small ticks
				for (int S=1; S<=smallTicksCount; S++)
				{
					// Update tick value and position
					tickValue += smallTicksDistance;
					tickPosition -= smallTicksPixels;
					
					// Draw small tick
					g.DrawLine(pen, 0,tickPosition, smallTicksLength,tickPosition);
				}
				
				// Update tick value and position
				tickValue += smallTicksDistance;
				tickPosition -= smallTicksPixels;
			}
			

			// Dispose drawing resources
			brush.Dispose();
			pen.Dispose();

			if (ShadowEnabled)
			{
				LinearGradientBrush LGBrush = null;

				Rectangle LGRect;

				// Draw left/top side shadow
				LGRect = new Rectangle(0, 0, W, Hm);
				LGBrush = new LinearGradientBrush(LGRect,
				                                  Color.FromArgb(255, ShadowColor),
				                                  Color.FromArgb(0, BackColor),
				                                  090, true);
				g.FillRectangle(LGBrush, LGRect);

				// Draw right/bottom side shadow
				LGRect = new Rectangle(0, Hm+1, W, Hm);
				LGBrush = new LinearGradientBrush(LGRect,
				                                  Color.FromArgb(255, ShadowColor),
				                                  Color.FromArgb(0, BackColor),
				                                  270, true);
				g.FillRectangle(LGBrush, LGRect);

				LGBrush.Dispose();
			}

			// Draw scale needle
			g.DrawLine(new Pen(NeedleColor), 0,Hm+0, W,Hm+0);
			g.DrawLine(new Pen(NeedleColor), 0,Hm+1, W,Hm+1);
		}

		#endregion [ Private ]
		
		#region [ Override ... ]

		/// <summary>
		/// OnPaint override.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (!Visible || !IsHandleCreated) return;
			
			// Draw simple text, don't waste time with luxus render:
			e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

			CalculateLocals();
			
			if (Orientation == Orientation.Horizontal)
				DrawHorizontal(e.Graphics);
			else
				DrawVertical(e.Graphics);
			
			base.OnPaint(e);
		}

		
//		public override string ToString()
//		{
//			return base.ToString() + ", Minimum: " + Minimum + ", Maximum: " + Maximum + ", Value: " + Value;
//		}

		#endregion [ Override ]
	}
}
