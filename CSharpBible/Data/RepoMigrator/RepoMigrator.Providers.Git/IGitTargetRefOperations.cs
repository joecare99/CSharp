namespace RepoMigrator.Providers.Git;

/// <summary>
/// Exposes Git-specific target-side ref operations needed by repository-backed destination flows.
/// </summary>
public interface IGitTargetRefOperations
{
    /// <summary>
    /// Gets the current HEAD commit identifier when available.
    /// </summary>
    Task<string?> GetHeadCommitIdAsync(CancellationToken ct);

    /// <summary>
    /// Determines whether the supplied local branch already exists.
    /// </summary>
    Task<bool> BranchExistsAsync(string branchName, CancellationToken ct);

    /// <summary>
    /// Determines whether the supplied tag already exists.
    /// </summary>
    Task<bool> TagExistsAsync(string tagName, CancellationToken ct);

    /// <summary>
    /// Ensures that the supplied branch exists on the supplied commit.
    /// </summary>
    Task EnsureBranchAsync(string branchName, string commitId, CancellationToken ct);

    /// <summary>
    /// Ensures that the supplied tag exists on the supplied commit.
    /// </summary>
    Task EnsureTagAsync(string tagName, string commitId, CancellationToken ct);
}
