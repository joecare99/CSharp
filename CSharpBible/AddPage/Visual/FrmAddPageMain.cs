using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpBible.AddPage.Visual
{
    public partial class FrmAddPageMain : Form
    {
        public FrmAddPageMain()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAddPage_Click(object sender, EventArgs e)
        {
            TabPage tb = new TabPage();
            tb.Name = "tabPage" + (tabControl1.TabCount + 1).ToString();
            tb.Padding = new System.Windows.Forms.Padding(3);
            tb.TabIndex = tabControl1.TabCount;
            tb.Text = tb.Name;
            tb.UseVisualStyleBackColor = true;
            tabControl1.Controls.Add(tb);
        }

        private void btnAddControl_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Controls.Count < 1)
            {
                CheckBox cb = new CheckBox();
                cb.AutoSize = true;
                cb.Location = new System.Drawing.Point(200, 100);
                cb.Name = "checkBox1";
                cb.Size = new System.Drawing.Size(113, 24);
                cb.TabIndex = 0;
                cb.Text = "checkBox1";
                cb.UseVisualStyleBackColor = true;
                tabControl1.SelectedTab.Controls.Add(cb);
            }
        }
    }
}
