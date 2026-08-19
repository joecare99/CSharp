using LibGit2Sharp;

namespace Ollama.CodingAgent.Git;

/// <summary>
/// Describes the index and working-tree state of one repository-relative path.
/// </summary>
public sealed record GitFileChange(string Path, FileStatus Status)
{
    /// <summary>Gets whether the path has index changes.</summary>
    public bool IsStaged => (Status & (FileStatus.NewInIndex | FileStatus.ModifiedInIndex | FileStatus.DeletedFromIndex | FileStatus.RenamedInIndex | FileStatus.TypeChangeInIndex)) != 0;

    /// <summary>Gets whether the path has working-tree changes.</summary>
    public bool IsChanged => (Status & (FileStatus.NewInWorkdir | FileStatus.ModifiedInWorkdir | FileStatus.DeletedFromWorkdir | FileStatus.RenamedInWorkdir | FileStatus.TypeChangeInWorkdir)) != 0;

    /// <summary>Gets whether Git reports an unresolved conflict for the path.</summary>
    public bool IsConflicted => (Status & FileStatus.Conflicted) != 0;
}
