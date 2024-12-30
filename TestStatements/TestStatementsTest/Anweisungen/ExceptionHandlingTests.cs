using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private const string cExpected1= "Two numbers required\r\nGood bye!";
        private const string cExpected1a= "0,5\r\nGood bye!";

		private const string cExpected2 = "Good bye!";
		private const string cExpected2a = "The parameter is: (1)\r\nGood bye!";
		private const string cExpected2c = "The parameter is: (2)\r\nGood bye!";
		private const string cExpected2b = "The first parameters are: (1) and (2)\r\nGood bye!";

		/// <summary>
		/// Defines the test method DoTryCatchTest.
		/// </summary>
		[DataTestMethod()]
		[TestProperty("Author","J.C.")]
		[DataRow("Empty", cExpected1, new string[] { })]
		[DataRow("(1)", cExpected1, new string[] { "1"})]
		[DataRow("(2)", cExpected1, new string[] { "2"})]
		[DataRow("(1,2)", cExpected1a, new string[] { "1", "2" })]
		public void DoTryCatchTest(string name,string sExp, string[] args)
        {
            AssertConsoleOutputArgs(sExp, args, ExceptionHandling.DoTryCatch);
        }

		/// <summary>
		/// Defines the test method DoTryCatchTest.
		/// </summary>
		[DataTestMethod()]
		[TestProperty("Author", "J.C.")]
		[DataRow("Empty", cExpected2, new string[] { })]
		[DataRow("(1)", cExpected2a, new string[] { "1" })]
		[DataRow("(2)", cExpected2c, new string[] { "2" })]
		[DataRow("(1,2)", cExpected2b, new string[] { "1", "2" })]
		public void DoTryFinallyTest(string name, string sExp, string[] args) {
			AssertConsoleOutputArgs(sExp, args, ExceptionHandling.DoTryFinally);
		}
	}
}
