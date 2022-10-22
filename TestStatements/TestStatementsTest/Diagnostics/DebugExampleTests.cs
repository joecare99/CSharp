using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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