using Document.Base.Models.Enums;
using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents a vector or drawing-based block in the document model.
/// </summary>
/// <remarks>
/// Drawing blocks are useful when a PDF contains path-based text, icons, diagrams, or outline
/// content that cannot be treated as plain text. They can later be rendered or analyzed further
/// by OCR or by a vector-aware layout processor.
/// </remarks>
public sealed class DrawingBlockModel : DocumentBlockModel, IDocumentDrawingBlock
{
    /// <summary>
    /// Initializes a new drawing block.
    /// </summary>
    /// <param name="id">An optional stable identifier.</param>
    public DrawingBlockModel(string? id = null)
        : base(DocumentElementKind.DrawingBlock, id)
    {
    }

    /// <summary>
    /// Gets or sets a reference to the source stream or object that produced the drawing.
    /// </summary>
    public string? SourceStreamReference { get; set; }

    /// <summary>
    /// Gets or sets a rendering hint for downstream renderers.
    /// </summary>
    public string? RenderHint { get; set; }
}
