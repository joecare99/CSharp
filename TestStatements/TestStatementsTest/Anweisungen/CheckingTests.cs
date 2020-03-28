using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TestStatements.ConsoleAsserts;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class CheckingTests : TestConsole
    {
        private readonly string cExpected1= "-2147483648\r\nDie arithmetische Operation hat einen Überlauf verursacht.";

        [TestMethod()]
        public void CheckedUncheckedTest()
        {
            AssertConsoleOutputArgs(cExpected1, new string[] { }, Checking.CheckedUnchecked);
        }
    }
}