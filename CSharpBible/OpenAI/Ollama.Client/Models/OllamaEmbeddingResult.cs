using System;
using System.Collections.Generic;

namespace Ollama.Client.Models;

/// <summary>
/// Represents embedding vectors returned by the public client layer.
/// </summary>
public sealed class OllamaEmbeddingResult
{
    /// <summary>
    /// Gets the embedding vectors.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double>> Embeddings { get; init; } = Array.Empty<IReadOnlyList<double>>();
}
