using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Planning.Core.Services;

/// <summary>
/// Resolves adapter credentials without exposing storage or provider-specific authentication details.
/// </summary>
public interface IPlanningCredentialService
{
    /// <summary>
    /// Resolves the credential value for an adapter and optional provider scope.
    /// </summary>
    /// <param name="adapterId">The stable adapter identifier.</param>
    /// <param name="scopeId">The optional provider-defined scope identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The credential value, or <see langword="null"/> when none is available.</returns>
    Task<string?> GetCredentialAsync(string adapterId, string? scopeId, CancellationToken cancellationToken = default);
}
