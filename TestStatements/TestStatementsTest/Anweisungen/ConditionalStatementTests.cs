using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class ConditionalStatementTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class ConditionalStatementTests : ConsoleTestsBase 
		{
        private const string Expected1 =
            "======================================================================\r\n" +
            "## Auswertung von Bedingungen (IF-Anweisung) \r\n" +
            "======================================================================\r\n" +
            "No arguments",

         Expected1a =
            "======================================================================\r\n" +
                "## Auswertung von Bedingungen (IF-Anweisung) \r\n" +
                "======================================================================\r\n" +
            "One or more arguments",

         Expected2If1 =
			/*  "======================================================================\r\n" +
				  "## Auswertung von Bedingungen (IF-Anweisung) \r\n" +
				  "======================================================================\r\n" +
			  "One or more arguments",*/
			"======================================================================\r\n" +
			"## Auswertung von Bedingungen (IF-Anweisung) \r\n" +
			"======================================================================\r\n" +
			"No arguments",

		 Expected2If2 =
            "======================================================================\r\n" +
                "## Auswertung von Bedingungen (IF-Anweisung) \r\n" +
                "======================================================================\r\n" +
            "One or more arguments",

         Expected2If3 =
            "======================================================================\r\n" +
                "## Auswertung von Bedingungen (IF-Anweisung) \r\n" +
                "======================================================================\r\n" +
            "One or more arguments",

        Expected3 = "======================================================================\r\n" +
                "## Auswertung von Bedingungen (SWITCH-Anweisung) \r\n" +
                "======================================================================\r\nNo arguments",

        Expected4 = "======================================================================\r\n" +
                "## Auswertung von Bedingungen (SWITCH-Anweisung) \r\n" +
                "======================================================================\r\nOne argument",

        Expected5 = "======================================================================\r\n" +
                "## Auswertung von Bedingungen (SWITCH-Anweisung) \r\n" +
                "======================================================================\r\n2 arguments";

        /// <summary>
        /// Defines the test method DoIfStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoIfStatementTest()
        {
            AssertConsoleOutputArgs(Expected1, new string[]{ }, ConditionalStatement.DoIfStatement);
            AssertConsoleOutputArgs(Expected1a, new string[]{ "one" }, ConditionalStatement.DoIfStatement);
        }

        /// <summary>
        /// Defines the test method DoIfStatementTest2.
        /// </summary>
        [TestMethod()]
        public void DoIfStatementTest2()
        {
            AssertConsoleOutputArgs(Expected2If1, new string[] { }, ConditionalStatement.DoIfStatement);
            AssertConsoleOutputArgs(Expected2If2, new string[] { "one" }, ConditionalStatement.DoIfStatement);
            AssertConsoleOutputArgs(Expected2If3, new string[] { "one","Two" }, ConditionalStatement.DoIfStatement);
        }

        /// <summary>
        /// Defines the test method DoSwitchStatementTest.
        /// </summary>
        [TestMethod()]
        public void DoSwitchStatementTest()
        {            
            AssertConsoleOutputArgs(Expected3,new string[]{  },ConditionalStatement.DoSwitchStatement);
            AssertConsoleOutputArgs(Expected4,new string[]{ "one" },ConditionalStatement.DoSwitchStatement);
            AssertConsoleOutputArgs(Expected5,new string[]{ "one", "two" },ConditionalStatement.DoSwitchStatement);
        }
    }
}
