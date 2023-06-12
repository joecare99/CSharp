﻿using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Helper.Tests
{
    /// <summary>
    /// Defines test class TypeUtilsTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class TypeUtilsTests
    {

        [DataTestMethod()]
        [DataRow(TypeCode.Object, "System.Object")]
        [DataRow(TypeCode.DBNull, "System.DBNull")]
        [DataRow(TypeCode.Boolean, "System.Boolean")]
        [DataRow(TypeCode.Char, "System.Char")]
        [DataRow(TypeCode.SByte, "System.SByte")]
        [DataRow(TypeCode.Byte, "System.Byte")]
        [DataRow(TypeCode.Int16, "System.Int16")]
        [DataRow(TypeCode.UInt16, "System.UInt16")]
        [DataRow(TypeCode.Int32, "System.Int32")]
        [DataRow(TypeCode.UInt32, "System.UInt32")]
        [DataRow(TypeCode.Int64, "System.Int64")]
        [DataRow(TypeCode.UInt64, "System.UInt64")]
        [DataRow(TypeCode.Single, "System.Single")]
        [DataRow(TypeCode.Double, "System.Double")]
        [DataRow(TypeCode.Decimal, "System.Decimal")]
        [DataRow(TypeCode.DateTime, "System.DateTime")]
        [DataRow(TypeCode.String, "System.String")]
        public void TCTest(TypeCode tc, string ts ) {
            var tpe = Type.GetType(ts, false, true);
            Assert.AreEqual(tc, tpe.TC());
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object, "System.Object")]
        [DataRow(TypeCode.DBNull, "System.DBNull")]
        [DataRow(TypeCode.Boolean, "System.Boolean")]
        [DataRow(TypeCode.Char, "System.Char")]
        [DataRow(TypeCode.SByte, "System.SByte")]
        [DataRow(TypeCode.Byte, "System.Byte")]
        [DataRow(TypeCode.Int16, "System.Int16")]
        [DataRow(TypeCode.UInt16, "System.UInt16")]
        [DataRow(TypeCode.Int32, "System.Int32")]
        [DataRow(TypeCode.UInt32, "System.UInt32")]
        [DataRow(TypeCode.Int64, "System.Int64")]
        [DataRow(TypeCode.UInt64, "System.UInt64")]
        [DataRow(TypeCode.Single, "System.Single")]
        [DataRow(TypeCode.Double, "System.Double")]
        [DataRow(TypeCode.Decimal, "System.Decimal")]
        [DataRow(TypeCode.DateTime, "System.DateTime")]
        [DataRow(TypeCode.String, "System.String")]
        public void TCTest2(TypeCode tc, string ts)
        {
            var t = Type.GetType(ts, false, true);
            Assert.AreEqual(tc, TypeUtils.TC(t));
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object, "System.Object")]
        [DataRow(TypeCode.DBNull, "System.DBNull")]
        [DataRow(TypeCode.Boolean, "System.Boolean")]
        [DataRow(TypeCode.Char, "System.Char")]
        [DataRow(TypeCode.SByte, "System.SByte")]
        [DataRow(TypeCode.Byte, "System.Byte")]
        [DataRow(TypeCode.Int16, "System.Int16")]
        [DataRow(TypeCode.UInt16, "System.UInt16")]
        [DataRow(TypeCode.Int32, "System.Int32")]
        [DataRow(TypeCode.UInt32, "System.UInt32")]
        [DataRow(TypeCode.Int64, "System.Int64")]
        [DataRow(TypeCode.UInt64, "System.UInt64")]
        [DataRow(TypeCode.Single, "System.Single")]
        [DataRow(TypeCode.Double, "System.Double")]
        [DataRow(TypeCode.Decimal, "System.Decimal")]
        [DataRow(TypeCode.DateTime, "System.DateTime")]
        [DataRow(TypeCode.String, "System.String")]
        public void ToTypeTest(TypeCode tc, string ts)
        {
            Assert.AreEqual(ts, tc.ToType()?.ToString());
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object, "System.Object")]
        [DataRow(TypeCode.DBNull, "System.DBNull")]
        [DataRow(TypeCode.Boolean, "System.Boolean")]
        [DataRow(TypeCode.Char, "System.Char")]
        [DataRow(TypeCode.SByte, "System.SByte")]
        [DataRow(TypeCode.Byte, "System.Byte")]
        [DataRow(TypeCode.Int16, "System.Int16")]
        [DataRow(TypeCode.UInt16, "System.UInt16")]
        [DataRow(TypeCode.Int32, "System.Int32")]
        [DataRow(TypeCode.UInt32, "System.UInt32")]
        [DataRow(TypeCode.Int64, "System.Int64")]
        [DataRow(TypeCode.UInt64, "System.UInt64")]
        [DataRow(TypeCode.Single, "System.Single")]
        [DataRow(TypeCode.Double, "System.Double")]
        [DataRow(TypeCode.Decimal, "System.Decimal")]
        [DataRow(TypeCode.DateTime, "System.DateTime")]
        [DataRow(TypeCode.String, "System.String")]
        public void ToTypeTest2(TypeCode tc, string ts)
        {
            Assert.AreEqual(ts, TypeUtils.ToType(tc)?.ToString());
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object, "Object")]
        [DataRow(TypeCode.DBNull, "DBNull")]
        [DataRow(TypeCode.Boolean, "Boolean")]
        [DataRow(TypeCode.Char, "Char")]
        [DataRow(TypeCode.SByte, "SByte")]
        [DataRow(TypeCode.Byte, "Byte")]
        [DataRow(TypeCode.Int16, "Int16")]
        [DataRow(TypeCode.UInt16, "UInt16")]
        [DataRow(TypeCode.Int32, "Int32")]
        [DataRow(TypeCode.UInt32, "UInt32")]
        [DataRow(TypeCode.Int64, "Int64")]
        [DataRow(TypeCode.UInt64, "UInt64")]
        [DataRow(TypeCode.Single, "Single")]
        [DataRow(TypeCode.Double, "Double")]
        [DataRow(TypeCode.Decimal, "Decimal")]
        [DataRow(TypeCode.DateTime, "DateTime")]
        [DataRow(TypeCode.String, "String")]
        public void ToTypeTest3(TypeCode tc, string ts)
        {
            Assert.AreEqual(tc, ts.ToType().TC());
        }

        [TestMethod()]
        public void ToTypeTest4()
        {
            Assert.AreEqual(typeof(object), "BumLux".ToType());
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object, "Object")]
        [DataRow(TypeCode.DBNull, "DBNull")]
        [DataRow(TypeCode.Boolean, "Boolean")]
        [DataRow(TypeCode.Char, "Char")]
        [DataRow(TypeCode.SByte, "SByte")]
        [DataRow(TypeCode.Byte, "Byte")]
        [DataRow(TypeCode.Int16, "Int16")]
        [DataRow(TypeCode.UInt16, "UInt16")]
        [DataRow(TypeCode.Int32, "Int32")]
        [DataRow(TypeCode.UInt32, "UInt32")]
        [DataRow(TypeCode.Int64, "Int64")]
        [DataRow(TypeCode.UInt64, "UInt64")]
        [DataRow(TypeCode.Single, "Single")]
        [DataRow(TypeCode.Double, "Double")]
        [DataRow(TypeCode.Decimal, "Decimal")]
        [DataRow(TypeCode.DateTime, "DateTime")]
        [DataRow(TypeCode.String, "String")]
        public void ToTypeTest5(TypeCode tc, string ts)
        {
            Assert.AreEqual(tc, ts.ToLower().ToType().TC());
            Assert.AreEqual(tc, ts.ToUpper().ToType().TC());
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object,   true)]
        [DataRow(TypeCode.DBNull,   true)]
        [DataRow(TypeCode.Boolean,  true)]
        [DataRow(TypeCode.Char,     true)]
        [DataRow(TypeCode.SByte,    true)]
        [DataRow(TypeCode.Byte,     true)]
        [DataRow(TypeCode.Int16,    true)]
        [DataRow(TypeCode.UInt16,   true)]
        [DataRow(TypeCode.Int32,    true)]
        [DataRow(TypeCode.UInt32,   true)]
        [DataRow(TypeCode.Int64,    true)]
        [DataRow(TypeCode.UInt64,   true)]
        [DataRow(TypeCode.Single,   true)]
        [DataRow(TypeCode.Double,   true)]
        [DataRow(TypeCode.Decimal,  true)]
        [DataRow(TypeCode.DateTime, true)]
        [DataRow(TypeCode.String,   true)]
        public void CompareTest0(TypeCode tc, bool _)
        {
            static bool? f(Type? t) => t?.Compare(t.Get(1), t.Get(0));
            Assert.AreEqual(false, f(tc.ToType()));
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object, false)]
        [DataRow(TypeCode.DBNull, false)]
        [DataRow(TypeCode.Boolean, true)]
        [DataRow(TypeCode.Char, true)]
        [DataRow(TypeCode.SByte, true)]
        [DataRow(TypeCode.Byte, true)]
        [DataRow(TypeCode.Int16, true)]
        [DataRow(TypeCode.UInt16, true)]
        [DataRow(TypeCode.Int32, true)]
        [DataRow(TypeCode.UInt32, true)]
        [DataRow(TypeCode.Int64, true)]
        [DataRow(TypeCode.UInt64, true)]
        [DataRow(TypeCode.Single, true)]
        [DataRow(TypeCode.Double, true)]
        [DataRow(TypeCode.Decimal, false)]
        [DataRow(TypeCode.DateTime, false)]
        [DataRow(TypeCode.String, false)]
        public void CompareTest1(TypeCode tc, bool xOK)
        {
            static bool? f(Type? t) => t?.Compare(t.Get(0), t.Get(1));
            Assert.AreEqual(xOK, f(tc.ToType()));
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object, false)]
        [DataRow(TypeCode.DBNull, false)]
        [DataRow(TypeCode.Boolean, true)]
        [DataRow(TypeCode.Char, false)]
        [DataRow(TypeCode.SByte, false)]
        [DataRow(TypeCode.Byte, false)]
        [DataRow(TypeCode.Int16, false)]
        [DataRow(TypeCode.UInt16, false)]
        [DataRow(TypeCode.Int32, false)]
        [DataRow(TypeCode.UInt32, false)]
        [DataRow(TypeCode.Int64, false)]
        [DataRow(TypeCode.UInt64, false)]
        [DataRow(TypeCode.Single, false)]
        [DataRow(TypeCode.Double, false)]
        [DataRow(TypeCode.Decimal, false)]
        [DataRow(TypeCode.DateTime, false)]
        [DataRow(TypeCode.String, false)]
        public void CompareTest2(TypeCode tc, bool xOK)
        {
            static bool? f(Type? t) => t?.Compare(null, t.Get(1));
            Assert.AreEqual(xOK, f(tc.ToType()));
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object, false)]
        [DataRow(TypeCode.DBNull, false)]
        [DataRow(TypeCode.Boolean, false)]
        [DataRow(TypeCode.Char, false)]
        [DataRow(TypeCode.SByte, false)]
        [DataRow(TypeCode.Byte, false)]
        [DataRow(TypeCode.Int16, false)]
        [DataRow(TypeCode.UInt16, false)]
        [DataRow(TypeCode.Int32, false)]
        [DataRow(TypeCode.UInt32, false)]
        [DataRow(TypeCode.Int64, false)]
        [DataRow(TypeCode.UInt64, false)]
        [DataRow(TypeCode.Single, false)]
        [DataRow(TypeCode.Double, false)]
        [DataRow(TypeCode.Decimal, false)]
        [DataRow(TypeCode.DateTime, false)]
        [DataRow(TypeCode.String, false)]
        public void CompareTest3(TypeCode tc, bool xOK)
        {
            static bool? f(Type? t) => t?.Compare(t.Get(0), null);
            Assert.AreEqual(xOK, f(tc.ToType()));
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object,0)]
        [DataRow(TypeCode.DBNull, 0)]
        [DataRow(TypeCode.Boolean, false)]
        [DataRow(TypeCode.Char, '\x0')]
        [DataRow(TypeCode.SByte, (sbyte)0)]
        [DataRow(TypeCode.Byte, (byte)0)]
        [DataRow(TypeCode.Int16, (short)0)]
        [DataRow(TypeCode.UInt16, (ushort)0u)]
        [DataRow(TypeCode.Int32, (int)0)]
        [DataRow(TypeCode.UInt32, (uint)0u)]
        [DataRow(TypeCode.Int64, (long)0)]
        [DataRow(TypeCode.UInt64, (ulong)0u)]
        [DataRow(TypeCode.Single, 0f)]
        [DataRow(TypeCode.Double, 0d)]
        [DataRow(TypeCode.Decimal, 0)]
        [DataRow(TypeCode.DateTime, 0)]
        [DataRow(TypeCode.String, "0")]
        public void GetTest0(TypeCode tc, object oExp)
        {
            Assert.AreEqual(oExp, tc.ToType()?.Get(0));
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Object, "")]
        [DataRow(TypeCode.DBNull, "")]
        [DataRow(TypeCode.Boolean, false)]
        [DataRow(TypeCode.Char, '\x0')]
        [DataRow(TypeCode.SByte, (sbyte)0)]
        [DataRow(TypeCode.Byte, (byte)0)]
        [DataRow(TypeCode.Int16, (short)0)]
        [DataRow(TypeCode.UInt16, (ushort)0u)]
        [DataRow(TypeCode.Int32, (int)0)]
        [DataRow(TypeCode.UInt32, (uint)0u)]
        [DataRow(TypeCode.Int64, (long)0)]
        [DataRow(TypeCode.UInt64, (ulong)0u)]
        [DataRow(TypeCode.Single, 0f)]
        [DataRow(TypeCode.Double, 0d)]
        [DataRow(TypeCode.Decimal, "")]
        [DataRow(TypeCode.DateTime, "")]
        [DataRow(TypeCode.String, "")]
        public void GetTest1(TypeCode tc, object oExp)
        {
            Assert.AreEqual(oExp, tc.ToType()?.Get(""));
        }

        [TestMethod]
        public void GetTest2()
        {
            Assert.AreEqual(0, typeof(int).Get(null));
            Assert.AreEqual(null, typeof(DBNull).Get(null));
            Assert.AreEqual(typeof(object), typeof(Type).Get(null));
            Assert.AreEqual(typeof(object), typeof(Type).Get(""));
            Assert.AreEqual(typeof(bool), typeof(Type).Get(TypeCode.Boolean));
            Assert.AreEqual(typeof(int), typeof(Type).Get("Int32"));
            Assert.AreEqual(typeof(long), typeof(Type).Get("System.Int64"));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 1, true)]
        [DataRow(1, 0, 1, true)]
        [DataRow(-1, 0, 1, false)]
        [DataRow(2, 0, 1, false)]
        [DataRow(0, -1, 1, true)]
        [DataRow(0, 1, -1, true)]
        [DataRow(2, 1, -1, true)]
        public void CheckLimitTest0(int i1, int im, int ix, bool xExp)
        {
            Assert.AreEqual(xExp, i1.CheckLimit(im, ix));
        }


        [DataTestMethod()]
        [DataRow(0,0,1,true)]
        [DataRow(1, 0, 1, true)]
        [DataRow(-1, 0, 1, false)]
        [DataRow(2, 0, 1, false)]
        [DataRow(0, -1, 1, true)]
        [DataRow(0, 1, -1, false)]
        [DataRow(2, 1, -1, false)]
        public void IsBetweenInclTest0(int i1,int im,int ix, bool xExp)
        {
            Assert.AreEqual(xExp, i1.IsBetweenIncl(im,ix));
        }

        [DataTestMethod()]
        [DataRow(0, 0, 1, false)]
        [DataRow(1, 0, 1, false)]
        [DataRow(-1, 0, 1, false)]
        [DataRow(2, 0, 1, false)]
        [DataRow(0, -1, 1, true)]
        [DataRow(0, 1, -1, false)]
        [DataRow(2, 1, -1, false)]
        public void IsBetweenExclTest0(int i1, int im, int ix, bool xExp)
        {
            Assert.AreEqual(xExp, i1.IsBetweenExcl(im, ix));
        }

    }
}
