namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents the detected routing decision for content analysis.
/// </summary>
public sealed class ContentAnalysisRoutingDecision
{
    /// <summary>
    /// Gets the selected analysis mode label.
    /// </summary>
    public string AnalysisLabel { get; init; } = string.Empty;

    /// <summary>
    /// Gets the selected content kind.
    /// </summary>
    public OllamaContentKind ContentKind { get; init; }

    /// <summary>
    /// Gets the selected media type.
    /// </summary>
    public string MediaType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the selected language.
    /// </summary>
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Gets the reason for the routing decision.
    /// </summary>
    public string Reason { get; init; } = string.Empty;
}
