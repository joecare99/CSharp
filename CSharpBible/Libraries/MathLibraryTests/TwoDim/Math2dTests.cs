// ***********************************************************************
// Assembly         : MathLibrary_netTests
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-11-2022
// ***********************************************************************
// <copyright file="Math2dTests.cs" company="MathLibrary_netTests">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MathLibrary.TwoDim.Tests
{
	/// <summary>
	/// Defines test class Math2dTests.
	/// </summary>
	[TestClass()]
	public class Math2dTests {
		/// <summary>
		/// The SQRT2
		/// </summary>
		const double sqrt2 = 1.41421356237309504880168872420d;
		/// <summary>
		/// The SQRT5
		/// </summary>
		const double sqrt5 = 2.2360679774997896964091736687313d;
		/// <summary>
		/// The atn05
		/// </summary>
		const double atn05 = 0.46364760900080611621425623146121d;

		/// <summary>
		/// Gets the vector angle length test data.
		/// </summary>
		/// <value>The vector angle length test data.</value>
		static IEnumerable<object[]> VectorAngleLengthTestData => new[] {
					new object[] {0, 0, 0, 0 },
					new object[] {1, 0, 1, 0 },
					new object[] {0, 1, 1, Math.PI * 0.5d},
					new object[] {-1, 0, 1, Math.PI},
					new object[] {0, -1, 1, Math.PI * 1.5d},

					new object[] {1, 1, sqrt2, Math.PI * 0.25d},
					new object[] {-1, 1, sqrt2, Math.PI * 0.75d},
					new object[] {-1, -1, sqrt2, Math.PI * 1.25d},
					new object[] {1, -1, sqrt2, Math.PI * 1.75d},

					new object[] {2, 1, sqrt5, atn05},
					new object[] {1, 2, sqrt5, Math.PI * 0.5d - atn05},
					new object[] {-1, 2, sqrt5, Math.PI * 0.5d + atn05},
					new object[] {-2, 1, sqrt5, Math.PI - atn05},
					new object[] {-2, -1, sqrt5, Math.PI + atn05},
					new object[] {-1, -2, sqrt5, Math.PI * 1.5d - atn05},
					new object[] {1, -2, sqrt5, Math.PI * 1.5d + atn05},
					new object[] {2, -1, sqrt5, Math.PI * 2d - atn05}
		};

		/// <summary>
		/// Gets the vector multiply test data.
		/// </summary>
		/// <value>The vector multiply test data.</value>
		static IEnumerable<object[]> VectorMultiplyTestData => new[] {
					new object[] { 0, 0, 0, 0, 0 },
					// Identität	
					new object[] { 0, 0, 1, 0, 0 },
					new object[] { 1, 0, 1, 1, 0 },
					new object[] { 1, 1, 1, 1, 1 },
					new object[] { 0, 1, 1, 0, 1 },
					new object[] {-1, 1, 1,-1, 1 },
					new object[] {-1, 0, 1,-1, 0 },
					new object[] {-1,-1, 1,-1,-1 },
					new object[] { 0,-1, 1, 0,-1 },
					new object[] { 1,-1, 1, 1,-1 },

					// Negation
					new object[] { 0, 0,-1, 0, 0 },
					new object[] { 1, 0,-1,-1, 0 },
					new object[] { 1, 1,-1,-1,-1 },
					new object[] { 0, 1,-1, 0,-1 },
					new object[] {-1, 1,-1, 1,-1 },
					new object[] {-1, 0,-1, 1, 0 },
					new object[] {-1,-1,-1, 1, 1 },
					new object[] { 0,-1,-1, 0, 1 },
					new object[] { 1,-1,-1,-1, 1 },

					// Verdoppelung	
					new object[] { 0, 0, 2, 0, 0 },
					new object[] { 1, 0, 2, 2, 0 },
					new object[] { 1, 1, 2, 2, 2 },
					new object[] { 0, 1, 2, 0, 2 },
					new object[] {-1, 1, 2,-2, 2 },
					new object[] {-1, 0, 2,-2, 0 },
					new object[] {-1,-1, 2,-2,-2 },
					new object[] { 0,-1, 2, 0,-2 },
					new object[] { 1,-1, 2, 2,-2 },

					// Verdoppelung negativ
					new object[] { 0, 0,-2, 0, 0 },
					new object[] { 1, 0,-2,-2, 0 },
					new object[] { 1, 1,-2,-2,-2 },
					new object[] { 0, 1,-2, 0,-2 },
					new object[] {-1, 1,-2, 2,-2 },
					new object[] {-1, 0,-2, 2, 0 },
					new object[] {-1,-1,-2, 2, 2 },
					new object[] { 0,-1,-2, 0, 2 },
					new object[] { 1,-1,-2,-2, 2 },

					// Halbierung	
					new object[] { 0, 0, 0.5, 0, 0 },
					new object[] { 2, 0, 0.5, 1, 0 },
					new object[] { 2, 2, 0.5, 1, 1 },
					new object[] { 0, 2, 0.5, 0, 1 },
					new object[] {-2, 2, 0.5,-1, 1 },
					new object[] {-2, 0, 0.5,-1, 0 },
					new object[] {-2,-2, 0.5,-1,-1 },
					new object[] { 0,-2, 0.5, 0,-1 },
					new object[] { 2,-2, 0.5, 1,-1 },

					// Halbierung negativ
					new object[] { 0, 0,-0.5, 0, 0 },
					new object[] {-2, 0,-0.5, 1, 0 },
					new object[] {-2,-2,-0.5, 1, 1 },
					new object[] { 0,-2,-0.5, 0, 1 },
					new object[] { 2,-2,-0.5,-1, 1 },
					new object[] { 2, 0,-0.5,-1, 0 },
					new object[] { 2, 2,-0.5,-1,-1 },
					new object[] { 0, 2,-0.5, 0,-1 },
					new object[] {-2, 2,-0.5, 1,-1 },

		};

		/// <summary>
		/// Asserts the are equal.
		/// </summary>
		/// <param name="exp">The exp.</param>
		/// <param name="act">The act.</param>
		/// <param name="message">The message.</param>
		private void AssertAreEqual(Math2d.Vector exp, Math2d.Vector act, String message = "") {
			AssertAreEqual(exp, act, 1e-15d, message);
		}
		/// <summary>
		/// Asserts the are equal.
		/// </summary>
		/// <param name="exp">The exp.</param>
		/// <param name="act">The act.</param>
		/// <param name="delta">The delta.</param>
		/// <param name="message">The message.</param>
		private void AssertAreEqual(Math2d.Vector exp, Math2d.Vector act, Double delta, String message = "") {
			Assert.AreEqual(exp.x, act.x, delta, message + " [x]");
			Assert.AreEqual(exp.y, act.y, delta, message + " [y]");
		}

		/// <summary>
		/// Defines the test method ToStringTest1.
		/// </summary>
		[TestMethod()]
		public void ToStringTest1() {
			Assert.AreEqual("( 0, 0)", Math2d.Null.ToString(), "Null.ToString()");
			Assert.AreEqual("( 1, 0)", Math2d.eX.ToString(), "eX.ToString()");
			Assert.AreEqual("( 0, 1)", Math2d.eY.ToString(), "eY.ToString()");
		}

		/// <summary>
		/// Converts to stringtest2.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <param name="exp">The exp.</param>
		[DataTestMethod()]
		[DataRow(0, 0, "( 0, 0)")]
		[DataRow(0, 1, "( 0, 1)")]
		[DataRow(1, 0, "( 1, 0)")]
		[DataRow(-1, 0, "( -1, 0)")]
		[DataRow(0, -1, "( 0, -1)")]
		[DataRow(1, -1, "( 1, -1)")]
		public void ToStringTest2(Double x, Double y, String exp) {
			var v1 = new Math2d.Vector(x, y);
			Assert.AreEqual(exp, v1.ToString(), $"({x},{y}).ToString()");
		}

		/// <summary>
		/// Defines the test method NullTest.
		/// </summary>
		[TestMethod()]
		public void NullTest() {
			var exp = new Math2d.Vector(0, 0);
			AssertAreEqual(exp, Math2d.Null, $"{exp} = Null");
		}

		/// <summary>
		/// Defines the test method eXTest.
		/// </summary>
		[TestMethod()]
		public void eXTest() {
			var exp = new Math2d.Vector(1, 0);
			AssertAreEqual(exp, Math2d.eX, $"{exp} = eX");
		}
		/// <summary>
		/// Defines the test method eYTest.
		/// </summary>
		[TestMethod()]
		public void eYTest() {
			var exp = new Math2d.Vector(0, 1);
			AssertAreEqual(exp, Math2d.eY, $"{exp} = eY");
		}

		/// <summary>
		/// Adds the test.
		/// </summary>
		/// <param name="v1x">The V1X.</param>
		/// <param name="v1y">The v1y.</param>
		/// <param name="v2x">The V2X.</param>
		/// <param name="v2y">The v2y.</param>
		/// <param name="expx">The expx.</param>
		/// <param name="expy">The expy.</param>
		[DataTestMethod()]
		[DataRow(0, 0, 0, 0, 0, 0)]
		[DataRow(0, 1, 0, 0, 0, 1)]
		[DataRow(1, 0, 0, 0, 1, 0)]
		[DataRow(0, 0, 0, 1, 0, 1)]
		[DataRow(0, 0, 1, 0, 1, 0)]
		[DataRow(1, 0, 1, 0, 2, 0)]
		[DataRow(0, 1, 0, 1, 0, 2)]
		[DataRow(1, 0, 0, 1, 1, 1)]
		[DataRow(1, 0, -1, 0, 0, 0)]
		[DataRow(0, 1, 0, -1, 0, 0)]
		[DataRow(-1, 0, 1, 0, 0, 0)]
		[DataRow(0, -1, 0, 1, 0, 0)]
		[DataRow(1, -1, -1, 1, 0, 0)]
		public void AddTest(double v1x, double v1y, double v2x, double v2y, double expx, double expy) {
			var v1 = new Math2d.Vector(v1x, v1y);
			var v2 = new Math2d.Vector(v2x, v2y);
			var exp = new Math2d.Vector(expx, expy);
			AssertAreEqual(exp, Math2d.Add(v1, v2), 1e-15d, $"{exp} = Add({v1},{v2})");
		}

		/// <summary>
		/// Subtracts the test.
		/// </summary>
		/// <param name="v1x">The V1X.</param>
		/// <param name="v1y">The v1y.</param>
		/// <param name="v2x">The V2X.</param>
		/// <param name="v2y">The v2y.</param>
		/// <param name="expx">The expx.</param>
		/// <param name="expy">The expy.</param>
		[DataTestMethod()]
		[DataRow(0, 0, 0, 0, 0, 0)]
		[DataRow(0, 1, 0, 0, 0, 1)]
		[DataRow(1, 0, 0, 0, 1, 0)]
		[DataRow(0, 0, 0, 1, 0, -1)]
		[DataRow(0, 0, 1, 0, -1, 0)]
		[DataRow(1, 0, 1, 0, 0, 0)]
		[DataRow(0, 1, 0, 1, 0, 0)]
		[DataRow(1, 0, 0, 1, 1, -1)]
		[DataRow(1, 0, -1, 0, 2, 0)]
		[DataRow(0, 1, 0, -1, 0, 2)]
		[DataRow(-1, 0, 1, 0, -2, 0)]
		[DataRow(0, -1, 0, 1, 0, -2)]
		[DataRow(1, -1, -1, 1, 2, -2)]
		public void SubtractTest(double v1x, double v1y, double v2x, double v2y, double expx, double expy) {
			var v1 = new Math2d.Vector(v1x, v1y);
			var v2 = new Math2d.Vector(v2x, v2y);
			var exp = new Math2d.Vector(expx, expy);
			AssertAreEqual(exp, Math2d.Subtract(v1, v2), 1e-15d, $"{exp} = Subtract({v1},{v2})");
		}

		/// <summary>
		/// Winkels the norm test.
		/// </summary>
		/// <param name="angle">The angle.</param>
		/// <param name="middle">The middle.</param>
		/// <param name="exp">The exp.</param>
		[DataTestMethod()]
		[DataRow(0, 0, 0)]
		[DataRow(0, Double.NaN, 0)]
		[DataRow(Math.PI, 0, -Math.PI)]
		[DataRow(Math.PI, Double.NaN, Math.PI)]
		[DataRow(-Math.PI, 0, -Math.PI)]
		[DataRow(-Math.PI, Double.NaN, Math.PI)]
		[DataRow(2 * Math.PI, 0, 0)]
		[DataRow(2 * Math.PI, Double.NaN, 0)]
		[DataRow(2 * -Math.PI, 0, 0)]
		[DataRow(2 * -Math.PI, Double.NaN, 0)]
		[DataRow(3 * Math.PI, 0, -Math.PI)]
		[DataRow(3 * Math.PI, Double.NaN, Math.PI)]
		[DataRow(3 * -Math.PI, 0, -Math.PI)]
		[DataRow(3 * -Math.PI, Double.NaN, Math.PI)]
		public void WinkelNormTest(Double angle, Double middle, Double exp) {
			if (Double.IsNaN(middle))
				Assert.AreEqual(exp, Math2d.WinkelNorm(angle), 1e-15d, $"{exp} = WinkelNorm({angle},{middle})");
			else
				Assert.AreEqual(exp, Math2d.WinkelNorm(angle, middle), 1e-15d, $"{exp} = WinkelNorm({angle},{middle})");
		}

		/// <summary>
		/// Rot90s the test.
		/// </summary>
		/// <param name="v_x">The v x.</param>
		/// <param name="v_y">The v y.</param>
		/// <param name="exp_x">The exp x.</param>
		/// <param name="exp_y">The exp y.</param>
		[DataTestMethod()]
		[DataRow(0, 0, 0, 0)]

		[DataRow(0, 1, -1, 0)]
		[DataRow(1, 0, 0, 1)]
		[DataRow(0, -1, 1, 0)]
		[DataRow(-1, 0, 0, -1)]

		[DataRow(1, 1, -1, 1)]
		[DataRow(1, -1, 1, 1)]
		[DataRow(-1, -1, 1, -1)]
		[DataRow(-1, 1, -1, -1)]

		[DataRow(1, 2, -2, 1)]
		[DataRow(-1, 2, -2, -1)]
		[DataRow(2, 1, -1, 2)]
		[DataRow(2, -1, 1, 2)]
		public void Rot90Test(double v_x, double v_y, double exp_x, double exp_y) {
			var v = new Math2d.Vector(v_x, v_y);
			var exp = new Math2d.Vector(exp_x, exp_y);
			AssertAreEqual(exp, Math2d.Rot90(v), $"{exp} = Rot90({v})");
		}

		/// <summary>
		/// Negates the test.
		/// </summary>
		/// <param name="v_x">The v x.</param>
		/// <param name="v_y">The v y.</param>
		/// <param name="exp_x">The exp x.</param>
		/// <param name="exp_y">The exp y.</param>
		[DataTestMethod()]
		[DataRow(0, 0, 0, 0)]

		[DataRow(0, 1, 0, -1)]
		[DataRow(1, 0, -1, 0)]
		[DataRow(0, -1, 0, 1)]
		[DataRow(-1, 0, 1, 0)]

		[DataRow(1, 1, -1, -1)]
		[DataRow(1, -1, -1, 1)]
		[DataRow(-1, -1, 1, 1)]
		[DataRow(-1, 1, 1, -1)]

		[DataRow(1, 2, -1, -2)]
		[DataRow(-1, 2, 1, -2)]
		[DataRow(2, 1, -2, -1)]
		[DataRow(2, -1, -2, 1)]
		public void NegateTest(double v_x, double v_y, double exp_x, double exp_y) {
			var v = new Math2d.Vector(v_x, v_y);
			var exp = new Math2d.Vector(exp_x, exp_y);
			AssertAreEqual(exp, Math2d.Negate(v), $"{exp} = Negate({v})");
			AssertAreEqual(v, Math2d.Negate(exp), $"{v} = Negate({exp})");
		}

		/// <summary>
		/// Tries the length angle test.
		/// </summary>
		/// <param name="v_x">The v x.</param>
		/// <param name="v_y">The v y.</param>
		/// <param name="exp">if set to <c>true</c> [exp].</param>
		/// <param name="exlength">The exlength.</param>
		/// <param name="exangle">The exangle.</param>
		[DataTestMethod()]
		[DataRow(0, 0, false, 0, 0)]

		[DataRow(1, 0, true, 1, 0)]
		[DataRow(0, 1, true, 1, Math.PI * 0.5d)]
		[DataRow(-1, 0, true, 1, Math.PI)]
		[DataRow(0, -1, true, 1, Math.PI * 1.5d)]

		[DataRow(1, 1, true, sqrt2, Math.PI * 0.25d)]
		[DataRow(-1, 1, true, sqrt2, Math.PI * 0.75d)]
		[DataRow(-1, -1, true, sqrt2, Math.PI * 1.25d)]
		[DataRow(1, -1, true, sqrt2, Math.PI * 1.75d)]

		[DataRow(2, 1, true, sqrt5, atn05)]
		[DataRow(1, 2, true, sqrt5, Math.PI * 0.5d - atn05)]
		[DataRow(-1, 2, true, sqrt5, Math.PI * 0.5d + atn05)]
		[DataRow(-2, 1, true, sqrt5, Math.PI - atn05)]
		[DataRow(-2, -1, true, sqrt5, Math.PI + atn05)]
		[DataRow(-1, -2, true, sqrt5, Math.PI * 1.5d - atn05)]
		[DataRow(1, -2, true, sqrt5, Math.PI * 1.5d + atn05)]
		[DataRow(2, -1, true, sqrt5, Math.PI * 2d - atn05)]
		public void TryLengthAngleTest(double v_x, double v_y, bool exp, double exlength, double exangle) {
			var v = new Math2d.Vector(v_x, v_y);
			double length = 0;
			double angle = 0;
			Assert.AreEqual(exp, Math2d.TryLengthAngle(v, out length, out angle), $"{exp} = TryLengthAngle({v})");
			Assert.AreEqual(exlength, length, $"Length: {exlength} = {length}");
			Assert.AreEqual(exangle, angle, $"Angle: {exangle} = {angle}");
		}

		/// <summary>
		/// Bies the length angle test.
		/// </summary>
		/// <param name="exp_x">The exp x.</param>
		/// <param name="exv_y">The exv y.</param>
		/// <param name="length">The length.</param>
		/// <param name="angle">The angle.</param>
		[DataTestMethod()]
		[DataRow(0, 0, 0, 0)]

		[DataRow(1, 0, 1, 0)]
		[DataRow(0, 1, 1, Math.PI * 0.5d)]
		[DataRow(-1, 0, 1, Math.PI)]
		[DataRow(0, -1, 1, Math.PI * 1.5d)]

		[DataRow(1, 1, sqrt2, Math.PI * 0.25d)]
		[DataRow(-1, 1, sqrt2, Math.PI * 0.75d)]
		[DataRow(-1, -1, sqrt2, Math.PI * 1.25d)]
		[DataRow(1, -1, sqrt2, Math.PI * 1.75d)]

		[DataRow(2, 1, sqrt5, atn05)]
		[DataRow(1, 2, sqrt5, Math.PI * 0.5d - atn05)]
		[DataRow(-1, 2, sqrt5, Math.PI * 0.5d + atn05)]
		[DataRow(-2, 1, sqrt5, Math.PI - atn05)]
		[DataRow(-2, -1, sqrt5, Math.PI + atn05)]
		[DataRow(-1, -2, sqrt5, Math.PI * 1.5d - atn05)]
		[DataRow(1, -2, sqrt5, Math.PI * 1.5d + atn05)]
		[DataRow(2, -1, sqrt5, Math.PI * 2d - atn05)]
		[DynamicData("VectorAngleLengthTestData")]
		public void ByLengthAngleTest(double exp_x, double exv_y, double length, double angle) {
			var exp = new Math2d.Vector(exp_x, exv_y);
			AssertAreEqual(exp, Math2d.ByLengthAngle(length, angle), $"{exp} = Math2d.ByLengthAngle({length},{angle})");
		}

		/// <summary>
		/// Defines the test method MultTest.
		/// </summary>
		/// <param name="v_x">The v x.</param>
		/// <param name="v_y">The v y.</param>
		/// <param name="s">The s.</param>
		/// <param name="exp_x">The exp x.</param>
		/// <param name="exp_y">The exp y.</param>
		[TestMethod()]
		[DynamicData("VectorMultiplyTestData")]

		public void MultTest(double v_x, double v_y, double s, double exp_x, double exp_y) {
			var v = new Math2d.Vector(v_x, v_y);
			var exp = new Math2d.Vector(exp_x, exp_y);
			AssertAreEqual(exp, Math2d.Mult(v, s), $"{exp} = Math2d.Mult({v},{s})");
			if (s != 0.0)
				AssertAreEqual(v, Math2d.Mult(exp, 1 / s), $"{v} = Math2d.Mult({exp},{1 / s})");
		}

		/// <summary>
		/// Mults the test1.
		/// </summary>
		/// <param name="v1_x">The v1 x.</param>
		/// <param name="v1_y">The v1 y.</param>
		/// <param name="v2_x">The v2 x.</param>
		/// <param name="v2_y">The v2 y.</param>
		/// <param name="exp">The exp.</param>
		[DataTestMethod()]
		[DataRow(0, 0, 0, 0, 0)]

		[DataRow(0, 1, 1, 0, 0)]
		[DataRow(1, 1, 1, 0, 1)]
		[DataRow(1, 0, 1, 0, 1)]
		[DataRow(1, -1, 1, 0, 1)]
		[DataRow(0, -1, 1, 0, 0)]
		[DataRow(-1, -1, 1, 0, -1)]
		[DataRow(-1, 0, 1, 0, -1)]
		[DataRow(-1, 1, 1, 0, -1)]

		[DataRow(0, 1, 1, 1, 1)]
		[DataRow(1, 1, 1, 1, 2)]
		[DataRow(1, 0, 1, 1, 1)]
		[DataRow(1, -1, 1, 1, 0)]
		[DataRow(0, -1, 1, 1, -1)]
		[DataRow(-1, -1, 1, 1, -2)]
		[DataRow(-1, 0, 1, 1, -1)]
		[DataRow(-1, 1, 1, 1, 0)]

		[DataRow(0, 1, 0, 1, 1)]
		[DataRow(1, 1, 0, 1, 1)]
		[DataRow(1, 0, 0, 1, 0)]
		[DataRow(1, -1, 0, 1, -1)]
		[DataRow(0, -1, 0, 1, -1)]
		[DataRow(-1, -1, 0, 1, -1)]
		[DataRow(-1, 0, 0, 1, 0)]
		[DataRow(-1, 1, 0, 1, 1)]

		[DataRow(0, 1, -1, 1, 1)]
		[DataRow(1, 1, -1, 1, 0)]
		[DataRow(1, 0, -1, 1, -1)]
		[DataRow(1, -1, -1, 1, -2)]
		[DataRow(0, -1, -1, 1, -1)]
		[DataRow(-1, -1, -1, 1, 0)]
		[DataRow(-1, 0, -1, 1, 1)]
		[DataRow(-1, 1, -1, 1, 2)]

		[DataRow(2, 1, -2, -1, -5)]
		[DataRow(1, 2, -2, -1, -4)]
		[DataRow(-1, 2, -2, 1, 4)]
		[DataRow(-2, 1, -2, 1, 5)]
		[DataRow(-2, -1, -1, 2, 0)]
		[DataRow(-1, -2, -1, 2, -3)]
		[DataRow(1, -2, 1, 2, -3)]
		[DataRow(2, -1, 1, 2, 0)]
		public void MultTest1(double v1_x, double v1_y, double v2_x, double v2_y, double exp) {
			var v1 = new Math2d.Vector(v1_x, v1_y);
			var v2 = new Math2d.Vector(v2_x, v2_y);
			Assert.AreEqual(exp, Math2d.Mult(v1, v2), $"{exp} = Math2d.Mult({v1},{v2})");
			Assert.AreEqual(exp, Math2d.Mult(v2, v1), $"{exp} = Math2d.Mult({v2},{v1})");
		}

		/// <summary>
		/// cs the mult test.
		/// </summary>
		/// <param name="v1_x">The v1 x.</param>
		/// <param name="v1_y">The v1 y.</param>
		/// <param name="v2_x">The v2 x.</param>
		/// <param name="v2_y">The v2 y.</param>
		/// <param name="exp_x">The exp x.</param>
		/// <param name="exp_y">The exp y.</param>
		[DataTestMethod()]
		[DataRow(0, 0, 0, 0, 0, 0)]

		[DataRow(0, 1, 1, 0, 0, 1)]
		[DataRow(1, 1, 1, 0, 1, 1)]
		[DataRow(1, 0, 1, 0, 1, 0)]
		[DataRow(1, -1, 1, 0, 1, -1)]
		[DataRow(0, -1, 1, 0, 0, -1)]
		[DataRow(-1, -1, 1, 0, -1, -1)]
		[DataRow(-1, 0, 1, 0, -1, 0)]
		[DataRow(-1, 1, 1, 0, -1, 1)]

		[DataRow(0, 1, 1, 1, -1, 1)]
		[DataRow(1, 1, 1, 1, 0, 2)]
		[DataRow(1, 0, 1, 1, 1, 1)]
		[DataRow(1, -1, 1, 1, 2, 0)]
		[DataRow(0, -1, 1, 1, 1, -1)]
		[DataRow(-1, -1, 1, 1, 0, -2)]
		[DataRow(-1, 0, 1, 1, -1, -1)]
		[DataRow(-1, 1, 1, 1, -2, 0)]

		[DataRow(0, 1, 0, 1, -1, 0)]
		[DataRow(1, 1, 0, 1, -1, 1)]
		[DataRow(1, 0, 0, 1, 0, 1)]
		[DataRow(1, -1, 0, 1, 1, 1)]
		[DataRow(0, -1, 0, 1, 1, 0)]
		[DataRow(-1, -1, 0, 1, 1, -1)]
		[DataRow(-1, 0, 0, 1, 0, -1)]
		[DataRow(-1, 1, 0, 1, -1, -1)]

		[DataRow(0, 1, -1, 1, -1, -1)]
		[DataRow(1, 1, -1, 1, -2, 0)]
		[DataRow(1, 0, -1, 1, -1, 1)]
		[DataRow(1, -1, -1, 1, 0, 2)]
		[DataRow(0, -1, -1, 1, 1, 1)]
		[DataRow(-1, -1, -1, 1, 2, 0)]
		[DataRow(-1, 0, -1, 1, 1, -1)]
		[DataRow(-1, 1, -1, 1, 0, -2)]

		[DataRow(2, 1, -2, -1, -3, -4)]
		[DataRow(1, 2, -2, -1, 0, -5)]
		[DataRow(-1, 2, -2, 1, 0, -5)]
		[DataRow(-2, 1, -2, 1, 3, -4)]
		[DataRow(-2, -1, -1, 2, 4, -3)]
		[DataRow(-1, -2, -1, 2, 5, 0)]
		[DataRow(1, -2, 1, 2, 5, 0)]
		[DataRow(2, -1, 1, 2, 4, 3)]
		public void CMultTest(double v1_x, double v1_y, double v2_x, double v2_y, double exp_x, double exp_y) {
			var v1 = new Math2d.Vector(v1_x, v1_y);
			var v2 = new Math2d.Vector(v2_x, v2_y);
			var exp = new Math2d.Vector(exp_x, exp_y);
			AssertAreEqual(exp, Math2d.CMult(v1, v2), $"{exp} = Math2d.CMult({v1},{v2})");
			AssertAreEqual(exp, Math2d.CMult(v2, v1), $"{exp} = Math2d.CMult({v2},{v1})");
		}

		/// <summary>
		/// Divs the test.
		/// </summary>
		/// <param name="exp_x">The exp x.</param>
		/// <param name="exp_y">The exp y.</param>
		/// <param name="s">The s.</param>
		/// <param name="v_x">The v x.</param>
		/// <param name="v_y">The v y.</param>
		[DataTestMethod()]
		[DynamicData("VectorMultiplyTestData")]
		public void DivTest(double exp_x, double exp_y, double s, double v_x, double v_y) {
			if (s == 0.0) return; // filter
			var v = new Math2d.Vector(v_x, v_y);
			var exp = new Math2d.Vector(exp_x, exp_y);
			AssertAreEqual(exp, Math2d.Div(v, s), $"{exp} = Math2d.Div({v},{s})");
			AssertAreEqual(v, Math2d.Div(exp, 1 / s), $"{v} = Math2d.Div({exp},{1 / s})");
		}

		/// <summary>
		/// Rotates the test.
		/// </summary>
		/// <param name="v_x">The v x.</param>
		/// <param name="v_y">The v y.</param>
		/// <param name="w">The w.</param>
		/// <param name="exp_x">The exp x.</param>
		/// <param name="exp_y">The exp y.</param>
		[DataTestMethod()]
		// Null
		[DataRow(0, 0, 0, 0, 0)]
		[DataRow(0, 0, 1, 0, 0)]

		// 0-Winkel (neutral)
		[DataRow(0, 1, 0, 0, 1)]
		[DataRow(1, 1, 0, 1, 1)]
		[DataRow(1, 0, 0, 1, 0)]
		[DataRow(1, -1, 0, 1, -1)]
		[DataRow(0, -1, 0, 0, -1)]
		[DataRow(-1, -1, 0, -1, -1)]
		[DataRow(-1, 0, 0, -1, 0)]
		[DataRow(-1, 1, 0, -1, 1)]
		// 90°-Winkel (well known)
		[DataRow(0, 1, 0.5 * Math.PI, -1, 0)]
		[DataRow(1, 1, 0.5 * Math.PI, -1, 1)]
		[DataRow(1, 0, 0.5 * Math.PI, 0, 1)]
		[DataRow(1, -1, 0.5 * Math.PI, 1, 1)]
		[DataRow(0, -1, 0.5 * Math.PI, 1, 0)]
		[DataRow(-1, -1, 0.5 * Math.PI, 1, -1)]
		[DataRow(-1, 0, 0.5 * Math.PI, 0, -1)]
		[DataRow(-1, 1, 0.5 * Math.PI, -1, -1)]
		public void RotateTest(double v_x, double v_y, double w, double exp_x, double exp_y) {
			var v = new Math2d.Vector(v_x, v_y);
			var exp = new Math2d.Vector(exp_x, exp_y);
			AssertAreEqual(exp, Math2d.Rotate(v, w), $"{exp} = Math2d.Rotate({v},{w})");
			AssertAreEqual(v, Math2d.Rotate(exp, -w), $"{v} = Math2d.Rotate({exp},{-w})");
		}
	}
}
