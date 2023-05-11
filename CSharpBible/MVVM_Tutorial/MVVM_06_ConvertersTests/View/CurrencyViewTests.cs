using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVVM_06_Converters.View.Tests
{
    [TestClass]
    public class CurrencyViewTests
    {
        [TestMethod()]
        public void CurrencyViewTest()
        {
            CurrencyView? testView = null;
            var t = new Thread(() => testView = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(CurrencyView));
        }
    }
}
