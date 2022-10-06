using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TestStatements.UnitTesting;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class ExceptionHandlingTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class ExceptionHandlingTests : ConsoleTestsBase 
		{
        private readonly string cExpected1= "Two numbers required\r\nGood bye!";
        private readonly string cExpected1a= "0,5\r\nGood bye!";

        /// <summary>
        /// Defines the test method DoTryCatchTest.
        /// </summary>
        [TestMethod()]
        public void DoTryCatchTest()
        {
            AssertConsoleOutputArgs(cExpected1, new string[] { }, ExceptionHandling.DoTryCatch);
            AssertConsoleOutputArgs(cExpected1, new string[] {"1" }, ExceptionHandling.DoTryCatch);
            AssertConsoleOutputArgs(cExpected1a, new string[] { "1","2"}, ExceptionHandling.DoTryCatch);
        }
    }
}
