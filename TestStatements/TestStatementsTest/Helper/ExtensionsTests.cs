using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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

        [DataTestMethod]
        [DataRow("0", new object[] { (sbyte)0x01, (sbyte)0x02, (sbyte)0x03, (sbyte)0x04 }, new string[] { "1", "2", "3", "4" })]
        [DataRow("1", new object[] { "Hallo", null, 1d, 2f, 4, 5u }, new object[] { "Hallo", "", "1", "2", "4", "5" })]
        [DataRow("2",new object[] {"Hallo",null,1d,2f,4,5u }, new object[] { "Hallo", "", "1", "2", "4", "5" })]
        public void ConvertTest(string name, dynamic[] oVal, dynamic[] oExp) {
            var oR = oVal.Convert((v)=>v?.ToString()??"").ToArray();
            for (var i = 0; i < oExp.Length; i++)
                Assert.AreEqual(oExp[i], oR[i]);
        }

    }
}
