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
using CSharpBible.AboutEx.ViewModels.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;
using System.Windows.Forms;

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
        private IAboutViewModel _vm;

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
        private const string cExpectedCompany = "BigCorp.";
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
            _vm = Substitute.For<IAboutViewModel>();
            _vm.Author.Returns(cExpectedProductname);
            _vm.Version.Returns(cExpectedVersion);
            _vm.Company.Returns(cExpectedCompany);
            _vm.Copyright.Returns(cExpectedCopyright);
            _vm.Description.Returns(cExpectedComments);
            frmAbout = new FrmAbout(_vm);
        }

        /// <summary>
        /// Defines the test method TestSetup.
        /// </summary>
        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(frmAbout);
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
        /// Defines the test method BtnOKTest.
        /// </summary>
        [TestMethod()]
        public void BtnOKTest()
        {
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