using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TestStatements.ConsoleAsserts;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class ExceptionHandlingTests : TestConsole
    {
        private readonly string cExpected1= "Two numbers required\r\nGood bye!";
        private readonly string cExpected1a= "0,5\r\nGood bye!";

        [TestMethod()]
        public void DoTryCatchTest()
        {
            AssertConsoleOutputArgs(cExpected1, new string[] { }, ExceptionHandling.DoTryCatch);
            AssertConsoleOutputArgs(cExpected1, new string[] {"1" }, ExceptionHandling.DoTryCatch);
            AssertConsoleOutputArgs(cExpected1a, new string[] { "1","2"}, ExceptionHandling.DoTryCatch);
        }
    }
}