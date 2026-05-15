namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a vector or drawing-based block in the document model.
/// </summary>
/// <remarks>
/// Drawing blocks are used for vector paths, outline glyphs, shapes, diagrams, and other content
/// that must be rendered before it can be interpreted semantically.
/// </remarks>
public interface IDocumentDrawingBlock : IDocumentBlock
{
    /// <summary>
    /// Gets the reference to the source stream or object that produced the drawing.
    /// </summary>
    string? SourceStreamReference { get; }

    /// <summary>
    /// Gets the rendering hint for the drawing.
    /// </summary>
    /// <remarks>
    /// A rendering hint can tell a downstream renderer whether the block is likely to need OCR,
    /// rasterization, special scaling, or other processing.
    /// </remarks>
    string? RenderHint { get; }
}
