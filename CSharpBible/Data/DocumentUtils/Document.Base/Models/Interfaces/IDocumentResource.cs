namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a reusable document artifact such as an exported image or OCR input.
/// </summary>
/// <remarks>
/// Resources are the link between the semantic document graph and external or generated assets.
/// They are useful for exported images, OCR input files, previews, and other reusable artifacts
/// that should remain attached to the document.
/// </remarks>
public interface IDocumentResource
{
    /// <summary>
    /// Gets the resource kind.
    /// </summary>
    /// <remarks>
    /// The kind should be a stable classification such as image, preview, OCR input, or output.
    /// </remarks>
    string Kind { get; }

    /// <summary>
    /// Gets the optional resource name.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets the optional resource URI.
    /// </summary>
    string? Uri { get; }

    /// <summary>
    /// Gets the optional resource content type.
    /// </summary>
    string? ContentType { get; }

    /// <summary>
    /// Gets the optional source reference.
    /// </summary>
    string? SourceReference { get; }
}
