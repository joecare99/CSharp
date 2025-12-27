using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using VTileEdit.Models;

namespace VTileEditTests.Models;

[TestClass]
public class VisTileDataMetadataTests
{
    private VisTileData _sut = null!;

    [TestInitialize]
    public void SetUp()
    {
        _sut = new VisTileData();
        _sut.SetTileSize(new Size(2, 1));
        _sut.SetTileDef(TestTile.First, CreateTile("AB"));
        _sut.SetTileDef(TestTile.Second, CreateTile("CD"));
    }

    [TestMethod]
    public void TileKeysExposeInsertedEntries()
    {
        var keys = _sut.Keys.ToList();

        CollectionAssert.AreEquivalent(new Enum[] { TestTile.First, TestTile.Second }, keys);
    }

    [TestMethod]
    public void SetTileInfoReturnsClonedCopy()
    {
        var expected = new TileInfo
        {
            Category = TileCategory.Creature,
            SubCategory = "Boss",
            Tags = new[] { "lava", "flying" }
        };

        _sut.SetTileInfo(TestTile.First, expected);

        var retrieved = _sut.GetTileInfo(TestTile.First);
        CollectionAssert.AreEqual(expected.Tags.ToArray(), retrieved.Tags.ToArray());
        Assert.AreEqual(expected.Category, retrieved.Category);
        Assert.AreEqual(expected.SubCategory, retrieved.SubCategory);

        retrieved.SubCategory = "Changed";
        retrieved.Tags = new[] { "other" };

        var secondFetch = _sut.GetTileInfo(TestTile.First);
        Assert.AreEqual("Boss", secondFetch.SubCategory);
        CollectionAssert.AreEqual(new[] { "lava", "flying" }, secondFetch.Tags.ToArray());
    }

    [TestMethod]
    public void Json2RoundtripPreservesMetadata()
    {
        _sut.SetTileInfo(TestTile.First, new TileInfo
        {
            Category = TileCategory.Item,
            SubCategory = "Artifact",
            Tags = new[] { "legendary", "quest" }
        });

        using var stream = new MemoryStream();
        Assert.IsTrue(_sut.WriteToStream(stream, EStreamType.Json2));
        stream.Position = 0;

        var clone = new VisTileData();
        Assert.IsTrue(clone.LoadFromStream(stream, EStreamType.Json2));

        var clonedInfo = clone.GetTileInfo(TestTile.First);
        Assert.AreEqual(TileCategory.Item, clonedInfo.Category);
        Assert.AreEqual("Artifact", clonedInfo.SubCategory);
        CollectionAssert.AreEqual(new[] { "legendary", "quest" }, clonedInfo.Tags.ToArray());
    }

    private static SingleTile CreateTile(string line)
    {
        var lines = new[] { line };
        var colors = Enumerable.Range(0, line.Length)
            .Select(_ => new FullColor(ConsoleColor.Gray, ConsoleColor.Black))
            .ToArray();
        return new SingleTile(lines, colors);
    }

    private enum TestTile
    {
        First = 0,
        Second = 1,
    }
}
