using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_18_MultiConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVVM_18_MultiConverters.View.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        MainWindow? testView;

        [TestMethod()]
        public void MainWindowTest()
        {
            testView = null;
            var t = new Thread(() => testView = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(MainWindow));
        }
    }
}