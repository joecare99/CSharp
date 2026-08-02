using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Represents a provider-neutral request to synchronize planning items with an external adapter.
/// </summary>
public sealed class PlanningSynchronizationRequest
{
    /// <summary>
    /// Gets or sets the identifier of the adapter that performs the operation.
    /// </summary>
    public string AdapterId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider-defined project, repository, or scope identifier.
    /// </summary>
    public string ScopeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the requested synchronization direction.
    /// </summary>
    public PlanningSynchronizationMode Mode { get; set; }

    /// <summary>
    /// Gets the local-to-provider item mappings known before synchronization.
    /// </summary>
    public IList<PlanningItemMapping> ItemMappings { get; } = new List<PlanningItemMapping>();

    /// <summary>
    /// Gets the local planning items to export when the mode supports export.
    /// </summary>
    public IList<PlanningItem> Items { get; } = new List<PlanningItem>();
}
