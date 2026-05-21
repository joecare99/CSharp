namespace RepoMigrator.Core;

/// <summary>
/// Represents one prepared migration source item in a normalized source plan.
/// </summary>
public sealed class MigrationSourcePlanItem
{
    /// <summary>
    /// Gets the stable logical identifier of the prepared source item.
    /// </summary>
    public string ItemId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the stable logical snapshot identifier associated with the source item.
    /// </summary>
    public string SnapshotId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the source-specific identifier or location associated with the planned item.
    /// </summary>
    public string SourceIdentifier { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional human-readable display name associated with the planned item.
    /// </summary>
    public string? DisplayName { get; init; }
}
