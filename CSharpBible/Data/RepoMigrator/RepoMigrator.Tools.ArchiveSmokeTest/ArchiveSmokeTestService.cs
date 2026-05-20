using RepoMigrator.Core;
using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Archive.Abstractions;
using RepoMigrator.Providers.Archive.Services;

namespace RepoMigrator.Tools.ArchiveSmokeTest;

/// <summary>
/// Runs archive-provider smoke tests against a local archive directory.
/// </summary>
public sealed class ArchiveSmokeTestService
{
    private readonly IArchiveImportPlanner _archiveImportPlanner;
    private readonly IArchiveDriverRegistry _archiveDriverRegistry;
    private readonly IArchiveImportStateStore _archiveImportStateStore;
    private readonly ArchiveExtractionRootDetectionService _archiveExtractionRootDetectionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchiveSmokeTestService"/> class.
    /// </summary>
    public ArchiveSmokeTestService(
        IArchiveImportPlanner archiveImportPlanner,
        IArchiveDriverRegistry archiveDriverRegistry,
        IArchiveImportStateStore archiveImportStateStore,
        ArchiveExtractionRootDetectionService archiveExtractionRootDetectionService)
    {
        _archiveImportPlanner = archiveImportPlanner ?? throw new ArgumentNullException(nameof(archiveImportPlanner));
        _archiveDriverRegistry = archiveDriverRegistry ?? throw new ArgumentNullException(nameof(archiveDriverRegistry));
        _archiveImportStateStore = archiveImportStateStore ?? throw new ArgumentNullException(nameof(archiveImportStateStore));
        _archiveExtractionRootDetectionService = archiveExtractionRootDetectionService ?? throw new ArgumentNullException(nameof(archiveExtractionRootDetectionService));
    }

    /// <summary>
    /// Executes the smoke test and writes a readable summary to the console.
    /// </summary>
    public async Task RunAsync(ArchiveSmokeTestOptions options, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(options);
        ct.ThrowIfCancellationRequested();

        var source = new ArchiveMigrationSourceDefinition
        {
            LocationKind = ArchiveSourceLocationKind.LocalDirectory,
            Location = options.SourceDirectoryPath,
            RecursiveDirectoryScan = options.Recursive,
            AllowedExtensions = options.AllowedExtensions
        }.ToMigrationSourceDefinition();

        var destination = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.SequentialArchiveCollection
        };

        var plan = await _archiveImportPlanner.PrepareAsync(source, destination, ct).ConfigureAwait(false);
        await _archiveImportStateStore.SavePlanAsync(plan, ct).ConfigureAwait(false);

        Console.WriteLine($"Archive source: {options.SourceDirectoryPath}");
        Console.WriteLine($"Workspace root: {options.WorkspaceRootPath}");
        Console.WriteLine($"Plan id: {plan.PlanId}");
        Console.WriteLine($"Items: {plan.Items.Count}");
        Console.WriteLine($"Plan file: {Path.Combine(_archiveImportStateStore.GetPlanDirectoryPath(plan.PlanId), "plan.json")}");
        Console.WriteLine();

        foreach (var item in plan.Items)
        {
            ct.ThrowIfCancellationRequested();

            var archivePath = item.SourceItem.SourceIdentifier;
            var inspection = await _archiveDriverRegistry.Resolve(archivePath).InspectAsync(archivePath, ct).ConfigureAwait(false);
            var extractionRootPath = item.ExtensionData.TryGetValue(ArchiveImportPlanItemExtensionKeys.ExtractionRootPath, out var configuredRootPath)
                ? configuredRootPath
                : string.Empty;
            var referenceDataEntryPath = _archiveExtractionRootDetectionService.GetReferenceDataEntryPath(inspection);

            Console.WriteLine($"[{item.FinalOrderIndex:D3}] {item.SourceItem.DisplayName ?? item.SourceItem.ItemId}");
            Console.WriteLine($"  Driver: {inspection.DriverId}");
            Console.WriteLine($"  Snapshot: {item.SnapshotId}");
            Console.WriteLine($"  Tag: {item.FinalTagName}");
            Console.WriteLine($"  Branch: {item.FinalBranchName ?? "(disabled)"}");
            Console.WriteLine($"  Reference file: {(string.IsNullOrWhiteSpace(referenceDataEntryPath) ? "(none)" : referenceDataEntryPath)}");
            Console.WriteLine($"  Root path: {(string.IsNullOrWhiteSpace(extractionRootPath) ? "(archive root)" : extractionRootPath)}");
            Console.WriteLine($"  Entries: {inspection.Entries.Count}");
            Console.WriteLine($"  Newest entry: {(inspection.NewestEntryTimestamp.HasValue ? inspection.NewestEntryTimestamp.Value.ToString("O") : "(none)")}");
            Console.WriteLine();
        }
    }
}
