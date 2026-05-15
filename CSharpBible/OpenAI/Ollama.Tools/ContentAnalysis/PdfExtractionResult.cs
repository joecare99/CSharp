namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents the result of a PDF text extraction operation.
/// </summary>
public sealed class PdfExtractionResult
{
    /// <summary>
    /// Gets a value indicating whether extraction succeeded.
    /// </summary>
    public bool IsSuccessful { get; init; }

    /// <summary>
    /// Gets the source file path when known.
    /// </summary>
    public string FilePath { get; init; } = string.Empty;

    /// <summary>
    /// Gets the extracted text.
    /// </summary>
    public string ExtractedText { get; init; } = string.Empty;

    /// <summary>
    /// Gets the extracted page count when known.
    /// </summary>
    public int? PageCount { get; init; }

    /// <summary>
    /// Gets optional source file metadata.
    /// </summary>
    public ContentAnalysisFileMetadata? FileMetadata { get; init; }

    /// <summary>
    /// Gets the error message when extraction fails.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Creates a successful extraction result.
    /// </summary>
    public static PdfExtractionResult Success(string sFilePath, string sExtractedText, int? iPageCount = null, ContentAnalysisFileMetadata? fileMetadata = null)
    {
        return new PdfExtractionResult
        {
            IsSuccessful = true,
            FilePath = sFilePath,
            ExtractedText = sExtractedText,
            PageCount = iPageCount,
            FileMetadata = fileMetadata,
        };
    }

    /// <summary>
    /// Creates a failed extraction result.
    /// </summary>
    public static PdfExtractionResult Failure(string sFilePath, string sErrorMessage, int? iPageCount = null, ContentAnalysisFileMetadata? fileMetadata = null)
    {
        return new PdfExtractionResult
        {
            IsSuccessful = false,
            FilePath = sFilePath,
            PageCount = iPageCount,
            FileMetadata = fileMetadata,
            ErrorMessage = sErrorMessage,
        };
    }
}