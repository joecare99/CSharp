using AppKomponentBaseLib.Diagnostics;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Represents the provider-neutral result of loading planning items.
/// </summary>
public sealed class PlanningReadResult
{
    /// <summary>
    /// Gets or sets the repository root path used for the read operation.
    /// </summary>
    public string RepositoryRootPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resolved planning root path used for the read operation.
    /// </summary>
    public string PlanningRootPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets the loaded planning items.
    /// </summary>
    public IList<PlanningItem> Items { get; } = new List<PlanningItem>();

    /// <summary>
    /// Gets the read-level diagnostics not tied to a single item.
    /// </summary>
    public IList<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();
}
