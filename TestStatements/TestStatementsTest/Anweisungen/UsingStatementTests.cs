using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class UsingStatementTests
    {
        private readonly string cExpected1 = "======================================================================\r\n"+
            "## Beispiel für Using-Statement \r\n======================================================================\r\n" +
            "Line one\r\nLine two\r\nLine three";

        [TestMethod()]
        public void DoUsingStatementTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                string[] TestArgs = { };
                UsingStatement.DoUsingStatement(TestArgs);

                var result = sw.ToString().Trim();
                Assert.AreEqual(cExpected1, result);
            }
        }
    }
}