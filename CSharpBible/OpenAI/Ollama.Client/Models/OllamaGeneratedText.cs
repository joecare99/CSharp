using System.Collections.Generic;

namespace Ollama.Client.Models;

/// <summary>
/// Represents a buffered generate result.
/// </summary>
public sealed class OllamaGeneratedText
{
    /// <summary>
    /// Gets the final generated text.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the collected thinking fragments.
    /// </summary>
    public IReadOnlyList<string> Thinking { get; init; } = new List<string>();
}
