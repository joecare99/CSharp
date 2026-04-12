// RepoMigrator.App.Wpf/ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepoMigrator.App.Logic.Models;
using RepoMigrator.App.Logic.Services;
using RepoMigrator.App.State.Services;
using RepoMigrator.App.State.Settings;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace RepoMigrator.App.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject, IMigrationProgress
{
    private readonly IMigrationService _migration;
    private readonly MigrationEndpointFactory _migrationEndpointFactory;
    private readonly MigrationQueryService _migrationQueryService;
    private readonly RecentPathHistoryService _recentPathHistoryService;
    private readonly RepositorySelectionService _repositorySelectionService;
    private readonly IProviderFactory _providerFactory;
    private readonly AppInputStateStore _inputStateStore;
    private bool _isLoadingInputState;
    private readonly HashSet<string> _hsSavedGitBranchSelections = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _hsSavedGitTagSelections = new(StringComparer.OrdinalIgnoreCase);
    private List<RepoTypeRecentValues> _lstRecentSourceUrlsByType = [];
    private List<RepoTypeRecentValues> _lstRecentTargetUrlsByType = [];
    private string? _sSavedSvnFromRevisionId;
    private string? _sSavedSvnToRevisionId;
    private string? _sSelectedSvnFromRevisionId;
    private string? _sSelectedSvnToRevisionId;
    private string? _sCurrentChangeSetId;
    private string? _targetUser;
    private string? _targetPassword;
    private bool _xUsePipelinedMigration;
    private int _iPipelinePrefetchCount = 3;
    private int _iPipelineExportWorkerCount = 2;
    private bool _xSplitIntoSubdirectoryBranches;
    private int _iBranchSplitDepth = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfigureGitHistory))]
    [NotifyPropertyChangedFor(nameof(CanConfigureSvnRevisions))]
    [NotifyPropertyChangedFor(nameof(CanConfigureAdvancedGitTargetOptions))]
    [NotifyPropertyChangedFor(nameof(ShowGenericRevisionInputs))]
    [NotifyPropertyChangedFor(nameof(ShowPipelineTuningOptions))]
    [NotifyPropertyChangedFor(nameof(ShowBranchSplitDepthOptions))]
    [NotifyPropertyChangedFor(nameof(CanStartMigration))]
    [NotifyPropertyChangedFor(nameof(OptionsHintMessage))]
    [NotifyPropertyChangedFor(nameof(SourceSummaryName))]
    public partial RepoType SourceType { get; set; } = RepoType.Git;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanContinueFromSetup))]
    [NotifyPropertyChangedFor(nameof(SetupHintMessage))]
    [NotifyPropertyChangedFor(nameof(SourceSummaryName))]
    public partial string SourceUrl { get; set; } = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SourceSummaryName))]
    public partial string? SourceBranch { get; set; }
    [ObservableProperty] public partial string? SourceUser {  get; set; }
    [ObservableProperty] public partial string? SourcePassword { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfigureGitHistory))]
    [NotifyPropertyChangedFor(nameof(CanConfigureSvnRevisions))]
    [NotifyPropertyChangedFor(nameof(CanConfigureAdvancedGitTargetOptions))]
    [NotifyPropertyChangedFor(nameof(ShowGenericRevisionInputs))]
    [NotifyPropertyChangedFor(nameof(ShowPipelineTuningOptions))]
    [NotifyPropertyChangedFor(nameof(ShowBranchSplitDepthOptions))]
    [NotifyPropertyChangedFor(nameof(CanStartMigration))]
    [NotifyPropertyChangedFor(nameof(OptionsHintMessage))]
    [NotifyPropertyChangedFor(nameof(TargetSummaryName))]
    public partial RepoType TargetType { get; set; } = RepoType.Git;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanContinueFromSetup))]
    [NotifyPropertyChangedFor(nameof(CanStartMigration))]
    [NotifyPropertyChangedFor(nameof(SetupHintMessage))]
    [NotifyPropertyChangedFor(nameof(TargetSummaryName))]
    public partial string TargetUrl { get; set; } = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanStartMigration))]
    [NotifyPropertyChangedFor(nameof(OptionsHintMessage))]
    [NotifyPropertyChangedFor(nameof(TargetSummaryName))]
    public partial string? TargetBranch { get; set; }
    [ObservableProperty] public partial bool TransferGitBranches { get; set; }
    [ObservableProperty] public partial bool TransferGitTags { get; set; }
    public string? TargetUser
    {
        get => _targetUser;
        set
        {
            if (SetProperty(ref _targetUser, value))
            {
                NotifySetupStateChanged();
                SetWorkflowStage(WorkflowStage.Setup);
                SaveInputState();
            }
        }
    }

    public string? TargetPassword
    {
        get => _targetPassword;
        set
        {
            if (SetProperty(ref _targetPassword, value))
            {
                NotifySetupStateChanged();
                SetWorkflowStage(WorkflowStage.Setup);
                SaveInputState();
            }
        }
    }

    [ObservableProperty] public partial string? FromId { get; set; }
    [ObservableProperty] public partial string? ToId { get; set; }
    [ObservableProperty] public partial int? MaxCount { get; set; }
    [ObservableProperty] public partial bool OldestFirst { get; set; } = true;
    public bool UsePipelinedMigration
    {
        get => _xUsePipelinedMigration;
        set
        {
            if (SetProperty(ref _xUsePipelinedMigration, value))
            {
                OnPropertyChanged(nameof(ShowPipelineTuningOptions));
                NotifyOptionStateChanged();
                SaveInputState();
            }
        }
    }

    public int PipelinePrefetchCount
    {
        get => _iPipelinePrefetchCount;
        set
        {
            if (SetProperty(ref _iPipelinePrefetchCount, value))
            {
                NotifyOptionStateChanged();
                SaveInputState();
            }
        }
    }

    public int PipelineExportWorkerCount
    {
        get => _iPipelineExportWorkerCount;
        set
        {
            if (SetProperty(ref _iPipelineExportWorkerCount, value))
            {
                NotifyOptionStateChanged();
                SaveInputState();
            }
        }
    }

    public bool SplitIntoSubdirectoryBranches
    {
        get => _xSplitIntoSubdirectoryBranches;
        set
        {
            if (SetProperty(ref _xSplitIntoSubdirectoryBranches, value))
            {
                OnPropertyChanged(nameof(ShowBranchSplitDepthOptions));
                NotifyOptionStateChanged();
                SaveInputState();
            }
        }
    }

    public int BranchSplitDepth
    {
        get => _iBranchSplitDepth;
        set
        {
            if (SetProperty(ref _iBranchSplitDepth, value))
            {
                NotifyOptionStateChanged();
                SaveInputState();
            }
        }
    }

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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    [NotifyPropertyChangedFor(nameof(CanStartMigration))]
    public partial bool IsRunning { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSetupStage))]
    [NotifyPropertyChangedFor(nameof(IsOptionsStage))]
    [NotifyPropertyChangedFor(nameof(IsExecutionStage))]
    [NotifyPropertyChangedFor(nameof(ShowCompactEndpointSummaries))]
    public partial WorkflowStage WorkflowStage { get; private set; } = WorkflowStage.Setup;

    public bool IsIdle => !IsRunning;
    public bool CanConfigureGitHistory => SourceType == RepoType.Git && TargetType == RepoType.Git;
    public bool CanConfigureSvnRevisions => SourceType == RepoType.Svn;
    public bool CanConfigureAdvancedGitTargetOptions => SourceType == RepoType.Svn && TargetType == RepoType.Git;
    public bool ShowGenericRevisionInputs => !CanConfigureSvnRevisions;
    public bool ShowPipelineTuningOptions => CanConfigureAdvancedGitTargetOptions && UsePipelinedMigration;
    public bool ShowBranchSplitDepthOptions => CanConfigureAdvancedGitTargetOptions && SplitIntoSubdirectoryBranches;
    public bool IsSetupStage => WorkflowStage == WorkflowStage.Setup;
    public bool IsOptionsStage => WorkflowStage == WorkflowStage.Options;
    public bool IsExecutionStage => WorkflowStage == WorkflowStage.Execution;
    public bool ShowCompactEndpointSummaries => !IsSetupStage;
    public bool CanContinueFromSetup => GetSetupValidationMessages().Count == 0;
    public bool CanStartMigration => IsIdle && CanContinueFromSetup && GetOptionValidationMessages().Count == 0;
    public string SetupHintMessage => BuildSetupHintMessage();
    public string OptionsHintMessage => BuildOptionsHintMessage();
    public string SourceSummaryName => BuildEndpointSummaryName(SourceUrl, SourceBranch);
    public string TargetSummaryName => BuildEndpointSummaryName(TargetUrl, TargetBranch);
    public string SourceSummaryCommit => BuildCommitSummary();
    public string TargetSummaryCommit => BuildCommitSummary();

    public ObservableCollection<RepoType> RepoTypes { get; } = new() { RepoType.Git, RepoType.Svn };
    public ObservableCollection<GitReferenceSelectionViewModel> GitBranchSelections { get; } = new();
    public ObservableCollection<GitReferenceSelectionViewModel> GitTagSelections { get; } = new();
    public ObservableCollection<RepositoryRevisionInfo> SvnRevisionSelections { get; } = new();
    public ObservableCollection<string> SvnFromRevisionOptions { get; } = new();
    public ObservableCollection<string> SvnToRevisionOptions { get; } = new();
    public ObservableCollection<string> RecentSourceUrls { get; } = new();
    public ObservableCollection<string> RecentSourceBranches { get; } = new();
    public ObservableCollection<string> RecentSourceUsers { get; } = new();
    public ObservableCollection<string> RecentTargetUrls { get; } = new();
    public ObservableCollection<string> RecentTargetBranches { get; } = new();
    public ObservableCollection<string> RecentTargetUsers { get; } = new();
    public ObservableCollection<string> TargetGitBranchOptions { get; } = new();
    public ObservableCollection<int> BranchSplitDepthOptions { get; } = new() { 1, 2 };

    private CancellationTokenSource? _cts;
    private int _total;

    public MainViewModel(
        IMigrationService migration,
        MigrationEndpointFactory migrationEndpointFactory,
        MigrationQueryService migrationQueryService,
        RecentPathHistoryService recentPathHistoryService,
        RepositorySelectionService repositorySelectionService,
        IProviderFactory providerFactory,
        AppInputStateStore inputStateStore)
    {
        _migration = migration;
        _migrationEndpointFactory = migrationEndpointFactory;
        _migrationQueryService = migrationQueryService;
        _recentPathHistoryService = recentPathHistoryService;
        _repositorySelectionService = repositorySelectionService;
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
        _lstRecentSourceUrlsByType = [.. state.RecentSourceUrlsByType];
        _lstRecentTargetUrlsByType = [.. state.RecentTargetUrlsByType];
        SyncRecentValues(RecentSourceBranches, state.RecentSourceBranches);
        SyncRecentValues(RecentSourceUsers, state.RecentSourceUsers);
        SyncRecentValues(RecentTargetBranches, state.RecentTargetBranches);
        SyncRecentValues(RecentTargetUsers, state.RecentTargetUsers);
        _sSavedSvnFromRevisionId = state.SelectedSvnFromRevisionId;
        _sSavedSvnToRevisionId = state.SelectedSvnToRevisionId;
        SelectedSvnFromRevisionId = state.SelectedSvnFromRevisionId;
        SelectedSvnToRevisionId = state.SelectedSvnToRevisionId;
        FromId = state.FromId;
        ToId = state.ToId;
        MaxCount = state.MaxCount;
        OldestFirst = state.OldestFirst;
        UsePipelinedMigration = state.UsePipelinedMigration;
        PipelinePrefetchCount = state.PipelinePrefetchCount;
        PipelineExportWorkerCount = state.PipelineExportWorkerCount;
        SplitIntoSubdirectoryBranches = state.SplitIntoSubdirectoryBranches;
        BranchSplitDepth = state.BranchSplitDepth;
        EnsureAdvancedMigrationOptionDefaults();
        RefreshProviderSpecificPathHistories();
        _isLoadingInputState = false;
    }

    [RelayCommand]
    private async Task Start()
    {
        if (!CanStartMigration)
        {
            Append(BuildOptionsHintMessage());
            return;
        }

        await RunOperationAsync(clearLog: true, async ct =>
        {
            var sourceEndpoint = CreateSourceEndpoint();
            var targetEndpoint = CreateTargetEndpoint();
            var q = CreateChangeSetQuery();
            var options = CreateMigrationOptions();

            CaptureRecentSourceInputs();
            CaptureRecentTargetInputs();

            await _migration.MigrateAsync(sourceEndpoint, targetEndpoint, q, options, this, ct);
        }, workflowStageOnStart: WorkflowStage.Execution, workflowStageOnFinish: WorkflowStage.Options);
    }

    [RelayCommand]
    private async Task TestSource()
        => await ProbeEndpointAsync("Quelle", CreateSourceEndpoint(), RepositoryAccessMode.Read);

    [RelayCommand]
    private async Task TestTarget()
        => await ProbeEndpointAsync("Ziel", CreateTargetEndpoint(), RepositoryAccessMode.Write);

    [RelayCommand]
    private async Task ContinueFromSetup()
    {
        if (!CanContinueFromSetup)
            return;

        await RunOperationAsync(clearLog: false, async ct =>
        {
            await EnsureOptionSelectionDataAsync(ct);
            SetWorkflowStage(WorkflowStage.Options);
        });
    }

    [RelayCommand]
    private void Cancel() => _cts?.Cancel();

    [RelayCommand]
    private void EditSetup() => SetWorkflowStage(WorkflowStage.Setup);

    public void Report(MigrationReportSeverity severity, MigrationReportMessage message, params object?[] arrAdditional)
    {
        ApplyProgressState(message, arrAdditional);
        Append(FormatProgressMessage(message, arrAdditional));
    }


    private void ApplyProgressState(MigrationReportMessage message, object?[] arrAdditional)
    {
        switch (message)
        {
            case MigrationReportMessage.ChangeSetProcessingStarting:
                CurrentChangeSetId = GetRequired<string>(arrAdditional, 0);
                _total = GetRequired<int>(arrAdditional, 2);
                ProgressMax = _total;
                ProgressValue = Math.Max(GetRequired<int>(arrAdditional, 1) - 1, 0);
                break;

            case MigrationReportMessage.CommitCompleted:
                CurrentChangeSetId = GetRequired<string>(arrAdditional, 0);
                ProgressValue = GetRequired<int>(arrAdditional, 1);
                UpdateResumeFromAfterSuccessfulCommit();
                break;
        }
    }

    public string? CurrentChangeSetId
    {
        get => _sCurrentChangeSetId;
        private set
        {
            if (!SetProperty(ref _sCurrentChangeSetId, value))
                return;

            OnPropertyChanged(nameof(SourceSummaryCommit));
            OnPropertyChanged(nameof(TargetSummaryCommit));
        }
    }

    private void Append(string line)
    {
        var sTimestampedLine = $"[{DateTimeOffset.Now:HH:mm:ss.fff}] {line}";
        Log += (Log.Length > 0 ? Environment.NewLine : "") + sTimestampedLine;
    }

    private static string FormatProgressMessage(MigrationReportMessage message, object?[] arrAdditional)
        => message switch
        {
            MigrationReportMessage.SourceOpening => $"Öffne Quelle ({GetRequired<string>(arrAdditional, 0)}) …",
            MigrationReportMessage.NativeHistoryTransferStarting => $"Übertrage Historie nativ ({GetRequired<string>(arrAdditional, 0)} -> {GetRequired<RepoType>(arrAdditional, 1)}) …",
            MigrationReportMessage.ChangeSetsLoading => "Lese Changesets …",
            MigrationReportMessage.NoChangeSetsFound => "Keine Changesets gefunden.",
            MigrationReportMessage.TargetInitializing => $"Initialisiere Ziel ({GetRequired<string>(arrAdditional, 0)}) …",
            MigrationReportMessage.PipelineEnabled => $"Pipeline aktiviert: {GetRequired<int>(arrAdditional, 0)} Worker, Prefetch {GetRequired<int>(arrAdditional, 1)}, {GetRequired<int>(arrAdditional, 2)} Changesets.",
            MigrationReportMessage.PipelineCleanupStarting => "Pipeline: Abbruch oder Fehler erkannt, bereinige ausstehende Snapshots …",
            MigrationReportMessage.ChangeSetProcessingStarting => $"Verarbeite {GetRequired<int>(arrAdditional, 1)}/{GetRequired<int>(arrAdditional, 2)}: {GetRequired<string>(arrAdditional, 0)}",
            MigrationReportMessage.CommitCompleted => $"Commit {GetRequired<int>(arrAdditional, 1)}/{GetRequired<int>(arrAdditional, 2)} übertragen: {Short(GetRequired<string>(arrAdditional, 0))}",
            MigrationReportMessage.FlushStarting => $"Starte Ziel-Synchronisierung ({GetRequired<string>(arrAdditional, 0)}) …",
            MigrationReportMessage.FlushCompleted => $"Ziel-Synchronisierung ({GetRequired<string>(arrAdditional, 0)}) abgeschlossen.",
            MigrationReportMessage.ExportWorkerSourceOpened => $"Export-Worker {GetRequired<int>(arrAdditional, 0)}: Quelle geöffnet.",
            MigrationReportMessage.ExportWorkerSnapshotExporting => $"Export-Worker {GetRequired<int>(arrAdditional, 0)}: exportiere {GetRequired<string>(arrAdditional, 1)} …",
            MigrationReportMessage.ExportWorkerSnapshotExported => $"Export-Worker {GetRequired<int>(arrAdditional, 0)}: Export {GetRequired<string>(arrAdditional, 1)} abgeschlossen.",
            MigrationReportMessage.ExportWorkerSnapshotHandedOff => $"Export-Worker {GetRequired<int>(arrAdditional, 0)}: Snapshot {GetRequired<string>(arrAdditional, 1)} an Commit-Stufe übergeben.",
            MigrationReportMessage.ExportWorkerCompleted => $"Export-Worker {GetRequired<int>(arrAdditional, 0)}: keine weiteren Export-Aufträge.",
            MigrationReportMessage.PipelineExportSlotRequested => $"Pipeline: fordere Export-Slot {GetRequired<int>(arrAdditional, 1)}/{GetRequired<int>(arrAdditional, 2)}: {GetRequired<string>(arrAdditional, 0)}",
            MigrationReportMessage.PipelineExportQueued => $"Pipeline: queue {GetRequired<int>(arrAdditional, 1)}/{GetRequired<int>(arrAdditional, 2)}: {GetRequired<string>(arrAdditional, 0)}",
            MigrationReportMessage.PipelineExportQueueCompleted => "Pipeline: alle Export-Aufträge wurden eingeplant.",
            MigrationReportMessage.PipelineSnapshotWaiting => $"Pipeline: warte auf Snapshot {GetRequired<int>(arrAdditional, 0)}/{GetRequired<int>(arrAdditional, 1)} …",
            MigrationReportMessage.PipelineSnapshotReady => $"Pipeline: Snapshot {GetRequired<string>(arrAdditional, 0)} ist in Commit-Reihenfolge bereit.",
            MigrationReportMessage.PipelineSnapshotBuffered => $"Pipeline: Snapshot {GetRequired<string>(arrAdditional, 0)} gepuffert, warte weiter auf Position {GetRequired<int>(arrAdditional, 1)}.",
            MigrationReportMessage.CommitToBranchStarting => $"Commit-Stufe: Revision {GetRequired<string>(arrAdditional, 0)} wird auf Branch '{GetRequired<string>(arrAdditional, 1)}' übertragen.",
            MigrationReportMessage.ProjectionPlanned => $"Commit-Stufe: Revision {GetRequired<string>(arrAdditional, 0)} wird in {GetRequired<int>(arrAdditional, 1)} Sub-Branches projiziert (Tiefe {GetRequired<int>(arrAdditional, 2)}).",
            MigrationReportMessage.ProjectedBranchPrepared => $"Commit-Stufe: Branch '{GetRequired<string>(arrAdditional, 1)}' erhält {GetRequired<int>(arrAdditional, 2)} Pfade aus Revision {GetRequired<string>(arrAdditional, 0)}.",
            MigrationReportMessage.ProjectedBranchEmpty => $"Commit-Stufe: Branch '{GetRequired<string>(arrAdditional, 1)}' wird für Revision {GetRequired<string>(arrAdditional, 0)} als leerer Snapshot fortgeführt.",
            MigrationReportMessage.ProjectedBranchCommitted => $"Commit-Stufe: Branch '{GetRequired<string>(arrAdditional, 1)}' für Revision {GetRequired<string>(arrAdditional, 0)} übertragen.",
            MigrationReportMessage.GitBranchTransferStarting => $"Übertrage Branch {GetRequired<string>(arrAdditional, 0)} -> {GetRequired<string>(arrAdditional, 1)} …",
            MigrationReportMessage.GitTagTransferStarting => $"Übertrage Tag {GetRequired<string>(arrAdditional, 0)} -> {GetRequired<string>(arrAdditional, 1)} …",
            MigrationReportMessage.MigrationCompleted => "Migration abgeschlossen.",
            _ => throw new InvalidEnumArgumentException(nameof(message), (int)message, typeof(MigrationReportMessage))
        };

    private static T GetRequired<T>(IReadOnlyList<object?> lstValues, int iIndex)
        => lstValues.Count > iIndex && lstValues[iIndex] is T value
            ? value
            : throw new InvalidOperationException($"Expected argument at index {iIndex} with type {typeof(T).Name}.");

    private static string Short(string? sId)
        => string.IsNullOrEmpty(sId) || sId.Length <= 8 ? sId ?? string.Empty : sId[..8];

    private RepositoryEndpoint CreateSourceEndpoint()
        => _migrationEndpointFactory.CreateSourceEndpoint(SourceType, SourceUrl, SourceBranch, SourceUser, SourcePassword);

    private RepositoryEndpoint CreateTargetEndpoint()
        => _migrationEndpointFactory.CreateTargetEndpoint(TargetType, TargetUrl, TargetBranch, TargetUser, TargetPassword);

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
                ApplySourceSelectionResult(await _repositorySelectionService.LoadSourceSelectionAsync(endpoint, ct));

            if (result.Success && label == "Ziel")
                ApplyTargetSelectionResult(await _repositorySelectionService.LoadTargetSelectionAsync(endpoint, ct));
        });
    }

    private ChangeSetQuery CreateChangeSetQuery()
        => _migrationQueryService.CreateQuery(SourceType, FromId, ToId, MaxCount, OldestFirst, SvnRevisionSelections, SelectedSvnFromRevisionId, SelectedSvnToRevisionId);

    private MigrationOptions CreateMigrationOptions()
    {
        if (!CanConfigureGitHistory)
        {
            return new MigrationOptions
            {
                ExecutionMode = UsePipelinedMigration && CanConfigureAdvancedGitTargetOptions ? MigrationExecutionMode.Pipelined : MigrationExecutionMode.Sequential,
                ProjectionMode = SplitIntoSubdirectoryBranches && CanConfigureAdvancedGitTargetOptions ? MigrationProjectionMode.SubdirectoryBranches : MigrationProjectionMode.None,
                BranchSplitDepth = BranchSplitDepth,
                PipelinePrefetchCount = Math.Max(PipelinePrefetchCount, 1),
                PipelineExportWorkerCount = Math.Max(PipelineExportWorkerCount, 1)
            };
        }

        return new MigrationOptions
        {
            TransferMode = RepositoryTransferMode.NativeHistory,
            TransferBranches = TransferGitBranches,
            TransferTags = TransferGitTags,
            SelectedBranches = GitBranchSelections.Where(vmItem => vmItem.IsSelected).Select(vmItem => vmItem.Name).ToList(),
            SelectedTags = GitTagSelections.Where(vmItem => vmItem.IsSelected).Select(vmItem => vmItem.Name).ToList()
        };
    }

    private void ApplySourceSelectionResult(SourceSelectionResult sourceSelection)
    {
        if (sourceSelection.Branches.Count > 0 || sourceSelection.Tags.Count > 0)
        {
            RebuildGitSelectionCollection(GitBranchSelections, sourceSelection.Branches, _hsSavedGitBranchSelections);
            RebuildGitSelectionCollection(GitTagSelections, sourceSelection.Tags, _hsSavedGitTagSelections);
            if (string.IsNullOrWhiteSpace(SourceBranch) && !string.IsNullOrWhiteSpace(sourceSelection.DefaultBranch))
                SourceBranch = sourceSelection.DefaultBranch;

            Append($"  Git-Branches geladen: {GitBranchSelections.Count}");
            Append($"  Git-Tags geladen: {GitTagSelections.Count}");
        }
        else
            ClearGitSelections();

        if (sourceSelection.Revisions.Count > 0)
            ApplySvnSelectionResult(sourceSelection);
        else
            ClearSvnSelections();

        NotifySetupStateChanged();
    }

    private async Task EnsureOptionSelectionDataAsync(CancellationToken ct)
    {
        if (SourceType != RepoType.Svn || SvnRevisionSelections.Count > 0 || string.IsNullOrWhiteSpace(SourceUrl))
            return;

        ApplySourceSelectionResult(await _repositorySelectionService.LoadSourceSelectionAsync(CreateSourceEndpoint(), ct));
    }

    private void ApplyTargetSelectionResult(TargetSelectionResult targetSelection)
    {
        if (targetSelection.Branches.Count == 0)
        {
            TargetGitBranchOptions.Clear();
            NotifySetupStateChanged();
            return;
        }

        var lstTargetBranchOptions = targetSelection.Branches
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!string.IsNullOrWhiteSpace(TargetBranch) && !lstTargetBranchOptions.Contains(TargetBranch, StringComparer.OrdinalIgnoreCase))
            lstTargetBranchOptions.Insert(0, TargetBranch);

        SyncRecentValues(TargetGitBranchOptions, lstTargetBranchOptions);
        if (string.IsNullOrWhiteSpace(TargetBranch) && !string.IsNullOrWhiteSpace(targetSelection.DefaultBranch))
            TargetBranch = targetSelection.DefaultBranch;

        Append($"  Ziel-Branches geladen: {TargetGitBranchOptions.Count}");
        NotifySetupStateChanged();
    }

    private void ApplySvnSelectionResult(SourceSelectionResult sourceSelection)
    {
        SvnRevisionSelections.Clear();
        foreach (var svnRevision in sourceSelection.Revisions)
            SvnRevisionSelections.Add(svnRevision);

        SvnFromRevisionOptions.Clear();
        foreach (var svnRevision in sourceSelection.Revisions)
            SvnFromRevisionOptions.Add(svnRevision.Id);

        SvnToRevisionOptions.Clear();
        SvnToRevisionOptions.Add(string.Empty);
        foreach (var svnRevision in sourceSelection.Revisions)
            SvnToRevisionOptions.Add(svnRevision.Id);

        SelectedSvnFromRevisionId = ResolveExistingOrSuggestedRevisionId(SvnFromRevisionOptions, _sSavedSvnFromRevisionId, sourceSelection.SuggestedFromRevisionId);
        SelectedSvnToRevisionId = ResolveExistingOrSuggestedRevisionId(SvnToRevisionOptions, _sSavedSvnToRevisionId, sourceSelection.SuggestedToRevisionId) ?? string.Empty;

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
        NotifySetupStateChanged();
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

    private static void SyncRecentValues(ObservableCollection<string> lstTargetCollection, IReadOnlyList<string> lstValues)
    {
        for (var iIndex = 0; iIndex < lstValues.Count; iIndex++)
        {
            if (iIndex < lstTargetCollection.Count)
            {
                if (!string.Equals(lstTargetCollection[iIndex], lstValues[iIndex], StringComparison.Ordinal))
                    lstTargetCollection[iIndex] = lstValues[iIndex];

                continue;
            }

            lstTargetCollection.Add(lstValues[iIndex]);
        }

        while (lstTargetCollection.Count > lstValues.Count)
            lstTargetCollection.RemoveAt(lstTargetCollection.Count - 1);
    }

    private static void ApplyRecentValue(ObservableCollection<string> lstTargetCollection, string? sValue)
    {
        var lstUpdatedValues = RecentValueHistory.AddValue(lstTargetCollection.ToList(), sValue);
        SyncRecentValues(lstTargetCollection, lstUpdatedValues);
    }

    private void CaptureRecentSourceInputs()
    {
        _lstRecentSourceUrlsByType = [.. _recentPathHistoryService.AddPath(_lstRecentSourceUrlsByType, SourceType, SourceUrl)];
        RefreshSourceUrlHistory();
        ApplyRecentValue(RecentSourceBranches, SourceBranch);
        ApplyRecentValue(RecentSourceUsers, SourceUser);
    }

    private void CaptureRecentTargetInputs()
    {
        _lstRecentTargetUrlsByType = [.. _recentPathHistoryService.AddPath(_lstRecentTargetUrlsByType, TargetType, TargetUrl)];
        RefreshTargetUrlHistory();
        ApplyRecentValue(RecentTargetBranches, TargetBranch);
        ApplyRecentValue(RecentTargetUsers, TargetUser);
    }

    private void UpdateResumeFromAfterSuccessfulCommit()
    {
        var resumeUpdate = _migrationQueryService.UpdateResumeAfterCommit(SourceType, CurrentChangeSetId, SvnRevisionSelections);
        if (!string.IsNullOrWhiteSpace(resumeUpdate.FromExclusiveId))
            FromId = resumeUpdate.FromExclusiveId;
        if (!string.IsNullOrWhiteSpace(resumeUpdate.SelectedSvnFromRevisionId))
            SelectedSvnFromRevisionId = resumeUpdate.SelectedSvnFromRevisionId;
    }

    private void RefreshProviderSpecificPathHistories()
    {
        RefreshSourceUrlHistory();
        RefreshTargetUrlHistory();
    }

    private void RefreshSourceUrlHistory()
        => SyncRecentValues(RecentSourceUrls, _recentPathHistoryService.GetPaths(_lstRecentSourceUrlsByType, SourceType));

    private void RefreshTargetUrlHistory()
        => SyncRecentValues(RecentTargetUrls, _recentPathHistoryService.GetPaths(_lstRecentTargetUrlsByType, TargetType));

    private IReadOnlyList<string> GetSetupValidationMessages()
    {
        var lstMessages = new List<string>();

        if (string.IsNullOrWhiteSpace(SourceUrl))
            lstMessages.Add("Quelle: URL/Pfad fehlt.");

        if (string.IsNullOrWhiteSpace(TargetUrl))
            lstMessages.Add("Ziel: URL/Pfad fehlt.");

        return lstMessages;
    }

    private IReadOnlyList<string> GetOptionValidationMessages()
    {
        var lstMessages = new List<string>();

        if (!CanConfigureAdvancedGitTargetOptions)
            return lstMessages;

        if (UsePipelinedMigration)
        {
            if (PipelinePrefetchCount < 1)
                lstMessages.Add("Pipeline: Prefetch muss größer 0 sein.");

            if (PipelineExportWorkerCount < 1)
                lstMessages.Add("Pipeline: Export-Worker muss größer 0 sein.");
        }

        if (SplitIntoSubdirectoryBranches)
        {
            if (string.IsNullOrWhiteSpace(TargetBranch))
                lstMessages.Add("Ziel: Branch für Root-Branch fehlt.");

            if (BranchSplitDepth is < 1 or > 2)
                lstMessages.Add("Sub-Branches: Es sind nur 1 oder 2 Ebenen erlaubt.");
        }

        return lstMessages;
    }

    private string BuildSetupHintMessage()
    {
        var lstMessages = GetSetupValidationMessages();
        return lstMessages.Count == 0
            ? "Quelle und Ziel sind plausibel. Mit 'Weiter' zu den Optionen."
            : string.Join(" ", lstMessages);
    }

    private string BuildOptionsHintMessage()
    {
        var lstMessages = GetOptionValidationMessages();
        if (lstMessages.Count > 0)
            return string.Join(" ", lstMessages);

        if (!CanConfigureAdvancedGitTargetOptions)
            return "Optionen sind konsistent. Migration kann gestartet werden.";

        if (UsePipelinedMigration && SplitIntoSubdirectoryBranches)
            return $"Pipeline ist aktiv. Der Ziel-Branch dient als Root-Branch; Unterverzeichnisse werden bis Ebene {BranchSplitDepth} direkt in Unter-Branches wie 'KG/2001' übernommen.";

        if (UsePipelinedMigration)
            return "Pipeline ist aktiv. Prefetch und Worker steuern den Export-Vorlauf für SVN→Git.";

        if (SplitIntoSubdirectoryBranches)
            return $"Der Ziel-Branch dient als Root-Branch. Unterverzeichnisse werden bis Ebene {BranchSplitDepth} direkt in Unter-Branches übernommen.";

        return "Optional kann SVN→Git per Pipeline beschleunigt oder direkt in Unter-Branches aufgeteilt werden. Der Ziel-Branch ist dabei der Root-Branch.";
    }

    private void NotifySetupStateChanged()
    {
        OnPropertyChanged(nameof(CanContinueFromSetup));
        OnPropertyChanged(nameof(CanStartMigration));
        OnPropertyChanged(nameof(SetupHintMessage));
    }

    private void NotifyOptionStateChanged()
    {
        OnPropertyChanged(nameof(CanStartMigration));
        OnPropertyChanged(nameof(OptionsHintMessage));
    }

    private void EnsureAdvancedMigrationOptionDefaults()
    {
        if (PipelinePrefetchCount < 1)
            PipelinePrefetchCount = 3;

        if (PipelineExportWorkerCount < 1)
            PipelineExportWorkerCount = 2;

        if (BranchSplitDepth is < 1 or > 2)
            BranchSplitDepth = 1;

        if (!CanConfigureAdvancedGitTargetOptions)
        {
            UsePipelinedMigration = false;
            SplitIntoSubdirectoryBranches = false;
        }
    }

    private string BuildCommitSummary()
        => $"Aktueller Commit: {(string.IsNullOrWhiteSpace(CurrentChangeSetId) ? "-" : CurrentChangeSetId)}";

    private static string BuildEndpointSummaryName(string sUrlOrPath, string? sBranchOrTrunk)
    {
        var sDisplayName = TryGetEndpointLeafName(sUrlOrPath);
        if (string.IsNullOrWhiteSpace(sBranchOrTrunk))
            return sDisplayName;

        return $"{sDisplayName} [{sBranchOrTrunk.Trim()}]";
    }

    private static string TryGetEndpointLeafName(string sUrlOrPath)
    {
        if (string.IsNullOrWhiteSpace(sUrlOrPath))
            return "-";

        if (Uri.TryCreate(sUrlOrPath, UriKind.Absolute, out var uriEndpoint))
        {
            var sSegment = uriEndpoint.Segments.LastOrDefault()?.Trim('/');
            if (!string.IsNullOrWhiteSpace(sSegment))
                return sSegment;
        }

        var sTrimmedPath = sUrlOrPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var sLeafName = Path.GetFileName(sTrimmedPath);
        return string.IsNullOrWhiteSpace(sLeafName) ? sUrlOrPath : sLeafName;
    }

    private void SetWorkflowStage(WorkflowStage workflowStage)
    {
        if (_isLoadingInputState)
            return;

        if (IsRunning && _migration.IsRunning && workflowStage != WorkflowStage.Execution && WorkflowStage == WorkflowStage.Execution)
            return;

        WorkflowStage = workflowStage;
    }

    private async Task RunOperationAsync(bool clearLog, Func<CancellationToken, Task> action, WorkflowStage? workflowStageOnStart = null, WorkflowStage? workflowStageOnFinish = null)
    {
        if (IsRunning)
            return;

        if (workflowStageOnStart is not null)
            SetWorkflowStage(workflowStageOnStart.Value);

        IsRunning = true;
        _cts = new CancellationTokenSource();
        try
        {
            if (clearLog)
            {
                Log = "";
                CurrentChangeSetId = null;
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
            _cts?.Dispose();
            _cts = null;

            if (workflowStageOnFinish is not null)
                SetWorkflowStage(workflowStageOnFinish.Value);
        }
    }

    partial void OnSourceTypeChanged(RepoType value)
    {
        NotifySetupStateChanged();
        NotifyOptionStateChanged();
        RefreshSourceUrlHistory();
        _hsSavedGitBranchSelections.Clear();
        _hsSavedGitTagSelections.Clear();
        _sSavedSvnFromRevisionId = null;
        _sSavedSvnToRevisionId = null;
        EnsureAdvancedMigrationOptionDefaults();
        ClearGitSelections();
        ClearSvnSelections();
        SetWorkflowStage(WorkflowStage.Setup);
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
        SetWorkflowStage(WorkflowStage.Setup);
        SaveInputState();
    }
    partial void OnSourceBranchChanged(string? value)
    {
        NotifySetupStateChanged();
        SetWorkflowStage(WorkflowStage.Setup);
        SaveInputState();
    }
    partial void OnSourceUserChanged(string? value)
    {
        NotifySetupStateChanged();
        SetWorkflowStage(WorkflowStage.Setup);
        SaveInputState();
    }
    partial void OnSourcePasswordChanged(string? value)
    {
        NotifySetupStateChanged();
        SetWorkflowStage(WorkflowStage.Setup);
        SaveInputState();
    }
    partial void OnTargetTypeChanged(RepoType value)
    {
        NotifySetupStateChanged();
        NotifyOptionStateChanged();
        RefreshTargetUrlHistory();
        EnsureAdvancedMigrationOptionDefaults();
        TargetGitBranchOptions.Clear();
        SetWorkflowStage(WorkflowStage.Setup);
        SaveInputState();
    }
    partial void OnTargetUrlChanged(string value)
    {
        NotifyOptionStateChanged();
        TargetGitBranchOptions.Clear();
        SetWorkflowStage(WorkflowStage.Setup);
        SaveInputState();
    }
    partial void OnTargetBranchChanged(string? value)
    {
        NotifySetupStateChanged();
        NotifyOptionStateChanged();
        SetWorkflowStage(WorkflowStage.Setup);
        SaveInputState();
    }
    partial void OnTransferGitBranchesChanged(bool value)
    {
        NotifySetupStateChanged();
        SaveInputState();
    }
    partial void OnTransferGitTagsChanged(bool value)
    {
        NotifySetupStateChanged();
        SaveInputState();
    }
    partial void OnFromIdChanged(string? value)
    {
        NotifySetupStateChanged();
        SaveInputState();
    }
    partial void OnToIdChanged(string? value)
    {
        NotifySetupStateChanged();
        SaveInputState();
    }
    partial void OnMaxCountChanged(int? value)
    {
        NotifySetupStateChanged();
        SaveInputState();
    }
    partial void OnOldestFirstChanged(bool value)
    {
        NotifySetupStateChanged();
        SaveInputState();
    }
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
            RecentSourceUrlsByType = [.. _lstRecentSourceUrlsByType],
            RecentTargetUrlsByType = [.. _lstRecentTargetUrlsByType],
            RecentSourceUrls = RecentSourceUrls.ToList(),
            RecentSourceBranches = RecentSourceBranches.ToList(),
            RecentSourceUsers = RecentSourceUsers.ToList(),
            RecentTargetUrls = RecentTargetUrls.ToList(),
            RecentTargetBranches = RecentTargetBranches.ToList(),
            RecentTargetUsers = RecentTargetUsers.ToList(),
            SelectedSvnFromRevisionId = string.IsNullOrWhiteSpace(SelectedSvnFromRevisionId) ? null : SelectedSvnFromRevisionId,
            SelectedSvnToRevisionId = string.IsNullOrWhiteSpace(SelectedSvnToRevisionId) ? null : SelectedSvnToRevisionId,
            FromId = FromId,
            ToId = ToId,
            MaxCount = MaxCount,
            OldestFirst = OldestFirst,
            UsePipelinedMigration = UsePipelinedMigration,
            PipelinePrefetchCount = PipelinePrefetchCount,
            PipelineExportWorkerCount = PipelineExportWorkerCount,
            SplitIntoSubdirectoryBranches = SplitIntoSubdirectoryBranches,
            BranchSplitDepth = BranchSplitDepth
        });
    }
}
