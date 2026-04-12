namespace RepoMigrator.Core;

/// <summary>
/// Provides repository migration operations.
/// </summary>
public interface IMigrationService
{
    bool IsRunning { get; }

    /// <summary>
    /// Migrates changes from a source repository endpoint to a target repository endpoint.
    /// </summary>
    Task MigrateAsync(
        RepositoryEndpoint source,
        RepositoryEndpoint target,
        ChangeSetQuery query,
        MigrationOptions options,
        IMigrationProgress progress,
        CancellationToken ct);
}
