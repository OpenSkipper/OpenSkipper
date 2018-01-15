using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;

namespace KaveExtControls
{
    [Description("Simple TextBox with Value (double) property")]
    class ValueBox : System.Windows.Forms.TextBox
    {
#region enum, var, delegate, event
        private double m_Value=0;
        private bool m_OK = false;
        private string m_Format="G";
        private Color m_DefBackColor;
#endregion

#region properties
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                m_OK = false;
                base.Text = value;
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Value"),
        System.ComponentModel.Description("Min value.")]
        public double Min {get; set;}

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Value"),
        System.ComponentModel.Description("Max value.")]
        public double Max { get; set; }

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
                if (!m_OK || (m_Value!=value)) {
                    m_Value = value;
                    m_OK = true;
                    base.Text = m_Value.ToString(m_Format,CultureInfo.CurrentCulture);
                    CheckLimits();
                }
            }
        }

        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Value"),
        System.ComponentModel.Description("Format for the value")]
        public string Format
        {
            get
            {
                return m_Format;
            }
            set
            {
                if (m_Format != value)
                {
                    m_Format = value;
                }
            }
        }
#endregion
#region methods
        public int AsInt32()
        {
            return System.Convert.ToInt32(Value);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            NumberFormatInfo fi = CultureInfo.CurrentCulture.NumberFormat;

                string c = e.KeyChar.ToString();
                if (char.IsDigit(c, 0))
                    return;

                if ((SelectionStart == 0) && (c.Equals(fi.NegativeSign)))
                    return;

                // copy/paste
                if ((((int)e.KeyChar == 22) || ((int)e.KeyChar == 3))
                    && ((ModifierKeys & Keys.Control) == Keys.Control))
                    return;

                if (e.KeyChar == '\b')
                    return;

                e.Handled = true;
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            // Call the base OnTextChanged method. 
            base.OnTextChanged(e);

            double TestValue;
            if (double.TryParse(Text, out TestValue)) m_Value = TestValue;
            CheckLimits();
        }

        private void CheckLimits()
        {
            if ((Min <= m_Value) && (m_Value <= Max))
            {
                BackColor = default(Color);
            }
            else 
            {
                BackColor = Color.Red;
            }
        }

        public ValueBox()
            : base()
        {
            Min = Double.MinValue;
            Max = Double.MaxValue;
            m_DefBackColor=BackColor;
        }
#endregion
    }
}
