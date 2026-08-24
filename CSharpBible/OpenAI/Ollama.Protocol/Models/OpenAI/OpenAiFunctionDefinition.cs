using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models.OpenAI;

/// <summary>Describes a function exposed as an OpenAI-compatible tool.</summary>
public sealed class OpenAiFunctionDefinition
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    [JsonPropertyName("parameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Parameters { get; init; }
}
