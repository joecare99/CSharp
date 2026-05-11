using System;
using System.Collections.Generic;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents the validation result for a content analysis request.
/// </summary>
public sealed class ContentAnalysisRequestValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the request is valid.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets the collected validation issues.
    /// </summary>
    public IReadOnlyList<ContentAnalysisValidationIssue> Issues { get; init; } = Array.Empty<ContentAnalysisValidationIssue>();
}
