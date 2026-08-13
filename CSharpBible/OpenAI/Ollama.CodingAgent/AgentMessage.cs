using System;

namespace Ollama.CodingAgent;

/// <summary>
/// Represents one role/content message in a conversation.
/// </summary>
public sealed class AgentMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentMessage"/> class.
    /// </summary>
    /// <param name="role">The message role.</param>
    /// <param name="content">The message content.</param>
    public AgentMessage(string role, string content)
    {
        Role = string.IsNullOrWhiteSpace(role)
            ? throw new ArgumentException("Role must not be empty.", nameof(role))
            : role;
        Content = string.IsNullOrWhiteSpace(content)
            ? throw new ArgumentException("Content must not be empty.", nameof(content))
            : content;
    }

    /// <summary>
    /// Gets the message role.
    /// </summary>
    public string Role { get; }

    /// <summary>
    /// Gets the message content.
    /// </summary>
    public string Content { get; }
}
