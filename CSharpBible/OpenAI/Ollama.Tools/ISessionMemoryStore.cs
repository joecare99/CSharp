using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.Tools;

/// <summary>
/// Persists and retrieves scoped agent session context.
/// </summary>
public interface ISessionMemoryStore
{
    /// <summary>
    /// Adds a memory entry to a session.
    /// </summary>
    Task RememberAsync(string sessionId, string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most relevant entries for a session and query.
    /// </summary>
    Task<IReadOnlyList<SessionMemoryEntry>> RecallAsync(
        string sessionId,
        string query,
        int maximumResults = 5,
        CancellationToken cancellationToken = default);
}
