// RepoMigrator.App.Wpf/ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepoMigrator.App.Wpf.Services;
using RepoMigrator.App.Wpf.Settings;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using System;
using System.Collections.ObjectModel;
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

    [ObservableProperty] public partial string Log { get; set; } = "";
    [ObservableProperty] public partial double ProgressValue { get; set; }
    [ObservableProperty] public partial double ProgressMax { get; set; } = 100;

    [ObservableProperty] public partial bool IsRunning { get; set; }
    public bool IsIdle => !IsRunning;

    public ObservableCollection<RepoType> RepoTypes { get; } = new() { RepoType.Git, RepoType.Svn };

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
        TargetUser = state.TargetUser;
        TargetPassword = state.TargetPassword;
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
            var q = new ChangeSetQuery
            {
                FromExclusiveId = FromId,
                ToInclusiveId = ToId,
                MaxCount = MaxCount,
                OldestFirst = OldestFirst
            };

            await _migration.MigrateAsync(CreateSourceEndpoint(), CreateTargetEndpoint(), q, this, ct);
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
        });
    }

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

    partial void OnSourceTypeChanged(RepoType value) => SaveInputState();
    partial void OnSourceUrlChanged(string value) => SaveInputState();
    partial void OnSourceBranchChanged(string? value) => SaveInputState();
    partial void OnSourceUserChanged(string? value) => SaveInputState();
    partial void OnSourcePasswordChanged(string? value) => SaveInputState();
    partial void OnTargetTypeChanged(RepoType value) => SaveInputState();
    partial void OnTargetUrlChanged(string value) => SaveInputState();
    partial void OnTargetBranchChanged(string? value) => SaveInputState();
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
            FromId = FromId,
            ToId = ToId,
            MaxCount = MaxCount,
            OldestFirst = OldestFirst
        });
    }
}
