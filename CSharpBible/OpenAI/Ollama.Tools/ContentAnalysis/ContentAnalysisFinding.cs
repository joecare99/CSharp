namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents one structured finding returned by content analysis.
/// </summary>
public sealed class ContentAnalysisFinding
{
    /// <summary>
    /// Gets the finding title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the detailed finding message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the finding severity.
    /// </summary>
    public ContentAnalysisSeverity Severity { get; init; } = ContentAnalysisSeverity.Info;

    /// <summary>
    /// Gets the optional evidence reference such as a line range or region identifier.
    /// </summary>
    public string Evidence { get; init; } = string.Empty;
}
