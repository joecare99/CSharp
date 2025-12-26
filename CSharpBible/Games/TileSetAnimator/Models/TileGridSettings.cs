namespace TileSetAnimator.Models;

/// <summary>
/// Describes the layout of the source tile sheet.
/// </summary>
public readonly record struct TileGridSettings(int TileWidth, int TileHeight, int Spacing, int Margin)
{
    /// <summary>
    /// Gets a value indicating whether the current layout can be used to slice tiles.
    /// </summary>
    public bool IsValid => TileWidth > 0 && TileHeight > 0;
}
