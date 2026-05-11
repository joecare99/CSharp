namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Describes the severity of one analysis finding.
/// </summary>
public enum ContentAnalysisSeverity
{
    /// <summary>
    /// Informational finding.
    /// </summary>
    Info,

    /// <summary>
    /// Warning finding.
    /// </summary>
    Warning,

    /// <summary>
    /// Error finding.
    /// </summary>
    Error,
}
