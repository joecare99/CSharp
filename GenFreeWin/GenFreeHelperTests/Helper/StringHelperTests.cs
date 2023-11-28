using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Interfaces.DB;
using NSubstitute;
using static BaseLib.Helper.TestHelper;

namespace GenFree.Helper.Tests
{
    [TestClass()]
    public class StringHelperTests
    {
        [DataTestMethod()]
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

        [DataTestMethod()]
        [DataRow("", "")]
        [DataRow("a", "a")]
        [DataRow("bb", "bb")]
        [DataRow("ccc", "ccc")]
        [DataRow("A", "Ä")]
        [DataRow("O", "Ö")]
        [DataRow("U", "Ü")]
        [DataRow("Á", "á")]
        [DataRow("Â", "â")]
        [DataRow("À", "à")]
        [DataRow("ss", "ß")]
        public void Uml2SuchTest(string sExp, string sAct)
        {
            Assert.AreEqual(sExp, sAct.Uml2Such(), $"Uml2Such({sAct})Test failed");
        }

        [DataTestMethod()]
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

        [DataTestMethod()]
        [DataRow("", null)]
        [DataRow("a", "a")]
        [DataRow("3", 3)]
        public void AsStringTest2(string sExp, object sAct0)
        {
            var sAct = Substitute.For<IField>();
            sAct.Value.Returns(sAct0);
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

        [DataTestMethod()]
        [DataRow("00", new string[] { }, new string[] { }, 0, new string[] { })]
        [DataRow("01", new string[] { "1" }, new string[] { }, 0, new string[] { })]
        [DataRow("0-1", new string[] { }, new string[] { "0" }, 0, new string[] { "0" })]
        [DataRow("0-2", new string[] { "1" }, new string[] { "0" }, 1, new string[] { "0" })]
        [DataRow("0-3", new string[] { "1" }, new string[] { "0" }, -1, new string[] { "0" })]
        [DataRow("1-0", new string[] { "1" }, null, 0, new string[] { "1" })]
        [DataRow("1-1", new string[] { "1" }, null, 1, new string[] { "", "1" })]
        [DataRow("1-2", new string[] { "1" }, null, -1, new string[] { })]
        [DataRow("2-2-0", new string[] { "1", "2" }, new string[] { "A", "B" }, 0, new string[] { "1", "2" })]
        [DataRow("2-2-1", new string[] { "1", "2" }, new string[] { "A", "B" }, 1, new string[] { "A", "1" })]
        [DataRow("2-2-2", new string[] { "1", "2" }, new string[] { "A", "B" }, -1, new string[] { "2", "B" })]
        public void IntoStringTest(string sName, string[] asAct1, string[]? asAct2, int iAct, string[] asExp)
        {
            AssertAreEqual(asExp, asAct1.IntoString(asAct2, iAct), sName);
        }

        [DataTestMethod()]
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

        [DataTestMethod()]
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
            AssertAreEqual(new[] { "'0'", "'1'", "'2'" }, new[]{"0","1","2" }.ToStrings((s)=>$"'{s}'"));
        }
    }
}