using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Numerics;
using System.Xml;

namespace JCAMS.Core.Math2.Tests
{
    [TestClass()]
    public class SMath2Tests
    {
        protected static IEnumerable<object?[]> IsUndefinedData => new[]
        {
            new object?[]{"null",null,true },
            new object[]{"DBNull",DBNull.Value,true },
            new object[]{ "UndefinedPoint", SMath2.UndefinedPoint,true },
            new object[]{ "UndefinedPointF", SMath2.UndefinedPointF, true },
            new object[]{ "UndefinedRectangle", SMath2.UndefinedRectangle,true },
            new object[]{ "UndefinedRectangleF", SMath2.UndefinedRectangleF,true },
			new object[]{ "1", 1,false },
			new object[]{ "String.Empty", string.Empty,false },
			new object[]{ "0f", 0f,false },
			new object[]{ "25d", 25d,false },
		};

        [TestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(IsUndefinedData))]

        public void IsUndefinedTest(string name,object o, bool xExp)
        {
            Assert.AreEqual(xExp,SMath2.IsUndefined(o));
        }
        [TestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(IsUndefinedData))]
        public void IsUndefinedTest1(string name, object o, bool xExp)
        {
            Assert.AreEqual(xExp, o.IsUndefined());
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Zero", 0f, 0f, 0f)]
        [DataRow("NaN,0", float.NaN, 0f, float.NaN)]
        [DataRow("0,NaN", 0f, float.NaN, float.NaN)]
        public void AbsArcLengthTest(string name, float fDeg, float fRadius, float fExp)
        {
			Assert.AreEqual(fExp, SMath2.AbsArcLength(fDeg, fRadius));
		}

        [TestMethod()]
        [TestProperty("Author", "JC")]
		[DataRow("Zero", 0,new float[] {0,0,0,0})] //??
		[DataRow("1;2", 0, new float[] { 1, 0, 2, 0 })] //??
		[DataRow("1i;2i", 90, new float[] { 0, 1, 0, 2 })] //??
		[DataRow("135°", 135, new float[] { 1, 0, 0, 1 })] //??
		public void AngleTest(string name,float fExp, float[] p)
        {
			PointF pA = new PointF(p[0], p[1]);
			PointF pB = new PointF(p[2], p[3]);
			Assert.AreEqual(fExp,SMath2.Angle(pA,pB));
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Zero",0f,0f,0f)]
        [DataRow("NaN,0", float.NaN, 0f, float.NaN)]
        [DataRow("0,NaN", 0f, float.NaN,  float.NaN)]
        public void ArcLengthTest(string name, float fDeg, float fRadius, float fExp)
        {
            Assert.AreEqual(fExp,SMath2.ArcLength(fDeg,fRadius));
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Zero", 0f, 0f, 0f)]
        [DataRow("NaN,0", float.NaN, 0f, float.NaN)]
        [DataRow("0,NaN", 0f, float.NaN, float.NaN)]
        public void ArcLengthTest2(string name, double dAngle, double dRadius, double dExp)
        {
            Assert.AreEqual(dExp, SMath2.ArcLength(dAngle, dRadius));
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        public void BaseOfPointOrthographicToStraightTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Null",0,0,new int[] {0,0,0,0 })]
        [DataRow("1;-1", 0, 0, new int[] { 1, 1, -1, -1 })]
        [DataRow("Max,Min,Min,Max", -1, -1, new int[] { int.MaxValue, int.MinValue, int.MinValue, int.MaxValue })]
        [DataRow("Max,Max,Max,Max", int.MaxValue, int.MaxValue, new int[] { int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue })]
        [DataRow("Min,Min,Min,Min", int.MinValue, int.MinValue, new int[] { int.MinValue, int.MinValue, int.MinValue, int.MinValue })]
        [DataRow("Min,Min,Min,Min", int.MinValue+1, int.MinValue+1, new int[] { int.MinValue+1, int.MinValue+1, int.MinValue+1, int.MinValue+1 })]
        public void BisectorTest(string name, int x3,int y3, int[] p )
        {

            Point pA = new Point(p[0], p[1]);
            Point pB = new Point(p[2], p[3]);
            Point pExp = new Point(x3, y3);
            Assert.AreEqual(pExp,SMath2.Bisector(pA,pB));
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Null", 0, 0, new float[] { 0, 0, 0, 0 })]
        [DataRow("1;-1", 0, 0, new float[] { 1, 1, -1, -1 })]
        public void MiddlePointTest(string name, float x3, float y3, float[] p)
        {
            PointF pA = new PointF(p[0], p[1]);
            PointF pB = new PointF(p[2], p[3]);
            PointF pExp = new PointF(x3, y3);
            Assert.AreEqual(pExp, SMath2.MiddlePoint(pA, pB));
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Null", 0, 0,0,0, true, new float[] { 0, 0, 0, 0, 0 })]
        [DataRow("1;-1", 1, -1,-1,1,true, new float[] { 1, 1, -1, -1,2 })]
        public void CircleCenterTest(string name, float x3, float y3, float x4, float y4, bool xExp, float[] p)
        {
            PointF pA = new PointF(p[0], p[1]);
            PointF pB = new PointF(p[2], p[3]);
             
            PointF pExp = new PointF(x3, y3);
            PointF pExp2 = new PointF(x4, y4);
            Assert.AreEqual(xExp, SMath2.CircleCenter(pA, pB, p[4],out var p1,out var p2));
            Assert.AreEqual(pExp, p1);
            Assert.AreEqual(pExp2, p2);

        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Null", float.NaN, float.NaN, false, new float[] { 0, 0, 0, 0, 0,0 })]
        [DataRow("1;-1", 1, -1, true, new float[] { 1, 1, -1, -1, 1,-3 })]
        public void CircleCenterTest1(string name, float x3, float y3, bool xExp, float[] p)
        {
            PointF pA = new PointF(p[0], p[1]);
            PointF pB = new PointF(p[2], p[3]);
            PointF pC = new PointF(p[4], p[5]);
            PointF pExp = new PointF(x3, y3);
            Assert.AreEqual(xExp, SMath2.CircleCenter(pA, pB, pC, out var p1, out var r));
            Assert.AreEqual(pExp, p1);
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        public void CircleIntersectionPointTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        public void CircleIntersectionStraightTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        public void DistanceABTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        public void DistanceABTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        public void DistancePointSegmentTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
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
        [TestProperty("Author", "JC")]
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
        [TestProperty("Author", "JC")]
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
        [TestProperty("Author", "JC")]
        [DataRow("Empty", new float[] { 0, 0 }, new[] { 0, 0 })]
        [DataRow("01 <1;2>", new float[] { 1, 2 }, new[] { 1, 2 })]
        public void ToPointTest(string name, float[] afVal, int[] aiExp)
        {
            var pExp = new Point(aiExp[0], aiExp[1]);
            var pVal = new PointF(afVal[0], afVal[1]);
            Assert.AreEqual(pExp, SMath2.ToPoint(pVal));
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Empty", new float[] { 0, 0 }, new[] { 0, 0 })]
        [DataRow("01 <1;2>", new float[] { 1, 2 }, new[] { 1, 2 })]
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
