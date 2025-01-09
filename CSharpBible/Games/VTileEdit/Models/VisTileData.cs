using ConsoleDisplay.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VTileEdit.Models;

public class VisTileData : ITileDef
{
    private Size _size;
    private Dictionary<Enum, (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors)> _storage = new();

    public Size TileSize => _size;

    public (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum? tile)
    {
        if (_storage.TryGetValue(tile, out var result))
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
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            Enum key = (Enum)Enum.ToObject(typeof(Enum), reader.ReadInt32());
                            int lineCount = reader.ReadInt32();
                            string[] lines = new string[lineCount];
                            for (int j = 0; j < lineCount; j++)
                            {
                                lines[j] = reader.ReadString();
                            }
                            int colorCount = reader.ReadInt32();
                            (ConsoleColor fgr, ConsoleColor bgr)[] colors = new (ConsoleColor fgr, ConsoleColor bgr)[colorCount];
                            for (int j = 0; j < colorCount; j++)
                            {
                                colors[j] = ((ConsoleColor)reader.ReadByte(), (ConsoleColor)reader.ReadByte());
                            }
                            _storage.Add(key, (lines, colors));
                        }
                    }
                    return true;
                }
            case EStreamType.Text:
                {
                    Dictionary<Enum, (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors)>? data = _storage;
                    using (TextReader reader = new StreamReader(stream))
                    {
                        var xml = new XmlSerializer(data.GetType());
                        data = (Dictionary<Enum, (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors)>?)xml.Deserialize(reader);
                    }
                    _storage = data ?? _storage;
                    return data != null;
                }
            case EStreamType.Json:
                {
                    var data = JsonSerializer.Deserialize<Dictionary<Enum, (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors)>>(new StreamReader(stream).ReadToEnd());
                    _storage = data ?? _storage;
                    return data != null;
                }
            default:
                return false;
        }
    }

    public bool WriteToStream(Stream stream, EStreamType eStreamType)
    {
        switch (eStreamType)
        {
            case EStreamType.Binary:
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(_storage.Count);
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
            case EStreamType.Text:
                {
                    using (TextWriter writer = new StreamWriter(stream))
                    {
                        var xml = new XmlSerializer(_storage.GetType());
                        xml.Serialize(writer, _storage);
                    }
                    return true;
                }
            case EStreamType.Json:
                {
                    var json = JsonSerializer.Serialize(_storage);
                    using (TextWriter writer = new StreamWriter(stream))
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
/// Implements the <see cref=""TileDef{Tiles}"" /></summary>
/// <seealso cref=""TileDef{Tiles}"" />
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

    ];

    /// <summary>
    /// Gets the tile definition.
    /// </summary>
    /// <param name=""tile"">The tile.</param>
    /// <returns>(string[] lines, (System.ConsoleColor fgr, System.ConsoleColor bgr)[] colors).</returns>
    /// <autogeneratedoc />
    public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum tile)
    {{
        (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) result = default;
        result.lines = GetArrayElement(_vTileDefStr, tile);

        result.colors = new (ConsoleColor fgr, ConsoleColor bgr)[result.lines.Length * result.lines[0].Length];
        byte[] colDef = GetArrayElement(_vTileColors, tile);
        for (var i = 0; i < result.lines.Length * result.lines[0].Length; i++)
            result.colors[i] = ByteTo2ConsColor(colDef[i % colDef.Length]);
        return result;
    }}

    public TileDef():base() {{
        TileSize = new Size({3});
    }}
}}";
                    using (TextWriter writer = new StreamWriter(stream))
                    {
                        string Quoted(string s) => $"@\"{s.Replace("\"", "\"\"")}\"";
                        string Brackeded(IEnumerable<string> @as) => $"[{string.Join(",", @as.Select(Quoted))}]";
                        string ToNibble(ConsoleColor[] c) => $"0x{(byte)c[0] & 0xf:X1}{(byte)c[1] & 0xf:X1}";

                        var sLines = _storage.Values.Select(v => Brackeded(v.lines));
                        var sColors = _storage.Values.Select(v => Brackeded(
                            v.colors.Select(c => ToNibble([c.fgr, c.bgr]))));

                        writer.Write(string.Format(code, [
                            "JC-Soft",
                            string.Join($",{Environment.NewLine.PadRight(8,' ')}",sLines),
                            string.Join($",{Environment.NewLine.PadRight(8,' ')}",sColors),
                            $"{_size.Width},{_size.Height}"
                            ]));
                    }
                    return true;
                }
            default:
                return false;
        }
    }

    private void Clear()
    {
        _storage.Clear();
    }
}
