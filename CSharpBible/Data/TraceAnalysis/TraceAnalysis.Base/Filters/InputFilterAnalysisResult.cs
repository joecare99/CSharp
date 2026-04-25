using System;
using System.Collections.Generic;
using System.Linq;

namespace TraceAnalysis.Base.Filters;

/// <summary>
/// Represents the deterministic analysis output of an input filter for a specific source.
/// </summary>
public sealed class InputFilterAnalysisResult
{
    /// <summary>
    /// Initializes a new instance of <see cref="InputFilterAnalysisResult"/>.
    /// </summary>
    /// <param name="filterId">Unique filter identifier.</param>
    /// <param name="canHandle">Indicates whether the filter can process the source.</param>
    /// <param name="confidenceScore">Confidence score used for deterministic ranking.</param>
    /// <param name="isExactExtensionMatch">Indicates whether extension matching is exact.</param>
    /// <param name="decisionLines">Human-readable decision lines for diagnostics.</param>
    public InputFilterAnalysisResult(
        string filterId,
        bool canHandle,
        int confidenceScore,
        bool isExactExtensionMatch,
        IEnumerable<string>? decisionLines = null)
    {
        if (string.IsNullOrWhiteSpace(filterId))
            throw new ArgumentException("A filter identifier is required.", nameof(filterId));

        FilterId = filterId;
        CanHandle = canHandle;
        ConfidenceScore = confidenceScore;
        IsExactExtensionMatch = isExactExtensionMatch;
        DecisionLines = (decisionLines ?? Enumerable.Empty<string>()).ToArray();
    }

    /// <summary>
    /// Unique filter identifier.
    /// </summary>
    public string FilterId { get; }

    /// <summary>
    /// Indicates whether the filter can process the source.
    /// </summary>
    public bool CanHandle { get; }

    /// <summary>
    /// Confidence score used for deterministic ranking.
    /// </summary>
    public int ConfidenceScore { get; }

    /// <summary>
    /// Indicates whether extension matching is exact.
    /// </summary>
    public bool IsExactExtensionMatch { get; }

    /// <summary>
    /// Human-readable decision lines for diagnostics.
    /// </summary>
    public IReadOnlyList<string> DecisionLines { get; }
}
