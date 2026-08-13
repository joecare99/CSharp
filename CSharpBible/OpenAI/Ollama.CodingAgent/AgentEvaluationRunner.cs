using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent;

/// <summary>
/// Executes a versioned set of deterministic evaluation scenarios.
/// </summary>
public sealed class AgentEvaluationRunner
{
    /// <summary>
    /// Executes all scenarios and evaluates the configured readiness threshold.
    /// </summary>
    public async Task<AgentEvaluationReport> RunAsync(
        IEnumerable<AgentEvaluationScenario> scenarios,
        double minimumPassRate = 1.0,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(scenarios);
        if (minimumPassRate is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumPassRate));
        }

        AgentEvaluationScenario[] scenarioArray = scenarios.ToArray();
        if (scenarioArray.Length == 0)
        {
            throw new ArgumentException("At least one evaluation scenario is required.", nameof(scenarios));
        }

        List<AgentEvaluationScenarioOutcome> outcomes = [];
        foreach (AgentEvaluationScenario scenario in scenarioArray)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                AgentEvaluationScenarioResult result = await scenario.ExecuteAsync(cancellationToken);
                stopwatch.Stop();
                outcomes.Add(new AgentEvaluationScenarioOutcome
                {
                    ScenarioId = scenario.Id,
                    Passed = result.Passed,
                    Details = result.Details,
                    Duration = stopwatch.Elapsed,
                });
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                stopwatch.Stop();
                outcomes.Add(new AgentEvaluationScenarioOutcome
                {
                    ScenarioId = scenario.Id,
                    Passed = false,
                    Details = $"{exception.GetType().Name}: {exception.Message}",
                    Duration = stopwatch.Elapsed,
                });
            }
        }

        int passed = outcomes.Count(static outcome => outcome.Passed);
        return new AgentEvaluationReport
        {
            Outcomes = outcomes,
            PassRate = (double)passed / outcomes.Count,
            MinimumPassRate = minimumPassRate,
        };
    }
}
