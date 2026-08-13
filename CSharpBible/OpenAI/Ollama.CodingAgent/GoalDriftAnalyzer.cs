using System;

namespace Ollama.CodingAgent;

/// <summary>
/// Evaluates whether a subtask result appears to drift away from the primary objective.
/// </summary>
public static class GoalDriftAnalyzer
{
    /// <summary>
    /// Returns true when the subtask outcome appears to drift from the goal.
    /// </summary>
    /// <param name="goal">The goal contract.</param>
    /// <param name="subtask">The planned subtask.</param>
    /// <param name="toolOutput">The tool output text.</param>
    /// <returns>True when drift is detected.</returns>
    public static bool IsDriftDetected(GoalContract goal, PlannedSubtask subtask, string toolOutput)
    {
        ArgumentNullException.ThrowIfNull(goal);
        ArgumentNullException.ThrowIfNull(subtask);

        string objective = goal.Objective ?? string.Empty;
        string output = toolOutput ?? string.Empty;
        if (output.Length == 0)
        {
            return true;
        }

        // Lightweight deterministic drift signal: execution failed or the output is generic/noisy
        // while not sharing key objective terms.
        bool hasGenericFailure = output.Contains("operation was canceled", StringComparison.OrdinalIgnoreCase)
            || output.Contains("failed", StringComparison.OrdinalIgnoreCase);
        bool sharesObjectiveHint = SharesKeyword(objective, output);

        return hasGenericFailure && !sharesObjectiveHint;
    }

    private static bool SharesKeyword(string objective, string output)
    {
        string[] tokens = objective.Split([' ', ',', '.', ';', ':', '(', ')', '-', '_', '/', '\\'], StringSplitOptions.RemoveEmptyEntries);
        foreach (string token in tokens)
        {
            if (token.Length < 4)
            {
                continue;
            }

            if (output.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return false;
    }
}
