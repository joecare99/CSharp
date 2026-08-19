using Ollama.CodingAgent.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.Application.Interfaces;

public interface IStreamingAgentSessionService : IAgentSessionService
{
    Task<AgentRunResult> RunAsync(
        string prompt,
        Action<AgentRuntimeUpdate> onUpdate,
        CancellationToken cancellationToken = default);
}
