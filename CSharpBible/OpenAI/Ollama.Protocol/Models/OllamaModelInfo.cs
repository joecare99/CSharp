using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents a model entry returned by the Ollama tags endpoint.
/// </summary>
public sealed class OllamaModelInfo
{
    /// <summary>
    /// Gets the display name of the model.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the internal model identifier.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; init; }
}
