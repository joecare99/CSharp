using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ConsoleDisplay.View.Tests;

/// <summary>
/// Defines tests for <see cref="TileDefRes"/>.
/// </summary>
[TestClass]
public class TileDefResTests
{
    /// <summary>
    /// Defines a test enum used to address tile keys.
    /// </summary>
    private enum TestTile
    {
        /// <summary>
        /// Represents the default tile key.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Represents the first test tile key.
        /// </summary>
        First = 1,

        /// <summary>
        /// Represents the second test tile key.
        /// </summary>
        Second = 2,

        /// <summary>
        /// Represents a missing tile key.
        /// </summary>
        Missing = 99,
    }

    /// <summary>
    /// Verifies that a JSON string source loads tile size and tile definitions.
    /// </summary>
    [TestMethod]
    public void ConstructorWithJsonStringLoadsTileDefinitions()
    {
        var json = CreateJsonPayload(
            new Size(2, 2),
            Tuple.Create(0, new TileDefRes.SingleTile(
                new[] { "<>", "\\/" },
                new[]
                {
                    new TileDefRes.FullColor(ConsoleColor.Yellow, ConsoleColor.DarkBlue),
                    new TileDefRes.FullColor(ConsoleColor.White, ConsoleColor.Black),
                    new TileDefRes.FullColor(ConsoleColor.Red, ConsoleColor.Gray),
                    new TileDefRes.FullColor(ConsoleColor.Green, ConsoleColor.DarkGreen),
                })),
            Tuple.Create(1, new TileDefRes.SingleTile(
                new[] { "[]", "()" },
                new[]
                {
                    new TileDefRes.FullColor(ConsoleColor.Cyan, ConsoleColor.Black),
                    new TileDefRes.FullColor(ConsoleColor.Magenta, ConsoleColor.DarkGray),
                    new TileDefRes.FullColor(ConsoleColor.Blue, ConsoleColor.White),
                    new TileDefRes.FullColor(ConsoleColor.DarkYellow, ConsoleColor.DarkRed),
                })));

        var sut = new TileDefRes(json);

        Assert.AreEqual(new Size(2, 2), sut.TileSize);
        AssertTileDef(
            sut.GetTileDef(TestTile.First),
            new[] { "[]", "()" },
            new[]
            {
                (ConsoleColor.Cyan, ConsoleColor.Black),
                (ConsoleColor.Magenta, ConsoleColor.DarkGray),
                (ConsoleColor.Blue, ConsoleColor.White),
                (ConsoleColor.DarkYellow, ConsoleColor.DarkRed),
            });
        AssertTileDef(
            sut.GetTileDef(null),
            new[] { "<>", "\\/" },
            new[]
            {
                (ConsoleColor.Yellow, ConsoleColor.DarkBlue),
                (ConsoleColor.White, ConsoleColor.Black),
                (ConsoleColor.Red, ConsoleColor.Gray),
                (ConsoleColor.Green, ConsoleColor.DarkGreen),
            });
    }

    /// <summary>
    /// Verifies that a missing tile key returns empty tile data.
    /// </summary>
    [TestMethod]
    public void GetTileDefWithMissingKeyReturnsEmptyArrays()
    {
        var sut = new TileDefRes(CreateJsonPayload(new Size(1, 1), Tuple.Create(1, new TileDefRes.SingleTile(
            new[] { "#" },
            new[] { new TileDefRes.FullColor(ConsoleColor.White, ConsoleColor.Black) }))));

        var result = sut.GetTileDef(TestTile.Missing);

        Assert.AreEqual(0, result.lines.Length);
        Assert.AreEqual(0, result.colors.Length);
    }

    /// <summary>
    /// Verifies that loading from a JSON file refreshes the cached tile data.
    /// </summary>
    [TestMethod]
    public void LoadWithJsonFileRefreshesTileCache()
    {
        var sut = new TileDefRes(CreateJsonPayload(new Size(1, 1), Tuple.Create(1, new TileDefRes.SingleTile(
            new[] { "A" },
            new[] { new TileDefRes.FullColor(ConsoleColor.White, ConsoleColor.Black) }))));

        var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tdj");
        try
        {
            File.WriteAllText(filePath, CreateJsonPayload(new Size(2, 1), Tuple.Create(2, new TileDefRes.SingleTile(
                new[] { "BC" },
                new[]
                {
                    new TileDefRes.FullColor(ConsoleColor.Red, ConsoleColor.Black),
                    new TileDefRes.FullColor(ConsoleColor.Green, ConsoleColor.DarkGray),
                }))));

            sut.Load(filePath);

            Assert.AreEqual(new Size(2, 1), sut.TileSize);
            Assert.AreEqual(0, sut.GetTileDef(TestTile.First).lines.Length, "Existing tiles should be cleared before reloading.");
            AssertTileDef(
                sut.GetTileDef(TestTile.Second),
                new[] { "BC" },
                new[]
                {
                    (ConsoleColor.Red, ConsoleColor.Black),
                    (ConsoleColor.Green, ConsoleColor.DarkGray),
                });
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    /// <summary>
    /// Verifies that a binary blob source loads tile size and tile definitions.
    /// </summary>
    [TestMethod]
    public void ConstructorWithBinaryBlobLoadsTileDefinitions()
    {
        var blob = CreateBinaryPayload(
            new Size(2, 1),
            Tuple.Create(1,
                new[] { "[]" },
                new[]
                {
                    (ConsoleColor.Blue, ConsoleColor.Black),
                    (ConsoleColor.White, ConsoleColor.DarkBlue),
                }));

        var sut = new TileDefRes(blob);

        Assert.AreEqual(new Size(2, 1), sut.TileSize);
        AssertTileDef(
            sut.GetTileDef(TestTile.First),
            new[] { "[]" },
            new[]
            {
                (ConsoleColor.Blue, ConsoleColor.Black),
                (ConsoleColor.White, ConsoleColor.DarkBlue),
            });
    }

    /// <summary>
    /// Verifies that a binary stream source loads tile size and tile definitions.
    /// </summary>
    [TestMethod]
    public void ConstructorWithBinaryStreamLoadsTileDefinitions()
    {
        var stream = new MemoryStream(CreateBinaryPayload(
            new Size(1, 2),
            Tuple.Create(2,
                new[] { "X", "Y" },
                new[]
                {
                    (ConsoleColor.Gray, ConsoleColor.Black),
                    (ConsoleColor.DarkGray, ConsoleColor.White),
                })));

        var sut = new TileDefRes(stream);

        Assert.AreEqual(new Size(1, 2), sut.TileSize);
        AssertTileDef(
            sut.GetTileDef(TestTile.Second),
            new[] { "X", "Y" },
            new[]
            {
                (ConsoleColor.Gray, ConsoleColor.Black),
                (ConsoleColor.DarkGray, ConsoleColor.White),
            });
        Assert.IsTrue(stream.CanRead, "Loading from an external stream should not dispose the caller-owned stream.");
    }

    /// <summary>
    /// Verifies that unsupported sources are rejected.
    /// </summary>
    [TestMethod]
    public void LoadWithUnsupportedSourceThrowsArgumentException()
    {
        var sut = new TileDefRes(CreateJsonPayload(new Size(1, 1)));

        Assert.ThrowsExactly<ArgumentException>(() => sut.Load(42));
    }

    /// <summary>
    /// Creates a JSON payload compatible with <see cref="TileDefRes"/>.
    /// </summary>
    /// <param name="size">The tile size to serialize.</param>
    /// <param name="tiles">The tile definitions to serialize.</param>
    /// <returns>A JSON payload string.</returns>
    private static string CreateJsonPayload(Size size, params Tuple<int, TileDefRes.SingleTile>[] tiles)
    {
        return JsonSerializer.Serialize(Tuple.Create("tiles", size, new List<Tuple<int, TileDefRes.SingleTile>>(tiles)));
    }

    /// <summary>
    /// Creates a binary payload compatible with <see cref="TileDefRes"/>.
    /// </summary>
    /// <param name="size">The tile size to serialize.</param>
    /// <param name="tiles">The tile definitions to serialize.</param>
    /// <returns>A binary payload.</returns>
    private static byte[] CreateBinaryPayload(Size size, params Tuple<int, string[], (ConsoleColor fgr, ConsoleColor bgr)[]>[] tiles)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write((short)tiles.Length);
        writer.Write((byte)size.Width);
        writer.Write((byte)size.Height);
        writer.Write(0);
        foreach (var tile in tiles)
        {
            writer.Write(tile.Item1);
            writer.Write((byte)tile.Item2.Length);
            foreach (var line in tile.Item2)
            {
                writer.Write(line);
            }

            writer.Write((byte)tile.Item3.Length);
            foreach (var color in tile.Item3)
            {
                writer.Write((byte)(((int)color.bgr << 4) | ((int)color.fgr & 0x0f)));
            }

            writer.Write(0);
        }

        writer.Flush();
        return stream.ToArray();
    }

    /// <summary>
    /// Asserts that an actual tile definition matches the expected content.
    /// </summary>
    /// <param name="actual">The actual tile definition.</param>
    /// <param name="expectedLines">The expected lines.</param>
    /// <param name="expectedColors">The expected colors.</param>
    private static void AssertTileDef(
        (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) actual,
        string[] expectedLines,
        (ConsoleColor fgr, ConsoleColor bgr)[] expectedColors)
    {
        CollectionAssert.AreEqual(expectedLines, actual.lines);
        Assert.AreEqual(expectedColors.Length, actual.colors.Length, "Color array length mismatch.");
        for (var i = 0; i < expectedColors.Length; i++)
        {
            Assert.AreEqual(expectedColors[i].fgr, actual.colors[i].fgr, $"Foreground mismatch at index {i}.");
            Assert.AreEqual(expectedColors[i].bgr, actual.colors[i].bgr, $"Background mismatch at index {i}.");
        }
    }
}
