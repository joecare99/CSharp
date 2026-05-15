using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents a width/height pair with an optional unit label.
/// </summary>
/// <remarks>
/// The unit label can be used to distinguish pixels, points, millimeters, or other coordinate systems.
/// </remarks>
public sealed record DocumentSizeModel(
    double Width,
    double Height,
    string? Unit = null) : IDocumentSize;
