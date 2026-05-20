using System.Collections.Generic;

namespace SharpHack.Persist.Models;

/// <summary>
/// Represents a serialized gameplay item including type data and nested container content.
/// </summary>
public sealed class SaveItemDto
{
    /// <summary>
    /// Gets or sets the stable item identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the concrete item type discriminator.
    /// </summary>
    public string ItemType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item position.
    /// </summary>
    public SavePointDto Position { get; set; } = new();

    /// <summary>
    /// Gets or sets the display symbol.
    /// </summary>
    public char Symbol { get; set; }

    /// <summary>
    /// Gets or sets the console color name.
    /// </summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item weight.
    /// </summary>
    public double Weight { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is stackable.
    /// </summary>
    public bool IsStackable { get; set; }

    /// <summary>
    /// Gets or sets the stack quantity.
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets the optional attack bonus for weapon items.
    /// </summary>
    public int? AttackBonus { get; set; }

    /// <summary>
    /// Gets or sets the optional defense bonus for armor items.
    /// </summary>
    public int? DefenseBonus { get; set; }

    /// <summary>
    /// Gets or sets nested contained items when the item acts as a container.
    /// </summary>
    public List<SaveItemDto> ContainedItems { get; set; } = [];
}
