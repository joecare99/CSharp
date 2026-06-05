using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Versioning.Models;

namespace AA98_AvlnCodeStudio.Base.Versioning.Services;

/// <summary>
/// Defines provider-neutral version control operations for studio components.
/// </summary>
public interface IVersionControlService
{
    /// <summary>
    /// Gets a repository status snapshot for the requested workspace.
    /// </summary>
    /// <param name="request">The status request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The repository status snapshot.</returns>
    Task<VersionControlStatus> GetStatusAsync(VersionControlStatusRequest request, CancellationToken cancellationToken = default);
}
