using Document.Base.Models.Enums;
using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents a single page in the canonical document model.
/// </summary>
/// <remarks>
/// Pages preserve the source order of the document and act as ordered containers for text,
/// image, and drawing blocks. They are intentionally lightweight and do not try to encode
/// rendering details beyond size and block ordering.
/// </remarks>
public sealed class DocumentPageModel : IDocumentPage
{
    private readonly List<IDocumentBlock> _blocks = [];

    /// <summary>
    /// Initializes a new page model.
    /// </summary>
    /// <param name="pageNumber">The source page number.</param>
    /// <param name="id">An optional stable identifier.</param>
    public DocumentPageModel(int pageNumber, string? id = null)
    {
        PageNumber = pageNumber;
        Id = id;
    }

    /// <summary>
    /// Gets the source page number.
    /// </summary>
    public int PageNumber { get; }

    /// <inheritdoc />
    public string? Id { get; }

    /// <inheritdoc />
    public DocumentElementKind Kind => DocumentElementKind.Page;

    /// <inheritdoc />
    public IReadOnlyList<IDocumentBlock> Children => _blocks;

    /// <summary>
    /// Gets or sets the physical or logical size of the page.
    /// </summary>
    public IDocumentSize? Size { get; set; }

    /// <summary>
    /// Adds a block to the page in source order.
    /// </summary>
    /// <param name="block">The block to add.</param>
    /// <returns>The added block.</returns>
    public DocumentBlockModel AddBlock(DocumentBlockModel block)
    {
        ArgumentNullException.ThrowIfNull(block);
        _blocks.Add(block);
        return block;
    }

    IReadOnlyList<IDocumentBlock> IDocumentContainer<IDocumentBlock>.Children => _blocks;
}
