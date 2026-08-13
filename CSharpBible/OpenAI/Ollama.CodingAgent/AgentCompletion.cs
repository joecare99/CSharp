using System.Collections.Generic;

namespace Ollama.CodingAgent;

/// <summary>
/// Represents model content together with optional reasoning fragments.
/// </summary>
public sealed class AgentCompletion
{
    /// <summary>
    /// Gets the response content.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the reasoning fragments emitted by the model.
    /// </summary>
    public IReadOnlyList<string> Thinking { get; init; } = [];
}
