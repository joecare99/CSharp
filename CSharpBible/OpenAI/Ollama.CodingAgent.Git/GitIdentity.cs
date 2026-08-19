namespace Ollama.CodingAgent.Git;

/// <summary>
/// Supplies the author and committer identity for a single local Git operation.
/// </summary>
public sealed record GitIdentity(string Name, string Email);
