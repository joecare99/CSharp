using SharpHack.Base.Data;

namespace SharpHack.Persist.Models;

/// <summary>
/// Represents the serializable run-level state needed to resume gameplay.
/// </summary>
public sealed class SaveRunStateDto
{
    /// <summary>
    /// Gets or sets the current run lifecycle state.
    /// </summary>
    public GameRunState RunState { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the run has reached victory.
    /// </summary>
    public bool HasWon { get; set; }

    /// <summary>
    /// Gets or sets the current dungeon level.
    /// </summary>
    public int CurrentLevel { get; set; }

    /// <summary>
    /// Gets or sets the total turns taken.
    /// </summary>
    public int TurnsTaken { get; set; }

    /// <summary>
    /// Gets or sets the victory objective label.
    /// </summary>
    public string VictoryObjective { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the completion summary shown after terminal runs.
    /// </summary>
    public string CompletionSummary { get; set; } = string.Empty;
}
