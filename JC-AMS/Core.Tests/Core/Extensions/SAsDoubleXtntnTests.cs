using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAMS.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCAMS.Core.Extensions.Tests
{
    [TestClass()]
    public class SAsDoubleXtntnTests
    {
        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 0)]
        [DataRow("0x8000000000000000", "0x8000000000000000", 0)]
        [DataRow("25", 25, 25)]
        [DataRow("25L", 25L, 25)]
        [DataRow("long.MaxValue", long.MaxValue, 9223372036854775807)]
        [DataRow("long.MinValue", long.MinValue, -9223372036854775808)]
        [DataRow("Math.PI", Math.PI, 3.141592653589792)]
        [DataRow("double.MaxValue", double.MaxValue, 1.7976931348623157E+308)]
        [DataRow("double.MinValue", double.MinValue, -1.7976931348623157E+308)]
//        [DataRow("double.NaN", double.NaN, double.NaN)]
//        [DataRow("double.NaN2", "NaN", double.NaN)]
        [DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, 0)]
        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, 0)]

        public void AsDoubleTest(string name, object o, double dExp)
        {
            Assert.AreEqual(dExp, o.AsDouble(),1e-12, $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 0)]
        [DataRow("0x8000000000000000", "0x8000000000000000", 0)]
        [DataRow("25", 25, 25)]
        [DataRow("25L", 25L, 25)]
        [DataRow("long.MaxValue", long.MaxValue, 9223372036854775807)]
        [DataRow("long.MinValue", long.MinValue, -9223372036854775808)]
        [DataRow("Math.PI", Math.PI, Math.PI)]
        [DataRow("double.MaxValue", double.MaxValue, 1.7976931348623157E+308)]
        [DataRow("double.MinValue", double.MinValue, -1.7976931348623157E+308)]
        [DataRow("double.NaN", double.NaN, double.NaN)]
        [DataRow("double.NaN2", "NaN", double.NaN)]
        [DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, 0)]
        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, 2.1219957272308E-314)]
        public void AsDoubleTest1(string name, object o, double dExp)
        {
            Assert.AreEqual(dExp, o.AsDouble(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 0)]
        [DataRow("0x8000000000000000", "0x8000000000000000", 0)]
        [DataRow("25", 25, 25)]
        [DataRow("25L", 25L, 25)]
        [DataRow("long.MaxValue", long.MaxValue, 9223372036854775807)]
        [DataRow("long.MinValue", long.MinValue, -9223372036854775808)]
        [DataRow("Math.PI", Math.PI, 3.141592653589792)]
        [DataRow("double.MaxValue", double.MaxValue, 1.7976931348623157E+308)]
        [DataRow("double.MinValue", double.MinValue, -1.7976931348623157E+308)]
        //        [DataRow("double.NaN", double.NaN, double.NaN)]
        //        [DataRow("double.NaN2", "NaN", double.NaN)]
        [DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, 0)]
        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, 0)]

        public void AsDoubleTest2(string name, object o, double dExp)
        {
            Assert.AreEqual(dExp, SAsDoubleXtntn.AsDouble(o), 1e-12, $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 0)]
        [DataRow("0x8000000000000000", "0x8000000000000000", 0)]
        [DataRow("25", 25, 25)]
        [DataRow("25L", 25L, 25)]
        [DataRow("long.MaxValue", long.MaxValue, 9223372036854775807)]
        [DataRow("long.MinValue", long.MinValue, -9223372036854775808)]
        [DataRow("Math.PI", Math.PI, Math.PI)]
        [DataRow("double.MaxValue", double.MaxValue, 1.7976931348623157E+308)]
        [DataRow("double.MinValue", double.MinValue, -1.7976931348623157E+308)]
        [DataRow("double.NaN", double.NaN, double.NaN)]
        [DataRow("double.NaN2", "NaN", double.NaN)]
        [DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, 0)]
        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, 2.1219957272308E-314)]
        public void AsDoubleTest3(string name, object o, double dExp)
        {
            Assert.AreEqual(dExp, SAsDoubleXtntn.AsDouble(o), $"Test: {name}");
        }
        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 0)]
        [DataRow("0x8000000000000000", "0x8000000000000000", 0)]
        [DataRow("25", 25, 25)]
        [DataRow("25L", 25L, 25)]
        [DataRow("long.MaxValue", long.MaxValue, 9223372036854775807)]
        [DataRow("long.MinValue", long.MinValue, -9223372036854775808)]
        [DataRow("Math.PI", Math.PI, 3.14159274101257)]
        [DataRow("double.MaxValue", double.MaxValue, float.PositiveInfinity)]
        [DataRow("double.MinValue", double.MinValue, float.NegativeInfinity)]
//        [DataRow("double.NaN", double.NaN, float.NaN)]
//        [DataRow("double.NaN2", "NaN", float.NaN)]
        [DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, 3.82047143454263E-37)]
//        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, float.NaN)]

        public void AsFloatTest(string name, object o, double fExp)
        {
            Assert.AreEqual(fExp, o.AsFloat(),1e-6, $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 0)]
        [DataRow("0x8000000000000000", "0x8000000000000000", 0)]
        [DataRow("25", 25, 25)]
        [DataRow("25L", 25L, 25)]
        [DataRow("long.MaxValue", long.MaxValue, 9223372036854775807)]
        [DataRow("long.MinValue", long.MinValue, -9223372036854775808)]
        [DataRow("Math.PI", Math.PI, (float)Math.PI)]
        [DataRow("double.MaxValue", double.MaxValue, float.PositiveInfinity)]
        [DataRow("double.MinValue", double.MinValue, float.NegativeInfinity)]
        [DataRow("double.NaN", double.NaN, float.NaN)]
        [DataRow("double.NaN2", "NaN", float.NaN)]
//        [DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, 3.82047143454263E-37)]
        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, float.NaN)]

        public void AsFloatTest1(string name, object o, double fExp)
        {
            Assert.AreEqual(fExp, o.AsFloat(),  $"Test: {name}");
        }

    }
}