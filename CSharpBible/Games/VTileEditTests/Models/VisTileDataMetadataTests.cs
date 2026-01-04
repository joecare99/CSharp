using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using VTileEdit.Models;

namespace VTileEdit.Models.Tests;

[TestClass]
public class VisTileDataMetadataTests
{
    private VisTileData _sut = null!;

    [TestInitialize]
    public void SetUp()
    {
        _sut = new VisTileData();
        _sut.SetTileSize(new Size(2, 1));
        _sut.SetTileDef(0, CreateTile("AB"));
        _sut.SetTileDef(1, CreateTile("CD"));
    }

    [TestMethod]
    public void TileKeysExposeInsertedEntries()
    {
        var keys = _sut.Keys.ToList();

        CollectionAssert.AreEquivalent(new int[] { 0, 1}, keys);
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

        _sut.SetTileInfo(0, expected);

        var retrieved = _sut.GetTileInfo(0);
        CollectionAssert.AreEqual(expected.Tags.ToArray(), retrieved.Tags.ToArray());
        Assert.AreEqual(expected.Category, retrieved.Category);
        Assert.AreEqual(expected.SubCategory, retrieved.SubCategory);

        retrieved.SubCategory = "Changed";
        retrieved.Tags = new[] { "other" };

        var secondFetch = _sut.GetTileInfo(0);
        Assert.AreEqual("Boss", secondFetch.SubCategory);
        CollectionAssert.AreEqual(new[] { "lava", "flying" }, secondFetch.Tags.ToArray());
    }

    [TestMethod]
    public void Json2RoundtripPreservesMetadata()
    {
        _sut.SetTileInfo(0, new TileInfo
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

        var clonedInfo = clone.GetTileInfo(0);
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

    
}
