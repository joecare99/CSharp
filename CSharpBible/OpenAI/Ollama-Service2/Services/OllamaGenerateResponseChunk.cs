using System.Text.Json.Serialization;

namespace Services;

internal sealed class OllamaGenerateResponseChunk
{
    [JsonPropertyName("response")]
    public string? Response { get; init; }

    [JsonPropertyName("thinking")]
    public string? Thinking { get; init; }

    [JsonPropertyName("done")]
    public bool Done { get; init; }
}
