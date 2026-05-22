namespace RepoMigrator.Core;

/// <summary>
/// Represents the selected execution path and related diagnostics for a migration decision.
/// </summary>
public sealed class MigrationExecutionPathSelection
{
    /// <summary>
    /// Gets the selected execution path kind.
    /// </summary>
    public MigrationExecutionPathKind Kind { get; init; } = MigrationExecutionPathKind.Unknown;

    /// <summary>
    /// Gets the concise rationale for the selected execution path.
    /// </summary>
    public string Rationale { get; init; } = string.Empty;

    /// <summary>
    /// Gets the rejected alternatives that were considered during path selection.
    /// </summary>
    public IReadOnlyList<MigrationExecutionPathKind> RejectedAlternatives { get; init; } = Array.Empty<MigrationExecutionPathKind>();

    /// <summary>
    /// Gets optional provider-agnostic metadata preserved for diagnostics.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
