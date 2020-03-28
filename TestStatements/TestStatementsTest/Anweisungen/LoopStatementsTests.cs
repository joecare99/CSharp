using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Anweisungen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class LoopStatementsTests
    {
        private readonly string Expected1= "======================================================================\r\n" +
            "## Beispiel für While-Schleife \r\n======================================================================";
        private readonly string Expected1a= "======================================================================\r\n" +
            "## Beispiel für While-Schleife \r\n======================================================================\r\none";
        private readonly string Expected1b= "======================================================================\r\n" +
            "## Beispiel für While-Schleife \r\n======================================================================\r\none\r\ntwo";
        private readonly string Expected2= "======================================================================\r\n" +
            "## Beispiel für Do-While Schleife \r\n======================================================================\r\none\r\ntwo";
        private readonly string Expected3= "======================================================================\r\n" +
            "## Beispiel für For-Schleife \r\n======================================================================";
        private readonly string Expected3a = "======================================================================\r\n" +
            "## Beispiel für For-Schleife \r\n======================================================================\r\none";
        private readonly string Expected3b = "======================================================================\r\n" +
            "## Beispiel für For-Schleife \r\n======================================================================\r\none\r\ntwo";
        private readonly string Expected4= "======================================================================\r\n" +
            "## Beispiel für Foreach-Schleife \r\n======================================================================";
        private readonly string Expected4a = "======================================================================\r\n" +
            "## Beispiel für Foreach-Schleife \r\n======================================================================\r\none";
        private readonly string Expected4b = "======================================================================\r\n" +
            "## Beispiel für Foreach-Schleife \r\n======================================================================\r\none\r\ntwo";

        [TestMethod()]
        public void DoWhileStatementTest()
        {
            AssertConsoleOutputArgs(Expected1, new string[] { }, LoopStatements.DoWhileStatement);
            AssertConsoleOutputArgs(Expected1a, new string[] { "one" }, LoopStatements.DoWhileStatement);
            AssertConsoleOutputArgs(Expected1b, new string[] { "one", "two" }, LoopStatements.DoWhileStatement);
        }

        [TestMethod()]
        public void DoDoStatementTest()
        {
            AssertConsoleInOutputArgs(Expected2, "one\ntwo\n\n", new string[] { }, LoopStatements.DoDoStatement);
        }


        [TestMethod()]
        public void DoForStatementTest()
        {
            AssertConsoleOutputArgs(Expected3, new string[] { }, LoopStatements.DoForStatement);
            AssertConsoleOutputArgs(Expected3a, new string[] { "one" }, LoopStatements.DoForStatement);
            AssertConsoleOutputArgs(Expected3b, new string[] { "one", "two" }, LoopStatements.DoForStatement);
        }

        [TestMethod()]
        public void DoForEachStatementTest()
        {
            AssertConsoleOutputArgs(Expected4, new string[] { }, LoopStatements.DoForEachStatement);
            AssertConsoleOutputArgs(Expected4a, new string[] { "one" }, LoopStatements.DoForEachStatement);
            AssertConsoleOutputArgs(Expected4b, new string[] { "one" , "two" }, LoopStatements.DoForEachStatement);   
        }

        private static void AssertConsoleOutput(string Expected, CrossAppDomainDelegate ToTest)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                ToTest?.Invoke();

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        private static void AssertConsoleOutputArgs(string Expected, string[] Args, Action<String[]> ToTest)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                ToTest?.Invoke(Args);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        private void AssertConsoleInOutputArgs(string Expected, string TestInput, string[] Args, Action<string[]> ToTest)
        {
            using (var sw = new StringWriter())
            using (var sr = new StringReader(TestInput))
            {
                Console.SetOut(sw);
                Console.SetIn(sr);

                ToTest?.Invoke(Args);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
    }
}