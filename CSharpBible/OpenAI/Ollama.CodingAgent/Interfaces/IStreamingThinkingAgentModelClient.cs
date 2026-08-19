using Ollama.CodingAgent.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.Interfaces;

/// <summary>
/// Provides reasoning completions while forwarding streamed thinking fragments.
/// </summary>
public interface IStreamingThinkingAgentModelClient : IThinkingAgentModelClient
{
    /// <summary>
    /// Requests a completion and reports thinking fragments as they arrive.
    /// </summary>
    Task<AgentCompletion> CompleteDetailedAsync(
        IReadOnlyList<AgentMessage> messages,
        Action<string> onThinkingFragment,
        CancellationToken cancellationToken = default);
}