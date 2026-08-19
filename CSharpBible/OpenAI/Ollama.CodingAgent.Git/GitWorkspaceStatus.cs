using System.Collections.Generic;
using System.Linq;

namespace Ollama.CodingAgent.Git;

/// <summary>
/// Provides the read-only staged, changed, and conflict state of a worktree.
/// </summary>
public sealed record GitWorkspaceStatus(GitRepositoryInfo Repository, IReadOnlyList<GitFileChange> Changes)
{
    /// <summary>Gets whether at least one path is staged.</summary>
    public bool HasStagedChanges => Changes.Any(change => change.IsStaged);

    /// <summary>Gets whether at least one path is changed in the worktree.</summary>
    public bool HasChangedFiles => Changes.Any(change => change.IsChanged);

    /// <summary>Gets whether the repository needs conflict resolution.</summary>
    public bool HasConflicts => Repository.HasConflicts || Changes.Any(change => change.IsConflicted);
}
