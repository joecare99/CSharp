using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents a lightweight analysis hint attached to the document graph.
/// </summary>
/// <remarks>
/// Hints are intentionally simple so they can be produced by heuristics, OCR, or LLM-based
/// classifiers and then consumed by routing or rendering logic.
/// </remarks>
public sealed record DocumentAnalysisHintModel(
    /// <summary>
    /// The hint category, such as text quality, image type, or face detection.
    /// </summary>
    string Category,
    /// <summary>
    /// The hint value.
    /// </summary>
    string Value,
    /// <summary>
    /// An optional confidence score.
    /// </summary>
    double? Confidence = null,
    /// <summary>
    /// An optional origin description identifying how the hint was produced.
    /// </summary>
    string? Origin = null) : IDocumentAnalysisHint;
