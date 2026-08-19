namespace Ollama.CodingAgent.Models;

/// <summary>
/// Describes provider capabilities exposed to the agent runtime.
/// </summary>
public sealed class AgentProviderCapabilities
{
    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public required string ProviderName { get; init; }

    /// <summary>
    /// Gets the model identifier.
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// Gets a value indicating whether the provider supports streaming responses.
    /// </summary>
    public bool SupportsStreaming { get; init; }

    /// <summary>
    /// Gets a value indicating whether the provider exposes native tool calls.
    /// </summary>
    public bool SupportsToolCalls { get; init; }

    /// <summary>
    /// Gets a value indicating whether the provider exposes reasoning fragments.
    /// </summary>
    public bool SupportsThinking { get; init; }
}
