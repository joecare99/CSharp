using BaseLib.Helper;
using ConsoleDisplay.Helpers;
using ConsoleDisplay.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace VTileEdit.Models;

public class VisTileData : ITileDef
{
    private sealed class TileEntry
    {
        public TileEntry(SingleTile tile, TileInfo info)
        {
            Tile = tile;
            Info = info;
        }

        public SingleTile Tile { get; set; }

        public TileInfo Info { get; set; }
    }

    private Size _size;
    private Dictionary<int, TileEntry> _storage = new();

    public Size TileSize => _size;

    // Expose current keys for selection in UI
    public IEnumerable<int> Keys => _storage.Keys;

    // Expose key type used by storage
    public Type KeyType { get; set; }

    public static bool EqualTo<T>(IEnumerable<T> a, IEnumerable<T> b)
    {
        return a.Zip(b).All((t) => t.First!.Equals(t.Second));
    }

    public SingleTile GetTileDef(int? tile)
    {
        if (tile != null && _storage.TryGetValue(tile.Value, out var result))
        {
            return result.Tile;
        }

        if (tile != null)
        {
            var numericValue = Convert.ToInt32(tile);
            foreach (var entry in _storage)
            {
                if (Convert.ToInt32(entry.Key) == numericValue)
                {
                    return entry.Value.Tile;
                }
            }
        }

        return new SingleTile(Array.Empty<string>(), Array.Empty<FullColor>());
    }

    // Allow external editor to change tile-size
    public void SetTileSize(Size size) => _size = size;

    // Allow external editor to set/replace a tile definition
    public void SetTileDef(int key, SingleTile tile)
    {
        if (_storage.TryGetValue(key, out var entry))
        {
            entry.Tile = tile;
        }
        else
        {
            _storage[key] = new TileEntry(tile, TileInfo.Default);
        }
    }

    public TileInfo GetTileInfo(int? tile)
    {
        if (tile != null && _storage.TryGetValue(tile.Value, out var entry))
        {
            return entry.Info.Clone();
        }

        if (tile != null)
        {
            var numericValue = Convert.ToInt32(tile);
            foreach (var candidate in _storage)
            {
                if (Convert.ToInt32(candidate.Key) == numericValue)
                {
                    return candidate.Value.Info.Clone();
                }
            }
        }

        return TileInfo.Default;
    }

    public void SetTileInfo(int key, TileInfo info)
    {
        var normalized = TileInfo.Normalize(info);

        if (_storage.TryGetValue(key, out var entry))
        {
            entry.Info = normalized;
        }
        else
        {
            _storage[key] = new TileEntry(new SingleTile(Array.Empty<string>(), Array.Empty<FullColor>()), normalized);
        }
    }

    public void SetTileDefs<T>(ITileDef tiledef) where T : Enum
    {
        Clear();
        _size = tiledef.TileSize;
        KeyType = typeof(T);
        foreach (T e in Enum.GetValues(typeof(T)))
        {
            var tile = tiledef.GetTileDef(e);
            _storage.Add(e.AsInt(), new TileEntry(tile, new TileInfo() { Name = $"{e}" }));
        }
        for (var i = _storage.Count - 2; i > 0; i--)
        {
            var e0 = _storage.ElementAt(i).Value.Tile;
            var e1 = _storage.ElementAt(i + 1).Value.Tile;
            if (e0.lines.Length == e1.lines.Length
                && EqualTo(e0.lines, e1.lines)
                && e0.colors.Length == e1.colors.Length
                && EqualTo(e0.colors.Select((c) => (int)c.fgr + (int)c.bgr * 256), e1.colors.Select((c) => (int)c.fgr + (int)c.bgr * 256)))
                _storage.Remove(_storage.ElementAt(i + 1).Key);
            else
                break;
        }
    }

    public bool LoadFromStream(Stream stream, EStreamType eStreamType)
    {
        switch (eStreamType)
        {
            case EStreamType.Binary:
                {
                    _storage.Clear();
                    TileBinaryLoader.Load(stream,
                        size => _size = size,
                        (key, lines, colors) => _storage.Add(key, new TileEntry(new SingleTile(lines, colors.Select(c => new FullColor(c.fgr, c.bgr)).ToArray()), TileInfo.Default)),
                        (blob) => { var KeyTypeStr = Encoding.UTF8.GetString(blob); KeyType = Type.GetType(KeyTypeStr) ?? typeof(int); }
                        );
                    return true;
                }
            case EStreamType.Text:
                {
                    int iSplPos;

                    Dictionary<int, TileEntry>? data = new();
                    using (TextReader reader = new StreamReader(stream))
                    {
                        string? line;
                        Dictionary<string, string> keyValuePairs = new();
                        while ((line = reader.ReadLine()) != null)
                        {
                            var (key, value2) = (line.Substring(0, iSplPos = line.IndexOf(':')), line.Substring(iSplPos + 1));
                            keyValuePairs.Add(key, value2);
                        }
                        if (keyValuePairs.TryGetValue("Count", out var value))
                        {
                            data = new();
                            int count = int.Parse(value);
                            if (count > 0)
                            {
                                if (keyValuePairs.TryGetValue("Size", out value))
                                {
                                    var size = value.Split(',');
                                    _size = new Size(int.Parse(size[0]), int.Parse(size[1]));
                                }
                                for (int i = 0; i < count; i++)
                                {
                                    int? keyE = default;
                                    int lineCount = 0;
                                    if (keyValuePairs.TryGetValue($"Key{i}", out value)  )
                                    {
                                        keyE = int.Parse(value.Substring(0, value.IndexOf(' ') - 0));
                                    }
                                    if (keyValuePairs.TryGetValue($"Lines{i}", out value))
                                    {
                                        lineCount = int.Parse(value);
                                    }

                                    string[] lines = new string[lineCount];
                                    for (int j = 0; j < lineCount; j++)
                                    {
                                        if (keyValuePairs.TryGetValue($"L{i}_{j}", out value))
                                            lines[j] = value;
                                    }
                                    int colorCount = 0;
                                    if (keyValuePairs.TryGetValue($"Colors{i}", out value))
                                    {
                                        colorCount = int.Parse(value);
                                    }
                                    FullColor[] colors = new FullColor[colorCount];
                                    for (int j = 0; j < colorCount; j++)
                                    {
                                        if (keyValuePairs.TryGetValue($"C{i}_{j}", out value))
                                        {
                                            var color = Convert.FromHexString(value.Substring(2));
                                            colors[j] = ((ConsoleColor)(color[0] >> 4), (ConsoleColor)(color[0] & 0xf));
                                        }
                                    }

                                    data.Add(keyE.Value, new TileEntry(new SingleTile(lines, colors), TileInfo.Default));

                                }
                            }
                        }

                    }
                    _storage = data ?? _storage;
                    return data != null;
                }
            case EStreamType.Xml:
                {
                    Dictionary<int, TileEntry>? data = new();

                    using (TextReader reader = new StreamReader(stream))
                    {
                        var xml = new XmlSerializer(typeof((string, Size, List<object[]>)), extraTypes: [typeof(SingleTile)]);
                        var xdata = ((string KeyType, Size sz, List<object[]> Data)?)xml.Deserialize(reader);
                        _size = xdata.Value.sz;
                        foreach (var itm in xdata.Value.Data)
                        {
                            data.Add((int)itm[0], new TileEntry((SingleTile)itm[1], TileInfo.Default));
                        }
                    }
                    _storage = data ?? _storage;
                    return data != null;
                }
            case EStreamType.Json:
                {
                    Dictionary<int, TileEntry>? data = new();
                    var lst = JsonSerializer.Deserialize<Tuple<string, Size, List<Tuple<int, SingleTile>>>>(new StreamReader(stream).ReadToEnd());
                    _size = lst.Item2;
                    foreach (var itm in lst.Item3)
                    {
                        data.Add(itm.Item1, new TileEntry(itm.Item2, TileInfo.Default));
                    }

                    _storage = data ?? _storage;
                    return data != null;
                }
            case EStreamType.Json2:
                {
                    Dictionary<int, TileEntry>? data = new();
                    var payload = JsonSerializer.Deserialize<Json2Data>(new StreamReader(stream).ReadToEnd());
                    if (payload == null)
                    {
                        return false;
                    }

                    var keyType = Type.GetType(payload.KeyType) ?? Assembly.GetExecutingAssembly().GetType();
                    _size = new Size(payload.TileWidth, payload.TileHeight);
                    if (payload.Tiles == null)
                    {
                        _storage.Clear();
                        return true;
                    }
                     foreach (var entry in payload.Tiles)
                     {
                        var key = entry.Key;
                        var colors = entry.Colors.Select(DecodePackedColor).ToArray();
                        var info = TileInfo.Normalize(new TileInfo
                        {
                            Category = entry.Category,
                            SubCategory = entry.SubCategory ?? string.Empty,
                            Tags = (entry.Tags ?? Array.Empty<string>()).ToArray()
                        });

                        data.Add(key, new TileEntry(new SingleTile(entry.Lines, colors), info));
                    }

                    _storage = data ?? _storage;
                    return true;
                }
            default:
                return false;
        }
    }

    public bool WriteToStream(Stream stream, EStreamType eStreamType)
    {
        string Quoted(string s) => $"@\"{s.Replace("\"", "\"\"")}\"";
        string ToNibble(ConsoleColor[] c) => $"0x{(byte)c[1] & 0xf:X1}{(byte)c[0] & 0xf:X1}";

        switch (eStreamType)
        {
            case EStreamType.Binary:
                {
                    using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true))
                    {
                        writer.Write((short)_storage.Count);
                        if (_storage.Count > 0)
                        {
                            writer.Write((byte)_size.Width);
                            writer.Write((byte)_size.Height);
                            // Additional Data
                            var blob = Encoding.UTF8.GetBytes(KeyType?.AssemblyQualifiedName);
                            writer.Write(blob.Length);
                            writer.Write(blob);
                        }
                        foreach (var item in _storage)
                        {
                            writer.Write(Convert.ToInt32(item.Key));
                            writer.Write((byte)item.Value.Tile.lines.Length);
                            foreach (var line in item.Value.Tile.lines)
                            {
                                writer.Write(line);
                            }
                            writer.Write((byte)item.Value.Tile.colors.Length);
                            foreach (var color in item.Value.Tile.colors)
                            {
                                byte b = (byte)(((byte)color.bgr << 4) + (byte)color.fgr);
                                writer.Write(b);
                            }
                            // Additional Data
                            var blob = Encoding.UTF8.GetBytes(item.Value.Info.Name);
                            writer.Write(blob.Length);
                            writer.Write(blob);
                        }
                    }
                    return true;
                }
            case EStreamType.Xml:
                {
                    using (TextWriter writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
                    {
                        (string KeyType, Size sz, List<object[]> Data) data = (KeyType?.AssemblyQualifiedName, _size, _storage.Select(v => new object[] { v.Key, v.Value.Tile }).ToList());
                        var xml = new XmlSerializer(data.GetType(), [typeof(SingleTile), typeof(Size)]);
                        xml.Serialize(writer, data);
                    }
                    return true;
                }
            case EStreamType.Text:
                {
                    using (TextWriter writer = new StreamWriter(stream, leaveOpen: true))
                    {
                        writer.WriteLine($"Count:{_storage.Count}");
                        writer.WriteLine($"Size:{_size.Width},{_size.Height}");
                        writer.WriteLine($"KeyType:{KeyType.AssemblyQualifiedName}");
                        for (int i = 0; i < _storage.Count; i++)
                        {
                            var item = _storage.ElementAt(i);
                            writer.WriteLine($"Key{i}:{item.Key} ({item.Value.Info.Name})");
                            writer.WriteLine($"Lines{i}:{item.Value.Tile.lines.Length}");
                            for (var j = 0; j < item.Value.Tile.lines.Length; j++)
                            {
                                writer.WriteLine($"L{i}_{j}:{Quoted(item.Value.Tile.lines[j])}");
                            }
                            writer.WriteLine($"Colors{i}:{item.Value.Tile.colors.Length}");
                            for (var j = 0; j < item.Value.Tile.colors.Length; j++)
                            {
                                var color = item.Value.Tile.colors[j];
                                writer.WriteLine($"C{i}_{j}:{ToNibble([color.fgr, color.bgr])}");
                            }
                        }
                    }
                    return true;
                }
            case EStreamType.Json:
                {
                    Tuple<string, Size, List<Tuple<int, SingleTile>>> data = new(KeyType?.AssemblyQualifiedName, _size, _storage.Select(v => new Tuple<int, SingleTile>(v.Key, v.Value.Tile)).ToList());
                    var json = JsonSerializer.Serialize(data);
                    using (TextWriter writer = new StreamWriter(stream, leaveOpen: true))
                    {
                        writer.Write(json);
                    }
                    return true;
                }
            case EStreamType.Json2:
                {
                    Type _keyType = _storage.Count > 0 ? _storage.First().Key.GetType() : typeof(object);
                    Json2Data data = new(
                        _keyType.AssemblyQualifiedName,
                        _size.Width,
                        _size.Height,
                        _storage.Select(v =>
                        {
                            var tile = v.Value.Tile;
                            var info = v.Value.Info;
                            var tags = (info.Tags?.Count ?? 0) > 0
                                ? info.Tags
                                : new[] { $"{v.Key}" };

                            return new Json2Data.TileEntry(
                                (int)(object)v.Key,
                                tags.ToArray(),
                                tile.lines,
                                tile.colors.Select(c => (byte)(((int)c.bgr) * 16 + (byte)c.fgr)).ToArray(),
                                info.Category,
                                string.IsNullOrWhiteSpace(info.SubCategory) ? null : info.SubCategory);
                        }).ToList());
                     var json = JsonSerializer.Serialize(data);
                     using (TextWriter writer = new StreamWriter(stream, leaveOpen: true))
                     {
                         writer.Write(json);
                     }
                     return true;
                }
            case EStreamType.Code:
                {
                    const string code = @"//***************************************************************
// Assembly         : SomeGame_Base
// Author           : Mir
// Created          : 01-01-2025
//
// Last Modified By : Mir
// Last Modified On : 01-01-2025
// ***********************************************************************
// <copyright file=""VTileDef.cs"" company=""{0}"">
//     Copyright (c) {0}. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using ConsoleDisplay.View;
using System.Drawing;

/// <summary>
/// The View namespace.
/// </summary>
/// <autogeneratedoc />
namespace Console.Views;

/// <summary>
/// Class TileDef.
/// Implements the <see cref=""TileDef{{Tiles}}"" /></summary>
/// <seealso cref=""TileDef{{Tiles}}"" />
public class TileDef : TileDefBase
{{
    /// <summary>
    /// The tile definition string
    /// </summary>
    private readonly string[][] _vTileDefStr = [
        {1}
    ];


    /// <summary>
    /// The tile colors
    /// </summary>
    private readonly byte[][] _vTileColors = [
        {2}
    ];

    /// <summary>
    /// Gets the tile definition.
    /// </summary>
    /// <param name=""tile"">The tile.</param>
    /// <returns>(string[] lines, (System.ConsoleColor fgr, System.ConsoleColor bgr)[] colors).</returns>
    /// <autogeneratedoc />
    public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum? tile)
    {{
        (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) result = default;
        result.lines = GetArrayElement(_vTileDefStr, tile);

        result.colors = new (ConsoleColor fgr, ConsoleColor bgr)[result.lines.Length * result.lines[0].Length];
        byte[] colDef = GetArrayElement(_vTileColors, tile);
        for (var i = 0; i < result.lines.Length * result.lines[0].Length; i++)
            result.colors[i] = ByteTo2ConsColor(colDef[i % colDef.Length]);
        return result;
    }}

    public TileDef() : base() => TileSize = new Size({3});
}}";
                    using (TextWriter writer = new StreamWriter(stream, leaveOpen: true))
                    {
                        string Brackeded(IEnumerable<string> @as, Func<string, string> Quoted) => $"[{Compress(string.Join(", ", @as.Select(Quoted)))}]";

                        var sLines = _storage.Select(v => Brackeded(v.Value.Tile.lines, Quoted) + $", //{v.Value.Info.Name}");
                        var sColors = _storage.Select(v => Brackeded(
                            v.Value.Tile.colors.Select(c => ToNibble([c.fgr, c.bgr])), (s) => s) + $", //{v.Value.Info.Name}");

                        writer.Write(string.Format(code, [
                            "JC-Soft",
                            string.Join($"{Environment.NewLine.PadRight(10,' ')}",sLines),
                            string.Join($"{Environment.NewLine.PadRight(10,' ')}",sColors),
                            $"{_size.Width}, {_size.Height}"
                            ]));
                    }
                    return true;
                }
            default:
                return false;
        }
    }

    private static FullColor DecodePackedColor(byte packed)
    {
        var bgr = (ConsoleColor)((packed >> 4) & 0x0f);
        var fgr = (ConsoleColor)(packed & 0x0f);
        return new FullColor(fgr, bgr);
    }

    public override bool Equals(object? obj)
        => obj is VisTileData ot
            && _size == ot._size
            && _storage.Count == ot._storage.Count
            && _storage.All((t) =>
                ot._storage.TryGetValue(t.Key, out var v)
                && t.Value.Tile.Equals(v.Tile)
                && MetadataEquals(t.Value.Info, v.Info));

    private string Compress(string s)
    {
        while (s.Length > 3
            && s.Substring(s.Length / 2 - 1, 2) == ", "
            && s.Substring(0, s.Length / 2 - 1) == s.Substring(s.Length / 2 + 1, s.Length / 2 - 1))
        {
            s = s.Substring(0, s.Length / 2 - 1);
        }
        return s;
    }

    private static bool MetadataEquals(TileInfo left, TileInfo right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left == null || right == null)
        {
            return false;
        }

        if (left.Category != right.Category)
        {
            return false;
        }

        if (!string.Equals(left.SubCategory, right.SubCategory, StringComparison.Ordinal))
        {
            return false;
        }

        var leftTags = left.Tags ?? Array.Empty<string>();
        var rightTags = right.Tags ?? Array.Empty<string>();

        if (leftTags.Count != rightTags.Count)
        {
            return false;
        }

        for (var i = 0; i < leftTags.Count; i++)
        {
            if (!string.Equals(leftTags[i], rightTags[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    public void Clear()
        => _storage.Clear();

    (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) ITileDef.GetTileDef(Enum? tile)
        => GetTileDef(tile.AsInt());
}

public record struct SingleTile(string[] lines, FullColor[] colors)
{
    public static implicit operator (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors)(SingleTile value)
        => (value.lines, value.colors.Select(fc => (fc.fgr, fc.bgr)).ToArray());

    public static implicit operator SingleTile((string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) value)
        => new SingleTile(value.lines, value.colors.Select(fc => new FullColor(fc.fgr, fc.bgr)).ToArray());

    public bool Equals(SingleTile other)
        => lines.Length == other.lines.Length
            && lines.Zip(other.lines).All((t) => t.First!.Equals(t.Second))
            && colors.Length == other.colors.Length
            && colors.Zip(other.colors).All((t) => t.First!.Equals(t.Second));
}

public record struct FullColor(ConsoleColor fgr, ConsoleColor bgr)
{
    public static implicit operator (ConsoleColor fgr, ConsoleColor bgr)(FullColor value)
        => (value.fgr, value.bgr);

    public static implicit operator FullColor((ConsoleColor fgr, ConsoleColor bgr) value)
        => new FullColor(value.fgr, value.bgr);

    public bool Equals(FullColor other)
        => fgr == other.fgr && bgr == other.bgr;
}

public record Json2Data(
    string KeyType,
    int TileWidth,
    int TileHeight,
    List<Json2Data.TileEntry> Tiles)
{
    public record TileEntry(
        int Key,
        IList<string>? Tags,
        string[] Lines,
        byte[] Colors,
        TileCategory Category = TileCategory.Unknown,
        string? SubCategory = null);
}
