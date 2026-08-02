using AppKomponentBaseLib.Diagnostics;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Represents the provider-neutral result of synchronizing planning items.
/// </summary>
public sealed class PlanningSynchronizationResult
{
    /// <summary>
    /// Gets the planning items imported from the external adapter.
    /// </summary>
    public IList<PlanningItem> ImportedItems { get; } = new List<PlanningItem>();

    /// <summary>
    /// Gets the resulting local-to-provider item mappings.
    /// </summary>
    public IList<PlanningItemMapping> ItemMappings { get; } = new List<PlanningItemMapping>();

    /// <summary>
    /// Gets diagnostics raised during synchronization.
    /// </summary>
    public IList<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();
}
