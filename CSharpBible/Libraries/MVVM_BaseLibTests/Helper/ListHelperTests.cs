using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseLib.Helper.TestHelper;

namespace BaseLib.Helper.Tests
{
    [TestClass()]
    public class ListHelperTests
    {
        [DataTestMethod()]
        [DataRow(new object[] { 0, 1, 2, 3, 4 }, 2, 2, new object[] { 0, 1, 2, 3, 4 })]
        [DataRow(new object[] { 0, 1, 2, 3, 4 }, 4, 0, new object[] { 4, 0, 1, 2, 3 })]
        [DataRow(new object[] { 0, 1, 2, 3, 4 }, 1, 3, new object[] { 0, 2, 1, 3, 4 })]
        [DataRow(new object[] { 0, 1, 2, 3, 4 }, 2, 5, new object[] { 0, 1, 3, 4, 2 })]
        [DataRow(new object[] { 0, 1, 2, 3, 4 }, 1, 6, new object[] { 0, 1, 2, 3, 4 })]
        public void MoveItemTest(object[] aoSrc, int iSrc, int iDst, object[] asExp)
        {
            var aoResult = aoSrc.ToList();
            if (iDst > asExp.Length)
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => aoResult.MoveItem(iSrc, iDst));
            else
            aoResult.MoveItem(iSrc, iDst);
            AssertAreEqual(asExp, aoResult.ToArray());
        }

        [DataTestMethod()]
        [DataRow(new object[] { 0, 1, 2, 3, 4 }, 2, 2, new object[] { 0, 1, 2, 3, 4 })]
        [DataRow(new object[] { 0, 1, 2, 3, 4 }, 4, 0, new object[] { 4, 1, 2, 3, 0 })]
        public void SwapTest(object[] aoSrc, int iSrc, int iDst, object[] asExp)
        {
            aoSrc.Swap(iSrc, iDst);
            AssertAreEqual(asExp, aoSrc);
        }

        [DataTestMethod()]
        [DataRow(0, -1, new int[] { })]
        [DataRow(2, 2, new[] { 2 })]
        [DataRow(0, 4, new[] { 0, 1, 2, 3, 4 })]
        public void RangeTest(int iSrc, int iDst, int[] aiExp)
        {
            AssertAreEqual(aiExp, typeof(int).Range(iSrc, iDst));
        }

        [DataTestMethod()]
        [DataRow('2', '6', new char[] { '2', '3', '4', '5', '6' })]
        public void RangeTest2(char iSrc, char iDst, char[] aoExp)
        {
            AssertAreEqual(aoExp, typeof(object).Range(iSrc, iDst));
        }

        [DataTestMethod()]
        [DataRow((byte)0, (byte)4, new byte[] { 0, 1, 2, 3, 4 })]
        public void RangeTest3(byte iSrc, byte iDst, byte[] aoExp)
        {
            AssertAreEqual(aoExp, typeof(object).Range(iSrc, iDst));
        }

        [DataTestMethod()]
        [DataRow(0, -1, new int[] { })]
        [DataRow(2, 2, new[] { 2 })]
        [DataRow(0, 4, new[] { 0, 1, 2, 3, 4 })]
        public void ToTest(int iSrc, int iDst, int[] aoExp)
        {
            AssertAreEqual(aoExp, iSrc.To(iDst));
        }
    }
}