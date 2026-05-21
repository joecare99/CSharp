namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Defines first-slice options for archive-derived release branches.
/// </summary>
public sealed class ArchiveBranchOptions
{
    /// <summary>
    /// Gets a value indicating whether release branches should be created in addition to required tags.
    /// </summary>
    public bool CreateBranches { get; init; }

    /// <summary>
    /// Gets the branch path prefix used when creating release branches.
    /// </summary>
    public string BranchPrefix { get; init; } = "releases/";
}
