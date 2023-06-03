using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MVVM_18_MultiConverters.View.Tests
{
    [TestClass()]
    public class DateDifViewTests
    {
        DateDifView? testView;

        [TestMethod()]
        public void DateDifViewTest()
        {
            testView = null;
            var t = new Thread(() => testView = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(DateDifView));
        }
    }
}