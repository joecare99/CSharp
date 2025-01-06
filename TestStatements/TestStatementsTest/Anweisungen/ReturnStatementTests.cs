using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class ReturnStatementTests.
    /// </summary>
    [TestClass()]
    public class ReturnStatementTests
    {
        private readonly string cExpected1= "======================================================================\r\n"+
            "## Beispiel für Return-Anweisung\r\n======================================================================\r\n3";

        /// <summary>
        /// Defines the test method DoReturnStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoReturnStatementTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                string[] TestArgs = { };
                ReturnStatement.DoReturnStatement(TestArgs);

                var result = sw.ToString().Trim();
                Assert.AreEqual(cExpected1, result);
            }
        }
    }
}