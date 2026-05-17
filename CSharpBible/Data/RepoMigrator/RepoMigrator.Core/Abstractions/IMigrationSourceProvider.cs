namespace RepoMigrator.Core.Abstractions;

/// <summary>
/// Provides normalized source-planning behavior for migration inputs.
/// </summary>
public interface IMigrationSourceProvider
{
    /// <summary>
    /// Gets the provider display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Determines whether the provider can prepare the supplied source definition.
    /// </summary>
    bool CanHandle(MigrationSourceDefinition source);

    /// <summary>
    /// Prepares a normalized source plan for the supplied source definition.
    /// </summary>
    Task<MigrationSourcePlan> PrepareAsync(MigrationSourceDefinition source, CancellationToken ct);
}
