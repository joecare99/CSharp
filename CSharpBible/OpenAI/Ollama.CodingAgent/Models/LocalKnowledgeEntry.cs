using System;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Represents one local knowledge-base entry.
/// </summary>
public sealed class LocalKnowledgeEntry
{
    /// <summary>
    /// Gets or sets the entry id.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the summary content.
    /// </summary>
    public required string Summary { get; init; }

    /// <summary>
    /// Gets or sets the source label.
    /// </summary>
    public string Source { get; init; } = "local";

    /// <summary>
    /// Gets or sets optional source citation URL.
    /// </summary>
    public string? CitationUrl { get; init; }

    /// <summary>
    /// Gets or sets optional topic tags.
    /// </summary>
    public string[] Tags { get; init; } = [];

    /// <summary>
    /// Gets or sets the UTC creation time.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
