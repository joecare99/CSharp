using AppKomponentBaseLib.Diagnostics;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
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
public partial class PlanningExplorerViewModel : ViewModelBase
{
    private readonly IPlanningProvider _planningProvider;

    public PlanningExplorerViewModel(IPlanningProvider planningProvider)
    {
        _planningProvider = planningProvider ?? throw new ArgumentNullException(nameof(planningProvider));
    }

    public ObservableCollection<PlanningTreeItemViewModel> RootItems { get; } = [];

    public ObservableCollection<Diagnostic> Diagnostics { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedItemId))]
    [NotifyPropertyChangedFor(nameof(SelectedItemTitle))]
    [NotifyPropertyChangedFor(nameof(SelectedItemKind))]
    [NotifyPropertyChangedFor(nameof(SelectedItemStatus))]
    [NotifyPropertyChangedFor(nameof(SelectedItemSourcePath))]
    [NotifyPropertyChangedFor(nameof(SelectedItemParentId))]
    private PlanningTreeItemViewModel? _selectedItem;

    [ObservableProperty]
    private string _repositoryRootPath = string.Empty;

    [ObservableProperty]
    private string _planningRootPath = string.Empty;

    [ObservableProperty]
    private string _statusText = string.Empty;

    public string? SelectedItemId => SelectedItem?.Id;

    public string? SelectedItemTitle => SelectedItem?.Title;

    public PlanningItemKind? SelectedItemKind => SelectedItem?.Kind;

    public PlanningItemStatus? SelectedItemStatus => SelectedItem?.Status;

    public string? SelectedItemSourcePath => SelectedItem?.SourcePath;

    public string? SelectedItemParentId => SelectedItem?.ParentId;

    public async Task LoadAsync(PlanningReadRequest request, CancellationToken cancellationToken = default)
    {
        PlanningReadResult result = await _planningProvider.ReadAsync(request, cancellationToken).ConfigureAwait(false);
        RepositoryRootPath = result.RepositoryRootPath;
        PlanningRootPath = result.PlanningRootPath;

        RootItems.Clear();
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

        SelectedItem = RootItems.FirstOrDefault();
        StatusText = $"Loaded {result.Items.Count} planning items. Diagnostics: {Diagnostics.Count}.";
    }
}
