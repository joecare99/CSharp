using AA98_AvlnCodeStudio.Planning.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace AA98_AvlnCodeStudio.Planning.UI.ViewModels;

/// <summary>
/// Represents a single planning tree node in the planning UI component.
/// </summary>
public sealed class PlanningTreeItemViewModel
{
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

    public PlanningTreeItemViewModel(string sourcePath)
    {
        SourcePath = sourcePath;
        Id = sourcePath;
        Title = Path.GetFileName(sourcePath);
        Kind = PlanningItemKind.Unknown;
        Status = PlanningItemStatus.Unknown;
    }

    public bool IsVirtualNode => string.Equals(Id, SourcePath, StringComparison.OrdinalIgnoreCase);

    public string Id { get; }

    public string Title { get; }

    public PlanningItemKind Kind { get; }

    public PlanningItemStatus Status { get; }

    public string SourcePath { get; }

    public string? ParentId { get; }

    public string? ParentSourcePath { get; }

    public ObservableCollection<PlanningTreeItemViewModel> Children { get; } = [];
}
