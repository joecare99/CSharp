using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class LoopStatementsTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class LoopStatementsTests : ConsoleTestsBase
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

        /// <summary>
        /// Defines the test method DoWhileStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoWhileStatementTest()
        {
            AssertConsoleOutputArgs(Expected1, new string[] { }, LoopStatements.DoWhileStatement);
            AssertConsoleOutputArgs(Expected1a, new string[] { "one" }, LoopStatements.DoWhileStatement);
            AssertConsoleOutputArgs(Expected1b, new string[] { "one", "two" }, LoopStatements.DoWhileStatement);
        }

        /// <summary>
        /// Defines the test method DoDoStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoDoStatementTest()
        {
            AssertConsoleInOutputArgs(Expected2, "one\ntwo\n\n", new string[] { }, LoopStatements.DoDoStatement);
        }


        /// <summary>
        /// Defines the test method DoForStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoForStatementTest()
        {
            AssertConsoleOutputArgs(Expected3, new string[] { }, LoopStatements.DoForStatement);
            AssertConsoleOutputArgs(Expected3a, new string[] { "one" }, LoopStatements.DoForStatement);
            AssertConsoleOutputArgs(Expected3b, new string[] { "one", "two" }, LoopStatements.DoForStatement);
        }

        /// <summary>
        /// Defines the test method DoForEachStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoForEachStatementTest()
        {
            AssertConsoleOutputArgs(Expected4, new string[] { }, LoopStatements.DoForEachStatement);
            AssertConsoleOutputArgs(Expected4a, new string[] { "one" }, LoopStatements.DoForEachStatement);
            AssertConsoleOutputArgs(Expected4b, new string[] { "one" , "two" }, LoopStatements.DoForEachStatement);   
        }

    }
}
