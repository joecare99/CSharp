namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a generic block in a document page.
/// </summary>
/// <remarks>
/// Blocks are page-local semantic units. Concrete block types represent text, images, drawings,
/// or other derived regions that share common placement and source-reference behavior.
/// </remarks>
public interface IDocumentBlock : IDocumentNode
{
    /// <summary>
    /// Gets the optional geometric bounds of the block.
    /// </summary>
    /// <remarks>
    /// Bounds are useful for layout reconstruction, OCR cropping, and visual debugging. They are
    /// optional because not every importer can provide precise coordinates.
    /// </remarks>
    IDocumentBounds? Bounds { get; }

    /// <summary>
    /// Gets the optional source reference for the block.
    /// </summary>
    /// <remarks>
    /// This can point back to a PDF object, stream, region, or other importer-specific source
    /// identifier that helps correlate the block with the original document data.
    /// </remarks>
    string? SourceReference { get; }
}
