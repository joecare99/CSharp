using System;
using System.Collections.Generic;

namespace Ollama.Client;

/// <summary>
/// Provides options for embedding requests.
/// </summary>
public sealed class EmbeddingOptions
{
    /// <summary>
    /// Gets the input texts to embed.
    /// </summary>
    public IReadOnlyList<string> Input { get; init; } = Array.Empty<string>();
}
