using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Collection.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.UnitTesting;

namespace TestStatements.Collection.Generic.Tests
{
    /// <summary>
    /// Defines test class DictionaryExampleTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class DictionaryExampleTests : ConsoleTestsBase
    {
        private readonly string cExpDictionaryExampleMain =
            "======================================================================\r\n## Dictionary<TKey,TValue> \r\n======================================================================\r\n\r\nKey = txt, Value = notepad.exe\r\nKey = bmp, Value = paint.exe\r\nKey = dib, Value = paint.exe\r\nKey = rtf, Value = wordpad.exe\r\nCount: 4\r\n\r\n+----------------------------------------------------------\r\n| Show Try to add Existing\r\n+----------------------------------------------------------\r\nAn element with Key = \"txt\" already exists.\r\n\r\n+----------------------------------------------------------\r\n| Show Index (get value)\r\n+----------------------------------------------------------\r\nFor key = \"rtf\", value = wordpad.exe.\r\n\r\n+----------------------------------------------------------\r\n| Show Index (change value)\r\n+----------------------------------------------------------\r\nFor key = \"rtf\", value = winword.exe.\r\n\r\nKey = txt, Value = notepad.exe\r\nKey = bmp, Value = paint.exe\r\nKey = dib, Value = paint.exe\r\nKey = rtf, Value = winword.exe\r\nCount: 4\r\n\r\n+----------------------------------------------------------\r\n| Show Index (add value)\r\n+----------------------------------------------------------\r\n\r\nKey = txt, Value = notepad.exe\r\nKey = bmp, Value = paint.exe\r\nKey = dib, Value = paint.exe\r\nKey = rtf, Value = winword.exe\r\nKey = doc, Value = winword.exe\r\nCount: 5\r\n\r\n+----------------------------------------------------------\r\n| Show Index (get exception)\r\n+----------------------------------------------------------\r\nKey = \"tif\" is not found.\r\n\r\n+----------------------------------------------------------\r\n| Show TryGetValue\r\n+----------------------------------------------------------\r\nKey = \"tif\" is not found.\r\n\r\n+----------------------------------------------------------\r\n| Show ContainsKey\r\n+----------------------------------------------------------\r\nValue added for key = \"ht\": hypertrm.exe\r\n\r\nKey = txt, Value = notepad.exe\r\nKey = bmp, Value = paint.exe\r\nKey = dib, Value = paint.exe\r\nKey = rtf, Value = winword.exe\r\nKey = doc, Value = winword.exe\r\nKey = ht, Value = hypertrm.exe\r\nCount: 6\r\n\r\n+----------------------------------------------------------\r\n| Show ValueCollection\r\n+----------------------------------------------------------\r\n\r\nValue = notepad.exe\r\nValue = paint.exe\r\nValue = paint.exe\r\nValue = winword.exe\r\nValue = winword.exe\r\nValue = hypertrm.exe\r\n\r\n+----------------------------------------------------------\r\n| Show KeyCollection\r\n+----------------------------------------------------------\r\n\r\nKey = txt\r\nKey = bmp\r\nKey = dib\r\nKey = rtf\r\nKey = doc\r\nKey = ht\r\n\r\n+----------------------------------------------------------\r\n| Show Remove\r\n+----------------------------------------------------------\r\n\nRemove(\"doc\")\r\nKey \"doc\" is not found.\r\n\r\nKey = txt, Value = notepad.exe\r\nKey = bmp, Value = paint.exe\r\nKey = dib, Value = paint.exe\r\nKey = rtf, Value = winword.exe\r\nKey = ht, Value = hypertrm.exe\r\nCount: 5";
        private readonly string cExpTryAddExisting = 
            "+----------------------------------------------------------\r\n| Show Try to add Existing\r\n+----------------------------------------------------------\r\nAn element with Key = \"txt\" already exists.";
        private readonly string cExpShowIndex1 = 
            "+----------------------------------------------------------\r\n| Show Index (get value)\r\n+----------------------------------------------------------\r\nFor key = \"rtf\", value = wordpad.exe.";
        private readonly string cExpShowIndex2 =
            "+----------------------------------------------------------\r\n| Show Index (change value)\r\n+----------------------------------------------------------\r\nFor key = \"rtf\", value = winword.exe.\r\n\r\nKey = txt, Value = notepad.exe\r\nKey = bmp, Value = paint.exe\r\nKey = dib, Value = paint.exe\r\nKey = rtf, Value = winword.exe\r\nCount: 4";
        private readonly string cExpShowIndex3 =
            "+----------------------------------------------------------\r\n| Show Index (add value)\r\n+----------------------------------------------------------\r\n\r\nKey = txt, Value = notepad.exe\r\nKey = bmp, Value = paint.exe\r\nKey = dib, Value = paint.exe\r\nKey = rtf, Value = winword.exe\r\nKey = doc, Value = winword.exe\r\nCount: 5";
        private readonly string cExpShowIndex4 = 
            "+----------------------------------------------------------\r\n| Show Index (get exception)\r\n+----------------------------------------------------------\r\nKey = \"tif\" is not found.";
        private readonly string cExpShowTryGetValue = 
            "+----------------------------------------------------------\r\n| Show TryGetValue\r\n+----------------------------------------------------------\r\nKey = \"tif\" is not found.";
        private readonly string cExpShowContainsKey = 
            "+----------------------------------------------------------\r\n| Show ContainsKey\r\n+----------------------------------------------------------\r\nValue added for key = \"ht\": hypertrm.exe";
        private readonly string cExpShowValueCollection =
            "+----------------------------------------------------------\r\n| Show ValueCollection\r\n+----------------------------------------------------------\r\n\r\nValue = notepad.exe\r\nValue = paint.exe\r\nValue = paint.exe\r\nValue = winword.exe\r\nValue = winword.exe\r\nValue = hypertrm.exe";
        private readonly string cExpShowKeyCollection =
            "+----------------------------------------------------------\r\n| Show KeyCollection\r\n+----------------------------------------------------------\r\n\r\nKey = txt\r\nKey = bmp\r\nKey = dib\r\nKey = rtf\r\nKey = doc\r\nKey = ht";
        private readonly string cExpShowRemove = 
            "+----------------------------------------------------------\r\n| Show Remove\r\n+----------------------------------------------------------\r\n\nRemove(\"doc\")\r\nKey \"doc\" is not found.\r\n\r\nKey = txt, Value = notepad.exe\r\nKey = bmp, Value = paint.exe\r\nKey = dib, Value = paint.exe\r\nKey = rtf, Value = winword.exe\r\nKey = ht, Value = hypertrm.exe\r\nCount: 5";


        /// <summary>
        /// Defines the test method DictionaryExampleMainTest.
        /// </summary>
        [TestMethod()]
        public void DictionaryExampleMainTest()
        {
            AssertConsoleOutput(cExpDictionaryExampleMain, DictionaryExample.DictionaryExampleMain);
        }

        /// <summary>
        /// Defines the test method TryAddExistingTest.
        /// </summary>
        [TestMethod()]
        public void TryAddExistingTest()
        {
            AssertConsoleOutput(cExpTryAddExisting, DictionaryExample.TryAddExisting);
        }

        /// <summary>
        /// Defines the test method ShowIndex1Test.
        /// </summary>
        [TestMethod()]
        public void ShowIndex1Test()
        {
            AssertConsoleOutput(cExpShowIndex1, DictionaryExample.ShowIndex1);
        }

        /// <summary>
        /// Defines the test method ShowIndex2Test.
        /// </summary>
        [TestMethod()]
        public void ShowIndex2Test()
        {
            AssertConsoleOutput(cExpShowIndex2, DictionaryExample.ShowIndex2);
        }

        /// <summary>
        /// Defines the test method ShowIndex3Test.
        /// </summary>
        [TestMethod()]
        public void ShowIndex3Test()
        {
            AssertConsoleOutput(cExpShowIndex3, DictionaryExample.AddValueWithDiffKeys);
        }

        /// <summary>
        /// Defines the test method ShowIndex4Test.
        /// </summary>
        [TestMethod()]
        public void ShowIndex4Test()
        {
            AssertConsoleOutput(cExpShowIndex4, DictionaryExample.ShowIndex4);
        }

        /// <summary>
        /// Defines the test method ShowTryGetValueTest.
        /// </summary>
        [TestMethod()]
        public void ShowTryGetValueTest()
        {
            AssertConsoleOutput(cExpShowTryGetValue, DictionaryExample.ShowTryGetValue);
        }

        /// <summary>
        /// Defines the test method ShowContainsKeyTest.
        /// </summary>
        [TestMethod()]
        public void ShowContainsKeyTest()
        {
            AssertConsoleOutput(cExpShowContainsKey, DictionaryExample.ShowContainsKey);
        }

        /// <summary>
        /// Defines the test method ShowValueCollectionTest.
        /// </summary>
        [TestMethod()]
        public void ShowValueCollectionTest()
        {
            AssertConsoleOutput(cExpShowValueCollection, DictionaryExample.ShowValueCollection);
        }

        /// <summary>
        /// Defines the test method ShowKeyCollectionTest.
        /// </summary>
        [TestMethod()]
        public void ShowKeyCollectionTest()
        {
            AssertConsoleOutput(cExpShowKeyCollection, DictionaryExample.ShowKeyCollection);
        }

        /// <summary>
        /// Defines the test method ShowRemoveTest.
        /// </summary>
        [TestMethod()]
        public void ShowRemoveTest()
        {
            AssertConsoleOutput(cExpShowRemove, DictionaryExample.ShowRemove);
        }
    }
}
