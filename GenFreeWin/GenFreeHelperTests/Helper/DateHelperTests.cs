using Microsoft.VisualStudio.TestTools.UnitTesting;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseLib.Helper.TestHelper;

namespace Helper.Tests
{
    [TestClass()]
    public class DateHelperTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            _ = ((object?)null).FormatDatum("", 1);
            DateHelper.SetDate(() => new DateTime(2022, 12, 31));
        }

        [DataTestMethod()]
        [DataRow("Dat0:", null, 0, "")]
        [DataRow("Dat1:", null, 1, "Dat1:31.12.2022")]
        [DataRow("Dat2:", "", 2, "")]
        [DataRow("Dat3:", "2022/12/30", 3, "Dat3:30.12.2022")]
        public void FormatDatumTest(string sHdr, object? oAct, int iAct, string sExp)
        {
            Assert.AreEqual(sExp, oAct.FormatDatum(sHdr, iAct), $"{oAct}.FormatDatum({sHdr},{iAct})");
        }

        [TestMethod()]
        public void FormatDatumTest2()
        {
            FormatDatumTest("Dat4:", new DateTime(2022,12,29),4, "Dat4:29.12.2022");
        }

        [DataTestMethod()]
        [DataRow("19800101", "01.01.1980")]
        [DataRow("", "")]
        public void Date2DotDateStrTest(string sAct, string sExp)
        {
            Assert.AreEqual(sExp, sAct.Date2DotDateStr(), $"{sAct}.Date2DotDateStr()");
        }

        [DataTestMethod()]
        [DataRow(3, new[] { "" }, 0, new[] { "1980.01.01" })]
        [DataRow(3, null, 0, new[] { "1980.01.01", "1980.09.12", "1999.12.31" })]
        [DataRow(3, null, 1, new[] { "", "1980.01.01", "1980.09.12", "1999.12.31" })]
        [DataRow(3, null, -1, new[] { "1980.09.12", "1999.12.31" })]
        public void IntoStringTest(int iIdx, string[]? asAct, int iAct, string[] asExp)
        {
            var dAct = iIdx switch
            {
                3 => new[] { new DateTime(1980, 1, 1), new DateTime(1980, 9, 12), new DateTime(1999, 12, 31) },
                2 => new[] { new DateTime(1980, 1, 1), new DateTime(1980, 9, 12), },
                1 => new[] { new DateTime(1980, 9, 12) },
                _ => Array.Empty<DateTime>(),
            };
            AssertAreEqual(asExp, dAct.IntoString(asAct, iAct));
        }
    }
}