namespace Ollama.Client.Models;

/// <summary>
/// Represents one streaming update from a generate request.
/// </summary>
public sealed class OllamaStreamingGenerateUpdate
{
    /// <summary>
    /// Gets the streamed text fragment.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Gets the streamed thinking fragment.
    /// </summary>
    public string? Thinking { get; init; }

    /// <summary>
    /// Gets a value indicating whether the stream completed.
    /// </summary>
    public bool Done { get; init; }
}
