using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Archive.Services;

namespace RepoMigrator.Tools.ArchiveSmokeTest;

/// <summary>
/// Resolves the local directory archive source provider used by the smoke-test tool.
/// </summary>
public sealed class ArchiveSmokeTestProviderFactory : IMigrationSourceProviderFactory
{
    private readonly DirectoryArchiveSnapshotSourceProvider _directoryArchiveSnapshotSourceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchiveSmokeTestProviderFactory"/> class.
    /// </summary>
    public ArchiveSmokeTestProviderFactory(DirectoryArchiveSnapshotSourceProvider directoryArchiveSnapshotSourceProvider)
    {
        _directoryArchiveSnapshotSourceProvider = directoryArchiveSnapshotSourceProvider ?? throw new ArgumentNullException(nameof(directoryArchiveSnapshotSourceProvider));
    }

    /// <inheritdoc />
    public IMigrationSourceProvider Create(MigrationSourceDefinition source)
        => _directoryArchiveSnapshotSourceProvider.CanHandle(source)
            ? _directoryArchiveSnapshotSourceProvider
            : throw new NotSupportedException("The supplied source definition is not supported by the archive smoke-test tool.");
}
