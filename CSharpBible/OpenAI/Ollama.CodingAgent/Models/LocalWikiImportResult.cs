namespace Ollama.CodingAgent.Models;

/// <summary>
/// Summarizes one Markdown wiki import.
/// </summary>
public sealed class LocalWikiImportResult
{
    /// <summary>
    /// Gets the number of Markdown files imported.
    /// </summary>
    public int ImportedCount { get; init; }

    /// <summary>
    /// Gets the number of files skipped because they were empty or invalid.
    /// </summary>
    public int SkippedCount { get; init; }
}
