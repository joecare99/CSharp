using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents a native tool call emitted by Ollama.
/// </summary>
public sealed class OllamaChatToolCall
{
    [JsonPropertyName("function")]
    public OllamaChatToolFunctionCall? Function { get; init; }
}

/// <summary>
/// Represents the function payload of a native Ollama tool call.
/// </summary>
public sealed class OllamaChatToolFunctionCall
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public JsonElement Arguments { get; init; }
}
