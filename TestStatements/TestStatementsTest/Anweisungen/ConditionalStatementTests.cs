using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TestStatements.ConsoleAsserts;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class ConditionalStatementTests : TestConsole
    {
        private const string Expected1 =
            "======================================================================\r\n" +
            "## Auswertung von Bedingungen (IF-Anweisung) \r\n" +
            "======================================================================\r\n" +
            "No arguments",

         Expected2 =
            "======================================================================\r\n" +
                "## Auswertung von Bedingungen (IF-Anweisung) \r\n" +
                "======================================================================\r\nOne or more arguments",

        Expected3 = "======================================================================\r\n" +
                "## Auswertung von Bedingungen (SWITCH-Anweisung) \r\n" +
                "======================================================================\r\nNo arguments",

        Expected4 = "======================================================================\r\n" +
                "## Auswertung von Bedingungen (SWITCH-Anweisung) \r\n" +
                "======================================================================\r\nOne argument",

        Expected5 = "======================================================================\r\n" +
                "## Auswertung von Bedingungen (SWITCH-Anweisung) \r\n" +
                "======================================================================\r\n2 arguments";

        [TestMethod()]
        public void DoIfStatementTest()
        {
            AssertConsoleOutputArgs(Expected1, new string[]{ }, ConditionalStatement.DoIfStatement);
            AssertConsoleOutputArgs(Expected2, new string[]{ "one" }, ConditionalStatement.DoIfStatement);
        }

        [TestMethod()]
        public void DoSwitchStatementTest()
        {            
            AssertConsoleOutputArgs(Expected3,new string[]{  },ConditionalStatement.DoSwitchStatement);
            AssertConsoleOutputArgs(Expected4,new string[]{ "one" },ConditionalStatement.DoSwitchStatement);
            AssertConsoleOutputArgs(Expected5,new string[]{ "one", "two" },ConditionalStatement.DoSwitchStatement);
        }
    }
}