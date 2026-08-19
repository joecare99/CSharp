namespace Ollama.CodingAgent.Models;

/// <summary>
/// Contains the outcome reported by one evaluation scenario.
/// </summary>
public sealed class AgentEvaluationScenarioResult
{
    /// <summary>
    /// Gets a value indicating whether the scenario passed.
    /// </summary>
    public bool Passed { get; init; }

    /// <summary>
    /// Gets the scenario result detail.
    /// </summary>
    public string Details { get; init; } = string.Empty;
}
