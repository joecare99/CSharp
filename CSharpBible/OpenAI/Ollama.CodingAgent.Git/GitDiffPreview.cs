namespace Ollama.CodingAgent.Git;

/// <summary>
/// Contains a length-bounded, read-only textual Git diff preview.
/// </summary>
public sealed record GitDiffPreview(string Content, int TotalCharacterCount, bool IsTruncated);
