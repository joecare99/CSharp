namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Specifies how content should be routed for analysis.
/// </summary>
public enum ContentAnalysisMode
{
    /// <summary>
    /// Detect the most suitable analysis mode automatically.
    /// </summary>
    Auto,

    /// <summary>
    /// Analyze the input as plain text.
    /// </summary>
    Text,

    /// <summary>
    /// Analyze the input as C# source code.
    /// </summary>
    CSharp,
}
