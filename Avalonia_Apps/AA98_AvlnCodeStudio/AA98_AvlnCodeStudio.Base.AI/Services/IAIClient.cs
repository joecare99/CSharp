using AA98_AvlnCodeStudio.Base.AI.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.AI.Services;

/// <summary>
/// Defines a provider-agnostic AI client for text generation operations.
/// </summary>
public interface IAIClient
{
    /// <summary>
    /// Generates a completion for the supplied provider-agnostic request.
    /// </summary>
    /// <param name="request">The completion request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The completion response.</returns>
    Task<AICompletionResponse> CompleteAsync(AICompletionRequest request, CancellationToken cancellationToken = default);
}