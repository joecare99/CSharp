namespace Ollama.CodingAgent.Git;

/// <summary>
/// Describes a local branch and its optional upstream branch.
/// </summary>
public sealed record GitBranchInfo(string Name, string TargetSha, bool IsCurrent, string? UpstreamName);
