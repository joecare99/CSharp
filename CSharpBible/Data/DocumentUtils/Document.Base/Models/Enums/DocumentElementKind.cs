namespace Document.Base.Models.Enums;

/// <summary>
/// Describes the coarse-grained role of a node within the shared document model.
/// </summary>
/// <remarks>
/// The enum is intentionally small and stable so that the base layer can stay lightweight.
/// More specialized semantic information belongs in the higher-level <c>Document.Model</c>
/// project or in dedicated analysis artifacts.
/// </remarks>
public enum DocumentElementKind
{
    /// <summary>
    /// The root document node.
    /// </summary>
    Document,

    /// <summary>
    /// A page-level container in the document structure.
    /// </summary>
    Page,

    /// <summary>
    /// A text-oriented block or region.
    /// </summary>
    TextBlock,

    /// <summary>
    /// A raster or embedded image block.
    /// </summary>
    ImageBlock,

    /// <summary>
    /// A vector-drawing or path-based block.
    /// </summary>
    DrawingBlock,

    /// <summary>
    /// A reusable resource attached to the document graph.
    /// </summary>
    Resource,

    /// <summary>
    /// A metadata node or metadata payload.
    /// </summary>
    Metadata,

    /// <summary>
    /// An analysis result or annotation attached to the document.
    /// </summary>
    Annotation
}
