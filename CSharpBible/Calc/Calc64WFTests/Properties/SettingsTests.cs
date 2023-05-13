using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calc64WF.Properties.Tests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void SettingsTest()
        {
            Assert.IsNotNull(Settings.Default);
        }
    }
}
