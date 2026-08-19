using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.Services;
using Ollama.CodingAgent.Application.ViewModels;

namespace Ollama.CodingAgent.Git;

/// <summary>
/// Applies Git mutations only after a complete deterministic preview is approved.
/// </summary>
public sealed class GitOperationExecutor : IGitOperationExecutor
{
    private readonly IAgentApprovalService _approvalService;
    private static Func<Repository, GitWorkspaceOperation, string?> ApplyOperation = Apply;

    /// <summary>
    /// Initializes a new approval-gated executor.
    /// </summary>
    public GitOperationExecutor(IAgentApprovalService approvalService)
    {
        _approvalService = approvalService ?? throw new ArgumentNullException(nameof(approvalService));
    }

    /// <inheritdoc />
    public async Task<GitOperationResult> ExecuteAsync(
        string workspacePath,
        GitWorkspaceOperation operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        cancellationToken.ThrowIfCancellationRequested();

        string repositoryPath = GitWorkspaceService.DiscoverRepositoryPath(workspacePath);
        using Repository repository = new(repositoryPath);
        EnsureRepositoryCanMutate(repository);
        GitOperationPreview preview = CreatePreview(repository, operation);

        AgentApprovalRequest request = new()
        {
            Id = $"git-{Guid.NewGuid():N}",
            Operation = preview.Operation,
            Preview = preview.Render(),
            CreatedAt = DateTimeOffset.UtcNow,
        };

        bool wasApproved = await _approvalService.RequestApprovalAsync(request, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        if (!wasApproved)
        {
            return new GitOperationResult(false, false, preview);
        }

        try
        {
            string? commitSha = ApplyOperation(repository, operation);
            return new GitOperationResult(true, true, preview, commitSha);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return new GitOperationResult(
                WasApproved: true,
                WasApplied: false,
                Preview: preview,
                ErrorMessage: GitDiagnosticRedactor.Redact(exception.Message));
        }
    }

    private static GitOperationPreview CreatePreview(Repository repository, GitWorkspaceOperation operation)
    {
        string workspacePath = repository.Info.WorkingDirectory;
        return operation switch
        {
            StageGitPathsOperation stage => new("Stage", workspacePath, CreatePathParameters(repository, stage.Paths)),
            UnstageGitPathsOperation unstage => new("Unstage", workspacePath, CreatePathParameters(repository, unstage.Paths)),
            CreateGitBranchOperation create => new("Create branch", workspacePath, new Dictionary<string, string>
            {
                ["Branch"] = ValidateBranchName(create.BranchName),
            }),
            SwitchGitBranchOperation @switch => new("Switch branch", workspacePath, new Dictionary<string, string>
            {
                ["Branch"] = ValidateBranchName(@switch.BranchName),
            }),
            CommitGitOperation commit => new("Commit", workspacePath, new Dictionary<string, string>
            {
                ["Author"] = FormatIdentity(ValidateIdentity(commit.Identity)),
                ["Message"] = ValidateCommitMessage(commit.Message),
            }),
            FetchGitOperation fetch => new("Fetch", workspacePath, new Dictionary<string, string>
            {
                ["Remote"] = GetRemote(repository, fetch.RemoteName).Name,
            }),
            PullGitOperation pull => new("Pull", workspacePath, new Dictionary<string, string>
            {
                ["Identity"] = FormatIdentity(ValidateIdentity(pull.Identity)),
            }),
            PushGitOperation push => new("Push", workspacePath, new Dictionary<string, string>
            {
                ["Branch"] = ValidateBranchName(push.BranchName),
                ["Remote"] = GetRemote(repository, push.RemoteName).Name,
            }),
            _ => throw new NotSupportedException($"The Git operation '{operation.GetType().Name}' is not supported."),
        };
    }

    private static string? Apply(Repository repository, GitWorkspaceOperation operation)
    {
        switch (operation)
        {
            case StageGitPathsOperation stage:
                Commands.Stage(repository, GetValidatedPaths(repository, stage.Paths));
                return null;
            case UnstageGitPathsOperation unstage:
                Commands.Unstage(repository, GetValidatedPaths(repository, unstage.Paths));
                return null;
            case CreateGitBranchOperation create:
                repository.CreateBranch(ValidateBranchName(create.BranchName));
                return null;
            case SwitchGitBranchOperation @switch:
                Branch branch = repository.Branches[ValidateBranchName(@switch.BranchName)]
                    ?? throw new InvalidOperationException($"Local branch '{@switch.BranchName}' does not exist.");
                Commands.Checkout(repository, branch);
                return null;
            case CommitGitOperation commit:
                GitIdentity commitIdentity = ValidateIdentity(commit.Identity);
                Signature signature = new(commitIdentity.Name, commitIdentity.Email, DateTimeOffset.UtcNow);
                Commit result = repository.Commit(ValidateCommitMessage(commit.Message), signature, signature);
                return result.Sha;
            case FetchGitOperation fetch:
                Remote fetchRemote = GetRemote(repository, fetch.RemoteName);
                Commands.Fetch(repository, fetchRemote.Name, fetchRemote.FetchRefSpecs.Select(refSpec => refSpec.Specification), null, null);
                return null;
            case PullGitOperation pull:
                GitIdentity pullIdentity = ValidateIdentity(pull.Identity);
                Signature pullSignature = new(pullIdentity.Name, pullIdentity.Email, DateTimeOffset.UtcNow);
                Commands.Pull(repository, pullSignature, new PullOptions());
                return null;
            case PushGitOperation push:
                Remote pushRemote = GetRemote(repository, push.RemoteName);
                string branchName = ValidateBranchName(push.BranchName);
                repository.Network.Push(pushRemote, $"refs/heads/{branchName}:refs/heads/{branchName}");
                return null;
            default:
                throw new NotSupportedException($"The Git operation '{operation.GetType().Name}' is not supported.");
        }
    }

    private static IReadOnlyDictionary<string, string> CreatePathParameters(Repository repository, IReadOnlyList<string> paths)
    {
        string[] validatedPaths = GetValidatedPaths(repository, paths).ToArray();
        return new Dictionary<string, string>
        {
            ["Paths"] = string.Join(", ", validatedPaths),
        };
    }

    private static IEnumerable<string> GetValidatedPaths(Repository repository, IReadOnlyList<string>? paths)
    {
        if (paths is null || paths.Count == 0)
        {
            throw new ArgumentException("At least one repository-relative path is required.", nameof(paths));
        }

        foreach (string path in paths)
        {
            if (string.IsNullOrWhiteSpace(path) || Path.IsPathRooted(path))
            {
                throw new ArgumentException("Each Git path must be a non-empty repository-relative path.", nameof(paths));
            }

            string fullPath = Path.GetFullPath(Path.Combine(repository.Info.WorkingDirectory, path));
            if (!fullPath.StartsWith(repository.Info.WorkingDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"The path '{path}' is outside the repository workspace.", nameof(paths));
            }

            string relativePath = Path.GetRelativePath(repository.Info.WorkingDirectory, fullPath).Replace(Path.DirectorySeparatorChar, '/');
            if (relativePath.Equals(".git", StringComparison.OrdinalIgnoreCase) || relativePath.StartsWith(".git/", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The Git metadata directory cannot be staged or unstaged.", nameof(paths));
            }

            yield return relativePath;
        }
    }

    private static void EnsureRepositoryCanMutate(Repository repository)
    {
        GitRepositoryInfo info = GitWorkspaceService.CreateRepositoryInfo(repository);
        if (info.HasConflicts || info.IsMergeInProgress)
        {
            throw new InvalidOperationException("The repository has unresolved conflicts or a merge/rebase in progress. Resolve it before requesting a Git mutation.");
        }
    }

    private static Remote GetRemote(Repository repository, string remoteName)
    {
        if (string.IsNullOrWhiteSpace(remoteName) || !IsSafeReferenceName(remoteName))
        {
            throw new ArgumentException("The remote name is unsafe or invalid.", nameof(remoteName));
        }

        return repository.Network.Remotes[remoteName]
            ?? throw new InvalidOperationException($"Remote '{remoteName}' is not configured for this repository.");
    }

    private static string ValidateBranchName(string? branchName)
    {
        if (string.IsNullOrWhiteSpace(branchName) || !IsSafeReferenceName(branchName))
        {
            throw new ArgumentException("The branch name is unsafe or invalid.", nameof(branchName));
        }

        return branchName;
    }

    private static bool IsSafeReferenceName(string value)
    {
        return !value.StartsWith("/", StringComparison.Ordinal)
            && !value.StartsWith("refs/", StringComparison.OrdinalIgnoreCase)
            && !value.EndsWith("/", StringComparison.Ordinal)
            && !value.EndsWith(".", StringComparison.Ordinal)
            && !value.Contains("..", StringComparison.Ordinal)
            && !value.Contains("@{", StringComparison.Ordinal)
            && !value.Contains("\\", StringComparison.Ordinal)
            && !value.Any(character => char.IsControl(character) || character is ' ' or '~' or '^' or ':' or '?' or '*' or '[')
            && value.Split('/').All(segment => !string.IsNullOrEmpty(segment) && !segment.EndsWith(".lock", StringComparison.OrdinalIgnoreCase));
    }

    private static GitIdentity ValidateIdentity(GitIdentity? identity)
    {
        if (identity is null || string.IsNullOrWhiteSpace(identity.Name) || string.IsNullOrWhiteSpace(identity.Email) || !identity.Email.Contains('@', StringComparison.Ordinal))
        {
            throw new ArgumentException("A Git identity with a name and email address is required.", nameof(identity));
        }

        return identity;
    }

    private static string ValidateCommitMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("A non-empty commit message is required.", nameof(message));
        }

        return message;
    }

    private static string FormatIdentity(GitIdentity identity) => $"{identity.Name} <{identity.Email}>";
}
