using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Constants.Tests
{
    [TestClass()]
    public class CMonthShortTests
    {
        [DataTestMethod()]
        [DataRow("Jan", true)]
        [DataRow("January", false)]
        [DataRow("Feb", true)]
        [DataRow("Mar", true)]
        [DataRow("Apr", true)]
        [DataRow("May", true)]
        [DataRow("Hello", false)]
        [DataRow("", false)]
        [DataRow("Mär", true)]
        [DataRow("Mai", true)]
        [DataRow("Jun", true)]
        [DataRow("Jul", true)]
        [DataRow("Aug", true)]
        [DataRow("Sep", true)]
        [DataRow("Okt", true)]
        [DataRow("Oct", true)]
        [DataRow("Nov", true)]
        [DataRow("Dez", true)]
        [DataRow("Dec", true)]
        public void IsMonthShortTest(string sAct, bool xExp)
        {
            Assert.AreEqual(xExp, sAct.IsMonthShort());
        }

        [DataTestMethod()]
        [DataRow("Jan", 1)]
        [DataRow("January", 0)]
        [DataRow("Feb", 2)]
        [DataRow("Mar", 3)]
        [DataRow("Apr", 4)]
        [DataRow("May", 5)]
        [DataRow("Hello", 0)]
        [DataRow("", 0)]
        [DataRow("Mär", 3)]
        [DataRow("Mai", 5)]
        [DataRow("Jun", 6)]
        [DataRow("Jul", 7)]
        [DataRow("Aug", 8)]
        [DataRow("Sep", 9)]
        [DataRow("Okt", 10)]
        [DataRow("Oct", 10)]
        [DataRow("Nov", 11)]
        [DataRow("Dez", 12)]
        [DataRow("Dec", 12)]
        public void MonthShortToMonthTest(string sAct, int iExp)
        {
            Assert.AreEqual(iExp, sAct.MonthShortToMonth(true));
        }

        [DataTestMethod()]
        [DataRow("January", 0)]
        [DataRow("Hello", 0)]
        [DataRow("", 0)]
        public void MonthShortToMonthTest2(string sAct, int iExp)
        {
            Assert.ThrowsException<ArgumentException>(() => sAct.MonthShortToMonth());
        }

        [DataTestMethod()]
        [DataRow(0, "???")]
        [DataRow(13, "???")]
        [DataRow(1, "JAN")]
        [DataRow(2, "FEB")]
        [DataRow(3, "MAR")]
        [DataRow(4, "APR")]
        [DataRow(5, "MAY")]
        [DataRow(6, "JUN")]
        [DataRow(7, "JUL")]
        [DataRow(8, "AUG")]
        [DataRow(9, "SEP")]
        [DataRow(10, "OCT")]
        [DataRow(11, "NOV")]
        [DataRow(12, "DEC")]

        public void MonthToMonthShortTest(int iAct, string sExp)
        {
            Assert.AreEqual(sExp, iAct.MonthToMonthShort(true));
        }

        [DataTestMethod()]
        [DataRow(0, "")]
        [DataRow(13, "")]
        public void MonthToMonthShortTest2(int iAct, string sExp)
        {
            Assert.ThrowsException<ArgumentException>(() => iAct.MonthToMonthShort());
        }


        [DataTestMethod()]
        [DataRow("???", "?", "???")]
        [DataRow("12 12 1212", " ", "12 DEC 1212")]
        [DataRow("01/11/1212", "/", "01 NOV 1212")]
        [DataRow("02-10-19", "-", "02 OCT 19")]
        [DataRow("03.09.18", ".", "03 SEP 18")]
        [DataRow("04.08.17", ".", "04 AUG 17")]
        [DataRow("05.07.16", ".", "05 JUL 16")]
        [DataRow("06.06.15", ".", "06 JUN 15")]
        [DataRow("07.05.14", ".", "07 MAY 14")]
        [DataRow("08.04.13", ".", "08 APR 13")]
        [DataRow("09.03.12", ".", "09 MAR 12")]
        [DataRow("10.02.11", ".", "10 FEB 11")]
        [DataRow("11.01.10", ".", "11 JAN 10")]
        [DataRow("12.00.09", ".", "12.00.09")]

        public void Dat2GedDatTest(string sAct, string sSep, string sExp)
        {
            Assert.AreEqual(sExp, sAct.Dat2GedDat(sSep));
        }
    }
}