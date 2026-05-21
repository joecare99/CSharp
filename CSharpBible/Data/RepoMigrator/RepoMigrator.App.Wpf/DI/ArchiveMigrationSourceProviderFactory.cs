using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Archive.Services;

namespace RepoMigrator.App.Wpf;

/// <summary>
/// Resolves archive source providers used by the WPF application archive workflow.
/// </summary>
public sealed class ArchiveMigrationSourceProviderFactory : IMigrationSourceProviderFactory
{
    private readonly DirectoryArchiveSnapshotSourceProvider _directoryArchiveSnapshotSourceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchiveMigrationSourceProviderFactory"/> class.
    /// </summary>
    /// <param name="directoryArchiveSnapshotSourceProvider">The local directory archive source provider.</param>
    public ArchiveMigrationSourceProviderFactory(DirectoryArchiveSnapshotSourceProvider directoryArchiveSnapshotSourceProvider)
        => _directoryArchiveSnapshotSourceProvider = directoryArchiveSnapshotSourceProvider ?? throw new ArgumentNullException(nameof(directoryArchiveSnapshotSourceProvider));

    /// <inheritdoc />
    public IMigrationSourceProvider Create(MigrationSourceDefinition source)
        => _directoryArchiveSnapshotSourceProvider.CanHandle(source)
            ? _directoryArchiveSnapshotSourceProvider
            : throw new NotSupportedException("The supplied archive source definition is not supported by the WPF application.");
}
