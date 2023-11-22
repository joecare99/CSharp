using Microsoft.VisualStudio.TestTools.UnitTesting;
/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net481-windows)"
Vor:
using WinAhnenCls.Model;
using System;
Nach:
using System;
*/

/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net6.0)"
Vor:
using WinAhnenCls.Model;
using System;
Nach:
using System;
*/

/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net481-windows)"
Vor:
using System.Threading.Tasks;
Nach:
using System.Threading.Tasks;
using WinAhnenCls.Model;
*/

/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net6.0)"
Vor:
using System.Threading.Tasks;
Nach:
using System.Threading.Tasks;
using WinAhnenCls.Model;
*/


namespace WinAhnenCls.Model.Tests
{
    [TestClass()]
    public class CHejBaseTests
    {


        [DataTestMethod()]
        [DataRow("", "", "", false, "")]
        [DataRow("", "", "1", false, "01.01.0001")]
        [DataRow("1", "1", "1", false, "01.01.0001")]
        [DataRow("1", "2", "3", false, "01.02.0003")]
        [DataRow("vo", "", "1900", false, "vo 01.01.1900")]
        public void HejDate2DateStrTest(string Day, string Month, string Year, bool dtOnly, string? sExp)
        {
            Assert.AreEqual(sExp, CHejBase.HejDate2DateStr(Day, Month, Year, dtOnly));
        }

        [DataTestMethod()]
        [DataRow("", new string[] { "", "", "" })]
        [DataRow("1", new string[] { "", "", "" })]
        [DataRow("1.2", new string[] { "", "", "" })]
        [DataRow("1.2.3", new string[] { "1", "2", "3" })]
        public void DateStr2HeyDateTest(string sVal, string[] asExp)
        {
            CHejBase.DateStr2HeyDate(sVal, out var Day, out var Month, out var Year);
            Assert.AreEqual(asExp[0], Day);
            Assert.AreEqual(asExp[1], Month);
            Assert.AreEqual(asExp[2], Year);
        }
    }
}