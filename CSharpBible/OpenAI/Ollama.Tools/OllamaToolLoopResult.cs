using System.Collections.Generic;

namespace Ollama.Tools;

/// <summary>
/// Represents the outcome of a bounded multi-turn tool loop.
/// </summary>
public sealed class OllamaToolLoopResult
{
    /// <summary>
    /// Gets the final model response.
    /// </summary>
    public required string FinalResponse { get; init; }

    /// <summary>
    /// Gets the executed tool invocations in order.
    /// </summary>
    public required IReadOnlyList<OllamaToolInvocationResult> Invocations { get; init; }

    /// <summary>
    /// Gets a value indicating whether the model produced a final response.
    /// </summary>
    public bool Completed { get; init; }
}
