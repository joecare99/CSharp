using System;
using System.Collections.Generic;
using System.Linq;

namespace Ollama.CodingAgent;

/// <summary>
/// Creates a deterministic initial plan for complex coding tasks.
/// </summary>
public static class SubtaskPlanner
{
    /// <summary>
    /// Creates an initial plan state from a user prompt.
    /// </summary>
    /// <param name="userPrompt">The user prompt.</param>
    /// <returns>The plan state.</returns>
    public static PlanState CreateInitialPlan(string userPrompt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userPrompt);

        GoalContract goal = new(
            objective: userPrompt,
            successCriteria:
            [
                "Keep the primary goal visible through all subtasks.",
                "Use delegated tools only when they move the goal forward.",
                "Return a focused implementation-oriented outcome.",
            ]);

        PlannedSubtask executionSubtask = CreateExecutionSubtask(userPrompt, ["collect-context"]);
        List<PlannedSubtask> subtasks =
        [
            new("analyze-goal", "Analyze the main coding objective and constraints.", "Establish goal focus before execution."),
            new("collect-context", "Collect the most relevant code/workspace context.", "Gather only context needed for the target change.", ["analyze-goal"]),
            executionSubtask,
            new("summarize-outcome", "Summarize progress against the primary objective.", "Ensure final answer remains aligned with the goal.", [executionSubtask.Id]),
        ];

        return new PlanState(goal, subtasks);
    }

    private static PlannedSubtask CreateExecutionSubtask(string userPrompt, IReadOnlyList<string> dependencies)
    {
        if (ContainsAny(userPrompt, "test", "unittest", "integration"))
        {
            return new PlannedSubtask("execute-tests", "Execute targeted tests for the scoped task.", "Validate behavioral impact for the intended change.", dependencies);
        }

        if (ContainsAny(userPrompt, "build", "compile"))
        {
            return new PlannedSubtask("execute-build", "Execute targeted build for the scoped task.", "Verify project-level consistency for the intended change.", dependencies);
        }

        return new PlannedSubtask("execute-change", "Execute one focused code change subtask.", "Progress the core implementation with minimal drift.", dependencies);
    }

    private static bool ContainsAny(string input, params string[] terms)
    {
        return terms.Any(term => input.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
    }
}
