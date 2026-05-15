namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents the root of a canonical document graph.
/// </summary>
/// <remarks>
/// The document model is the top-level semantic object produced by importers. It groups pages,
/// reusable resources, and analysis hints while remaining renderer-neutral and document-type
/// agnostic.
/// </remarks>
public interface IDocumentModel : IDocumentNode, IDocumentContainer<IDocumentPage>, IDocumentImportSource
{
    /// <summary>
    /// Gets the reusable resources associated with the document.
    /// </summary>
    /// <remarks>
    /// Resources typically represent exported images, OCR artifacts, generated previews, or other
    /// file-like assets that remain linked to the document graph.
    /// </remarks>
    IReadOnlyList<IDocumentResource> Resources { get; }

    /// <summary>
    /// Gets the analysis hints attached to the document.
    /// </summary>
    /// <remarks>
    /// Hints can describe extraction quality, OCR fallback decisions, image classification results,
    /// or any other lightweight semantic signal that helps downstream processing.
    /// </remarks>
    IReadOnlyList<IDocumentAnalysisHint> AnalysisHints { get; }
}
