using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;

namespace Ollama.CodingAgent;

/// <summary>
/// Input for local wiki write tool.
/// </summary>
public sealed class LocalWikiWriteToolInput
{
    /// <summary>
    /// Gets or sets entry id.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets title.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets summary.
    /// </summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets source label.
    /// </summary>
    public string Source { get; init; } = "local";

    /// <summary>
    /// Gets or sets optional citation URL.
    /// </summary>
    public string? CitationUrl { get; init; }

    /// <summary>
    /// Gets or sets optional tags.
    /// </summary>
    public string[] Tags { get; init; } = [];
}
