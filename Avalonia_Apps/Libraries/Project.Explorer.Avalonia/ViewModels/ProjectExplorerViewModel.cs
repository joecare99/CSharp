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
public sealed partial class ProjectExplorerViewModel : ObservableObject, IDisposable
{
    private readonly IProjectExplorerSource _source;
    private readonly IProjectExplorerState _state;
    private readonly IProjectExplorerItemOpener _itemOpener;
    private string? _rootPath;
    private CancellationTokenSource? _navigationCancellation;
    private CancellationToken _navigationToken;
    private long _navigationVersion;
    private CancellationTokenSource? _refreshCancellation;
    private long _refreshVersion;
    private int _busyCount;
    private bool _restoringSelection;

    public event Action<ProjectExplorerItem?>? UserSelectionChanged;

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
        if (_restoringSelection)
        {
            return;
        }

        _state.Select(value?.Id);
        UserSelectionChanged?.Invoke(value?.Item);
        CancelPendingNavigation();
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(ErrorMessage));
        if (value is { Item.Kind: ProjectExplorerItemKind.File, Item.IsAvailable: true })
        {
            if (_itemOpener is IProjectExplorerPreviewOpener previewOpener)
            {
                CancellationToken cancellationToken = StartPendingNavigation();
                _ = PreviewFileAsync(previewOpener, value.Item, _navigationVersion, cancellationToken);
            }
            else
            {
                _ = OpenFileAsync(value.Item, explicitActivation: false, _navigationVersion, CancellationToken.None);
            }
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
        await RefreshCoreAsync(cancellationToken);
    }

    /// <summary>Refreshes the most recently loaded root.</summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (_rootPath is not null)
        {
            await RefreshCoreAsync(CancellationToken.None);
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
        CancelPendingNavigation();
        if (item.Item.Kind != ProjectExplorerItemKind.File || !item.Item.IsAvailable)
        {
            return Task.CompletedTask;
        }

        CancellationToken cancellationToken = _itemOpener is IProjectExplorerPreviewOpener
            ? StartPendingNavigation()
            : CancellationToken.None;
        return OpenFileAsync(item.Item, explicitActivation: true, _navigationVersion, cancellationToken);
    }

    private async Task RefreshCoreAsync(CancellationToken cancellationToken)
    {
        CancelPendingNavigation();
        long version = Interlocked.Increment(ref _refreshVersion);
        string rootPath = _rootPath!;
        CancellationTokenSource refreshCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        CancellationToken refreshToken = refreshCancellation.Token;
        CancelRefresh(Interlocked.Exchange(ref _refreshCancellation, refreshCancellation));
        SetBusy(true);
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(ErrorMessage));
        try
        {
            ProjectExplorerSnapshot snapshot = await _source.RefreshAsync(rootPath, refreshToken);
            refreshToken.ThrowIfCancellationRequested();
            if (version != Interlocked.Read(ref _refreshVersion))
            {
                return;
            }

            string? selectedId = _state.SelectedItemId;
            _restoringSelection = _itemOpener is IProjectExplorerPreviewOpener;
            try
            {
                RootItems.Clear();
                foreach (ProjectExplorerItem item in snapshot.RootItems)
                {
                    RootItems.Add(CreateItemViewModel(item));
                }

                SelectedItem = FindById(RootItems, selectedId);
                if (_restoringSelection)
                {
                    _state.Select(SelectedItem?.Id);
                }
            }
            finally
            {
                _restoringSelection = false;
            }
        }
        catch (OperationCanceledException) when (refreshToken.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            if (version == Interlocked.Read(ref _refreshVersion))
            {
                ErrorMessage = exception.Message;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        finally
        {
            Interlocked.CompareExchange(ref _refreshCancellation, null, refreshCancellation);
            refreshCancellation.Dispose();
            SetBusy(false);
        }
    }

    private async Task PreviewFileAsync(
        IProjectExplorerPreviewOpener opener,
        ProjectExplorerItem item,
        long version,
        CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(180), cancellationToken);
            SetBusy(true);
            try
            {
                await opener.PreviewAsync(item, cancellationToken);
            }
            finally
            {
                SetBusy(false);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            if (version == Interlocked.Read(ref _navigationVersion))
            {
                ErrorMessage = exception.Message;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
    }

    private async Task OpenFileAsync(
        ProjectExplorerItem item,
        bool explicitActivation,
        long version,
        CancellationToken cancellationToken)
    {
        SetBusy(true);
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(ErrorMessage));
        try
        {
            if (explicitActivation && _itemOpener is IProjectExplorerPreviewOpener previewOpener)
            {
                await previewOpener.ActivateAsync(item, cancellationToken);
            }
            else
            {
                await _itemOpener.OpenAsync(item, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            if (version == Interlocked.Read(ref _navigationVersion))
            {
                ErrorMessage = exception.Message;
                OnPropertyChanged(nameof(ErrorMessage));
            }
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
        int count = isBusy ? Interlocked.Increment(ref _busyCount) : Interlocked.Decrement(ref _busyCount);
        IsBusy = count > 0;
        OnPropertyChanged(nameof(IsBusy));
        RefreshCommand.NotifyCanExecuteChanged();
    }

    private CancellationToken StartPendingNavigation()
    {
        CancellationTokenSource cancellation = new();
        _navigationCancellation = cancellation;
        _navigationToken = cancellation.Token;
        return _navigationToken;
    }

    /// <summary>
    /// Cancels pending explorer navigation. Pass its token when an explorer-originated
    /// document open must continue without cancelling itself.
    /// </summary>
    public void CancelPendingNavigation(CancellationToken except = default)
    {
        if (except.CanBeCanceled && except == _navigationToken)
        {
            return;
        }

        Interlocked.Increment(ref _navigationVersion);
        CancellationTokenSource? cancellation = _navigationCancellation;
        _navigationCancellation = null;
        _navigationToken = default;
        cancellation?.Cancel();
        cancellation?.Dispose();
    }

    private static void CancelRefresh(CancellationTokenSource? cancellation)
    {
        try
        {
            cancellation?.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // The superseded request may already have completed and disposed its source.
        }
    }

    public void Dispose()
    {
        CancelPendingNavigation();
        Interlocked.Increment(ref _refreshVersion);
        CancelRefresh(Interlocked.Exchange(ref _refreshCancellation, null));
    }
}
