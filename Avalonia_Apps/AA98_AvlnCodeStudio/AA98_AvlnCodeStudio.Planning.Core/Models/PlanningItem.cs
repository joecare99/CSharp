using AppKomponentBaseLib.Diagnostics;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Represents a provider-neutral local planning item.
/// </summary>
public sealed class PlanningItem
{
    /// <summary>
    /// Gets or sets the stable planning item identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the planning item title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized planning item kind.
    /// </summary>
    public PlanningItemKind Kind { get; set; } = PlanningItemKind.Unknown;

    /// <summary>
    /// Gets or sets the normalized planning item status.
    /// </summary>
    public PlanningItemStatus Status { get; set; } = PlanningItemStatus.Unknown;

    /// <summary>
    /// Gets or sets the repository-relative source path for the planning item.
    /// </summary>
    public string SourcePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional parent link declared for the planning item.
    /// </summary>
    public PlanningItemLink? Parent { get; set; }

    /// <summary>
    /// Gets additional associated or related parent references beyond the main parent link.
    /// </summary>
    public IList<PlanningItemLink> RelatedParents { get; } = new List<PlanningItemLink>();

    /// <summary>
    /// Gets the provider-neutral child links declared for the planning item.
    /// </summary>
    public IList<PlanningItemLink> Children { get; } = new List<PlanningItemLink>();

    /// <summary>
    /// Gets the normalized diagnostics associated with the item.
    /// </summary>
    public IList<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();
}
