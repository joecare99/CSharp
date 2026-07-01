using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Versioning.Git.Services;

/// <summary>
/// Provides local Git-backed repository inspection for Code Studio.
/// </summary>
public sealed class GitVersionControlService(IGitCommandRunner gitCommandRunner) : IVersionControlService
{
    private readonly IGitCommandRunner _gitCommandRunner = gitCommandRunner;
    private static readonly VersionControlCapability[] s_supportedCapabilities =
    [
        VersionControlCapability.InspectStatus,
        VersionControlCapability.InspectReferences,
        VersionControlCapability.EnumerateChanges,
        VersionControlCapability.DistinguishStagedChanges,
        VersionControlCapability.EvaluateIgnoreRules,
    ];

    /// <inheritdoc/>
    public async Task<VersionControlStatus> GetStatusAsync(VersionControlStatusRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        string workingDirectory = ResolveWorkingDirectory(request);
        VersionControlStatus status = CreateBaseStatus(request, workingDirectory);

        GitCommandResult repositoryRootResult = await _gitCommandRunner
            .RunAsync(workingDirectory, "rev-parse --show-toplevel", cancellationToken)
            .ConfigureAwait(false);

        if (repositoryRootResult.ExitCode != 0)
        {
            return status;
        }

        string repositoryRootPath = NormalizeGitOutput(repositoryRootResult.StandardOutput);
        if (string.IsNullOrWhiteSpace(repositoryRootPath))
        {
            return status;
        }

        status.RepositoryRootPath = repositoryRootPath;
        status.RepositoryName = Path.GetFileName(repositoryRootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        status.IsRepositoryRootDiscovered = true;

        if (request.IncludeCapabilities)
        {
            foreach (VersionControlCapability capability in s_supportedCapabilities)
            {
                status.Capabilities.Add(capability);
            }
        }

        GitCommandResult branchResult = await _gitCommandRunner
            .RunAsync(repositoryRootPath, "branch --show-current", cancellationToken)
            .ConfigureAwait(false);

        string branchName = NormalizeGitOutput(branchResult.StandardOutput);
        if (!string.IsNullOrWhiteSpace(branchName))
        {
            status.ActiveReferenceName = branchName;
            status.ActiveReferenceKind = VersionControlReferenceKind.Branch;
            status.IsDetached = false;
        }
        else
        {
            GitCommandResult detachedHeadResult = await _gitCommandRunner
                .RunAsync(repositoryRootPath, "rev-parse --short HEAD", cancellationToken)
                .ConfigureAwait(false);

            string detachedHead = NormalizeGitOutput(detachedHeadResult.StandardOutput);
            if (!string.IsNullOrWhiteSpace(detachedHead))
            {
                status.ActiveReferenceName = detachedHead;
                status.ActiveReferenceKind = VersionControlReferenceKind.Detached;
                status.IsDetached = true;
            }
        }

        if (!request.IncludeChanges)
        {
            return status;
        }

        GitCommandResult porcelainResult = await _gitCommandRunner
            .RunAsync(repositoryRootPath, "status --porcelain --ignored", cancellationToken)
            .ConfigureAwait(false);

        if (porcelainResult.ExitCode != 0)
        {
            return status;
        }

        foreach (VersionControlChangeSummary change in ParseChanges(porcelainResult.StandardOutput))
        {
            status.Changes.Add(change);
        }

        status.HasLocalChanges = status.Changes.Any(static change => !change.IsIgnored);
        return status;
    }

    public static IReadOnlyList<VersionControlChangeSummary> ParseChanges(string? porcelainOutput)
    {
        List<VersionControlChangeSummary> changes = [];
        if (string.IsNullOrWhiteSpace(porcelainOutput))
        {
            return changes;
        }

        string[] lines = porcelainOutput
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            if (line.Length < 3)
            {
                continue;
            }

            string statusCode = line[..2];
            string pathSegment = line[3..];
            string? previousPath = null;
            string path = pathSegment;

            string[] renameSegments = pathSegment.Split(" -> ", StringSplitOptions.None);
            if (renameSegments.Length == 2)
            {
                previousPath = renameSegments[0];
                path = renameSegments[1];
            }

            VersionControlChangeSummary change = new()
            {
                Path = path,
                PreviousPath = previousPath,
                IsIgnored = statusCode == "!!",
                IsStaged = IsStaged(statusCode),
                ChangeKind = MapChangeKind(statusCode),
            };

            changes.Add(change);
        }

        return changes;
    }

    private static VersionControlStatus CreateBaseStatus(VersionControlStatusRequest request, string workingDirectory)
    {
        string repositoryRootPath = string.IsNullOrWhiteSpace(request.RepositoryRootPath) ? string.Empty : request.RepositoryRootPath;
        if (string.IsNullOrWhiteSpace(repositoryRootPath) && !string.IsNullOrWhiteSpace(request.RepositoryContextPath))
        {
            repositoryRootPath = request.RepositoryContextPath;
        }

        return new VersionControlStatus
        {
            RepositoryRootPath = repositoryRootPath,
            RepositoryName = TryGetRepositoryName(repositoryRootPath),
            IsRepositoryRootDiscovered = !string.IsNullOrWhiteSpace(request.RepositoryRootPath),
            HasLocalChanges = false,
            ActiveReferenceName = null,
            ActiveReferenceKind = VersionControlReferenceKind.Unknown,
            IsDetached = false,
        };
    }

    private static string ResolveWorkingDirectory(VersionControlStatusRequest request)
    {
        string candidatePath = !string.IsNullOrWhiteSpace(request.RepositoryContextPath)
            ? request.RepositoryContextPath
            : request.RepositoryRootPath;

        if (string.IsNullOrWhiteSpace(candidatePath))
        {
            candidatePath = Directory.GetCurrentDirectory();
        }

        return Path.GetFullPath(candidatePath);
    }

    private static string? TryGetRepositoryName(string? repositoryRootPath)
    {
        if (string.IsNullOrWhiteSpace(repositoryRootPath))
        {
            return null;
        }

        string normalizedPath = repositoryRootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Path.GetFileName(normalizedPath);
    }

    private static string NormalizeGitOutput(string? value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private static bool IsStaged(string statusCode)
        => statusCode.Length >= 1 && statusCode[0] != ' ' && statusCode[0] != '?' && statusCode[0] != '!';

    private static VersionControlChangeKind MapChangeKind(string statusCode)
    {
        if (statusCode == "!!")
        {
            return VersionControlChangeKind.Unknown;
        }

        if (statusCode.Contains('R', StringComparison.Ordinal))
        {
            return VersionControlChangeKind.Renamed;
        }

        if (statusCode.Contains('C', StringComparison.Ordinal))
        {
            return VersionControlChangeKind.Copied;
        }

        if (statusCode.Contains('A', StringComparison.Ordinal) || statusCode == "??")
        {
            return VersionControlChangeKind.Added;
        }

        if (statusCode.Contains('D', StringComparison.Ordinal))
        {
            return VersionControlChangeKind.Deleted;
        }

        if (statusCode.Contains('M', StringComparison.Ordinal) || statusCode.Contains('T', StringComparison.Ordinal) || statusCode.Contains('U', StringComparison.Ordinal))
        {
            return VersionControlChangeKind.Modified;
        }

        return VersionControlChangeKind.Unknown;
    }
}
