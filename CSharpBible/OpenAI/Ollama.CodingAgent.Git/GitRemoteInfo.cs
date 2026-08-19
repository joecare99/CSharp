namespace Ollama.CodingAgent.Git;

/// <summary>
/// Describes a configured remote using a credential-sanitized URL.
/// </summary>
public sealed record GitRemoteInfo(string Name, string FetchUrl, string PushUrl);
