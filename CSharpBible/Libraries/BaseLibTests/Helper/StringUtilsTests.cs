using BaseLib.Helper;
// ***********************************************************************
// Assembly         : BaseLibTests
// Author           : Mir
// Created          : 03-27-2023
//
// Last Modified By : Mir
// Last Modified On : 03-27-2023
// ***********************************************************************
// <copyright file="StringUtilsTests.cs" company="JC-Soft">
//     Copyright � JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using static BaseLib.Helper.TestHelper;
/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace BaseLib.Helper.Tests
{
    /// <summary>
    /// Defines test class StringUtilsTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class StringUtilsTests
    {
        /// <summary>
        /// Quotes the test.
        /// </summary>
        /// <param name="sExp">The s exp.</param>
        /// <param name="sWork">The s work.</param>
        /// <param name="sMsg">The s MSG.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("", "", "Empty")]
        [DataRow("", null, "Empty2")]
        [DataRow(@"\n", "\n", "Newline")]
        [DataRow(@"\t", "\t", "Tab")]
        [DataRow(@"\r", "\r", "Linefeed")]
        [DataRow(@"\\", @"\", "Backslash")]
        [DataRow(@"\\\\", @"\\", "Double-backslash")]
        [DataRow(@"\\n", @"\n", "Backslash-n")]
        public void QuoteTest(string sExp, string? sWork, string sMsg)
        {
            //			Assert.AreEqual(null, LogData.Quote(null), "null");
            Assert.AreEqual(sExp, StringUtils.Quote(sWork), sMsg);
        }

        /// <summary>
        /// Uns the quote test.
        /// </summary>
        /// <param name="sExp">The s exp.</param>
        /// <param name="sWork">The s work.</param>
        /// <param name="sMsg">The s MSG.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("", "", "Empty")]
        [DataRow("", null, "Empty2")]
        [DataRow("\n", @"\n", "Newline")]
        [DataRow("\t", @"\t", "Tab")]
        [DataRow("\r", @"\r", "Linefeed")]
        [DataRow(@"\", @"\", "Backslash")]
        [DataRow(@"\", @"\\", "Double-Backslash")]
        [DataRow(@"\n", @"\\n", "Backslash-n")]
        public void UnQuoteTest(string sExp, string? sWork, string sMsg)
        {
            //			Assert.AreEqual(null, LogData.UnQuote(null), "null");
            Assert.AreEqual(sExp, StringUtils.UnQuote(sWork), sMsg);
        }

        /// <summary>
        /// Quotes the unq test.
        /// </summary>
        /// <param name="sWork">The s work.</param>
        /// <param name="sMsg">The s MSG.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("", "Empty")]
        [DataRow("\n", "Newline")]
        [DataRow("\t", "Tab")]
        [DataRow("\r", "Linefeed")]
        [DataRow(@"\", "Backslash")]
        [DataRow(@"\\", "Double-backslash")]
        [DataRow(@"\n", "Newline2")]
        public void QuoteUnqTest(string sWork, string sMsg)
        {
            Assert.AreEqual(sWork, StringUtils.UnQuote(StringUtils.Quote(sWork)), sMsg);
        }
        /// <summary>
        /// Defines the test method UnQuoteQuoteTest.
        /// </summary>
        /// <param name="sExp">The s exp.</param>
        /// <param name="sWork">The s work.</param>
        /// <param name="sMsg">The s MSG.</param>
        /// <autogeneratedoc />
        [TestMethod()]
        [DataRow("", "", "Empty")]
        [DataRow(@"\n", @"\n", "Newline")]
        [DataRow(@"\t", @"\t", "Tab")]
        [DataRow(@"\r", @"\r", "Linefeed")]
        [DataRow(@"\\", @"\", "Backslash")] //!!
        [DataRow(@"\\", @"\\", "Double-backslash")]
        [DataRow(@"\\n", @"\\n", "Newline2")]
        public void UnQuoteQuoteTest(string sExp, string sWork, string sMsg)
        {
            Assert.AreEqual(sExp, StringUtils.Quote(StringUtils.UnQuote(sWork)), sMsg);
        }

        [DataTestMethod()]
        [DataRow("", "", "Empty")]
        [DataRow("This is a Newline", "This is a {0}", "Newline")]
        [DataRow("This is a HexNumber 008CEA1F", "This is a HexNumber {0:X8}", 9234975)]
        [DataRow("This is a Number 07346238", "This is a Number {0:D8}", 7346238)]
        [DataRow("This is a Number 1,414", "This is a Number {0}", 1.414)] //!!
        [DataRow("This is a Number 3,14", "This is a Number {0:F2}", 3.141525345)]
        [DataRow("A () guess !", "A ({0}) guess !", null)]
        public void FormatTest(string sExp, string sWork, object sMsg)
        {
            Assert.AreEqual(sExp, StringUtils.Format(sWork, sMsg));
        }

        [DataTestMethod()]
        [DataRow("", "", "Empty")]
        [DataRow("This is a Newline", "This is a {0}", "Newline")]
        [DataRow("This is a HexNumber 008CEA1F", "This is a HexNumber {0:X8}", 9234975)]
        [DataRow("This is a Number 07346238", "This is a Number {0:D8}", 7346238)]
        [DataRow("This is a Number 1,414", "This is a Number {0}", 1.414)] //!!
        [DataRow("This is a Number 3,14", "This is a Number {0:F2}", 3.141525345)]
        [DataRow("A () guess !", "A ({0}) guess !", null)]
        public void FormatTest1(string sExp, string sWork, object sMsg)
        {
            Assert.AreEqual(sExp, sWork.Format(sMsg));
        }

        /// <summary>
        /// ses the first test.
        /// </summary>
        /// <param name="sArg">The s argument.</param>
        /// <param name="sSep">The s sep.</param>
        /// <param name="sExp">The s exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("", null, "")]
        [DataRow("1", null, "1")]
        [DataRow("1 2", null, "1")]
        [DataRow(" 2", null, "")]
        [DataRow("1", ";", "1")]
        [DataRow("1 2", ";", "1 2")]
        [DataRow("1;2", ";", "1")]
        [DataRow(";2", ";", "")]
        public void SFirstTest(string sArg, string sSep, string sExp)
        {
            if (sSep == null)
                Assert.AreEqual(sExp, sArg.SFirst());
            else
                Assert.AreEqual(sExp, sArg.SFirst(sSep));
        }

        /// <summary>
        /// ses the rest test.
        /// </summary>
        /// <param name="sArg">The s argument.</param>
        /// <param name="sSep">The s sep.</param>
        /// <param name="sExp">The s exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("", null, "")]
        [DataRow("1", null, "")]
        [DataRow("1 2", null, "2")]
        [DataRow(" 2", null, "2")]
        [DataRow("1", ";", "")]
        [DataRow("1 2", ";", "")]
        [DataRow("1;2", ";", "2")]
        [DataRow(";2", ";", "2")]
        public void SRestTest(string sArg, string sSep, string sExp)
        {
            if (sSep == null)
                Assert.AreEqual(sExp, sArg.SRest());
            else
                Assert.AreEqual(sExp, sArg.SRest(sSep));
        }

        /// <summary>
        /// Pads the tab test.
        /// </summary>
        /// <param name="sArg">The s argument.</param>
        /// <param name="iOffs">The i offs.</param>
        /// <param name="sExp">The s exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("", 0, "")]
        [DataRow("s", 0, "s")]
        [DataRow(" s", 0, " s")]
        [DataRow("s s", 0, "s s")]
        [DataRow("1\t2", 0, "1       2")]
        [DataRow("12\tX", 0, "12      X")]
        [DataRow("123\tX", 0, "123     X")]
        [DataRow("1234\tX", 0, "1234    X")]
        [DataRow("12345\tX", 0, "12345   X")]
        [DataRow("123456\tX", 0, "123456  X")]
        [DataRow("1234567\tX", 0, "1234567 X")]
        [DataRow("12345678\tX", 0, "12345678        X")]
        [DataRow("1\t2", 4, "1   2")]
        [DataRow("12\tX", 4, "12  X")]
        [DataRow("123\tX", 4, "123 X")]
        [DataRow("1234\tX", 4, "1234        X")]
        [DataRow("12345\tX", 4, "12345       X")]
        [DataRow("123456\tX", 4, "123456      X")]
        [DataRow("1234567\tX", 4, "1234567     X")]
        [DataRow("12345678\tX", 4, "12345678    X")]
        [DataRow("s s", null, "s s")]
        [DataRow("1\t2", null, "1       2")]
        public void PadTabTest(string sArg, int? iOffs, string sExp)
        {
            if (iOffs == null)
                Assert.AreEqual(sExp, sArg.PadTab());
            else
                Assert.AreEqual(sExp, sArg.PadTab(iOffs ?? 0));
        }

        [DataTestMethod()]
        [DataRow("", "", "Empty")]
        [DataRow("Affe", "Affe", "Word")]
        [DataRow("Werden", "werden", "Word2")]
        [DataRow("As f qw dpqw ", "as f qw Dpqw ", "Satz")]
        [DataRow("Aaaaaaa", "aAaAaAa", "Switch-Case")]
        [DataRow("Bbbbbbbbbb", "BbBbBbBbBb", "Switch-Case2")] //!!
        [DataRow("�������", "�������", "Umlauts")]
        [DataRow("Qwep as03�3 ", "qwep as03�3 ", "Wild")]
        public void ToNormalTest1(string sExp, string sWork, string sMsg)
        {
            Assert.AreEqual(sExp, StringUtils.ToNormal(sWork), sMsg);
        }

        [DataTestMethod()]
        [DataRow("Empty", new string[] { "" }, "")]
        [DataRow("1,2,3", new string[] { "1", "2", "3" }, "1,2,3")]
        [DataRow("1, 2, 3 ", new string[] { "1", "2", "3" }, "1, 2, 3 ")]
        [DataRow(" 1 , 2 , 3 ", new string[] { "1", "2", "3" }, " 1 , 2 , 3 ")]
        [DataRow("1,", new string[] { "1", "" }, "1,")]
        [DataRow("1,Test", new string[] { "1", "Test" }, "1,Test")]
        [DataRow("1,\"Test\"", new string[] { "1", "Test" }, "1,\"Test\"")]
        [DataRow("1,\"2,3\"", new string[] { "1", "2,3" }, "1,\"2,3\"")]
        [DataRow("1,\"2", new string[] { "1", "2" }, "1,\"2")]
        [DataRow("1,\"2,3", new string[] { "1", "2,3" }, "1,\"2,3")]
        [DataRow("1,2,3\"", new string[] { "1", "2", "3\"" }, "1,2,3\"")]
        [DataRow("\"1,2\" ,3", new string[] { "1,2", "3" }, "\"1,2\" ,3")]
        [DataRow("\"1, 2\" ,3", new string[] { "1, 2", "3" }, "\"1, 2\" ,3")]
        public void QuotedSplitTest(string Msg, string[] arExpStr, string testStr, params string[] options)
        {
            var lExp = new List<string>(arExpStr);
            AssertAreEqual(lExp, StringUtils.QuotedSplit(testStr), Msg);
        }

        [DataTestMethod()]
        [DataRow("Empty", new string[] { "" }, "")]
        [DataRow("1,2,3", new string[] { "1", "2", "3" }, "1,2,3")]
        [DataRow("1, 2, 3 ", new string[] { "1", "2", "3" }, "1, 2, 3 ")]
        [DataRow(" 1 , 2 , 3 ", new string[] { "1", "2", "3" }, " 1 , 2 , 3 ")]
        [DataRow("1,", new string[] { "1", "" }, "1,")]
        [DataRow("1,Test", new string[] { "1", "Test" }, "1,Test")]
        [DataRow("1,\"Test\"", new string[] { "1", "Test" }, "1,\"Test\"")]
        [DataRow("1,\"2,3\"", new string[] { "1", "2,3" }, "1,\"2,3\"")]
        [DataRow("1,\"2", new string[] { "1", "2" }, "1,\"2")]
        [DataRow("1,\"2,3", new string[] { "1", "2,3" }, "1,\"2,3")]
        [DataRow("1,2,3\"", new string[] { "1", "2", "3\"" }, "1,2,3\"")]
        [DataRow("\"1,2\" ,3", new string[] { "1,2", "3" }, "\"1,2\" ,3")]
        [DataRow("\"1, 2\" ,3", new string[] { "1, 2", "3" }, "\"1, 2\" ,3")]
        public void QuotedSplit2Test(string Msg, string[] arExpStr, string testStr, params string[] options)
        {
            var lExp = new List<string>(arExpStr);
            AssertAreEqual(lExp, testStr.QuotedSplit(), Msg);
        }

        [DataTestMethod()]
        [DataRow("Empty", new string[] { "" }, false)]
        [DataRow("Empty2", new string[] { "a", "tx2", "ty2" }, false)]
        [DataRow("Emp ty2", new string[] { "a", "tx2", "ty2" }, true)]
        [DataRow("NIO1", new string[] { "a", "tx2", "ty2" }, false)]
        public void EndswithAnyTest(string sAct, string[] sAct2, bool xExp)
        {
            Assert.AreEqual(xExp, sAct.EndswithAny(sAct2));
        }

        [DataTestMethod()]
        [DataRow("Empty", new string[] { "" }, false)]
        [DataRow("Empty2", new string[] { "E", "Em", "Emp", "ty2" }, false)]
        [DataRow("Emp ty2", new string[] { "a", "Emp", "ty2" }, true)]
        [DataRow("NIO1", new string[] { "a", "tx2", "ty2" }, false)]
        public void StartswithAnyTest(string sAct, string[] sAct2, bool xExp)
        {
            Assert.AreEqual(xExp, sAct.StartswithAny(sAct2));
        }
        //private void AssertAreEqual(List<string> lExp, List<string> list, string Msg = "")
        //{
        //    Assert.AreEqual(lExp?.GetType(), list?.GetType(), $"{Msg}.Type");
        //    for (var i = 0; i < Math.Min(lExp?.Count ?? 0, list?.Count ?? 0); i++)
        //        Assert.AreEqual(lExp![i], list![i], $"{Msg}.Item[{i}]");
        //    Assert.AreEqual(lExp?.Count, list?.Count, $"{Msg}.Count");
        //}

        [DataTestMethod()]
        [DataRow("Empty", new string[] { "" }, true)]
        [DataRow("Empty2", new string[] { "E", "Em", "Emp", "ty2" }, true)]
        [DataRow("Emp ty2", new string[] { "a", "Emp", "ty2" }, true)]
        [DataRow("NIO1", new string[] { "a", "tx2", "ty2" }, false)]

        public void ContainsAnyTest(string sAct, string[] sAct2, bool xExp)
        {
            Assert.AreEqual(xExp, sAct.ContainsAny(sAct2));
        }

        [DataTestMethod()]
        [DataRow(null,  false)]
        [DataRow("",  false)]
        [DataRow("_",  false)]
        [DataRow("Empty",  true)]
        [DataRow("Empty_34",  true)]
        [DataRow("2mpty2",  false)]
        [DataRow("Emp ty2",  false)]
        [DataRow("NIO1 ",  false)]
        public void IsValidIdentifyerTest(string sAct, bool xExp)
        {
            Assert.AreEqual(xExp,sAct.IsValidIdentifyer());
        }
    }
}
