namespace Ollama.CodingAgent;

/// <summary>
/// Input for local wiki search tool.
/// </summary>
public sealed class LocalWikiSearchToolInput
{
    /// <summary>
    /// Gets or sets the query text.
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets max result count.
    /// </summary>
    public int MaxResults { get; init; } = 10;
}
