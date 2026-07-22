using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLibrary.TwoDim.Tests;

/// <summary>
/// Defines tests for <see cref="IntPoint"/>.
/// </summary>
[TestClass]
public class IntPointTests
{
    /// <summary>
    /// Verifies that the predefined basis vectors expose the expected coordinates.
    /// </summary>
    [TestMethod]
    public void PredefinedVectorsReturnExpectedCoordinates()
    {
        Assert.AreEqual(new IntPoint(0, 0), IntPoint.Zero);
        Assert.AreEqual(new IntPoint(1, 0), IntPoint.EX);
        Assert.AreEqual(new IntPoint(0, 1), IntPoint.EY);
    }

    /// <summary>
    /// Verifies that <see cref="IntPoint.Add(IntPoint)"/> returns the component-wise sum.
    /// </summary>
    [TestMethod]
    [DataRow(1, 2, 3, 4, 4, 6)]
    [DataRow(-1, 5, 2, -3, 1, 2)]
    [DataRow(0, 0, 0, 0, 0, 0)]
    public void AddReturnsExpectedPoint(int x1, int y1, int x2, int y2, int expectedX, int expectedY)
    {
        var left = new IntPoint(x1, y1);
        var right = new IntPoint(x2, y2);

        var result = left.Add(right);

        Assert.AreEqual(new IntPoint(expectedX, expectedY), result);
        Assert.AreEqual(new IntPoint(x1, y1), left);
    }

    /// <summary>
    /// Verifies that <see cref="IntPoint.Sub(IntPoint)"/> returns the component-wise difference.
    /// </summary>
    [TestMethod]
    [DataRow(5, 7, 2, 3, 3, 4)]
    [DataRow(-1, 5, 2, -3, -3, 8)]
    [DataRow(0, 0, 0, 0, 0, 0)]
    public void SubReturnsExpectedPoint(int x1, int y1, int x2, int y2, int expectedX, int expectedY)
    {
        var left = new IntPoint(x1, y1);
        var right = new IntPoint(x2, y2);

        var result = left.Sub(right);

        Assert.AreEqual(new IntPoint(expectedX, expectedY), result);
        Assert.AreEqual(new IntPoint(x1, y1), left);
    }

    /// <summary>
    /// Verifies that <see cref="IntPoint.Neg()"/> negates both coordinates.
    /// </summary>
    [TestMethod]
    [DataRow(1, 2, -1, -2)]
    [DataRow(-3, 4, 3, -4)]
    [DataRow(0, 0, 0, 0)]
    public void NegReturnsExpectedPoint(int x, int y, int expectedX, int expectedY)
    {
        var point = new IntPoint(x, y);

        var result = point.Neg();

        Assert.AreEqual(new IntPoint(expectedX, expectedY), result);
        Assert.AreEqual(new IntPoint(x, y), point);
    }

    /// <summary>
    /// Verifies that <see cref="IntPoint.Scale(int)"/> multiplies both coordinates by the scalar.
    /// </summary>
    [TestMethod]
    [DataRow(1, 2, 3, 3, 6)]
    [DataRow(-2, 5, -2, 4, -10)]
    [DataRow(7, -4, 0, 0, 0)]
    public void ScaleReturnsExpectedPoint(int x, int y, int factor, int expectedX, int expectedY)
    {
        var point = new IntPoint(x, y);

        var result = point.Scale(factor);

        Assert.AreEqual(new IntPoint(expectedX, expectedY), result);
        Assert.AreEqual(new IntPoint(x, y), point);
    }

    /// <summary>
    /// Verifies that <see cref="IntPoint.GLen()"/> returns the Manhattan length.
    /// </summary>
    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(1, 2, 3)]
    [DataRow(-3, 4, 7)]
    [DataRow(-5, -6, 11)]
    public void GLenReturnsManhattanLength(int x, int y, int expected)
    {
        var point = new IntPoint(x, y);

        var result = point.GLen();

        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// Verifies that <see cref="IntPoint.MLen()"/> returns the maximum norm.
    /// </summary>
    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(1, 2, 2)]
    [DataRow(-3, 4, 4)]
    [DataRow(-5, -6, 6)]
    public void MLenReturnsMaximumNorm(int x, int y, int expected)
    {
        var point = new IntPoint(x, y);

        var result = point.MLen();

        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// Verifies that <see cref="IntPoint.Dot(IntPoint)"/> returns the scalar product.
    /// </summary>
    [TestMethod]
    [DataRow(1, 2, 3, 4, 11)]
    [DataRow(-1, 5, 2, -3, -17)]
    [DataRow(0, 0, 7, 9, 0)]
    public void DotReturnsExpectedScalar(int x1, int y1, int x2, int y2, int expected)
    {
        var left = new IntPoint(x1, y1);
        var right = new IntPoint(x2, y2);

        var result = left.Dot(right);

        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// Verifies that <see cref="IntPoint.ToString()"/> formats the point as expected.
    /// </summary>
    [TestMethod]
    [DataRow(0, 0, "<0,0>")]
    [DataRow(1, -2, "<1,-2>")]
    [DataRow(-3, 4, "<-3,4>")]
    public void ToStringReturnsExpectedFormat(int x, int y, string expected)
    {
        var point = new IntPoint(x, y);

        var result = point.ToString();

        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// Verifies record-struct value equality and hash code consistency.
    /// </summary>
    [TestMethod]
    public void EqualityAndHashCodeUseCoordinateValues()
    {
        var first = new IntPoint(3, -4);
        var same = new IntPoint(3, -4);
        var different = new IntPoint(3, 4);

        Assert.AreEqual(first, same);
        Assert.AreNotEqual(first, different);
        Assert.AreEqual(first.GetHashCode(), same.GetHashCode());
    }
}
