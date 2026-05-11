namespace Ollama.Tools;

/// <summary>
/// Represents the outcome of a tool invocation attempt.
/// </summary>
public sealed class OllamaToolInvocationResult
{
    /// <summary>
    /// Gets the tool name.
    /// </summary>
    public required string ToolName { get; init; }

    /// <summary>
    /// Gets the raw tool input.
    /// </summary>
    public string Input { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the tool was resolved and executed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the returned tool output.
    /// </summary>
    public string Output { get; init; } = string.Empty;

    /// <summary>
    /// Gets the error text if invocation failed.
    /// </summary>
    public string? Error { get; init; }
}
