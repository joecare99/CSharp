using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
namespace Ollama.CodingAgent.Application.Models;

/// <summary>
/// Identifies the author of a persisted conversation turn.
/// </summary>
public enum AgentConversationRole
{
    /// <summary>
    /// The user submitted the turn.
    /// </summary>
    User,

    /// <summary>
    /// The agent produced the turn.
    /// </summary>
    Assistant,

    /// <summary>
    /// The application produced a workflow message.
    /// </summary>
    System,
}
