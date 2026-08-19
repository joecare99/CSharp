using System.Collections.Generic;

namespace Ollama.CodingAgent.Git;

/// <summary>
/// Provides read-only discovery and inspection of local Git workspaces.
/// </summary>
public interface IGitWorkspaceService
{
    /// <summary>Discovers the repository containing a workspace path.</summary>
    GitRepositoryInfo Discover(string workspacePath);

    /// <summary>Gets staged, changed, and conflict state.</summary>
    GitWorkspaceStatus GetStatus(string workspacePath);

    /// <summary>Gets a length-bounded diff preview.</summary>
    GitDiffPreview GetDiffPreview(string workspacePath, int maximumCharacters);

    /// <summary>Gets all local branches.</summary>
    IReadOnlyList<GitBranchInfo> GetLocalBranches(string workspacePath);

    /// <summary>Gets remotes with sanitized addresses.</summary>
    IReadOnlyList<GitRemoteInfo> GetRemotes(string workspacePath);
}
