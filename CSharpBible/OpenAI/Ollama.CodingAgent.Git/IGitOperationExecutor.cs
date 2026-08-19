using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.Git;

/// <summary>
/// Executes local Git mutations only after an explicit application-level approval.
/// </summary>
public interface IGitOperationExecutor
{
    /// <summary>
    /// Creates the exact preview, requests approval, and applies the approved operation.
    /// </summary>
    Task<GitOperationResult> ExecuteAsync(
        string workspacePath,
        GitWorkspaceOperation operation,
        CancellationToken cancellationToken = default);
}
