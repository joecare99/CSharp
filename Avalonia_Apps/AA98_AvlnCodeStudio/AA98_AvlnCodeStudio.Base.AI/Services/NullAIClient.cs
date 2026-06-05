using AA98_AvlnCodeStudio.Base.AI.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.AI.Services;

/// <summary>
/// Provides a provider-neutral fallback AI client without external provider integration.
/// </summary>
public sealed class NullAIClient : IAIClient
{
    /// <inheritdoc/>
    public Task<AICompletionResponse> CompleteAsync(AICompletionRequest request, CancellationToken cancellationToken = default)
    {
        var response = new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, string.Empty));
        return Task.FromResult(response);
    }
}
