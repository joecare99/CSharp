using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSQBrowser.ViewModels.Interfaces;

namespace MSQBrowser.Tests
{
    [TestClass()]
    public class AppTests
    {
        [TestMethod()]
        public void AppTest()
        {
            var testClass = new App();
            Assert.IsNotNull(testClass);
            Assert.IsNotNull(IoC.GetRequiredService<ITableViewViewModel>());
        }
    }
}
