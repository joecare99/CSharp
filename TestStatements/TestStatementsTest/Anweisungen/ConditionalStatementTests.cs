using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace TestStatements.Anweisungen.Tests;

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
        "## Auswertung von Bedingungen (IF-Anweisung)\r\n" +
        "======================================================================\r\n" +
        "No arguments",

     Expected1a =
        "======================================================================\r\n" +
            "## Auswertung von Bedingungen (IF-Anweisung)\r\n" +
            "======================================================================\r\n" +
        "One or more arguments",

     Expected2If1 =
        "======================================================================\r\n" +
        "## Auswertung von Bedingungen (IF-Anweisung) 2\r\n" +
        "======================================================================\r\n" +
        "No arguments",

     Expected2If2 =
        "======================================================================\r\n" +
            "## Auswertung von Bedingungen (IF-Anweisung) 2\r\n" +
            "======================================================================\r\n" +
        "One argument",

     Expected2If3 =
        "======================================================================\r\n" +
            "## Auswertung von Bedingungen (IF-Anweisung) 2\r\n" +
            "======================================================================\r\n" +
        "Done ...",

     Expected3If1 =
        "======================================================================\r\n" +
        "## Auswertung von Bedingungen (IF-Anweisung) 3\r\n" +
        "======================================================================\r\n" +
        "Done ...\r\nDone ...\r\nDone ...",

    Expected3 = "======================================================================\r\n" +
            "## Auswertung von Bedingungen (SWITCH-Anweisung)\r\n" +
            "======================================================================\r\nNo arguments",

    Expected4 = "======================================================================\r\n" +
            "## Auswertung von Bedingungen (SWITCH-Anweisung)\r\n" +
            "======================================================================\r\nOne argument",

    Expected5 = "======================================================================\r\n" +
            "## Auswertung von Bedingungen (SWITCH-Anweisung)\r\n" +
            "======================================================================\r\n2 arguments";

    /// <summary>
    /// Defines the test method DoIfStatementTest.
    /// </summary>
    [TestMethod()]
    [DataRow(new string[] { }, Expected1)]
    [DataRow(new string[] { "one" }, Expected1a)]
    public void DoIfStatementTest(string[] args, string Exp)
    {
        AssertConsoleOutputArgs(Exp, args, ConditionalStatement.DoIfStatement);
    }

    /// <summary>
    /// Defines the test method DoIfStatementTest2.
    /// </summary>
    [TestMethod()]
    [DataRow(new string[] { }, Expected2If1)]
    [DataRow(new string[] { "one" }, Expected2If2)]
    [DataRow(new string[] { "one", "Two" }, Expected2If3)]
    public void DoIfStatementTest2(string[] args,string Exp)
    {
        AssertConsoleOutputArgs(Exp, args, ConditionalStatement.DoIfStatement2);
    }
       
    /// <summary>
    /// Defines the test method DoIfStatementTest2.
    /// </summary>
    [TestMethod()]
    [DataRow(false,false)]
    [DataRow(true,false)]
    [DataRow(true,true)]
    public void DoIfStatementTest3(bool a,bool b)
    {
        ConditionalStatement.A = a;
        ConditionalStatement.B = b;
        AssertConsoleOutput(Expected3If1, ConditionalStatement.DoIfStatement3);
    }

    /// <summary>
    /// Defines the test method DoSwitchStatementTest.
    /// </summary>
    [TestMethod()]
    public void DoSwitchStatementTest()
    {
        AssertConsoleOutputArgs(Expected3, new string[] { }, ConditionalStatement.DoSwitchStatement);
        AssertConsoleOutputArgs(Expected4, new string[] { "one" }, ConditionalStatement.DoSwitchStatement);
        AssertConsoleOutputArgs(Expected5, new string[] { "one", "two" }, ConditionalStatement.DoSwitchStatement);
    }

    /// <summary>
    /// Defines the test method DoSwitchStatementTest.
    /// </summary>
    [TestMethod()]
    public void DoSwitchStatement2Test()
    {
        const string Expected1 = "======================================================================\r\n" +
            "## Auswertung von Bedingungen (SWITCH-Anweisung) 2\r\n" +
            "======================================================================";
        AssertConsoleOutputArgs(Expected1, new string[] { }, ConditionalStatement.DoSwitchStatement1);
        AssertConsoleOutputArgs(Expected1, new string[] { "one" }, ConditionalStatement.DoSwitchStatement1);
        AssertConsoleOutputArgs(Expected1, new string[] { "one", "two" }, ConditionalStatement.DoSwitchStatement1);
    }
}
