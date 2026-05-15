using Document.Base.Models.Interfaces;

namespace Document.Model.Interfaces;

/// <summary>
/// Defines the minimal write API for populating a canonical document model.
/// </summary>
/// <remarks>
/// This contract works only with interfaces from <c>Document.Base</c> so that importers do not
/// depend on the concrete classes in <c>Document.Model</c>. The writer is responsible for
/// preserving order, attaching resources, and keeping derived analysis artifacts linked to the
/// correct document nodes.
/// 
/// <para>
/// The writer is intentionally model-focused rather than renderer-focused. It should only expose
/// the operations needed to populate the document graph; rendering to Markdown, HTML, or other
/// output formats belongs elsewhere.
/// </para>
/// </remarks>
public interface IDocumentModelWriter
{
    /// <summary>
    /// Gets the document instance being populated.
    /// </summary>
    IDocumentModel Document { get; }

    /// <summary>
    /// Sets or replaces the document-level metadata payload.
    /// </summary>
    /// <param name="metadata">The metadata to attach to the document.</param>
    /// <remarks>
    /// Importers should call this once they have gathered the document-level metadata from the
    /// source. The metadata should represent the source document, not the importer itself.
    /// 
    /// Metadata values should remain renderer-neutral and should not be formatted for display here.
    /// </remarks>
    void SetMetadata(IDocumentMetadata metadata);

    /// <summary>
    /// Adds a page to the document.
    /// </summary>
    /// <param name="pageNumber">The source page number.</param>
    /// <param name="size">The optional page size.</param>
    /// <param name="id">An optional stable page identifier.</param>
    /// <returns>The created page model.</returns>
    /// <remarks>
    /// Pages should be added in source order. If page geometry is known, provide it here so that
    /// downstream renderers and OCR fallbacks can reuse the same coordinates.
    /// </remarks>
    IDocumentPage AddPage(int pageNumber, IDocumentSize? size = null, string? id = null);

    /// <summary>
    /// Adds a text block to an existing page.
    /// </summary>
    /// <param name="page">The page that should receive the text block.</param>
    /// <param name="text">The normalized text content.</param>
    /// <param name="bounds">Optional block bounds.</param>
    /// <param name="id">An optional stable block identifier.</param>
    /// <returns>The created text block model.</returns>
    /// <remarks>
    /// Text should be normalized as far as practical, but the importer should avoid inventing
    /// content that is not present in the source.
    /// </remarks>
    IDocumentTextBlock AddTextBlock(IDocumentPage page, string text, IDocumentBounds? bounds = null, string? id = null);

    /// <summary>
    /// Adds an image block to an existing page.
    /// </summary>
    /// <param name="page">The page that should receive the image block.</param>
    /// <param name="sourceObjectReference">A reference back to the source object or stream.</param>
    /// <param name="exportedImagePath">The exported image path or URI.</param>
    /// <param name="bounds">Optional block bounds.</param>
    /// <param name="id">An optional stable block identifier.</param>
    /// <returns>The created image block model.</returns>
    /// <remarks>
    /// Image blocks should reference both the original source object and the exported image when
    /// available so that the document can be rendered, inspected, or passed to OCR later.
    /// </remarks>
    IDocumentImageBlock AddImageBlock(IDocumentPage page, string? sourceObjectReference = null, string? exportedImagePath = null, IDocumentBounds? bounds = null, string? id = null);

    /// <summary>
    /// Adds a drawing block to an existing page.
    /// </summary>
    /// <param name="page">The page that should receive the drawing block.</param>
    /// <param name="sourceStreamReference">A reference to the source stream or object.</param>
    /// <param name="renderHint">An optional render hint for downstream consumers.</param>
    /// <param name="bounds">Optional block bounds.</param>
    /// <param name="id">An optional stable block identifier.</param>
    /// <returns>The created drawing block model.</returns>
    /// <remarks>
    /// Drawing blocks are used for vector content, outline-based glyphs, and other content that
    /// must be rendered before it can be interpreted by OCR or a downstream analysis step.
    /// </remarks>
    IDocumentDrawingBlock AddDrawingBlock(IDocumentPage page, string? sourceStreamReference = null, string? renderHint = null, IDocumentBounds? bounds = null, string? id = null);

    /// <summary>
    /// Adds a reusable resource to the document.
    /// </summary>
    /// <param name="kind">The resource kind.</param>
    /// <param name="name">An optional resource name.</param>
    /// <param name="uri">An optional resource URI.</param>
    /// <param name="contentType">An optional resource content type.</param>
    /// <param name="sourceReference">An optional source reference.</param>
    /// <returns>The created resource model.</returns>
    /// <remarks>
    /// Resources are intended for exported images, OCR input artifacts, or other reusable files
    /// that remain linked to the document graph.
    /// </remarks>
    IDocumentResource AddResource(string kind, string? name = null, string? uri = null, string? contentType = null, string? sourceReference = null);

    /// <summary>
    /// Adds an analysis hint to the document.
    /// </summary>
    /// <param name="category">The hint category.</param>
    /// <param name="value">The hint value.</param>
    /// <param name="confidence">The optional confidence value.</param>
    /// <param name="origin">The optional origin of the hint.</param>
    /// <returns>The created analysis hint model.</returns>
    /// <remarks>
    /// Hints should capture routing or classifier output, such as text quality, image content, or
    /// OCR fallback decisions.
    /// </remarks>
    IDocumentAnalysisHint AddAnalysisHint(string category, string value, double? confidence = null, string? origin = null);
}
