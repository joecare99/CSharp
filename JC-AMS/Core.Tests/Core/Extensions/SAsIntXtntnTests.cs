using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAMS.Core.Extensions;
using System;
using System.Collections.Generic;

namespace JCAMS.Core.Extensions.Tests
{
    [TestClass()]
    public class SAsIntXtntnTests
    {
        protected static IEnumerable<object?[]> AsDBNullData => new[]
        {
            new object?[]{"DBNull",DBNull.Value,0 }
        };    

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("25", 25, 25)]
        [DataRow("25L", 25L, 25)]
        [DataRow("long.MaxValue", long.MaxValue, 0)]
        [DataRow("long.MinValue", long.MinValue, -0)]
        [DataRow("int.MaxValue", int.MaxValue, 2147483647)]
        [DataRow("int.MinValue", int.MinValue, -2147483648)]
        [DataRow("-1", -1, -1)]
        [DataRow("Math.PI", Math.PI, 3)]
        [DataRow("NaN", double.NaN, 0)]
        [DataRow("PositiveInfinity", double.PositiveInfinity, 0)]
        [DataRow("NegativeInfinity", double.NegativeInfinity, 0)]
        [DataRow("double.MaxValue", double.MaxValue, 0)]
        [DataRow("double.MinValue", double.MinValue, 0)]
        [DynamicData("AsDBNullData")]
        public void AsInt32Test(string name, object o, int iExp)
        {
            Assert.AreEqual(iExp, o.AsInt32(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("25", 25, 25)]
        [DataRow("25L", 25L, 25)]
        [DataRow("long.MaxValue", long.MaxValue, 0)]
        [DataRow("long.MinValue", long.MinValue, -0)]
        [DataRow("int.MaxValue", int.MaxValue, 2147483647)]
        [DataRow("int.MinValue", int.MinValue, -2147483648)]
        [DataRow("-1", -1, -1)]
        [DataRow("Math.PI", Math.PI, 3)]
        [DataRow("NaN", double.NaN, 0)]
        [DataRow("PositiveInfinity", double.PositiveInfinity, 0)]
        [DataRow("NegativeInfinity", double.NegativeInfinity, 0)]
        [DataRow("double.MaxValue", double.MaxValue, 0)]
        [DataRow("double.MinValue", double.MinValue, 0)]
        [DynamicData("AsDBNullData")]
        public void AsInt32Test2(string name, object o, int iExp)
        {
            Assert.AreEqual(iExp, SAsIntXtntn.AsInt32(o), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("empty", "", 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("25", "25", 25)]
        [DataRow("25L", "25L", 0)] //!! ??
        [DataRow("long.MaxValue", "9223372036854775807", 0)]
        [DataRow("long.MinValue", "-9223372036854775808", -0)]
        [DataRow("int.MaxValue", "2147483647", 2147483647)]
        [DataRow("int.MinValue", "-2147483648", -2147483648)]
        [DataRow("-1", "-1", -1)]
        [DataRow("Math.PI", "3,14159265358979", 0)] // ??
        [DataRow("NaN", "NaN", 0)]
        [DataRow("Infinty", "∞", 0)]
        [DataRow("Neg.Infinity", "-∞", 0)]
        [DataRow("double.MaxValue", "1,79769313486232E+308", 0)]
        [DataRow("double.MinValue", "-1,79769313486232E+308", 0)]
        public void AsInt32Test1(string name, string s, int iExp)
        {
            Assert.AreEqual(iExp, s.AsInt32(), $"Test: {name}");
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("25", 25, 25)]
        [DataRow("25L", 25L, 25)]
        [DataRow("long.MaxValue", long.MaxValue, 9223372036854775807)]
        [DataRow("long.MinValue", long.MinValue, -9223372036854775808)]
        [DataRow("int.MaxValue", int.MaxValue, 2147483647)]
        [DataRow("int.MinValue", int.MinValue, -2147483648)]
        [DataRow("-1", "-1", -1)]
        [DataRow("Math.PI", Math.PI, 3)]
        [DataRow("double.MaxValue", double.MaxValue, 0)]
        [DataRow("double.MinValue", double.MinValue, 0)]
        [DynamicData("AsDBNullData")]
        public void AsInt64Test(string name, object o, long lExp)
        {
            Assert.AreEqual(lExp, o.AsInt64(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0L)]
        [DataRow("empty", "", 0L)]
        [DataRow("Hello", "Hello", 0L)]
        [DataRow("25", "25", 25L)]
        [DataRow("25L", "25L", 0L)] //!! ??
        [DataRow("long.MaxValue", "9223372036854775807", 9223372036854775807L)]
        [DataRow("long.MinValue", "-9223372036854775808", -9223372036854775808L)]
        [DataRow("int.MaxValue", "2147483647", 2147483647L)]
        [DataRow("int.MinValue", "-2147483648", -2147483648L)]
        [DataRow("-1", "-1", -1L)]
        [DataRow("Math.PI", "3,14159265358979", 0L)] // ??
        [DataRow("NaN", "NaN", 0L)]
        [DataRow("Infinty", "∞", 0L)]
        [DataRow("Neg.Infinity", "-∞", 0L)]
        [DataRow("double.MaxValue", "1,79769313486232E+308", 0L)]
        [DataRow("double.MinValue", "-1,79769313486232E+308", 0L)]
        public void AsInt64Test1(string name, string s, long lExp)
        {
            Assert.AreEqual(lExp, s.AsInt64(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, false)]
        [DataRow("empty", "", false)]
        [DataRow("Hello", "Hello", false)]
        [DataRow("25", "25", true)]
        [DataRow("25L", "25L", false)] //!! ??
        [DataRow("long.MaxValue", "9223372036854775807", true)] //?
        [DataRow("long.MinValue", "-9223372036854775808", true)] //?
        [DataRow("int.MaxValue", "2147483647", true)]
        [DataRow("int.MinValue", "-2147483648", true)]
        [DataRow("-1", "-1", true)]
        [DataRow("Math.PI", "3,14159265358979", false)] // ??
        [DataRow("NaN", "NaN", false)]
        [DataRow("Infinty", "∞", false)]
        [DataRow("Neg.Infinity", "-∞", false)]
        [DataRow("double.MaxValue", "1,79769313486232E+308", false)]
        [DataRow("double.MinValue", "-1,79769313486232E+308", false)]
        public void IsIntTest(string name, string s, bool xExp)
        {
            Assert.AreEqual(xExp, s.IsInt(), $"Test: {name}");
        }
    }
}