namespace Ollama.CodingAgent;

/// <summary>
/// Represents input for the workspace file-read tool.
/// </summary>
public sealed class ReadWorkspaceFileToolInput
{
    /// <summary>
    /// Gets or sets the relative file path.
    /// </summary>
    public required string RelativePath { get; init; }

    /// <summary>
    /// Gets or sets the one-based start line.
    /// </summary>
    public int StartLine { get; init; } = 1;

    /// <summary>
    /// Gets or sets the maximum number of lines to return.
    /// </summary>
    public int LineCount { get; init; } = 120;
}
