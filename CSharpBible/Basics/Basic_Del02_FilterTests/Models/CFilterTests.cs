using BaseLib.Model.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BaseLib.Helper.TestHelper;

namespace Basic_Del02_Filter.Models.Tests
{
    [TestClass]
    public class CFilterTests : BaseTest
    {
        [TestMethod]
        [DataRow(new[] { 1 }, new[] { 1 },DisplayName ="1")]
        [DataRow(new[] { 2}, new int[]{ }, DisplayName = "2")]
        [DataRow(new[] { 2, 5, -1, 11, 0, 18, 22, 67, 51, 6 }, new[] { 5, -1, 11, 67, 51 })]
        public void FilterOddTest(int[] data, int[] expected)
        {
            int[] actual = data.Filter((i) => i % 2 != 0);
            AssertAreEqual(expected,actual);
        }
    }
}
