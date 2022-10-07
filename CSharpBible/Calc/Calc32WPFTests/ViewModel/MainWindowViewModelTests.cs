using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc32WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Calc32WPF.ViewModel.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests
    {
        private int _PropChangedCount;
        private string _PropChanged="";
        private MainWindowViewModel _ModelView;

        [TestInitialize]
        public void TestInitialize()
        {
            _PropChangedCount = 0;
            _PropChanged = "";
            _ModelView = new MainWindowViewModel();
            _ModelView.PropertyChanged += MainWindow_VM_PropChanged;            
        }

        private void MainWindow_VM_PropChanged(object sender, PropertyChangedEventArgs e)
        {
            _PropChangedCount++;
            _PropChanged += $"{e.PropertyName}\r\n";
        }

        [TestMethod()]
        public void MainWindowViewModelTestSetup()
        {
            Assert.IsNotNull(_ModelView);
            Assert.AreEqual(0, _PropChangedCount);
        }

        [DataTestMethod()]
        [DataRow("0",0,0)]
        [DataRow("1", 3, 1)]
        [DataRow("2", 3, 2)]
        [DataRow("3", 3, 3)]
        [DataRow("4", 3, 4)]
        [DataRow("5", 3, 5)]
        [DataRow("6", 3, 6)]
        [DataRow("7", 3, 7)]
        [DataRow("8", 3, 8)]
        [DataRow("9", 3, 9)]
        [DataRow("00", 0, 0)]
        [DataRow("01", 3, 1)]
        [DataRow("10", 6, 10)]
        public void MainWindow_VM_NumButton(string sButtons,int iPCCount,int sAkk)
        {
            foreach(var button in sButtons)
            {
                if (button >= '0' && button <= '9')
                    _ModelView.btnNumber.Execute($"{button}");

            }
            Assert.AreEqual(iPCCount, _PropChangedCount);
            Assert.AreEqual(sAkk, _ModelView.Akkumulator);

        }

    }
}