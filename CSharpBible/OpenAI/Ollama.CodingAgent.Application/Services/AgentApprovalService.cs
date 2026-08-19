using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.Application.Services;

/// <summary>
/// In-memory approval queue shared by interactive client adapters.
/// </summary>
public sealed class AgentApprovalService : IAgentApprovalService
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<string, TaskCompletionSource<bool>> _pendingDecisions = new(StringComparer.Ordinal);
    private readonly List<AgentApprovalRequest> _pendingRequests = [];

    /// <inheritdoc />
    public IReadOnlyList<AgentApprovalRequest> PendingRequests
    {
        get
        {
            lock (_syncRoot)
            {
                return _pendingRequests.ToArray();
            }
        }
    }

    /// <inheritdoc />
    public async Task<bool> RequestApprovalAsync(AgentApprovalRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Operation);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Preview);

        TaskCompletionSource<bool> decision = new(TaskCreationOptions.RunContinuationsAsynchronously);
        lock (_syncRoot)
        {
            if (!_pendingDecisions.TryAdd(request.Id, decision))
            {
                throw new InvalidOperationException($"Approval request '{request.Id}' already exists.");
            }

            _pendingRequests.Add(request);
        }

        using CancellationTokenRegistration cancellationRegistration = cancellationToken.Register(() => Resolve(request.Id, approved: false));
        return await decision.Task.ConfigureAwait(false);
    }

    /// <inheritdoc />
    public bool Resolve(string requestId, bool approved)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestId);

        TaskCompletionSource<bool>? decision;
        lock (_syncRoot)
        {
            if (!_pendingDecisions.Remove(requestId, out decision))
            {
                return false;
            }

            _pendingRequests.RemoveAll(request => string.Equals(request.Id, requestId, StringComparison.Ordinal));
        }

        return decision.TrySetResult(approved);
    }
}
