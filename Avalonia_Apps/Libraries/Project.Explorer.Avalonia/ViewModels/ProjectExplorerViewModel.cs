using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project.Explorer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Explorer.Avalonia.ViewModels;

/// <summary>Coordinates a project explorer source, neutral view state, and host-provided file opening.</summary>
public sealed partial class ProjectExplorerViewModel : ObservableObject
{
    private readonly IProjectExplorerSource _source;
    private readonly IProjectExplorerState _state;
    private readonly IProjectExplorerItemOpener _itemOpener;
    private string? _rootPath;

    /// <summary>Initializes an explorer with host-provided source, state, and file-opening capabilities.</summary>
    public ProjectExplorerViewModel(
        IProjectExplorerSource source,
        IProjectExplorerState state,
        IProjectExplorerItemOpener itemOpener)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _itemOpener = itemOpener ?? throw new ArgumentNullException(nameof(itemOpener));
    }

    /// <summary>Gets loaded root items.</summary>
    public ObservableCollection<ProjectExplorerItemViewModel> RootItems { get; } = [];

    /// <summary>Gets whether a refresh or file activation is in progress.</summary>
    public bool IsBusy { get; private set; }

    /// <summary>Gets an activation or source error that can be presented by the host UI.</summary>
    public string ErrorMessage { get; private set; } = string.Empty;

    /// <summary>Gets or sets the currently selected explorer item.</summary>
    [ObservableProperty]
    private ProjectExplorerItemViewModel? _selectedItem;

    partial void OnSelectedItemChanged(ProjectExplorerItemViewModel? value)
    {
        _state.Select(value?.Id);
        if (value is { Item.Kind: ProjectExplorerItemKind.File, Item.IsAvailable: true })
        {
            _ = OpenFileAsync(value.Item);
        }
    }

    /// <summary>Loads a hierarchy, restoring selection and expansion state by stable item identity.</summary>
    public async Task LoadAsync(string rootPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("An explorer root path is required.", nameof(rootPath));
        }

        _rootPath = rootPath;
        await RefreshCoreAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Refreshes the most recently loaded root.</summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (_rootPath is not null)
        {
            await RefreshCoreAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }

    /// <summary>Updates the persisted expansion state for an item.</summary>
    public void SetExpanded(ProjectExplorerItemViewModel item, bool isExpanded)
    {
        ArgumentNullException.ThrowIfNull(item);
        item.IsExpanded = isExpanded;
        _state.SetExpanded(item.Id, isExpanded);
    }

    /// <summary>Activates a file item through the host opening capability.</summary>
    public Task OpenAsync(ProjectExplorerItemViewModel item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return item.Item.Kind == ProjectExplorerItemKind.File && item.Item.IsAvailable
            ? OpenFileAsync(item.Item)
            : Task.CompletedTask;
    }

    private async Task RefreshCoreAsync(CancellationToken cancellationToken)
    {
        SetBusy(true);
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(ErrorMessage));
        try
        {
            ProjectExplorerSnapshot snapshot = await _source.RefreshAsync(_rootPath!, cancellationToken).ConfigureAwait(false);
            RootItems.Clear();
            foreach (ProjectExplorerItem item in snapshot.RootItems)
            {
                RootItems.Add(CreateItemViewModel(item));
            }

            SelectedItem = FindById(RootItems, _state.SelectedItemId);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            ErrorMessage = exception.Message;
            OnPropertyChanged(nameof(ErrorMessage));
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task OpenFileAsync(ProjectExplorerItem item)
    {
        SetBusy(true);
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(ErrorMessage));
        try
        {
            await _itemOpener.OpenAsync(item).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
            OnPropertyChanged(nameof(ErrorMessage));
        }
        finally
        {
            SetBusy(false);
        }
    }

    private ProjectExplorerItemViewModel CreateItemViewModel(ProjectExplorerItem item)
    {
        ProjectExplorerItemViewModel viewModel = new(item)
        {
            IsExpanded = _state.ExpandedItemIds.Contains(item.Id)
        };
        RestoreExpandedState(viewModel);
        SubscribeToExpansionState(viewModel);
        return viewModel;
    }

    private void RestoreExpandedState(ProjectExplorerItemViewModel item)
    {
        item.IsExpanded = _state.ExpandedItemIds.Contains(item.Id);
        foreach (ProjectExplorerItemViewModel child in item.Children)
        {
            RestoreExpandedState(child);
        }
    }

    private void SubscribeToExpansionState(ProjectExplorerItemViewModel item)
    {
        item.PropertyChanged += OnExplorerItemPropertyChanged;
        foreach (ProjectExplorerItemViewModel child in item.Children)
        {
            SubscribeToExpansionState(child);
        }
    }

    private void OnExplorerItemPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(ProjectExplorerItemViewModel.IsExpanded)
            && sender is ProjectExplorerItemViewModel item)
        {
            _state.SetExpanded(item.Id, item.IsExpanded);
        }
    }

    private static ProjectExplorerItemViewModel? FindById(
        IEnumerable<ProjectExplorerItemViewModel> items,
        string? itemId)
    {
        foreach (ProjectExplorerItemViewModel item in items)
        {
            if (string.Equals(item.Id, itemId, StringComparison.Ordinal))
            {
                return item;
            }

            ProjectExplorerItemViewModel? child = FindById(item.Children, itemId);
            if (child is not null)
            {
                return child;
            }
        }

        return null;
    }

    private void SetBusy(bool isBusy)
    {
        IsBusy = isBusy;
        OnPropertyChanged(nameof(IsBusy));
        RefreshCommand.NotifyCanExecuteChanged();
    }
}
