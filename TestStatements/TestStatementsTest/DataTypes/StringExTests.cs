using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestStatements.UnitTesting;

namespace TestStatements.DataTypes.Tests
{
    [TestClass]
    public class StringExTests:ConsoleTestsBase
    {
        [TestMethod]
        public void StringEx1Test()
        {
            AssertConsoleOutput(@"This is a string created by assignment.
The path is C:\PublicDocuments\Report1.doc
The path is C:\PublicDocuments\Report1.doc
The path is C:\PublicDocuments\Report1.doc", StringEx.StringEx1);
        }

        [TestMethod]
        public void StringEx2Test()
        {
            AssertConsoleOutput(@"word
cccccccccccccccccccc
ABCDE
word", StringEx.StringEx2);
        }

        [TestMethod]
        public void StringEx3Test()
        {
            StringEx.GetNow = () => DateTime.Parse("2023-01-01");
            AssertConsoleOutput(@"Today is Sonntag, 1. Januar 2023.
This is one sentence. This is a second. This is a third sentence.", StringEx.StringEx3);
        }
        [TestMethod]
        public void StringEx4Test()
        {
            AssertConsoleOutput(@"Second word: sentence", StringEx.StringEx4);
        }
        [TestMethod]
        public void StringEx5Test()
        {
            AssertConsoleOutput(@"At 07:32 on Mittwoch, 6. Juli 2011, the temperature was 68,3 degrees Fahrenheit.", StringEx.StringEx5);
        }
        [TestMethod]
        public void UnicodeEx1Test()
        {
            AssertConsoleOutput(@"", StringEx.UnicodeEx1);
        }

        [TestMethod]
        public void StringSurogarteEx1Test()
        {
            AssertConsoleOutput(@"U+D800 U+DC03 
   Is Surrogate Pair: True", StringEx.StringSurogarteEx1);
        }

        [TestMethod]
        public void AllTestsTest()
        {
            StringEx.GetNow = () => DateTime.Parse("2023-04-01");
            AssertConsoleOutput(@"This is a string created by assignment.
The path is C:\PublicDocuments\Report1.doc
The path is C:\PublicDocuments\Report1.doc
The path is C:\PublicDocuments\Report1.doc
word
cccccccccccccccccccc
ABCDE
word
Today is Samstag, 1. April 2023.
This is one sentence. This is a second. This is a third sentence.
Second word: sentence
At 07:32 on Mittwoch, 6. Juli 2011, the temperature was 68,3 degrees Fahrenheit.
U+D800 U+DC03 
   Is Surrogate Pair: True", StringEx.AllTests);
        }

    }
}
