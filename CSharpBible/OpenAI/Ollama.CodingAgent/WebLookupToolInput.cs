namespace Ollama.CodingAgent;

/// <summary>
/// Input for web lookup tool.
/// </summary>
public sealed class WebLookupToolInput
{
    /// <summary>
    /// Gets or sets the allowed source key.
    /// </summary>
    public string Source { get; init; } = "wikipedia";

    /// <summary>
    /// Gets or sets the query text.
    /// </summary>
    public string Query { get; init; } = string.Empty;
}
