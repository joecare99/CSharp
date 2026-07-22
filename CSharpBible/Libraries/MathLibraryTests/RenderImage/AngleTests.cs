using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace MathLibrary.RenderImage.Tests;

/// <summary>
/// Defines tests for <see cref="Angle"/>.
/// </summary>
[TestClass]
public class AngleTests
{
    /// <summary>
    /// Defines the numeric tolerance used by floating-point assertions.
    /// </summary>
    private const double Epsilon = 1e-12;

    /// <summary>
    /// Verifies that reading <see cref="Angle.AsGrad"/> converts radians to degrees.
    /// </summary>
    [TestMethod]
    [DataRow(0.0, 0.0)]
    [DataRow(Math.PI * 0.5, 90.0)]
    [DataRow(Math.PI, 180.0)]
    [DataRow(-Math.PI * 0.25, -45.0)]
    public void AsGradGetterReturnsDegrees(double fRadians, double fExpectedDegrees)
    {
        var angle = (Angle)fRadians;

        Assert.AreEqual(fExpectedDegrees, angle.AsGrad, Epsilon);
    }

    /// <summary>
    /// Verifies that writing <see cref="Angle.AsGrad"/> converts degrees to radians.
    /// </summary>
    [TestMethod]
    [DataRow(0.0, 0.0)]
    [DataRow(90.0, Math.PI * 0.5)]
    [DataRow(180.0, Math.PI)]
    [DataRow(-45.0, -Math.PI * 0.25)]
    public void AsGradSetterStoresRadians(double fDegrees, double fExpectedRadians)
    {
        var angle = Angle.Zero;
        angle.AsGrad = fDegrees;

        Assert.AreEqual(fExpectedRadians, angle.Value, Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="Angle.Normalize(Angle)"/> maps angles into the canonical interval.
    /// </summary>
    [TestMethod]
    [DataRow(0.0, 0.0)]
    [DataRow(Math.PI, -Math.PI)]
    [DataRow(-Math.PI, -Math.PI)]
    [DataRow(2.0 * Math.PI, 0.0)]
    [DataRow(-2.0 * Math.PI, 0.0)]
    [DataRow(2.5 * Math.PI, 0.5 * Math.PI)]
    [DataRow(-2.5 * Math.PI, -0.5 * Math.PI)]
    public void NormalizeStaticReturnsCanonicalValue(double fInput, double fExpected)
    {
        var angle = Angle.Normalize((Angle)fInput);

        Assert.AreEqual(fExpected, angle.Value, Epsilon);
        Assert.IsTrue(angle.Value >= -Math.PI, "Normalized angle must be greater than or equal to -PI.");
        Assert.IsTrue(angle.Value < Math.PI, "Normalized angle must be less than PI.");
    }

    /// <summary>
    /// Verifies that the instance normalization delegates to the static normalization logic.
    /// </summary>
    [TestMethod]
    public void NormalizeInstanceReturnsSameResultAsStaticNormalize()
    {
        var angle = (Angle)(3.5 * Math.PI);

        var expected = Angle.Normalize(angle);
        var actual = angle.Normalize();

        Assert.AreEqual(expected.Value, actual.Value, Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="Angle.Sum(Angle)"/> returns a normalized sum.
    /// </summary>
    [TestMethod]
    public void SumReturnsNormalizedAngle()
    {
        var angle1 = (Angle)(0.75 * Math.PI);
        var angle2 = (Angle)(0.75 * Math.PI);

        var result = angle1.Sum(angle2);

        Assert.AreEqual(-0.5 * Math.PI, result.Value, Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="Angle.Add(Angle)"/> updates the current struct value and returns it.
    /// </summary>
    [TestMethod]
    public void AddUpdatesCurrentValue()
    {
        var angle = (Angle)(0.75 * Math.PI);

        var result = angle.Add((Angle)(0.75 * Math.PI));

        Assert.AreEqual(-0.5 * Math.PI, angle.Value, Epsilon);
        Assert.AreEqual(angle.Value, result.Value, Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="Angle.Diff(Angle)"/> returns a normalized difference.
    /// </summary>
    [TestMethod]
    public void DiffReturnsNormalizedAngle()
    {
        var angle1 = (Angle)(-0.75 * Math.PI);
        var angle2 = (Angle)(0.75 * Math.PI);

        var result = angle1.Diff(angle2);

        Assert.AreEqual(0.5 * Math.PI, result.Value, Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="Angle.Subt(Angle)"/> updates the current struct value and returns it.
    /// </summary>
    [TestMethod]
    public void SubtUpdatesCurrentValue()
    {
        var angle = (Angle)(-0.75 * Math.PI);

        var result = angle.Subt((Angle)(0.75 * Math.PI));

        Assert.AreEqual(0.5 * Math.PI, angle.Value, Epsilon);
        Assert.AreEqual(angle.Value, result.Value, Epsilon);
    }

    /// <summary>
    /// Verifies that implicit conversion operators preserve the underlying radian value.
    /// </summary>
    [TestMethod]
    public void ImplicitConversionsRoundTripRadians()
    {
        const double fExpected = 1.23456789;

        Angle angle = fExpected;
        double actual = angle;

        Assert.AreEqual(fExpected, angle.Value, Epsilon);
        Assert.AreEqual(fExpected, actual, Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="Angle.Zero"/> represents zero radians.
    /// </summary>
    [TestMethod]
    public void ZeroReturnsZeroRadians()
    {
        Assert.AreEqual(0.0, Angle.Zero.Value, Epsilon);
    }

    /// <summary>
    /// Verifies that <see cref="Angle.ToString()"/> formats the degree value using invariant culture.
    /// </summary>
    [TestMethod]
    public void ToStringUsesInvariantCultureFormatting()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var angle = Angle.Zero;
            angle.AsGrad = 12.5;

            var result = angle.ToString();

            StringAssert.Contains(result, "12.50");
            Assert.IsFalse(result.Contains(","), "Invariant formatting should use a decimal point instead of a comma.");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
