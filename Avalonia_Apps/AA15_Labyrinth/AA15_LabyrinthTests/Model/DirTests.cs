using System.Collections.Generic;
using AA15_Labyrinth.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA15_Labyrinth.Model.Tests;

[TestClass]
public class DirTests
{
    [TestMethod]
    public void All_ContainsExactlyTheTwelveExpectedDirections()
    {
        HashSet<Dir> expected = new HashSet<Dir>
        {
            new Dir(2, 0),
            new Dir(0, -2),
            new Dir(0, 2),
            new Dir(-2, 0),
            new Dir(2, 1),
            new Dir(1, -2),
            new Dir(-1, 2),
            new Dir(-2, -1),
            new Dir(1, 2),
            new Dir(2, -1),
            new Dir(-2, 1),
            new Dir(-1, -2)
        };

        Assert.AreEqual(expected.Count, Dir.All.Length);
        Assert.IsTrue(expected.SetEquals(Dir.All));
    }

    [TestMethod]
    public void All_ContainsOnlyUniqueDirections()
    {
        HashSet<Dir> uniqueDirections = new HashSet<Dir>(Dir.All);

        Assert.AreEqual(Dir.All.Length, uniqueDirections.Count);
    }

    [TestMethod]
    public void Constructor_PreservesCoordinatesAndValueEquality()
    {
        Dir direction = new Dir(2, -1);
        Dir equalDirection = new Dir(2, -1);
        Dir differentDirection = new Dir(-2, 1);

        Assert.AreEqual(2, direction.Dx);
        Assert.AreEqual(-1, direction.Dy);
        Assert.AreEqual(equalDirection, direction);
        Assert.AreNotEqual(differentDirection, direction);
    }

    [TestMethod]
    public void Equals_ReturnsExpectedResultForDifferentObjectValues()
    {
        Dir direction = new Dir(2, -1);
        object equalDirection = new Dir(2, -1);
        object differentDirection = new Dir(-2, 1);

        Assert.IsTrue(direction.Equals(equalDirection));
        Assert.IsFalse(direction.Equals(differentDirection));
        Assert.IsFalse(direction.Equals(null));
        Assert.IsFalse(direction.Equals(new object()));
    }
}
