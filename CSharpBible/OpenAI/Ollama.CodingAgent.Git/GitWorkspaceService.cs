using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace Ollama.CodingAgent.Git;

/// <summary>
/// LibGit2Sharp-backed implementation of safe, read-only Git workspace inspection.
/// </summary>
public sealed class GitWorkspaceService : IGitWorkspaceService
{
    /// <inheritdoc />
    public GitRepositoryInfo Discover(string workspacePath)
    {
        string repositoryPath = DiscoverRepositoryPath(workspacePath);
        using Repository repository = new(repositoryPath);
        return CreateRepositoryInfo(repository);
    }

    /// <inheritdoc />
    public GitWorkspaceStatus GetStatus(string workspacePath)
    {
        string repositoryPath = DiscoverRepositoryPath(workspacePath);
        using Repository repository = new(repositoryPath);
        IReadOnlyList<GitFileChange> changes = repository.RetrieveStatus()
            .Select(entry => new GitFileChange(entry.FilePath, entry.State))
            .ToArray();
        return new GitWorkspaceStatus(CreateRepositoryInfo(repository), changes);
    }

    /// <inheritdoc />
    public GitDiffPreview GetDiffPreview(string workspacePath, int maximumCharacters)
    {
        if (maximumCharacters is < 1 or > 100_000)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumCharacters), "The diff preview limit must be from 1 through 100000 characters.");
        }

        string repositoryPath = DiscoverRepositoryPath(workspacePath);
        using Repository repository = new(repositoryPath);
        string stagedPatch = repository.Head.Tip is null
            ? string.Empty
            : repository.Diff.Compare<Patch>(repository.Head.Tip.Tree, DiffTargets.Index).Content;
        string workingTreePatch = repository.Diff.Compare<Patch>().Content;
        string patch = string.Concat(stagedPatch, workingTreePatch);
        int totalCharacterCount = patch.Length;
        bool isTruncated = totalCharacterCount > maximumCharacters;
        string content = isTruncated ? patch[..maximumCharacters] : patch;
        return new GitDiffPreview(content, totalCharacterCount, isTruncated);
    }

    /// <inheritdoc />
    public IReadOnlyList<GitBranchInfo> GetLocalBranches(string workspacePath)
    {
        string repositoryPath = DiscoverRepositoryPath(workspacePath);
        using Repository repository = new(repositoryPath);
        return repository.Branches
            .Where(branch => !branch.IsRemote)
            .OrderBy(branch => branch.FriendlyName, StringComparer.Ordinal)
            .Select(branch => new GitBranchInfo(
                branch.FriendlyName,
                GetTargetSha(branch.Tip),
                branch.IsCurrentRepositoryHead,
                GetUpstreamName(branch.TrackedBranch)))
            .ToArray();
    }

    /// <inheritdoc />
    public IReadOnlyList<GitRemoteInfo> GetRemotes(string workspacePath)
    {
        string repositoryPath = DiscoverRepositoryPath(workspacePath);
        using Repository repository = new(repositoryPath);
        return repository.Network.Remotes
            .OrderBy(remote => remote.Name, StringComparer.Ordinal)
            .Select(remote => new GitRemoteInfo(
                remote.Name,
                SanitizeRemoteUrl(remote.Url),
                SanitizeRemoteUrl(remote.PushUrl)))
            .ToArray();
    }

    internal static string DiscoverRepositoryPath(string workspacePath)
    {
        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            throw new ArgumentException("A workspace path is required.", nameof(workspacePath));
        }

        string fullWorkspacePath;
        try
        {
            fullWorkspacePath = Path.GetFullPath(workspacePath);
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw new ArgumentException("The workspace path is invalid.", nameof(workspacePath), exception);
        }

        if (!Directory.Exists(fullWorkspacePath))
        {
            throw new DirectoryNotFoundException($"The workspace directory '{fullWorkspacePath}' does not exist.");
        }

        string? repositoryPath = Repository.Discover(fullWorkspacePath);
        if (repositoryPath is null)
        {
            throw new InvalidOperationException($"The workspace '{fullWorkspacePath}' is not inside a supported Git repository.");
        }

        using (Repository repository = new(repositoryPath))
        {
            if (repository.Info.IsBare)
            {
                throw new InvalidOperationException($"The workspace '{fullWorkspacePath}' is a bare repository and cannot be used as a Git workspace.");
            }
        }

        return repositoryPath;
    }

    internal static string GetTargetSha(Commit? tip)
        => tip?.Sha ?? string.Empty;

    internal static string? GetUpstreamName(Branch? trackedBranch)
        => trackedBranch?.FriendlyName;

    internal static GitRepositoryInfo CreateRepositoryInfo(Repository repository)
    {
        RepositoryStatus status = repository.RetrieveStatus();
        bool hasConflicts = status.Any(entry => (entry.State & FileStatus.Conflicted) != 0);
        bool isMergeInProgress = repository.Info.CurrentOperation != CurrentOperation.None;
        return new GitRepositoryInfo(
            repository.Info.WorkingDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
            repository.Info.Path,
            repository.Info.IsHeadDetached ? null : repository.Head.FriendlyName,
            repository.Info.IsHeadDetached,
            hasConflicts,
            isMergeInProgress);
    }

    internal static string SanitizeRemoteUrl(string? remoteUrl)
    {
        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            return string.Empty;
        }

        string redactedUrl = GitDiagnosticRedactor.Redact(remoteUrl);
        if (Uri.TryCreate(redactedUrl, UriKind.Absolute, out Uri? uri))
        {
            UriBuilder builder = new(uri)
            {
                UserName = string.Empty,
                Password = string.Empty,
                Query = string.Empty,
                Fragment = string.Empty,
            };
            return builder.Uri.ToString().TrimEnd('/');
        }

        return redactedUrl.TrimEnd('/');
    }
}
