namespace RepoMigrator.Providers.Archive.Abstractions;

/// <summary>
/// Defines archive inspection and extraction behavior for one concrete archive format.
/// </summary>
public interface IArchiveDriver
{
    /// <summary>
    /// Gets the driver identifier.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Determines whether the driver can handle the supplied archive file path or name.
    /// </summary>
    bool CanHandle(string archivePathOrName);

    /// <summary>
    /// Inspects the supplied archive and returns metadata needed for planning and diagnostics.
    /// </summary>
    Task<ArchiveInspectionResult> InspectAsync(string archiveFilePath, CancellationToken ct);

    /// <summary>
    /// Extracts the supplied archive into the target directory.
    /// </summary>
    Task ExtractToAsync(string archiveFilePath, string targetDirectory, CancellationToken ct);
}
