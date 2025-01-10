using ConsoleDisplay.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace VTileEdit.Models;

public class VisTileData : ITileDef
{
    private Size _size;
    private Dictionary<Enum, SingleTile> _storage = new();

    public Size TileSize => _size;

    public static bool EqualTo<T>(IEnumerable<T> a, IEnumerable<T> b)
    {
        return a.Zip(b).All((t) => t.First!.Equals(t.Second));
    }



    public SingleTile GetTileDef(Enum? tile)
    {
        if (_storage.TryGetValue(tile, out var result))
        {
            return result;
        }
        else
        if (_storage.TryGetValue(_storage.Keys.FirstOrDefault((t) => (int)(object)t == (int)(object)tile), out result))
        {
            return result;
        }
        else
            return (new string[0], new (ConsoleColor fgr, ConsoleColor bgr)[0]);
    }


    public void SetTileDefs<T>(ITileDef tiledef) where T : Enum
    {
        Clear();
        _size = tiledef.TileSize;
        foreach (T e in Enum.GetValues(typeof(T)))
        {
            var tile = tiledef.GetTileDef(e);
            _storage.Add(e, tile);
        }
        for (var i = _storage.Count - 2; i > 0; i--)
        {
            var e0 = _storage.ElementAt(i).Value;
            var e1 = _storage.ElementAt(i + 1).Value;
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
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        Type _keyType = typeof(object);
                        int count = reader.ReadInt32();
                        if (count > 0)
                        {
                            _size = new Size(reader.ReadInt32(), reader.ReadInt32());
                            _keyType = Type.GetType(reader.ReadString()) ?? Assembly.GetExecutingAssembly().GetType();
                        }
                        for (int i = 0; i < count; i++)
                        {
                            Enum key = (Enum)Enum.ToObject(_keyType, reader.ReadInt32());
                            int lineCount = reader.ReadInt32();
                            string[] lines = new string[lineCount];
                            for (int j = 0; j < lineCount; j++)
                            {
                                lines[j] = reader.ReadString();
                            }
                            int colorCount = reader.ReadInt32();
                            FullColor[] colors = new FullColor[colorCount];
                            for (int j = 0; j < colorCount; j++)
                            {
                                colors[j] = ((ConsoleColor)reader.ReadByte(), (ConsoleColor)reader.ReadByte());
                            }
                            _storage.Add(key, new SingleTile(lines, colors));
                        }
                    }
                    return true;
                }
            case EStreamType.Text:
                {
                    int iSplPos;
                    (string key, string Value) kv(string line)
                        => (line.Substring(0, iSplPos = line.IndexOf(':')), line.Substring(iSplPos + 1));

                    Dictionary<Enum, SingleTile>? data = new();
                    using (TextReader reader = new StreamReader(stream))
                    {
                        string? line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var (key, value) = kv(line);
                            if (key == "Count")
                            {
                                data = new();
                                Type KeyType;
                                int count = int.Parse(value);
                                if (count > 0)
                                {
                                    (key, value) = kv(reader.ReadLine() ?? ":");
                                    var size = value.Split(',');
                                    _size = new Size(int.Parse(size[0]), int.Parse(size[1]));
                                    (key, value) = kv(reader.ReadLine() ?? ":");
                                    KeyType = Type.GetType(value);

                                    for (int i = 0; i < count; i++)
                                    {
                                        (key, value) = kv(reader.ReadLine() ?? ":");
                                        var keyE = (Enum)Enum.Parse(KeyType, value.Substring(0, value.IndexOf(' ') - 0));
                                        (key, value) = kv(reader.ReadLine() ?? ":");
                                        var lineCount = int.Parse(value);
                                        string[] lines = new string[lineCount];
                                        for (int j = 0; j < lineCount; j++)
                                        {
                                            (key, value) = kv(reader.ReadLine() ?? ":");
                                            lines[j] = value;
                                        }

                                        (key, value) = kv(reader.ReadLine() ?? ":");
                                        var colorCount = int.Parse(value);
                                        FullColor[] colors = new FullColor[colorCount];
                                        for (int j = 0; j < colorCount; j++)
                                        {
                                            (key, value) = kv(reader.ReadLine() ?? ":");
                                            var color = Convert.FromHexString(value.Substring(2));
                                            colors[j] = ((ConsoleColor)(color[0] >> 4), (ConsoleColor)(color[0] & 0xf));
                                        }
                                        data.Add(keyE, new SingleTile(lines, colors));

                                    }
                                }
                            }
                        }
                    }
                    _storage = data ?? _storage;
                    return data != null;
                }
            case EStreamType.Xml:
                {
                    Dictionary<Enum, SingleTile>? data = new();

                    using (TextReader reader = new StreamReader(stream))
                    {
                        var xml = new XmlSerializer(typeof(List<object[]>), extraTypes: [typeof(SingleTile)]);
                        var xdata = ((string KeyType, List<object[]> Data)?)xml.Deserialize(reader);
                        var _keyType = Type.GetType(xdata.Value.KeyType) ?? Assembly.GetExecutingAssembly().GetType();

                        foreach (var itm in xdata.Value.Data)
                        {
                            data.Add((Enum)Enum.ToObject(_keyType, (int)itm[0]), (SingleTile)itm[1]);
                        }
                    }
                    _storage = data ?? _storage;
                    return data != null;
                }
            case EStreamType.Json:
                {
                    Dictionary<Enum, SingleTile>? data = new();
                    var lst = JsonSerializer.Deserialize<(string, List<object[]>)>(new StreamReader(stream).ReadToEnd());
                    var _keyType = Type.GetType(lst.Item1) ?? Assembly.GetExecutingAssembly().GetType();
                    foreach (var itm in lst.Item2)
                    {
                        data.Add((Enum)Enum.ToObject(_keyType, itm[0]), (SingleTile)itm[1]);
                    }
                    _storage = data ?? _storage;
                    return data != null;
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
                        writer.Write(_storage.Count);
                        if (_storage.Count > 0)
                        {
                            writer.Write(_size.Width);
                            writer.Write(_size.Height);
                            writer.Write(_storage.First().Key.GetType().AssemblyQualifiedName);
                        }
                        foreach (var item in _storage)
                        {
                            writer.Write(Convert.ToInt32(item.Key));
                            writer.Write(item.Value.lines.Length);
                            foreach (var line in item.Value.lines)
                            {
                                writer.Write(line);
                            }
                            writer.Write(item.Value.colors.Length);
                            foreach (var color in item.Value.colors)
                            {
                                writer.Write((byte)color.fgr);
                                writer.Write((byte)color.bgr);
                            }
                        }
                    }
                    return true;
                }
            case EStreamType.Xml:
                {
                    using (TextWriter writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
                    {
                        Type _keyType = _storage.Count > 0 ? _storage.First().Key.GetType() : typeof(object);
                        (string KeyType, List<object[]> Data) data = (_keyType.AssemblyQualifiedName, _storage.Select(v => new object[] { v.Key, v.Value }).ToList());
                        var xml = new XmlSerializer(data.GetType(), [typeof(SingleTile)]);
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
                        if (_storage.Count > 0)
                        {
                            writer.WriteLine($"KeyType:{_storage.First().Key.GetType().AssemblyQualifiedName}");
                        }
                        for (int i = 0; i < _storage.Count; i++)
                        {
                            var item = _storage.ElementAt(i);
                            writer.WriteLine($"Key{i}:{item.Key} ({item.Key.GetType().Name})");
                            writer.WriteLine($"Lines{i}:{item.Value.lines.Length}");
                            for (var j = 0; j < item.Value.lines.Length; j++)
                            {
                                writer.WriteLine($"L{i}_{j}:{Quoted(item.Value.lines[j])}");
                            }
                            writer.WriteLine($"Colors:{item.Value.colors.Length}");
                            for (var j = 0; j < item.Value.colors.Length; j++)
                            {
                                var color = item.Value.colors[j];
                                writer.WriteLine($"L{i}_{j}:{ToNibble([color.fgr, color.bgr])}");
                            }
                        }
                    }
                    return true;
                }
            case EStreamType.Json:
                {
                    var json = JsonSerializer.Serialize(_storage.Select(v => new object[] { v.Key, v.Value }));
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

    public TileDef():base() => TileSize = new Size({3});
}}";
                    using (TextWriter writer = new StreamWriter(stream, leaveOpen: true))
                    {
                        string Brackeded(IEnumerable<string> @as, Func<string, string> Quoted) => $"[{Compress(string.Join(", ", @as.Select(Quoted)))}]";

                        var sLines = _storage.Select(v => Brackeded(v.Value.lines, Quoted) + $", //{v.Key}");
                        var sColors = _storage.Select(v => Brackeded(
                            v.Value.colors.Select(c => ToNibble([c.fgr, c.bgr])), (s) => s) + $", //{v.Key}");

                        writer.Write(string.Format(code, [
                            "JC-Soft",
                            string.Join($"{Environment.NewLine.PadRight(10,' ')}",sLines),
                            string.Join($"{Environment.NewLine.PadRight(10,' ')}",sColors),
                            $"{_size.Width},{_size.Height}"
                            ]));
                    }
                    return true;
                }
            default:
                return false;
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is VisTileData ot
            && _size == ot._size
            && _storage.Count == ot._storage.Count
            && _storage.All((t) => ot._storage.TryGetValue(t.Key, out var v) && t.Value.Equals(v));
    }

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

    private void Clear()
    {
        _storage.Clear();
    }

    (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) ITileDef.GetTileDef(Enum? tile)
        => GetTileDef(tile);
}

public record struct SingleTile(string[] lines, FullColor[] colors)
{
    public static implicit operator (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors)(SingleTile value)
    {
        return (value.lines, value.colors.Select(fc => (fc.fgr, fc.bgr)).ToArray());
    }

    public static implicit operator SingleTile((string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) value)
    {
        return new SingleTile(value.lines, value.colors.Select(fc => new FullColor(fc.fgr, fc.bgr)).ToArray());
    }

    public bool Equals(SingleTile other)
    {
        return lines.Length == other.lines.Length
            && lines.Zip(other.lines).All((t) => t.First!.Equals(t.Second))
            && colors.Length == other.colors.Length
            && colors.Zip(other.colors).All((t) => t.First!.Equals(t.Second));
    }
}

public record struct FullColor(ConsoleColor fgr, ConsoleColor bgr)
{
    public static implicit operator (ConsoleColor fgr, ConsoleColor bgr)(FullColor value)
    {
        return (value.fgr, value.bgr);
    }

    public static implicit operator FullColor((ConsoleColor fgr, ConsoleColor bgr) value)
    {
        return new FullColor(value.fgr, value.bgr);
    }

    public bool Equals(FullColor other)
    {
        return fgr == other.fgr && bgr == other.bgr;
    }
}