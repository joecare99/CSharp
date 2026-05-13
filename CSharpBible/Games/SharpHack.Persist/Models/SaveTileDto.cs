using System.Collections.Generic;
using SharpHack.Base.Data;

namespace SharpHack.Persist.Models;

/// <summary>
/// Represents a serialized tile including geometry, visibility state, and resident content references.
/// </summary>
public sealed class SaveTileDto
{
    /// <summary>
    /// Gets or sets the tile position.
    /// </summary>
    public SavePointDto Position { get; set; } = new();

    /// <summary>
    /// Gets or sets the tile type.
    /// </summary>
    public TileType Type { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tile is visible in the saved state.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tile has been explored.
    /// </summary>
    public bool IsExplored { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the creature occupying the tile.
    /// </summary>
    public string CreatureId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifiers of the items currently on the tile.
    /// </summary>
    public List<string> ItemIds { get; set; } = [];
}
