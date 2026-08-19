using System;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Identifies the external source used for a knowledge result.
/// </summary>
public sealed class WebKnowledgeCitation
{
    /// <summary>
    /// Gets the configured source key.
    /// </summary>
    public required string Source { get; init; }

    /// <summary>
    /// Gets the original lookup query.
    /// </summary>
    public required string Query { get; init; }

    /// <summary>
    /// Gets the resolved source URL.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// Gets the retrieval timestamp.
    /// </summary>
    public DateTimeOffset RetrievedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
