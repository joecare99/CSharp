using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MVVM_31a_CTValidation2.View.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void MainWindowTest()
        {
            MainWindow? mw=null;
            var t = new Thread(()=> mw = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(mw);
            Assert.IsInstanceOfType(mw, typeof(MainWindow));    
        }
    }
}
