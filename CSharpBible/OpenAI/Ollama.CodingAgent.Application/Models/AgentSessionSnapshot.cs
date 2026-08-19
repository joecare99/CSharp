using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System.Collections.Generic;

namespace Ollama.CodingAgent.Application.Models;

/// <summary>
/// Contains the persisted, UI-neutral state of one agent session.
/// </summary>
public sealed class AgentSessionSnapshot
{
    /// <summary>
    /// Gets or sets the stable session identifier.
    /// </summary>
    public required string SessionId { get; init; }

    /// <summary>
    /// Gets or sets the selected workspace path.
    /// </summary>
    public required string WorkspacePath { get; init; }

    /// <summary>
    /// Gets or sets the visible conversation history.
    /// </summary>
    public IReadOnlyList<AgentConversationTurn> Conversation { get; init; } = [];
}
