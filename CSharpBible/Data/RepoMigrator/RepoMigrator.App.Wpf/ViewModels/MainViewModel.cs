// RepoMigrator.App.Wpf/ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepoMigrator.App.Wpf.Services;
using RepoMigrator.App.Wpf.Settings;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RepoMigrator.App.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject, IMigrationProgress
{
    private readonly IMigrationService _migration;
    private readonly IProviderFactory _providerFactory;
    private readonly AppInputStateStore _inputStateStore;
    private bool _isLoadingInputState;
    private readonly HashSet<string> _hsSavedGitBranchSelections = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _hsSavedGitTagSelections = new(StringComparer.OrdinalIgnoreCase);
    private string? _sSavedSvnFromRevisionId;
    private string? _sSavedSvnToRevisionId;
    private string? _sSelectedSvnFromRevisionId;
    private string? _sSelectedSvnToRevisionId;
    private string? _targetUser;
    private string? _targetPassword;

    [ObservableProperty] public partial RepoType SourceType { get; set; } = RepoType.Git;
    [ObservableProperty] public partial string SourceUrl { get; set; } = "";
    [ObservableProperty] public partial string? SourceBranch { get; set; }
    [ObservableProperty] public partial string? SourceUser {  get; set; }
    [ObservableProperty] public partial string? SourcePassword { get; set; }

    [ObservableProperty] public partial RepoType TargetType { get; set; } = RepoType.Git;
    [ObservableProperty] public partial string TargetUrl { get; set; } = "";
    [ObservableProperty] public partial string? TargetBranch { get; set; }
    [ObservableProperty] public partial bool TransferGitBranches { get; set; }
    [ObservableProperty] public partial bool TransferGitTags { get; set; }
    public string? TargetUser
    {
        get => _targetUser;
        set
        {
            if (SetProperty(ref _targetUser, value))
                SaveInputState();
        }
    }

    public string? TargetPassword
    {
        get => _targetPassword;
        set
        {
            if (SetProperty(ref _targetPassword, value))
                SaveInputState();
        }
    }

    [ObservableProperty] public partial string? FromId { get; set; }
    [ObservableProperty] public partial string? ToId { get; set; }
    [ObservableProperty] public partial int? MaxCount { get; set; }
    [ObservableProperty] public partial bool OldestFirst { get; set; } = true;

    public string? SelectedSvnFromRevisionId
    {
        get => _sSelectedSvnFromRevisionId;
        set
        {
            if (SetProperty(ref _sSelectedSvnFromRevisionId, value))
                SaveInputState();
        }
    }

    public string? SelectedSvnToRevisionId
    {
        get => _sSelectedSvnToRevisionId;
        set
        {
            if (SetProperty(ref _sSelectedSvnToRevisionId, value))
                SaveInputState();
        }
    }

    [ObservableProperty] public partial string Log { get; set; } = "";
    [ObservableProperty] public partial double ProgressValue { get; set; }
    [ObservableProperty] public partial double ProgressMax { get; set; } = 100;

    [ObservableProperty] public partial bool IsRunning { get; set; }
    public bool IsIdle => !IsRunning;
    public bool CanConfigureGitHistory => SourceType == RepoType.Git && TargetType == RepoType.Git;
    public bool CanConfigureSvnRevisions => SourceType == RepoType.Svn;
    public bool ShowGenericRevisionInputs => !CanConfigureSvnRevisions;

    public ObservableCollection<RepoType> RepoTypes { get; } = new() { RepoType.Git, RepoType.Svn };
    public ObservableCollection<GitReferenceSelectionViewModel> GitBranchSelections { get; } = new();
    public ObservableCollection<GitReferenceSelectionViewModel> GitTagSelections { get; } = new();
    public ObservableCollection<RepositoryRevisionInfo> SvnRevisionSelections { get; } = new();
    public ObservableCollection<string> SvnFromRevisionOptions { get; } = new();
    public ObservableCollection<string> SvnToRevisionOptions { get; } = new();

    private CancellationTokenSource? _cts;
    private int _total;

    public MainViewModel(IMigrationService migration, IProviderFactory providerFactory, AppInputStateStore inputStateStore)
    {
        _migration = migration;
        _providerFactory = providerFactory;
        _inputStateStore = inputStateStore;

        _isLoadingInputState = true;
        var state = _inputStateStore.Load();
        SourceType = state.SourceType;
        SourceUrl = state.SourceUrl;
        SourceBranch = state.SourceBranch;
        SourceUser = state.SourceUser;
        SourcePassword = state.SourcePassword;
        TargetType = state.TargetType;
        TargetUrl = state.TargetUrl;
        TargetBranch = state.TargetBranch;
        TransferGitBranches = state.TransferGitBranches;
        TransferGitTags = state.TransferGitTags;
        TargetUser = state.TargetUser;
        TargetPassword = state.TargetPassword;
        _hsSavedGitBranchSelections.UnionWith(state.SelectedGitBranches);
        _hsSavedGitTagSelections.UnionWith(state.SelectedGitTags);
        _sSavedSvnFromRevisionId = state.SelectedSvnFromRevisionId;
        _sSavedSvnToRevisionId = state.SelectedSvnToRevisionId;
        SelectedSvnFromRevisionId = state.SelectedSvnFromRevisionId;
        SelectedSvnToRevisionId = state.SelectedSvnToRevisionId;
        FromId = state.FromId;
        ToId = state.ToId;
        MaxCount = state.MaxCount;
        OldestFirst = state.OldestFirst;
        _isLoadingInputState = false;
    }

    [RelayCommand]
    private async Task Start()
    {
        await RunOperationAsync(clearLog: true, async ct =>
        {
            var q = CreateChangeSetQuery();

            await _migration.MigrateAsync(CreateSourceEndpoint(), CreateTargetEndpoint(), q, CreateMigrationOptions(), this, ct);
        });
    }

    [RelayCommand]
    private async Task TestSource()
        => await ProbeEndpointAsync("Quelle", CreateSourceEndpoint(), RepositoryAccessMode.Read);

    [RelayCommand]
    private async Task TestTarget()
        => await ProbeEndpointAsync("Ziel", CreateTargetEndpoint(), RepositoryAccessMode.Write);

    [RelayCommand]
    private void Cancel() => _cts?.Cancel();

    public void Report(string message) => Append(message);

    public void ReportStep(string changeSetId, int index, int total)
    {
        _total = total;
        ProgressMax = total;
        ProgressValue = index - 1;
        Append($"Verarbeite {index}/{total}: {changeSetId}");
    }

    private void Append(string line)
    {
        Log += (Log.Length > 0 ? Environment.NewLine : "") + line;
        if (_total > 0 && line.StartsWith("Commit "))
        {
            var parts = line.Split(' ');
            if (int.TryParse(parts.Skip(1).FirstOrDefault()?.Split('/').FirstOrDefault(), out var idx))
                ProgressValue = idx;
        }
    }

    private RepositoryEndpoint CreateSourceEndpoint()
        => new()
        {
            Type = SourceType,
            UrlOrPath = SourceUrl,
            BranchOrTrunk = SourceBranch,
            Credentials = new RepoCredentials { Username = SourceUser, Password = SourcePassword }
        };

    private RepositoryEndpoint CreateTargetEndpoint()
        => new()
        {
            Type = TargetType,
            UrlOrPath = TargetUrl,
            BranchOrTrunk = TargetBranch,
            Credentials = new RepoCredentials { Username = TargetUser, Password = TargetPassword }
        };

    private async Task ProbeEndpointAsync(string label, RepositoryEndpoint endpoint, RepositoryAccessMode accessMode)
    {
        await RunOperationAsync(clearLog: false, async ct =>
        {
            await using var provider = _providerFactory.Create(endpoint.Type);
            Append($"Teste {label} ({provider.Name}) …");

            var result = await provider.ProbeAsync(endpoint, accessMode, ct);
            Append(result.Success
                ? $"{label} erfolgreich getestet: {result.Summary}"
                : $"{label}-Test fehlgeschlagen: {result.Summary}");

            foreach (var detail in result.Details)
                Append($"  {detail}");

            if (result.Success && label == "Quelle")
                await LoadSourceSelectionDataAsync(provider, endpoint, ct);
        });
    }

    private ChangeSetQuery CreateChangeSetQuery()
    {
        if (CanConfigureSvnRevisions)
        {
            return new ChangeSetQuery
            {
                FromExclusiveId = SvnRevisionRangeResolver.ResolveFromExclusiveId(SvnRevisionSelections, SelectedSvnFromRevisionId),
                ToInclusiveId = SvnRevisionRangeResolver.ResolveToInclusiveId(SelectedSvnToRevisionId),
                MaxCount = MaxCount,
                OldestFirst = OldestFirst
            };
        }

        return new ChangeSetQuery
        {
            FromExclusiveId = FromId,
            ToInclusiveId = ToId,
            MaxCount = MaxCount,
            OldestFirst = OldestFirst
        };
    }

    private MigrationOptions CreateMigrationOptions()
    {
        if (!CanConfigureGitHistory)
            return new MigrationOptions();

        return new MigrationOptions
        {
            TransferMode = RepositoryTransferMode.NativeHistory,
            TransferBranches = TransferGitBranches,
            TransferTags = TransferGitTags,
            SelectedBranches = GitBranchSelections.Where(vmItem => vmItem.IsSelected).Select(vmItem => vmItem.Name).ToList(),
            SelectedTags = GitTagSelections.Where(vmItem => vmItem.IsSelected).Select(vmItem => vmItem.Name).ToList()
        };
    }

    private async Task LoadGitSelectionDataAsync(IVersionControlProvider provider, RepositoryEndpoint endpoint, CancellationToken ct)
    {
        var capabilities = await provider.GetCapabilitiesAsync(endpoint, ct);
        if (!capabilities.SupportsBranchSelection && !capabilities.SupportsTagSelection)
        {
            ClearGitSelections();
            return;
        }

        var selectionData = await provider.GetSelectionDataAsync(endpoint, ct);
        RebuildGitSelectionCollection(GitBranchSelections, selectionData.Branches, _hsSavedGitBranchSelections);
        RebuildGitSelectionCollection(GitTagSelections, selectionData.Tags, _hsSavedGitTagSelections);

        if (string.IsNullOrWhiteSpace(SourceBranch) && !string.IsNullOrWhiteSpace(selectionData.DefaultBranch))
            SourceBranch = selectionData.DefaultBranch;

        Append($"  Git-Branches geladen: {GitBranchSelections.Count}");
        Append($"  Git-Tags geladen: {GitTagSelections.Count}");
    }

    private async Task LoadSourceSelectionDataAsync(IVersionControlProvider provider, RepositoryEndpoint endpoint, CancellationToken ct)
    {
        var capabilities = await provider.GetCapabilitiesAsync(endpoint, ct);

        if (capabilities.SupportsBranchSelection || capabilities.SupportsTagSelection)
            await LoadGitSelectionDataAsync(provider, endpoint, ct);
        else
            ClearGitSelections();

        if (capabilities.SupportsRevisionSelection)
            await LoadSvnSelectionDataAsync(provider, endpoint, ct);
        else
            ClearSvnSelections();
    }

    private async Task LoadSvnSelectionDataAsync(IVersionControlProvider provider, RepositoryEndpoint endpoint, CancellationToken ct)
    {
        var selectionData = await provider.GetSelectionDataAsync(endpoint, ct);

        SvnRevisionSelections.Clear();
        foreach (var svnRevision in selectionData.Revisions)
            SvnRevisionSelections.Add(svnRevision);

        SvnFromRevisionOptions.Clear();
        foreach (var svnRevision in selectionData.Revisions)
            SvnFromRevisionOptions.Add(svnRevision.Id);

        SvnToRevisionOptions.Clear();
        SvnToRevisionOptions.Add(string.Empty);
        foreach (var svnRevision in selectionData.Revisions)
            SvnToRevisionOptions.Add(svnRevision.Id);

        SelectedSvnFromRevisionId = ResolveExistingOrSuggestedRevisionId(SvnFromRevisionOptions, _sSavedSvnFromRevisionId, selectionData.SuggestedFromRevisionId);
        SelectedSvnToRevisionId = ResolveExistingOrSuggestedRevisionId(SvnToRevisionOptions, _sSavedSvnToRevisionId, selectionData.SuggestedToRevisionId) ?? string.Empty;

        Append($"  SVN-Revisionen geladen: {SvnRevisionSelections.Count}");
    }

    private void RebuildGitSelectionCollection(ObservableCollection<GitReferenceSelectionViewModel> lstTargetCollection, IReadOnlyList<RepositoryReferenceInfo> lstSourceItems, HashSet<string> hsSavedSelections)
    {
        foreach (var vmExistingItem in lstTargetCollection)
            vmExistingItem.PropertyChanged -= OnGitSelectionItemPropertyChanged;

        lstTargetCollection.Clear();
        foreach (var repositoryReference in lstSourceItems)
        {
            var vmItem = new GitReferenceSelectionViewModel(repositoryReference.Name, repositoryReference.CommitId, hsSavedSelections.Contains(repositoryReference.Name));
            vmItem.PropertyChanged += OnGitSelectionItemPropertyChanged;
            lstTargetCollection.Add(vmItem);
        }
    }

    private void OnGitSelectionItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GitReferenceSelectionViewModel.IsSelected))
            SaveInputState();
    }

    private void ClearGitSelections()
    {
        foreach (var vmBranch in GitBranchSelections)
            vmBranch.PropertyChanged -= OnGitSelectionItemPropertyChanged;
        foreach (var vmTag in GitTagSelections)
            vmTag.PropertyChanged -= OnGitSelectionItemPropertyChanged;

        GitBranchSelections.Clear();
        GitTagSelections.Clear();
    }

    private void ClearSvnSelections()
    {
        SvnRevisionSelections.Clear();
        SvnFromRevisionOptions.Clear();
        SvnToRevisionOptions.Clear();
        SelectedSvnFromRevisionId = null;
        SelectedSvnToRevisionId = string.Empty;
    }

    private static string? ResolveExistingOrSuggestedRevisionId(IEnumerable<string> lstOptions, string? sSavedRevisionId, string? sSuggestedRevisionId)
    {
        var lstAvailableOptions = lstOptions.ToList();
        if (!string.IsNullOrWhiteSpace(sSavedRevisionId) && lstAvailableOptions.Contains(sSavedRevisionId, StringComparer.OrdinalIgnoreCase))
            return sSavedRevisionId;

        if (!string.IsNullOrWhiteSpace(sSuggestedRevisionId) && lstAvailableOptions.Contains(sSuggestedRevisionId, StringComparer.OrdinalIgnoreCase))
            return sSuggestedRevisionId;

        return lstAvailableOptions.FirstOrDefault();
    }

    private List<string> GetSelectedGitBranchesForPersistence()
        => GitBranchSelections.Count > 0
            ? GitBranchSelections.Where(vmItem => vmItem.IsSelected).Select(vmItem => vmItem.Name).ToList()
            : _hsSavedGitBranchSelections.ToList();

    private List<string> GetSelectedGitTagsForPersistence()
        => GitTagSelections.Count > 0
            ? GitTagSelections.Where(vmItem => vmItem.IsSelected).Select(vmItem => vmItem.Name).ToList()
            : _hsSavedGitTagSelections.ToList();

    private async Task RunOperationAsync(bool clearLog, Func<CancellationToken, Task> action)
    {
        if (IsRunning)
            return;

        IsRunning = true;
        OnPropertyChanged(nameof(IsIdle));
        _cts = new CancellationTokenSource();
        try
        {
            if (clearLog)
            {
                Log = "";
                _total = 0;
                ProgressValue = 0;
                ProgressMax = 100;
            }

            await action(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            Append("Abgebrochen.");
        }
        catch (Exception ex)
        {
            Append("Fehler: " + ex.Message);
        }
        finally
        {
            IsRunning = false;
            OnPropertyChanged(nameof(IsIdle));
            _cts?.Dispose();
            _cts = null;
        }
    }

    partial void OnSourceTypeChanged(RepoType value)
    {
        OnPropertyChanged(nameof(CanConfigureGitHistory));
        OnPropertyChanged(nameof(CanConfigureSvnRevisions));
        OnPropertyChanged(nameof(ShowGenericRevisionInputs));
        _hsSavedGitBranchSelections.Clear();
        _hsSavedGitTagSelections.Clear();
        _sSavedSvnFromRevisionId = null;
        _sSavedSvnToRevisionId = null;
        ClearGitSelections();
        ClearSvnSelections();
        SaveInputState();
    }
    partial void OnSourceUrlChanged(string value)
    {
        _hsSavedGitBranchSelections.Clear();
        _hsSavedGitTagSelections.Clear();
        _sSavedSvnFromRevisionId = null;
        _sSavedSvnToRevisionId = null;
        ClearGitSelections();
        ClearSvnSelections();
        SaveInputState();
    }
    partial void OnSourceBranchChanged(string? value) => SaveInputState();
    partial void OnSourceUserChanged(string? value) => SaveInputState();
    partial void OnSourcePasswordChanged(string? value) => SaveInputState();
    partial void OnTargetTypeChanged(RepoType value)
    {
        OnPropertyChanged(nameof(CanConfigureGitHistory));
        OnPropertyChanged(nameof(CanConfigureSvnRevisions));
        OnPropertyChanged(nameof(ShowGenericRevisionInputs));
        SaveInputState();
    }
    partial void OnTargetUrlChanged(string value) => SaveInputState();
    partial void OnTargetBranchChanged(string? value) => SaveInputState();
    partial void OnTransferGitBranchesChanged(bool value) => SaveInputState();
    partial void OnTransferGitTagsChanged(bool value) => SaveInputState();
    partial void OnFromIdChanged(string? value) => SaveInputState();
    partial void OnToIdChanged(string? value) => SaveInputState();
    partial void OnMaxCountChanged(int? value) => SaveInputState();
    partial void OnOldestFirstChanged(bool value) => SaveInputState();

    private void SaveInputState()
    {
        if (_isLoadingInputState)
            return;

        _inputStateStore.Save(new AppInputState
        {
            SourceType = SourceType,
            SourceUrl = SourceUrl,
            SourceBranch = SourceBranch,
            SourceUser = SourceUser,
            SourcePassword = SourcePassword,
            TargetType = TargetType,
            TargetUrl = TargetUrl,
            TargetBranch = TargetBranch,
            TargetUser = TargetUser,
            TargetPassword = TargetPassword,
            TransferGitBranches = TransferGitBranches,
            TransferGitTags = TransferGitTags,
            SelectedGitBranches = GetSelectedGitBranchesForPersistence(),
            SelectedGitTags = GetSelectedGitTagsForPersistence(),
            SelectedSvnFromRevisionId = string.IsNullOrWhiteSpace(SelectedSvnFromRevisionId) ? null : SelectedSvnFromRevisionId,
            SelectedSvnToRevisionId = string.IsNullOrWhiteSpace(SelectedSvnToRevisionId) ? null : SelectedSvnToRevisionId,
            FromId = FromId,
            ToId = ToId,
            MaxCount = MaxCount,
            OldestFirst = OldestFirst
        });
    }
}
