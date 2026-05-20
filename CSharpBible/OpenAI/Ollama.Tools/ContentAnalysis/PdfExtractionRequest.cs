namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents a request to extract text from a PDF document.
/// </summary>
public sealed class PdfExtractionRequest
{
    /// <summary>
    /// Gets the PDF file path to extract.
    /// </summary>
    public string FilePath { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional PDF password.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Gets optional metadata about the source file.
    /// </summary>
    public ContentAnalysisFileMetadata? FileMetadata { get; init; }
}