using System;
using System.Collections.Generic;
using TileSetAnimator.Models;

namespace TileSetAnimator.Persistence;

/// <summary>
/// Captures a single tile's metadata for persistence.
/// </summary>
public sealed class TileMetadataSnapshot
{
    public int Index { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public TileCategory Category { get; init; } = TileCategory.Unknown;

    public string? SubCategory { get; init; }

    /// <summary>
    /// Optional background tile index to use for cutout computation for this tile.
    /// </summary>
    public int? CutoutBackgroundTileIndex { get; init; }
}

/// <summary>
/// Captures the animation definition in a serializable form.
/// </summary>
public sealed class TileAnimationSnapshot
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public List<int> FrameIndices { get; init; } = new();

    public int FrameDuration { get; init; }
}

/// <summary>
/// Stores the state of a single mini map slot.
/// </summary>
public sealed class MiniMapSlotSnapshot
{
    public int Row { get; init; }

    public int Column { get; init; }

    public int? TileIndex { get; init; }

    public string? SuggestionKey { get; init; }
}

/// <summary>
/// Stores the serialized representation of a named mini map.
/// </summary>
public sealed class MiniMapDefinitionSnapshot
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; init; } = string.Empty;

    public List<MiniMapSlotSnapshot> Slots { get; init; } = new();
}

/// <summary>
/// Represents all persisted state for a tile sheet.
/// </summary>
public sealed class TileSetState
{
    public string TileSheetPath { get; init; } = string.Empty;

    public string? TileSetClassKey { get; init; }

    public TileGridSettings Grid { get; init; }

    public int MinimumAnimationLength { get; init; } = 2;

    public int FrameDuration { get; init; } = 120;

    public double ZoomFactor { get; init; } = 4;

    public List<TileMetadataSnapshot> Tiles { get; init; } = new();

    public List<TileAnimationSnapshot> Animations { get; init; } = new();

    /// <summary>
    /// The legacy single mini map serialized prior to collections being introduced.
    /// </summary>
    [Obsolete("Use MiniMaps instead.")]
    public List<MiniMapSlotSnapshot> MiniMap { get; init; } = new();

    public List<MiniMapDefinitionSnapshot> MiniMaps { get; init; } = new();

    public Guid? SelectedMiniMapId { get; init; }
}

/// <summary>
/// Describes a shared metadata definition that multiple tile sets can reference.
/// </summary>
public sealed class TileSetClassMetadata
{
    public string Key { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public List<TileMetadataSnapshot> Tiles { get; init; } = new();

    public Dictionary<TileCategory, List<string>> CategoryDefinitions { get; set; } = new();
}
