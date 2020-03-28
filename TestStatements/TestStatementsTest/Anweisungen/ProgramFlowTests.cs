using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class ProgramFlowTests
    {
        private readonly string cExpected1=
            "======================================================================\r\n## Beispiel für Break-Anweisung \r\n" +
            "======================================================================\r\nHello World";
        private readonly string cExpected2=
            "======================================================================\r\n## Beispiel für Continue-Anweisung \r\n" +
            "======================================================================";
        private readonly string cExpected3=
            "======================================================================\r\n## Beispiel für Goto-Anweisung \r\n" +
            "======================================================================";
        private readonly string cInput="Hello World\n\n";
        private readonly string cExpected1b=
            "======================================================================\r\n## Beispiel für Break-Anweisung \r\n" +
            "======================================================================\r\nThis\r\nis\r\na\r\nwonderfull\r\nDay\r\n!";
        private readonly string cInputb= "This\r\nis\r\na\r\nwonderfull\r\nDay\r\n!\r\n\r\n";

        [TestMethod()]
        public void DoBreakStatementTest()
        {
            AssertConsoleInOutputArgs(cExpected1, cInput, new string[] { }, ProgramFlow.DoBreakStatement);
            AssertConsoleInOutputArgs(cExpected1b, cInputb, new string[] { }, ProgramFlow.DoBreakStatement);
        }

        [TestMethod()]
        public void DoContinueStatementTest()
        {
            AssertConsoleOutputArgs(cExpected2, new string[] { }, ProgramFlow.DoContinueStatement);
        }

        [TestMethod()]
        public void DoGoToStatementTest()
        {
            AssertConsoleOutputArgs(cExpected3, new string[] { }, ProgramFlow.DoGoToStatement);
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