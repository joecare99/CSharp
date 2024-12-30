using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestStatements.Diagnostics.Tests
{
    [TestClass()]
    public class DebugExampleTests
    {
        [TestMethod()]
        public void DebugDivideExampleTest()
        {
            Assert.ThrowsException<Exception>(() => DebugExample.DebugDivideExample(10, 0));
        }

        [TestMethod()]
        public void DebugWriteExampleTest()
        {
            Assert.Fail();
        }
    }
}