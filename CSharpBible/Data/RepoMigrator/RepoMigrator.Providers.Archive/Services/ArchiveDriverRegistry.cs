using RepoMigrator.Providers.Archive.Abstractions;

namespace RepoMigrator.Providers.Archive.Services;

/// <summary>
/// Resolves archive drivers from a registered driver set.
/// </summary>
public sealed class ArchiveDriverRegistry : IArchiveDriverRegistry
{
    private readonly IReadOnlyList<IArchiveDriver> _drivers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchiveDriverRegistry"/> class.
    /// </summary>
    public ArchiveDriverRegistry(IEnumerable<IArchiveDriver> drivers)
    {
        ArgumentNullException.ThrowIfNull(drivers);
        _drivers = drivers.ToArray();
    }

    /// <inheritdoc/>
    public IArchiveDriver Resolve(string archivePathOrName)
        => TryResolve(archivePathOrName, out var driver)
            ? driver
            : throw new NotSupportedException($"No archive driver is registered for '{archivePathOrName}'.");

    /// <inheritdoc/>
    public bool TryResolve(string archivePathOrName, out IArchiveDriver? driver)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(archivePathOrName);
        driver = _drivers.FirstOrDefault(candidate => candidate.CanHandle(archivePathOrName));
        return driver is not null;
    }
}
