using System;
using System.Collections.Generic;

namespace Ollama.CodingAgent.Models;

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

    /// <summary>
    /// Gets the validated input payload sent to the tool.
    /// </summary>
    public string Input { get; init; } = string.Empty;

    /// <summary>
    /// Gets the elapsed execution time.
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Gets reasoning emitted while selecting this tool.
    /// </summary>
    public IReadOnlyList<string> Thinking { get; init; } = [];
}
