using CommunityToolkit.Mvvm.ComponentModel;
using Project.Explorer;
using System;
using System.Collections.ObjectModel;

namespace Project.Explorer.Avalonia.ViewModels;

/// <summary>Provides observable presentation state for one neutral explorer item.</summary>
public sealed partial class ProjectExplorerItemViewModel : ObservableObject
{
    /// <summary>Initializes a view model from an immutable explorer item.</summary>
    public ProjectExplorerItemViewModel(ProjectExplorerItem item)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        foreach (ProjectExplorerItem child in item.Children)
        {
            Children.Add(new ProjectExplorerItemViewModel(child));
        }
    }

    /// <summary>Gets the neutral item represented by this view model.</summary>
    public ProjectExplorerItem Item { get; }

    /// <summary>Gets the item identity used for view-state persistence.</summary>
    public string Id => Item.Id;

    /// <summary>Gets the source-provided display name.</summary>
    public string Name => Item.Name;

    /// <summary>Gets the kind of project hierarchy item.</summary>
    public ProjectExplorerItemKind Kind => Item.Kind;

    /// <summary>Gets whether the source could access the backing path.</summary>
    public bool IsAvailable => Item.IsAvailable;

    /// <summary>Gets child items in source order.</summary>
    public ObservableCollection<ProjectExplorerItemViewModel> Children { get; } = [];

    /// <summary>Gets or sets whether this item is expanded in the UI.</summary>
    [ObservableProperty]
    private bool _isExpanded;
}
