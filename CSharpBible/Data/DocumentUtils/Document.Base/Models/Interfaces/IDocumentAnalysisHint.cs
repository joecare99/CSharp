namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a lightweight analysis hint attached to a document.
/// </summary>
/// <remarks>
/// Analysis hints are intentionally compact and renderer-neutral. They can be produced by heuristics,
/// OCR, or LLM-based classification and then used for routing or reporting.
/// </remarks>
public interface IDocumentAnalysisHint
{
    /// <summary>
    /// Gets the hint category.
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Gets the hint value.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Gets the optional confidence value.
    /// </summary>
    /// <remarks>
    /// Confidence can be used when the hint comes from a probabilistic classifier or heuristic
    /// scoring system.
    /// </remarks>
    double? Confidence { get; }

    /// <summary>
    /// Gets the optional origin of the hint.
    /// </summary>
    string? Origin { get; }
}
