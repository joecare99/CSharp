using System;
using System.Collections.Generic;

namespace Ollama.Tools;

/// <summary>
/// Represents the validation outcome for tool arguments.
/// </summary>
public sealed class OllamaToolValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the input is valid.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A successful validation result.</returns>
    public static OllamaToolValidationResult Success() => new OllamaToolValidationResult
    {
        IsValid = true,
    };

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A failed validation result.</returns>
    public static OllamaToolValidationResult Failure(params string[] errors) => new OllamaToolValidationResult
    {
        IsValid = false,
        Errors = errors,
    };
}
