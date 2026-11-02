#if NET10_0
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Versioning.Git.Services;
using AA98_AvlnCodeStudio.Versioning.Svn.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Selects the version-control adapter for the repository containing a workspace.
/// </summary>
public sealed class WorkspaceVersionControlService(
    GitVersionControlService gitVersionControlService,
    SvnVersionControlService svnVersionControlService,
    NullVersionControlService fallbackVersionControlService) : IVersionControlService
{
    private readonly GitVersionControlService _gitVersionControlService = gitVersionControlService
        ?? throw new ArgumentNullException(nameof(gitVersionControlService));
    private readonly SvnVersionControlService _svnVersionControlService = svnVersionControlService
        ?? throw new ArgumentNullException(nameof(svnVersionControlService));
    private readonly NullVersionControlService _fallbackVersionControlService = fallbackVersionControlService
        ?? throw new ArgumentNullException(nameof(fallbackVersionControlService));

    public Task<VersionControlStatus> GetStatusAsync(
        VersionControlStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        IVersionControlService selectedService = FindRepositoryProvider(request) switch
        {
            RepositoryProvider.Git => _gitVersionControlService,
            RepositoryProvider.Svn => _svnVersionControlService,
            _ => _fallbackVersionControlService,
        };
        return selectedService.GetStatusAsync(request, cancellationToken);
    }

    private static RepositoryProvider FindRepositoryProvider(VersionControlStatusRequest request)
    {
        string path = !string.IsNullOrWhiteSpace(request.RepositoryContextPath)
            ? request.RepositoryContextPath
            : request.RepositoryRootPath;
        if (string.IsNullOrWhiteSpace(path))
        {
            path = Directory.GetCurrentDirectory();
        }
        string fullPath = Path.GetFullPath(path);
        string? directory = File.Exists(fullPath) ? Path.GetDirectoryName(fullPath) : fullPath;
        while (!string.IsNullOrWhiteSpace(directory))
        {
            if (Directory.Exists(Path.Combine(directory, ".svn")))
            {
                return RepositoryProvider.Svn;
            }

            if (Directory.Exists(Path.Combine(directory, ".git"))
                || File.Exists(Path.Combine(directory, ".git")))
            {
                return RepositoryProvider.Git;
            }

            string? parent = Path.GetDirectoryName(directory);
            if (string.Equals(parent, directory, OperatingSystem.IsWindows()
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal))
            {
                break;
            }

            directory = parent;
        }

        return RepositoryProvider.None;
    }

    private enum RepositoryProvider
    {
        None,
        Git,
        Svn,
    }
}
#endif
