using System;
using System.Collections.Generic;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Represents one structured runtime diagnostic event.
/// </summary>
public sealed class AgentDiagnosticEvent
{
    /// <summary>
    /// Gets the UTC timestamp at which the event was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the run correlation identifier.
    /// </summary>
    public required string CorrelationId { get; init; }

    /// <summary>
    /// Gets the event name.
    /// </summary>
    public required string EventName { get; init; }

    /// <summary>
    /// Gets the provider-neutral, redacted event detail.
    /// </summary>
    public string? Detail { get; init; }

    /// <summary>
    /// Gets structured redacted data associated with the event.
    /// </summary>
    public IReadOnlyDictionary<string, string> Data { get; init; } = new Dictionary<string, string>();

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
