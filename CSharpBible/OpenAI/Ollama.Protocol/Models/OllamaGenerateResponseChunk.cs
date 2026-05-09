using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents one streamed chunk from the Ollama generate endpoint.
/// </summary>
public sealed class OllamaGenerateResponseChunk
{
    /// <summary>
    /// Gets the generated response fragment.
    /// </summary>
    [JsonPropertyName("response")]
    public string? Response { get; init; }

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
