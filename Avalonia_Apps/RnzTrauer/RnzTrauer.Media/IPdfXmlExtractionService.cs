using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Media;

/// <summary>Runs a PDF XML converter and interprets its output.</summary>
public interface IPdfXmlExtractionService
{
    /// <summary>Extracts text and image candidates from one PDF file.</summary>
    Task<PdfXmlExtractionResult> ExtractAsync(
        PdfXmlExtractionRequest request,
        CancellationToken cancellationToken = default);
}
