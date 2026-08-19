using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.Application.Interfaces;

/// <summary>
/// Coordinates explicit interactive approvals for state-changing operations.
/// </summary>
public interface IAgentApprovalService
{
    /// <summary>
    /// Gets queued approval requests.
    /// </summary>
    IReadOnlyList<AgentApprovalRequest> PendingRequests { get; }

    /// <summary>
    /// Queues a request and waits for the user's decision.
    /// </summary>
    Task<bool> RequestApprovalAsync(AgentApprovalRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves an existing request.
    /// </summary>
    bool Resolve(string requestId, bool approved);
}
