namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents one discovered archive snapshot plus the metadata needed for first-slice planning.
/// </summary>
public sealed class ArchiveSnapshotDescriptor
{
    /// <summary>
    /// Gets the stable logical snapshot identifier.
    /// </summary>
    public string SnapshotId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the source archive path or URL.
    /// </summary>
    public string ArchivePathOrUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets the archive file name.
    /// </summary>
    public string ArchiveFileName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the archive base name without the supported archive extension.
    /// </summary>
    public string ArchiveBaseName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the matched archive extension.
    /// </summary>
    public string ArchiveExtension { get; init; } = string.Empty;

    /// <summary>
    /// Gets the archive driver identifier.
    /// </summary>
    public string DriverId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the detected version text when available.
    /// </summary>
    public string? DetectedVersionText { get; init; }

    /// <summary>
    /// Gets the archive creation timestamp when available.
    /// </summary>
    public DateTimeOffset? ArchiveCreatedTimestamp { get; init; }

    /// <summary>
    /// Gets the newest entry timestamp derived from archive inspection when available.
    /// </summary>
    public DateTimeOffset? NewestEntryTimestamp { get; init; }

    /// <summary>
    /// Gets the archive external last-write timestamp when available.
    /// </summary>
    public DateTimeOffset? ExternalLastWriteTimestamp { get; init; }

    /// <summary>
    /// Gets the stable discovery index used as a tie-breaker.
    /// </summary>
    public int DiscoveryIndex { get; init; }

    /// <summary>
    /// Gets the optional manual order index that overrides inferred ordering.
    /// </summary>
    public int? ManualOrderIndex { get; init; }

    /// <summary>
    /// Gets the proposed tag name when already derived.
    /// </summary>
    public string? ProposedTagName { get; init; }

    /// <summary>
    /// Gets the proposed branch name when already derived.
    /// </summary>
    public string? ProposedBranchName { get; init; }

    /// <summary>
    /// Gets the explainable ordering evidence.
    /// </summary>
    public ArchiveOrderingEvidence OrderingEvidence { get; init; } = new();
}
