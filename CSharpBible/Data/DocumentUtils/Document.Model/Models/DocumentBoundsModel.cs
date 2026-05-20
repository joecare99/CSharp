using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents a rectangular region within a document or page coordinate system.
/// </summary>
/// <remarks>
/// Bounds are used for layout, rendering, OCR cropping, and image-to-page linkage. The coordinate
/// system label helps consumers interpret the values consistently across different sources.
/// </remarks>
public sealed record DocumentBoundsModel(
    double X,
    double Y,
    double Width,
    double Height,
    string? CoordinateSystem = null) : IDocumentBounds;
