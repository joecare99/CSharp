using AppKomponentBaseLib.Diagnostics;
using AA98_AvlnCodeStudio.Base.UI.Properties;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Planning.UI.ViewModels;

/// <summary>
/// Provides reusable planning explorer state for Avalonia-based planning hosts.
/// </summary>
public partial class PlanningExplorerViewModel : ViewModelBase, IHasProperties
{
    private readonly IPlanningProvider _planningProvider;

    public PlanningExplorerViewModel(IPlanningProvider planningProvider)
    {
        _planningProvider = planningProvider ?? throw new ArgumentNullException(nameof(planningProvider));
    }

    public ObservableCollection<PlanningTreeItemViewModel> RootItems { get; } = [];

    public ObservableCollection<PlanningCategoryGroupViewModel> CategoryGroups { get; } = [];

    public ObservableCollection<Diagnostic> Diagnostics { get; } = [];

    private ObservableCollection<IPropertyItem> PropertyItems { get; } = [];

    public IReadOnlyList<IPropertyItem> Properties => PropertyItems;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedItemId))]
    [NotifyPropertyChangedFor(nameof(SelectedItemTitle))]
    [NotifyPropertyChangedFor(nameof(SelectedItemKind))]
    [NotifyPropertyChangedFor(nameof(SelectedItemStatus))]
    [NotifyPropertyChangedFor(nameof(SelectedItemSourcePath))]
    [NotifyPropertyChangedFor(nameof(SelectedItemParentId))]
    [NotifyPropertyChangedFor(nameof(SelectedItemDocumentText))]
    [NotifyPropertyChangedFor(nameof(Properties))]
    [NotifyPropertyChangedFor(nameof(ExplorerStatusText))]
    private PlanningTreeItemViewModel? _selectedItem;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHierarchyMode))]
    [NotifyPropertyChangedFor(nameof(IsCategoryMode))]
    [NotifyPropertyChangedFor(nameof(ExplorerStatusText))]
    private PlanningExplorerViewMode _viewMode = PlanningExplorerViewMode.Hierarchy;

    [ObservableProperty]
    private string _repositoryRootPath = string.Empty;

    [ObservableProperty]
    private string _planningRootPath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ExplorerStatusText))]
    private string _statusText = string.Empty;

    public bool IsHierarchyMode => ViewMode == PlanningExplorerViewMode.Hierarchy;

    public bool IsCategoryMode => ViewMode == PlanningExplorerViewMode.Category;

    public string? SelectedItemId => SelectedItem?.Id;

    public string? SelectedItemTitle => SelectedItem?.Title;

    public PlanningItemKind? SelectedItemKind => SelectedItem?.Kind;

    public PlanningItemStatus? SelectedItemStatus => SelectedItem?.Status;

    public string? SelectedItemSourcePath => SelectedItem?.SourcePath;

    public string? SelectedItemParentId => SelectedItem?.ParentId;

    public string SelectedItemDocumentText
    {
        get => SelectedItem?.DocumentText ?? string.Empty;
        set
        {
            if (SelectedItem is null)
            {
                return;
            }

            SelectedItem.DocumentText = value ?? string.Empty;
            OnPropertyChanged(nameof(SelectedItemDocumentText));
        }
    }

    public string ExplorerStatusText
    {
        get
        {
            int visibleCount = ViewMode == PlanningExplorerViewMode.Hierarchy
                ? CountHierarchyNodes(RootItems)
                : CategoryGroups.Sum(static group => group.Items.Count);

            string modeText = ViewMode == PlanningExplorerViewMode.Hierarchy ? "Hierarchy" : "Category";
            string selectedText = string.IsNullOrWhiteSpace(SelectedItemId) ? "none" : SelectedItemId;
            return $"{modeText} | Visible: {visibleCount} | Selected: {selectedText} | {StatusText}";
        }
    }

    partial void OnSelectedItemChanged(PlanningTreeItemViewModel? value)
    {
        BuildPropertyItems(value);
    }

    [RelayCommand]
    private void ShowHierarchyView()
    {
        ViewMode = PlanningExplorerViewMode.Hierarchy;
    }

    [RelayCommand]
    private void ShowCategoryView()
    {
        ViewMode = PlanningExplorerViewMode.Category;
    }

    public async Task LoadAsync(PlanningReadRequest request, CancellationToken cancellationToken = default)
    {
        PlanningReadResult result = await _planningProvider.ReadAsync(request, cancellationToken).ConfigureAwait(false);
        RepositoryRootPath = result.RepositoryRootPath;
        PlanningRootPath = result.PlanningRootPath;

        RootItems.Clear();
        CategoryGroups.Clear();
        Diagnostics.Clear();
        SelectedItem = null;

        foreach (Diagnostic diagnostic in result.Diagnostics)
        {
            Diagnostics.Add(diagnostic);
        }

        Dictionary<string, PlanningTreeItemViewModel> nodesById = new(StringComparer.Ordinal);
        Dictionary<string, PlanningTreeItemViewModel> externalParentNodesByPath = new(StringComparer.OrdinalIgnoreCase);
        List<PlanningTreeItemViewModel> allNodes = [];

        foreach (PlanningItem item in result.Items)
        {
            PlanningTreeItemViewModel node = new(item);
            allNodes.Add(node);

            if (!string.IsNullOrWhiteSpace(node.Id) && !nodesById.ContainsKey(node.Id))
            {
                nodesById.Add(node.Id, node);
            }

            foreach (Diagnostic diagnostic in item.Diagnostics)
            {
                Diagnostics.Add(diagnostic);
            }
        }

        foreach (PlanningTreeItemViewModel node in allNodes)
        {
            if (!string.IsNullOrWhiteSpace(node.ParentId) && nodesById.TryGetValue(node.ParentId, out PlanningTreeItemViewModel? parentNode))
            {
                parentNode.Children.Add(node);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(node.ParentSourcePath))
            {
                string parentSourcePath = node.ParentSourcePath;
                if (!externalParentNodesByPath.TryGetValue(parentSourcePath, out PlanningTreeItemViewModel? externalParentNode))
                {
                    externalParentNode = new PlanningTreeItemViewModel(parentSourcePath);
                    externalParentNodesByPath[parentSourcePath] = externalParentNode;
                    RootItems.Add(externalParentNode);
                }

                externalParentNode.Children.Add(node);
                continue;
            }

            RootItems.Add(node);
        }

        BuildCategoryGroups(allNodes);
        SelectedItem = RootItems.FirstOrDefault() ?? CategoryGroups.SelectMany(static group => group.Items).FirstOrDefault();
        StatusText = $"Loaded {result.Items.Count} planning items. Diagnostics: {Diagnostics.Count}.";
    }

    private void BuildPropertyItems(PlanningTreeItemViewModel? item)
    {
        PropertyItems.Clear();
        if (item is null)
        {
            return;
        }

        PropertyItems.Add(new PlanningPropertyItemViewModel("Id", "ID", item.Id, false));
        PropertyItems.Add(new PlanningPropertyItemViewModel("Title", "Title", item.Title, true, value =>
        {
            item.Title = value ?? string.Empty;
            OnPropertyChanged(nameof(SelectedItemTitle));
        }));
        PropertyItems.Add(new PlanningPropertyItemViewModel("Kind", "Kind", item.Kind.ToString(), false));
        PropertyItems.Add(new PlanningPropertyItemViewModel("Status", "Status", item.Status.ToString(), true, value =>
        {
            if (!Enum.TryParse(value, true, out PlanningItemStatus parsedStatus))
            {
                return;
            }

            item.Status = parsedStatus;
            OnPropertyChanged(nameof(SelectedItemStatus));
        }));
        PropertyItems.Add(new PlanningPropertyItemViewModel("Parent", "Parent", item.ParentId, false));
        PropertyItems.Add(new PlanningPropertyItemViewModel("SourcePath", "Source", item.SourcePath, false));
    }

    private void BuildCategoryGroups(IReadOnlyList<PlanningTreeItemViewModel> allNodes)
    {
        IEnumerable<IGrouping<PlanningItemKind, PlanningTreeItemViewModel>> groups = allNodes
            .Where(static node => !node.IsVirtualNode)
            .OrderBy(static node => node.Title, StringComparer.CurrentCultureIgnoreCase)
            .GroupBy(static node => node.Kind)
            .OrderBy(static group => group.Key.ToString(), StringComparer.CurrentCultureIgnoreCase);

        foreach (IGrouping<PlanningItemKind, PlanningTreeItemViewModel> group in groups)
        {
            ObservableCollection<PlanningTreeItemViewModel> groupedItems = new(group);
            CategoryGroups.Add(new PlanningCategoryGroupViewModel(group.Key.ToString(), groupedItems));
        }

        OnPropertyChanged(nameof(ExplorerStatusText));
    }

    private static int CountHierarchyNodes(IEnumerable<PlanningTreeItemViewModel> nodes)
    {
        int count = 0;

        foreach (PlanningTreeItemViewModel node in nodes)
        {
            count++;
            if (node.Children.Count > 0)
            {
                count += CountHierarchyNodes(node.Children);
            }
        }

        return count;
    }
}
