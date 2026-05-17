namespace RepoMigrator.Core.Abstractions;

/// <summary>
/// Resolves migration source providers for normalized source definitions.
/// </summary>
public interface IMigrationSourceProviderFactory
{
    /// <summary>
    /// Resolves the provider that can handle the supplied source definition.
    /// </summary>
    IMigrationSourceProvider Create(MigrationSourceDefinition source);
}
