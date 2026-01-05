using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using ConsoleDisplay.View;
using VTileEdit;

namespace VTileEdit;

/// <summary>
/// Lightweight TileDef loader for apps; can be created from file path, JSON string, byte[] blob or stream.
/// </summary>
public sealed class TileDefRes : TileDefBase
{
    private readonly Dictionary<int, (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors)> _tiles = new();

    /// <summary>
    /// Initializes a new instance of <see cref="TileDefRes"/> using a file path, JSON string, byte[] blob or stream.
    /// </summary>
    /// <param name="source">File path, JSON string, byte[] blob or Stream containing tile data.</param>
    public TileDefRes(object source)
    {
        Load(source);
    }

    /// <inheritdoc />
    public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum? tile)
    {
        var key = tile != null ? Convert.ToInt32(tile) : 0;
        return _tiles.TryGetValue(key, out var def)
            ? def
            : (Array.Empty<string>(), Array.Empty<(ConsoleColor, ConsoleColor)>());
    }

    /// <summary>
    /// Loads/refreshes tiles from the given source.
    /// </summary>
    /// <param name="source">File path, JSON string, byte[] blob or Stream containing tile data.</param>
    public void Load(object source)
    {
        _tiles.Clear();
        switch (source)
        {
            case string text when File.Exists(text):
                using (var fs = File.OpenRead(text))
                {
                    LoadInternal(fs, ResolveStreamType(Path.GetExtension(text)));
                }
                break;
            case string json:
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    LoadInternal(ms, EStreamType.Json);
                }
                break;
            case byte[] blob:
                using (var ms = new MemoryStream(blob, writable: false))
                {
                    LoadInternal(ms, EStreamType.Binary);
                }
                break;
            case Stream stream:
                LoadInternal(stream, EStreamType.Binary);
                break;
            default:
                throw new ArgumentException("Unsupported tile source", nameof(source));
        }
    }

    private void LoadInternal(Stream stream, EStreamType streamType)
    {
        switch (streamType)
        {
            case EStreamType.Binary:
                LoadBinary(stream);
                break;
            case EStreamType.Json:
                LoadJson(stream);
                break;
            default:
                throw new NotSupportedException($"Stream type '{streamType}' not supported.");
        }
    }

    private void LoadBinary(Stream stream)
    {
        TileBinaryLoader.Load(stream,
            size => TileSize = size,
            (key, lines, colors) => _tiles[key] = (lines, colors));
    }

    private void LoadJson(Stream stream)
    {
        using var sr = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var json = sr.ReadToEnd();
        var payload = JsonSerializer.Deserialize<Json2Data>(json);
        if (payload == null)
        {
            throw new InvalidDataException("Unable to parse tile JSON.");
        }

        TileSize = new System.Drawing.Size(payload.TileWidth, payload.TileHeight);
        if (payload.Tiles == null)
        {
            return;
        }

        foreach (var entry in payload.Tiles)
        {
            var colors = entry.Colors.Select(DecodePackedColor).ToArray();
            _tiles[entry.Key] = (entry.Lines, colors);
        }
    }

    private static (ConsoleColor fgr, ConsoleColor bgr) DecodePackedColor(byte packed)
    {
        var bgr = (ConsoleColor)((packed >> 4) & 0x0f);
        var fgr = (ConsoleColor)(packed & 0x0f);
        return (fgr, bgr);
    }

    private static EStreamType ResolveStreamType(string extension)
        => extension.ToLowerInvariant() switch
        {
            ".tdf" => EStreamType.Binary,
            ".tdj" => EStreamType.Json,
            ".tdx" => EStreamType.Json,
            ".txt" => EStreamType.Json,
            ".cs" => EStreamType.Json,
             _ => EStreamType.Binary
         };

    private enum EStreamType { Binary, Json }

    private record Json2Data(string KeyType, int TileWidth, int TileHeight, List<Json2TileEntry> Tiles)
    {
        public record Json2TileEntry(int Key, string[] Lines, byte[] Colors);
    }
}
