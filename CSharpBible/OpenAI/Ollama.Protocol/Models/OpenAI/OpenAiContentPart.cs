using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models.OpenAI;

/// <summary>Represents one text or image content part in a chat message.</summary>
public sealed class OpenAiContentPart
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; init; }

    [JsonPropertyName("image_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OpenAiImageUrl? ImageUrl { get; init; }
}
