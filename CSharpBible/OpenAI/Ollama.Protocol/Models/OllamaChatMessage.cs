using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents a chat message used by the Ollama chat endpoint.
/// </summary>
public sealed class OllamaChatMessage
{
    /// <summary>
    /// Gets the chat role.
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    /// <summary>
    /// Gets the chat message content.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }

    /// <summary>
    /// Gets an optional list of base64-encoded images to be sent alongside the chat text.
    /// </summary>
    [JsonPropertyName("images")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Images { get; init; }
}
