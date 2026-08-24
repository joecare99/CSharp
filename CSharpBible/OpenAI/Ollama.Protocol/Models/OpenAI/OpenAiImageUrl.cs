using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models.OpenAI;

/// <summary>Represents an image URL content value. Ollama currently supports base64 data URLs.</summary>
public sealed class OpenAiImageUrl
{
    [JsonPropertyName("url")]
    public required string Url { get; init; }

    [JsonPropertyName("detail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Detail { get; init; }
}
