using GenFree.Helper;
using GenFree.Interfaces.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GenFreeWin.Tests
{
    [TestClass]
    public class DateHelperTests
    {
        [TestMethod]
        [DataRow("", "", new[] { "" }, "")]
        [DataRow("32", "", new[] { "" }, "")]
        [DataRow("", "32", new[] { "" }, "")]
        [DataRow("31", "", new[] { "A", "B", "C", "D", "E", "F" }, "")]
        [DataRow("", "31", new[] { "A", "B", "C", "D", "E", "F" }, "")]
        [DataRow("10.07.2023", "31.01.2024", new[] { "A", "B", "C", "D", "E", "F" }, "B0C6D21A")]
        [DataRow("30.01.2024", "31.01.2024", new[] { "A", "B", "C", "D", "E", "F" }, "B0C0D1A")]
        public void RechTest(string D1, string D2, string[] IT, string expected)
        {
            IApplUserTexts asIT = Substitute.For<IApplUserTexts>();
            asIT[EUserText.t216].Returns(IT[0]);
            for (int i = 1; i < IT.Length; i++)
                asIT[(EUserText)(116 + i)].Returns(IT[i]);

            Assert.AreEqual(expected, DateHelper2.CalcAge(D1, D2, asIT) ?? "");
        }
    }
}