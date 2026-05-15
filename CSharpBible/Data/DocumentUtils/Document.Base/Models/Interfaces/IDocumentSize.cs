namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a width/height pair with an optional unit label.
/// </summary>
/// <remarks>
/// The size abstraction is intentionally minimal so that it can represent page geometry, image
/// sizes, or other rectangular dimensions without depending on a specific coordinate system.
/// </remarks>
public interface IDocumentSize
{
    /// <summary>
    /// Gets the width.
    /// </summary>
    double Width { get; }

    /// <summary>
    /// Gets the height.
    /// </summary>
    double Height { get; }

    /// <summary>
    /// Gets the optional unit label.
    /// </summary>
    /// <remarks>
    /// The unit may be pixels, points, millimeters, or another domain-specific label.
    /// </remarks>
    string? Unit { get; }
}
