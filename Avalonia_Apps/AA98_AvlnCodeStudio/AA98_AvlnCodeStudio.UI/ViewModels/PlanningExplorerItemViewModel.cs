using AA98_AvlnCodeStudio.Base.Planning.Models;
using System;
using System.Collections.ObjectModel;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>
/// Represents a single planning item node for UI hierarchy navigation.
/// </summary>
public sealed class PlanningExplorerItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlanningExplorerItemViewModel"/> class.
    /// </summary>
    /// <param name="item">The source planning item.</param>
    public PlanningExplorerItemViewModel(PlanningItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Id = item.Id;
        Title = item.Title;
        Kind = item.Kind;
        Status = item.Status;
        SourcePath = item.SourcePath;
        ParentId = item.Parent?.ItemId;
    }

    /// <summary>
    /// Gets the stable planning item identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the planning item title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the normalized planning item kind.
    /// </summary>
    public PlanningItemKind Kind { get; }

    /// <summary>
    /// Gets the normalized planning item status.
    /// </summary>
    public PlanningItemStatus Status { get; }

    /// <summary>
    /// Gets the repository-relative source path for the planning item.
    /// </summary>
    public string SourcePath { get; }

    /// <summary>
    /// Gets the optional parent planning item identifier.
    /// </summary>
    public string? ParentId { get; }

    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    public ObservableCollection<PlanningExplorerItemViewModel> Children { get; } = [];
}
