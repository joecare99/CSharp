using Microsoft.VisualStudio.TestTools.UnitTesting;
/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net481-windows)"
Vor:
using WinAhnenCls.Helper;
using System;
Nach:
using System;
*/

/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net6.0)"
Vor:
using WinAhnenCls.Helper;
using System;
Nach:
using System;
*/

/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net481-windows)"
Vor:
using System.Threading.Tasks;
Nach:
using System.Threading.Tasks;
using WinAhnenCls.Helper;
*/

/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net6.0)"
Vor:
using System.Threading.Tasks;
Nach:
using System.Threading.Tasks;
using WinAhnenCls.Helper;
*/


namespace WinAhnenCls.Helper.Tests
{
    [TestClass()]
    public class StringHelperTests
    {
        [DataTestMethod()]
        [DataRow(null, 3, null)]
        [DataRow("", 0, "")]
        [DataRow("123", 0, "")]
        [DataRow("123", 1, "3")]
        [DataRow("123", 2, "23")]
        [DataRow("123", 3, "123")]
        [DataRow("123", 4, "123")]
        public void RightStrTest(string? sVal, int iVal, string? sExp)
        {
            Assert.AreEqual(sExp, sVal.RightStr(iVal));
        }

        [DataTestMethod()]
        [DataRow(null, 2, null)]
        [DataRow("", 0, "")]
        [DataRow("123", 0, "")]
        [DataRow("123", 1, "1")]
        [DataRow("123", 2, "12")]
        [DataRow("123", 3, "123")]
        [DataRow("123", 4, "123")]
        public void LeftStrTest(string? sVal, int iVal, string? sExp)
        {
            Assert.AreEqual(sExp, sVal.LeftStr(iVal));
        }

    }
}