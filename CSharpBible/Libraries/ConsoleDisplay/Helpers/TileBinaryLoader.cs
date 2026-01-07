using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace ConsoleDisplay.Helpers;

/// <summary>
/// Shared binary loader for tile data used by runtime and editor components.
/// </summary>
public static class TileBinaryLoader
{
    /// <summary>
    /// Reads tile data from the given stream and invokes callbacks for each tile and optional metadata.
    /// </summary>
    /// <param name="stream">Binary stream positioned at start.</param>
    /// <param name="setSize">Callback to set tile size.</param>
    /// <param name="addTile">Callback receiving key, lines, colors.</param>
    /// <param name="loadAdditionalData">Optional action to load any additional metadata after size, before tiles.</param>
    /// <param name="loadAdditionalTileData">Optional action to load tile-scoped metadata after each tile's color data.</param>
    public static void Load(
        Stream stream,
        Action<Size> setSize,
        Action<int, string[], (ConsoleColor fgr, ConsoleColor bgr)[]> addTile,
        Action<byte[]>? loadAdditionalData = null,
        Action<int, byte[]>? loadAdditionalTileData = null)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        var count = reader.ReadInt16();
        Size size = default;
        if (count > 0)
        {
            size = new Size(reader.ReadByte(), reader.ReadByte());
            setSize(size);
            if (TryReadBlob(reader, out var blob))
            {
                loadAdditionalData?.Invoke(blob);
            }
        }

        for (var i = 0; i < count; i++)
        {
            var key = reader.ReadInt32();
            var lineCount = reader.ReadByte();
            var lines = new string[lineCount];
            for (var j = 0; j < lineCount; j++)
            {
                lines[j] = reader.ReadString();
            }
            var colorCount = reader.ReadByte();
            var colors = new (ConsoleColor fgr, ConsoleColor bgr)[colorCount];
            for (var j = 0; j < colorCount; j++)
            {
                var b = reader.ReadByte();
                colors[j] = ((ConsoleColor)(b & 0xf), (ConsoleColor)(b >> 4));
            }

            addTile(key, lines, colors);
            if (TryReadBlob(reader, out var tileBlob))
            {
                loadAdditionalTileData?.Invoke(key, tileBlob);
            }
        }
    }

    private static bool TryReadBlob(BinaryReader reader, out byte[] blob)
    {
        blob = Array.Empty<byte>();
        if (reader.BaseStream.Length - reader.BaseStream.Position < sizeof(int))
        {
            return false;
        }

        var length = reader.ReadInt32();
        if (length <= 0)
        {
            return true;
        }

        if (reader.BaseStream.Length - reader.BaseStream.Position < length)
        {
            reader.BaseStream.Seek(-sizeof(int), SeekOrigin.Current);
            return false;
        }

        blob = reader.ReadBytes(length);
        return true;
    }
}
