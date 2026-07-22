using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace MathLibrary.RenderImage.Tests;

/// <summary>
/// Defines tests for <see cref="TFTriple"/>.
/// </summary>
[TestClass]
public class TFTripleTests
{
    /// <summary>
    /// Defines the numeric tolerance used by floating-point assertions.
    /// </summary>
    private const double Epsilon = 1e-12;

    /// <summary>
    /// Verifies that <see cref="TFTriple.Zero"/> exposes zero in all components.
    /// </summary>
    [TestMethod]
    public void ZeroReturnsZeroVector()
    {
        AssertTriple(TFTriple.Zero, 0.0, 0.0, 0.0);
    }

    /// <summary>
    /// Verifies that the indexer reads and writes components by axis index.
    /// </summary>
    [TestMethod]
    public void IndexerGetsAndSetsComponents()
    {
        var vector = TFTriple.Zero;
        vector[0] = 1.25;
        vector[1] = -2.5;
        vector[2] = 3.75;

        Assert.AreEqual(1.25, vector[0], Epsilon);
        Assert.AreEqual(-2.5, vector[1], Epsilon);
        Assert.AreEqual(3.75, vector[2], Epsilon);
        AssertTriple(vector, 1.25, -2.5, 3.75);
    }

    /// <summary>
    /// Verifies that invalid indexer access throws <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [TestMethod]
    public void IndexerThrowsForInvalidIndex()
    {
        var vector = TFTriple.Zero;

        AssertThrowsArgumentOutOfRange(() => _ = vector[-1]);
        AssertThrowsArgumentOutOfRange(() => vector[3] = 1.0);
    }

    /// <summary>
    /// Verifies that <see cref="TFTriple.ToString()"/> uses invariant culture formatting.
    /// </summary>
    [TestMethod]
    public void ToStringUsesInvariantCultureFormatting()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var vector = TFTriple.Copy(1.5, -2.25, 3.75);

            var result = vector.ToString();

            Assert.AreEqual("< 1.50; -2.25; 3.75 >", result);
            Assert.IsFalse(result.Contains(","), "Invariant formatting should use a decimal point instead of a comma.");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// Verifies that <see cref="TFTriple.Init(double, double, double)"/> assigns all components.
    /// </summary>
    [TestMethod]
    public void InitAssignsAllComponents()
    {
        var vector = TFTriple.Zero;

        vector.Init(4.0, -5.0, 6.5);

        AssertTriple(vector, 4.0, -5.0, 6.5);
    }

    /// <summary>
    /// Verifies that <see cref="TFTriple.InitDirLen(double, double, double)"/> converts spherical-style input into Cartesian components.
    /// </summary>
    [TestMethod]
    public void InitDirLenCreatesExpectedVector()
    {
        var vector = TFTriple.Zero;

        vector.InitDirLen(2.0, Math.PI / 2.0, Math.PI / 6.0);

        AssertTriple(vector, 0.0, Math.Sqrt(3.0), 1.0);
    }

    /// <summary>
    /// Verifies that <see cref="TFTriple.InitTuple(in TFTuple, int)"/> maps tuple components to the requested plane.
    /// </summary>
    [TestMethod]
    [DataRow(0, 2.0, 3.0, 2.0, 3.0, 0.0)]
    [DataRow(1, 2.0, 3.0, 0.0, 2.0, 3.0)]
    [DataRow(2, 2.0, 3.0, 3.0, 0.0, 2.0)]
    public void InitTupleMapsTupleToSelectedPlane(int iPlane, double fTupleX, double fTupleY, double fExpectedX, double fExpectedY, double fExpectedZ)
    {
        var tuple = TFTuple.Copy(fTupleX, fTupleY);
        var vector = TFTriple.Zero;

        vector.InitTuple(tuple, iPlane);

        AssertTriple(vector, fExpectedX, fExpectedY, fExpectedZ);
    }

    /// <summary>
    /// Verifies that <see cref="TFTriple.InitTuple(in TFTuple, int)"/> rejects unsupported plane identifiers.
    /// </summary>
    [TestMethod]
    public void InitTupleThrowsForInvalidPlane()
    {
        var tuple = TFTuple.Copy(2.0, 3.0);
        var vector = TFTriple.Zero;

        AssertThrowsArgumentOutOfRange(() => vector.InitTuple(tuple, 3));
    }

    /// <summary>
    /// Verifies that <see cref="TFTriple.Add(in TFTriple)"/> and <see cref="TFTriple.Subt(in TFTriple)"/> return new values without mutating the source.
    /// </summary>
    [TestMethod]
    public void AddAndSubtReturnNewVectors()
    {
        var left = TFTriple.Copy(1.0, 2.0, 3.0);
        var right = TFTriple.Copy(4.0, -5.0, 6.0);

        var sum = left.Add(right);
        var diff = left.Subt(right);

        AssertTriple(sum, 5.0, -3.0, 9.0);
        AssertTriple(diff, -3.0, 7.0, -3.0);
        AssertTriple(left, 1.0, 2.0, 3.0);
    }

    /// <summary>
    /// Verifies that in-place arithmetic methods update the current instance and return the updated value.
    /// </summary>
    [TestMethod]
    public void AddToSubtToAndMulToUpdateCurrentInstance()
    {
        var vector = TFTriple.Copy(1.0, 2.0, 3.0);

        var afterAdd = vector.AddTo(TFTriple.Copy(4.0, -1.0, 2.0));
        AssertTriple(vector, 5.0, 1.0, 5.0);
        AssertTriple(afterAdd, 5.0, 1.0, 5.0);

        var afterSubt = vector.SubtTo(TFTriple.Copy(2.0, 3.0, 4.0));
        AssertTriple(vector, 3.0, -2.0, 1.0);
        AssertTriple(afterSubt, 3.0, -2.0, 1.0);

        var afterMul = vector.MulTo(0.5);
        AssertTriple(vector, 1.5, -1.0, 0.5);
        AssertTriple(afterMul, 1.5, -1.0, 0.5);
    }

    /// <summary>
    /// Verifies dot product, scalar multiplication, and division.
    /// </summary>
    [TestMethod]
    public void MulAndDivideReturnExpectedValues()
    {
        var left = TFTriple.Copy(1.0, 2.0, 3.0);
        var right = TFTriple.Copy(4.0, -5.0, 6.0);

        var dot = left.Mul(right);
        var scaled = left.Mul(2.5);
        var divided = right.Divide(2.0);

        Assert.AreEqual(12.0, dot, Epsilon);
        AssertTriple(scaled, 2.5, 5.0, 7.5);
        AssertTriple(divided, 2.0, -2.5, 3.0);
    }

    /// <summary>
    /// Verifies that <see cref="TFTriple.XMul(in TFTriple)"/> calculates the right-hand cross product.
    /// </summary>
    [TestMethod]
    public void XMulReturnsCrossProduct()
    {
        var xAxis = TFTriple.Copy(1.0, 0.0, 0.0);
        var yAxis = TFTriple.Copy(0.0, 1.0, 0.0);

        var result = xAxis.XMul(yAxis);

        AssertTriple(result, 0.0, 0.0, 1.0);
    }

    /// <summary>
    /// Verifies that <see cref="TFTriple.Equals(in TFTriple, double)"/> uses the supplied tolerance.
    /// </summary>
    [TestMethod]
    public void EqualsUsesAbsoluteTolerance()
    {
        var left = TFTriple.Copy(1.0, 2.0, 3.0);
        var almostEqual = TFTriple.Copy(1.0 + 1e-13, 2.0 - 1e-13, 3.0 + 1e-13);
        var different = TFTriple.Copy(1.0, 2.0, 3.01);

        Assert.IsTrue(left.Equals(almostEqual, 1e-12));
        Assert.IsFalse(left.Equals(different, 1e-12));
    }

    /// <summary>
    /// Verifies that normalization returns a unit-length vector preserving direction.
    /// </summary>
    [TestMethod]
    public void NormalizeReturnsUnitVector()
    {
        var vector = TFTriple.Copy(3.0, 4.0, 0.0);

        var normalized = vector.Normalize();

        AssertTriple(normalized, 0.6, 0.8, 0.0);
        Assert.AreEqual(1.0, normalized.GLen(), Epsilon);
    }

    /// <summary>
    /// Verifies that the copy helpers return equivalent values.
    /// </summary>
    [TestMethod]
    public void CopyMethodsReturnEquivalentValues()
    {
        var original = TFTriple.Copy(7.0, -8.0, 9.5);

        var copiedFromComponents = TFTriple.Copy(7.0, -8.0, 9.5);
        var copiedFromVector = TFTriple.Copy(original);
        var copiedFromInstance = original.Copy();

        AssertTriple(copiedFromComponents, 7.0, -8.0, 9.5);
        AssertTriple(copiedFromVector, 7.0, -8.0, 9.5);
        AssertTriple(copiedFromInstance, 7.0, -8.0, 9.5);
    }

    /// <summary>
    /// Verifies Euclidean length, max norm, squared length, and absolute helper calculations.
    /// </summary>
    [TestMethod]
    public void LengthHelpersReturnExpectedValues()
    {
        var vector = TFTriple.Copy(2.0, -3.0, 6.0);

        Assert.AreEqual(7.0, vector.GLen(), Epsilon);
        Assert.AreEqual(6.0, vector.MLen(), Epsilon);
        Assert.AreEqual(49.0, TFTriple.Sqr(vector), Epsilon);
        Assert.AreEqual(7.0, TFTriple.Abs(vector), Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="TFTriple.GDir()"/> handles vectors on the X axis as special cases.
    /// </summary>
    [TestMethod]
    public void GDirHandlesPositiveAndNegativeXAxis()
    {
        var positive = TFTriple.Copy(5.0, 0.0, 0.0);
        var negative = TFTriple.Copy(-5.0, 0.0, 0.0);

        var positiveDirection = positive.GDir();
        var negativeDirection = negative.GDir();

        Assert.AreEqual(0.0, positiveDirection.X, Epsilon);
        Assert.AreEqual(0.0, positiveDirection.Y, Epsilon);
        Assert.AreEqual(Math.PI, negativeDirection.X, Epsilon);
        Assert.AreEqual(0.0, negativeDirection.Y, Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="TFTriple.GDir()"/> returns angles that reconstruct vectors created by <see cref="TFTriple.InitDirLen(double, double, double)"/>.
    /// </summary>
    [TestMethod]
    public void GDirRoundTripsAnglesFromInitDirLen()
    {
        const double fLength = 3.0;
        const double fDirZ = Math.PI / 3.0;
        const double fDirX = Math.PI / 6.0;

        var vector = TFTriple.Zero;
        vector.InitDirLen(fLength, fDirZ, fDirX);

        var direction = vector.GDir();

        Assert.AreEqual(fDirZ, direction.X, Epsilon);
        Assert.AreEqual(fDirX, direction.Y, Epsilon);
    }

    /// <summary>
    /// Verifies that arithmetic operators delegate to the same mathematical behavior as the named methods.
    /// </summary>
    [TestMethod]
    public void OperatorsReturnExpectedResults()
    {
        var left = TFTriple.Copy(1.0, 2.0, 3.0);
        var right = TFTriple.Copy(4.0, -5.0, 6.0);

        var sum = left + right;
        var diff = left - right;
        var negated = -left;
        var dot = left * right;
        var scaledLeft = left * 2.0;
        var scaledRight = 2.0 * left;
        var divided = right / 2.0;

        AssertTriple(sum, 5.0, -3.0, 9.0);
        AssertTriple(diff, -3.0, 7.0, -3.0);
        AssertTriple(negated, -1.0, -2.0, -3.0);
        Assert.AreEqual(12.0, dot, Epsilon);
        AssertTriple(scaledLeft, 2.0, 4.0, 6.0);
        AssertTriple(scaledRight, 2.0, 4.0, 6.0);
        AssertTriple(divided, 2.0, -2.5, 3.0);
    }

    /// <summary>
    /// Asserts that a vector exposes the expected Cartesian components.
    /// </summary>
    /// <param name="vector">The vector under test.</param>
    /// <param name="fExpectedX">The expected X component.</param>
    /// <param name="fExpectedY">The expected Y component.</param>
    /// <param name="fExpectedZ">The expected Z component.</param>
    private static void AssertTriple(TFTriple vector, double fExpectedX, double fExpectedY, double fExpectedZ)
    {
        Assert.AreEqual(fExpectedX, vector.X, Epsilon);
        Assert.AreEqual(fExpectedY, vector.Y, Epsilon);
        Assert.AreEqual(fExpectedZ, vector.Z, Epsilon);
    }

    /// <summary>
    /// Asserts that the supplied action throws <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    /// <param name="action">The action expected to throw.</param>
    private static void AssertThrowsArgumentOutOfRange(Action action)
    {
        try
        {
            action();
        }
        catch (ArgumentOutOfRangeException)
        {
            return;
        }

        Assert.Fail("Expected ArgumentOutOfRangeException.");
    }
}
