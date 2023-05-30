using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MVVM_28.1_DataGridExt.Views.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void MainWindowTest()
        {
            MainWindow? testView=null;
            var t = new Thread(()=> testView = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(MainWindow));    
        }
    }
}
