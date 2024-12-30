using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class DeclarationsTests.
    /// </summary>
    /// <summary>
    /// Defines test class DeclarationsTests.
    /// </summary>
    [TestClass()]
    public class DeclarationsTests
    {

        private const string Expected1 = "======================================================================\r\n" +
                 "## Deklaration von Variablen\r\n" +
                 "======================================================================\r\n6\r";

        /// <summary>
        /// The expected2
        /// </summary>
        private const string Expected2 = "======================================================================\r\n" +
                 "## Deklaration von Konstanten\r\n" +
                 "======================================================================\r\n1963,495";

        /// <summary>
        /// Defines the test method DoVarDeclarationsTest.
        /// </summary>
        [TestMethod()]
        public void DoVarDeclarationsTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                string[] TestArgs = { };
                Declarations.DoVarDeclarations(TestArgs);

                string[] separators = { "\n" };
                var result = sw.ToString().Split(separators,StringSplitOptions.None);
                int i = 0;
                foreach (string s in Expected1.Split(separators, StringSplitOptions.None))
                {
                    Assert.AreEqual(s, result[i++]);
                }
            }
        }

        /// <summary>
        /// Defines the test method DoConstantDeclarationsTest.
        /// </summary>
        [TestMethod()]
        public void DoConstantDeclarationsTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                string[] TestArgs = { };
                Declarations.DoConstantDeclarations(TestArgs);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected2, result);
            }
        }
    }
}