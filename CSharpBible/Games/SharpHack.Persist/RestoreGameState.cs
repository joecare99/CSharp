using System.Collections.Generic;
using SharpHack.Base.Data;
using SharpHack.Base.Interfaces;

namespace SharpHack.Persist;

/// <summary>
/// Represents restored runtime state reconstructed from a save DTO payload.
/// </summary>
public sealed class RestoreGameState
{
    public RestoreGameState()
    {
        VictoryObjective = string.Empty;
        CompletionSummary = string.Empty;
        Enemies = new List<ICreature>();
    }

    /// <summary>
    /// Gets or sets the restored level map.
    /// </summary>
    public IMap? Map { get; set; }

    /// <summary>
    /// Gets or sets the restored player creature.
    /// </summary>
    public ICreature? Player { get; set; }

    /// <summary>
    /// Gets or sets the restored enemy list.
    /// </summary>
    public IList<ICreature> Enemies { get; set; }

    /// <summary>
    /// Gets or sets the restored level number.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the restored run lifecycle state.
    /// </summary>
    public GameRunState RunState { get; set; }

    /// <summary>
    /// Gets or sets the restored turn count.
    /// </summary>
    public int TurnsTaken { get; set; }

    /// <summary>
    /// Gets or sets the restored victory objective text.
    /// </summary>
    public string VictoryObjective { get; set; }

    /// <summary>
    /// Gets or sets the restored completion summary text.
    /// </summary>
    public string CompletionSummary { get; set; }
}
