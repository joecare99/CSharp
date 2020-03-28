using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class ExpressionsTests
    {
        private const string Expected = 
            "======================================================================\r\n" +
            "## Beispiel für Ausdrücke \r\n" +
            "======================================================================\r\n123\r\n124";

        [TestMethod()]
        public void DoExpressionsTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                string[] TestArgs = { };
                Expressions.DoExpressions(TestArgs);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
    }
}