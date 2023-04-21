﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BaseLib.Helper.Tests
{
    /// <summary>
    /// Defines test class ByteUtilsTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class ByteUtilsTests
    {
        static IEnumerable<object[]> UIntTestDataIO => new[] {
        new object[]{0u, 0, true, 1u},
        new object[]{1u, 0, true, 1u},
        new object[]{0xffffffffu, 0, true, 0xffffffffu},
        new object[]{0xfffffffeu, 0, true, 0xffffffffu},
        new object[]{0u, 31, true, 0x80000000u},
        new object[]{0x80000000u, 31, true, 0x80000000u},
        new object[]{0xffffffffu, 31, true, 0xffffffffu},
        new object[]{0x7fffffffu, 31, true, 0xffffffffu},
        new object[]{0u, 0, false, 0u},
        new object[]{1u, 0, false, 0u},
        new object[]{0xffffffffu, 0, false, 0xfffffffeu},
        new object[]{0xfffffffeu, 0, false, 0xfffffffeu},
        new object[]{0u, 31, false, 0u},
        new object[]{0x80000000u, 31, false, 0u},
        new object[]{0xffffffffu, 31, false, 0x7fffffffu},
        new object[]{0x7fffffffu, 31, false, 0x7fffffffu},
        };
        static IEnumerable<object[]> IntTestDataIO => new[] {
        new object[]{0, 0, true, 1},
        new object[]{1, 0, true, 1},
        new object[]{-1, 0, true, -1},
        new object[]{-2, 0, true, -1},
        new object[]{0, 31, true, int.MinValue},
        new object[]{int.MinValue, 31, true, int.MinValue},
        new object[]{-1, 31, true, -1},
        new object[]{int.MaxValue, 31, true, -1},
        new object[]{0, 0,false, 0},
        new object[]{1, 0, false, 0},
        new object[]{-1, 0, false, -2},
        new object[]{-2, 0, false, -2},
        new object[]{0, 31, false, 0},
        new object[]{int.MinValue, 31, false, 0},
        new object[]{-1, 31, false, int.MaxValue},
        new object[]{int.MaxValue, 31, false, int.MaxValue},
        };
        static IEnumerable<object[]> LongTestDataIO => new[] {
        new object[]{0, 0,true, 1L},
        new object[]{1L, 0, true, 1L},
        new object[]{-1L, 0, true, -1L},
        new object[]{-2L, 0, true, -1L},
        new object[]{0, 63, true, long.MinValue},
        new object[]{long.MinValue, 63, true, long.MinValue},
        new object[]{-1L, 63, true, -1L},
        new object[]{long.MaxValue, 63, true, -1L},
        new object[]{0, 0,false, 0},
        new object[]{1, 0, false, 0},
        new object[]{-1L, 0, false, -2L},
        new object[]{-2L, 0, false, -2L},
        new object[]{0, 63, false, 0},
        new object[]{long.MinValue, 63, false, 0},
        new object[]{-1L, 63, false, long.MaxValue},
        new object[]{long.MaxValue, 63, false, long.MaxValue},
        };

        static IEnumerable<object[]> IntTestDataSwitch => new[] {
        new object[]{0, 0, true, 1},
        new object[]{1, 0, true, 0},
        new object[]{-1, 0, true, -2},
        new object[]{-2, 0, true, -1},
        new object[]{0, 31, true, int.MinValue},
        new object[]{int.MinValue, 31, true, 0},
        new object[]{-1, 31, true, int.MaxValue},
        new object[]{int.MaxValue, 31, true, -1},
        };

        static IEnumerable<object[]> UIntTestDataSwitch => new[] {
        new object[]{0u, 0, true, 1u},
        new object[]{1u, 0, true, 0u},
        new object[]{0xffffffffu, 0, true, 0xfffffffeu},
        new object[]{0xfffffffeu, 0, true, 0xffffffffu},
        new object[]{0u, 31, true, 0x80000000u},
        new object[]{0x80000000u, 31, true, 0x0u},
        new object[]{0xffffffffu, 31, true, 0x7fffffffu},
        new object[]{0x7fffffffu, 31, true, 0xffffffffu},
        };

        static IEnumerable<object[]> LongTestDataSwitch => new[] {
        new object[]{0, 0,true, 1L},
        new object[]{1L, 0, true, 0L},
        new object[]{-1L, 0, true, -2L},
        new object[]{-2L, 0, true, -1L},
        new object[]{0, 63, true, long.MinValue},
        new object[]{long.MinValue, 63, true, 0},
        new object[]{-1L, 63, true, long.MaxValue},
        new object[]{long.MaxValue, 63, true, -1L},
        };

        /// Switches the bit test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(IntTestDataSwitch))] 
        public void SwitchBitTest(int iVal,int ix,bool _,int iExp)
        {
            Assert.AreEqual(iExp, iVal.SwitchBit(ix));
        }

        /// <summary>
        /// Switches the bit1 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(IntTestDataSwitch))]
        public void SwitchBit1Test(int iVal, int ix,bool _, int iExp)
        {
            Assert.AreEqual(iExp, ByteUtils.SwitchBit(iVal, ix));
        }

        /// <summary>
        /// Switches the bit2 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 32)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void SwitchBit1NIOTest(int iVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(()=> ByteUtils.SwitchBit(iVal, ix));
        }

        /// Switches the bit test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(UIntTestDataSwitch))]
        public void SwitchBit0Test(uint uVal, int ix, bool _, uint uExp)
        {
            Assert.AreEqual(uExp, uVal.SwitchBit(ix));
        }

        /// <summary>
        /// Switches the bit1 test.
        /// </summary>
        /// <param name="uVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="uExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(UIntTestDataSwitch))]
        public void SwitchBit2Test(uint uVal, int ix, bool _, uint uExp)
        {
            Assert.AreEqual(uExp, ByteUtils.SwitchBit(uVal, ix));
        }

        /// <summary>
        /// Switches the bit2 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow(0u, -1)]
        [DataRow(1u, 32)]
        [DataRow(0u, int.MinValue)]
        [DataRow(1u, int.MaxValue)]
        public void SwitchBit2NIOTest(uint iVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.SwitchBit(iVal, ix));
        }

        /// <summary>
        /// Switches the bit3 test.
        /// </summary>
        /// <param name="lVal">The l value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="lExp">The l exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(LongTestDataSwitch))]
        public void SwitchBit3Test(long lVal, int ix,bool _, long lExp)
        {
            Assert.AreEqual(lExp, lVal.SwitchBit(ix));
        }

        /// <summary>
        /// Switches the bit4 test.
        /// </summary>
        /// <param name="lVal">The l value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="lExp">The l exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(LongTestDataSwitch))]
        public void SwitchBit4Test(long lVal, int ix,bool _, long lExp)
        {
            Assert.AreEqual(lExp, ByteUtils.SwitchBit(lVal, ix));
        }

        /// <summary>
        /// Switches the bit5 test.
        /// </summary>
        /// <param name="lVal">The l value.</param>
        /// <param name="ix">The ix.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 64)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void SwitchBit5NIOTest(long lVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.SwitchBit(lVal, ix));
        }

        /// <summary>
        /// Sets the bit test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(IntTestDataIO))]
        public void SetBitTest(int iVal, int ix,bool xSet, int iExp)
        {
            if (!xSet) return;
            Assert.AreEqual(iExp, iVal.SetBit(ix));
        }

        /// <summary>
        /// Sets the bit1 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(IntTestDataIO))]
        public void SetBit1Test(int iVal, int ix, bool xSet, int iExp)
        {
            if (!xSet) return;
            Assert.AreEqual(iExp, ByteUtils.SetBit(iVal, ix));
        }

        /// <summary>
        /// Sets the bit2 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 32)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void SetBit0NIOTest(int iVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.SetBit(iVal, ix));
        }

        /// <summary>
        /// Sets the bit3 test.
        /// </summary>
        /// <param name="lVal">The l value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="lExp">The l exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(LongTestDataIO))]
        public void SetBit2IOTest(long lVal, int ix, bool xSet, long lExp)
        {
            if (!xSet) return;
            Assert.AreEqual(lExp, lVal.SetBit(ix));
        }

        /// <summary>
        /// Sets the bit4 test.
        /// </summary>
        /// <param name="lVal">The l value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="lExp">The l exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(LongTestDataIO))]
        public void SetBit3IOTest(long lVal, int ix, bool xSet, long lExp)
        {
            if (!xSet) return;
            Assert.AreEqual(lExp, ByteUtils.SetBit(lVal, ix));
        }

        /// <summary>
        /// Sets the bit5 test.
        /// </summary>
        /// <param name="lVal">The l value.</param>
        /// <param name="ix">The ix.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 64)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void SetBit2NIOTest(long lVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.SetBit(lVal, ix));
        }

        /// <summary>
        /// Sets the bit1 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(UIntTestDataIO))]
        public void SetBit4Test(uint uVal, int ix, bool xSet, uint uExp)
        {
            if (!xSet) return;
            Assert.AreEqual(uExp, uVal.SetBit(ix));
        }

        /// <summary>
        /// Sets the bit1 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(IntTestDataIO))]
        public void SetBit6Test(int iVal, int ix,bool b, int iExp)
        {
            Assert.AreEqual(iExp, ByteUtils.SetBit(iVal, ix,b));
        }

        /// <summary>
        /// Sets the bit4 test.
        /// </summary>
        /// <param name="lVal">The l value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="lExp">The l exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(LongTestDataIO))]
        public void SetBit8Test(long lVal, int ix,bool b, long lExp)
        {
            Assert.AreEqual(lExp, ByteUtils.SetBit(lVal, ix,b));
        }

        /// <summary>
        /// Get the bit - test.
        /// </summary>
        /// <param name="iVal">The int value.</param>
        /// <param name="ix">The int Index.</param>
        /// <param name="xExp">The expected bool value.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(UIntTestDataIO))]
        public void SetBit10Test(uint uVal, int ix, bool b, uint uExp)
        {
            Assert.AreEqual(uExp, ByteUtils.SetBit(uVal, ix, b));
        }

        /// <summary>
        /// Get the bit - test.
        /// </summary>
        /// <param name="iVal">The int value.</param>
        /// <param name="ix">The int Index.</param>
        /// <param name="xExp">The expected bool value.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(IntTestDataIO))]
        public void GetBit0Test(int _, int ix, bool xExp, int iVal)
        {
            Assert.AreEqual(xExp, ByteUtils.GetBit(iVal, ix));
        }

        /// <summary>
        /// Get the bit - test.
        /// </summary>
        /// <param name="lVal">The long value.</param>
        /// <param name="ix">The int Index.</param>
        /// <param name="xExp">The expected bool value.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(LongTestDataIO))]
        public void GetBit2Test(long _, int ix, bool xExp, long lVal)
        {
            Assert.AreEqual(xExp, ByteUtils.GetBit(lVal, ix));
        }

        /// <summary>
        /// Clears the bit test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(UIntTestDataIO))]
        public void GetBit4Test(uint _, int ix, bool xExp, uint iVal)
        {
            Assert.AreEqual(xExp, ByteUtils.GetBit(iVal, ix));
        }

        /// <summary>
        /// Clears the bit test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(IntTestDataIO))]
        public void ClearBitTest(int iVal, int ix, bool xSet,int iExp)
        {
            if (xSet) return;
            Assert.AreEqual(iExp, iVal.ClearBit(ix));
        }

        /// <summary>
        /// Clears the bit1 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(IntTestDataIO))]
        public void ClearBit1Test(int iVal, int ix,bool xSet, int iExp)
        {
            if (xSet) return;
            Assert.AreEqual(iExp, ByteUtils.ClearBit(iVal, ix));
        }

        /// <summary>
        /// Clears the bit2 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(UIntTestDataIO))]
        public void ClearBit5Test(uint iVal, int ix,bool xSet, uint iExp)
        {
            if (xSet) return;
            Assert.AreEqual(iExp, iVal.ClearBit(ix));
        }

        /// <summary>
        /// Clears the bit1 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="iExp">The i exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(UIntTestDataIO))]
        public void ClearBit6Test(uint iVal, int ix, bool xSet, uint iExp)
        {
            if (xSet) return;
            Assert.AreEqual((uint)iExp, (uint)ByteUtils.ClearBit((uint)iVal, ix));
        }

        /// <summary>
        /// Clears the bit2 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 32)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void ClearBit2NIOTest(int iVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.ClearBit(iVal, ix));
        }

        /// <summary>
        /// Clears the bit3 test.
        /// </summary>
        /// <param name="lVal">The l value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="lExp">The l exp.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DynamicData(nameof(LongTestDataIO))]
        public void ClearBit3Test(long lVal, int ix,bool xSet, long lExp)
        {
            if (xSet) return;
            Assert.AreEqual(lExp, lVal.ClearBit(ix));
        }

        /// <summary>
        /// Clears the bit4 test.
        /// </summary>
        /// <param name="lVal">The l value.</param>
        /// <param name="ix">The ix.</param>
        /// <param name="lExp">The l exp.</param>
        /// <autogeneratedoc />
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

        /// <summary>
        /// Clears the bit5 test.
        /// </summary>
        /// <param name="lVal">The l value.</param>
        /// <param name="ix">The ix.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow(0, -1)]
        [DataRow(1, 64)]
        [DataRow(0, int.MinValue)]
        [DataRow(1, int.MaxValue)]
        public void ClearBit5NIOTest(long lVal, int ix)
        {
            Assert.ThrowsException<ArgumentException>(() => ByteUtils.ClearBit(lVal, ix));
        }

        /// <summary>
        /// Bits the mask32 test.
        /// </summary>
        /// <param name="iVal">The i value.</param>
        /// <param name="ix">The ix.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow(1, 0)]
        [DataRow(2, 1)]
        [DataRow(1073741824, 30)]
        [DataRow(int.MinValue, 31)]
        public void BitMask32Test(int iVal, int ix)
        {
            Assert.AreEqual(iVal, ByteUtils.BitMask32(ix));
        }

        /// <summary>
        /// Bits the mask32 test1.
        /// </summary>
        /// <param name="_">The .</param>
        /// <param name="ix">The ix.</param>
        /// <autogeneratedoc />
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