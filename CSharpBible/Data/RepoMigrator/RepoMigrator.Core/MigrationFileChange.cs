namespace RepoMigrator.Core;

/// <summary>
/// Represents one normalized file-level change inside a structured migration change set.
/// </summary>
public sealed class MigrationFileChange
{
    /// <summary>
    /// Gets the normalized file change kind.
    /// </summary>
    public MigrationFileChangeKind Kind { get; init; } = MigrationFileChangeKind.Modify;

    /// <summary>
    /// Gets the normalized path before the change when a prior path exists.
    /// </summary>
    public string PathBefore { get; init; } = string.Empty;

    /// <summary>
    /// Gets the normalized path after the change when a resulting path exists.
    /// </summary>
    public string PathAfter { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional normalized text change payload.
    /// </summary>
    public MigrationTextChange? TextChange { get; init; }

    /// <summary>
    /// Gets the optional normalized binary change payload.
    /// </summary>
    public MigrationBinaryChange? BinaryChange { get; init; }

    /// <summary>
    /// Gets the ordered path rewrite rules already applied while normalizing this change.
    /// </summary>
    public IReadOnlyList<PathRewriteRule> AppliedPathRewrites { get; init; } = Array.Empty<PathRewriteRule>();

    /// <summary>
    /// Gets optional provider-agnostic metadata preserved for diagnostics or execution.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
