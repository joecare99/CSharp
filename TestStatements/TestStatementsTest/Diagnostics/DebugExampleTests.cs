using TestStatements.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestStatements.UnitTesting;
using System.Diagnostics;

namespace TestStatements.Diagnostics.Tests
{
    [TestClass()]
    public class DebugExampleTests : ConsoleTestsBase
    {
        [TestMethod()]
        [DataRow(1, 1, 1)]
        [DataRow(0, 1, 0)]
        [DataRow(10, 2, 5)]
        [DataRow(10, 0, 0)]
        public void DebugDivideExampleTest(int v1, int v2, int iexp)
        {
            if (v2 == 0 && Debugger.IsAttached)
                Assert.ThrowsException<AssertFailedException>(() => DebugExample.DebugDivideExample(v1, v2));
            else
                Assert.AreEqual(iexp, DebugExample.DebugDivideExample(v1, v2));
        }

        [TestMethod()]
        public void DebugWriteExampleTest()
        {
            AssertConsoleOutput("", () => DebugExample.DebugWriteExample("Test message"));
        }

        [TestMethod()]
        [DataRow(1, 1, 1)]
        [DataRow(0, 1, 0)]
        [DataRow(10, 2, 5)]
        [DataRow(10, 0, 0)]
        public void NormalDivideExampleTest(int v1, int v2,int iexp)
        {
            if (v2 == 0)
                Assert.ThrowsException<DivideByZeroException>(() => DebugExample.NormalDivideExample(v1, v2));
            else
                Assert.AreEqual(iexp, DebugExample.NormalDivideExample(v1, v2));
        }
    }
}