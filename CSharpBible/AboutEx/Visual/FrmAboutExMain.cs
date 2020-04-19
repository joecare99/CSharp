using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpBible.AboutEx.Visual
{
    public partial class FrmAboutExMain : Form
    {

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
