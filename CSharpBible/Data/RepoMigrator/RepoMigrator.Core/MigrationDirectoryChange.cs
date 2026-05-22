namespace RepoMigrator.Core;

/// <summary>
/// Represents one normalized directory-level change inside a structured migration change set.
/// </summary>
public sealed class MigrationDirectoryChange
{
    /// <summary>
    /// Gets the normalized directory change kind.
    /// </summary>
    public MigrationDirectoryChangeKind Kind { get; init; } = MigrationDirectoryChangeKind.Rename;

    /// <summary>
    /// Gets the normalized directory path before the change when a prior path exists.
    /// </summary>
    public string PathBefore { get; init; } = string.Empty;

    /// <summary>
    /// Gets the normalized directory path after the change when a resulting path exists.
    /// </summary>
    public string PathAfter { get; init; } = string.Empty;

    /// <summary>
    /// Gets the ordered path rewrite rules already applied while normalizing this change.
    /// </summary>
    public IReadOnlyList<PathRewriteRule> AppliedPathRewrites { get; init; } = Array.Empty<PathRewriteRule>();

    /// <summary>
    /// Gets optional provider-agnostic metadata preserved for emulation, inference, or diagnostics.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
