namespace Ollama.CodingAgent;

/// <summary>
/// Defines the named HTTP clients used by the coding-agent runtime.
/// </summary>
public static class OllamaHttpClientNames
{
    /// <summary>
    /// The client used for Ollama provider traffic, including long-running streaming chat.
    /// Retries transient failures; total and per-attempt timeouts are disabled because
    /// <see cref="AgentRunner"/> owns step timeout control.
    /// </summary>
    public const string Agent = "ollama-agent";

    /// <summary>
    /// The client used for bounded web lookups with a short total timeout and standard resilience.
    /// </summary>
    public const string WebLookup = "web-lookup";
}
