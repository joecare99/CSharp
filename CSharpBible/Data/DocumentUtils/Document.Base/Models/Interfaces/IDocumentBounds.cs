namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a rectangular region in a document coordinate system.
/// </summary>
/// <remarks>
/// Bounds describe a page-local region and can be used for rendering, OCR cropping, hit-testing,
/// or debugging layout extraction. Consumers should interpret the coordinates in the associated
/// coordinate system.
/// </remarks>
public interface IDocumentBounds
{
    /// <summary>
    /// Gets the X coordinate of the region.
    /// </summary>
    double X { get; }

    /// <summary>
    /// Gets the Y coordinate of the region.
    /// </summary>
    double Y { get; }

    /// <summary>
    /// Gets the region width.
    /// </summary>
    double Width { get; }

    /// <summary>
    /// Gets the region height.
    /// </summary>
    double Height { get; }

    /// <summary>
    /// Gets the coordinate system label.
    /// </summary>
    /// <remarks>
    /// Examples include PDF points, pixels, or another importer-defined coordinate system.
    /// </remarks>
    string? CoordinateSystem { get; }
}
