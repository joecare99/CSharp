namespace SharpHack.Persist.Models;

/// <summary>
/// Represents equipment references for a saved creature.
/// </summary>
public sealed class SaveEquipmentDto
{
    /// <summary>
    /// Gets or sets the equipped main-hand item identifier.
    /// </summary>
    public string MainHandItemId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the equipped body item identifier.
    /// </summary>
    public string BodyItemId { get; set; } = string.Empty;
}
