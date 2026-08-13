using System;

namespace Ollama.Tools;

/// <summary>
/// Represents one durable piece of session context.
/// </summary>
public sealed class SessionMemoryEntry
{
    /// <summary>
    /// Gets the session scope.
    /// </summary>
    public required string SessionId { get; init; }

    /// <summary>
    /// Gets the remembered context.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
