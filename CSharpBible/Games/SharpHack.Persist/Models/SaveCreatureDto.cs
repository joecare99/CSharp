using System.Collections.Generic;

namespace SharpHack.Persist.Models;

/// <summary>
/// Represents a serialized creature including combat state, inventory, and equipment references.
/// </summary>
public sealed class SaveCreatureDto
{
    /// <summary>
    /// Gets or sets the stable creature identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the concrete creature type discriminator.
    /// </summary>
    public string CreatureType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creature position.
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
    /// Gets or sets current hit points.
    /// </summary>
    public int HP { get; set; }

    /// <summary>
    /// Gets or sets maximum hit points.
    /// </summary>
    public int MaxHP { get; set; }

    /// <summary>
    /// Gets or sets base attack.
    /// </summary>
    public int BaseAttack { get; set; }

    /// <summary>
    /// Gets or sets base defense.
    /// </summary>
    public int BaseDefense { get; set; }

    /// <summary>
    /// Gets or sets movement speed.
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// Gets or sets the inventory item identifiers.
    /// </summary>
    public List<string> InventoryItemIds { get; set; } = [];

    /// <summary>
    /// Gets or sets the current equipment references.
    /// </summary>
    public SaveEquipmentDto Equipment { get; set; } = new();
}
