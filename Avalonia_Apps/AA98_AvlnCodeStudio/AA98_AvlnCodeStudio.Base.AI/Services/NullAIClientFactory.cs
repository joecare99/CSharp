namespace AA98_AvlnCodeStudio.Base.AI.Services;

/// <summary>
/// Creates fallback AI client instances without provider integration.
/// </summary>
public sealed class NullAIClientFactory : IAIClientFactory
{
    /// <inheritdoc/>
    public IAIClient Create()
    {
        return new NullAIClient();
    }
}
