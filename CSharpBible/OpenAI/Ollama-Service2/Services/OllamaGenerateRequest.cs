using System.Text.Json.Serialization;

namespace Services;

public sealed class OllamaGenerateRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    [JsonPropertyName("stream")]
    public bool Stream { get; init; }
}
