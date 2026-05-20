using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents the response returned by the Ollama embed endpoint.
/// </summary>
public sealed class OllamaEmbedResponse
{
    /// <summary>
    /// Gets the returned embedding vectors.
    /// </summary>
    [JsonPropertyName("embeddings")]
    public IReadOnlyList<IReadOnlyList<double>> Embeddings { get; init; } = Array.Empty<IReadOnlyList<double>>();
}
