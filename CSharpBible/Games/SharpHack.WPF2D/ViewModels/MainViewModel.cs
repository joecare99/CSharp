using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using SharpHack.ViewModel;

namespace SharpHack.WPF2D.ViewModels;

public sealed record TilePaletteEntry(DisplayTile Tile, string Name, int Index);

/// <summary>
/// Main window view model.
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="game">The game view model.</param>
    public MainViewModel(LayeredGameViewModel game)
    {
        Game = game;
        TilePalette = Enum.GetValues<DisplayTile>()
            .OrderBy(tile => (int)tile)
            .Select(tile => new TilePaletteEntry(tile, tile.ToString(), (int)tile))
            .ToArray();
    }

    /// <summary>
    /// Gets the game view model.
    /// </summary>
    public LayeredGameViewModel Game { get; }

    public IReadOnlyList<TilePaletteEntry> TilePalette { get; }
}
