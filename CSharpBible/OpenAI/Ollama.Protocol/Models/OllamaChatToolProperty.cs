using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents one property in a chat tool parameter schema.
/// </summary>
public sealed class OllamaChatToolProperty
{
    /// <summary>
    /// Gets the JSON value type.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Gets the property description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }
}
