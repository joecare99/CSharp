using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents the response from the Ollama running-models endpoint.
/// </summary>
public sealed class OllamaPsResponse
{
    /// <summary>
    /// Gets the models currently loaded into memory.
    /// </summary>
    [JsonPropertyName("models")]
    public IReadOnlyList<OllamaRunningModel> Models { get; init; } = Array.Empty<OllamaRunningModel>();
}
