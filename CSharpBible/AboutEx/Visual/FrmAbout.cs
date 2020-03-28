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
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
        }

        public new string ProductName { get => lblProductName.Text; set => lblProductName.Text = value; }
        public string Version { get => lblVersion.Text; set => lblVersion.Text = value; }
        public string Copyright { get => lblCopyright.Text; set => lblCopyright.Text = value; }
        public string Comments { get => lblComments.Text; set => lblComments.Text = value; }

        public void btnOK_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
