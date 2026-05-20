using System.Collections.Generic;

namespace SharpHack.Persist.Models;

/// <summary>
/// Represents a serialized map including dimensions and tile data.
/// </summary>
public sealed class SaveMapDto
{
    /// <summary>
    /// Gets or sets the map width.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the map height.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the serialized tiles for the map.
    /// </summary>
    public List<SaveTileDto> Tiles { get; set; } = [];
}
