namespace RepoMigrator.Core.Abstractions;

/// <summary>
/// Resolves structured change-set sources for normalized migration source definitions.
/// </summary>
public interface IMigrationChangeSetSourceFactory
{
    /// <summary>
    /// Resolves the structured change-set source that can handle the supplied source definition.
    /// </summary>
    IMigrationChangeSetSource Create(MigrationSourceDefinition source);
}
