using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BaseLib.Helper.Tests
{
    [TestClass()]
    public class ByteUtilsTests
    {
        [DataTestMethod()]
        [DataRow(0,0,1)]
        [DataRow(1, 0, 0)]
        [DataRow(-1, 0, -2)]
        [DataRow(-2, 0, -1)]
        [DataRow(0, 31, int.MinValue)]
        [DataRow(int.MinValue, 31, 0)]
        [DataRow(-1, 31, int.MaxValue)]
        [DataRow(int.MaxValue, 31, -1)]
        public void SwitchBitTest(int iVal,int ix,int iExp)
        {
            Assert.AreEqual(iExp, iVal.SwitchBit(ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 1)]
        [DataRow(1, 0, 0)]
        [DataRow(-1, 0, -2)]
        [DataRow(-2, 0, -1)]
        [DataRow(0, 31, int.MinValue)]
        [DataRow(int.MinValue, 31, 0)]
        [DataRow(-1, 31, int.MaxValue)]
        [DataRow(int.MaxValue, 31, -1)]
        public void SwitchBit1Test(int iVal, int ix, int iExp)
        {
            Assert.AreEqual(iExp, ByteUtils.SwitchBit(iVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 32)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void SwitchBit2Test(int iVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(()=> ByteUtils.SwitchBit(iVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 1)]
        [DataRow(1, 0, 0)]
        [DataRow(-1, 0, -2)]
        [DataRow(-2, 0, -1)]
        [DataRow(0, 63, long.MinValue)]
        [DataRow(long.MinValue, 63, 0)]
        [DataRow(-1, 63, long.MaxValue)]
        [DataRow(long.MaxValue, 63, -1)]
        public void SwitchBit3Test(long lVal, int ix, long lExp)
        {
            Assert.AreEqual(lExp, lVal.SwitchBit(ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 1)]
        [DataRow(1, 0, 0)]
        [DataRow(-1, 0, -2)]
        [DataRow(-2, 0, -1)]
        [DataRow(0, 63, long.MinValue)]
        [DataRow(long.MinValue, 63, 0)]
        [DataRow(-1, 63, long.MaxValue)]
        [DataRow(long.MaxValue, 63, -1)]
        public void SwitchBit4Test(long lVal, int ix, long lExp)
        {
            Assert.AreEqual(lExp, ByteUtils.SwitchBit(lVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 64)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void SwitchBit5Test(long lVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.SwitchBit(lVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 1)]
        [DataRow(1, 0, 1)]
        [DataRow(-1, 0, -1)]
        [DataRow(-2, 0, -1)]
        [DataRow(0, 31, int.MinValue)]
        [DataRow(int.MinValue, 31, int.MinValue)]
        [DataRow(-1, 31, -1)]
        [DataRow(int.MaxValue, 31, -1)]
        public void SetBitTest(int iVal, int ix, int iExp)
        {
            Assert.AreEqual(iExp, iVal.SetBit(ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 1)]
        [DataRow(1, 0, 1)]
        [DataRow(-1, 0, -1)]
        [DataRow(-2, 0, -1)]
        [DataRow(0, 31, int.MinValue)]
        [DataRow(int.MinValue, 31, int.MinValue)]
        [DataRow(-1, 31, -1)]
        [DataRow(int.MaxValue, 31, -1)]
        public void SetBit1Test(int iVal, int ix, int iExp)
        {
            Assert.AreEqual(iExp, ByteUtils.SetBit(iVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 32)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void SetBit2Test(int iVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.SetBit(iVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 1L)]
        [DataRow(1L, 0, 1L)]
        [DataRow(-1L, 0, -1L)]
        [DataRow(-2L, 0, -1L)]
        [DataRow(0, 63, long.MinValue)]
        [DataRow(long.MinValue, 63, long.MinValue)]
        [DataRow(-1L, 63, -1L)]
        [DataRow(long.MaxValue, 63, -1L)]
        public void SetBit3Test(long lVal, int ix, long lExp)
        {
            Assert.AreEqual(lExp, lVal.SetBit(ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 1L)]
        [DataRow(1L, 0, 1L)]
        [DataRow(-1L, 0, -1L)]
        [DataRow(-2L, 0, -1L)]
        [DataRow(0, 63, long.MinValue)]
        [DataRow(long.MinValue, 63, long.MinValue)]
        [DataRow(-1L, 63, -1L)]
        [DataRow(long.MaxValue, 63, -1L)]
        public void SetBit4Test(long lVal, int ix, long lExp)
        {
            Assert.AreEqual(lExp, ByteUtils.SetBit(lVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 64)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void SetBit5Test(long lVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.SetBit(lVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 0)]
        [DataRow(1, 0, 0)]
        [DataRow(-1, 0, -2)]
        [DataRow(-2, 0, -2)]
        [DataRow(0, 31, 0)]
        [DataRow(int.MinValue, 31, 0)]
        [DataRow(-1, 31, int.MaxValue)]
        [DataRow(int.MaxValue, 31, int.MaxValue)]
        public void ClearBitTest(int iVal, int ix, int iExp)
        {
            Assert.AreEqual(iExp, iVal.ClearBit(ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 0)]
        [DataRow(1, 0, 0)]
        [DataRow(-1, 0, -2)]
        [DataRow(-2, 0, -2)]
        [DataRow(0, 31, 0)]
        [DataRow(int.MinValue, 31, 0)]
        [DataRow(-1, 31, int.MaxValue)]
        [DataRow(int.MaxValue, 31, int.MaxValue)]
        public void ClearBit1Test(int iVal, int ix, int iExp)
        {
            Assert.AreEqual(iExp, ByteUtils.ClearBit(iVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 32)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void ClearBit2Test(int iVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.ClearBit(iVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 0)]
        [DataRow(1, 0, 0)]
        [DataRow(-1L, 0, -2L)]
        [DataRow(-2L, 0, -2L)]
        [DataRow(0, 63, 0)]
        [DataRow(long.MinValue, 63, 0)]
        [DataRow(-1L, 63, long.MaxValue)]
        [DataRow(long.MaxValue, 63, long.MaxValue)]
        public void ClearBit3Test(long lVal, int ix, long lExp)
        {
            Assert.AreEqual(lExp, lVal.ClearBit(ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 0)]
        [DataRow(1, 0, 0)]
        [DataRow(-1L, 0, -2L)]
        [DataRow(-2L, 0, -2L)]
        [DataRow(0, 63, 0)]
        [DataRow(long.MinValue, 63, 0)]
        [DataRow(-1L, 63, long.MaxValue)]
        [DataRow(long.MaxValue, 63, long.MaxValue)]
        public void ClearBit4Test(long lVal, int ix, long lExp)
        {
            Assert.AreEqual(lExp, ByteUtils.ClearBit(lVal, ix));
        }

        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 64)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void ClearBit5Test(long lVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.ClearBit(lVal, ix));
        }

        [DataTestMethod()]
        [DataRow(1, 0)]
        [DataRow(2, 1)]
        [DataRow(1073741824, 30)]
        [DataRow(int.MinValue, 31)]
        public void BitMask32Test(int iVal, int ix)
        {
            Assert.AreEqual(iVal, ByteUtils.BitMask32(ix));
        }

        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 32)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void BitMask32Test1(int _, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.BitMask32(ix));
        }

        [DataTestMethod()]
        [DataRow(1u, 0u)]
        [DataRow(2u, 1u)]
        [DataRow(1073741824u, 30u)]
        [DataRow(2147483648u, 31u)]
        public void BitMask32Test2(uint uVal, uint ix)
        {
            Assert.AreEqual(uVal, ByteUtils.BitMask32(ix));
        }

        [DataTestMethod()]
        [DataRow(1u, 32u)]
        [DataRow(1u, uint.MaxValue)]
        public void BitMask32Test3(uint _, uint ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.BitMask32(ix));
        }

        [DataTestMethod()]
        [DataRow(1L, 0)]
        [DataRow(2L, 1)]
        [DataRow(4611686018427387904L, 62)]
        [DataRow(long.MinValue, 63)]
        public void BitMask64Test(long iVal, int ix)
        {
            Assert.AreEqual(iVal, ByteUtils.BitMask64(ix));
        }

        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 64)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void BitMask64Test1(long _, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.BitMask64(ix));
        }
    }
}