using System;

namespace Ollama.CodingAgent;

/// <summary>
/// Contains one agent run request.
/// </summary>
public sealed class AgentRunRequest
{
    /// <summary>
    /// Gets or sets the user prompt.
    /// </summary>
    public required string Prompt { get; init; }

    /// <summary>
    /// Gets or sets the system prompt for runtime behavior.
    /// </summary>
    public string SystemPrompt { get; init; } = AgentPromptBuilder.BuildDefaultSystemPrompt();

    /// <summary>
    /// Validates the request.
    /// </summary>
    public void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Prompt);
        ArgumentException.ThrowIfNullOrWhiteSpace(SystemPrompt);
    }
}
