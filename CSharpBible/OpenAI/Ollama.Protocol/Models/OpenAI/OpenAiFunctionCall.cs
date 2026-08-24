using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models.OpenAI;

/// <summary>Contains the name and JSON arguments of an emitted function call.</summary>
public sealed class OpenAiFunctionCall
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("arguments")]
    public required string Arguments { get; init; }
}
