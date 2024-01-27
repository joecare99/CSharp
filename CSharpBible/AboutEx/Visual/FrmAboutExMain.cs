using System;
using System.Windows.Forms;

namespace CSharpBible.AboutEx.Visual
{
    /// <summary>
    /// Class FrmAboutExMain.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class FrmAboutExMain : Form
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmAboutExMain"/> class.
        /// </summary>
        public FrmAboutExMain()
        {
            InitializeComponent();
        }

        private void btnClickMe_Click(object sender, EventArgs e)
        {
            new FrmAbout().Show();
        }

        private void btnClickMe2_Click(object sender, EventArgs e)
        {
            new AboutBox1().Show();
        }
    }
}
