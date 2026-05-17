namespace RepoMigrator.Core.Abstractions;

/// <summary>
/// Provides normalized destination-writing behavior for migration outputs.
/// </summary>
public interface IMigrationDestinationProvider : IAsyncDisposable
{
    /// <summary>
    /// Gets the provider display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Determines whether the provider can write to the supplied destination definition.
    /// </summary>
    bool CanHandle(MigrationDestinationDefinition destination);

    /// <summary>
    /// Initializes the destination for subsequent snapshot writes.
    /// </summary>
    Task InitializeAsync(MigrationDestinationDefinition destination, CancellationToken ct);

    /// <summary>
    /// Writes one prepared snapshot from the supplied working directory to the destination.
    /// </summary>
    Task WriteSnapshotAsync(string workDir, MigrationDestinationCommit metadata, IMigrationProgress progress, CancellationToken ct);

    /// <summary>
    /// Finalizes destination output after all snapshot writes completed.
    /// </summary>
    Task FinalizeAsync(CancellationToken ct);
}
