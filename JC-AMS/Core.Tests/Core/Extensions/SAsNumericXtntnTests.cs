using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAMS.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;

namespace JCAMS.Core.Extensions.Tests
{
    [TestClass()]
    public class SAsNumericXtntnTests
    {
        /// <summary>
        /// Gets the test data2 ok x.
        /// </summary>
        /// <value>The test data2 ok x.</value>
        protected static IEnumerable<object?[]> AsDateTimeData => new[]
        {
            new object?[]{"null",null,DateTime.MinValue },
            new object?[]{"Hello", "Hello", DateTime.MinValue },
            new object[]{"25", 25, DateTime.MinValue },
            new object[]{"25L", 25L, DateTime.MinValue },
            new object[]{"long.MaxValue", long.MaxValue, DateTime.MinValue },
            new object[]{"long.MinValue", long.MinValue, DateTime.MinValue },
            new object[]{"Math.PI", Math.PI, DateTime.MinValue },
            new object[]{"double.MaxValue", double.MaxValue, DateTime.MinValue },
            new object[]{ "double.MinValue", double.MinValue, DateTime.MinValue },
            new object[]{"'1;2'", "1;2", DateTime.MinValue },
            new object[]{"'1.2.3'", "1.2.3",new DateTime(2003,2,1) },
            new object[]{"'2022-01-12'", "2022-01-12",new DateTime(2022,1,12) },
            new object[]{ "int[] { 3, 7 }", new int[] { 3, 7 }, DateTime.MinValue }
            // Todo: Min & Max für alle Typen
        };

        /// <summary>
        /// Gets the test data2 ok x.
        /// </summary>
        /// <value>The test data2 ok x.</value>
        protected static IEnumerable<object?[]> AsSizeData => new[]
        {
            new object?[]{"null",null,new Size(0,0) },
            new object[]{"Hello", "Hello", new Size(0,0) },
            new object[]{"25", 25, new Size(0,0) },
            new object[]{"25L", 25L, new Size(0,0) },
            new object[]{"long.MaxValue", long.MaxValue, new Size(0,0) },
            new object[]{"long.MinValue", long.MinValue, new Size(0,0) },
            new object[]{"Math.PI", Math.PI, new Size(0,0) },
            new object[]{"double.MaxValue", double.MaxValue, new Size(0,0) },
            new object[]{ "double.MinValue", double.MinValue, new Size(0, 0) },
            new object[]{"'1;2'", "1;2", new Size(1,2) },
            new object[]{ "int[] { 3, 7 }", new int[] { 3, 7 }, new Size(3, 7) }
            // Todo: Min & Max für alle Typen
        };

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData("AsDateTimeData")]
        public void AsDateTimeTest(string name,object? o, DateTime dt)
        {
            Assert.AreEqual(dt,o.AsDateTime());
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData("AsDateTimeData")]
        public void AsDateTimeTest2(string name, object? o, DateTime dt)
        {
            Assert.AreEqual(dt, SAsNumericXtntn.AsDateTime(o));
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData("AsSizeData")]
        public void AsSizeTest(string name, object? o, Size szExp)
        {
            Assert.AreEqual(szExp, o.AsSize(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, "")]
        [DataRow("Hello", "Hello", "Hello")]
        [DataRow("25", 25, "25")]
        [DataRow("25L", 25L, "25")]
        [DataRow("long.MaxValue", long.MaxValue, "9223372036854775807")]
        [DataRow("long.MinValue", long.MinValue, "-9223372036854775808")]
#if NET6_0_OR_GREATER
        [DataRow("Math.PI", Math.PI, "3,141592653589793")]
        [DataRow("double.MaxValue", double.MaxValue, "1,7976931348623157E+308")]
        [DataRow("double.MinValue", double.MinValue, "-1,7976931348623157E+308")]
#else
        [DataRow("Math.PI", Math.PI, "3,14159265358979")]
        [DataRow("double.MaxValue", double.MaxValue, "1,79769313486232E+308")]
        [DataRow("double.MinValue", double.MinValue, "-1,79769313486232E+308")]
#endif
        public void AsStringTest(string name,object o,string sExp)
        {
            Assert.AreEqual(sExp,o.AsString(),$"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, "0x0000000000000000")]
        [DataRow("Hello", "Hello", "0x0000000000000000")]
        [DataRow("25", 25, "0x0000000000000019")]
        [DataRow("25L", 25L, "0x0000000000000019")]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF")]
        [DataRow("0x8000000000000000", "0x8000000000000000", "0x8000000000000000")]
        [DataRow("long.MaxValue", long.MaxValue, "0x7FFFFFFFFFFFFFFF")]
        [DataRow("long.MinValue", long.MinValue, "0x8000000000000000")]
        [DataRow("Math.PI", Math.PI, "0x0000000000000003")]
        [DataRow("double.MaxValue", double.MaxValue, "0x8000000000000000")]
        [DataRow("double.MinValue", double.MinValue, "0x8000000000000000")]
		[DataRow("{47,11 }", new byte[] { 47, 11 }, "0x0000000000002F0B")]
		[DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, "0x0000000000010203")]
        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, "0x7FFFFFFF00000000")]
        public void AsTimeStampSTest(string name, object o, string sExp)
        {
            Assert.AreEqual(sExp, o.AsTimeStampS(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0)]
        [DataRow("Hello", "Hello", 0)]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 9223372036854775807)]
        [DataRow("0x8000000000000000", "0x8000000000000000", -9223372036854775808)]
        [DataRow("25", 25, 25)]
        [DataRow("25L", 25L, 25)]
        [DataRow("long.MaxValue", long.MaxValue, 9223372036854775807)]
        [DataRow("long.MinValue", long.MinValue, -9223372036854775808)]
        [DataRow("Math.PI", Math.PI, 3L)]
        [DataRow("double.MaxValue", double.MaxValue, -9223372036854775808)]
        [DataRow("double.MinValue", double.MinValue, -9223372036854775808)]
		[DataRow("{47,11 }", new byte[] { 47, 11 }, 12043)]
		[DataRow("{0,1,2,3 }", new byte[] {0,1,2,3 }, 66051)]
        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, 9223372032559808512)]
        public void AsTimeStampLTest(string name, object o, long lExp)
        {
            Assert.AreEqual(lExp, o.AsTimeStampL(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0UL)]
        [DataRow("Hello", "Hello", 0UL)]
        [DataRow("0x7F", "0x7F", 127UL)]
        [DataRow("0x80", "0x80", 128UL)]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 9223372036854775807UL)]
        [DataRow("0x8000000000000000", "0x8000000000000000", 9223372036854775808UL)]
        [DataRow("25", 25, 25UL)]
        [DataRow("25L", 25L, 25UL)]
        [DataRow("ulong.MaxValue", ulong.MaxValue, 18446744073709551615U)]
        [DataRow("ulong.MinValue", ulong.MinValue, 0U)]
        [DataRow("Math.PI", Math.PI, 3UL)]
        [DataRow("double.MaxValue", double.MaxValue, 0U)]
        [DataRow("double.MinValue", double.MinValue, 9223372036854775808U)]
		[DataRow("{47,11 }", new byte[] { 47, 11 }, 12043U)]
		[DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, 66051UL)]
        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, 9223372032559808512UL)]
        public void AsUInt64Test(string name, object o, ulong ulExp)
        {
            Assert.AreEqual(ulExp, o.AsUInt64(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0UL)]
        [DataRow("Hello", "Hello", 0UL)]
        [DataRow("0x7F", "0x7F", 127UL)]
        [DataRow("0x80", "0x80", 128UL)]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 9223372036854775807UL)]
        [DataRow("0x8000000000000000", "0x8000000000000000", 9223372036854775808UL)]
        [DataRow("25", 25, 25UL)]
        [DataRow("25L", 25L, 25UL)]
        [DataRow("ulong.MaxValue", ulong.MaxValue, 18446744073709551615U)]
        [DataRow("ulong.MinValue", ulong.MinValue, 0U)]
        [DataRow("Math.PI", Math.PI, 3UL)]
        [DataRow("double.MaxValue", double.MaxValue, 0U)]
        [DataRow("double.MinValue", double.MinValue, 9223372036854775808U)]
		[DataRow("{47,11 }", new byte[] { 47, 11 }, 12043U)]
		[DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, 66051UL)]
        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, 9223372032559808512UL)]
        public void AsUInt64Test1(string name, object o, ulong ulExp)
        {
            Assert.AreEqual(ulExp, SAsNumericXtntn.AsUInt64(o), $"Test: {name}");
        }


        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("null", null, 0u)]
        [DataRow("Hello", "Hello", 0u)]
        [DataRow("0x7F", "0x7F", 127u)]
        [DataRow("0x80", "0x80", 128u)]
        [DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 2147483647u)]
        [DataRow("0x8000000000000000", "0x8000000000000000", 2147483648u)]
        [DataRow("25", 25, 25u)]
        [DataRow("25L", 25L, 25u)]
        [DataRow("ulong.MaxValue", ulong.MaxValue, 0u)]
        [DataRow("ulong.MinValue", ulong.MinValue, 0U)]
        [DataRow("Math.PI", Math.PI, 3u)]
        [DataRow("double.MaxValue", double.MaxValue, 0U)]
        [DataRow("double.MinValue", double.MinValue, 0u)]
		[DataRow("{47,11 }", new byte[] { 47, 11 }, 12043U)]
		[DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, 66051U)]
        [DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, 2147483647u)]
        public void AsUInt32Test(string name, object o, uint ulExp)
        {
            Assert.AreEqual(ulExp, o.AsUInt32(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
		[DataRow("null", null, 0u)]
		[DataRow("Hello", "Hello", 0u)]
		[DataRow("0x7F", "0x7F", 127u)]
		[DataRow("0x80", "0x80", 128u)]
		[DataRow("0x7FFFFFFFFFFFFFFF", "0x7FFFFFFFFFFFFFFF", 2147483647u)]
		[DataRow("0x8000000000000000", "0x8000000000000000", 2147483648u)]
		[DataRow("25", 25, 25u)]
		[DataRow("25L", 25L, 25u)]
		[DataRow("ulong.MaxValue", ulong.MaxValue, 0u)]
		[DataRow("ulong.MinValue", ulong.MinValue, 0U)]
		[DataRow("Math.PI", Math.PI, 3u)]
		[DataRow("double.MaxValue", double.MaxValue, 0U)]
		[DataRow("double.MinValue", double.MinValue, 0u)]
		[DataRow("{47,11 }", new byte[] { 47, 11 }, 12043U)]
		[DataRow("{0,1,2,3 }", new byte[] { 0, 1, 2, 3 }, 66051U)]
		[DataRow("{127,255,255,255,0,0,0,0 }", new byte[] { 127, 255, 255, 255, 0, 0, 0, 0 }, 2147483647u)]
		public void AsUInt32Test1(string name, object o, uint ulExp)
        {
            Assert.AreEqual(ulExp, SAsNumericXtntn.AsUInt32(o), $"Test: {name}");
        }
    }
}
