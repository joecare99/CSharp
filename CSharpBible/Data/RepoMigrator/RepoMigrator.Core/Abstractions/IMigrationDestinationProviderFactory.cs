namespace RepoMigrator.Core.Abstractions;

/// <summary>
/// Resolves migration destination providers for normalized destination definitions.
/// </summary>
public interface IMigrationDestinationProviderFactory
{
    /// <summary>
    /// Resolves the provider that can handle the supplied destination definition.
    /// </summary>
    IMigrationDestinationProvider Create(MigrationDestinationDefinition destination);
}
