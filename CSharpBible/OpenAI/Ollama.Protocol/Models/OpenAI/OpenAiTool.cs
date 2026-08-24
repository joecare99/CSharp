using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models.OpenAI;

/// <summary>Represents a function tool available to a chat completion.</summary>
public sealed class OpenAiTool
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "function";

    [JsonPropertyName("function")]
    public required OpenAiFunctionDefinition Function { get; init; }
}
