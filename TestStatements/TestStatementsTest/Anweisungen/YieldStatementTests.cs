using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class YieldStatementTests.
    /// </summary>
    [TestClass()]
    public class YieldStatementTests
    {
        private readonly object cExpected1= "======================================================================\r\n"+
            "## Beispiel für Yield-Statement\r\n======================================================================\r\n"+
            "-10\r\n-9\r\n-8\r\n-7\r\n-6\r\n-5\r\n-4\r\n-3\r\n-2\r\n-1\r\n0\r\n1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n9";

        /// <summary>
        /// Defines the test method DoYieldStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoYieldStatementTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                string[] TestArgs = { };
                YieldStatement.DoYieldStatement(TestArgs);

                var result = sw.ToString().Trim();
                Assert.AreEqual(cExpected1, result);
            }
        }
    }
}