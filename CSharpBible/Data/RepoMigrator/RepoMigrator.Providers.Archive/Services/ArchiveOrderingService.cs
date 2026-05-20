using System.Text.RegularExpressions;

namespace RepoMigrator.Providers.Archive.Services;

/// <summary>
/// Produces first-slice deterministic ordering decisions for discovered archive snapshots.
/// </summary>
public sealed class ArchiveOrderingService
{
    private static readonly Regex VersionRegex = new(@"\d+(?:\.\d+)+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Orders the supplied archive snapshots deterministically and returns explainable decisions.
    /// </summary>
    /// <param name="snapshots">The snapshots to order.</param>
    /// <param name="options">The optional ordering options.</param>
    /// <returns>The ordered archive decisions.</returns>
    public IReadOnlyList<ArchiveOrderingDecision> Order(IReadOnlyList<ArchiveSnapshotDescriptor> snapshots, ArchiveOrderingOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(snapshots);
        options ??= new ArchiveOrderingOptions();

        var decisions = snapshots
            .Select(snapshot => new Candidate(snapshot, BuildSignals(snapshot, options), ParseVersion(snapshot, options)))
            .OrderBy(candidate => candidate.Snapshot.ManualOrderIndex ?? int.MaxValue)
            .ThenBy(candidate => candidate.Version, VersionKeyComparer.Instance)
            .ThenBy(candidate => GetPrimaryTimestamp(candidate.Snapshot, options) ?? DateTimeOffset.MaxValue)
            .ThenBy(candidate => candidate.Snapshot.ArchiveFileName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(candidate => candidate.Snapshot.ArchiveFileName, StringComparer.Ordinal)
            .ThenBy(candidate => candidate.Snapshot.DiscoveryIndex)
            .Select((candidate, index) => new ArchiveOrderingDecision
            {
                SnapshotId = candidate.Snapshot.SnapshotId,
                FinalOrderIndex = index,
                Evidence = new ArchiveOrderingEvidence
                {
                    Signals = candidate.Signals
                }
            })
            .ToArray();

        return decisions;
    }

    private static IReadOnlyList<ArchiveOrderingSignal> BuildSignals(ArchiveSnapshotDescriptor snapshot, ArchiveOrderingOptions options)
    {
        var signals = new List<ArchiveOrderingSignal>();

        if (snapshot.ManualOrderIndex is int manualOrderIndex)
            signals.Add(new ArchiveOrderingSignal { Kind = ArchiveOrderingSignalKind.ManualOrder, Value = manualOrderIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) });

        var versionText = options.UseDetectedVersionText ? GetVersionText(snapshot) : null;
        if (!string.IsNullOrWhiteSpace(versionText))
            signals.Add(new ArchiveOrderingSignal { Kind = ArchiveOrderingSignalKind.DetectedVersion, Value = versionText });

        if (options.PreferNewestEntryTimestamp && snapshot.NewestEntryTimestamp is DateTimeOffset newestEntryTimestamp)
            signals.Add(new ArchiveOrderingSignal { Kind = ArchiveOrderingSignalKind.NewestEntryTimestamp, Value = newestEntryTimestamp.ToString("O", System.Globalization.CultureInfo.InvariantCulture) });

        if (options.UseExternalLastWriteTimestamp && snapshot.ExternalLastWriteTimestamp is DateTimeOffset externalLastWriteTimestamp)
            signals.Add(new ArchiveOrderingSignal { Kind = ArchiveOrderingSignalKind.ExternalLastWriteTimestamp, Value = externalLastWriteTimestamp.ToString("O", System.Globalization.CultureInfo.InvariantCulture) });

        signals.Add(new ArchiveOrderingSignal { Kind = ArchiveOrderingSignalKind.ArchiveFileName, Value = snapshot.ArchiveFileName });
        signals.Add(new ArchiveOrderingSignal { Kind = ArchiveOrderingSignalKind.DiscoveryIndex, Value = snapshot.DiscoveryIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) });
        return signals;
    }

    private static VersionKey? ParseVersion(ArchiveSnapshotDescriptor snapshot, ArchiveOrderingOptions options)
    {
        if (!options.UseDetectedVersionText)
            return null;

        var versionText = GetVersionText(snapshot);
        return string.IsNullOrWhiteSpace(versionText)
            ? null
            : VersionKey.Parse(versionText);
    }

    private static string? GetVersionText(ArchiveSnapshotDescriptor snapshot)
    {
        if (!string.IsNullOrWhiteSpace(snapshot.DetectedVersionText))
            return snapshot.DetectedVersionText;

        var match = VersionRegex.Match(snapshot.ArchiveBaseName);
        if (match.Success)
            return match.Value;

        match = VersionRegex.Match(snapshot.ArchiveFileName);
        return match.Success ? match.Value : null;
    }

    private static DateTimeOffset? GetPrimaryTimestamp(ArchiveSnapshotDescriptor snapshot, ArchiveOrderingOptions options)
    {
        if (options.PreferNewestEntryTimestamp && snapshot.NewestEntryTimestamp is not null)
            return snapshot.NewestEntryTimestamp;

        if (options.UseExternalLastWriteTimestamp && snapshot.ExternalLastWriteTimestamp is not null)
            return snapshot.ExternalLastWriteTimestamp;

        return snapshot.ArchiveCreatedTimestamp;
    }

    private sealed record Candidate(ArchiveSnapshotDescriptor Snapshot, IReadOnlyList<ArchiveOrderingSignal> Signals, VersionKey? Version);

    private sealed class VersionKeyComparer : IComparer<VersionKey?>
    {
        public static VersionKeyComparer Instance { get; } = new();

        public int Compare(VersionKey? x, VersionKey? y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x is null)
                return 1;
            if (y is null)
                return -1;

            var length = Math.Max(x.Parts.Count, y.Parts.Count);
            for (var i = 0; i < length; i++)
            {
                var left = i < x.Parts.Count ? x.Parts[i] : 0;
                var right = i < y.Parts.Count ? y.Parts[i] : 0;
                var compare = left.CompareTo(right);
                if (compare != 0)
                    return compare;
            }

            return 0;
        }
    }

    private sealed class VersionKey
    {
        public required IReadOnlyList<int> Parts { get; init; }

        public static VersionKey Parse(string versionText)
        {
            var parts = versionText
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Select(static part => int.TryParse(part, out var value) ? value : 0)
                .ToArray();
            return new VersionKey { Parts = parts };
        }
    }
}
