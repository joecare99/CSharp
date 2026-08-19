using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents the JSON schema for a chat tool's parameters.
/// </summary>
public sealed class OllamaChatToolParameters
{
    /// <summary>
    /// Gets the parameter schema type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "object";

    /// <summary>
    /// Gets the parameter properties.
    /// </summary>
    [JsonPropertyName("properties")]
    public IReadOnlyDictionary<string, OllamaChatToolProperty> Properties { get; init; } = new Dictionary<string, OllamaChatToolProperty>();

    /// <summary>
    /// Gets the required parameter names.
    /// </summary>
    [JsonPropertyName("required")]
    public IReadOnlyList<string> Required { get; init; } = [];
}
