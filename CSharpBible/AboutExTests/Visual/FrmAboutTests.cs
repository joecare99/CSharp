using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpBible.AboutEx.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CSharpBible.AboutEx.Visual.Tests
{
    [TestClass()]
    public class FrmAboutTests
    {
        private FrmAbout frmAbout;
        private const string cExpectedProductname= "About Example";
        private const string cExpectedVersion= "Code name Siberia aka. DuckCalc98";
        private const string cExpectedCopyright= "Copyright © 2001 by the Ugly Ducking";
        private const string cExpectedComments= "Beta Test Version.\n!!! CONFIDENTIAL !!!\n Do not copy. Do not trade. All rights re" +
    "served.\n This means you.\n Violators shot at dawn.";
        private const string cSomeOtherProduct = "Some other Product";
        private const string cSomeVersion = "The brandnew Version";
        private const string cSomeCopyright = "Copyright © 2020 by Joe Care";
        private readonly string cSomeComments ="Alpha version";

        [TestInitialize()]
        public void Init()
        {
            frmAbout = new FrmAbout();
        }

        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(frmAbout);
            Assert.AreEqual(cExpectedProductname, frmAbout.ProductName);
            Assert.AreEqual(cExpectedVersion, frmAbout.Version);
            Assert.AreEqual(cExpectedCopyright, frmAbout.Copyright);
            Assert.AreEqual(cExpectedComments, frmAbout.Comments);
        }

        [TestMethod()]
        public void FrmAboutTest()
        {
            Assert.IsInstanceOfType(frmAbout, typeof(FrmAbout));
        }

        [TestMethod()]
        public void ProductNameTest()
        {
            frmAbout.ProductName = cSomeOtherProduct;
            Assert.AreEqual(cSomeOtherProduct, frmAbout.ProductName);
        }

        [TestMethod()]
        public void VersionTest()
        {
            frmAbout.Version = cSomeVersion;
            Assert.AreEqual(cSomeVersion, frmAbout.Version);
        }

        [TestMethod()]
        public void CopyrightTest()
        {
            frmAbout.Copyright = cSomeCopyright;
            Assert.AreEqual(cSomeCopyright, frmAbout.Copyright);
        }

        [TestMethod()]
        public void BtnOKTest()
        {
            frmAbout.ProductName = cSomeOtherProduct;
            frmAbout.Version = cSomeVersion;
            frmAbout.Copyright = cSomeCopyright;
            frmAbout.Comments = cSomeComments;
            frmAbout.Show();
            for (int i = 0; i < 50; i++)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
            frmAbout.btnOK_Click(null,null);
            Assert.IsFalse(frmAbout.Visible);
        }

    }
}