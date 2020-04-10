using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ctlClockLib
{
    public partial class ctlClock: UserControl
    {
        private Color colFColor;
        private Color colBColor;

// Declares the name and type of the property.
        public Color ClockBackColor
        {
            // Retrieves the value of the private variable colBColor.
            get
            {
                return colBColor;
            }
            // Stores the selected value in the private variable colBColor, and
            // updates the background color of the label control lblDisplay.
            set
            {
                colBColor = value;
                lblDisplay.BackColor = colBColor;
            }
        }
        // Provides a similar set of instructions for the foreground color.
        public Color ClockForeColor
        {
            get
            {
                return colFColor;
            }
            set
            {
                colFColor = value;
                lblDisplay.ForeColor = colFColor;
            }
        }

        public ctlClock()
        {
            InitializeComponent();
        }

        protected virtual void timer1_Tick(object sender, EventArgs e)
        {
            lblDisplay.Text = DateTime.Now.ToLongTimeString();
        }
    }
}
