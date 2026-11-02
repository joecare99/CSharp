using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AA98_AvlnCodeStudio.Versioning.Svn.Services;

/// <summary>
/// Provides local Subversion working-copy status through the system SVN client.
/// </summary>
public sealed class SvnVersionControlService(ISvnCommandRunner svnCommandRunner) : IVersionControlService
{
    private readonly ISvnCommandRunner _svnCommandRunner = svnCommandRunner
        ?? throw new ArgumentNullException(nameof(svnCommandRunner));

    public async Task<VersionControlStatus> GetStatusAsync(
        VersionControlStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        string workingDirectory = ResolveWorkingDirectory(request);
        VersionControlStatus status = CreateBaseStatus(request, workingDirectory);
        SvnCommandResult infoResult = await _svnCommandRunner.RunAsync(
            workingDirectory,
            ["info", "--xml", "--depth", "empty", "--", workingDirectory],
            cancellationToken).ConfigureAwait(false);
        if (infoResult.ExitCode != 0)
        {
            return status;
        }

        XElement entry = XDocument.Parse(infoResult.StandardOutput)
            .Root?.Element("entry")
            ?? throw new InvalidDataException("SVN info returned XML without a working-copy entry.");
        XElement workingCopyInfo = entry.Element("wc-info")
            ?? throw new InvalidDataException("SVN info returned no working-copy metadata.");
        string workingCopyRoot = (string?)workingCopyInfo.Element("wcroot-abspath")
            ?? throw new InvalidDataException("SVN info returned no working-copy root path.");
        workingCopyRoot = Path.GetFullPath(workingCopyRoot);

        status.RepositoryRootPath = workingCopyRoot;
        status.RepositoryName = Path.GetFileName(workingCopyRoot.TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar));
        status.IsRepositoryRootDiscovered = true;

        string? relativeUrl = (string?)entry.Element("relative-url");
        if (!string.IsNullOrWhiteSpace(relativeUrl))
        {
            string referenceName = relativeUrl.StartsWith("^/", StringComparison.Ordinal)
                ? relativeUrl[2..]
                : relativeUrl;
            if (!string.IsNullOrWhiteSpace(referenceName))
            {
                status.ActiveReferenceName = referenceName;
                status.ActiveReferenceKind = VersionControlReferenceKind.Other;
            }
        }

        if (request.IncludeCapabilities)
        {
            status.Capabilities.Add(VersionControlCapability.InspectStatus);
            status.Capabilities.Add(VersionControlCapability.InspectReferences);
            status.Capabilities.Add(VersionControlCapability.EnumerateChanges);
            status.Capabilities.Add(VersionControlCapability.UpdateWorkingCopy);
        }

        if (!request.IncludeChanges)
        {
            return status;
        }

        SvnCommandResult statusResult = await _svnCommandRunner.RunAsync(
            workingDirectory,
            ["status", "--xml", "--no-ignore", "--depth", "infinity", "--", workingDirectory],
            cancellationToken).ConfigureAwait(false);
        if (statusResult.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"SVN status failed with exit code {statusResult.ExitCode}: {statusResult.StandardError.Trim()}");
        }

        IEnumerable<XElement> entries = XDocument.Parse(statusResult.StandardOutput)
            .Root?.Element("target")
            ?.Elements("entry")
            ?? throw new InvalidDataException("SVN status returned XML without a target entry list.");
        foreach (XElement changeEntry in entries)
        {
            VersionControlChangeSummary? change = ParseChange(changeEntry, workingCopyRoot);
            if (change is not null)
            {
                status.Changes.Add(change);
            }
        }

        status.HasLocalChanges = status.Changes.Any(static change => !change.IsIgnored
            && change.ChangeKind != VersionControlChangeKind.Unknown);
        return status;
    }

    private static VersionControlChangeSummary? ParseChange(XElement entry, string workingCopyRoot)
    {
        XElement? workingCopyStatus = entry.Element("wc-status");
        if (workingCopyStatus is null)
        {
            return null;
        }

        string item = (string?)workingCopyStatus.Attribute("item") ?? "none";
        string? entryPath = (string?)entry.Attribute("path");
        if (string.IsNullOrWhiteSpace(entryPath))
        {
            return null;
        }

        string absolutePath = Path.GetFullPath(Path.IsPathRooted(entryPath)
            ? entryPath
            : Path.Combine(workingCopyRoot, entryPath));
        VersionControlChangeKind changeKind = MapChangeKind(item, workingCopyStatus);
        string? movedFromPath = (string?)workingCopyStatus.Attribute("moved-from");
        return new VersionControlChangeSummary
        {
            Path = Path.GetRelativePath(workingCopyRoot, absolutePath),
            PreviousPath = string.IsNullOrWhiteSpace(movedFromPath)
                ? null
                : Path.GetRelativePath(
                    workingCopyRoot,
                    Path.GetFullPath(Path.IsPathRooted(movedFromPath)
                        ? movedFromPath
                        : Path.Combine(workingCopyRoot, movedFromPath))),
            IsIgnored = string.Equals(item, "ignored", StringComparison.Ordinal),
            ChangeKind = changeKind,
        };
    }

    private static VersionControlChangeKind MapChangeKind(string item, XElement workingCopyStatus)
    {
        if (workingCopyStatus.Attribute("moved-from") is not null
            || workingCopyStatus.Attribute("moved-to") is not null)
        {
            return VersionControlChangeKind.Moved;
        }

        if (string.Equals((string?)workingCopyStatus.Attribute("copied"), "true", StringComparison.OrdinalIgnoreCase))
        {
            return VersionControlChangeKind.Copied;
        }

        return item switch
        {
            "added" or "unversioned" => VersionControlChangeKind.Added,
            "deleted" or "missing" => VersionControlChangeKind.Deleted,
            "modified" or "replaced" or "conflicted" or "obstructed" => VersionControlChangeKind.Modified,
            _ => VersionControlChangeKind.Unknown,
        };
    }

    private static VersionControlStatus CreateBaseStatus(
        VersionControlStatusRequest request,
        string workingDirectory)
    {
        string repositoryRootPath = !string.IsNullOrWhiteSpace(request.RepositoryRootPath)
            ? request.RepositoryRootPath
            : workingDirectory;
        return new VersionControlStatus
        {
            RepositoryRootPath = repositoryRootPath,
            RepositoryName = Path.GetFileName(repositoryRootPath.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar)),
            IsRepositoryRootDiscovered = false,
        };
    }

    private static string ResolveWorkingDirectory(VersionControlStatusRequest request)
    {
        string path = !string.IsNullOrWhiteSpace(request.RepositoryContextPath)
            ? request.RepositoryContextPath
            : request.RepositoryRootPath;
        if (string.IsNullOrWhiteSpace(path))
        {
            path = Directory.GetCurrentDirectory();
        }

        string fullPath = Path.GetFullPath(path);
        return File.Exists(fullPath) ? Path.GetDirectoryName(fullPath)! : fullPath;
    }
}
