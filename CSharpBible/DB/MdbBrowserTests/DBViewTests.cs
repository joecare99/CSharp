using Microsoft.VisualStudio.TestTools.UnitTesting;
using MdbBrowser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using NSubstitute;
using CommonDialogs.Interfaces;

namespace MdbBrowser.Views.Tests
{
    [TestClass()]
    public class DBViewTests
    {
        [TestMethod()]
        public void DBViewTest()
        {
            DBView? mw = null;
            var t = new Thread(() => mw = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(mw);
            Assert.IsInstanceOfType(mw, typeof(DBView));
        }

        [DataTestMethod()]
        [DataRow(true)]
        [DataRow(false)]
        [DataRow(null)]
        public void DoFileDialogTest(bool? xExp)
        {
            DBView? mw = null;
            var t = new Thread(() => mw = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            int _iCnt= 0;
            IFileDialog fd = Substitute.For<IFileDialog>();
            fd.FileName.Returns("testFile");
            fd.ShowDialog(Arg.Any<Window>()).Returns(xExp);
            var result = mw?.DoFileDialog("test", fd, (s,f)=>_iCnt++);
            Assert.AreEqual(xExp, result);
            Assert.AreEqual(xExp ==true?1:0, _iCnt);
        }
    }
}