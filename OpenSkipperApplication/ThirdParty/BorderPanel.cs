using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace KaveExtControls
{
    [Description("Panel with different border color and width")]
    class BorderPanel : System.Windows.Forms.Panel
    {
#region enum, var, delegate, event
        private Color m_BorderColor=Control.DefaultForeColor;
        private int m_BorderWidth=2;
        private Rectangle m_DisplayRectangle;
        private VisualStyleRenderer renderer;
        private BorderStyle m_BorderStyle;
        private ButtonBorderStyle m_BorderStyle3DExt;

#endregion

#region properties
        //        [System.ComponentModel.Browsable(true)]
        public new BorderStyle BorderStyle
        {
            get { return m_BorderStyle; }
            set
            {
                m_BorderStyle = value;
                Invalidate();
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Border"),
        System.ComponentModel.Description("Extra style definition for Fixed3D")]
        public ButtonBorderStyle BorderStyle3DExt
        {
            get
            {
                return m_BorderStyle3DExt;
            }
            set
            {
                m_BorderStyle3DExt = value;
                Refresh();
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Border"),
        System.ComponentModel.Description("Width of border")]
        public int BorderWidth
        {
            get
            {
                return m_BorderWidth;
            }
            set
            {
                m_BorderWidth = value;
                m_DisplayRectangle = this.ClientRectangle;
                m_DisplayRectangle.Inflate(-BorderWidth, -BorderWidth);
                Refresh();
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Border"),
        System.ComponentModel.Description("Color of the border")]
        public Color BorderColor
        {
            get
            {
                return m_BorderColor;
            }
            set
            {
                m_BorderColor = value;
                Refresh();
            }
        }

        [BrowsableAttribute(false)]
        public override Rectangle DisplayRectangle 
        {
            get
            {
                return m_DisplayRectangle;
            }
        }
#endregion
        public BorderPanel()
            : base()
        {
            m_DisplayRectangle=this.ClientRectangle; 
            m_DisplayRectangle.Inflate(-BorderWidth,-BorderWidth);
            renderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);
            this.SetStyle(ControlStyles.UserPaint, true);
            base.BorderStyle = BorderStyle.None;
            this.BorderStyle = BorderStyle.None;
            m_BorderStyle3DExt = ButtonBorderStyle.Inset;
        }

/*
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84: m.Result = new IntPtr(0x2);
                    return;
            }
            base.WndProc(ref m);
        }
*/
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
//            renderer.DrawEdge(e.Graphics, ClientRectangle,
//              Edges.Bottom | Edges.Left | Edges.Right | Edges.Top,
//              EdgeStyle.Raised, EdgeEffects.Flat);
            switch (m_BorderStyle)
            {
                case BorderStyle.None:
                    break;
                case BorderStyle.Fixed3D:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                                         m_BorderColor, BorderWidth, m_BorderStyle3DExt,
                                         m_BorderColor, BorderWidth, m_BorderStyle3DExt,
                                         m_BorderColor, BorderWidth, m_BorderStyle3DExt,
                                         m_BorderColor, BorderWidth, m_BorderStyle3DExt);
                    break;
            }

//            e.Graphics.DrawRectangle(
//                new Pen(
//                    new SolidBrush(BorderColor), BorderWidth),
//                    e.ClipRectangle);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetDisplayRectLocation(BorderWidth, BorderWidth);
        }
    }
}
