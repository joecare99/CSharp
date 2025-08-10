﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Interfaces.DB;
using NSubstitute;
using static BaseLib.Helper.TestHelper;
using BaseLib.Helper;
using BaseLib.Interfaces;

namespace GenFree.Helper.Tests
{
    [TestClass()]
    public class StringHelperTests
    {
        [TestMethod()]
        [DataRow("", "", new string[] { "(", ")" })]
        [DataRow("(a)", "a", new string[] { "(", ")" })]
        [DataRow("'b'", "b", new string[] { "'" })]
        [DataRow("c", "c", new string[] { })]
        [DataRow(" ", " ", new string[] { "(", ")" })]
        public void FrameIfNEoWTest(string sExp, string sAct, string[] sFrame)
        {
            //Test for FrameIfNEoW
            Assert.AreEqual(sExp, sAct.FrameIfNEoW(sFrame), $"FrameIfNEoW({sAct},{string.Join(", ", sFrame)})Test failed");
        }

        [TestMethod()]
        [DataRow("", "")]
        [DataRow("a", "a")]
        [DataRow("bb", "bb")]
        [DataRow("ccc", "ccc")]
        [DataRow("A", "Ä")]
        [DataRow("O", "Ö")]
        [DataRow("U", "Ü")]
        [DataRow("a", "á")]
        [DataRow("a", "â")]
        [DataRow("a", "à")]
        [DataRow("ss", "ß")]
        public void Uml2SuchTest(string sExp, string sAct)
        {
            Assert.AreEqual(sExp, sAct.Uml2Such(), $"Uml2Such({sAct})Test failed");
        }


        [TestMethod()]
        [DataRow("", null)]
        [DataRow("a", "a")]
        [DataRow("b", 'b')]
        [DataRow("3", 3)]
        [DataRow("4", 4.0)]
        [DataRow("5", 5.0f)]
        [DataRow("6", 6L)]
        [DataRow("7", 7U)]
        [DataRow("8", 8UL)]
        [DataRow("9", 9)]
        public void AsStringTest(string sExp, object sAct)
        {
            Assert.AreEqual(sExp, sAct.AsString(), $"AsString({sAct})");
        }

        [TestMethod()]
        [DataRow("", null)]
        [DataRow("a", "a")]
        [DataRow("3", 3)]
        public void AsStringTest2(string sExp, object sAct0)
        {
            var sAct = Substitute.For<IField>();
            (sAct as IHasValue).Value.Returns(sAct0);
            Assert.AreEqual(sExp, sAct.AsString(), $"AsString({sAct})");
        }

        [System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
        private class CTest
        {
            public override string? ToString() => null;
            private string GetDebuggerDisplay() => "CTest: Null";
        }
        [TestMethod()]
        public void AsStringTest3()
        {
            var sAct = new CTest();
            Assert.AreEqual("", sAct.AsString(), $"AsString({sAct})");
        }

        [TestMethod()]
        [DataRow("00", new string[] { }, new string[] { }, 0, new string[] { })]
        [DataRow("01", new string[] { "1" }, new string[] { }, 0, new string[] { })]
        [DataRow("0-1", new string[] { }, new string[] { "0" }, 0, new string[] { "0" })]
        [DataRow("0-2", new string[] { "1" }, new string[] { "0" }, 1, new string[] { "0" })]
        [DataRow("0-3", new string[] { "1" }, new string[] { "0" }, -1, new string[] { "0" })]
        [DataRow("1-0", new string[] { "1" }, null, 0, new string[] { "1" })]
        [DataRow("1-1", new string[] { "1" }, null, 1, new string[] { "", "1", "" })]
        [DataRow("1-2", new string[] { "1" }, null, -1, new string[] { })]
        [DataRow("2-2-0", new string[] { "1", "2" }, new string[] { "A", "B" }, 0, new string[] { "1", "2" })]
        [DataRow("2-2-1", new string[] { "1", "2" }, new string[] { "A", "B" }, 1, new string[] { "A", "1" })]
        [DataRow("2-2-2", new string[] { "1", "2" }, new string[] { "A", "B" }, -1, new string[] { "2", "B" })]
        public void IntoStringTest(string sName, string[] asAct1, string[]? asAct2, int iAct, string[] asExp)
        {
            AssertAreEqual(asExp, asAct1.IntoString(asAct2, iAct), sName);
        }

        [TestMethod()]
        [DataRow("a", 0, "")]
        [DataRow("a", 1, "a")]
        [DataRow("ab", 1, "b")]
        [DataRow("ab", 2, "ab")]
        [DataRow("abc", 2, "bc")]
        [DataRow("abc", 5, "abc")]
        [DataRow("ab", -1, "b")]
        [DataRow("ab", -2, "")]
        [DataRow("abc", -2, "c")]
        [DataRow("abc", -5, "")]
        public void RightTest(string sAct, int iAct, string sExp)
        {
            Assert.AreEqual(sExp, sAct.Right(iAct));
        }

        [TestMethod()]
        [DataRow("a", 0, "")]
        [DataRow("a", 1, "a")]
        [DataRow("ab", 1, "a")]
        [DataRow("ab", 2, "ab")]
        [DataRow("abc", 2, "ab")]
        [DataRow("abc", 5, "abc")]
        [DataRow("ab", -1, "a")]
        [DataRow("ab", -2, "")]
        [DataRow("abc", -2, "a")]
        [DataRow("abc", -5, "")]
        public void LeftTest(string sAct, int iAct, string sExp)
        {
            Assert.AreEqual(sExp, sAct.Left(iAct));
        }

        [TestMethod()]
        public void ToStringsTest()
        {
            AssertAreEqual(new[] { "'0'", "'1'", "'2'" }, new[] { "0", "1", "2" }.ToStrings((s) => $"'{s}'"));
        }

        [TestMethod()]
        [DataRow("a", "a")]
        [DataRow("A", "A")]
        [DataRow("\xCF", "ß")]
        [DataRow("\xC5", "¿")]
        [DataRow("\xE8u", "ü")]
        [DataRow("\xE2o", "ó")]
        [DataRow("\x00E3a", "â")]
        public void ANSELDecodeTest(string sAct, string sExp)
        {
            Assert.AreEqual(sExp, sAct.ANSELDecode());
        }

        [TestMethod()]
        [DataRow("a", "a")]
        [DataRow("A", "A")]
        [DataRow("\xCF", "ß")]
        [DataRow("\x81", "ü")]
        [DataRow("\x82", "é")]
        public void IBM_DOSDecodeTest(string sAct, string sExp)
        {
            Assert.AreEqual(sExp, sAct.IBM_DOSDecode());
        }

        [TestMethod()]
        [DataRow("a", "a")]
        [DataRow("A", "A")]
        [DataRow("\xC2\xB0", "°")]
        [DataRow("\xC3\xA0", "à")]
        [DataRow("\xC4\x99", "ӧ")]
        [DataRow("\xC5\xE5", "†")]
        [DataRow("\xC8\x98", "Ș")]
        [DataRow("\xE2\x80\xA0", "†")]
        public void UTF8DecodeTest(string sAct, string sExp)
        {
            Assert.AreEqual(sExp, sAct.UTF8Decode());
        }

    }
}