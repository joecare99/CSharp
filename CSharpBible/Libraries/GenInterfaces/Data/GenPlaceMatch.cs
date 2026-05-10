using System.Collections.Generic;

namespace GenInterfaces.Data;

/// <summary>
/// Represents a place lookup hit returned by a place authority.
/// </summary>
public sealed class GenPlaceMatch
{
    /// <summary>
    /// Gets the authority-specific external identifier.
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Gets the display name of the place.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the place type when available.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets the parent display name when available.
    /// </summary>
    public string? ParentDisplayName { get; set; }

    /// <summary>
    /// Gets the latitude when available.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Gets the longitude when available.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Gets the hierarchy segments from broader to narrower place scopes.
    /// </summary>
    public IReadOnlyList<string> Hierarchy { get; set; } = System.Array.Empty<string>();

    /// <summary>
    /// Gets the originating authority or dataset name.
    /// </summary>
    public string? Source { get; set; }
}
