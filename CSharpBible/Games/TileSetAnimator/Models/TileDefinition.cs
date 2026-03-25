using System.Windows;

namespace TileSetAnimator.Models;

/// <summary>
/// Represents a single tile extracted from the sprite sheet.
/// </summary>
public sealed record TileDefinition(int Index, int Row, int Column, Int32Rect Bounds, string Name, string Notes, TileCategory Category, string SubCategory);
