namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Represents a provider-neutral planning relationship reference.
/// </summary>
public sealed class PlanningItemLink
{
    /// <summary>
    /// Gets or sets the stable identifier of the linked planning item.
    /// </summary>
    public string ItemId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional title of the linked planning item.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the optional normalized kind of the linked planning item.
    /// </summary>
    public PlanningItemKind Kind { get; set; } = PlanningItemKind.Unknown;

    /// <summary>
    /// Gets or sets the optional local source path of the linked planning item.
    /// </summary>
    public string? SourcePath { get; set; }
}