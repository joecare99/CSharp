using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents a request sent to the Ollama generate endpoint.
/// </summary>
public sealed class OllamaGenerateRequest
{
    /// <summary>
    /// Gets the model name that should handle the prompt.
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>
    /// Gets the prompt that should be processed.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    /// <summary>
    /// Gets a value indicating whether the response should be streamed.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; init; }
}
