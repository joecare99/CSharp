namespace RepoMigrator.Core.Abstractions;

/// <summary>
/// Emits provider-agnostic structured change sets from a normalized migration source definition.
/// </summary>
public interface IMigrationChangeSetSource
{
    /// <summary>
    /// Gets the provider display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Determines whether the source can emit structured changes for the supplied source definition.
    /// </summary>
    bool CanHandle(MigrationSourceDefinition source);

    /// <summary>
    /// Gets the structured-change capabilities supported by the source for the supplied source definition.
    /// </summary>
    Task<ChangeApplicationCapabilities> GetCapabilitiesAsync(MigrationSourceDefinition source, CancellationToken ct);

    /// <summary>
    /// Emits normalized change sets for the supplied source definition.
    /// </summary>
    Task<IReadOnlyList<MigrationChangeSet>> GetChangeSetsAsync(MigrationSourceDefinition source, CancellationToken ct);
}
