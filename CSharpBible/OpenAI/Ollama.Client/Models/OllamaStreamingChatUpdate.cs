using System.Collections.Generic;

namespace Ollama.Client.Models;

/// <summary>
/// Represents one streaming update from a chat completion.
/// </summary>
public sealed class OllamaStreamingChatUpdate
{
    /// <summary>
    /// Gets the streamed content fragment.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Gets the streamed thinking fragment.
    /// </summary>
    public string? Thinking { get; init; }

    /// <summary>
    /// Gets streamed tool calls.
    /// </summary>
    public IReadOnlyList<OllamaChatToolCall> ToolCalls { get; init; } = new List<OllamaChatToolCall>();

    /// <summary>
    /// Gets a value indicating whether the stream completed.
    /// </summary>
    public bool Done { get; init; }
}
