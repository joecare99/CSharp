using AA98_AvlnCodeStudio.Base.Versioning.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Versioning.Services;

/// <summary>
/// Provides a provider-neutral fallback version control service without repository integration.
/// </summary>
public sealed class NullVersionControlService : IVersionControlService
{
    /// <inheritdoc/>
    public Task<VersionControlStatus> GetStatusAsync(VersionControlStatusRequest request, CancellationToken cancellationToken = default)
    {
        string repositoryRootPath = string.IsNullOrWhiteSpace(request.RepositoryRootPath)
            ? request.RepositoryContextPath ?? string.Empty
            : request.RepositoryRootPath;

        var status = new VersionControlStatus
        {
            RepositoryRootPath = repositoryRootPath,
            RepositoryName = TryGetRepositoryName(repositoryRootPath),
            IsRepositoryRootDiscovered = !string.IsNullOrWhiteSpace(request.RepositoryRootPath),
            HasLocalChanges = false,
        };

        if (request.IncludeCapabilities)
        {
            status.Capabilities.Add(VersionControlCapability.InspectStatus);
        }

        return Task.FromResult(status);
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
}
