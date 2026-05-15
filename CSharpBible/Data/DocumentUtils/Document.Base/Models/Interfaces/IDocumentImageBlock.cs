namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents an image-derived block in the document model.
/// </summary>
/// <remarks>
/// Image blocks link the logical document graph to exported image assets and can carry OCR output
/// or a short internal analysis summary for downstream routing or user-facing descriptions.
/// </remarks>
public interface IDocumentImageBlock : IDocumentBlock
{
    /// <summary>
    /// Gets the source object reference if the image came from an embedded source object.
    /// </summary>
    string? SourceObjectReference { get; }

    /// <summary>
    /// Gets the exported image path or URI.
    /// </summary>
    /// <remarks>
    /// The exported image is typically the artifact that can be passed to OCR or image analysis.
    /// </remarks>
    string? ExportedImagePath { get; }

    /// <summary>
    /// Gets the OCR text that was produced for this image, if available.
    /// </summary>
    string? OcrText { get; }

    /// <summary>
    /// Gets the short internal analysis summary, if available.
    /// </summary>
    /// <remarks>
    /// This can describe whether the image appears to be a photo, drawing, screenshot, or face
    /// candidate without committing to a specific UI representation.
    /// </remarks>
    string? AnalysisSummary { get; }
}
