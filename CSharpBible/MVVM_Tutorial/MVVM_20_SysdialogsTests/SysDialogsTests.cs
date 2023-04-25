using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_20_Sysdialogs.ViewModel;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

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

        static async void HitEnter()
        {
            SendKeys.Flush();
            await Task.Delay(200);
            SendKeys.SendWait("{ENTER}");
        }

        static async void HitESC()
        {
            SendKeys.Flush();
            await Task.Delay(200);
            SendKeys.SendWait("{ESC}");
        }

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
        [DataRow(false,"")]
        [DataRow(true, "")]
        public void DoPrintDialogTest(bool xRes,string sExp)
        {
            var par = new System.Windows.Controls.PrintDialog()
            {
               // MaxPage =3,
                PageRangeSelection = PageRangeSelection.AllPages
            };
            if (xRes)
                SysDialogsTests.HitEnter();
            else
                SysDialogsTests.HitESC();
            Assert.AreEqual(xRes,vm.dPrintDialog?.Invoke(ref par, (p) => DoLog($"OnPrint({par})")));
            Assert.AreEqual(sExp,DebugOut);
        }

        [DataTestMethod()]
        [DataRow(false, "")]
        [DataRow(true, "")]
        public void DoFontDialogTest(bool xRes, string sExp)
        {
            var par = new CommonDialogs.FontDialog
            {
                Font = System.Drawing.SystemFonts.DefaultFont
            };

            if (xRes)
                SysDialogsTests.HitEnter();
            else
                SysDialogsTests.HitESC();
            Assert.AreEqual(xRes, vm.dFontDialog?.Invoke(par.Font, ref par, (f,p) => DoLog($"OnPrint({f},{par})")));
            Assert.AreEqual(sExp, DebugOut);
        }

        private void DoLog(string v)
        {
           DebugOut+=$"{v}{Environment.NewLine}";
        }
    }
}