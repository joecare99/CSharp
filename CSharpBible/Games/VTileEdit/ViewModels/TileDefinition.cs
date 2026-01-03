using System;
using System.Collections.Generic;

namespace VTileEdit;

public class TileDefinition
{
    private GlyphData[,] _glyphs;

    public TileDefinition(string displayName, int width, int height, IEnumerable<GlyphData>? glyphs = null)
    {
        DisplayName = displayName;
        Width = width;
        Height = height;
        _glyphs = InitializeGrid(width, height, glyphs);
    }

    public string DisplayName { get; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public IEnumerable<GlyphData> Glyphs
    {
        get
        {
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    yield return _glyphs[row, column];
                }
            }
        }
    }

    public GlyphData GetGlyph(int row, int column)
        => _glyphs[row, column];

    public void SetGlyph(int row, int column, char character)
    {
        var glyph = _glyphs[row, column];
        _glyphs[row, column] = glyph with { Character = character };
    }

    public void SetForeground(int row, int column, ConsoleColor color)
    {
        var glyph = _glyphs[row, column];
        _glyphs[row, column] = glyph with { Foreground = color };
    }

    public void SetBackground(int row, int column, ConsoleColor color)
    {
        var glyph = _glyphs[row, column];
        _glyphs[row, column] = glyph with { Background = color };
    }

    public void ApplyDimensions(int width, int height)
    {
        var newGrid = new GlyphData[height, width];
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                if (row < Height && column < Width)
                {
                    newGrid[row, column] = _glyphs[row, column];
                }
                else
                {
                    newGrid[row, column] = GlyphData.CreateDefault(row, column);
                }
            }
        }

        Width = width;
        Height = height;
        _glyphs = newGrid;
    }

    public static TileDefinition CreatePattern(string name, int width, int height, Func<int, int, char> selector)
    {
        var glyphs = new List<GlyphData>(width * height);
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                glyphs.Add(new GlyphData(row, column, selector(row, column), ConsoleColor.Gray, ConsoleColor.Black));
            }
        }

        return new TileDefinition(name, width, height, glyphs);
    }

    private static GlyphData[,] InitializeGrid(int width, int height, IEnumerable<GlyphData>? glyphs)
    {
        var grid = new GlyphData[height, width];
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                grid[row, column] = GlyphData.CreateDefault(row, column);
            }
        }

        if (glyphs != null)
        {
            foreach (var glyph in glyphs)
            {
                if (glyph.Row >= 0 && glyph.Row < height && glyph.Column >= 0 && glyph.Column < width)
                {
                    grid[glyph.Row, glyph.Column] = glyph;
                }
            }
        }

        return grid;
    }
}
