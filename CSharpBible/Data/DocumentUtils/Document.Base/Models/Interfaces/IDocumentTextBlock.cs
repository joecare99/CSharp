namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a text block in the document model.
/// </summary>
/// <remarks>
/// Text blocks contain normalized textual content and may also preserve language or layout hints
/// that help downstream renderers reconstruct line breaks, flow, or reading order.
/// </remarks>
public interface IDocumentTextBlock : IDocumentBlock
{
    /// <summary>
    /// Gets the normalized text content.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Gets the optional language hint.
    /// </summary>
    /// <remarks>
    /// This value can be used by search, OCR, or language-aware renderers to select the correct
    /// locale or text-processing strategy.
    /// </remarks>
    string? Language { get; }

    /// <summary>
    /// Gets the optional layout hint.
    /// </summary>
    /// <remarks>
    /// Layout hints can encode information such as paragraph flow, list structure, or line-break
    /// normalization preferences without hard-coding a rendering format.
    /// </remarks>
    string? LayoutHint { get; }
}
