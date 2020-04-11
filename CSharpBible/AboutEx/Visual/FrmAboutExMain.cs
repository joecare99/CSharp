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
        private FrmAbout frmAbout;
        private AboutBox1 frmAbout2;

        public FrmAboutExMain()
        {
            InitializeComponent();
            frmAbout = new FrmAbout();
            frmAbout2 = new AboutBox1();
        }

        private void btnClickMe_Click(object sender, EventArgs e)
        {
            frmAbout?.Show();
        }

        private void btnClickMe2_Click(object sender, EventArgs e)
        {
            frmAbout2?.Show();
        }
    }
}
