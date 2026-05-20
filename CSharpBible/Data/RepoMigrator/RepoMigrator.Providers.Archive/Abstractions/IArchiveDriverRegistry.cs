namespace RepoMigrator.Providers.Archive.Abstractions;

/// <summary>
/// Resolves archive drivers for concrete archive files.
/// </summary>
public interface IArchiveDriverRegistry
{
    /// <summary>
    /// Resolves the driver that can handle the supplied archive file path or name.
    /// </summary>
    IArchiveDriver Resolve(string archivePathOrName);

    /// <summary>
    /// Attempts to resolve the driver that can handle the supplied archive file path or name.
    /// </summary>
    bool TryResolve(string archivePathOrName, out IArchiveDriver? driver);
}
