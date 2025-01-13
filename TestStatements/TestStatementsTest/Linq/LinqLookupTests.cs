using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace TestStatements.Linq.Tests
{
    /// <summary>
    /// Defines test class LinqLookupTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class LinqLookupTests : ConsoleTestsBase
    {
        private readonly string cExpLookupExample =
            "======================================================================\r\n## Show HashSet<T>\r\n======================================================================\r\n\r\n+----------------------------------------------------------\r\n| Show Grouping Example\r\n+----------------------------------------------------------\r\nC\r\n    Coho Vineyard 89453312\r\n    Contoso Pharmaceuticals 670053128\r\nL\r\n    Lucerne Publishing 89112755\r\nW\r\n    Wingtip Toys 299456122\r\n    Wide World Importers 4665518773\r\n\r\n+----------------------------------------------------------\r\n| Show Count of Lookup\r\n+----------------------------------------------------------\r\n    3\r\n\r\n+----------------------------------------------------------\r\n| Show IEnumerable\r\n+----------------------------------------------------------\r\n\nPackages that have a key of 'C':\r\nCoho Vineyard 89453312\r\nContoso Pharmaceuticals 670053128\r\n\r\n+----------------------------------------------------------\r\n| Show Contains\r\n+----------------------------------------------------------\r\nThere's no Group with the Key of \"G\"";
        private readonly string cExpShowContains =
            "+----------------------------------------------------------\r\n| Show Contains\r\n+----------------------------------------------------------\r\nThere's no Group with the Key of \"G\"";
        private readonly string cExpShowIEnumerable =
            "+----------------------------------------------------------\r\n| Show IEnumerable\r\n+----------------------------------------------------------\r\n\nPackages that have a key of 'C':\r\nCoho Vineyard 89453312\r\nContoso Pharmaceuticals 670053128";
        private readonly string cExpShowCount =
            "+----------------------------------------------------------\r\n| Show Count of Lookup\r\n+----------------------------------------------------------\r\n    3";
        private readonly string cExpShowGrouping =
            "+----------------------------------------------------------\r\n| Show Grouping Example\r\n+----------------------------------------------------------\r\nC\r\n    Coho Vineyard 89453312\r\n    Contoso Pharmaceuticals 670053128\r\nL\r\n    Lucerne Publishing 89112755\r\nW\r\n    Wingtip Toys 299456122\r\n    Wide World Importers 4665518773";

        /// <summary>
        /// Defines the test method LookupExampleTest.
        /// </summary>
        [TestMethod()]
        public void LookupExampleTest()
        {
            AssertConsoleOutput(cExpLookupExample, LinqLookup.LookupExample);
        }

        /// <summary>
        /// Defines the test method ShowContainsTest.
        /// </summary>
        [TestMethod()]
        public void ShowContainsTest()
        {
            AssertConsoleOutput(cExpShowContains, LinqLookup.ShowContains);
        }

        /// <summary>
        /// Defines the test method ShowIEnumerableTest.
        /// </summary>
        [TestMethod()]
        public void ShowIEnumerableTest()
        {
            AssertConsoleOutput(cExpShowIEnumerable, LinqLookup.ShowIEnumerable);
        }

        /// <summary>
        /// Defines the test method ShowCountTest.
        /// </summary>
        [TestMethod()]
        public void ShowCountTest()
        {
            AssertConsoleOutput(cExpShowCount, LinqLookup.ShowCount);
        }

        /// <summary>
        /// Defines the test method ShowGroupingTest.
        /// </summary>
        [TestMethod()]
        public void ShowGroupingTest()
        {
            AssertConsoleOutput(cExpShowGrouping, LinqLookup.ShowGrouping);
        }

    }
}