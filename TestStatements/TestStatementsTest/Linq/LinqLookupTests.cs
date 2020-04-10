using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.ConsoleAsserts;

namespace TestStatements.Linq.Tests
{
    [TestClass()]
    public class LinqLookupTests : TestConsole
    {
        private readonly string cExpLookupExample =
            "======================================================================\r\n## Show HashSet<T> \r\n======================================================================\r\n\r\n+----------------------------------------------------------\r\n| Show Grouping Example\r\n+----------------------------------------------------------\r\nC\r\n    Coho Vineyard 89453312\r\n    Contoso Pharmaceuticals 670053128\r\nL\r\n    Lucerne Publishing 89112755\r\nW\r\n    Wingtip Toys 299456122\r\n    Wide World Importers 4665518773\r\n\r\n+----------------------------------------------------------\r\n| Show Count of Lookup\r\n+----------------------------------------------------------\r\n    3\r\n\r\n+----------------------------------------------------------\r\n| Show IEnumerable\r\n+----------------------------------------------------------\r\n\nPackages that have a key of 'C':\r\nCoho Vineyard 89453312\r\nContoso Pharmaceuticals 670053128\r\n\r\n+----------------------------------------------------------\r\n| Show Contains\r\n+----------------------------------------------------------\r\nThere's no Group with the Key of \"G\"";
        private readonly string cExpShowContains =
            "+----------------------------------------------------------\r\n| Show Contains\r\n+----------------------------------------------------------\r\nThere's no Group with the Key of \"G\"";
        private readonly string cExpShowIEnumerable =
            "+----------------------------------------------------------\r\n| Show IEnumerable\r\n+----------------------------------------------------------\r\n\nPackages that have a key of 'C':\r\nCoho Vineyard 89453312\r\nContoso Pharmaceuticals 670053128";
        private readonly string cExpShowCount =
            "+----------------------------------------------------------\r\n| Show Count of Lookup\r\n+----------------------------------------------------------\r\n    3";
        private readonly string cExpShowGrouping =
            "+----------------------------------------------------------\r\n| Show Grouping Example\r\n+----------------------------------------------------------\r\nC\r\n    Coho Vineyard 89453312\r\n    Contoso Pharmaceuticals 670053128\r\nL\r\n    Lucerne Publishing 89112755\r\nW\r\n    Wingtip Toys 299456122\r\n    Wide World Importers 4665518773";

        [TestMethod()]
        public void LookupExampleTest()
        {
            AssertConsoleOutput(cExpLookupExample, LinqLookup.LookupExample);
        }

        [TestMethod()]
        public void ShowContainsTest()
        {
            AssertConsoleOutput(cExpShowContains, LinqLookup.ShowContains);
        }

        [TestMethod()]
        public void ShowIEnumerableTest()
        {
            AssertConsoleOutput(cExpShowIEnumerable, LinqLookup.ShowIEnumerable);
        }

        [TestMethod()]
        public void ShowCountTest()
        {
            AssertConsoleOutput(cExpShowCount, LinqLookup.ShowCount);
        }

        [TestMethod()]
        public void ShowGroupingTest()
        {
            AssertConsoleOutput(cExpShowGrouping, LinqLookup.ShowGrouping);
        }

    }
}