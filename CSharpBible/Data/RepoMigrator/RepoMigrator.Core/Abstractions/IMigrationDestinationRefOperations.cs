namespace RepoMigrator.Core.Abstractions;

/// <summary>
/// Exposes optional destination-side reference operations for commit-addressable targets.
/// </summary>
public interface IMigrationDestinationRefOperations
{
    /// <summary>
    /// Gets the current destination-side head commit or write identifier when available.
    /// </summary>
    Task<string?> GetHeadCommitIdAsync(CancellationToken ct);

    /// <summary>
    /// Determines whether the supplied branch already exists on the destination.
    /// </summary>
    Task<bool> BranchExistsAsync(string branchName, CancellationToken ct);

    /// <summary>
    /// Determines whether the supplied tag already exists on the destination.
    /// </summary>
    Task<bool> TagExistsAsync(string tagName, CancellationToken ct);

    /// <summary>
    /// Ensures that the supplied branch exists on the supplied commit or write identifier.
    /// </summary>
    Task EnsureBranchAsync(string branchName, string commitId, CancellationToken ct);

    /// <summary>
    /// Ensures that the supplied tag exists on the supplied commit or write identifier.
    /// </summary>
    Task EnsureTagAsync(string tagName, string commitId, CancellationToken ct);
}
