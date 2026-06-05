using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Versioning.Models;

namespace AA98_AvlnCodeStudio.Base.Versioning.Services;

/// <summary>
/// Provides a provider-neutral fallback version control service without repository integration.
/// </summary>
public sealed class NullVersionControlService : IVersionControlService
{
    /// <inheritdoc/>
    public Task<VersionControlStatus> GetStatusAsync(VersionControlStatusRequest request, CancellationToken cancellationToken = default)
    {
        var status = new VersionControlStatus
        {
            RepositoryRootPath = request.RepositoryRootPath,
            HasLocalChanges = false,
        };

        return Task.FromResult(status);
    }
}
