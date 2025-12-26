using CommunityToolkit.Mvvm.ComponentModel;
using TileSetAnimator.Models;

namespace TileSetAnimator.ViewModels;

/// <summary>
/// Represents a single slot inside the 5x5 mini map preview.
/// </summary>
public partial class MiniMapSlotViewModel : ObservableObject
{
    public MiniMapSlotViewModel(int row, int column)
    {
        Row = row;
        Column = column;
        DisplayLabel = $"{row + 1},{column + 1}";
    }

    /// <summary>
    /// Gets the zero-based row of the slot.
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Gets the zero-based column of the slot.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets a friendly label shown when the slot is empty.
    /// </summary>
    public string DisplayLabel { get; }

    [ObservableProperty]
    private TileDefinition? tile;

    [ObservableProperty]
    private TileDefinition? suggestedTile;

    [ObservableProperty]
    private string? suggestionKey;
}
