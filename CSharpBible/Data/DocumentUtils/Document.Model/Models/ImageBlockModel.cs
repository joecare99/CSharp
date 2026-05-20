using Document.Base.Models.Enums;
using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents an image-derived block in the document model.
/// </summary>
/// <remarks>
/// The image block can point to an exported image file, an original PDF object, an OCR result,
/// or a downstream analysis summary. This allows the image to remain part of the same document
/// graph while still supporting separate export and LLM-based evaluation steps.
/// </remarks>
public sealed class ImageBlockModel : DocumentBlockModel, IDocumentImageBlock
{
    /// <summary>
    /// Initializes a new image block.
    /// </summary>
    /// <param name="id">An optional stable identifier.</param>
    public ImageBlockModel(string? id = null)
        : base(DocumentElementKind.ImageBlock, id)
    {
    }

    /// <summary>
    /// Gets or sets a reference to the original source object or stream.
    /// </summary>
    public string? SourceObjectReference { get; set; }

    /// <summary>
    /// Gets or sets the exported image path or URI.
    /// </summary>
    public string? ExportedImagePath { get; set; }

    /// <summary>
    /// Gets or sets OCR text produced from this image when OCR fallback is required.
    /// </summary>
    public string? OcrText { get; set; }

    /// <summary>
    /// Gets or sets a short internal analysis summary.
    /// </summary>
    public string? AnalysisSummary { get; set; }
}
