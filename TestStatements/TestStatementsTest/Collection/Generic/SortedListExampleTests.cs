using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Collection.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestStatements.ConsoleAsserts;

namespace TestStatements.Collection.Generic.Tests
{
    [TestClass()]
    public class SortedListExampleTests : TestConsole
    {
        private readonly string cExpected1 = "+----------------------------------------------------------\r\n" +
            "| Show Values - list\r\n" +
            "+----------------------------------------------------------\r\n\r\nValue = paint.exe\r\n" +
            "Value = paint.exe\r\n" +
            "Value = winword.exe\r\nValue = hypertrm.exe\r\nValue = wordpad.exe\r\nValue = notepad.exe";
        private readonly string cExpShowValues2 = "+----------------------------------------------------------\r\n" +
            "| Show Values - index\r\n" +
            "+----------------------------------------------------------\r\n\n" +
            "Indexed retrieval using the Values property: Values[2] = winword.exe";
        private readonly string cExpShowKeys1 = "+----------------------------------------------------------\r\n" +
            "| Show Keys - list\r\n" +
            "+----------------------------------------------------------\r\n\r\nKey = bmp\r\nKey = dib\r\n" +
            "Key = doc\r\nKey = ht\r\n" +
            "Key = rtf\r\nKey = txt";
        private readonly string cExpShowKeys2 = "+----------------------------------------------------------\r\n" +
            "| Show Keys - index\r\n" +
            "+----------------------------------------------------------\r\n\n" +
            "Indexed retrieval using the Keys property: Keys[2] = doc";
        private readonly string cExpShowRemove = "+----------------------------------------------------------\r\n" +
            "| Show Remove \r\n" +
            "+----------------------------------------------------------\r\n\nRemove(\"doc\")\r\n" +
            "Key \"doc\" is not found.";
        private readonly string cExpShowForEach = "+----------------------------------------------------------\r\n" +
            "| Show ForEach\r\n" +
            "+----------------------------------------------------------\r\n\r\nKey = bmp, Value = paint.exe\r\n" +
            "Key = dib, Value = paint.exe\r\nKey = doc, Value = winword.exe\r\nKey = ht, Value = hypertrm.exe\r\n" +
            "Key = rtf, Value = wordpad.exe\r\nKey = txt, Value = notepad.exe";
        private readonly string cExpShowContainsKey= "+----------------------------------------------------------\r\n" +
            "| Show ContainsKey\r\n" +
            "+----------------------------------------------------------\r\nValue added for key = \"ht\": hypertrm.exe";
        private readonly string cExpShowTryGetValue = "+----------------------------------------------------------\r\n" +
            "| Show TryGetValue\r\n" +
            "+----------------------------------------------------------\r\nKey = \"tif\" is not found.";
        private readonly string cExpTestIndexr = "+----------------------------------------------------------\r\n" +
            "| Use Index to access SortedList\r\n" +
            "+----------------------------------------------------------\r\nFor key = \"rtf\", value = wordpad.exe.\r\n" +
            "For key = \"rtf\", value = winword.exe.\r\nKey = \"tif\" is not found.";
        private readonly string cExpTestAddExisting = "+----------------------------------------------------------\r\n" +
            "| Add Existing Value to SortedList\r\n" +
            "+----------------------------------------------------------\r\nAn element with Key = \"txt\" already exists.";
        private readonly string cExpSortedListMain =
            "======================================================================\r\n## SortedList<TKey,TValue> \r\n" +
            "======================================================================\r\n\r\n" +
            "+----------------------------------------------------------\r\n| Add Existing Value to SortedList\r\n" +
            "+----------------------------------------------------------\r\nAn element with Key = \"txt\" already exists.\r\n" +
            "\r\n+----------------------------------------------------------\r\n| Use Index to access SortedList\r\n" +
            "+----------------------------------------------------------\r\nFor key = \"rtf\", value = wordpad.exe.\r\n" +
            "For key = \"rtf\", value = winword.exe.\r\nKey = \"tif\" is not found.\r\n\r\n" +
            "+----------------------------------------------------------\r\n| Show TryGetValue\r\n" +
            "+----------------------------------------------------------\r\nKey = \"tif\" is not found.\r\n\r\n" +
            "+----------------------------------------------------------\r\n| Show ContainsKey\r\n" +
            "+----------------------------------------------------------\r\nValue added for key = \"ht\": hypertrm.exe\r\n" +
            "\r\n+----------------------------------------------------------\r\n| Show ForEach\r\n" +
            "+----------------------------------------------------------\r\n\r\nKey = bmp, Value = paint.exe\r\n" +
            "Key = dib, Value = paint.exe\r\nKey = doc, Value = winword.exe\r\nKey = ht, Value = hypertrm.exe\r\n" +
            "Key = rtf, Value = wordpad.exe\r\nKey = txt, Value = notepad.exe\r\n\r\n" +
            "+----------------------------------------------------------\r\n| Show Values - list\r\n" +
            "+----------------------------------------------------------\r\n\r\nValue = paint.exe\r\nValue = paint.exe\r\n" +
            "Value = winword.exe\r\nValue = hypertrm.exe\r\nValue = wordpad.exe\r\nValue = notepad.exe\r\n\r\n" +
            "+----------------------------------------------------------\r\n| Show Values - index\r\n" +
            "+----------------------------------------------------------\r\n\n" +
            "Indexed retrieval using the Values property: Values[2] = winword.exe\r\n\r\n" +
            "+----------------------------------------------------------\r\n| Show Keys - list\r\n" +
            "+----------------------------------------------------------\r\n\r\nKey = bmp\r\nKey = dib\r\nKey = doc\r\n" +
            "Key = ht\r\nKey = rtf\r\nKey = txt\r\n\r\n+----------------------------------------------------------\r\n" +
            "| Show Keys - index\r\n+----------------------------------------------------------\r\n\n" +
            "Indexed retrieval using the Keys property: Keys[2] = doc\r\n\r\n" +
            "+----------------------------------------------------------\r\n| Show Remove \r\n" +
            "+----------------------------------------------------------\r\n\nRemove(\"doc\")\r\n" +
            "Key \"doc\" is not found.";

        [TestMethod()]
        public void SortedListMainTest()
        {
            AssertConsoleOutput(cExpSortedListMain, SortedListExample.SortedListMain);
        }

        [TestMethod()]
        public void ShowValues1Test()
        {
            string Expected = cExpected1;
            CrossAppDomainDelegate ToTest = SortedListExample.ShowValues1;
            AssertConsoleOutput(Expected, ToTest);
        }

        [TestMethod()]
        public void ShowValues2Test()
        {
            AssertConsoleOutput(cExpShowValues2, SortedListExample.ShowValues2);
        }

        [TestMethod()]
        public void ShowKeys1Test()
        {
            AssertConsoleOutput(cExpShowKeys1, SortedListExample.ShowKeys1);
        }

        [TestMethod()]
        public void ShowKeys2Test()
        {
            AssertConsoleOutput(cExpShowKeys2, SortedListExample.ShowKeys2);
        }

        [TestMethod()]
        public void ShowRemoveTest()
        {
            AssertConsoleOutput(cExpShowRemove, SortedListExample.ShowRemove);
        }

        [TestMethod()]
        public void ShowForEachTest()
        {
            AssertConsoleOutput(cExpShowForEach, SortedListExample.ShowForEach);
        }

        [TestMethod()]
        public void ShowContainsKeyTest()
        {
            AssertConsoleOutput(cExpShowContainsKey, SortedListExample.ShowContainsKey);
        }

        [TestMethod()]
        public void ShowTryGetValueTest()
        {
            AssertConsoleOutput(cExpShowTryGetValue, SortedListExample.ShowTryGetValue);
        }

        [TestMethod()]
        public void TestIndexrTest()
        {
            AssertConsoleOutput(cExpTestIndexr, SortedListExample.TestIndexr);
        }

        [TestMethod()]
        public void TestAddExistingTest()
        {
            AssertConsoleOutput(cExpTestAddExisting, SortedListExample.TestAddExisting);
        }
    }
}