using BaseLib.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpHack.Base.Data;
using SharpHack.LevelGen.BSP;

namespace SharpHack.LevelGenTests;

[TestClass]
public class BSPRoomMazeMapGeneratorTests
{
    [TestMethod]
    public void Generate_ReturnsMapWithCorrectDimensions()
    {
        var random = Substitute.For<IRandom>();
        random.Next(Arg.Any<int>()).Returns(1);
        random.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(ci => (ci.ArgAt<int>(0) + ci.ArgAt<int>(1)) / 2);

        var gen = new BSPRoomMazeMapGenerator(random);
        var map = gen.Generate(41, 37);

        Assert.AreEqual(41, map.Width);
        Assert.AreEqual(37, map.Height);
    }

    [TestMethod]
    public void Generate_Creates_3x3_StartRoom_Around_StartPos_With_Even_Center()
    {
        var random = Substitute.For<IRandom>();
        random.Next(Arg.Any<int>()).Returns(1);
        random.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(ci => (ci.ArgAt<int>(0) + ci.ArgAt<int>(1)) / 2);

        var gen = new BSPRoomMazeMapGenerator(random);
        var start = new SharpHack.Base.Model.Point(21, 21); // odd -> generator should clamp to even
        var map = gen.Generate(50, 50, start);

        // center should be even/even
        int cx = 22;
        int cy = 22;

        for (int x = cx - 1; x <= cx + 1; x++)
        {
            for (int y = cy - 1; y <= cy + 1; y++)
            {
                Assert.AreEqual(TileType.Room, map[x, y].Type, "Start room must be a 3x3 Room block.");
            }
        }

        // The wall ring around the room must remain walls (distance 2 from center in cardinal directions)
        Assert.AreEqual(TileType.Wall, map[cx + 2, cy].Type);
        Assert.AreEqual(TileType.Wall, map[cx - 2, cy].Type);
        Assert.AreEqual(TileType.Wall, map[cx, cy + 2].Type);
        Assert.AreEqual(TileType.Wall, map[cx, cy - 2].Type);
    }

    [TestMethod]
    public void Generate_Creates_Maze_Floors_And_Doors()
    {
        var random = Substitute.For<IRandom>();
        random.Next(Arg.Any<int>()).Returns(1);
        random.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(ci => (ci.ArgAt<int>(0) + ci.ArgAt<int>(1)) / 2);

        var gen = new BSPRoomMazeMapGenerator(random);
        var map = gen.Generate(60, 40, new SharpHack.Base.Model.Point(5, 5));

        bool hasFloor = false;
        bool hasRoom = false;
        bool hasDoor = false;

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                hasFloor |= map[x, y].Type == TileType.Floor;
                hasRoom |= map[x, y].Type == TileType.Room;
                hasDoor |= map[x, y].Type == TileType.DoorClosed;
            }
        }

        Assert.IsTrue(hasRoom, "Map should contain room tiles.");
        Assert.IsTrue(hasFloor, "Map should contain floor (maze) tiles.");
        Assert.IsTrue(hasDoor, "Map should contain doors connecting rooms to the maze.");
    }
}
