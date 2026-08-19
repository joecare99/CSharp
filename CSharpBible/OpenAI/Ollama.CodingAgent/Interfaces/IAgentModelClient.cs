using Ollama.CodingAgent.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.Interfaces;

/// <summary>
/// Defines model interactions required by the agent runtime.
/// </summary>
public interface IAgentModelClient
{
    /// <summary>
    /// Requests a model completion for the current conversation.
    /// </summary>
    /// <param name="messages">The current conversation messages.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The completion text.</returns>
    Task<string> CompleteAsync(IReadOnlyList<AgentMessage> messages, CancellationToken cancellationToken = default);
}
