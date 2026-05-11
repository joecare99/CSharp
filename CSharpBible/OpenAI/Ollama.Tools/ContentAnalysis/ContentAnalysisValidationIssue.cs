namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents one validation issue for a content analysis request.
/// </summary>
public sealed class ContentAnalysisValidationIssue
{
    /// <summary>
    /// Gets the affected field name.
    /// </summary>
    public required string Field { get; init; }

    /// <summary>
    /// Gets the stable validation code.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets the human-readable validation message.
    /// </summary>
    public required string Message { get; init; }
}
