using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Calc32WPF.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        MainWindow testView;

        [TestInitialize]
        public void Init()
        {
            var t=new Thread(()=>
            testView = new MainWindow());
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        [TestMethod()]
        public void MainWindowTest()
        {
            Assert.IsNotNull(testView);
        }
    }
}