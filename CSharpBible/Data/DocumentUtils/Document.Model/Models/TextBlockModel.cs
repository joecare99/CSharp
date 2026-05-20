using Document.Base.Models.Enums;
using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents a text-bearing block in the document model.
/// </summary>
/// <remarks>
/// The text value should contain the normalized textual content for the block. Formatting and
/// layout hints can be preserved separately so that renderers can decide how much of the original
/// structure to reconstruct.
/// </remarks>
public sealed class TextBlockModel : DocumentBlockModel, IDocumentTextBlock
{
    /// <summary>
    /// Initializes a new text block.
    /// </summary>
    /// <param name="id">An optional stable identifier.</param>
    public TextBlockModel(string? id = null)
        : base(DocumentElementKind.TextBlock, id)
    {
    }

    /// <summary>
    /// Gets or sets the normalized text content of the block.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional language tag or language hint.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets a layout hint that can help a renderer preserve or rebuild structure.
    /// </summary>
    public string? LayoutHint { get; set; }
}
