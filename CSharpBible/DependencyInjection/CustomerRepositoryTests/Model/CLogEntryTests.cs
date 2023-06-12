using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerRepository.Model.Tests
{
    [TestClass]
    public class CLogEntryTests
    {
        [TestMethod]
        public void MessageTest() {
            CLogEntry testModel = new();
            Assert.AreEqual("", testModel.Message);
            testModel.Message = "123";
            Assert.AreEqual("123", testModel.Message);
        }

        [TestMethod]
        public void TimeTest()
        {
            CLogEntry testModel = new();
            Assert.AreEqual(new DateTimeOffset(new DateTime(2000, 1, 1)), testModel.Time);
            testModel.Time = new DateTime(2023,5,1);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2023,5,1)), testModel.Time);
        }
    }
}
