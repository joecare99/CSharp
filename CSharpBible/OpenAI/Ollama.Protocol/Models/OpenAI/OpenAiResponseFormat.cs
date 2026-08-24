using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models.OpenAI;

/// <summary>Specifies the response serialization mode for a chat completion.</summary>
public sealed class OpenAiResponseFormat
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }
}
