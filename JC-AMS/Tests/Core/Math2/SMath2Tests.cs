using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Numerics;

namespace JCAMS.Core.Math2.Tests
{
    [TestClass()]
    public class SMath2Tests
    {
        [TestMethod()]
        public void IsUndefinedTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        public void AbsArcLengthTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AngleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ArcLengthTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BaseOfPointOrthographicToStraightTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BisectorTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BisectorTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CircleCenterTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CircleCenterTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CircleIntersectionPointTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CircleIntersectionStraightTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DistanceABTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DistanceABTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DistancePointSegmentTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DistancePointStraightTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IntersectionLineSegmentLineSegmentTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IntersectionLineSegmentLineSegmentTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void OffsetTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [DataRow("Empty", new[] { 0, 0 }, new float[] { 0, 0 })]
        [DataRow("01 <1;2>", new[] { 1, 2 }, new float[] { 1, 2 })]
        [DataRow("02 <Max;Max>", new[] { int.MaxValue, int.MaxValue }, new float[] { int.MaxValue, int.MaxValue })]
        [DataRow("03 <Min;Min>", new[] { int.MinValue, int.MinValue }, new float[] { int.MinValue, int.MinValue })]
        public void ToPointFTest(string name, int[] aiVal, float[] afExp)
        {
            var pExp = new PointF(afExp[0], afExp[1]);
            var pVal = new Point(aiVal[0], aiVal[1]);
            Assert.AreEqual(pExp, SMath2.ToPointF(pVal));
        }

        [DataTestMethod()]
        [DataRow("Empty", new[] { 0, 0 }, new float[] { 0, 0 })]
        [DataRow("01 <1;2>", new[] { 1, 2 }, new float[] { 1, 2 })]
        [DataRow("02 <Max;Max>", new[] { int.MaxValue, int.MaxValue }, new float[] { int.MaxValue, int.MaxValue })]
        [DataRow("03 <Min;Min>", new[] { int.MinValue, int.MinValue }, new float[] { int.MinValue, int.MinValue })]
        public void ToPointFTest1(string name, int[] aiVal, float[] afExp)
        {
            var pExp = new PointF(afExp[0], afExp[1]);
            var pVal = new Point(aiVal[0], aiVal[1]);
            Assert.AreEqual(pExp, pVal.ToPointF());
        }

        [DataTestMethod()]
        public void ToPointTest(string name, float[] afVal, int[] aiExp)
        {
            var pExp = new Point(aiExp[0], aiExp[1]);
            var pVal = new PointF(afVal[0], afVal[1]);
            Assert.AreEqual(pExp, SMath2.ToPoint(pVal));
        }

        [DataTestMethod()]
        public void ToPointTest1(string name, float[] afVal, int[] aiExp)
        {
            var pExp = new Point(aiExp[0], aiExp[1]);
            var pVal = new PointF(afVal[0], afVal[1]);
            Assert.AreEqual(pExp, pVal.ToPoint());
        }

        [TestMethod()]
        public void PointVectorToPointTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RotateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RotateTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RotateTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RotatePointTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void StretchTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void StretchTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToArrayTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [DataRow(0,0,new float[] {0,0 },DisplayName ="Zero")]
        [DataRow(0, 1, new float[] { 1, 0f }, DisplayName = "0°;1")]
        [DataRow(Math.PI / 6, 0, new float[] { 0, 0f }, DisplayName = "30°;0")]
        [DataRow(Math.PI/6, 1, new float[] { 0.8660254f, 0.5f }, DisplayName = "30°;1")]
        [DataRow(Math.PI / 2, 2, new float[] { 0, 2 }, DisplayName = "90°,2")]
        [DataRow(Math.PI, 3, new float[] { -3, 0 }, DisplayName = "180°,3")]
        public void Vector2alTest(double dAng,double dVal, float[] afExp)
        {
            var vExp = new Vector2(afExp[0], afExp[1]);
            Assert.AreEqual(vExp,SMath2.Vector2al(dAng,dVal));
        }
    }
}