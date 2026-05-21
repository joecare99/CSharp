namespace RepoMigrator.Core;

/// <summary>
/// Represents a normalized migration destination definition above provider-specific target implementations.
/// </summary>
public sealed class MigrationDestinationDefinition
{
    /// <summary>
    /// Gets the normalized destination kind.
    /// </summary>
    public MigrationDestinationKind Kind { get; init; } = MigrationDestinationKind.Repository;

    /// <summary>
    /// Gets the repository-specific destination endpoint when the destination kind is repository-backed.
    /// </summary>
    public RepositoryEndpoint? Repository { get; init; }

    /// <summary>
    /// Gets provider-specific destination configuration values owned by the responsible destination provider.
    /// </summary>
    public IReadOnlyDictionary<string, string> ProviderData { get; init; } = new Dictionary<string, string>();
}
