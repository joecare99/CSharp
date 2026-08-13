using Ollama.Tools.Abstractions;

namespace Ollama.Tools;

/// <summary>
/// Applies host-controlled policy decisions before tool execution.
/// </summary>
public interface IOllamaToolExecutionPolicy
{
    /// <summary>
    /// Evaluates whether the registered tool may execute.
    /// </summary>
    OllamaToolPolicyDecision Evaluate(IOllamaTool tool);
}
