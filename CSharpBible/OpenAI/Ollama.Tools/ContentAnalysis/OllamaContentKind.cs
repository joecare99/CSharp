namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Identifies the declared content kind for an analysis request.
/// </summary>
public enum OllamaContentKind
{
    /// <summary>
    /// Plain text content.
    /// </summary>
    Text,

    /// <summary>
    /// Source code content.
    /// </summary>
    SourceCode,

    /// <summary>
    /// Image content.
    /// </summary>
    Image,
}
