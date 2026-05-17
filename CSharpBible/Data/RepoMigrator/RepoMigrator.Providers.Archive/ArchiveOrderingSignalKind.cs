namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Identifies one ordering signal that contributed to an archive ordering decision.
/// </summary>
public enum ArchiveOrderingSignalKind
{
    /// <summary>
    /// No signal was recorded.
    /// </summary>
    None = 0,

    /// <summary>
    /// A manually assigned order index was used.
    /// </summary>
    ManualOrder = 1,

    /// <summary>
    /// A version value derived from archive naming was used.
    /// </summary>
    DetectedVersion = 2,

    /// <summary>
    /// The newest entry timestamp from archive inspection was used.
    /// </summary>
    NewestEntryTimestamp = 3,

    /// <summary>
    /// The archive file system last-write timestamp was used.
    /// </summary>
    ExternalLastWriteTimestamp = 4,

    /// <summary>
    /// A stable file-name fallback was used.
    /// </summary>
    ArchiveFileName = 5,

    /// <summary>
    /// A stable discovery index tie-breaker was used.
    /// </summary>
    DiscoveryIndex = 6
}
