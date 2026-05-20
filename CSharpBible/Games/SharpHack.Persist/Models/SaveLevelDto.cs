using System.Collections.Generic;

namespace SharpHack.Persist.Models;

/// <summary>
/// Represents the saved state for one dungeon level.
/// </summary>
public sealed class SaveLevelDto
{
    /// <summary>
    /// Gets or sets the logical level number.
    /// </summary>
    public int LevelNumber { get; set; }

    /// <summary>
    /// Gets or sets the level map state.
    /// </summary>
    public SaveMapDto Map { get; set; } = new();

    /// <summary>
    /// Gets or sets the creatures present on the level.
    /// </summary>
    public List<SaveCreatureDto> Creatures { get; set; } = [];

    /// <summary>
    /// Gets or sets the items present on the level floor.
    /// </summary>
    public List<SaveItemDto> Items { get; set; } = [];
}
