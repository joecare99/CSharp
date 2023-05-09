using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestStatements.UnitTesting {
	/// <summary>
	/// Class ConsoleTestsBase.
	/// </summary>
	public class ConsoleTestsBase {
		protected static void AssertConsoleOutput(string Expected, Action ToTest) {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            ToTest?.Invoke();

            var result = sw.ToString().Trim();
            Assert.AreEqual(Expected, result);
        }

		protected static void AssertConsoleOutputArgs(string Expected, string[] Args, Action<String[]> ToTest) {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            ToTest?.Invoke(Args);

            var result = sw.ToString().Trim();
            Assert.AreEqual(Expected, result);
        }
		protected void AssertConsoleInOutputArgs(string Expected, string TestInput, string[] Args, Action<string[]> ToTest) {
            using var sw = new StringWriter();
            using var sr = new StringReader(TestInput);
            Console.SetOut(sw);
            Console.SetIn(sr);

            ToTest?.Invoke(Args);

            var result = sw.ToString().Trim();
            Assert.AreEqual(Expected, result);
        }
	}
}
