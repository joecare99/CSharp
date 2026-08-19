using System.Text.Json.Serialization;

namespace Ollama_Service2.Models;

internal sealed class OllamaGenerateResponseChunk
{
    [JsonPropertyName("response")]
    public string? Response { get; init; }

    [JsonPropertyName("thinking")]
    public string? Thinking { get; init; }

    [JsonPropertyName("done")]
    public bool Done { get; init; }
}
