namespace RepoMigrator.Core.Abstractions;

/// <summary>
/// Resolves structured change-set sinks for normalized migration destination definitions.
/// </summary>
public interface IMigrationChangeSetSinkFactory
{
    /// <summary>
    /// Resolves the structured change-set sink that can handle the supplied destination definition.
    /// </summary>
    IMigrationChangeSetSink Create(MigrationDestinationDefinition destination);
}
