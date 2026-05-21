namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Defines first-slice options for deriving default tag and branch names from archive files.
/// </summary>
public sealed class ArchiveRefNamingOptions
{
    /// <summary>
    /// Gets the optional prefix to prepend to generated tag names.
    /// </summary>
    public string? TagPrefix { get; init; }

    /// <summary>
    /// Gets the optional prefix to prepend to generated branch names.
    /// </summary>
    public string? BranchPrefix { get; init; }
}
