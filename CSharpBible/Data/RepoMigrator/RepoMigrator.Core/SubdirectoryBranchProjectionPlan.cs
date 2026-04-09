namespace RepoMigrator.Core;

/// <summary>
/// Represents one projected branch together with the tracked paths that belong to it.
/// </summary>
public sealed class SubdirectoryBranchProjectionPlan
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubdirectoryBranchProjectionPlan" /> class.
    /// </summary>
    /// <param name="sBranchName">The branch name that should receive the projected snapshot.</param>
    /// <param name="lstPaths">The tracked paths that should remain in that branch.</param>
    public SubdirectoryBranchProjectionPlan(string sBranchName, IReadOnlySet<string> lstPaths)
    {
        BranchName = sBranchName;
        Paths = lstPaths;
    }

    /// <summary>
    /// Gets the projected branch name.
    /// </summary>
    public string BranchName { get; }

    /// <summary>
    /// Gets the tracked paths that should remain in the projected branch snapshot.
    /// </summary>
    public IReadOnlySet<string> Paths { get; }
}
