using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class CheckingTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class CheckingTests : ConsoleTestsBase
    {
        private readonly string cExpected1= "-2147483648\r\nArithmetic operation resulted in an overflow.";

        /// <summary>
        /// Defines the test method CheckedUncheckedTest.
        /// </summary>
        [TestMethod()]
        public void CheckedUncheckedTest()
        {
            AssertConsoleOutputArgs(cExpected1, new string[] { }, Checking.CheckedUnchecked);
        }
    }
}
