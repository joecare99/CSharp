using AA98_AvlnCodeStudio.Base.AI.Models;

namespace AA98_AvlnCodeStudio.Base.AI.Services;

/// <summary>
/// Creates clients for OpenAI Chat Completions-compatible APIs.
/// </summary>
public sealed class OpenAICompatibleAIClientFactory(OpenAICompatibleAISettings settings) : IAIClientFactory
{
    public IAIClient Create()
    {
        return new OpenAICompatibleAIClient(settings);
    }
}
