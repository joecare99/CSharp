using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents the response from the Ollama tags endpoint.
/// </summary>
public sealed class OllamaTagsResponse
{
    /// <summary>
    /// Gets the available models reported by the server.
    /// </summary>
    [JsonPropertyName("models")]
    public IReadOnlyList<OllamaModelInfo> Models { get; init; } = Array.Empty<OllamaModelInfo>();
}
