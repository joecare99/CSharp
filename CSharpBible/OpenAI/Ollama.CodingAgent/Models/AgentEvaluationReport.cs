using System;
using System.Collections.Generic;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Summarizes one executable evaluation matrix run.
/// </summary>
public sealed class AgentEvaluationReport
{
    /// <summary>
    /// Gets the scenario outcomes.
    /// </summary>
    public required IReadOnlyList<AgentEvaluationScenarioOutcome> Outcomes { get; init; }

    /// <summary>
    /// Gets the achieved pass rate.
    /// </summary>
    public double PassRate { get; init; }

    /// <summary>
    /// Gets the configured minimum pass rate.
    /// </summary>
    public double MinimumPassRate { get; init; }

    /// <summary>
    /// Gets a value indicating whether the matrix meets the readiness threshold.
    /// </summary>
    public bool Ready => PassRate >= MinimumPassRate;
}

/// <summary>
/// Represents one timed scenario outcome.
/// </summary>
public sealed class AgentEvaluationScenarioOutcome
{
    /// <summary>
    /// Gets the scenario identifier.
    /// </summary>
    public required string ScenarioId { get; init; }

    /// <summary>
    /// Gets a value indicating whether the scenario passed.
    /// </summary>
    public bool Passed { get; init; }

    /// <summary>
    /// Gets the scenario details or failure information.
    /// </summary>
    public required string Details { get; init; }

    /// <summary>
    /// Gets the measured execution duration.
    /// </summary>
    public TimeSpan Duration { get; init; }
}
