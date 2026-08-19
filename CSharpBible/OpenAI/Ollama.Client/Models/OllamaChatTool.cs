using System.Collections.Generic;

namespace Ollama.Client.Models;

/// <summary>
/// Describes a function tool sent to Ollama.
/// </summary>
public sealed class OllamaChatTool
{
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, OllamaChatToolParameter> Parameters { get; init; } = new Dictionary<string, OllamaChatToolParameter>();
}

/// <summary>
/// Describes one function parameter sent to Ollama.
/// </summary>
public sealed class OllamaChatToolParameter
{
    public string Type { get; init; } = "string";
    public string Description { get; init; } = string.Empty;
    public bool Required { get; init; }
}
