using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VTileEdit.WPF.ViewModels;

/// <summary>
/// Represents a tile surface that can be edited within the UI.
/// </summary>
public partial class TileViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TileViewModel"/> class.
    /// </summary>
    /// <param name="displayName">The user facing display name.</param>
    /// <param name="tileWidth">The tile width.</param>
    /// <param name="tileHeight">The tile height.</param>
    /// <param name="glyphs">The glyph collection describing the tile.</param>
    public TileViewModel(string displayName, int tileWidth, int tileHeight, IEnumerable<GlyphCellViewModel> glyphs)
    {
        if (glyphs is null)
        {
            throw new ArgumentNullException(nameof(glyphs));
        }

        DisplayName = displayName;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Glyphs = new ObservableCollection<GlyphCellViewModel>(glyphs);
    }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the tile width.
    /// </summary>
    public int TileWidth { get; }

    /// <summary>
    /// Gets the tile height.
    /// </summary>
    public int TileHeight { get; }

    /// <summary>
    /// Gets the glyphs composing the tile.
    /// </summary>
    public ObservableCollection<GlyphCellViewModel> Glyphs { get; }

    /// <summary>
    /// Creates a small set of demo tiles that light up the UI before real data is wired.
    /// </summary>
    /// <returns>A read only list of demo tiles.</returns>
    public static IReadOnlyList<TileViewModel> CreateSampleTiles()
    {
        var samples = new List<TileViewModel>
        {
            CreatePatternTile("Checker", 8, 8, (row, column) => (row + column) % 2 == 0 ? '#' : '.'),
            CreatePatternTile("Borders", 8, 8, (row, column) => row == 0 || column == 0 || row == 7 || column == 7 ? '+' : ' ')
        };

        return samples;
    }

    private static TileViewModel CreatePatternTile(string name, int width, int height, Func<int, int, char> selector)
    {
        var glyphs = new List<GlyphCellViewModel>(width * height);
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                var chr = selector(row, column);
                var foreground = (row + column) % 2 == 0 ? ConsoleColor.White : ConsoleColor.DarkGray;
                var background = (row + column) % 2 == 0 ? ConsoleColor.DarkBlue : ConsoleColor.Black;
                glyphs.Add(new GlyphCellViewModel(row, column, chr, foreground, background));
            }
        }

        return new TileViewModel(name, width, height, glyphs);
    }
}
