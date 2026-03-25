using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHack.Base.Data;
using SharpHack.Base.Model;
using SharpHack.Engine;

namespace SharpHack.EngineTests;

[TestClass]
public class FieldOfViewTests
{
    [TestMethod]
    public void Compute_Always_MarksOriginVisibleAndExplored()
    {
        var map = new Map(10, 10);
        map[5, 5].Type = TileType.Floor;

        var fov = new FieldOfView(map);

        fov.Compute(new Point(5, 5), range: 3);

        Assert.IsTrue(map[5, 5].IsVisible);
        Assert.IsTrue(map[5, 5].IsExplored);
    }

    [TestMethod]
    public void Compute_SetsTilesOutsideRange_NotVisible()
    {
        var map = new Map(10, 10);
        map[5, 5].Type = TileType.Floor;
        map[9, 9].Type = TileType.Floor;

        var fov = new FieldOfView(map);

        fov.Compute(new Point(5, 5), range: 3);

        Assert.IsFalse(map[9, 9].IsVisible);
    }

    [TestMethod]
    public void Compute_DoesNotRevealTilesBehindWallInCardinalDirection()
    {
        var map = new Map(10, 10);

        map[5, 5].Type = TileType.Floor; // origin
        map[6, 5].Type = TileType.Wall;  // blocker
        map[7, 5].Type = TileType.Floor; // behind wall

        var fov = new FieldOfView(map);

        fov.Compute(new Point(5, 5), range: 5);

        Assert.IsTrue(map[6, 5].IsVisible, "The blocking wall itself should be visible.");
        Assert.IsFalse(map[7, 5].IsVisible, "Tile directly behind wall should not be visible.");
        Assert.IsFalse(map[7, 5].IsExplored, "Tile directly behind wall should not be explored.");
    }
}
