using CSharpBible.Calc32;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calc32.Tests
{
    class testFrm : System.Windows.Forms.Form
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Close();
        }
    }
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void MainTest()
        {
            var f = Program.GetMainForm();
            Program.GetMainForm = () => new testFrm();
            Program.Main();
        }
    }
}
