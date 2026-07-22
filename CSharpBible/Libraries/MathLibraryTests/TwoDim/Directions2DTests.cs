using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLibrary.TwoDim.Tests;

/// <summary>
/// Defines tests for <see cref="Directions2D"/>.
/// </summary>
[TestClass]
public class Directions2DTests
{
    /// <summary>
    /// Verifies that <see cref="Directions2D.Dir4"/>, <see cref="Directions2D.Dir8"/>, and <see cref="Directions2D.Dir12"/> expose the expected predefined vectors.
    /// </summary>
    [TestMethod]
    public void PredefinedDirectionSetsReturnExpectedVectors()
    {
        CollectionAssert.AreEqual(
            new[] { new IntPoint(0, 0), new IntPoint(1, 0), new IntPoint(0, 1), new IntPoint(-1, 0), new IntPoint(0, -1) },
            Directions2D.Dir4);

        CollectionAssert.AreEqual(
            new[]
            {
                new IntPoint(0, 0), new IntPoint(1, 0), new IntPoint(1, 1), new IntPoint(0, 1), new IntPoint(-1, 1),
                new IntPoint(-1, 0), new IntPoint(-1, -1), new IntPoint(0, -1), new IntPoint(1, -1)
            },
            Directions2D.Dir8);

        CollectionAssert.AreEqual(
            new[]
            {
                new IntPoint(0, 0), new IntPoint(2, 0), new IntPoint(2, 1), new IntPoint(1, 2), new IntPoint(0, 2),
                new IntPoint(-1, 2), new IntPoint(-2, 1), new IntPoint(-2, 0), new IntPoint(-2, -1), new IntPoint(-1, -2),
                new IntPoint(0, -2), new IntPoint(1, -2), new IntPoint(2, -1)
            },
            Directions2D.Dir12);
    }

    /// <summary>
    /// Verifies that <see cref="Directions2D.GetDirNo(IntPoint)"/> returns the index for supported predefined directions.
    /// </summary>
    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(1, 0, 1)]
    [DataRow(1, 1, 2)]
    [DataRow(0, 1, 3)]
    [DataRow(-1, 1, 4)]
    [DataRow(-1, 0, 5)]
    [DataRow(-1, -1, 6)]
    [DataRow(0, -1, 7)]
    [DataRow(1, -1, 8)]
    [DataRow(2, 0, 1)]
    [DataRow(2, 1, 2)]
    [DataRow(1, 2, 3)]
    [DataRow(0, 2, 4)]
    [DataRow(-1, 2, 5)]
    [DataRow(-2, 1, 6)]
    [DataRow(-2, 0, 7)]
    [DataRow(-2, -1, 8)]
    [DataRow(-1, -2, 9)]
    [DataRow(0, -2, 10)]
    [DataRow(1, -2, 11)]
    [DataRow(2, -1, 12)]
    public void GetDirNoReturnsExpectedIndexForSupportedVectors(int x, int y, int expected)
    {
        var vector = new IntPoint(x, y);

        var result = Directions2D.GetDirNo(vector);

        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// Verifies that <see cref="Directions2D.GetDirNo(IntPoint)"/> rejects unsupported vectors.
    /// </summary>
    [TestMethod]
    [DataRow(3, 0)]
    [DataRow(2, 2)]
    [DataRow(1, 3)]
    [DataRow(-3, -1)]
    public void GetDirNoReturnsMinusOneForUnsupportedVectors(int x, int y)
    {
        var vector = new IntPoint(x, y);

        var result = Directions2D.GetDirNo(vector);

        Assert.AreEqual(-1, result);
    }

    /// <summary>
    /// Verifies that <see cref="Directions2D.GetInvDir(int, int)"/> returns the input unchanged for non-positive direction numbers.
    /// </summary>
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetInvDirReturnsInputForNonPositiveDirectionNumbers(int dir)
    {
        Assert.AreEqual(dir, Directions2D.GetInvDir(dir, 15));
    }

    /// <summary>
    /// Verifies the inverse mapping used for radius 10.
    /// </summary>
    [TestMethod]
    [DataRow(1, 4)]
    [DataRow(2, 5)]
    [DataRow(3, 6)]
    [DataRow(4, 1)]
    [DataRow(5, 2)]
    [DataRow(6, 3)]
    public void GetInvDirReturnsExpectedOppositeForRadius10(int dir, int expected)
    {
        Assert.AreEqual(expected, Directions2D.GetInvDir(dir, 10));
    }

    /// <summary>
    /// Verifies the inverse mapping used for radius 15 based on <see cref="Directions2D.Dir8"/>.
    /// </summary>
    [TestMethod]
    [DataRow(1, 5)]
    [DataRow(2, 6)]
    [DataRow(3, 7)]
    [DataRow(4, 8)]
    [DataRow(5, 1)]
    [DataRow(6, 2)]
    [DataRow(7, 3)]
    [DataRow(8, 4)]
    public void GetInvDirReturnsExpectedOppositeForRadius15(int dir, int expected)
    {
        Assert.AreEqual(expected, Directions2D.GetInvDir(dir, 15));
    }

    /// <summary>
    /// Verifies the inverse mapping used for radius 22 based on <see cref="Directions2D.Dir12"/>.
    /// </summary>
    [TestMethod]
    [DataRow(1, 7)]
    [DataRow(2, 8)]
    [DataRow(3, 9)]
    [DataRow(4, 10)]
    [DataRow(5, 11)]
    [DataRow(6, 12)]
    [DataRow(7, 1)]
    [DataRow(8, 2)]
    [DataRow(9, 3)]
    [DataRow(10, 4)]
    [DataRow(11, 5)]
    [DataRow(12, 6)]
    public void GetInvDirReturnsExpectedOppositeForRadius22(int dir, int expected)
    {
        Assert.AreEqual(expected, Directions2D.GetInvDir(dir, 22));
    }

    /// <summary>
    /// Verifies that unsupported radii leave the direction unchanged.
    /// </summary>
    [TestMethod]
    [DataRow(1, 0)]
    [DataRow(3, 5)]
    [DataRow(8, 99)]
    public void GetInvDirReturnsInputForUnsupportedRadius(int dir, int radius)
    {
        Assert.AreEqual(dir, Directions2D.GetInvDir(dir, radius));
    }
}
