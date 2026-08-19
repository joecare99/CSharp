using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.Application.Interfaces;

/// <summary>
/// Runs one prompt through the configured agent runtime.
/// </summary>
public interface IAgentSessionService
{
    /// <summary>
    /// Runs a prompt and returns the visible agent result.
    /// </summary>
    Task<AgentRunResult> RunAsync(string prompt, CancellationToken cancellationToken = default);
}
