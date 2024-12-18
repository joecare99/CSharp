using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Collection.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestStatements.UnitTesting;

namespace TestStatements.Collection.Generic.Tests
{
    /// <summary>
    /// Defines test class SortedListExampleTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class SortedListExampleTests : ConsoleTestsBase
		{
        private readonly string cExpected1 = "+----------------------------------------------------------\r\n" +
            "| Show Values - list\r\n" +
            "+----------------------------------------------------------\r\n\r\nValue = paint.exe\r\n" +
            "Value = paint.exe\r\n" +
            "Value = winword.exe\r\nValue = hypertrm.exe\r\nValue = wordpad.exe\r\nValue = notepad.exe";
        private readonly string cExpShowValues2 = "+----------------------------------------------------------\r\n" +
            "| Show Values - index\r\n" +
            "+----------------------------------------------------------\r\n\r\n" +
            "Indexed retrieval using the Values property: Values[2] = winword.exe";
        private readonly string cExpShowKeys1 = "+----------------------------------------------------------\r\n" +
            "| Show Keys - list\r\n" +
            "+----------------------------------------------------------\r\n\r\nKey = bmp\r\nKey = dib\r\n" +
            "Key = doc\r\nKey = ht\r\n" +
            "Key = rtf\r\nKey = txt";
        private readonly string cExpShowKeys2 = "+----------------------------------------------------------\r\n" +
            "| Show Keys - index\r\n" +
            "+----------------------------------------------------------\r\n\r\n" +
            "Indexed retrieval using the Keys property: Keys[2] = doc";
        private readonly string cExpShowRemove = "+----------------------------------------------------------\r\n" +
            "| Show Remove \r\n" +
            "+----------------------------------------------------------\r\n\r\nRemove(\"doc\")\r\n" +
            "Key \"doc\" is not found.";
        private readonly string cExpShowForEach = "+----------------------------------------------------------\r\n" +
            "| Show ForEach\r\n" +
            "+----------------------------------------------------------\r\n\r\nKey = bmp, Value = paint.exe\r\n" +
            "Key = dib, Value = paint.exe\r\nKey = doc, Value = winword.exe\r\nKey = ht, Value = hypertrm.exe\r\n" +
            "Key = rtf, Value = wordpad.exe\r\nKey = txt, Value = notepad.exe";
        private readonly string cExpShowContainsKey= @"+----------------------------------------------------------
| Show ContainsKey
+----------------------------------------------------------
Value added for key = ""ht"": hypertrm.exe";
        private readonly string cExpShowTryGetValue = @"+----------------------------------------------------------
| Show TryGetValue
+----------------------------------------------------------
Key = ""tif"" is not found.
For key = ""txt"", value = notepad.exe.";
        private readonly string cExpTestIndexr = "+----------------------------------------------------------\r\n" +
            "| Use Index to access SortedList\r\n" +
            "+----------------------------------------------------------\r\nFor key = \"rtf\", value = wordpad.exe.\r\n" +
            "For key = \"rtf\", value = winword.exe.\r\nKey = \"tif\" is not found.";
        private readonly string cExpTestAddExisting = "+----------------------------------------------------------\r\n" +
            "| Add Existing Value to SortedList\r\n" +
            "+----------------------------------------------------------\r\nAn element with Key = \"txt\" already exists.";
        private readonly string cExpSortedListMain =
            @"======================================================================
## SortedList<TKey,TValue> 
======================================================================

+----------------------------------------------------------
| Add Existing Value to SortedList
+----------------------------------------------------------
An element with Key = ""txt"" already exists.

+----------------------------------------------------------
| Use Index to access SortedList
+----------------------------------------------------------
For key = ""rtf"", value = wordpad.exe.
For key = ""rtf"", value = winword.exe.
Key = ""tif"" is not found.

+----------------------------------------------------------
| Show TryGetValue
+----------------------------------------------------------
Key = ""tif"" is not found.
For key = ""txt"", value = notepad.exe.

+----------------------------------------------------------
| Show ContainsKey
+----------------------------------------------------------
Value added for key = ""ht"": hypertrm.exe

+----------------------------------------------------------
| Show ForEach
+----------------------------------------------------------

Key = bmp, Value = paint.exe
Key = dib, Value = paint.exe
Key = doc, Value = winword.exe
Key = ht, Value = hypertrm.exe
Key = rtf, Value = wordpad.exe
Key = txt, Value = notepad.exe

+----------------------------------------------------------
| Show Values - list
+----------------------------------------------------------

Value = paint.exe
Value = paint.exe
Value = winword.exe
Value = hypertrm.exe
Value = wordpad.exe
Value = notepad.exe

+----------------------------------------------------------
| Show Values - index
+----------------------------------------------------------

Indexed retrieval using the Values property: Values[2] = winword.exe

+----------------------------------------------------------
| Show Keys - list
+----------------------------------------------------------

Key = bmp
Key = dib
Key = doc
Key = ht
Key = rtf
Key = txt

+----------------------------------------------------------
| Show Keys - index
+----------------------------------------------------------

Indexed retrieval using the Keys property: Keys[2] = doc

+----------------------------------------------------------
| Show Remove 
+----------------------------------------------------------

Remove(""doc"")
Key ""doc"" is not found.";

        /// <summary>
        /// Defines the test method SortedListMainTest.
        /// </summary>
        [TestMethod()]
        public void SortedListMainTest()
        {
            AssertConsoleOutput(cExpSortedListMain, SortedListExample.SortedListMain);
        }

        /// <summary>
        /// Defines the test method ShowValues1Test.
        /// </summary>
        [TestMethod()]
        public void ShowValues1Test()
        {
            string Expected = cExpected1;
            Action ToTest = SortedListExample.ShowValues1;
            AssertConsoleOutput(Expected, ToTest);
        }

        /// <summary>
        /// Defines the test method ShowValues2Test.
        /// </summary>
        [TestMethod()]
        public void ShowValues2Test()
        {
            AssertConsoleOutput(cExpShowValues2, SortedListExample.ShowValues2);
        }

        /// <summary>
        /// Defines the test method ShowKeys1Test.
        /// </summary>
        [TestMethod()]
        public void ShowKeys1Test()
        {
            AssertConsoleOutput(cExpShowKeys1, SortedListExample.ShowKeys1);
        }

        /// <summary>
        /// Defines the test method ShowKeys2Test.
        /// </summary>
        [TestMethod()]
        public void ShowKeys2Test()
        {
            AssertConsoleOutput(cExpShowKeys2, SortedListExample.ShowKeys2);
        }

        /// <summary>
        /// Defines the test method ShowRemoveTest.
        /// </summary>
        [TestMethod()]
        public void ShowRemoveTest()
        {
            AssertConsoleOutput(cExpShowRemove, SortedListExample.ShowRemove);
        }

        /// <summary>
        /// Defines the test method ShowForEachTest.
        /// </summary>
        [TestMethod()]
        public void ShowForEachTest()
        {
            AssertConsoleOutput(cExpShowForEach, SortedListExample.ShowForEach);
        }

        /// <summary>
        /// Defines the test method ShowContainsKeyTest.
        /// </summary>
        [TestMethod()]
        public void ShowContainsKeyTest()
        {
            AssertConsoleOutput(cExpShowContainsKey, SortedListExample.ShowContainsKey);
        }

        /// <summary>
        /// Defines the test method ShowTryGetValueTest.
        /// </summary>
        [TestMethod()]
        public void ShowTryGetValueTest()
        {
            AssertConsoleOutput(cExpShowTryGetValue, SortedListExample.ShowTryGetValue);
        }

        /// <summary>
        /// Defines the test method TestIndexrTest.
        /// </summary>
        [TestMethod()]
        public void TestIndexrTest()
        {
            AssertConsoleOutput(cExpTestIndexr, SortedListExample.TestIndexr);
        }

        /// <summary>
        /// Defines the test method TestAddExistingTest.
        /// </summary>
        [TestMethod()]
        public void TestAddExistingTest()
        {
            AssertConsoleOutput(cExpTestAddExisting, SortedListExample.TestAddExisting);
        }
    }
}
