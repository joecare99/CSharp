using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents a reusable artifact attached to the document graph.
/// </summary>
/// <remarks>
/// Resources are used for exported images, rendered blocks, OCR inputs, or other derived
/// artifacts that need to stay linked to the logical document structure.
/// </remarks>
public sealed record DocumentResourceModel(
    /// <summary>
    /// The resource kind, for example image, OCR artifact, or render output.
    /// </summary>
    string Kind,
    /// <summary>
    /// An optional friendly name for the resource.
    /// </summary>
    string? Name = null,
    /// <summary>
    /// An optional URI or path to the resource.
    /// </summary>
    string? Uri = null,
    /// <summary>
    /// An optional MIME type or content type.
    /// </summary>
    string? ContentType = null,
    /// <summary>
    /// An optional internal source reference.
    /// </summary>
    string? SourceReference = null) : IDocumentResource;
