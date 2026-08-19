using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents the function metadata of a chat tool definition.
/// </summary>
public sealed class OllamaChatToolFunctionDefinition
{
    /// <summary>
    /// Gets the function name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the function description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the function parameters schema.
    /// </summary>
    [JsonPropertyName("parameters")]
    public required OllamaChatToolParameters Parameters { get; init; }
}
