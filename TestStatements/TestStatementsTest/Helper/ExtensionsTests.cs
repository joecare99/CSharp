using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestStatements.Helper.Tests
{
    [TestClass]
    public class ExtensionsTests
    {
        [DataTestMethod]
        [DataRow(null,0)]
        [DataRow("", 0)]
        [DataRow("0", 0)]
        [DataRow("0.0", 0)]
        [DataRow("1.2", 0)]
        [DataRow("2,3", 0)]
        public void AsIntTest(string? sVal,int iExp)
        {
            Assert.AreEqual(iExp, sVal.AsInt());
        }

        [DataTestMethod]
        [DataRow(null, float.NaN)]
        [DataRow("", float.NaN)]
        [DataRow("0", 0f)]
        [DataRow("1.2", 1.2f)]
        [DataRow("2,3", 2.3f)]
        public void AsFloatTest(string? sVal, float fExp)
        {
            Assert.AreEqual(fExp, sVal.AsFloat());
        }

        [DataTestMethod]
        [DataRow(null, double.NaN)]
        [DataRow("", double.NaN)]
        [DataRow("0", 0d)]
        [DataRow("1.2", 1.2d)]
        [DataRow("2,3", 2.3d)]
        public void AsDoubleTest(string? sVal, double dExp)
        {
            Assert.AreEqual(dExp, sVal.AsDouble());
        }

    }
}
