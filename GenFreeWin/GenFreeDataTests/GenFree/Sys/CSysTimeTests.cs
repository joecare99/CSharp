using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Sys.Tests
{
    [TestClass]
    public class CSysTimeTests
    {

        [TestMethod]
        public void NowTest()
        {
            Assert.IsTrue(new CSysTime().Now > DateTime.MinValue);
        }

        [TestMethod]
        public void DefaultTest()
        {
            Assert.AreEqual(default, new CSysTime().Default);
        }
    }
}
