using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents one streamed chunk from the Ollama chat endpoint.
/// </summary>
public sealed class OllamaChatResponseChunk
{
    /// <summary>
    /// Gets the streamed message fragment.
    /// </summary>
    [JsonPropertyName("message")]
    public OllamaChatMessage? Message { get; init; }

    /// <summary>
    /// Gets the intermediate thinking fragment if the model emits it.
    /// </summary>
    [JsonPropertyName("thinking")]
    public string? Thinking { get; init; }

    /// <summary>
    /// Gets a value indicating whether the stream has completed.
    /// </summary>
    [JsonPropertyName("done")]
    public bool Done { get; init; }
}
