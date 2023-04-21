using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc32WPF.Properties.Tests
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
