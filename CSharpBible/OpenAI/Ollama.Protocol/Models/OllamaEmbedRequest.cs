using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents a request sent to the Ollama embed endpoint.
/// </summary>
public sealed class OllamaEmbedRequest
{
    /// <summary>
    /// Gets the model name that should generate embeddings.
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>
    /// Gets the input values for which embeddings should be created.
    /// </summary>
    [JsonPropertyName("input")]
    public IReadOnlyList<string> Input { get; init; } = Array.Empty<string>();
}
