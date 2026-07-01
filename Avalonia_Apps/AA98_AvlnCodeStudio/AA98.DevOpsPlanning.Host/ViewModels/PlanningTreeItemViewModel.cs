using AA98_AvlnCodeStudio.Planning.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace AA98.DevOpsPlanning.Host.ViewModels;

/// <summary>
/// Represents a single planning tree node in the DevOps planning host.
/// </summary>
public sealed class PlanningTreeItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlanningTreeItemViewModel"/> class.
    /// </summary>
    /// <param name="item">The source planning item.</param>
    public PlanningTreeItemViewModel(PlanningItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Id = item.Id;
        Title = item.Title;
        Kind = item.Kind;
        Status = item.Status;
        SourcePath = item.SourcePath;
        ParentId = item.Parent?.ItemId;
        ParentSourcePath = item.Parent?.SourcePath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanningTreeItemViewModel"/> class for external parent files.
    /// </summary>
    /// <param name="sourcePath">The repository-relative source path.</param>
    public PlanningTreeItemViewModel(string sourcePath)
    {
        SourcePath = sourcePath;
        Id = sourcePath;
        Title = Path.GetFileName(sourcePath);
        Kind = PlanningItemKind.Unknown;
        Status = PlanningItemStatus.Unknown;
    }

    /// <summary>
    /// Gets a value indicating whether the node is a virtual external parent node.
    /// </summary>
    public bool IsVirtualNode => string.Equals(Id, SourcePath, StringComparison.OrdinalIgnoreCase);


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
    /// Gets the repository-relative source path.
    /// </summary>
    public string SourcePath { get; }

    /// <summary>
    /// Gets the optional parent identifier.
    /// </summary>
    public string? ParentId { get; }

    /// <summary>
    /// Gets the optional parent source path when the parent is represented by an external file.
    /// </summary>
    public string? ParentSourcePath { get; }

    /// <summary>
    /// Gets child planning nodes.
    /// </summary>
    public ObservableCollection<PlanningTreeItemViewModel> Children { get; } = [];
}
