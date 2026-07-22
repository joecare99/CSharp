using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace MathLibrary.RenderImage.Tests;

/// <summary>
/// Defines tests for <see cref="TFTuple"/>.
/// </summary>
[TestClass]
public class TFTupleTests
{
    /// <summary>
    /// Defines the numeric tolerance used by floating-point assertions.
    /// </summary>
    private const double Epsilon = 1e-12;

    /// <summary>
    /// Verifies that <see cref="TFTuple.Zero"/> exposes zero in both components.
    /// </summary>
    [TestMethod]
    public void ZeroReturnsZeroTuple()
    {
        AssertTuple(TFTuple.Zero, 0.0, 0.0);
    }

    /// <summary>
    /// Verifies that the indexer reads and writes components by axis index.
    /// </summary>
    [TestMethod]
    public void IndexerGetsAndSetsComponents()
    {
        var tuple = TFTuple.Zero;
        tuple[0] = 1.25;
        tuple[1] = -2.5;

        Assert.AreEqual(1.25, tuple[0], Epsilon);
        Assert.AreEqual(-2.5, tuple[1], Epsilon);
        AssertTuple(tuple, 1.25, -2.5);
    }

    /// <summary>
    /// Verifies that invalid indexer access throws <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [TestMethod]
    public void IndexerThrowsForInvalidIndex()
    {
        var tuple = TFTuple.Zero;

        AssertThrowsArgumentOutOfRange(() => _ = tuple[-1]);
        AssertThrowsArgumentOutOfRange(() => tuple[2] = 1.0);
    }

    /// <summary>
    /// Verifies that <see cref="TFTuple.ToString()"/> uses invariant culture formatting.
    /// </summary>
    [TestMethod]
    public void ToStringUsesInvariantCultureFormatting()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var tuple = TFTuple.Copy(1.5, -2.25);

            var result = tuple.ToString();

            Assert.AreEqual("<1.50;-2.25>", result);
            Assert.IsFalse(result.Contains(","), "Invariant formatting should use a decimal point instead of a comma.");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// Verifies that <see cref="TFTuple.Init(double, double)"/> assigns both components.
    /// </summary>
    [TestMethod]
    public void InitAssignsBothComponents()
    {
        var tuple = TFTuple.Zero;

        tuple.Init(4.0, -5.0);

        AssertTuple(tuple, 4.0, -5.0);
    }

    /// <summary>
    /// Verifies that <see cref="TFTuple.InitLenDir(double, double)"/> converts polar input into Cartesian components.
    /// </summary>
    [TestMethod]
    public void InitLenDirCreatesExpectedTuple()
    {
        var tuple = TFTuple.Zero;

        tuple.InitLenDir(2.0, Math.PI / 6.0);

        AssertTuple(tuple, Math.Sqrt(3.0), 1.0);
    }

    /// <summary>
    /// Verifies that <see cref="TFTuple.Sum(in TFTuple)"/> and <see cref="TFTuple.Subt(in TFTuple)"/> return new values without mutating the source.
    /// </summary>
    [TestMethod]
    public void SumAndSubtReturnNewTuples()
    {
        var left = TFTuple.Copy(1.0, 2.0);
        var right = TFTuple.Copy(4.0, -5.0);

        var sum = left.Sum(right);
        var diff = left.Subt(right);

        AssertTuple(sum, 5.0, -3.0);
        AssertTuple(diff, -3.0, 7.0);
        AssertTuple(left, 1.0, 2.0);
    }

    /// <summary>
    /// Verifies that in-place arithmetic methods update the current instance and return the updated value.
    /// </summary>
    [TestMethod]
    public void AddSubtToAndMulToUpdateCurrentInstance()
    {
        var tuple = TFTuple.Copy(1.0, 2.0);

        var afterAdd = tuple.Add(TFTuple.Copy(4.0, -1.0));
        AssertTuple(tuple, 5.0, 1.0);
        AssertTuple(afterAdd, 5.0, 1.0);

        var afterSubt = tuple.SubtTo(TFTuple.Copy(2.0, 3.0));
        AssertTuple(tuple, 3.0, -2.0);
        AssertTuple(afterSubt, 3.0, -2.0);

        var afterMul = tuple.MulTo(0.5);
        AssertTuple(tuple, 1.5, -1.0);
        AssertTuple(afterMul, 1.5, -1.0);
    }

    /// <summary>
    /// Verifies dot product, scalar multiplication, and division.
    /// </summary>
    [TestMethod]
    public void MulAndDivideReturnExpectedValues()
    {
        var left = TFTuple.Copy(1.0, 2.0);
        var right = TFTuple.Copy(4.0, -5.0);

        var dot = left.Mul(right);
        var scaled = left.Mul(2.5);
        var divided = right.Divide(2.0);

        Assert.AreEqual(-6.0, dot, Epsilon);
        AssertTuple(scaled, 2.5, 5.0);
        AssertTuple(divided, 2.0, -2.5);
    }

    /// <summary>
    /// Verifies that <see cref="TFTuple.VMul(in TFTuple)"/> matches complex-number multiplication semantics.
    /// </summary>
    [TestMethod]
    public void VMulReturnsComplexLikeProduct()
    {
        var left = TFTuple.Copy(1.0, 2.0);
        var right = TFTuple.Copy(3.0, 4.0);

        var result = left.VMul(right);

        AssertTuple(result, -5.0, 10.0);
    }

    /// <summary>
    /// Verifies that <see cref="TFTuple.Equals(in TFTuple, double)"/> uses the supplied tolerance.
    /// </summary>
    [TestMethod]
    public void EqualsUsesAbsoluteTolerance()
    {
        var left = TFTuple.Copy(1.0, 2.0);
        var almostEqual = TFTuple.Copy(1.0 + 1e-13, 2.0 - 1e-13);
        var different = TFTuple.Copy(1.0, 2.01);

        Assert.IsTrue(left.Equals(almostEqual, 1e-12));
        Assert.IsFalse(left.Equals(different, 1e-12));
    }

    /// <summary>
    /// Verifies that the copy helpers return equivalent values.
    /// </summary>
    [TestMethod]
    public void CopyMethodsReturnEquivalentValues()
    {
        var original = TFTuple.Copy(7.0, -8.5);

        var copiedFromComponents = TFTuple.Copy(7.0, -8.5);
        var copiedFromTuple = TFTuple.Copy(original);
        var copiedFromInstance = original.Copy();

        AssertTuple(copiedFromComponents, 7.0, -8.5);
        AssertTuple(copiedFromTuple, 7.0, -8.5);
        AssertTuple(copiedFromInstance, 7.0, -8.5);
    }

    /// <summary>
    /// Verifies Euclidean length, max norm, squared length, and absolute helper calculations.
    /// </summary>
    [TestMethod]
    public void LengthHelpersReturnExpectedValues()
    {
        var tuple = TFTuple.Copy(3.0, -4.0);

        Assert.AreEqual(5.0, tuple.GLen(), Epsilon);
        Assert.AreEqual(4.0, tuple.MLen(), Epsilon);
        Assert.AreEqual(25.0, TFTuple.Sqr(tuple), Epsilon);
        Assert.AreEqual(5.0, TFTuple.Abs(tuple), Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="TFTuple.GDir()"/> returns the expected direction angle.
    /// </summary>
    [TestMethod]
    [DataRow(1.0, 0.0, 0.0)]
    [DataRow(0.0, 1.0, Math.PI / 2.0)]
    [DataRow(-1.0, 0.0, Math.PI)]
    [DataRow(0.0, -1.0, -Math.PI / 2.0)]
    public void GDirReturnsExpectedAngle(double fX, double fY, double fExpectedAngle)
    {
        var tuple = TFTuple.Copy(fX, fY);

        var angle = tuple.GDir();

        Assert.AreEqual(fExpectedAngle, angle, Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="TFTuple.GDir()"/> returns angles that reconstruct tuples created by <see cref="TFTuple.InitLenDir(double, double)"/>.
    /// </summary>
    [TestMethod]
    public void GDirRoundTripsAngleFromInitLenDir()
    {
        const double fLength = 3.0;
        const double fDirection = Math.PI / 3.0;

        var tuple = TFTuple.Zero;
        tuple.InitLenDir(fLength, fDirection);

        var direction = tuple.GDir();

        Assert.AreEqual(fDirection, direction, Epsilon);
    }

    /// <summary>
    /// Verifies that arithmetic operators delegate to the same mathematical behavior as the named methods.
    /// </summary>
    [TestMethod]
    public void OperatorsReturnExpectedResults()
    {
        var left = TFTuple.Copy(1.0, 2.0);
        var right = TFTuple.Copy(4.0, -5.0);

        var sum = left + right;
        var diff = left - right;
        var negated = -left;
        var dot = left * right;
        var scaledLeft = left * 2.0;
        var scaledRight = 2.0 * left;
        var divided = right / 2.0;

        AssertTuple(sum, 5.0, -3.0);
        AssertTuple(diff, -3.0, 7.0);
        AssertTuple(negated, -1.0, -2.0);
        Assert.AreEqual(-6.0, dot, Epsilon);
        AssertTuple(scaledLeft, 2.0, 4.0);
        AssertTuple(scaledRight, 2.0, 4.0);
        AssertTuple(divided, 2.0, -2.5);
    }

    /// <summary>
    /// Asserts that a tuple exposes the expected Cartesian components.
    /// </summary>
    /// <param name="tuple">The tuple under test.</param>
    /// <param name="fExpectedX">The expected X component.</param>
    /// <param name="fExpectedY">The expected Y component.</param>
    private static void AssertTuple(TFTuple tuple, double fExpectedX, double fExpectedY)
    {
        Assert.AreEqual(fExpectedX, tuple.X, Epsilon);
        Assert.AreEqual(fExpectedY, tuple.Y, Epsilon);
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
