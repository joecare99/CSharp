using System.Collections.Generic;

namespace Ollama.Client.Models;

/// <summary>
/// Represents a buffered chat completion result.
/// </summary>
public sealed class OllamaChatCompletion
{
    /// <summary>
    /// Gets the final content text.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the collected thinking fragments.
    /// </summary>
    public IReadOnlyList<string> Thinking { get; init; } = new List<string>();
}
