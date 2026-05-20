namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents one suggested follow-up action from content analysis.
/// </summary>
public sealed class ContentAnalysisSuggestion
{
    /// <summary>
    /// Gets the suggested action title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the suggested action description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional priority hint.
    /// </summary>
    public string Priority { get; init; } = string.Empty;
}
