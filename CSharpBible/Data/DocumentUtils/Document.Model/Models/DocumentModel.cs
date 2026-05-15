using Document.Base.Models.Enums;
using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents the root of the shared document graph.
/// </summary>
/// <remarks>
/// This class belongs to <c>Document.Model</c>, not <c>Document.Base</c>, because it holds the
/// richer canonical document structure. The base project only defines the minimal contracts that
/// this model implements.
/// 
/// <para>
/// The intended population flow is importer-driven. For a PDF pipeline, the importer maps
/// source-level information into the model as follows:
/// </para>
/// <list type="bullet">
/// <item><description>PDF file path, file name, or logical source name into <see cref="SourcePath"/> and <see cref="SourceName"/>.</description></item>
/// <item><description>PDF metadata entries into <see cref="Metadata"/>.</description></item>
/// <item><description>PDF pages into <see cref="Children"/> via <see cref="AddPage(int)"/>.</description></item>
/// <item><description>Extracted text into text blocks on the relevant page models.</description></item>
/// <item><description>Image XObjects into resources and image blocks.</description></item>
/// <item><description>Vector or drawing streams into drawing blocks.</description></item>
/// <item><description>OCR results and image-analysis output into resources and analysis hints.</description></item>
/// </list>
/// 
/// The document model itself stays renderer-neutral. Markdown, HTML, JSON, OCR, and assistant
/// workflows should all consume this structure rather than encode their output format here.
/// </remarks>
public sealed class DocumentModel : IDocumentModel
{
    private readonly List<IDocumentPage> _pages = [];

    /// <summary>
    /// Initializes a new document model.
    /// </summary>
    /// <param name="id">An optional stable identifier for the document.</param>
    /// <remarks>
    /// The identifier is useful for correlating a parsed document with downstream artifacts,
    /// but it is intentionally separate from the source path so that in-memory and virtual
    /// documents can still be modeled consistently.
    /// </remarks>
    public DocumentModel(string? id = null)
    {
        Id = id;
    }

    /// <inheritdoc />
    public string? Id { get; }

    /// <inheritdoc />
    public DocumentElementKind Kind => DocumentElementKind.Document;

    /// <inheritdoc />
    public string? SourcePath { get; set; }

    /// <inheritdoc />
    public string? SourceName { get; set; }

    /// <inheritdoc />
    public string? MediaType { get; set; }

    /// <inheritdoc />
    public IReadOnlyList<IDocumentPage> Children => _pages;

    /// <summary>
    /// Gets or sets the optional document metadata payload.
    /// </summary>
    /// <remarks>
    /// For a PDF importer, this is where document-level facts such as title, author, producer,
    /// creation date, and source filename should be mapped.
    /// </remarks>
    public IDocumentMetadata Metadata { get; set; }

    /// <summary>
    /// Gets the resources that belong to the document.
    /// </summary>
    /// <remarks>
    /// Resources typically represent exported images, generated artifacts, OCR inputs, or other
    /// reusable document assets that can be referenced from blocks or renderers.
    /// </remarks>
    public IReadOnlyList<IDocumentResource> Resources => _resources;

    private readonly List<IDocumentResource> _resources = [];
    private readonly List<IDocumentAnalysisHint> _analysisHints = [];

    /// <summary>
    /// Gets the analysis hints attached to the document.
    /// </summary>
    /// <remarks>
    /// Hints are intended for downstream routing or interpretation, for example whether a page
    /// should use OCR fallback, whether text extraction is meaningful, or whether an image might
    /// contain a face or represent a drawing.
    /// </remarks>
    public IReadOnlyList<IDocumentAnalysisHint> AnalysisHints => _analysisHints;

    /// <summary>
    /// Adds a page to the document in source order.
    /// </summary>
    /// <param name="pageNumber">The page number as represented in the source document.</param>
    /// <returns>The created page model.</returns>
    /// <remarks>
    /// Importers should call this in the same order that pages appear in the source document so
    /// that traversal and rendering can rely on the stored sequence.
    /// </remarks>
    public DocumentPageModel AddPage(int pageNumber)
    {
        DocumentPageModel page = new(pageNumber);
        _pages.Add(page);
        return page;
    }

    /// <summary>
    /// Adds a reusable resource to the document.
    /// </summary>
    /// <param name="kind">The resource category, such as image, render output, or OCR artifact.</param>
    /// <param name="name">An optional friendly name.</param>
    /// <param name="uri">An optional URI or path to the resource.</param>
    /// <returns>The created resource model.</returns>
    /// <remarks>
    /// Use resources when a derived artifact needs to stay linked to the document graph but is
    /// not itself a primary page block.
    /// </remarks>
    public DocumentResourceModel AddResource(string kind, string? name = null, string? uri = null)
    {
        DocumentResourceModel resource = new(kind, name, uri);
        _resources.Add(resource);
        return resource;
    }

    /// <summary>
    /// Adds an analysis hint to the document.
    /// </summary>
    /// <param name="category">The hint category.</param>
    /// <param name="value">The hint value.</param>
    /// <param name="confidence">An optional confidence score.</param>
    /// <param name="origin">An optional origin description.</param>
    /// <returns>The created hint model.</returns>
    /// <remarks>
    /// Importers and analyzers can use this to record findings such as text quality, image type,
    /// OCR fallback decisions, or LLM-based image classifications.
    /// </remarks>
    public DocumentAnalysisHintModel AddAnalysisHint(string category, string value, double? confidence = null, string? origin = null)
    {
        DocumentAnalysisHintModel hint = new(category, value, confidence, origin);
        _analysisHints.Add(hint);
        return hint;
    }
}
