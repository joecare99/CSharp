using System.Collections.Generic;

namespace SharpHack.Persist.Models;

/// <summary>
/// Root DTO for a complete SharpHack save payload.
/// </summary>
public sealed class SaveGameDto
{
    /// <summary>
    /// Gets or sets general save metadata.
    /// </summary>
    public SaveGameMetadataDto Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets recovery metadata for backup and integrity workflows.
    /// </summary>
    public SaveRecoveryMetadataDto Recovery { get; set; } = new();

    /// <summary>
    /// Gets or sets the current run-level state.
    /// </summary>
    public SaveRunStateDto Run { get; set; } = new();

    /// <summary>
    /// Gets or sets the player creature state.
    /// </summary>
    public SaveCreatureDto Player { get; set; } = new();

    /// <summary>
    /// Gets or sets the saved dungeon levels.
    /// </summary>
    public List<SaveLevelDto> Levels { get; set; } = [];
}
