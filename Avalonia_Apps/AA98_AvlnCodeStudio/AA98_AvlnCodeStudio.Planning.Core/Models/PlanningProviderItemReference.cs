namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Identifies a planning item in an external adapter independently from its local identifier.
/// </summary>
public sealed class PlanningProviderItemReference
{
    /// <summary>
    /// Gets or sets the identifier of the adapter that owns the external item.
    /// </summary>
    public string AdapterId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider-defined item identifier.
    /// </summary>
    public string ProviderItemId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional provider-defined project, repository, or scope identifier.
    /// </summary>
    public string ScopeId { get; set; } = string.Empty;
}
