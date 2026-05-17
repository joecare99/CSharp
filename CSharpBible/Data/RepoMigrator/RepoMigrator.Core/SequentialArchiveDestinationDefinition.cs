namespace RepoMigrator.Core;

/// <summary>
/// Represents a future archive-output destination definition that can emit ordered snapshot archives.
/// </summary>
public sealed class SequentialArchiveDestinationDefinition
{
    /// <summary>
    /// Gets the output directory where generated archives should be written.
    /// </summary>
    public string OutputDirectory { get; init; } = string.Empty;

    /// <summary>
    /// Gets the archive file extension that should be emitted for generated snapshots.
    /// </summary>
    public string ArchiveFileExtension { get; init; } = ".zip";

    /// <summary>
    /// Gets a value indicating whether existing generated archives may be overwritten.
    /// </summary>
    public bool OverwriteExistingArchives { get; init; }
}
