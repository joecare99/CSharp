using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using JCAMS.Core.Tests;

namespace JCAMS.Core.Math2.Tests
{
    [TestClass()]
    public class SMathTests
    {
        protected static IEnumerable<Object[]> AritmeticAverageData => new[]
        {
            new object[] {"Field100", TestData.SortedArrayDouble1000 ,true, 499.5d}
        };

        protected static IEnumerable<Object[]> StandardDeviationData => new[]
        {
            new object[] {"Field100", TestData.SortedArrayDouble1000 ,true, 499.5d, 288.67499025721 }
        };

        protected static IEnumerable<Object[]> SqrData => new[]
        {
            new object[] {SMath.SqRoot_Of_2, 2}
        };

        [TestMethod()]
        public void SqRoot_Of_2Test()
        {
            Assert.AreEqual(2d, SMath.SqRoot_Of_2 * SMath.SqRoot_Of_2, 1e-12, "The sqare-root of 2");
        }

        [TestMethod()]
        public void PI_DIV_180Test()
        {
            Assert.AreEqual(-1d, Math.Cos(180 * SMath.PI_DIV_180), 1e-14, "pi/180");
            Assert.AreEqual(0d, Math.Cos(90 * SMath.PI_DIV_180), 1e-14, "pi/180");
            Assert.AreEqual(0d, Math.Sin(180 * SMath.PI_DIV_180), 1e-14, "pi/180");
            Assert.AreEqual(1d, Math.Sin(90 * SMath.PI_DIV_180), 1e-14, "pi/180");
        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity)]
        [DynamicData("AritmeticAverageData")]
        public void ArithmeticAverageTest(string name, double[] ad, bool xExp, double dRes)
        {

            var ld = (ad != null) ? new List<double>(ad.ToList()) : null;
            Assert.AreEqual(xExp, SMath.ArithmeticAverage(out double dAct, ld), $"Test:{name}");
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, dAct, $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, dAct, 1e-12, $"Test:{name}.result1");
        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity)]
        [DynamicData("AritmeticAverageData")]
        public void ArithmeticAverageTest2(string name, double[] ad, bool xExp, double dRes)
        {
            Assert.AreEqual(xExp, SMath.ArithmeticAverage(out double dAct, ad), $"Test:{name}");
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, dAct, $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, dAct, 1e-12, $"Test:{name}.result1");
        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity)]
        [DynamicData("AritmeticAverageData")]
        public void ArithmeticAverageTest1(string name, double[] ad, bool xExp, double dRes)
        {
            Queue<double> q = (ad == null) ? null : new Queue<double>(ad.ToList());
            Assert.AreEqual(xExp, SMath.ArithmeticAverage(out double dAct, q), $"Test:{name}");
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, dAct, $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, dAct, 1e-12, $"Test:{name}.result1");
        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity)]
        [DynamicData("AritmeticAverageData")]
        public void ArithmeticAverageTest3(string name, double[] ad, bool xExp, double dRes)
        {
            var ld = (ad != null) ? new List<double>(ad.ToList()) : null;
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, SMath.ArithmeticAverage(ld), $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, SMath.ArithmeticAverage(ld), 1e-12, $"Test:{name}.result1");

        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity)]
        [DynamicData("AritmeticAverageData")]
        public void ArithmeticAverageTest3a(string name, double[] ad, bool xExp, double dRes)
        {
            var ld = (ad != null) ? new List<double>(ad.ToList()) : null;
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, ld.ArithmeticAverage(), $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, ld.ArithmeticAverage(), 1e-12, $"Test:{name}.result1");

        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity)]
        [DynamicData("AritmeticAverageData")]
        public void ArithmeticAverageTest4(string name, double[] ad, bool xExp, double dRes)
        {
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, SMath.ArithmeticAverage(ad), $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, SMath.ArithmeticAverage(ad), 1e-12, $"Test:{name}.result1");

        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity)]
        [DynamicData("AritmeticAverageData")]
        public void ArithmeticAverageTest4a(string name, double[] ad, bool xExp, double dRes)
        {
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, ad.ArithmeticAverage(), $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, ad.ArithmeticAverage(), 1e-12, $"Test:{name}.result1");

        }

        [DataTestMethod()]
        [DataRow("Empty",0,0,0,false,double.NaN)]
        [DataRow("01 (0;6;1)", 0, 6, 1, true, 1)]
        [DataRow("01 (6;12;1)", 6, 12, 1, true, 1)]
        [DataRow("01 (6;0;1)", 6, 0, 1, true, 1)]
        [DataRow("01 (12;6;1)", 12, 6, 1, true, 1)]
        [DataRow("01 (6;12;0.5)", 6, 12, 0.5, true, 2)]
        [DataRow("01 (12;6;0.5)", 12, 6, 0.5, true, 2)]
        public void CpTest(string name, double dMin, double dMax, double dSigma, bool xExp, double dExp)
        {
            Assert.AreEqual(xExp, SMath.Cp(out double dVal, dMin, dMax, dSigma), $"Test: {name}");
            Assert.AreEqual(dExp, dVal);
        }

        [DataTestMethod()]
        [DataRow("Empty", 0, 0, 0, false, double.NaN)]
        [DataRow("01 (0;6;1)", 0, 6, 1, true, 1)]
        [DataRow("01 (6;12;1)", 6, 12, 1, true, 1)]
        [DataRow("01 (6;0;1)", 6, 0, 1, true, 1)]
        [DataRow("01 (12;6;1)", 12, 6, 1, true, 1)]
        [DataRow("01 (6;12;0.5)", 6, 12, 0.5, true, 2)]
        [DataRow("01 (12;6;0.5)", 12, 6, 0.5, true, 2)]
        public void CpTest1(string name, double dMin, double dMax, double dSigma, bool xExp, double dExp)
        {
            Assert.AreEqual(dExp, SMath.Cp(dMin, dMax, dSigma), $"Test: {name}");
        }

        [DataTestMethod()]
        [DataRow("Empty", 0, 0, 0, 0, false, false, double.NaN)]
        [DataRow("Empty2", 0, 0, 0, 1, false, false, double.NaN)]
        [DataRow("NaN", double.NaN, 6, 3, 1, false, true, 1)]
        [DataRow("NaN2", double.NaN, 6, 3, 1, true, false, double.NaN)]
        [DataRow("NaN", 0,double.NaN,  3, 1, false, false, double.NaN)] //??
        [DataRow("NaN2",0, double.NaN,  3, 1, true, false, double.NaN)]
        [DataRow("01 (0;6;1)", 0, 6, 3, 1, false, true, 1)]
        [DataRow("01 (6;12;1)", 6, 12, 9, 1, false, true, 1)]
        [DataRow("01 (6;0;1)", 6, 0, 3, 1, false, true, -1)]
        [DataRow("01 (12;6;1)", 12, 6, 9, 1, false, true, -1)]
        [DataRow("01 (6;12;0.5)", 6, 12, 9, 0.5, false, true, 2)]
        [DataRow("01 (12;6;0.5)", 12, 6, 9, 0.5, false, true, -2)]
        public void CpKTest(string name, double dMin, double dMax, double dMW, double dSigma, bool xFlag, bool xExp, double dExp)
        {
            Assert.AreEqual(xExp, SMath.CpK(out double dVal, dMin, dMax,dMW, dSigma,xFlag), $"Test: {name}");
            Assert.AreEqual(dExp, dVal);
        }

        [TestMethod()]
        public void Deg2RadTest()
        {
            Assert.AreEqual(-1f, Math.Cos(SMath.Deg2Rad(180f)), 1e-6, "c180°");
            Assert.AreEqual(0f, Math.Cos(SMath.Deg2Rad(90f)), 1e-6, "c90°");
            Assert.AreEqual(0f, Math.Sin(SMath.Deg2Rad(180f)), 1e-6, "s180°");
            Assert.AreEqual(1f, Math.Sin(SMath.Deg2Rad(90f)), 1e-6, "s90°");
        }

        [TestMethod()]
        public void Deg2RadTest1()
        {
            Assert.AreEqual(-1d, Math.Cos(SMath.Deg2Rad(180d)), 1e-12, "c180°");
            Assert.AreEqual(0d, Math.Cos(SMath.Deg2Rad(90d)), 1e-12, "c90°");
            Assert.AreEqual(0d, Math.Sin(SMath.Deg2Rad(180d)), 1e-12, "s180°");
            Assert.AreEqual(1d, Math.Sin(SMath.Deg2Rad(90d)), 1e-12, "s90°");
        }

        [DataTestMethod()]
        [DataRow("0", 0d, 0d, 0d, 0d)]
        [DataRow("1 -1;0;1", -1d, 0d, 1d, 0d)]
        [DataRow("2 -1;-2;1", -1d, -2d, 1d, -1d)]
        [DataRow("3 -1;-1;1", -1d, -1d, 1d, -1d)]
        [DataRow("4 -1;1;1", -1d, 1d, 1d, 1d)]
        [DataRow("5 -1;2;1", -1d, 2d, 1d, 1d)]
        [DataRow("6 10;9;12", 10d, 9d, 12d, 10d)]
        [DataRow("07 10;10;12", 10d, 10d, 12d, 10d)]
        [DataRow("08 10;11;12", 10d, 11d, 12d, 11d)]
        [DataRow("09 10;12;12", 10d, 12d, 12d, 12d)]
        [DataRow("0A 10;13;12", 10d, 13d, 12d, 12d)]
        public void MinMaxTest(string name, double dMin, double dVal, double dMax, double dExp)
        {
            Assert.AreEqual(dExp, SMath.MinMax(dMin, dVal, dMax));
        }

        [DataTestMethod()]
        [DataRow("0", 0f, 0f, 0f, 0f)]
        [DataRow("1 -1;0;1", -1f, 0f, 1f, 0f)]
        [DataRow("2 -1;-2;1", -1f, -2f, 1f, -1f)]
        [DataRow("3 -1;-1;1", -1f, -1f, 1f, -1f)]
        [DataRow("4 -1;1;1", -1f, 1f, 1f, 1f)]
        [DataRow("5 -1;2;1", -1f, 2f, 1f, 1f)]
        [DataRow("6 10;9;12", 10f, 9f, 12f, 10f)]
        [DataRow("07 10;10;12", 10f, 10f, 12f, 10f)]
        [DataRow("08 10;11;12", 10f, 11f, 12f, 11f)]
        [DataRow("09 10;12;12", 10f, 12f, 12f, 12f)]
        [DataRow("0A 10;13;12", 10f, 13f, 12f, 12f)]
        public void MinMaxTest1(string name, float dMin, float dVal, float dMax, float dExp)
        {
            Assert.AreEqual(dExp, SMath.MinMax(dMin, dVal, dMax));
        }

        [DataTestMethod()]
        [DataRow("0", 0, 0, 0, 0)]
        [DataRow("1 -1;0;1", -1, 0, 1, 0)]
        [DataRow("2 -1;-2;1", -1, -2, 1, -1)]
        [DataRow("3 -1;-1;1", -1, -1, 1, -1)]
        [DataRow("4 -1;1;1", -1, 1, 1, 1)]
        [DataRow("5 -1;2;1", -1, 2, 1, 1)]
        [DataRow("6 10;9;12", 10, 9, 12, 10)]
        [DataRow("07 10;10;12", 10, 10, 12, 10)]
        [DataRow("08 10;11;12", 10, 11, 12, 11)]
        [DataRow("09 10;12;12", 10, 12, 12, 12)]
        [DataRow("0A 10;13;12", 10, 13, 12, 12)]
        public void MinMaxTest2(string name, int dMin, int dVal, int dMax, int dExp)
        {
            Assert.AreEqual(dExp, SMath.MinMax(dMin, dVal, dMax));
        }

        [DataTestMethod()]
        [DataRow("0", 0, 0, 0, 0)]
        [DataRow("1 -1;0;1", -1, 0, 1, 0)]
        [DataRow("2 -1;-2;1", -1, -2, 1, -1)]
        [DataRow("3 -1;-1;1", -1, -1, 1, -1)]
        [DataRow("4 -1;1;1", -1, 1, 1, 1)]
        [DataRow("5 -1;2;1", -1, 2, 1, 1)]
        [DataRow("6 10;9;12", 10, 9, 12, 10)]
        [DataRow("07 10;10;12", 10, 10, 12, 10)]
        [DataRow("08 10;11;12", 10, 11, 12, 11)]
        [DataRow("09 10;12;12", 10, 12, 12, 12)]
        [DataRow("0A 10;13;12", 10, 13, 12, 12)]
        public void MinMaxTest2a(string name, int dMin, int dVal, int dMax, int dExp)
        {
            Assert.AreEqual((long)dExp, SMath.MinMax((long)dMin, (long)dVal, (long)dMax));
        }

        [DataTestMethod()]
        [DataRow("0", 0, 0, 0, 0)]
        [DataRow("1 -1;0;1", -1, 0, 1, 0)]
        [DataRow("2 -1;-2;1", -1, -2, 1, -1)]
        [DataRow("3 -1;-1;1", -1, -1, 1, -1)]
        [DataRow("4 -1;1;1", -1, 1, 1, 1)]
        [DataRow("5 -1;2;1", -1, 2, 1, 1)]
        [DataRow("6 10;9;12", 10, 9, 12, 10)]
        [DataRow("07 10;10;12", 10, 10, 12, 10)]
        [DataRow("08 10;11;12", 10, 11, 12, 11)]
        [DataRow("09 10;12;12", 10, 12, 12, 12)]
        [DataRow("0A 10;13;12", 10, 13, 12, 12)]
        public void MinMaxTest2b(string name, int dMin, int dVal, int dMax, int dExp)
        {
            Assert.AreEqual((decimal)dExp, SMath.MinMax((decimal)dMin, (decimal)dVal, (decimal)dMax));
        }

        [DataTestMethod()]
        [DataRow("0", 0u, 0u, 0u, 0u)]
        [DataRow("1 1;0;3", 1u, 0u, 3u, 1u)]
        [DataRow("2 1;1;3", 1u, 1u, 3u, 1u)]
        [DataRow("3 1;2;3", 1u, 2u, 3u, 2u)]
        [DataRow("4 1;3;3", 1u, 3u, 3u, 3u)]
        [DataRow("5 1;4;3", 1u, 4u, 3u, 3u)]
        [DataRow("6 10;9;12", 10u, 9u, 12u, 10u)]
        [DataRow("07 10;9;12", 10u, 10u, 12u, 10u)]
        [DataRow("08 10;9;12", 10u, 11u, 12u, 11u)]
        [DataRow("09 10;9;12", 10u, 12u, 12u, 12u)]
        [DataRow("0A 10;9;12", 10u, 13u, 12u, 12u)]
        public void MinMaxTest3(string name, uint dMin, uint dVal, uint dMax, uint dExp)
        {
            Assert.AreEqual(dExp, SMath.MinMax(dMin, dVal, dMax));
        }

        [DataTestMethod()]
        [DataRow("0", 0u, 0u, 0u, 0u)]
        [DataRow("1 1;0;3", 1u, 0u, 3u, 1u)]
        [DataRow("2 1;1;3", 1u, 1u, 3u, 1u)]
        [DataRow("3 1;2;3", 1u, 2u, 3u, 2u)]
        [DataRow("4 1;3;3", 1u, 3u, 3u, 3u)]
        [DataRow("5 1;4;3", 1u, 4u, 3u, 3u)]
        [DataRow("6 10;9;12", 10u, 9u, 12u, 10u)]
        [DataRow("07 10;9;12", 10u, 10u, 12u, 10u)]
        [DataRow("08 10;9;12", 10u, 11u, 12u, 11u)]
        [DataRow("09 10;9;12", 10u, 12u, 12u, 12u)]
        [DataRow("0A 10;9;12", 10u, 13u, 12u, 12u)]
        public void LimitTest(string name, uint dMin, uint dVal, uint dMax, uint dExp)
        {
            Assert.AreEqual(dExp, dVal.Limit(dMin, dMax));
        }

        [DataTestMethod()]
        [DataRow("0,0", 0d, 0d, true, 0d)]
        [DataRow("0,360", 0d, 360d, true, 0d)]
        [DataRow("10,350", 10d, 350d, true, -20d)]
        [DataRow("0,720", 0d, 720d, true, 0d)] //?
        [DataRow("0,1720", 0d, 1720d, true, -80d)] //??
        [DataRow("720,360", 720d, 360d, true, 0d)]
        [DataRow("720,0", 720d, 0d, true, 0d)]
        [DataRow("720,-360", 720d, -360d, true, 0d)]
        [DataRow("100,200", 100d, 200d, true, 100d)]
        [DataRow("720,250", 720d, 250d, true, -110d)]
        [DataRow("1000,-360", 1000d, -360d, true, 80d)] //??
        [DataRow("1000000,-360000", 1000000d, -360000d, true, 80d)] //??
        public void DeltaAngleTest(string name, double dStart, double dStop, bool xExp, double dSExp)
        {
            Assert.AreEqual(dSExp, SMath.DeltaAngle(dStart,dStop), $"Test:{name}");
        }

        [DataTestMethod()]
        [DataRow("0,0", 0d, 0d, true, 0d)]
        [DataRow("0,360", 0d, 360d, true, 0d)]
        [DataRow("10,350", 10d, 350d, true, 340d)]
        [DataRow("0,720", 0d, 720d, true, 0d)] //?
        [DataRow("0,1720", 0d, 1720d, true, 280d)] //??
        [DataRow("720,360", 720d, 360d, true, 0d)]
        [DataRow("720,0", 720d, 0d, true, 0d)]
        [DataRow("720,-360", 720d, -360d, true, 0d)]
        [DataRow("100,200", 100d, 200d, true, 100d)]
        [DataRow("720,250", 720d, 250d, true, 250d)]
        [DataRow("1000,-360", 1000d, -360d, true, 80d)] //??
        [DataRow("1000000,-360000", 1000000d, -360000d, true, 80d)] //??
        public void DeltaAngleClockwiseTest(string name, double dStart, double dStop, bool xExp, double dSExp)
        {
            Assert.AreEqual(dSExp, SMath.DeltaAngleClockwise(dStart, dStop), $"Test:{name}");
        }

        [DataTestMethod()]
        [DataRow("0", 0, 0, 0, true)]
        [DataRow("01 -1;0;1  ", -1, 0, 1, true)]
        [DataRow("02 -1;-2;1 ", -1, -2, 1, false)]
        [DataRow("03 -1;-1;1 ", -1, -1, 1, true)]
        [DataRow("04 -1;1;1  ", -1, 1, 1, true)]
        [DataRow("05 -1;2;1  ", -1, 2, 1, false)]
        [DataRow("06 10;9;12 ", 10, 9, 12, false)]
        [DataRow("07 10;10;12", 10, 10, 12, true)]
        [DataRow("08 10;11;12", 10, 11, 12, true)]
        [DataRow("09 10;12;12", 10, 12, 12, true)]
        [DataRow("0A 10;13;12", 10, 13, 12, false)]
        [DataRow("11 1;0;-1  ", 1, 0, -1, true)]
        [DataRow("12 1;-2;-1 ", 1, -2, -1, false)]
        [DataRow("13 1;-1;-1 ", 1, -1, -1, true)]
        [DataRow("14 1;1;-1  ", 1, 1, -1, true)]
        [DataRow("15 1;2;-1  ", 1, 2, -1, false)]
        [DataRow("16 12;9;10 ", 12, 9, 10, false)]
        [DataRow("17 12;10;10", 12, 10, 10, true)]
        [DataRow("18 12;11;10", 12, 11, 10, true)]
        [DataRow("19 12;12;10", 12, 12, 10, true)]
        [DataRow("1A 12;13;10", 12, 13, 10, false)]
        public void IsBetweenTest(string name, int dMin, int dVal, int dMax, bool xExp)
        {
            Assert.AreEqual(xExp, SMath.IsBetween(dVal, dMin, dMax));
        }

        [DataTestMethod()]
        [DataRow("0", 0u, 0u, 0u, true)]
        [DataRow("01 1;0;3", 1u, 0u, 3u, false)]
        [DataRow("02 1;1;3", 1u, 1u, 3u, true)]
        [DataRow("03 1;2;3", 1u, 2u, 3u, true)]
        [DataRow("04 1;3;3", 1u, 3u, 3u, true)]
        [DataRow("05 1;4;3", 1u, 4u, 3u, false)]
        [DataRow("06 10;9;12", 10u, 9u, 12u, false)]
        [DataRow("07 10;9;12", 10u, 10u, 12u, true)]
        [DataRow("08 10;9;12", 10u, 11u, 12u, true)]
        [DataRow("09 10;9;12", 10u, 12u, 12u, true)]
        [DataRow("0A 10;9;12", 10u, 13u, 12u, false)]
        [DataRow("11 3;0;1", 1u, 0u, 3u, false)]
        [DataRow("12 3;1;1", 1u, 1u, 3u, true)]
        [DataRow("13 3;2;1", 1u, 2u, 3u, true)]
        [DataRow("14 3;3;1", 1u, 3u, 3u, true)]
        [DataRow("15 3;4;1", 1u, 4u, 3u, false)]
        [DataRow("16 12;9;10", 10u, 9u, 12u, false)]
        [DataRow("17 12;9;10", 10u, 10u, 12u, true)]
        [DataRow("18 12;9;10", 10u, 11u, 12u, true)]
        [DataRow("19 12;9;10", 10u, 12u, 12u, true)]
        [DataRow("1A 12;9;10", 10u, 13u, 12u, false)]
        public void IsBetweenTest1(string name, uint dMin, uint dVal, uint dMax, bool xExp)
        {
            Assert.AreEqual(xExp, SMath.IsBetween(dVal, dMin, dMax));
        }

        [DataTestMethod()]
        [DataRow("0", 0u, 0u, 0u, true)]
        [DataRow("01 1;0;3", 1u, 0u, 3u, false)]
        [DataRow("02 1;1;3", 1u, 1u, 3u, true)]
        [DataRow("03 1;2;3", 1u, 2u, 3u, true)]
        [DataRow("04 1;3;3", 1u, 3u, 3u, true)]
        [DataRow("05 1;4;3", 1u, 4u, 3u, false)]
        [DataRow("06 10;9;12", 10u, 9u, 12u, false)]
        [DataRow("07 10;9;12", 10u, 10u, 12u, true)]
        [DataRow("08 10;9;12", 10u, 11u, 12u, true)]
        [DataRow("09 10;9;12", 10u, 12u, 12u, true)]
        [DataRow("0A 10;9;12", 10u, 13u, 12u, false)]
        [DataRow("11 3;0;1", 1u, 0u, 3u, false)]
        [DataRow("12 3;1;1", 1u, 1u, 3u, true)]
        [DataRow("13 3;2;1", 1u, 2u, 3u, true)]
        [DataRow("14 3;3;1", 1u, 3u, 3u, true)]
        [DataRow("15 3;4;1", 1u, 4u, 3u, false)]
        [DataRow("16 12;9;10", 10u, 9u, 12u, false)]
        [DataRow("17 12;9;10", 10u, 10u, 12u, true)]
        [DataRow("18 12;9;10", 10u, 11u, 12u, true)]
        [DataRow("19 12;9;10", 10u, 12u, 12u, true)]
        [DataRow("1A 12;9;10", 10u, 13u, 12u, false)]
        public void IsBetweenTest1a(string name, uint dMin, uint dVal, uint dMax, bool xExp)
        {
            Assert.AreEqual(xExp, SMath.IsBetween((ulong)dVal, (ulong)dMin, (ulong)dMax));
        }


        [DataTestMethod()]
        [DataRow("0", 0, 0, 0, true)]
        [DataRow("01 -1;0;1  ", -1, 0, 1, true)]
        [DataRow("02 -1;-2;1 ", -1, -2, 1, false)]
        [DataRow("03 -1;-1;1 ", -1, -1, 1, true)]
        [DataRow("04 -1;1;1  ", -1, 1, 1, true)]
        [DataRow("05 -1;2;1  ", -1, 2, 1, false)]
        [DataRow("06 10;9;12 ", 10, 9, 12, false)]
        [DataRow("07 10;10;12", 10, 10, 12, true)]
        [DataRow("08 10;11;12", 10, 11, 12, true)]
        [DataRow("09 10;12;12", 10, 12, 12, true)]
        [DataRow("0A 10;13;12", 10, 13, 12, false)]
        [DataRow("11 1;0;-1  ", 1, 0, -1, true)]
        [DataRow("12 1;-2;-1 ", 1, -2, -1, false)]
        [DataRow("13 1;-1;-1 ", 1, -1, -1, true)]
        [DataRow("14 1;1;-1  ", 1, 1, -1, true)]
        [DataRow("15 1;2;-1  ", 1, 2, -1, false)]
        [DataRow("16 12;9;10 ", 12, 9, 10, false)]
        [DataRow("17 12;10;10", 12, 10, 10, true)]
        [DataRow("18 12;11;10", 12, 11, 10, true)]
        [DataRow("19 12;12;10", 12, 12, 10, true)]
        [DataRow("1A 12;13;10", 12, 13, 10, false)]
        public void IsBetweenTest2(string name, int dMin, int dVal, int dMax, bool xExp)
        {
            Assert.AreEqual(xExp, SMath.IsBetween((double)dVal, (double)dMin, (double)dMax));
        }

        [DataTestMethod()]
        [DataRow("0", 0, 0, 0, true)]
        [DataRow("01 -1;0;1  ", -1, 0, 1, true)]
        [DataRow("02 -1;-2;1 ", -1, -2, 1, false)]
        [DataRow("03 -1;-1;1 ", -1, -1, 1, true)]
        [DataRow("04 -1;1;1  ", -1, 1, 1, true)]
        [DataRow("05 -1;2;1  ", -1, 2, 1, false)]
        [DataRow("06 10;9;12 ", 10, 9, 12, false)]
        [DataRow("07 10;10;12", 10, 10, 12, true)]
        [DataRow("08 10;11;12", 10, 11, 12, true)]
        [DataRow("09 10;12;12", 10, 12, 12, true)]
        [DataRow("0A 10;13;12", 10, 13, 12, false)]
        [DataRow("11 1;0;-1  ", 1, 0, -1, true)]
        [DataRow("12 1;-2;-1 ", 1, -2, -1, false)]
        [DataRow("13 1;-1;-1 ", 1, -1, -1, true)]
        [DataRow("14 1;1;-1  ", 1, 1, -1, true)]
        [DataRow("15 1;2;-1  ", 1, 2, -1, false)]
        [DataRow("16 12;9;10 ", 12, 9, 10, false)]
        [DataRow("17 12;10;10", 12, 10, 10, true)]
        [DataRow("18 12;11;10", 12, 11, 10, true)]
        [DataRow("19 12;12;10", 12, 12, 10, true)]
        [DataRow("1A 12;13;10", 12, 13, 10, false)]
        public void IsBetweenTest2a(string name, int dMin, int dVal, int dMax, bool xExp)
        {
            Assert.AreEqual(xExp, SMath.IsBetween((float)dVal, (float)dMin, (float)dMax));
        }

        [DataTestMethod()]
        [DataRow("0", 0, 0, 0, true)]
        [DataRow("01 -1;0;1  ", -1, 0, 1, true)]
        [DataRow("02 -1;-2;1 ", -1, -2, 1, false)]
        [DataRow("03 -1;-1;1 ", -1, -1, 1, true)]
        [DataRow("04 -1;1;1  ", -1, 1, 1, true)]
        [DataRow("05 -1;2;1  ", -1, 2, 1, false)]
        [DataRow("06 10;9;12 ", 10, 9, 12, false)]
        [DataRow("07 10;10;12", 10, 10, 12, true)]
        [DataRow("08 10;11;12", 10, 11, 12, true)]
        [DataRow("09 10;12;12", 10, 12, 12, true)]
        [DataRow("0A 10;13;12", 10, 13, 12, false)]
        [DataRow("11 1;0;-1  ", 1, 0, -1, true)]
        [DataRow("12 1;-2;-1 ", 1, -2, -1, false)]
        [DataRow("13 1;-1;-1 ", 1, -1, -1, true)]
        [DataRow("14 1;1;-1  ", 1, 1, -1, true)]
        [DataRow("15 1;2;-1  ", 1, 2, -1, false)]
        [DataRow("16 12;9;10 ", 12, 9, 10, false)]
        [DataRow("17 12;10;10", 12, 10, 10, true)]
        [DataRow("18 12;11;10", 12, 11, 10, true)]
        [DataRow("19 12;12;10", 12, 12, 10, true)]
        [DataRow("1A 12;13;10", 12, 13, 10, false)]
        public void IsBetweenTest2b(string name, int dMin, int dVal, int dMax, bool xExp)
        {
            Assert.AreEqual(xExp, SMath.IsBetween((decimal)dVal, (decimal)dMin, (decimal)dMax));
        }

        [DataTestMethod()]
        [DataRow("0,0", 0d, 0d, true, 0d, 0d)]
        [DataRow("0,360", 0d, 360d, true, 0d, 0d)]
        [DataRow("10,350", 10d, 350d, true, 10d, 350d)]
        [DataRow("0,720", 0d, 720d, true, 0d, 0d)]
        [DataRow("0,1720", 0d, 1720d, true, 0d, 280d)]
        [DataRow("720,360", 720d, 360d, true, 0d, 0d)]
        [DataRow("720,0", 720d, 0d, true, 0d, 0d)]
        [DataRow("720,-360", 720d, -360d, true, 0d, 0d)]
        [DataRow("100,200", 100d, 200d, true, 100d, 200d)]
        [DataRow("720,250", 720d, 250d, true, 0d, 250d)]
        [DataRow("1000,-360", 1000d, -360d, true, 280d, 0d)]
        [DataRow("1000000,-360000", 1000000d, -360000d, true, 280d, 0d)]
        public void NormalizeAngleTest1(string name, double dStart, double dStop, bool xExp, double dSExp, double dSExp2)
        {
            var fStart = (float)dStart;
            var fStop = (float)dStop;
            Assert.AreEqual(xExp, SMath.NormalizeAngle(ref fStart),  $"Test:{name}.Start");
            Assert.AreEqual(xExp, SMath.NormalizeAngle(ref fStop),  $"Test:{name}.Stop");
            Assert.AreEqual((float)dSExp, fStart, 1e-12, $"Test:{name}.Start");
            Assert.AreEqual((float)dSExp2, fStop, 1e-12, $"Test:{name}.Stop");
        }

        [DataTestMethod()]
        [DataRow("0,0", 0d, 0d, true, 0d, 0d)]
        [DataRow("0,360", 0d, 360d, true, 0d, 0d)]
        [DataRow("10,350", 10d, 350d, true, 10d, 350d)]
        [DataRow("0,720", 0d, 720d, true, 0d, 0d)]
        [DataRow("0,1720", 0d, 1720d, true, 0d, 280d)]
        [DataRow("720,360", 720d, 360d, true, 0d, 0d)]
        [DataRow("720,0", 720d, 0d, true, 0d, 0d)]
        [DataRow("720,-360", 720d, -360d, true, 0d, 0d)]
        [DataRow("100,200", 100d, 200d, true, 100d, 200d)]
        [DataRow("720,250", 720d, 250d, true, 0d, 250d)]
        [DataRow("1000,-360", 1000d, -360d, true, 280d, 0d)]
        [DataRow("1000000,-360000", 1000000d, -360000d, true, 280d, 0d)]
        public void NormalizeAngleTest2(string name, double dStart, double dStop, bool xExp, double dSExp, double dSExp2)
        {
            var iStart = (int)dStart;
            var iStop = (int)dStop;
            Assert.AreEqual(xExp, SMath.NormalizeAngle(ref iStart), $"Test:{name}.Start");
            Assert.AreEqual(xExp, SMath.NormalizeAngle(ref iStop), $"Test:{name}.Stop");
            Assert.AreEqual((int)dSExp, iStart, 1e-12, $"Test:{name}.Start");
            Assert.AreEqual((int)dSExp2, iStop, 1e-12, $"Test:{name}.Stop");
        }


        [DataTestMethod()]
        [DataRow("0,0", 0d, 0d, true, 0d, 0d)]
        [DataRow("0,360", 0d, 360d, true, 0d, 0d)]
        [DataRow("10,350", 10d, 350d, true, 10d, 350d)]
        [DataRow("0,720", 0d, 720d, true, 0d, 0d)]
        [DataRow("0,1720", 0d, 1720d, true, 0d, 280d)]
        [DataRow("720,360", 720d, 360d, true, 0d, 0d)]
        [DataRow("720,0", 720d, 0d, true, 0d, 0d)]
        [DataRow("720,-360", 720d, -360d, true, 0d, 0d)]
        [DataRow("100,200", 100d, 200d, true, 100d, 200d)]
        [DataRow("720,250", 720d, 250d, true, 0d, 250d)]
        [DataRow("1000,-360", 1000d, -360d, true, 280d, 0d)]
        [DataRow("1000000,-360000", 1000000d, -360000d, true, 280d, 0d)]
        public void NormalizeAngleTest3(string name, double dStart, double dStop, bool xExp, double dSExp, double dSExp2)
        {
            Assert.AreEqual(xExp, SMath.NormalizeAngle(ref dStart), $"Test:{name}.Start");
            Assert.AreEqual(xExp, SMath.NormalizeAngle(ref dStop), $"Test:{name}.Stop");
            Assert.AreEqual(dSExp, dStart, 1e-12, $"Test:{name}.Start");
            Assert.AreEqual(dSExp2, dStop, 1e-12, $"Test:{name}.Stop");
        }

        [DataTestMethod()]
        [DataRow("0,0", 0d, 0d, true, 0d, 0d)]
        [DataRow("0,360", 0d, 360d, true, 0d, 0d)]
        [DataRow("10,350", 10d, 350d, true, 10d, 350d)]
        [DataRow("0,720", 0d, 720d, true, 0d, 0d)]
        [DataRow("0,1720", 0d, 1720d, true, 0d, 280d)]
        [DataRow("720,360", 720d, 360d, true, 0d, 0d)]
        [DataRow("720,0", 720d, 0d, true, 0d, 0d)]
        [DataRow("720,-360", 720d, -360d, true, 0d, 0d)]
        [DataRow("100,200", 100d, 200d, true, 100d, 200d)]
        [DataRow("720,250", 720d, 250d, true, 0d, 250d)]
        [DataRow("1000,-360", 1000d, -360d, true, 280d, 0d)]
        [DataRow("1000000,-360000", 1000000d, -360000d, true, 280d, 0d)]
        public void NormalizeAngleTest4(string name, double dStart, double dStop, bool xExp, double dSExp, double dSExp2)
        {
            Assert.AreEqual(dSExp, SMath.NormalizeAngle(dStart),1e-12, $"Test:{name}.Start");
            Assert.AreEqual(dSExp2, SMath.NormalizeAngle(dStop), 1e-12, $"Test:{name}.Stop");
        }

        [DataTestMethod()]
        [DataRow("0,0", 0d, 0d, true, 0d, 0d)]
        [DataRow("0,360", 0d, 360d, true, 0d, 0d)]
        [DataRow("10,350", 10d, 350d, true, 10d, 350d)]
        [DataRow("0,720", 0d, 720d, true, 0d, 0d)]
        [DataRow("0,1720", 0d, 1720d, true, 0d, 280d)]
        [DataRow("720,360", 720d, 360d, true, 0d, 0d)]
        [DataRow("720,0", 720d, 0d, true, 0d, 0d)]
        [DataRow("720,-360", 720d, -360d, true, 0d, 0d)]
        [DataRow("100,200", 100d, 200d, true, 100d, 200d)]
        [DataRow("720,250", 720d, 250d, true, 0d, 250d)]
        [DataRow("1000,-360", 1000d, -360d, true, 280d, 0d)]
        [DataRow("1000000,-360000", 1000000d, -360000d, true, 280d, 0d)]
        public void NormalizeAngleTest4a(string name, double dStart, double dStop, bool xExp, double dSExp, double dSExp2)
        {
            Assert.AreEqual(dSExp, dStart.NormalizeAngle(), 1e-12, $"Test:{name}.Start");
            Assert.AreEqual(dSExp2, dStop.NormalizeAngle(), 1e-12, $"Test:{name}.Stop");
        }

        [DataTestMethod()]
        [DataRow(0, 0)]
        [DataRow(Math.PI / 6, 30)]
        [DataRow(Math.PI / 4, 45)]
        [DataRow(Math.PI / 3, 60)]
        [DataRow(Math.PI / 2, 90)]
        [DataRow(Math.PI / 3 * 2, 120)]
        [DataRow(Math.PI / 4 * 3, 135)]
        [DataRow(Math.PI / 6 * 5, 150)]
        [DataRow(Math.PI, 180)]
        [DataRow(Math.PI / 4 * 5, 225)]
        [DataRow(Math.PI / 2 * 3, 270)]
        [DataRow(Math.PI / 4 * 7, 315)]
        [DataRow(-Math.PI / 6, -30)]
        [DataRow(-Math.PI / 4, -45)]
        [DataRow(-Math.PI / 3, -60)]
        [DataRow(-Math.PI / 2, -90)]
        [DataRow(-Math.PI / 3 * 2, -120)]
        [DataRow(-Math.PI / 4 * 3, -135)]
        [DataRow(-Math.PI / 6 * 5, -150)]
        [DataRow(-Math.PI, -180)]
        public void Rad2DegTest(double dVal, double dExp)
        {
            Assert.AreEqual(dExp, SMath.Rad2Deg(dVal), 1e-13);
        }

        [DataTestMethod()]
        [DataRow(0, 0)]
        [DataRow(Math.PI / 6, 30)]
        [DataRow(Math.PI / 4, 45)]
        [DataRow(Math.PI / 3, 60)]
        [DataRow(Math.PI / 2, 90)]
        [DataRow(Math.PI / 3 * 2, 120)]
        [DataRow(Math.PI / 4 * 3, 135)]
        [DataRow(Math.PI / 6 * 5, 150)]
        [DataRow(Math.PI, 180)]
        [DataRow(Math.PI / 4 * 5, 225)]
        [DataRow(Math.PI / 2 * 3, 270)]
        [DataRow(Math.PI / 4 * 7, 315)]
        [DataRow(-Math.PI / 6, -30)]
        [DataRow(-Math.PI / 4, -45)]
        [DataRow(-Math.PI / 3, -60)]
        [DataRow(-Math.PI / 2, -90)]
        [DataRow(-Math.PI / 3 * 2, -120)]
        [DataRow(-Math.PI / 4 * 3, -135)]
        [DataRow(-Math.PI / 6 * 5, -150)]
        [DataRow(-Math.PI, -180)]
        public void Rad2DegTest1(double fVal, double fExp)
        {
            Assert.AreEqual((float)fExp, SMath.Rad2Deg((float)fVal), 1e-7);
        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d, 0d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue, 0d)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d, 1d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d, double.PositiveInfinity)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity, double.NaN)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity, double.NaN)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity, double.NaN)]
        [DynamicData("StandardDeviationData")]

        public void StandardDeviationTest(string name, double[] ad, bool xExp,double dPred, double dRes)
        {
            var ld = (ad != null) ? new List<double>(ad.ToList()) : null;
            Assert.AreEqual(xExp, SMath.StandardDeviation(out double dAct, ld, dPred), $"Test:{name}");
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, dAct, $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, dAct, 1e-12, $"Test:{name}.result1");
        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d, 0d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue, 0d)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d, 1d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d, double.PositiveInfinity)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity, double.NaN)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity, double.NaN)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity, double.NaN)]
        [DynamicData("StandardDeviationData")]

        public void StandardDeviationTest1(string name, double[] ad, bool xExp, double dPred, double dRes)
        {
            Assert.AreEqual(xExp, SMath.StandardDeviation(out double dAct, ad,dPred), $"Test:{name}");
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, dAct, $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, dAct, 1e-12, $"Test:{name}.result1");
        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d, 0d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue, 0d)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d, 1d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d, double.PositiveInfinity)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity, double.NaN)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity, double.NaN)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity, double.NaN)]
        [DynamicData("StandardDeviationData")]

        public void StandardDeviationTest2(string name, double[] ad, bool xExp, double dPred, double dRes)
        {
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, SMath.StandardDeviation(ad, dPred), $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, SMath.StandardDeviation(ad, dPred), 1e-12, $"Test:{name}.result1");
        }

        [DataTestMethod()]
        [DataRow("Null", null, false, double.NaN, double.NaN)]
        [DataRow("Empty", new double[] { }, false, double.NaN, double.NaN)]
        [DataRow("(0d)", new double[] { 0d }, true, 0d, 0d)]
        [DataRow("(1d)", new double[] { 1d }, true, 1d, 0d)]
        [DataRow("(Max)", new double[] { double.MaxValue }, true, double.MaxValue, 0d)]
        [DataRow("(0d,2d)", new double[] { 0d, 2d }, true, 1d, 1d)]
        [DataRow("(1d,-1d)", new double[] { 1d, -1d }, true, 0d, 1d)]
        [DataRow("(Max,Min)", new double[] { double.MaxValue, double.MinValue }, true, 0d, double.PositiveInfinity)]
        [DataRow("(Inf+,0d)", new double[] { double.PositiveInfinity, 0d }, true, double.PositiveInfinity, double.NaN)]
        [DataRow("(Inf+,Inf+)", new double[] { double.PositiveInfinity, double.PositiveInfinity }, true, double.PositiveInfinity, double.NaN)]
        [DataRow("(Inf+,Min)", new double[] { double.PositiveInfinity, double.MinValue }, true, double.PositiveInfinity, double.NaN)]
        [DynamicData("StandardDeviationData")]

        public void StandardDeviationTest2a(string name, double[] ad, bool xExp, double dPred, double dRes)
        {
            if (double.IsNaN(dRes))
                Assert.AreEqual(dRes, ad.StandardDeviation(dPred), $"Test:{name}.result1");
            else
                Assert.AreEqual(dRes, ad.StandardDeviation(dPred), 1e-12, $"Test:{name}.result1");
        }


        [DataTestMethod()]
        [DataRow("0,0", 0d, 0d, true, 0d, 0d)]
        [DataRow("0,360", 0d, 360d, true, 180d, 360d)]
        [DataRow("10,350", 10d, 350d, true, 190d, 350d)]
        [DataRow("0,720", 0d, 720d, true, 180d, 720d)]
        [DataRow("0,1720", 0d, 1720d, true, 180d, 1720d)]
        [DataRow("720,360", 720d, 360d, true, 720d, 540d)]
        [DataRow("720,0", 720d, 0d, true, 720d, 180d)]
        [DataRow("720,-360", 720d, -360d, true, 720d, -180d)]
        [DataRow("100,200", 100d, 200d, true, 280d, 200d)]
        [DataRow("720,250", 720d, 250d, true, 720d, 430d)]
        [DataRow("1000,-360", 1000d, -360d, true, 1000d, -180d)]
        public void TrimAngle180Test(string name, double dStart, double dStop, bool xExp, double dSExp, double dSExp2)
        {
            Assert.AreEqual(xExp, SMath.TrimAngle180(ref dStart, ref dStop));
            Assert.AreEqual(dSExp, dStart, 1e-12, $"Test:{name}.Start");
            Assert.AreEqual(dSExp2, dStop, 1e-12, $"Test:{name}.Stop");
        }

        [DataTestMethod()]
        [DataRow("0,0", 0d, 0d, true, 0d, 0d)]
        [DataRow("0,360", 0d, 360d, true, 360d, 360d)]
        [DataRow("10,350", 10d, 350d, true, 370d, 350d)]
        [DataRow("0,720", 0d, 720d, true, 360d, 720d)]
        [DataRow("0,1720", 0d, 1720d, true, 360d, 1720d)]
        [DataRow("720,360", 720d, 360d, true, 720d, 720d)]
        [DataRow("720,0", 720d, 0d, true, 720d, 360d)]
        [DataRow("720,-360", 720d, -360d, true, 720d, 0d)]
        [DataRow("100,200", 100d, 200d, true, 100d, 200d)]
        [DataRow("720,250", 720d, 250d, true, 720d, 610d)]
        [DataRow("1000,-360", 1000d, -360d, true, 1000d, 0d)]
        public void TrimAngle360Test(string name, double dStart, double dStop, bool xExp, double dSExp, double dSExp2)
        {
            Assert.AreEqual(xExp, SMath.TrimAngle360(ref dStart, ref dStop));
            Assert.AreEqual(dSExp, dStart, 1e-12, $"Test:{name}.Start");
            Assert.AreEqual(dSExp2, dStop, 1e-12, $"Test:{name}.Stop");
        }

        [DataTestMethod()]
        [DataRow(null, 0)]
        [DataRow(double.NaN, double.NaN)]
        [DataRow(double.MaxValue, double.PositiveInfinity)]
        [DataRow(double.MinValue, double.PositiveInfinity)]
        [DataRow(double.PositiveInfinity, double.PositiveInfinity)]
        [DataRow(double.NegativeInfinity, double.PositiveInfinity)]
        [DataRow(-10, 100)]
        [DataRow(-9, 81)]
        [DataRow(-8, 64)]
        [DataRow(-7, 49)]
        [DataRow(-6, 36)]
        [DataRow(-5, 25)]
        [DataRow(-4, 16)]
        [DataRow(-3, 9)]
        [DataRow(-2, 4)]
        [DataRow(-1, 1)]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 4)]
        [DataRow(3, 9)]
        [DataRow(4, 16)]
        [DataRow(5, 25)]
        [DataRow(6, 36)]
        [DataRow(7, 49)]
        [DataRow(8, 64)]
        [DataRow(9, 81)]
        [DataRow(10, 100)]
        [DynamicData("SqrData")]
        public void SqrTest(double dVal,double dExp)
        {
            if (double.IsNaN(dExp))
            Assert.AreEqual(dExp,SMath.Sqr(dVal));
            else
            Assert.AreEqual(dExp, SMath.Sqr(dVal),1e-13);
        }

		[DataTestMethod()]
		[DataRow(null, 0)]
		[DataRow(double.NaN, double.NaN)]
		[DataRow(double.MaxValue, double.PositiveInfinity)]
		[DataRow(double.MinValue, double.PositiveInfinity)]
		[DataRow(double.PositiveInfinity, double.PositiveInfinity)]
		[DataRow(double.NegativeInfinity, double.PositiveInfinity)]
		[DataRow(-10, 100)]
		[DataRow(-9, 81)]
		[DataRow(-8, 64)]
		[DataRow(-7, 49)]
		[DataRow(-6, 36)]
		[DataRow(-5, 25)]
		[DataRow(-4, 16)]
		[DataRow(-3, 9)]
		[DataRow(-2, 4)]
		[DataRow(-1, 1)]
		[DataRow(0, 0)]
		[DataRow(1, 1)]
		[DataRow(2, 4)]
		[DataRow(3, 9)]
		[DataRow(4, 16)]
		[DataRow(5, 25)]
		[DataRow(6, 36)]
		[DataRow(7, 49)]
		[DataRow(8, 64)]
		[DataRow(9, 81)]
		[DataRow(10, 100)]
		[DynamicData("SqrData")]
		public void SqrTest1(double dVal, double dExp) {
			if (double.IsNaN(dExp))
				Assert.AreEqual(dExp, dVal.Sqr());
			else
				Assert.AreEqual(dExp, dVal.Sqr(), 1e-13);
		}
	}
}
