namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Describes one evaluation criterion for content analysis.
/// </summary>
public sealed class ContentAnalysisCriterion
{
    /// <summary>
    /// Gets the criterion name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the human-readable criterion description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional criterion weight.
    /// </summary>
    public double? Weight { get; init; }
}
