namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Runtime settings for an OpenAI-compatible chat-completions endpoint.
/// </summary>
public sealed class OpenAICompatibleAISettings
{
    public string Endpoint { get; set; } = "http://localhost:11434/v1/chat/completions";

    public string ModelId { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
}
