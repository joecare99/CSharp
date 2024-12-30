using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace TestStatements.Collection.Generic.Tests
{
    /// <summary>
    /// Defines test class DinosaurExampleTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class DinosaurExampleTests : ConsoleTestsBase
    {
        private readonly string cExpListDinos =	"======================================================================\r\n## Dinosaur Example \r\n======================================================================\r\nCapacity: 0\r\nCount: 0\r\n\r\n+----------------------------------------------------------\r\n| Show Create (default) Data\r\n+----------------------------------------------------------\r\n\r\nTyrannosaurus\r\nAmargasaurus\r\nMamenchisaurus\r\nDeinonychus\r\nCompsognathus\r\nCapacity: 8\r\nCount: 5\r\n\r\n+----------------------------------------------------------\r\n| Show Contains\r\n+----------------------------------------------------------\r\n\nContains(\"Deinonychus\"): True\r\n\r\n+----------------------------------------------------------\r\n| Show Insert\r\n+----------------------------------------------------------\r\n\nInsert(2, \"Compsognathus\")\r\n\r\nTyrannosaurus\r\nAmargasaurus\r\nCompsognathus\r\nMamenchisaurus\r\nDeinonychus\r\nCompsognathus\r\nCapacity: 8\r\nCount: 6\r\n\r\n+----------------------------------------------------------\r\n| Show Item-Property\r\n+----------------------------------------------------------\r\n\ndinosaurs[3]: Mamenchisaurus\r\n\r\n+----------------------------------------------------------\r\n| Show Remove\r\n+----------------------------------------------------------\r\n\nRemove(\"Compsognathus\")\r\n\r\nTyrannosaurus\r\nAmargasaurus\r\nMamenchisaurus\r\nDeinonychus\r\nCompsognathus\r\nCapacity: 8\r\nCount: 5\r\n\r\n+----------------------------------------------------------\r\n| Show TrimExcess\r\n+----------------------------------------------------------\r\n\nTrimExcess()\r\n\r\nTyrannosaurus\r\nAmargasaurus\r\nMamenchisaurus\r\nDeinonychus\r\nCompsognathus\r\nCapacity: 5\r\nCount: 5\r\n\r\n+----------------------------------------------------------\r\n| Show Clear\r\n+----------------------------------------------------------\r\n\nClear()\r\n\r\nCapacity: 5\r\nCount: 0";
        private readonly string cExpShowCreateData = "+----------------------------------------------------------\r\n| Show Create (default) Data\r\n+----------------------------------------------------------\r\n\r\nTyrannosaurus\r\nAmargasaurus\r\nMamenchisaurus\r\nDeinonychus\r\nCompsognathus\r\nCapacity: 5\r\nCount: 5";
        private readonly string cExpShowContains = "+----------------------------------------------------------\r\n| Show Contains\r\n+----------------------------------------------------------\r\n\nContains(\"Deinonychus\"): True";
        private readonly string cExpShowInsert = "+----------------------------------------------------------\r\n| Show Insert\r\n+----------------------------------------------------------\r\n\nInsert(2, \"Compsognathus\")\r\n\r\nTyrannosaurus\r\nAmargasaurus\r\nCompsognathus\r\nMamenchisaurus\r\nDeinonychus\r\nCompsognathus\r\nCapacity: 10\r\nCount: 6";
        private readonly string cExpShowItemProperty = "+----------------------------------------------------------\r\n| Show Item-Property\r\n+----------------------------------------------------------\r\n\ndinosaurs[3]: Mamenchisaurus";
        private readonly string cExpShowRemove = "+----------------------------------------------------------\r\n| Show Remove\r\n+----------------------------------------------------------\r\n\nRemove(\"Compsognathus\")\r\n\r\nTyrannosaurus\r\nAmargasaurus\r\nMamenchisaurus\r\nDeinonychus\r\nCompsognathus\r\nCapacity: 10\r\nCount: 5";
        private readonly string cExpShowTrimExcess = "+----------------------------------------------------------\r\n| Show TrimExcess\r\n+----------------------------------------------------------\r\n\nTrimExcess()\r\n\r\nTyrannosaurus\r\nAmargasaurus\r\nMamenchisaurus\r\nDeinonychus\r\nCompsognathus\r\nCapacity: 5\r\nCount: 5";
        private readonly string cExpShowClear = "+----------------------------------------------------------\r\n| Show Clear\r\n+----------------------------------------------------------\r\n\nClear()\r\n\r\nCapacity: 5\r\nCount: 0";

        /// <summary>
        /// Defines the test method ListDinosTest.
        /// </summary>
        [TestMethod()]
        public void ListDinosTest()
        {
            AssertConsoleOutput(cExpListDinos, DinosaurExample.ListDinos);
        }

        /// <summary>
        /// Defines the test method ShowCreateDataTest.
        /// </summary>
        [TestMethod()]
        public void ShowCreateDataTest()
        {
            AssertConsoleOutput(cExpShowCreateData, DinosaurExample.ShowCreateData);
        }

        /// <summary>
        /// Defines the test method ShowContainsTest.
        /// </summary>
        [TestMethod()]
        public void ShowContainsTest()
        {
            AssertConsoleOutput(cExpShowContains, DinosaurExample.ShowContains);
        }

        /// <summary>
        /// Defines the test method ShowInsertTest.
        /// </summary>
        [TestMethod()]
        public void ShowInsertTest()
        {
            AssertConsoleOutput(cExpShowInsert, DinosaurExample.ShowInsert);
        }

        /// <summary>
        /// Defines the test method ShowItemPropertyTest.
        /// </summary>
        [TestMethod()]
        public void ShowItemPropertyTest()
        {
            AssertConsoleOutput(cExpShowItemProperty, DinosaurExample.ShowItemProperty);
        }

        /// <summary>
        /// Defines the test method ShowRemoveTest.
        /// </summary>
        [TestMethod()]
        public void ShowRemoveTest()
        {
            AssertConsoleOutput(cExpShowRemove, DinosaurExample.ShowRemove);
        }

        /// <summary>
        /// Defines the test method ShowTrimExcessTest.
        /// </summary>
        [TestMethod()]
        public void ShowTrimExcessTest()
        {
            AssertConsoleOutput(cExpShowTrimExcess, DinosaurExample.ShowTrimExcess);
        }

        /// <summary>
        /// Defines the test method ShowClearTest.
        /// </summary>
        [TestMethod()]
        public void ShowClearTest()
        {
            AssertConsoleOutput(cExpShowClear, DinosaurExample.ShowClear);
        }
    }
}
