using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHack.Base.Data;
using SharpHack.Base.Model;

namespace SharpHack.BaseTests.Model;

[TestClass]
public class MapTests
{
    [TestMethod]
    public void Constructor_InitializesDimensions()
    {
        var map = new Map(10, 20);
        Assert.AreEqual(10, map.Width);
        Assert.AreEqual(20, map.Height);
    }

    [TestMethod]
    public void Constructor_InitializesEmptyTiles()
    {
        var map = new Map(5, 5);
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Assert.AreEqual(TileType.Empty, map[x, y].Type);
                Assert.AreEqual(new Point(x, y), map[x, y].Position);
            }
        }
    }

    [TestMethod]
    public void IsValid_ReturnsTrueForValidCoordinates()
    {
        var map = new Map(10, 10);
        Assert.IsTrue(map.IsValid(0, 0));
        Assert.IsTrue(map.IsValid(9, 9));
        Assert.IsTrue(map.IsValid(new Point(5, 5)));
    }

    [TestMethod]
    public void IsValid_ReturnsFalseForInvalidCoordinates()
    {
        var map = new Map(10, 10);
        Assert.IsFalse(map.IsValid(-1, 0));
        Assert.IsFalse(map.IsValid(0, -1));
        Assert.IsFalse(map.IsValid(10, 0));
        Assert.IsFalse(map.IsValid(0, 10));
    }

    [TestMethod]
    public void Indexer_ReturnsEmptyTileForOutOfBounds()
    {
        var map = new Map(10, 10);
        var tile = map[100, 100];
        Assert.IsNotNull(tile);
        Assert.AreEqual(TileType.Empty, tile.Type);
        // Note: The current implementation returns a new tile with the requested coordinates
        Assert.AreEqual(100, tile.Position.X);
    }
}
