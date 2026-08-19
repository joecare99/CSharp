using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace Ollama.CodingAgent.Git.Tests;

/// <summary>
/// Creates a disposable local-only repository for Git provider tests.
/// </summary>
internal sealed class GitTestRepository : IDisposable
{
    private readonly List<string> _pathsToDelete;

    private GitTestRepository(string workspacePath, params string[] additionalPaths)
    {
        WorkspacePath = workspacePath;
        _pathsToDelete = [workspacePath, .. additionalPaths];
    }

    public string WorkspacePath { get; }

    public string? BareRemotePath { get; private init; }

    public static GitTestRepository Create()
    {
        string workspacePath = Path.Combine(AppContext.BaseDirectory, "TestWorkspaces", $"coding-agent-git-{Guid.NewGuid():N}");
        Directory.CreateDirectory(workspacePath);
        Repository.Init(workspacePath);
        File.WriteAllText(Path.Combine(workspacePath, "readme.txt"), "initial");
        using Repository repository = new(workspacePath);
        Commands.Stage(repository, "readme.txt");
        Signature signature = new("Test User", "test@example.invalid", DateTimeOffset.UtcNow);
        repository.Commit("Initial commit", signature, signature);
        Directory.CreateDirectory(Path.Combine(workspacePath, "nested"));
        return new GitTestRepository(workspacePath);
    }

    public static GitTestRepository CreateWithBareRemote()
    {
        GitTestRepository repository = Create();
        string remotePath = Path.Combine(AppContext.BaseDirectory, "TestWorkspaces", $"coding-agent-git-remote-{Guid.NewGuid():N}.git");
        string divergentWorkspacePath = Path.Combine(AppContext.BaseDirectory, "TestWorkspaces", $"coding-agent-git-divergent-{Guid.NewGuid():N}");
        Repository.Init(remotePath, isBare: true);

        using (Repository localRepository = new(repository.WorkspacePath))
        {
            Remote remote = localRepository.Network.Remotes.Add("origin", remotePath);
            string branchName = localRepository.Head.FriendlyName;
            localRepository.Network.Push(remote, $"refs/heads/{branchName}:refs/heads/{branchName}");
        }

        Repository.Clone(remotePath, divergentWorkspacePath);
        return new GitTestRepository(repository.WorkspacePath, remotePath, divergentWorkspacePath)
        {
            BareRemotePath = remotePath,
        };
    }

    public void CreateRemoteDivergence()
    {
        if (BareRemotePath is null)
        {
            throw new InvalidOperationException("A bare remote is required to create divergence.");
        }

        string divergentWorkspacePath = _pathsToDelete.Single(path => path.Contains("coding-agent-git-divergent-", StringComparison.Ordinal));
        using Repository divergentRepository = new(divergentWorkspacePath);
        File.WriteAllText(Path.Combine(divergentWorkspacePath, "remote.txt"), "remote change");
        Commands.Stage(divergentRepository, "remote.txt");
        Signature signature = new("Remote User", "remote@example.invalid", DateTimeOffset.UtcNow);
        divergentRepository.Commit("Remote commit", signature, signature);
        divergentRepository.Network.Push(
            divergentRepository.Network.Remotes["origin"],
            $"refs/heads/{divergentRepository.Head.FriendlyName}:refs/heads/{divergentRepository.Head.FriendlyName}");
    }

    public void Dispose()
    {
        foreach (string path in _pathsToDelete.OrderByDescending(path => path.Length))
        {
            try
            {
                Directory.Delete(path, recursive: true);
            }
            catch (UnauthorizedAccessException)
            {
                // LibGit2Sharp can hold loose-object handles until the test host exits.
            }
        }
    }
}
