using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpHack.LevelGen.BSP;
using BaseLib.Models.Interfaces;
using SharpHack.Base.Data;
using BaseLib.Helper;

namespace SharpHack.LevelGenTests;

[TestClass]
public class BSPMapGeneratorTests
{
    [TestMethod]
    public void Generate_ReturnsMapWithCorrectDimensions()
    {
        var random = Substitute.For<IRandom>();
        var generator = new BSPMapGenerator(random);
        int width = 50;
        int height = 50;

        var map = generator.Generate(width, height);

        Assert.AreEqual(width, map.Width);
        Assert.AreEqual(height, map.Height);
    }

    [TestMethod]
    public void Generate_CreatesWalkableTiles()
    {
        var random = Substitute.For<IRandom>();
        // Mock random to ensure some splits happen if needed, 
        // but BSP logic is complex to mock deterministically without knowing internal calls.
        // For now, we rely on the fact that it should produce *some* floor tiles.
        random.Next(Arg.Any<int>()).Returns(10); 
        random.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(10); 
        
        var generator = new BSPMapGenerator(random);
        var map = generator.Generate(40, 40);

        bool hasFloor = false;
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                if (map[x, y].Type == TileType.Floor)
                {
                    hasFloor = true;
                    break;
                }
            }
        }

        Assert.IsTrue(hasFloor, "Map should contain floor tiles.");
    }

    [TestMethod]
    [DataRow(20, 20)]
    [DataRow(19, 20)]
    [DataRow(20, 19)]
    public void Generate_CreatesRoomWhereplayer(int x, int y)
    {
        var random = Substitute.For<IRandom>();
        // Mock random to ensure some splits happen if needed, 
        // but BSP logic is complex to mock deterministically without knowing internal calls.
        // For now, we rely on the fact that it should produce *some* floor tiles.
        random.Next(Arg.Any<int>()).Returns(o => o.Args()[0].AsInt()/2); 
        random.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(o =>( o.Args()[0].AsInt()+ o.Args()[1].AsInt() ) / 2); 
        
        var generator = new BSPMapGenerator(random);
        var map = generator.Generate(40, 40 , new Base.Model.Point(x,y));


        Assert.AreEqual(TileType.Floor, map[x, y].Type, "Map should contain floor tiles.");
    }
}
