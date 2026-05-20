namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a page in the document graph.
/// </summary>
/// <remarks>
/// Pages are ordered children of the document root. They act as containers for blocks and may
/// expose page geometry so that renderers and OCR components can operate in the same coordinate
/// space.
/// </remarks>
public interface IDocumentPage : IDocumentNode, IDocumentContainer<IDocumentBlock>
{
    /// <summary>
    /// Gets the page size or geometry information.
    /// </summary>
    /// <remarks>
    /// The size is optional because some sources may only provide logical pages without reliable
    /// geometry. When present, it should describe the page in the source coordinate system.
    /// </remarks>
    IDocumentSize? Size { get; }
}
