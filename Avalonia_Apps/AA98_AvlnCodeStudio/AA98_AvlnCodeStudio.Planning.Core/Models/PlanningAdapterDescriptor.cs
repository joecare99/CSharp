namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Describes an external planning adapter without exposing provider-specific types.
/// </summary>
public sealed class PlanningAdapterDescriptor
{
    /// <summary>
    /// Gets or sets the stable identifier of the adapter implementation.
    /// </summary>
    public string AdapterId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the adapter.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the operations supported by the adapter.
    /// </summary>
    public PlanningAdapterCapability Capabilities { get; set; }
}
