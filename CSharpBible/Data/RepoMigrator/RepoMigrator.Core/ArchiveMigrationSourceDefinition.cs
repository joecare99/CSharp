namespace RepoMigrator.Core;

/// <summary>
/// Represents the archive-specific source configuration used by archive-backed migration source providers.
/// </summary>
public sealed class ArchiveMigrationSourceDefinition
{
    /// <summary>
    /// Gets the location kind that identifies how the archive collection should be accessed.
    /// </summary>
    public ArchiveSourceLocationKind LocationKind { get; init; } = ArchiveSourceLocationKind.LocalDirectory;

    /// <summary>
    /// Gets the source location as a URL or directory path.
    /// </summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>
    /// Gets the allowed archive file extensions that discovery should consider.
    /// </summary>
    public IReadOnlyList<string> AllowedExtensions { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets a value indicating whether relative links should be followed for HTTP-based discovery.
    /// </summary>
    public bool AllowRelativeLinks { get; init; }

    /// <summary>
    /// Gets a value indicating whether local directory discovery should recurse into subdirectories.
    /// </summary>
    public bool RecursiveDirectoryScan { get; init; }
}
