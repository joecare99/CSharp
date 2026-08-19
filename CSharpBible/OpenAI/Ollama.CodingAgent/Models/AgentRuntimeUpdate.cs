namespace Ollama.CodingAgent.Models;

/// <summary>
/// Identifies a live update emitted by the agent runtime.
/// </summary>
public enum AgentRuntimeUpdateKind
{
    Thinking,
    Tool,
    Workflow,
}

/// <summary>
/// Represents one live runtime update.
/// </summary>
public sealed class AgentRuntimeUpdate
{
    public required AgentRuntimeUpdateKind Kind { get; init; }
    public required string Content { get; init; }
}