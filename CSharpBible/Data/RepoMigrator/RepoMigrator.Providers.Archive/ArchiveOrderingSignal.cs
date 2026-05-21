namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents one explainable ordering signal captured for an archive snapshot.
/// </summary>
public sealed class ArchiveOrderingSignal
{
    /// <summary>
    /// Gets the kind of ordering signal.
    /// </summary>
    public ArchiveOrderingSignalKind Kind { get; init; }

    /// <summary>
    /// Gets the textual value that was observed for the signal.
    /// </summary>
    public string Value { get; init; } = string.Empty;
}
