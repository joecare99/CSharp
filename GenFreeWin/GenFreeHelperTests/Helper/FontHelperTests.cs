using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;

namespace GenFree.Helper.Tests
{
    [TestClass()]
    public class FontHelperTests
    {
        [DataTestMethod()]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        public void ChangeFUnderlineTest(bool xAct, bool xExp)
        {
            Font f = new Font("Arial", 12, xAct ? FontStyle.Underline : FontStyle.Regular);
            f = f.ChangeFUnderline(xExp);
            Assert.AreEqual(xExp, f.Underline);
            Assert.AreEqual(false, f.Bold);
            Assert.AreEqual("Arial", f.Name);
            Assert.AreEqual(12, f.Size);
        }

        [DataTestMethod()]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        public void ChangeFBoldTest(bool xAct, bool xExp)
        {
            Font f = new Font("Arial", 12, xAct ? FontStyle.Bold : FontStyle.Regular);
            f = f.ChangeFBold(xExp);
            Assert.AreEqual(false, f.Underline);
            Assert.AreEqual(xExp, f.Bold);
            Assert.AreEqual("Arial", f.Name);
            Assert.AreEqual(12, f.Size);
        }

        [DataTestMethod()]
        [DataRow(12, 14)]
        [DataRow(14, 12)]
        [DataRow(12, 12)]
        public void ChangeFSizeTest(float fAct, float fExp)
        {
            Font f = new Font("Arial", fAct, FontStyle.Regular);
            f = f.ChangeFSize(fExp);
            Assert.AreEqual(false, f.Underline);
            Assert.AreEqual(false, f.Bold);
            Assert.AreEqual("Arial", f.Name);
            Assert.AreEqual(fExp, f.Size);
        }

        [DataTestMethod()]
        [DataRow("Arial", "Times New Roman")]
        [DataRow("Times New Roman", "Arial")]
        [DataRow("Arial", "Arial")]
        public void ChangeFNameTest(string sAct, string sExp)
        {
            Font f = new Font(sAct, 12, FontStyle.Regular);
            f = f.ChangeFName(sExp);
            Assert.AreEqual(false, f.Underline);
            Assert.AreEqual(false, f.Bold);
            Assert.AreEqual(sExp, f.Name);
            Assert.AreEqual(12, f.Size);
        }
    }
}