using BaseLib.Helper;
using CommonDialogs.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_20_Sysdialogs.ViewModel;
using NSubstitute;
using NSubstitute.Core.Arguments;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Controls;

namespace MVVM_20_Sysdialogs.View.Tests
{
    [TestClass()]
    public class SysDialogsTests
    {
        SysDialogs? testView = null;
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private SysDialogsViewModel vm;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private string DebugOut="";

        [TestInitialize()]
        public void Init()
        {
            Thread thread = new(() =>
            {
                testView = new()
                {
                    DataContext = vm = new SysDialogsViewModel()
                };
                testView.Page_Loaded(testView, new System.Windows.RoutedEventArgs());
            });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testView);
            Assert.IsNotNull(vm);
            Assert.IsInstanceOfType(testView, typeof(SysDialogs));
            Assert.IsInstanceOfType(vm, typeof(SysDialogsViewModel));     
        }

        [DataTestMethod()]
        [DataRow(true,false,"")]
        [DataRow(false,true,"")]
        [DataRow(null, true, "")]
        [DataRow(true, true, "OnPrint({0})\r\n")]
        public void DoPrintDialogTest(bool? xRes, bool xAct, string sExp)
        {
            var par = Substitute.For<IPrintDialog>();
            par.PageRangeSelection.Returns(PageRangeSelection.AllPages);
            par.ShowDialog().Returns(xRes);
            Assert.AreEqual(xRes,vm.dPrintDialog?.Invoke(par,xAct? (p) => DoLog($"OnPrint({p})"):null));
            par.Received(1).ShowDialog();
            Assert.AreEqual(sExp.Format(par.ToString()), DebugOut);
        }

        [DataTestMethod()]
        [DataRow(true, false, "")]
        [DataRow(false, true, "")]
        [DataRow(null, true, "")]
        [DataRow(true, true, "OnFont([Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=1, GdiVerticalFont=False],{0})\r\n")]
        public void DoFontDialogTest(bool? xRes, bool xAct, string sExp)
        {
            var par = Substitute.For<IFontDialog>();
            par.Font.Returns(new System.Drawing.Font("Microsoft Sans Serif",8.25f));
            par.ShowDialog().Returns(xRes);
            Assert.AreEqual(xRes, vm.dFontDialog?.Invoke(par.Font, par, xAct? (f,p) => DoLog($"OnFont({f},{p})"):null));
            par.Received(1).ShowDialog();
            Assert.AreEqual(sExp.Format(par.ToString()), DebugOut);
        }

        [DataTestMethod()]
        [DataRow(true, false, "")]
        [DataRow(false, true, "")]
        [DataRow(null, true, "")]
        [DataRow(true, true, "OnColor(Color [DarkBlue],{0})\r\n")]
        public void DoColorDialogTest(bool? xRes, bool xAct, string sExp)
        {
            var par = Substitute.For<IColorDialog>();
            par.Color.Returns(Color.DarkBlue);
            par.ShowDialog().Returns(xRes);
            Assert.AreEqual(xRes, vm.dColorDialog?.Invoke(par.Color, par, xAct? (f, p) => DoLog($"OnColor({f},{p})"):null));
            par.Received(1).ShowDialog();
            Assert.AreEqual(sExp.Format($"{par}"), DebugOut);
        }

        [DataTestMethod()]
        [DataRow(true, false, "")]
        [DataRow(false, true, "")]
        [DataRow(null, true, "")]
        [DataRow(true, true, "OnOpen(.\\SomeFile.txt,{0})\r\n")]
        public void DoOpenDialogTest(bool? xRes, bool xAct, string sExp)
        {
            var par = Substitute.For<IFileDialog>();
            par.FileName.Returns(".\\SomeFile.txt");
            par.ShowDialog(Arg.Any<object>()).Returns(xRes);
            Assert.AreEqual(xRes, vm.FileOpenDialog?.Invoke(par.FileName, par, xAct ? (f, p) => DoLog($"OnOpen({f},{p})") : null));
            par.Received(1).ShowDialog(null);
            Assert.AreEqual(sExp.Format(par.ToString()), DebugOut);
        }

        [DataTestMethod()]
        [DataRow(true, false, "")]
        [DataRow(false, true, "")]
        [DataRow(null, true, "")]
        [DataRow(true, true, "OnBrowse(.\\SomeDir\\,{0})\r\n")]
        public void DoBrowseDirDialogTest(bool? xRes,bool xAct, string sExp)
        {
            var par = Substitute.For<IFileDialog>();
            par.FileName.Returns(".\\SomeDir\\");
            par.ShowDialog().Returns(xRes);
            Assert.AreEqual(xRes, vm.DirectoryBrowseDialog?.Invoke(par.FileName, par, xAct? (f, p) => DoLog($"OnBrowse({f},{p})"):null));
            par.Received(1).ShowDialog();
            Assert.AreEqual(sExp.Format(par.ToString()), DebugOut);
        }
        private void DoLog(string v)
        {
           DebugOut+=$"{v}{Environment.NewLine}";
        }
    }
}