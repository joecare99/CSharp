using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MVVM_24a_CTUserControl.Views.Tests
{
    [TestClass()]
    public class UserControlViewTests
    {
        [TestMethod()]
        public void MainWindowTest()
        {
			UserControlView? mw=null;
            var t = new Thread(()=> mw = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(mw);
            Assert.IsInstanceOfType(mw, typeof(UserControlView));    
        }
    }
}
