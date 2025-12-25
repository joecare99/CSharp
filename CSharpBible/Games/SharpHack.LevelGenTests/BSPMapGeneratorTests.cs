using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpHack.LevelGen.BSP;
using SharpHack.Base.Model;
using BaseLib.Models.Interfaces;

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
}
