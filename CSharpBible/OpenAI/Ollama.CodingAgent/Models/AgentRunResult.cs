using System.Collections.Generic;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Contains one completed agent run result.
/// </summary>
public sealed class AgentRunResult
{
    /// <summary>
    /// Gets or sets the final response text.
    /// </summary>
    public required string FinalResponse { get; init; }

    /// <summary>
    /// Gets or sets the number of iterations consumed.
    /// </summary>
    public required int IterationsUsed { get; init; }

    /// <summary>
    /// Gets or sets the number of retry attempts consumed.
    /// </summary>
    public required int RetryAttemptsUsed { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the final marker was used.
    /// </summary>
    public required bool FinalizedWithMarker { get; init; }

    /// <summary>
    /// Gets the model reasoning fragments collected during the run.
    /// </summary>
    public IReadOnlyList<string> Thinking { get; init; } = [];
}
