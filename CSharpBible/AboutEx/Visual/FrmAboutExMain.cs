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

        public FrmAboutExMain()
        {
            InitializeComponent();
            frmAbout = new FrmAbout(); 
        }

        private void btnClickMe_Click(object sender, EventArgs e)
        {
            frmAbout?.Show();
        }
    }
}
