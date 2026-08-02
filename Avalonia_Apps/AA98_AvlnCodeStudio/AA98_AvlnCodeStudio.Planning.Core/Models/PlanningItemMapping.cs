namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Maps a local planning item to an external provider item identity.
/// </summary>
public sealed class PlanningItemMapping
{
    /// <summary>
    /// Gets or sets the stable local planning item identifier.
    /// </summary>
    public string LocalItemId { get; set; } = string.Empty;

    /// <summary>
    /// Gets the external provider item identity.
    /// </summary>
    public PlanningProviderItemReference ProviderItem { get; } = new();
}
