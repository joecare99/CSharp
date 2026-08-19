using Ollama.CodingAgent.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.Interfaces;

/// <summary>
/// Extends the agent model client with model reasoning metadata.
/// </summary>
public interface IThinkingAgentModelClient : IAgentModelClient
{
    /// <summary>
    /// Requests a completion while preserving reasoning fragments.
    /// </summary>
    /// <param name="messages">The current conversation messages.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The completion and optional reasoning fragments.</returns>
    Task<AgentCompletion> CompleteDetailedAsync(
        IReadOnlyList<AgentMessage> messages,
        CancellationToken cancellationToken = default);
}
