using System;
using System.Drawing;
using System.Windows.Forms;

namespace ctlClockLib
{
    /// <summary>
    /// Class ctlClock.
    /// Implements the <see cref="UserControl" />
    /// </summary>
    /// <seealso cref="UserControl" />
    public partial class ctlClock: UserControl
    {
        private Color colFColor;
        private Color colBColor;

        // Declares the name and type of the property.
        /// <summary>
        /// Gets or sets the color of the clock back.
        /// </summary>
        /// <value>The color of the clock back.</value>
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
        /// <summary>
        /// Gets or sets the color of the clock fore.
        /// </summary>
        /// <value>The color of the clock fore.</value>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ctlClock"/> class.
        /// </summary>
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
