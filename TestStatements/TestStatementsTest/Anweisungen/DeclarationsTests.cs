using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class DeclarationsTests
    {

        private const string Expected1 = "======================================================================\r\n" +
                 "## Deklaration von Variablen \r\n" +
                 "======================================================================\r\n6\r";

        private const string Expected2 = "======================================================================\r\n" +
                 "## Deklaration von Konstanten \r\n" +
                 "======================================================================\r\n1963,495";

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