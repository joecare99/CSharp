using System.Collections.Generic;

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

    /// <summary>
    /// Gets an optional list of base64-encoded images to be sent alongside the chat text.
    /// </summary>
    public IReadOnlyList<string>? Images { get; init; }
}
