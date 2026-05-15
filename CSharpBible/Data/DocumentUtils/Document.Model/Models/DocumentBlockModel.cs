using Document.Base.Models.Enums;
using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Base class for all document blocks.
/// </summary>
/// <remarks>
/// A block represents a logically coherent region within a page. Concrete block types describe
/// whether the region contains text, an image, a drawing, or other derived content. The base
/// class intentionally only carries the shared information needed for routing and linkage.
/// </remarks>
public abstract class DocumentBlockModel : IDocumentBlock
{
    /// <summary>
    /// Initializes a new block base instance.
    /// </summary>
    /// <param name="kind">The concrete element kind for the block.</param>
    /// <param name="id">An optional stable identifier.</param>
    protected DocumentBlockModel(DocumentElementKind kind, string? id = null)
    {
        Kind = kind;
        Id = id;
    }

    /// <inheritdoc />
    public string? Id { get; }

    /// <inheritdoc />
    public DocumentElementKind Kind { get; }

    /// <summary>
    /// Gets or sets the block bounds within the page coordinate system.
    /// </summary>
    /// <remarks>
    /// Bounds are optional because some sources can only provide logical grouping without
    /// precise geometric coordinates.
    /// </remarks>
    public DocumentBoundsModel? Bounds { get; set; }

    IDocumentBounds? IDocumentBlock.Bounds => Bounds;

    /// <summary>
    /// Gets or sets the source reference for the block.
    /// </summary>
    /// <remarks>
    /// This is usually an internal reference back to the source object, stream, region, or
    /// analysis unit from which the block was derived.
    /// </remarks>
    public string? SourceReference { get; set; }
}
