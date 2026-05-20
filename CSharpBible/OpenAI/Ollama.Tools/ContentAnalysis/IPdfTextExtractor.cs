using System.Threading;
using System.Threading.Tasks;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Defines a contract for extracting text from PDF documents.
/// </summary>
public interface IPdfTextExtractor
{
    /// <summary>
    /// Extracts text from the specified PDF request.
    /// </summary>
    Task<PdfExtractionResult> ExtractAsync(PdfExtractionRequest request, CancellationToken cancellationToken = default);
}