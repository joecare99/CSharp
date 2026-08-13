namespace Ollama.CodingAgent;

/// <summary>
/// Represents one delegated tool step execution.
/// </summary>
public sealed class DelegatedToolStep
{
    /// <summary>
    /// Gets or sets the step index.
    /// </summary>
    public required int StepIndex { get; init; }

    /// <summary>
    /// Gets or sets the requested tool name.
    /// </summary>
    public required string ToolName { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the tool succeeded.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets or sets the tool output or error payload.
    /// </summary>
    public required string Output { get; init; }
}
