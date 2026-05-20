namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Identifies how content is supplied for analysis.
/// </summary>
public enum OllamaContentSourceKind
{
    /// <summary>
    /// The content is provided inline in the request.
    /// </summary>
    Inline,

    /// <summary>
    /// The content is referenced by a file path.
    /// </summary>
    FilePath,
}
