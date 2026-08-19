using System;
using System.Collections.Generic;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Represents the result of a local Ollama baseline check.
/// </summary>
public sealed class OllamaBaselineCheckResult
{
    /// <summary>
    /// Gets a value indicating whether the check completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets a value indicating whether the configured model was found.
    /// </summary>
    public bool ModelAvailable { get; init; }

    /// <summary>
    /// Gets the available model names observed during preflight.
    /// </summary>
    public IReadOnlyList<string> AvailableModels { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets the baseline response, if a smoke request was executed.
    /// </summary>
    public string Response { get; init; } = string.Empty;

    /// <summary>
    /// Gets the failure detail, if any.
    /// </summary>
    public string? Error { get; init; }
}
