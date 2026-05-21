namespace RepoMigrator.Core.Abstractions;

/// <summary>
/// Consumes provider-agnostic structured change sets for a normalized migration destination definition.
/// </summary>
public interface IMigrationChangeSetSink : IAsyncDisposable
{
    /// <summary>
    /// Gets the provider display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Determines whether the sink can consume structured changes for the supplied destination definition.
    /// </summary>
    bool CanHandle(MigrationDestinationDefinition destination);

    /// <summary>
    /// Gets the structured-change capabilities supported by the sink for the supplied destination definition.
    /// </summary>
    Task<ChangeApplicationCapabilities> GetCapabilitiesAsync(MigrationDestinationDefinition destination, CancellationToken ct);

    /// <summary>
    /// Initializes the destination for subsequent structured change application.
    /// </summary>
    Task InitializeAsync(MigrationDestinationDefinition destination, CancellationToken ct);

    /// <summary>
    /// Applies one normalized change set to the destination.
    /// </summary>
    Task ApplyChangeSetAsync(MigrationChangeSet changeSet, IMigrationProgress progress, CancellationToken ct);

    /// <summary>
    /// Finalizes destination output after all structured changes completed.
    /// </summary>
    Task FinalizeAsync(CancellationToken ct);
}
