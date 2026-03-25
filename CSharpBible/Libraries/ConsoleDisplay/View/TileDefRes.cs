using ConsoleDisplay.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ConsoleDisplay.View;

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
            case string json when json.StartsWith("{"):
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
                TileBinaryLoader.Load(stream,
                    size => TileSize = size,
                    (key, lines, colors) => _tiles[key] = (lines, colors));
                break;
            case EStreamType.Json:
                LoadJson(stream);
                break;
            default:
                throw new NotSupportedException($"Stream type '{streamType}' not supported.");
        }
    }

    private void LoadJson(Stream stream)
    {
#if NET5_0_OR_GREATER
        using var sr = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
#else
        using var sr = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true, bufferSize: 1024);
#endif

        var json = sr.ReadToEnd();
        var payload = JsonSerializer.Deserialize< Tuple<string, Size, List<Tuple<int, SingleTile>>>>(json);
        if (payload == null)
        {
            throw new InvalidDataException("Unable to parse tile JSON.");
        }

        TileSize = payload.Item2;
        if (payload.Item3 == null)
        {
            return;
        }

        foreach (var entry in payload.Item3)
        {
            _tiles[entry.Item1] = (entry.Item2.lines, entry.Item2.colors.Select(f=> ((ConsoleColor, ConsoleColor))f).ToArray());
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

    public record struct SingleTile(string[] lines, FullColor[] colors);
    public record struct FullColor(ConsoleColor fgr, ConsoleColor bgr) {
        public static implicit operator (ConsoleColor fgr, ConsoleColor bgr)(FullColor value)
        => (value.fgr, value.bgr);

    }

}
