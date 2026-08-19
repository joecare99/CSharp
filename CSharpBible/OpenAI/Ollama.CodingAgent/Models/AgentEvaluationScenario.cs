using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Defines one executable evaluation scenario.
/// </summary>
public sealed class AgentEvaluationScenario
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentEvaluationScenario"/> class.
    /// </summary>
    public AgentEvaluationScenario(
        string id,
        string description,
        Func<CancellationToken, Task<AgentEvaluationScenarioResult>> executeAsync)
    {
        Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("Scenario ID is required.", nameof(id)) : id;
        Description = string.IsNullOrWhiteSpace(description)
            ? throw new ArgumentException("Scenario description is required.", nameof(description))
            : description;
        ExecuteAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
    }

    /// <summary>
    /// Gets the stable scenario identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the scenario description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the executable scenario.
    /// </summary>
    public Func<CancellationToken, Task<AgentEvaluationScenarioResult>> ExecuteAsync { get; }
}
