using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VBUnObfusicator.Tests
{
    [TestClass()]
    public class AppTests
    {
        [TestMethod()]
        public void AppTest()
        {
            var app = new App();
            Assert.IsNotNull(app);
        }
    }
}