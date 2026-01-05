using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace VTileEdit;

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
    public static void Load(Stream stream,
        Action<Size> setSize,
        Action<int, string[], (ConsoleColor fgr, ConsoleColor bgr)[]> addTile,
        Action<BinaryReader>? loadAdditionalData = null,
        Action<BinaryReader, int>? loadAdditionalTileData = null)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        var count = reader.ReadInt32();
        Size size = default;
        if (count > 0)
        {
            size = new Size(reader.ReadInt32(), reader.ReadInt32());
            _ = reader.ReadString(); // key type (unused here)
            setSize(size);
        }

        loadAdditionalData?.Invoke(reader);

        for (var i = 0; i < count; i++)
        {
            var key = reader.ReadInt32();
            var lineCount = reader.ReadInt32();
            var lines = new string[lineCount];
            for (var j = 0; j < lineCount; j++)
            {
                lines[j] = reader.ReadString();
            }
            var colorCount = reader.ReadInt32();
            var colors = new (ConsoleColor fgr, ConsoleColor bgr)[colorCount];
            for (var j = 0; j < colorCount; j++)
            {
                colors[j] = ((ConsoleColor)reader.ReadByte(), (ConsoleColor)reader.ReadByte());
            }

            loadAdditionalTileData?.Invoke(reader, key);
            addTile(key, lines, colors);
        }
    }
}
