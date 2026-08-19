namespace Ollama.CodingAgent.Git;

/// <summary>
/// Reports whether an approval-gated Git mutation was applied.
/// </summary>
public sealed record GitOperationResult(
    bool WasApproved,
    bool WasApplied,
    GitOperationPreview Preview,
    string? CommitSha = null,
    string? ErrorMessage = null);
