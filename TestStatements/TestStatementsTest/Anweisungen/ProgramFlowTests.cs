using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class ProgramFlowTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class ProgramFlowTests : ConsoleTestsBase
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

        /// <summary>
        /// Defines the test method DoBreakStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoBreakStatementTest()
        {
            AssertConsoleInOutputArgs(cExpected1, cInput, new string[] { }, ProgramFlow.DoBreakStatement);
            AssertConsoleInOutputArgs(cExpected1b, cInputb, new string[] { }, ProgramFlow.DoBreakStatement);
        }

        /// <summary>
        /// Defines the test method DoContinueStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoContinueStatementTest()
        {
            AssertConsoleOutputArgs(cExpected2, new string[] { }, ProgramFlow.DoContinueStatement);
        }

        /// <summary>
        /// Defines the test method DoGoToStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoGoToStatementTest()
        {
            AssertConsoleOutputArgs(cExpected3, new string[] { }, ProgramFlow.DoGoToStatement);
        }

    }
}
