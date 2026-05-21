namespace RepoMigrator.Providers.Archive.Services;

/// <summary>
/// Detects effective archive extraction-root paths from inspected archive entries.
/// </summary>
public sealed class ArchiveExtractionRootDetectionService
{
    /// <summary>
    /// Detects extraction-root paths for the supplied archive plan candidates.
    /// </summary>
    /// <param name="items">The source items to evaluate.</param>
    /// <param name="inspections">The corresponding inspection results keyed by snapshot identifier.</param>
    /// <param name="manualOverrides">The optional per-archive manual overrides keyed by archive item identifier.</param>
    /// <returns>The detected extraction-root paths keyed by snapshot identifier.</returns>
    public IReadOnlyDictionary<string, string> Detect(
        IReadOnlyList<RepoMigrator.Core.MigrationSourcePlanItem> items,
        IReadOnlyDictionary<string, ArchiveInspectionResult> inspections,
        IReadOnlyDictionary<string, string> manualOverrides)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(inspections);
        ArgumentNullException.ThrowIfNull(manualOverrides);

        var nearestDataEntryPathBySnapshotId = items.ToDictionary(
            item => item.SnapshotId,
            item => GetReferenceDataEntryPath(inspections[item.SnapshotId]),
            StringComparer.Ordinal);
        var searchCandidatePaths = items
            .Select(item => nearestDataEntryPathBySnapshotId[item.SnapshotId])
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var detected = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var item in items)
        {
            if (manualOverrides.TryGetValue(item.ItemId, out var manualRootPath))
            {
                detected[item.SnapshotId] = NormalizeRelativePath(manualRootPath);
                continue;
            }

            var inspection = inspections[item.SnapshotId];
            var rootPath = string.Empty;
            foreach (var candidatePath in searchCandidatePaths)
            {
                if (string.IsNullOrWhiteSpace(candidatePath))
                    continue;

                if (inspection.Entries.Any(entry => !entry.IsDirectory && string.Equals(NormalizeRelativePath(entry.EntryPath), candidatePath, StringComparison.Ordinal)))
                {
                    rootPath = GetParentDirectoryPath(candidatePath);
                    break;
                }
            }

            detected[item.SnapshotId] = rootPath;
        }

        return detected;
    }

    /// <summary>
    /// Gets the non-directory archive entry path that is closest to the archive root.
    /// </summary>
    /// <param name="inspection">The inspection result to evaluate.</param>
    /// <returns>The normalized relative entry path that is closest to the archive root.</returns>
    public string GetReferenceDataEntryPath(ArchiveInspectionResult inspection)
    {
        ArgumentNullException.ThrowIfNull(inspection);

        return inspection.Entries
            .Where(static entry => !entry.IsDirectory)
            .Select(static entry => NormalizeRelativePath(entry.EntryPath))
            .Where(static path => !string.IsNullOrWhiteSpace(path))
            .OrderBy(static path => GetPathDepth(path))
            .ThenBy(static path => path, StringComparer.Ordinal)
            .FirstOrDefault(string.Empty);
    }

    private static string GetParentDirectoryPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        var lastSeparatorIndex = path.LastIndexOf('/');
        return lastSeparatorIndex <= 0 ? string.Empty : path[..lastSeparatorIndex];
    }

    private static string NormalizeRelativePath(string path)
        => string.IsNullOrWhiteSpace(path)
            ? string.Empty
            : path.Replace('\\', '/').Trim('/');

    private static int GetPathDepth(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return int.MaxValue;

        var normalizedPath = NormalizeRelativePath(path);
        if (string.IsNullOrWhiteSpace(normalizedPath))
            return int.MaxValue;

        return normalizedPath.Count(static ch => ch == '/');
    }
}
