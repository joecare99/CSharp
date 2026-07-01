using AppKomponentBaseLib.Diagnostics;
using AA98_AvlnCodeStudio.Base.Planning.Models;
using AA98_AvlnCodeStudio.Base.Planning.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>
/// Provides provider-neutral planning hierarchy browsing over local planning markdown items.
/// </summary>
public partial class PlanningExplorerViewModel : ViewModelBase
{
    private readonly IPlanningReader _planningReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanningExplorerViewModel"/> class.
    /// </summary>
    /// <param name="planningReader">The planning reader service.</param>
    public PlanningExplorerViewModel(IPlanningReader planningReader)
    {
        _planningReader = planningReader ?? throw new ArgumentNullException(nameof(planningReader));
    }

    /// <summary>
    /// Gets the root planning hierarchy items.
    /// </summary>
    public ObservableCollection<PlanningExplorerItemViewModel> RootItems { get; } = [];

    /// <summary>
    /// Gets the latest read diagnostics.
    /// </summary>
    public ObservableCollection<Diagnostic> Diagnostics { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedItemId))]
    [NotifyPropertyChangedFor(nameof(SelectedItemTitle))]
    [NotifyPropertyChangedFor(nameof(SelectedItemKind))]
    [NotifyPropertyChangedFor(nameof(SelectedItemStatus))]
    [NotifyPropertyChangedFor(nameof(SelectedItemSourcePath))]
    [NotifyPropertyChangedFor(nameof(SelectedItemParentId))]
    private PlanningExplorerItemViewModel? _selectedItem;

    /// <summary>
    /// Gets the selected planning item identifier.
    /// </summary>
    public string? SelectedItemId => SelectedItem?.Id;

    /// <summary>
    /// Gets the selected planning item title.
    /// </summary>
    public string? SelectedItemTitle => SelectedItem?.Title;

    /// <summary>
    /// Gets the selected planning item kind.
    /// </summary>
    public PlanningItemKind? SelectedItemKind => SelectedItem?.Kind;

    /// <summary>
    /// Gets the selected planning item status.
    /// </summary>
    public PlanningItemStatus? SelectedItemStatus => SelectedItem?.Status;

    /// <summary>
    /// Gets the selected planning item source path.
    /// </summary>
    public string? SelectedItemSourcePath => SelectedItem?.SourcePath;

    /// <summary>
    /// Gets the selected planning item parent identifier.
    /// </summary>
    public string? SelectedItemParentId => SelectedItem?.ParentId;

    /// <summary>
    /// Loads planning items from the requested repository root and rebuilds the hierarchy.
    /// </summary>
    /// <param name="repositoryRootPath">The repository root path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when loading is done.</returns>
    public async Task LoadAsync(string repositoryRootPath, CancellationToken cancellationToken = default)
    {
        PlanningReadResult result = await _planningReader.ReadAsync(new PlanningReadRequest
        {
            RepositoryRootPath = repositoryRootPath,
        }, cancellationToken).ConfigureAwait(false);

        RootItems.Clear();
        Diagnostics.Clear();
        SelectedItem = null;

        foreach (Diagnostic diagnostic in result.Diagnostics)
        {
            Diagnostics.Add(diagnostic);
        }

        Dictionary<string, PlanningExplorerItemViewModel> nodesById = new(StringComparer.Ordinal);
        List<PlanningExplorerItemViewModel> allNodes = [];

        foreach (PlanningItem item in result.Items)
        {
            PlanningExplorerItemViewModel node = new(item);
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

        foreach (PlanningExplorerItemViewModel node in allNodes)
        {
            if (!string.IsNullOrWhiteSpace(node.ParentId) && nodesById.TryGetValue(node.ParentId, out PlanningExplorerItemViewModel? parentNode))
            {
                parentNode.Children.Add(node);
                continue;
            }

            RootItems.Add(node);
        }

        SelectedItem = RootItems.FirstOrDefault();
    }
}
