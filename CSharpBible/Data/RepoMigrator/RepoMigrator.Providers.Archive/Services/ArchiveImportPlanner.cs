using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Archive.Abstractions;

namespace RepoMigrator.Providers.Archive.Services;

/// <summary>
/// Combines archive discovery, inspection, ordering, and naming into a durable archive import plan.
/// </summary>
public sealed class ArchiveImportPlanner : IArchiveImportPlanner
{
    private readonly IMigrationSourceProviderFactory _sourceProviderFactory;
    private readonly IArchiveDriverRegistry _archiveDriverRegistry;
    private readonly ArchiveOrderingService _orderingService;
    private readonly ArchiveRefNamingService _namingService;
    private readonly ArchiveExtractionRootDetectionService _extractionRootDetectionService;
    private readonly ArchiveExtractionRootConfigurationStore _extractionRootConfigurationStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchiveImportPlanner"/> class.
    /// </summary>
    /// <param name="sourceProviderFactory">The source provider factory used for normalized discovery.</param>
    /// <param name="archiveDriverRegistry">The archive driver registry used for archive inspection.</param>
    /// <param name="orderingService">The ordering service used to derive deterministic snapshot order.</param>
    /// <param name="namingService">The naming service used to derive default tags and branches.</param>
    public ArchiveImportPlanner(
        IMigrationSourceProviderFactory sourceProviderFactory,
        IArchiveDriverRegistry archiveDriverRegistry,
        ArchiveOrderingService orderingService,
        ArchiveRefNamingService namingService,
        ArchiveExtractionRootDetectionService? extractionRootDetectionService = null,
        ArchiveExtractionRootConfigurationStore? extractionRootConfigurationStore = null)
    {
        _sourceProviderFactory = sourceProviderFactory ?? throw new ArgumentNullException(nameof(sourceProviderFactory));
        _archiveDriverRegistry = archiveDriverRegistry ?? throw new ArgumentNullException(nameof(archiveDriverRegistry));
        _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));
        _namingService = namingService ?? throw new ArgumentNullException(nameof(namingService));
        _extractionRootDetectionService = extractionRootDetectionService ?? new ArchiveExtractionRootDetectionService();
        _extractionRootConfigurationStore = extractionRootConfigurationStore ?? new ArchiveExtractionRootConfigurationStore();
    }

    /// <inheritdoc/>
    public async Task<ArchiveImportPlan> PrepareAsync(MigrationSourceDefinition source, MigrationDestinationDefinition destination, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        ct.ThrowIfCancellationRequested();

        var sourceProvider = _sourceProviderFactory.Create(source);
        var sourcePlan = await sourceProvider.PrepareAsync(source, ct).ConfigureAwait(false);
        var archiveSource = ArchiveMigrationSourceDefinition.FromMigrationSourceDefinition(source);
        var detailedInspectionsBySnapshotId = await InspectDetailedSnapshotsAsync(sourcePlan.Items, ct).ConfigureAwait(false);
        var descriptors = CreateSnapshotDescriptors(sourcePlan.Items, detailedInspectionsBySnapshotId);
        var branchOptions = destination.Kind == MigrationDestinationKind.Repository
            ? new ArchiveBranchOptions { CreateBranches = true }
            : new ArchiveBranchOptions { CreateBranches = false };
        var extractionRootConfiguration = _extractionRootConfigurationStore.Load(archiveSource);
        var manualOverrides = extractionRootConfiguration.Entries.ToDictionary(entry => entry.ArchiveItemId, entry => entry.RootPath, StringComparer.Ordinal);
        var extractionRootPaths = _extractionRootDetectionService.Detect(sourcePlan.Items, detailedInspectionsBySnapshotId, manualOverrides);
        var orderingDecisions = _orderingService.Order(descriptors);
        var descriptorsBySnapshotId = descriptors.ToDictionary(descriptor => descriptor.SnapshotId, StringComparer.Ordinal);
        var sourceItemsBySnapshotId = sourcePlan.Items.ToDictionary(item => item.SnapshotId, StringComparer.Ordinal);
        var planItems = orderingDecisions
            .Select(decision => CreatePlanItem(decision, descriptorsBySnapshotId[decision.SnapshotId], sourceItemsBySnapshotId[decision.SnapshotId], branchOptions, extractionRootPaths[decision.SnapshotId]))
            .ToArray();

        return new ArchiveImportPlan
        {
            PlanId = CreatePlanId(sourcePlan.Items),
            CreatedUtc = DateTimeOffset.UtcNow,
            Source = source,
            Destination = destination,
            Items = planItems,
            Status = ArchiveImportPlanStatus.Ready,
            SourceProviderData = new Dictionary<string, string>(source.ProviderData, StringComparer.OrdinalIgnoreCase),
            DestinationProviderData = new Dictionary<string, string>(destination.ProviderData, StringComparer.OrdinalIgnoreCase)
        };
    }

    private async Task<IReadOnlyDictionary<string, ArchiveInspectionResult>> InspectDetailedSnapshotsAsync(IReadOnlyList<MigrationSourcePlanItem> items, CancellationToken ct)
    {
        var inspections = new Dictionary<string, ArchiveInspectionResult>(StringComparer.Ordinal);
        foreach (var item in items)
        {
            ct.ThrowIfCancellationRequested();
            var driver = _archiveDriverRegistry.Resolve(item.SourceIdentifier);
            inspections[item.SnapshotId] = await driver.InspectAsync(item.SourceIdentifier, ct).ConfigureAwait(false);
        }

        return inspections;
    }

    private IReadOnlyList<ArchiveSnapshotDescriptor> CreateSnapshotDescriptors(
        IReadOnlyList<MigrationSourcePlanItem> items,
        IReadOnlyDictionary<string, ArchiveInspectionResult> inspectionsBySnapshotId)
    {
        var descriptors = new List<ArchiveSnapshotDescriptor>(items.Count);
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var inspection = inspectionsBySnapshotId[item.SnapshotId];
            var fileInfo = new FileInfo(item.SourceIdentifier);
            var fileName = !string.IsNullOrWhiteSpace(item.DisplayName)
                ? item.DisplayName
                : Path.GetFileName(item.SourceIdentifier);
            var archiveBaseName = _namingService.GetArchiveBaseName(new ArchiveSnapshotDescriptor
            {
                ArchivePathOrUrl = item.SourceIdentifier,
                ArchiveFileName = fileName
            });

            descriptors.Add(new ArchiveSnapshotDescriptor
            {
                SnapshotId = item.SnapshotId,
                ArchivePathOrUrl = item.SourceIdentifier,
                ArchiveFileName = fileName,
                ArchiveBaseName = archiveBaseName,
                ArchiveExtension = GetArchiveExtension(fileName),
                DriverId = inspection.DriverId,
                DetectedVersionText = null,
                ArchiveCreatedTimestamp = null,
                NewestEntryTimestamp = inspection.NewestEntryTimestamp,
                ExternalLastWriteTimestamp = fileInfo.Exists ? fileInfo.LastWriteTimeUtc : null,
                DiscoveryIndex = i
            });
        }

        return descriptors;
    }

    private ArchiveImportPlanItem CreatePlanItem(
        ArchiveOrderingDecision decision,
        ArchiveSnapshotDescriptor descriptor,
        MigrationSourcePlanItem sourceItem,
        ArchiveBranchOptions branchOptions,
        string extractionRootPath)
    {
        var tagName = _namingService.CreateTagName(descriptor);
        var branchName = _namingService.CreateBranchName(descriptor, branchOptions);
        var extensionData = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ArchiveImportPlanItemExtensionKeys.DriverId] = descriptor.DriverId,
            [ArchiveImportPlanItemExtensionKeys.ArchivePathOrUrl] = descriptor.ArchivePathOrUrl,
            [ArchiveImportPlanItemExtensionKeys.ArchiveFileName] = descriptor.ArchiveFileName,
            [ArchiveImportPlanItemExtensionKeys.ArchiveBaseName] = descriptor.ArchiveBaseName,
            [ArchiveImportPlanItemExtensionKeys.ExtractionRootPath] = extractionRootPath
        };

        foreach (var signal in decision.Evidence.Signals)
            extensionData[$"Ordering.{signal.Kind}"] = signal.Value;

        return new ArchiveImportPlanItem
        {
            SnapshotId = descriptor.SnapshotId,
            FinalOrderIndex = decision.FinalOrderIndex,
            SourceItem = sourceItem,
            FinalTagName = tagName,
            FinalBranchName = branchName,
            CreateBranch = !string.IsNullOrWhiteSpace(branchName),
            ExtensionData = extensionData
        };
    }

    private static string CreatePlanId(IReadOnlyList<MigrationSourcePlanItem> items)
    {
        if (items.Count == 0)
            return $"archive-import-{Guid.NewGuid():N}";

        var firstItemName = Path.GetFileNameWithoutExtension(items[0].ItemId.Replace('/', Path.DirectorySeparatorChar));
        var normalized = string.Concat((firstItemName ?? "archive-import")
            .Select(ch => char.IsLetterOrDigit(ch) ? char.ToLowerInvariant(ch) : '-'))
            .Trim('-');
        if (string.IsNullOrWhiteSpace(normalized))
            normalized = "archive-import";

        return $"{normalized}-{items.Count:D4}";
    }

    private static string GetArchiveExtension(string fileName)
    {
        if (fileName.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
            return ".tar.gz";

        return Path.GetExtension(fileName);
    }
}
