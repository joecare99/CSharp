using Microsoft.VisualStudio.TestTools.UnitTesting;
using VBUnObfusicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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