using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace VBUnObfusicator.Tests
{
    [TestClass()]
    public class AppTests
    {
        [TestMethod()]
        public void AppTest()
        {
            App? app = null;
            var thread = new Thread(() =>
            {
                WpfTestApplication.EnsureCreated();
                app = WpfTestApplication.Instance;
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            Assert.IsNotNull(app);
        }
    }
}