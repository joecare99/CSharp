namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Defines first-slice options for deterministic archive ordering.
/// </summary>
public sealed class ArchiveOrderingOptions
{
    /// <summary>
    /// Gets a value indicating whether detected version text should participate in ordering when available.
    /// </summary>
    public bool UseDetectedVersionText { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the newest archive entry timestamp should be preferred as the archive metadata timestamp.
    /// </summary>
    public bool PreferNewestEntryTimestamp { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the external last-write timestamp may be used as a fallback ordering signal.
    /// </summary>
    public bool UseExternalLastWriteTimestamp { get; init; } = true;
}
