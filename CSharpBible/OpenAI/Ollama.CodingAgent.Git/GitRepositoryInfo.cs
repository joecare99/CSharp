namespace Ollama.CodingAgent.Git;

/// <summary>
/// Identifies a discovered local Git worktree without exposing repository credentials.
/// </summary>
public sealed record GitRepositoryInfo(
    string RootPath,
    string GitDirectoryPath,
    string? CurrentBranch,
    bool IsDetached,
    bool HasConflicts,
    bool IsMergeInProgress);
