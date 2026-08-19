namespace Ollama.Client.Models;

/// <summary>
/// Represents a native tool call returned by Ollama.
/// </summary>
public sealed class OllamaChatToolCall
{
    public string Name { get; init; } = string.Empty;
    public string Arguments { get; init; } = "{}";
}
