using System;

namespace Ollama.CodingAgent;

/// <summary>
/// Represents one structured runtime diagnostic event.
/// </summary>
public sealed class AgentDiagnosticEvent
{
    /// <summary>
    /// Gets the run correlation identifier.
    /// </summary>
    public required string CorrelationId { get; init; }

    /// <summary>
    /// Gets the event name.
    /// </summary>
    public required string EventName { get; init; }

    /// <summary>
    /// Gets the turn number, when applicable.
    /// </summary>
    public int? Iteration { get; init; }

    /// <summary>
    /// Gets the retry attempt number, when applicable.
    /// </summary>
    public int? Attempt { get; init; }

    /// <summary>
    /// Gets the measured duration.
    /// </summary>
    public TimeSpan? Duration { get; init; }

    /// <summary>
    /// Gets the failure detail, when applicable.
    /// </summary>
    public string? Error { get; init; }
}
