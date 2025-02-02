using System;
using System.Windows.Forms;
using TestProjLib;
using TestProjLib2;

namespace TestProjDep.Views
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.Text = $"{TestProLibClass.GetTestString()}{Environment.NewLine}{TestProLib2Class.GetTestString()}";
        }
    }
}
