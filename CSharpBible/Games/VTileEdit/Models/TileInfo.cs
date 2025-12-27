using System;
using System.Collections.Generic;
using System.Linq;

namespace VTileEdit.Models;

/// <summary>
/// Describes metadata associated with a tile definition.
/// </summary>
public sealed class TileInfo
{
    /// <summary>
    /// Gets a default tile info instance.
    /// </summary>
    public static TileInfo Default => new();

    /// <summary>
    /// Gets or sets the tile category.
    /// </summary>
    public TileCategory Category { get; set; } = TileCategory.Unknown;

    /// <summary>
    /// Gets or sets the optional sub-category label.
    /// </summary>
    public string SubCategory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the arbitrary tag collection associated with the tile.
    /// </summary>
    public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Creates a deep copy of the current instance.
    /// </summary>
    public TileInfo Clone()
        => new()
        {
            Category = Category,
            SubCategory = SubCategory,
            Tags = Tags?.ToArray() ?? Array.Empty<string>()
        };

    internal static TileInfo Normalize(TileInfo? info)
    {
        if (info == null)
        {
            return new TileInfo();
        }

        var tags = info.Tags?
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? Array.Empty<string>();

        return new TileInfo
        {
            Category = info.Category,
            SubCategory = info.SubCategory?.Trim() ?? string.Empty,
            Tags = tags
        };
    }
}
