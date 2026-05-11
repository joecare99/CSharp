namespace Ollama.Tools;

/// <summary>
/// Represents the result of a tool execution.
/// </summary>
public sealed class OllamaToolResult
{
    /// <summary>
    /// Gets the output text returned by the tool.
    /// </summary>
    public string Output { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the tool execution succeeded.
    /// </summary>
    public bool Success { get; init; } = true;
}
