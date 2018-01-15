// Copyright (C) 2014 Timo Lappalainen
//
//  This software is provided as-is, without any express or implied
//  warranty.  In no event will the authors be held liable for any damages
//  arising from the use of this software.

//  Permission is granted to anyone to use this software for any purpose,
//  including commercial applications, and to alter it and redistribute it
//  freely, subject to the following restrictions:

//  1. The origin of this software must not be misrepresented; you must not
//     claim that you wrote the original software. if you use this software
//     in a product, an acknowledgment in the product documentation would be
//     appreciated but is not required.
//  2. Altered source versions must be plainly marked as such, and must not be
//     misrepresented as being the original software.
//  3. This notice may not be removed or altered from any source distribution.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;


namespace KaveExtControls
{
    [Description("Displays a value on an analog gauge. Raises an event if the value enters one of the definable ranges.")]
    public partial class Compass : Control
    {
#region enum, var, delegate, event
        private Boolean drawCompassBackground;
        private Single m_value;
        private Bitmap compassBitmap;
        private Point[][] RoseWhite=new Point[8][];
        private Point[][] RoseBlack=new Point[8][];
        private Point[] HeadingMark = new Point[3];
        private const int _DrawSize = 300;
        private const int HeadingMarkSize = 10;
        private Brush Rose1 = new SolidBrush(Color.Black);
        private Brush Rose2 = new SolidBrush(Color.White);
        private Font TextFont = new Font("Arial",20);
        private Brush TextBrush = new SolidBrush(Color.Black);
        private StringFormat TextFormat = new StringFormat(StringFormatFlags.NoClip);

        public class ValueInRangeChangedEventArgs : EventArgs
        {
            public Int32 valueInRange;

            public ValueInRangeChangedEventArgs(Int32 valueInRange)
            {
                this.valueInRange = valueInRange;
            }
        }

        public delegate void ValueInRangeChangedDelegate(Object sender, ValueInRangeChangedEventArgs e);
        [Description("This event is raised if the value falls into a defined range.")]
        public event ValueInRangeChangedDelegate ValueInRangeChanged;
#endregion

#region hidden , overridden inherited properties
        public new Boolean AllowDrop
        {
            get
            {
                return false;
            }
            set
            {

            }
        }
        public new Boolean AutoSize
        {
            get
            {
                return false;
            }
            set
            {

            }
        }
        public new Boolean ForeColor
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        public new Boolean ImeMode
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public override System.Drawing.Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                drawCompassBackground = true;
                Refresh();
            }
        }
        public override System.Drawing.Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                drawCompassBackground = true;
                Refresh();
            }
        }
        public override System.Windows.Forms.ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
                drawCompassBackground = true;
                Refresh();
            }
        }
        #endregion

        public Compass()
        {
            for (int i = 0; i < RoseWhite.Length; i++) RoseWhite[i] = new Point[3];
            for (int i = 0; i < RoseBlack.Length; i++) RoseBlack[i] = new Point[3];
            CalculateRosePoints();
            TextFormat.Alignment = StringAlignment.Center;

            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void CalculateRosePoints()
        { 
          int RS=3;
          RoseBlack[0][0].X =  0*RS; RoseBlack[0][0].Y =  0*RS;
          RoseBlack[0][1].X =  0*RS; RoseBlack[0][1].Y = 28*RS;
          RoseBlack[0][2].X =  4*RS; RoseBlack[0][2].Y =  8*RS;
          // N Black side small
          RoseBlack[4][0].X =  0*RS; RoseBlack[4][0].Y =  0*RS;
          RoseBlack[4][1].X = 14*RS; RoseBlack[4][1].Y = 14*RS;
          RoseBlack[4][2].X =  8*RS; RoseBlack[4][2].Y =  4*RS;
          // N White side main
          RoseWhite[0][0].X =  0*RS; RoseWhite[0][0].Y =  0*RS;
          RoseWhite[0][1].X =  0*RS; RoseWhite[0][1].Y = 28*RS;
          RoseWhite[0][2].X = -4*RS; RoseWhite[0][2].Y =  8*RS;
          // N White side small
          RoseWhite[4][0].X =  0*RS; RoseWhite[4][0].Y =  0*RS;
          RoseWhite[4][1].X = 14*RS; RoseWhite[4][1].Y = 14*RS;
          RoseWhite[4][2].X =  4*RS; RoseWhite[4][2].Y =  8*RS;

          for (int I=1; I<4; I++) {
            double sinRot = Math.Sin(I * 90 * Math.PI / 180); 
            double cosRot=Math.Cos(I*90*Math.PI/180);
            for (int J=0; J<3; J++) {
              RoseBlack[I][J].X=(int)(cosRot*RoseBlack[0][J].X+sinRot*RoseBlack[0][J].Y);
              RoseBlack[I][J].Y=(int)(-sinRot*RoseBlack[0][J].X+cosRot*RoseBlack[0][J].Y);
              RoseBlack[4+I][J].X=(int)(cosRot*RoseBlack[4][J].X+sinRot*RoseBlack[4][J].Y);
              RoseBlack[4+I][J].Y=(int)(-sinRot*RoseBlack[4][J].X+cosRot*RoseBlack[4][J].Y);
              RoseWhite[I][J].X=(int)(cosRot*RoseWhite[0][J].X+sinRot*RoseWhite[0][J].Y);
              RoseWhite[I][J].Y=(int)(-sinRot*RoseWhite[0][J].X+cosRot*RoseWhite[0][J].Y);
              RoseWhite[4+I][J].X=(int)(cosRot*RoseWhite[4][J].X+sinRot*RoseWhite[4][J].Y);
              RoseWhite[4 + I][J].Y = (int)(-sinRot * RoseWhite[4][J].X + cosRot * RoseWhite[4][J].Y);
            };
          }
        }

        private void CalcHeadingPoints()
        {
            HeadingMark[0] = new Point(Width / 2, HeadingMarkSize+Margin.Top);
            HeadingMark[1] = new Point((Width - HeadingMarkSize) / 2, Margin.Top);
            HeadingMark[2] = new Point((Width + HeadingMarkSize) / 2, Margin.Top);
        }
#region properties
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Value"),
        System.ComponentModel.Description("The value.")]
        public Single Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if (m_value != value)
                {
                    m_value = value;

                    if (this.DesignMode)
                    {
                        drawCompassBackground = true;
                    }

                    Refresh();
                }
            }
        }

        #endregion
        private void DrawRose(Graphics ggr)
        {
            for (int i = 0; i < RoseBlack.Length; i++)
            {
                ggr.FillPolygon(Rose1, RoseBlack[i]);
            }
            for (int i = 0; i < RoseWhite.Length; i++)
            {
                ggr.FillPolygon(Rose2, RoseWhite[i]);
            }
        }

        private void DrawTics(Graphics ggr, int CircleRadius)
        {
            Pen bp = new Pen(Color.Black, 2);

            // Draw main tics
            int mtl = 10;
            for (int I = 0; I < 36; I++)
            {
                double angle = I * 10 * Math.PI / 180.0;
                ggr.DrawLine(bp, new Point((int)((CircleRadius - mtl) * Math.Cos(angle)), (int)((CircleRadius - mtl) * Math.Sin(angle))),
                                 new Point((int)((CircleRadius) * Math.Cos(angle)), (int)((CircleRadius) * Math.Sin(angle))));
            }

            // Draw minor tics
            mtl /= 2;
            bp = new Pen(Color.Black, 1);
            for (int I = 0; I < 36; I++)
            {
                double angle = (5 + I * 10) * Math.PI / 180.0;
                ggr.DrawLine(bp, new Point((int)((CircleRadius - mtl) * Math.Cos(angle)), (int)((CircleRadius - mtl) * Math.Sin(angle))),
                                 new Point((int)((CircleRadius) * Math.Cos(angle)), (int)((CircleRadius) * Math.Sin(angle))));
            }

        }

#region base member overrides
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if ((Width < 10) || (Height < 10))
            {
                return;
            }

                drawCompassBackground = false;

                pe.Graphics.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, Width, Height));
                // Draw heading mark on top
                pe.Graphics.FillPolygon(new SolidBrush(Color.Black), HeadingMark);

                compassBitmap = new Bitmap(_DrawSize, _DrawSize, pe.Graphics);
                Graphics ggr = Graphics.FromImage(compassBitmap);
                ggr.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, _DrawSize, _DrawSize));


                ggr.SmoothingMode = SmoothingMode.HighQuality;
                ggr.PixelOffsetMode = PixelOffsetMode.HighQuality;

//                GraphicsPath gp = new GraphicsPath();
//                ggr.FillPie(new SolidBrush(Color.Red), new Rectangle(0, 0, Width, Height), 0, 360);
//                ggr.DrawPolygon(new Pen(Color.Black, 2), HeadingPoints);
                ggr.TranslateTransform(_DrawSize/2, _DrawSize/2);
                ggr.RotateTransform(-Value);
                int CircleRadius = (_DrawSize - 2) / 2;

                Pen bp=new Pen(Color.Black, 2);
                ggr.DrawArc(bp, new Rectangle(-CircleRadius, -CircleRadius, 2*CircleRadius, 2*CircleRadius), 0, 360);

                DrawTics(ggr, CircleRadius);
                DrawRose(ggr);

                // Finally draw text
                int TextX = 0;
                ggr.DrawString("N", TextFont, TextBrush, TextX, -CircleRadius + 20, TextFormat);
                ggr.RotateTransform(90);
                ggr.DrawString("E", TextFont, TextBrush, TextX, -CircleRadius + 20, TextFormat);
                ggr.RotateTransform(90);
                ggr.DrawString("S", TextFont, TextBrush, TextX, -CircleRadius + 20, TextFormat);
                ggr.RotateTransform(90);
                ggr.DrawString("W", TextFont, TextBrush, TextX, -CircleRadius + 20, TextFormat);

                int RoseSize = Math.Min(Width-Margin.Left-Margin.Right, Height - Margin.Top- Margin.Bottom - HeadingMarkSize);
//                Point Center = new Point(Width/2,Height/2+Margin.Top+HeadingMarkSize);
                pe.Graphics.DrawImage(compassBitmap, new Rectangle((Width-RoseSize) / 2, Margin.Top + HeadingMarkSize, RoseSize, RoseSize));
        }

        protected override void OnResize(EventArgs e)
        {
            drawCompassBackground = true;
            CalcHeadingPoints();
            CalculateRosePoints();
            Refresh();
        }
#endregion

    }
}
