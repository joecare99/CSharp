using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSQBrowser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using NSubstitute;
using CommonDialogs.Interfaces;
using MSQBrowser.ViewModels;

namespace MSQBrowser.Views.Tests
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
        [DataRow(true,false)]
        [DataRow(false)]
        [DataRow(null)]
        public void DoFileDialogTest(bool? xExp,bool xAct=true)
        {
            DBView? mw = null;
            var t = new Thread(() => mw = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            int _iCnt = 0;
            IFileDialog fd = Substitute.For<IFileDialog>();
            fd.FileName.Returns("testFile");
            fd.ShowDialog(Arg.Any<Window>()).Returns(xExp);
            bool? result =  mw?.DoFileDialog("test", fd, xAct? (s, f) => _iCnt++:null);
            Assert.AreEqual(xExp, result);
            Assert.AreEqual(xExp == true && xAct? 1 : 0, _iCnt);
            fd.Received(1).ShowDialog(Arg.Any<Window>());
        }

        [TestMethod()]
        public void Page_LoadedTest()
        {
            DBView? mw = null;
            DBViewViewModel? vm = null;
            var t = new Thread(() => { mw = new(); mw.Page_Loaded(mw, null!);vm = (DBViewViewModel)mw.DataContext; });
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm.FileOpenDialog);
            Assert.IsNotNull(vm.FileSaveAsDialog);
        }
    }
}
