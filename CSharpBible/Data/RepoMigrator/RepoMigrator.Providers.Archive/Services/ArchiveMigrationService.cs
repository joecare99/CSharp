using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Archive.Abstractions;

namespace RepoMigrator.Providers.Archive.Services;

/// <summary>
/// Orchestrates first-slice archive import execution and deterministic resume checkpoints.
/// </summary>
public sealed class ArchiveMigrationService : IArchiveMigrationService
{
    private readonly IArchiveImportPlanner _planner;
    private readonly IArchiveImportStateStore _stateStore;
    private readonly IArchiveDriverRegistry _archiveDriverRegistry;
    private readonly IMigrationDestinationProviderFactory _destinationProviderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchiveMigrationService"/> class.
    /// </summary>
    public ArchiveMigrationService(
        IArchiveImportPlanner planner,
        IArchiveImportStateStore stateStore,
        IArchiveDriverRegistry archiveDriverRegistry,
        IMigrationDestinationProviderFactory destinationProviderFactory)
    {
        _planner = planner ?? throw new ArgumentNullException(nameof(planner));
        _stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        _archiveDriverRegistry = archiveDriverRegistry ?? throw new ArgumentNullException(nameof(archiveDriverRegistry));
        _destinationProviderFactory = destinationProviderFactory ?? throw new ArgumentNullException(nameof(destinationProviderFactory));
    }

    /// <inheritdoc/>
    public async Task<ArchiveImportPlan> PreparePlanAsync(MigrationSourceDefinition source, MigrationDestinationDefinition destination, CancellationToken ct)
    {
        var plan = await _planner.PrepareAsync(source, destination, ct).ConfigureAwait(false);
        await _stateStore.SavePlanAsync(plan, ct).ConfigureAwait(false);
        await _stateStore.SaveStateAsync(CreateInitialState(plan), ct).ConfigureAwait(false);
        return plan;
    }

    /// <inheritdoc/>
    public Task<ArchiveImportState> ExecuteAsync(string planId, IMigrationProgress progress, CancellationToken ct)
        => ExecuteInternalAsync(planId, progress, resume: false, ct);

    /// <inheritdoc/>
    public Task<ArchiveImportState> ResumeAsync(string planId, IMigrationProgress progress, CancellationToken ct)
        => ExecuteInternalAsync(planId, progress, resume: true, ct);

    private async Task<ArchiveImportState> ExecuteInternalAsync(string planId, IMigrationProgress progress, bool resume, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(planId);
        ArgumentNullException.ThrowIfNull(progress);
        ct.ThrowIfCancellationRequested();

        var plan = await _stateStore.LoadPlanAsync(planId, ct).ConfigureAwait(false);
        var state = await _stateStore.LoadStateAsync(planId, ct).ConfigureAwait(false);
        var destinationProvider = _destinationProviderFactory.Create(plan.Destination);
        await using (destinationProvider.ConfigureAwait(false))
        {
            await destinationProvider.InitializeAsync(plan.Destination, ct).ConfigureAwait(false);
            var refOperations = destinationProvider as IMigrationDestinationRefOperations;
            if (refOperations is null)
                throw new NotSupportedException("The resolved destination provider does not support destination ref operations required for archive migration.");

            state = await SaveStateAsync(new ArchiveImportState
            {
                SchemaVersion = state.SchemaVersion,
                PlanId = state.PlanId,
                Status = ArchiveImportRunStatus.Running,
                LastUpdatedUtc = DateTimeOffset.UtcNow,
                LastMachineName = Environment.MachineName,
                LastWorkspaceRoot = Directory.GetCurrentDirectory(),
                CurrentCheckpoint = state.CurrentCheckpoint,
                Snapshots = state.Snapshots,
                LastValidationSummary = resume ? "Resume validation succeeded." : "Execution started.",
                ExtensionData = state.ExtensionData
            }, ct).ConfigureAwait(false);

            foreach (var item in plan.Items.OrderBy(static item => item.FinalOrderIndex))
            {
                ct.ThrowIfCancellationRequested();
                var snapshotState = GetSnapshotState(state, item.SnapshotId);
                if (snapshotState.BranchCreated || (!item.CreateBranch && snapshotState.TagCreated && snapshotState.CommitCompleted))
                    continue;

                state = await ProcessItemAsync(plan, item, state, snapshotState, destinationProvider, refOperations, progress, ct).ConfigureAwait(false);
            }

            await destinationProvider.FinalizeAsync(ct).ConfigureAwait(false);
        }

        state = new ArchiveImportState
        {
            SchemaVersion = state.SchemaVersion,
            PlanId = state.PlanId,
            Status = ArchiveImportRunStatus.Completed,
            LastUpdatedUtc = DateTimeOffset.UtcNow,
            LastMachineName = state.LastMachineName,
            LastWorkspaceRoot = state.LastWorkspaceRoot,
            CurrentCheckpoint = new ArchiveImportCheckpoint
            {
                Phase = ArchiveImportCheckpointPhase.RunCompleted,
                SnapshotId = string.Empty,
                ItemOrderIndex = null,
                Summary = "Archive import completed.",
                RecordedAtUtc = DateTimeOffset.UtcNow
            },
            Snapshots = state.Snapshots,
            LastValidationSummary = state.LastValidationSummary,
            ExtensionData = state.ExtensionData
        };

        return await SaveStateAsync(state, ct).ConfigureAwait(false);
    }

    private async Task<ArchiveImportState> ProcessItemAsync(
        ArchiveImportPlan plan,
        ArchiveImportPlanItem item,
        ArchiveImportState state,
        ArchiveImportSnapshotState snapshotState,
        IMigrationDestinationProvider destinationProvider,
        IMigrationDestinationRefOperations refOperations,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        var archivePath = item.SourceItem.SourceIdentifier;
        var driver = _archiveDriverRegistry.Resolve(archivePath);
        var extractionRootPath = Path.Combine(Path.GetTempPath(), "RepoMigrator", "ArchiveMigration", plan.PlanId, item.SnapshotId);
        var extractionWorkspacePath = Path.Combine(extractionRootPath, "archive");
        Directory.CreateDirectory(extractionWorkspacePath);

        try
        {
            if (!snapshotState.CommitCompleted)
            {
                await driver.ExtractToAsync(archivePath, extractionWorkspacePath, ct).ConfigureAwait(false);
                snapshotState = CloneSnapshotState(
                    snapshotState,
                    phase: ArchiveImportCheckpointPhase.ExtractionCompleted,
                    acquisitionCompleted: true,
                    extractionCompleted: true,
                    lastAttemptUtc: DateTimeOffset.UtcNow,
                    failureSummary: null);
                state = await SaveSnapshotStateAsync(state, item, snapshotState, ArchiveImportCheckpointPhase.ExtractionCompleted, "Archive extracted.", ct).ConfigureAwait(false);

                var effectiveWorkDirectory = ResolveEffectiveWorkDirectory(item, extractionWorkspacePath);
                await destinationProvider.WriteSnapshotAsync(effectiveWorkDirectory, new MigrationDestinationCommit
                {
                    SnapshotId = item.SnapshotId,
                    Message = $"Import archive snapshot {item.FinalTagName}",
                    AuthorName = "RepoMigrator",
                    AuthorEmail = "repomigrator@example.invalid",
                    Timestamp = ResolveCommitTimestamp(item),
                    DestinationReference = plan.Destination.Repository?.BranchOrTrunk
                }, progress, ct).ConfigureAwait(false);

                var commitId = await refOperations.GetHeadCommitIdAsync(ct).ConfigureAwait(false)
                    ?? throw new InvalidOperationException("The destination provider did not return a head commit id after writing the snapshot.");
                snapshotState = CloneSnapshotState(
                    snapshotState,
                    phase: ArchiveImportCheckpointPhase.CommitCompleted,
                    commitCompleted: true,
                    targetWriteId: commitId,
                    lastAttemptUtc: DateTimeOffset.UtcNow);
                state = await SaveSnapshotStateAsync(state, item, snapshotState, ArchiveImportCheckpointPhase.CommitCompleted, "Snapshot committed.", ct).ConfigureAwait(false);
            }

            if (!snapshotState.TagCreated)
            {
                if (string.IsNullOrWhiteSpace(snapshotState.TargetWriteId))
                    throw new InvalidOperationException($"Snapshot '{item.SnapshotId}' cannot create a tag without a persisted target write id.");

                await refOperations.EnsureTagAsync(item.FinalTagName, snapshotState.TargetWriteId, ct).ConfigureAwait(false);
                snapshotState = CloneSnapshotState(
                    snapshotState,
                    phase: ArchiveImportCheckpointPhase.TagCreated,
                    tagCreated: true,
                    lastAttemptUtc: DateTimeOffset.UtcNow);
                state = await SaveSnapshotStateAsync(state, item, snapshotState, ArchiveImportCheckpointPhase.TagCreated, "Tag created.", ct).ConfigureAwait(false);
            }

            if (item.CreateBranch && !snapshotState.BranchCreated)
            {
                if (string.IsNullOrWhiteSpace(item.FinalBranchName))
                    throw new InvalidOperationException($"Snapshot '{item.SnapshotId}' is configured to create a branch but no branch name is available.");
                if (string.IsNullOrWhiteSpace(snapshotState.TargetWriteId))
                    throw new InvalidOperationException($"Snapshot '{item.SnapshotId}' cannot create a branch without a persisted target write id.");

                await refOperations.EnsureBranchAsync(item.FinalBranchName, snapshotState.TargetWriteId, ct).ConfigureAwait(false);
                snapshotState = CloneSnapshotState(
                    snapshotState,
                    phase: ArchiveImportCheckpointPhase.BranchCreated,
                    branchCreated: true,
                    lastAttemptUtc: DateTimeOffset.UtcNow);
                state = await SaveSnapshotStateAsync(state, item, snapshotState, ArchiveImportCheckpointPhase.BranchCreated, "Branch created.", ct).ConfigureAwait(false);
            }
            else if (!item.CreateBranch)
            {
                snapshotState = CloneSnapshotState(
                    snapshotState,
                    phase: ArchiveImportCheckpointPhase.SnapshotCompleted,
                    branchCreated: false,
                    lastAttemptUtc: DateTimeOffset.UtcNow);
            }

            snapshotState = CloneSnapshotState(
                snapshotState,
                phase: ArchiveImportCheckpointPhase.SnapshotCompleted,
                lastAttemptUtc: DateTimeOffset.UtcNow,
                failureSummary: null);
            return await SaveSnapshotStateAsync(state, item, snapshotState, ArchiveImportCheckpointPhase.SnapshotCompleted, "Snapshot completed.", ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            snapshotState = CloneSnapshotState(
                snapshotState,
                lastAttemptUtc: DateTimeOffset.UtcNow,
                failureSummary: ex.Message);
            state = new ArchiveImportState
            {
                SchemaVersion = state.SchemaVersion,
                PlanId = state.PlanId,
                Status = ArchiveImportRunStatus.Failed,
                LastUpdatedUtc = DateTimeOffset.UtcNow,
                LastMachineName = state.LastMachineName,
                LastWorkspaceRoot = state.LastWorkspaceRoot,
                CurrentCheckpoint = state.CurrentCheckpoint,
                Snapshots = state.Snapshots,
                LastValidationSummary = ex.Message,
                ExtensionData = state.ExtensionData
            };
            state = ReplaceSnapshotState(state, snapshotState);
            await SaveStateAsync(state, ct).ConfigureAwait(false);
            throw;
        }
        finally
        {
            TryDeleteDirectory(extractionRootPath);
        }
    }

    private static string ResolveEffectiveWorkDirectory(ArchiveImportPlanItem item, string extractionWorkspacePath)
    {
        if (!item.ExtensionData.TryGetValue(ArchiveImportPlanItemExtensionKeys.ExtractionRootPath, out var configuredRootPath)
            || string.IsNullOrWhiteSpace(configuredRootPath))
            return extractionWorkspacePath;

        var normalizedRootPath = configuredRootPath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        var effectiveWorkDirectory = Path.GetFullPath(Path.Combine(extractionWorkspacePath, normalizedRootPath));
        var fullExtractionWorkspacePath = Path.GetFullPath(extractionWorkspacePath);
        if (!effectiveWorkDirectory.StartsWith(fullExtractionWorkspacePath, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"The configured extraction root '{configuredRootPath}' escapes the extracted archive workspace.");
        if (!Directory.Exists(effectiveWorkDirectory))
            throw new DirectoryNotFoundException($"The configured extraction root '{configuredRootPath}' was not found in extracted archive '{item.SourceItem.ItemId}'.");

        return effectiveWorkDirectory;
    }

    private static DateTimeOffset ResolveCommitTimestamp(ArchiveImportPlanItem item)
    {
        if (item.ExtensionData.TryGetValue(ArchiveImportPlanItemExtensionKeys.CommitTimestamp, out var timestampValue)
            && DateTimeOffset.TryParse(timestampValue, out var commitTimestamp))
        {
            return commitTimestamp;
        }

        return DateTimeOffset.UtcNow;
    }

    private static ArchiveImportState CreateInitialState(ArchiveImportPlan plan)
        => new()
        {
            PlanId = plan.PlanId,
            Status = ArchiveImportRunStatus.NotStarted,
            LastUpdatedUtc = DateTimeOffset.UtcNow,
            CurrentCheckpoint = new ArchiveImportCheckpoint
            {
                Phase = ArchiveImportCheckpointPhase.PlanPrepared,
                SnapshotId = string.Empty,
                Summary = "Plan prepared.",
                RecordedAtUtc = DateTimeOffset.UtcNow
            },
            Snapshots = plan.Items
                .OrderBy(static item => item.FinalOrderIndex)
                .Select(static item => new ArchiveImportSnapshotState { SnapshotId = item.SnapshotId, Phase = ArchiveImportCheckpointPhase.PlanPrepared })
                .ToArray()
        };

    private static ArchiveImportSnapshotState GetSnapshotState(ArchiveImportState state, string snapshotId)
        => state.Snapshots.FirstOrDefault(snapshot => string.Equals(snapshot.SnapshotId, snapshotId, StringComparison.Ordinal))
            ?? new ArchiveImportSnapshotState { SnapshotId = snapshotId, Phase = ArchiveImportCheckpointPhase.None };

    private async Task<ArchiveImportState> SaveSnapshotStateAsync(
        ArchiveImportState state,
        ArchiveImportPlanItem item,
        ArchiveImportSnapshotState snapshotState,
        ArchiveImportCheckpointPhase checkpointPhase,
        string summary,
        CancellationToken ct)
    {
        var replacedState = ReplaceSnapshotState(state, snapshotState);
        var updatedState = new ArchiveImportState
        {
            SchemaVersion = replacedState.SchemaVersion,
            PlanId = replacedState.PlanId,
            Status = checkpointPhase == ArchiveImportCheckpointPhase.SnapshotCompleted ? ArchiveImportRunStatus.Running : replacedState.Status,
            LastUpdatedUtc = DateTimeOffset.UtcNow,
            LastMachineName = replacedState.LastMachineName,
            LastWorkspaceRoot = replacedState.LastWorkspaceRoot,
            CurrentCheckpoint = new ArchiveImportCheckpoint
            {
                Phase = checkpointPhase,
                SnapshotId = item.SnapshotId,
                ItemOrderIndex = item.FinalOrderIndex,
                Summary = summary,
                RecordedAtUtc = DateTimeOffset.UtcNow
            },
            Snapshots = replacedState.Snapshots,
            LastValidationSummary = replacedState.LastValidationSummary,
            ExtensionData = replacedState.ExtensionData
        };

        return await SaveStateAsync(updatedState, ct).ConfigureAwait(false);
    }

    private async Task<ArchiveImportState> SaveStateAsync(ArchiveImportState state, CancellationToken ct)
    {
        await _stateStore.SaveStateAsync(state, ct).ConfigureAwait(false);
        return state;
    }

    private static ArchiveImportState ReplaceSnapshotState(ArchiveImportState state, ArchiveImportSnapshotState snapshotState)
    {
        var snapshots = state.Snapshots
            .Where(existing => !string.Equals(existing.SnapshotId, snapshotState.SnapshotId, StringComparison.Ordinal))
            .Append(snapshotState)
            .OrderBy(static item => item.SnapshotId, StringComparer.Ordinal)
            .ToArray();

        return new ArchiveImportState
        {
            SchemaVersion = state.SchemaVersion,
            PlanId = state.PlanId,
            Status = state.Status,
            LastUpdatedUtc = state.LastUpdatedUtc,
            LastMachineName = state.LastMachineName,
            LastWorkspaceRoot = state.LastWorkspaceRoot,
            CurrentCheckpoint = state.CurrentCheckpoint,
            Snapshots = snapshots,
            LastValidationSummary = state.LastValidationSummary,
            ExtensionData = state.ExtensionData
        };
    }

    private static ArchiveImportSnapshotState CloneSnapshotState(
        ArchiveImportSnapshotState snapshotState,
        ArchiveImportCheckpointPhase? phase = null,
        bool? acquisitionCompleted = null,
        bool? extractionCompleted = null,
        bool? commitCompleted = null,
        string? targetWriteId = null,
        bool? tagCreated = null,
        bool? branchCreated = null,
        string? failureSummary = null,
        DateTimeOffset? lastAttemptUtc = null)
        => new()
        {
            SnapshotId = snapshotState.SnapshotId,
            Phase = phase ?? snapshotState.Phase,
            AcquisitionCompleted = acquisitionCompleted ?? snapshotState.AcquisitionCompleted,
            ExtractionCompleted = extractionCompleted ?? snapshotState.ExtractionCompleted,
            CommitCompleted = commitCompleted ?? snapshotState.CommitCompleted,
            TargetWriteId = targetWriteId ?? snapshotState.TargetWriteId,
            TagCreated = tagCreated ?? snapshotState.TagCreated,
            BranchCreated = branchCreated ?? snapshotState.BranchCreated,
            FailureSummary = failureSummary,
            LastAttemptUtc = lastAttemptUtc ?? snapshotState.LastAttemptUtc,
            ExtensionData = snapshotState.ExtensionData
        };

    private static void TryDeleteDirectory(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
                return;

            foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
                File.SetAttributes(filePath, FileAttributes.Normal);

            Directory.Delete(directoryPath, recursive: true);
        }
        catch
        {
        }
    }
}
