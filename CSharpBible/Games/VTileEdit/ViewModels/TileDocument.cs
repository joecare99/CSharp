using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace VTileEdit;

/// <summary>
/// UI-agnostic workspace for managing a tile set and glyph data.
/// </summary>
public class TileDocument
{
    private readonly ObservableCollection<TileDefinition> _tiles;
    private readonly ReadOnlyCollection<char> _characterPalette;

    public TileDocument(string name, int tileWidth, int tileHeight, IEnumerable<TileDefinition>? tiles = null)
    {
        Name = NormalizeName(name);
        TileWidth = NormalizeDimension(tileWidth);
        TileHeight = NormalizeDimension(tileHeight);
        _tiles = new ObservableCollection<TileDefinition>(tiles ?? Enumerable.Empty<TileDefinition>());
        _characterPalette = BuildCharacterPalette();
    }

    public string Name { get; private set; }

    public int TileWidth { get; private set; }

    public int TileHeight { get; private set; }

    public ObservableCollection<TileDefinition> Tiles => _tiles;

    public ReadOnlyCollection<char> CharacterPalette => _characterPalette;

    public TileDefinition CreateTile(int id, string displayName)
    {
        var tile = new TileDefinition(id, displayName, TileWidth, TileHeight);
        _tiles.Add(tile);
        return tile;
    }

    public void ApplyDimensions(int width, int height)
    {
        TileWidth = NormalizeDimension(width);
        TileHeight = NormalizeDimension(height);
        foreach (var tile in _tiles)
        {
            tile.ApplyDimensions(TileWidth, TileHeight);
        }
    }

    public void Rename(string name) => Name = NormalizeName(name);

    public void SetGlyphCharacter(TileDefinition tile, int row, int column, char character) => tile.SetGlyph(row, column, character);

    public void SetForeground(TileDefinition tile, int row, int column, ConsoleColor color) => tile.SetForeground(row, column, color);

    public void SetBackground(TileDefinition tile, int row, int column, ConsoleColor color) => tile.SetBackground(row, column, color);

    public static TileDocument CreateSample(int defaultWidth, int defaultHeight)
    {
        var checker = TileDefinition.CreatePattern(0, "Checker", defaultWidth, defaultHeight, (row, column) => (row + column) % 2 == 0 ? '#' : '.');
        var borders = TileDefinition.CreatePattern(1, "Borders", defaultWidth, defaultHeight, (row, column) => row == 0 || column == 0 || row == defaultHeight - 1 || column == defaultWidth - 1 ? '+' : ' ');
        return new TileDocument("Tile Set", defaultWidth, defaultHeight, new[] { checker, borders });
    }

    private static int NormalizeDimension(int value)
        => value <= 0 ? 1 : value;

    private static string NormalizeName(string name)
        => string.IsNullOrWhiteSpace(name) ? "Tile Set" : name.Trim();

    private static ReadOnlyCollection<char> BuildCharacterPalette()
    {
        var encoding = Encoding.GetEncoding(437);
        var buffer = new byte[1];
        var decorative = DecorativeCp437Glyphs;
        var glyphs = new char[256];

        for (var i = 0; i < glyphs.Length; i++)
        {
            buffer[0] = (byte)i;
            var glyph = encoding.GetChars(buffer)[0];
            if (i < decorative.Length)
            {
                glyph = decorative[i];
            }

            glyphs[i] = glyph;
        }

        return Array.AsReadOnly(glyphs);
    }

    private static readonly char[] DecorativeCp437Glyphs =
    {
        ' ', '☺', '☻', '♥', '♦', '♣', '♠', '•', '◘', '○', '◙', '♂', '♀', '♪', '♫', '☼',
        '►', '◄', '↕', '‼', '¶', '§', '▬', '↨', '↑', '↓', '→', '←', '∟', '↔', '▲', '▼'
    };
}

public record struct GlyphData(int Row, int Column, char Character, ConsoleColor Foreground, ConsoleColor Background)
{
    public static GlyphData CreateDefault(int row, int column)
        => new(row, column, ' ', ConsoleColor.Gray, ConsoleColor.Black);
}
