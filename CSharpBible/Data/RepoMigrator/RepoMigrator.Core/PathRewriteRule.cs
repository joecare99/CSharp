namespace RepoMigrator.Core;

/// <summary>
/// Represents one explicit path-prefix rewrite applied while normalizing source-specific paths.
/// </summary>
public sealed class PathRewriteRule
{
    /// <summary>
    /// Gets the normalized source prefix that should be rewritten.
    /// </summary>
    public string FromPrefix { get; init; } = string.Empty;

    /// <summary>
    /// Gets the normalized target prefix that should replace the source prefix.
    /// </summary>
    public string ToPrefix { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether directory separators should be normalized while evaluating the rule.
    /// </summary>
    public bool NormalizeDirectorySeparators { get; init; }

    /// <summary>
    /// Gets a value indicating whether prefix matching should ignore path casing differences.
    /// </summary>
    public bool IgnoreCase { get; init; }
}
