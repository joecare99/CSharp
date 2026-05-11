namespace Ollama.Client.Models;

/// <summary>
/// Represents a chat message in the public client API.
/// </summary>
public sealed class OllamaClientChatMessage
{
    /// <summary>
    /// Gets the chat role.
    /// </summary>
    public required string Role { get; init; }

    /// <summary>
    /// Gets the message content.
    /// </summary>
    public required string Content { get; init; }
}
