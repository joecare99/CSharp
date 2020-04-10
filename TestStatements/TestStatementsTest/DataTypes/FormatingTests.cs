using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestStatements.DataTypes.Tests
{
    [TestClass()]
    public class FormatingTests
    {
        private readonly string cExpCombinedFormating = "======================================================================\r\n## Combined Formating \r\n======================================================================\r\nName = Fred, hours = 12";
        private readonly string cExpEscapeSequence = "{D}\r\n{6324}\r\n{6324}";
        private readonly string cExpIndentationKomponent = "Name                 Hours\n\r\nAdam                  40,0\r\n" +
            "Bridgette              6,7\r\nCarla                 40,4\r\n" +
            "Daniel                82,0\r\nEbenezer              40,3\r\n" +
            "Francine              80,0\r\nGeorge                16,8";
        private readonly string cExpIndexKomponent2 = "======================================================================\r\n## Formating with Index 2 \r\n======================================================================\r\n0x7FFFFFFFFFFFFFFF 9,223372E+018 9.223.372.036.854.775.807,00";
        private readonly string cExpIndexKomponent = "======================================================================\r\n## Formating with Index \r\n======================================================================\r\nPrime numbers less than 10: 2, 3, 5, 7";
        private readonly string cExpCodeExamples1= "Dienstag Januar\r\nDienstag Januar";
        private readonly string cExpCodeExamples2= "100,00 €";
        private readonly string cExpCodeExamples3= "Name = Fred, hours = 12, minutes = 00";
        private readonly string cExpCodeExamples4= "First Name = |      Fred|\r\nLast Name =  |     Opals|\r\n" +
            "Price =      |  100,00 €|\r\n\r\nFirst Name = |Fred      |\r\nLast Name =  |Opals     |\r\n" +
            "Price =      |100,00 €  |";

        [TestInitialize()]
        public void Init()
        {
            Formating.GetNow = delegate { return new DateTime(2019, 1, 1); };
        }

        [TestMethod()]
        public void CombinedFormatingTest()
        {
            AssertConsoleOutput(cExpCombinedFormating, Formating.CombinedFormating);
        }

        [TestMethod()]
        public void IndexKomponentTest()
        {
            AssertConsoleOutput(cExpIndexKomponent, Formating.IndexKomponent);
        }

        [TestMethod()]
        public void IndexKomponent2Test()
        {
            AssertConsoleOutput(cExpIndexKomponent2, Formating.IndexKomponent2);
        }

        [TestMethod()]
        public void IndentationKomponentTest()
        {
            AssertConsoleOutput(cExpIndentationKomponent, Formating.IndentationKomponent);
        }

        [TestMethod()]
        public void EscapeSequenceTest()
        {
            AssertConsoleOutput(cExpEscapeSequence, Formating.EscapeSequence);
        }

        [TestMethod()]
        public void CodeExamples1Test()
        {
            AssertConsoleOutput(cExpCodeExamples1, Formating.CodeExamples1);
        }

        [TestMethod()]
        public void CodeExamples2Test()
        {
            AssertConsoleOutput(cExpCodeExamples2, Formating.CodeExamples2);
        }

        [TestMethod()]
        public void CodeExamples3Test()
        {
            AssertConsoleOutput(cExpCodeExamples3, Formating.CodeExamples3);
        }

        [TestMethod()]
        public void CodeExamples4Test()
        {
            AssertConsoleOutput(cExpCodeExamples4, Formating.CodeExamples4);
        }
        private static void AssertConsoleOutput(string Expected, CrossAppDomainDelegate ToTest)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                ToTest?.Invoke();

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        private static void AssertConsoleOutputArgs(string Expected, string[] Args, Action<String[]> ToTest)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                ToTest?.Invoke(Args);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        private void AssertConsoleInOutputArgs(string Expected, string TestInput, string[] Args, Action<string[]> ToTest)
        {
            using (var sw = new StringWriter())
            using (var sr = new StringReader(TestInput))
            {
                Console.SetOut(sw);
                Console.SetIn(sr);

                ToTest?.Invoke(Args);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
    }
}