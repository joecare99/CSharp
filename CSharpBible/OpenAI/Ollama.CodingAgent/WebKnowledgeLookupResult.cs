namespace Ollama.CodingAgent;

/// <summary>
/// Contains bounded web content together with its citation metadata.
/// </summary>
public sealed class WebKnowledgeLookupResult
{
    /// <summary>
    /// Gets the citation metadata.
    /// </summary>
    public required WebKnowledgeCitation Citation { get; init; }

    /// <summary>
    /// Gets the HTTP status code returned by the source.
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// Gets the bounded response preview.
    /// </summary>
    public required string ContentPreview { get; init; }
}
