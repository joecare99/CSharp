using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestStatements.UnitTesting;

namespace TestStatements.DataTypes.Tests
{
    /// <summary>
    /// Defines test class FormatingTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class FormatingTests : ConsoleTestsBase
    {
        private readonly string cExpCombinedFormating = "======================================================================\r\n## Combined Formating \r\n======================================================================\r\nName = Fred, hours = 12";
#if NET5_0_OR_GREATER
        private readonly string cExpEscapeSequence = "{D}\r\n{6324}\r\n{6324}"; 
#else
        private readonly string cExpEscapeSequence = "{D}\r\n{6324}\r\n{6324 }"; 
#endif
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

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            Formating.GetNow = delegate { return new DateTime(2019, 1, 1); };
        }

        /// <summary>
        /// Defines the test method CombinedFormatingTest.
        /// </summary>
        [TestMethod()]
        public void CombinedFormatingTest()
        {
            AssertConsoleOutput(cExpCombinedFormating, Formating.CombinedFormating);
        }

        /// <summary>
        /// Defines the test method IndexKomponentTest.
        /// </summary>
        [TestMethod()]
        public void IndexKomponentTest()
        {
            AssertConsoleOutput(cExpIndexKomponent, Formating.IndexKomponent);
        }

        /// <summary>
        /// Defines the test method IndexKomponent2Test.
        /// </summary>
        [TestMethod()]
        public void IndexKomponent2Test()
        {
            AssertConsoleOutput(cExpIndexKomponent2, Formating.IndexKomponent2);
        }

        /// <summary>
        /// Defines the test method IndentationKomponentTest.
        /// </summary>
        [TestMethod()]
        public void IndentationKomponentTest()
        {
            AssertConsoleOutput(cExpIndentationKomponent, Formating.IndentationKomponent);
        }

        /// <summary>
        /// Defines the test method EscapeSequenceTest.
        /// </summary>
        [TestMethod()]
        public void EscapeSequenceTest()
        {
            AssertConsoleOutput(cExpEscapeSequence, Formating.EscapeSequence);
        }

        /// <summary>
        /// Defines the test method CodeExamples1Test.
        /// </summary>
        [TestMethod()]
        public void CodeExamples1Test()
        {
            AssertConsoleOutput(cExpCodeExamples1, Formating.CodeExamples1);
        }

        /// <summary>
        /// Defines the test method CodeExamples2Test.
        /// </summary>
        [TestMethod()]
        public void CodeExamples2Test()
        {
            AssertConsoleOutput(cExpCodeExamples2, Formating.CodeExamples2);
        }

        /// <summary>
        /// Defines the test method CodeExamples3Test.
        /// </summary>
        [TestMethod()]
        public void CodeExamples3Test()
        {
            AssertConsoleOutput(cExpCodeExamples3, Formating.CodeExamples3);
        }

        /// <summary>
        /// Defines the test method CodeExamples4Test.
        /// </summary>
        [TestMethod()]
        public void CodeExamples4Test()
        {
            AssertConsoleOutput(cExpCodeExamples4, Formating.CodeExamples4);
        }
    }
}
