using GenFree.Helper;
using GenFree.Interfaces.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Gen_FreeWin.Tests
{
    [TestClass()]
    public class DateHelperTests
    {
        [DataTestMethod()]
        [DataRow("", "", new[] { "" }, new[] { "", "", "" })]
        [DataRow("32", "", new[] { "" }, new[] { "", "32", "" })]
        [DataRow("", "32", new[] { "" }, new[] { "", "", "32" })]
        [DataRow("31", "", new[] { "A", "B", "C", "D", "E", "F" }, new[] { "", "31", "" })]
        [DataRow("", "31", new[] { "A", "B", "C", "D", "E", "F" }, new[] { "", "", "31" })]
        [DataRow("", "31.1", new[] { "A", "B", "C", "D", "E", "F" }, new[] { "B C 2024A", "10.07.2023", "31.1" })]
        [DataRow("30.1", "31.1", new[] { "A", "B", "C", "D", "E", "F" }, new[] { "B C 1A", "", "31.1" })]
        public void RechTest(string D1, string D2, string[] IT, string[] asExp)
        {
            IApplUserTexts asIT = Substitute.For<IApplUserTexts>();
            asIT[216] = IT[0];
            for (int i = 1; i < IT.Length; i++)
                asIT[116 + i] = IT[i];

            Assert.AreEqual(asExp[0], DateHelper2.CalcAge(D1, D2, asIT));
            Assert.AreEqual(asExp[1], D1);
            Assert.AreEqual(asExp[2], D2);
        }
    }
}