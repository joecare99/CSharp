using System;

namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Represents a single provider-agnostic AI interaction message.
/// </summary>
public sealed class AIMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AIMessage"/> class.
    /// </summary>
    /// <param name="role">The message role.</param>
    /// <param name="content">The message content.</param>
    public AIMessage(AIMessageRole role, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Message content is required.", nameof(content));
        }

        Role = role;
        Content = content;
    }

    /// <summary>
    /// Gets the message role.
    /// </summary>
    public AIMessageRole Role { get; }

    /// <summary>
    /// Gets the message content.
    /// </summary>
    public string Content { get; }
}