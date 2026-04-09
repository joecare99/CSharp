namespace RepoMigrator.Tools.GitBranchSplitter;

/// <summary>
/// Represents one generated branch together with the tracked paths that remain in it.
/// </summary>
public sealed class BranchSplitPlan
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BranchSplitPlan"/> class.
    /// </summary>
    /// <param name="sBranchName">The generated branch name.</param>
    /// <param name="lstPaths">The tracked paths that should remain in the branch.</param>
    public BranchSplitPlan(string sBranchName, IReadOnlySet<string> lstPaths)
    {
        BranchName = sBranchName;
        Paths = lstPaths;
    }

    /// <summary>
    /// Gets the generated branch name.
    /// </summary>
    public string BranchName { get; }

    /// <summary>
    /// Gets the tracked paths that should remain in the generated branch.
    /// </summary>
    public IReadOnlySet<string> Paths { get; }
}
