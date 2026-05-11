using System;
using System.Collections.Generic;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents the structured result of content analysis.
/// </summary>
public sealed class ContentAnalysisResult
{
    /// <summary>
    /// Gets the analysis summary.
    /// </summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>
    /// Gets the normalized score in the range from 0.0 to 1.0 when available.
    /// </summary>
    public double? Score { get; init; }

    /// <summary>
    /// Gets the normalized confidence in the range from 0.0 to 1.0 when available.
    /// </summary>
    public double? Confidence { get; init; }

    /// <summary>
    /// Gets the detailed rationale for the analysis result.
    /// </summary>
    public string Rationale { get; init; } = string.Empty;

    /// <summary>
    /// Gets the returned findings.
    /// </summary>
    public IReadOnlyList<ContentAnalysisFinding> Findings { get; init; } = Array.Empty<ContentAnalysisFinding>();

    /// <summary>
    /// Gets the suggested follow-up actions.
    /// </summary>
    public IReadOnlyList<ContentAnalysisSuggestion> Suggestions { get; init; } = Array.Empty<ContentAnalysisSuggestion>();
}
