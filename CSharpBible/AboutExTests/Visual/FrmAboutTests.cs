// ***********************************************************************
// Assembly         : AboutExTests
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 08-12-2022
// ***********************************************************************
// <copyright file="FrmAboutTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace CSharpBible.AboutEx.Visual.Tests
{
    /// <summary>
    /// Defines test class FrmAboutTests.
    /// </summary>
    [TestClass()]
    public class FrmAboutTests
    {
        /// <summary>
        /// The FRM about
        /// </summary>
        private FrmAbout frmAbout;
        /// <summary>
        /// The c expected productname
        /// </summary>
        private const string cExpectedProductname= "About Example";
        /// <summary>
        /// The c expected version
        /// </summary>
        private const string cExpectedVersion= "Code name Siberia aka. DuckCalc98";
        /// <summary>
        /// The c expected copyright
        /// </summary>
        private const string cExpectedCopyright= "Copyleft © 2001 by the Ugly Ducking";
        /// <summary>
        /// The c expected comments
        /// </summary>
        private const string cExpectedComments= "Beta Test Version.\n!!! CONFIDENTIAL !!!\n Do not copy. Do not trade. All rights re" +
    "served.\n This means you.\n Violators shot at dawn.";
        /// <summary>
        /// The c some other product
        /// </summary>
        private const string cSomeOtherProduct = "Some other Product";
        /// <summary>
        /// The c some version
        /// </summary>
        private const string cSomeVersion = "The brandnew Version";
        /// <summary>
        /// The c some copyright
        /// </summary>
        private const string cSomeCopyright = "Copyright © 2020 by Joe Care";
        /// <summary>
        /// The c some comments
        /// </summary>
        private readonly string cSomeComments ="Alpha version";

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            frmAbout = new FrmAbout();
        }

        /// <summary>
        /// Defines the test method TestSetup.
        /// </summary>
        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(frmAbout);
            Assert.AreEqual(cExpectedProductname, frmAbout.ProductName);
            Assert.AreEqual(cExpectedVersion, frmAbout.Version);
            Assert.AreEqual(cExpectedCopyright, frmAbout.Copyright);
            Assert.AreEqual(cExpectedComments, frmAbout.Comments);
        }

        /// <summary>
        /// Defines the test method FrmAboutTest.
        /// </summary>
        [TestMethod()]
        public void FrmAboutTest()
        {
            Assert.IsInstanceOfType(frmAbout, typeof(FrmAbout));
        }

        /// <summary>
        /// Defines the test method ProductNameTest.
        /// </summary>
        [TestMethod()]
        public void ProductNameTest()
        {
            frmAbout.ProductName = cSomeOtherProduct;
            Assert.AreEqual(cSomeOtherProduct, frmAbout.ProductName);
        }

        /// <summary>
        /// Defines the test method VersionTest.
        /// </summary>
        [TestMethod()]
        public void VersionTest()
        {
            frmAbout.Version = cSomeVersion;
            Assert.AreEqual(cSomeVersion, frmAbout.Version);
        }

        /// <summary>
        /// Defines the test method CopyrightTest.
        /// </summary>
        [TestMethod()]
        public void CopyrightTest()
        {
            frmAbout.Copyright = cSomeCopyright;
            Assert.AreEqual(cSomeCopyright, frmAbout.Copyright);
        }

        /// <summary>
        /// Defines the test method BtnOKTest.
        /// </summary>
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