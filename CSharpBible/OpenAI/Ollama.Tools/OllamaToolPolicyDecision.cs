namespace Ollama.Tools;

/// <summary>
/// Describes whether a tool invocation is permitted.
/// </summary>
public sealed class OllamaToolPolicyDecision
{
    /// <summary>
    /// Gets a value indicating whether execution is permitted.
    /// </summary>
    public bool IsAllowed { get; init; }

    /// <summary>
    /// Gets the reason when execution is denied.
    /// </summary>
    public string? Reason { get; init; }
}
