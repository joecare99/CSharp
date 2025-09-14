// RepoMigrator.App.Wpf/ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepoMigrator.Core;
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

    [ObservableProperty] public partial RepoType SourceType { get; set; } = RepoType.Git;
    [ObservableProperty] public partial string SourceUrl { get; set; } = "";
    [ObservableProperty] public partial string? SourceBranch { get; set; }
    [ObservableProperty] public partial string? SourceUser {  get; set; }
    [ObservableProperty] public partial string? SourcePassword { get; set; }

    [ObservableProperty] public partial RepoType TargetType { get; set; } = RepoType.Git;
    [ObservableProperty] public partial string TargetUrl { get; set; } = "";

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

    public MainViewModel(IMigrationService migration) => _migration = migration;

    [RelayCommand]
    private async Task Start()
    {
        IsRunning = true;
        OnPropertyChanged(nameof(IsIdle));
        _cts = new CancellationTokenSource();
        try
        {
            Log = "";
            ProgressValue = 0;

            var src = new RepositoryEndpoint
            {
                Type = SourceType,
                UrlOrPath = SourceUrl,
                BranchOrTrunk = SourceBranch,
                Credentials = new RepoCredentials { Username = SourceUser, Password = SourcePassword }
            };

            var dst = new RepositoryEndpoint
            {
                Type = TargetType,
                UrlOrPath = TargetUrl
            };

            var q = new ChangeSetQuery
            {
                FromExclusiveId = FromId,
                ToInclusiveId = ToId,
                MaxCount = MaxCount,
                OldestFirst = OldestFirst
            };

            await _migration.MigrateAsync(src, dst, q, this, _cts.Token);
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
}
