using AA98_AvlnCodeStudio.Planning.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace AA98_AvlnCodeStudio.Planning.UI.ViewModels;

/// <summary>
/// Represents a single planning tree node in the planning UI component.
/// </summary>
public partial class PlanningTreeItemViewModel : ObservableObject
{
    public PlanningTreeItemViewModel(PlanningItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Id = item.Id;
        _title = item.Title;
        Kind = item.Kind;
        _status = item.Status;
        SourcePath = item.SourcePath;
        _documentText = item.DocumentText;
        ParentId = item.Parent?.ItemId;
        ParentSourcePath = item.Parent?.SourcePath;
    }

    public PlanningTreeItemViewModel(string sourcePath)
    {
        SourcePath = sourcePath;
        Id = sourcePath;
        _title = Path.GetFileName(sourcePath);
        Kind = PlanningItemKind.Unknown;
        _status = PlanningItemStatus.Unknown;
    }

    public bool IsVirtualNode => string.Equals(Id, SourcePath, StringComparison.OrdinalIgnoreCase);

    public string Id { get; }

    [ObservableProperty]
    private string _title;

    public PlanningItemKind Kind { get; }

    [ObservableProperty]
    private PlanningItemStatus _status;

    public string SourcePath { get; }

    [ObservableProperty]
    private string _documentText = string.Empty;

    public string? ParentId { get; }

    public string? ParentSourcePath { get; }

    public ObservableCollection<PlanningTreeItemViewModel> Children { get; } = [];
}
