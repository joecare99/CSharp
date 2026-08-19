using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.Application.Interfaces;

/// <summary>
/// Persists UI-neutral agent session snapshots.
/// </summary>
public interface IAgentSessionStore
{
    /// <summary>
    /// Saves the supplied session snapshot.
    /// </summary>
    Task SaveAsync(AgentSessionSnapshot snapshot, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the persisted session snapshot.
    /// </summary>
    Task<AgentSessionSnapshot> LoadAsync(CancellationToken cancellationToken = default);
}
