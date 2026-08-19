using System.Collections.Generic;

namespace Ollama.CodingAgent.Git;

/// <summary>
/// Base type for a state-changing operation requested for a local Git workspace.
/// </summary>
public abstract record GitWorkspaceOperation;

/// <summary>Stages repository-relative paths.</summary>
public sealed record StageGitPathsOperation(IReadOnlyList<string> Paths) : GitWorkspaceOperation;

/// <summary>Unstages repository-relative paths.</summary>
public sealed record UnstageGitPathsOperation(IReadOnlyList<string> Paths) : GitWorkspaceOperation;

/// <summary>Creates a local branch from the current HEAD.</summary>
public sealed record CreateGitBranchOperation(string BranchName) : GitWorkspaceOperation;

/// <summary>Switches the worktree to an existing local branch.</summary>
public sealed record SwitchGitBranchOperation(string BranchName) : GitWorkspaceOperation;

/// <summary>Creates a local commit from the current index.</summary>
public sealed record CommitGitOperation(string Message, GitIdentity Identity) : GitWorkspaceOperation;

/// <summary>Fetches from a configured remote without supplying credentials.</summary>
public sealed record FetchGitOperation(string RemoteName) : GitWorkspaceOperation;

/// <summary>Pulls the configured upstream using an explicit local identity.</summary>
public sealed record PullGitOperation(GitIdentity Identity) : GitWorkspaceOperation;

/// <summary>Pushes one local branch to a same-named branch on a configured remote.</summary>
public sealed record PushGitOperation(string RemoteName, string BranchName) : GitWorkspaceOperation;
