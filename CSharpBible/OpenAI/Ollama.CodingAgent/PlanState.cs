using System;
using System.Collections.Generic;
using System.Linq;

namespace Ollama.CodingAgent;

/// <summary>
/// Represents one executable plan state for a goal contract.
/// </summary>
public sealed class PlanState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlanState"/> class.
    /// </summary>
    /// <param name="goalContract">The goal contract.</param>
    /// <param name="subtasks">The planned subtasks.</param>
    public PlanState(GoalContract goalContract, IReadOnlyList<PlannedSubtask> subtasks)
    {
        GoalContract = goalContract ?? throw new ArgumentNullException(nameof(goalContract));
        Subtasks = subtasks ?? throw new ArgumentNullException(nameof(subtasks));
        if (Subtasks.Count == 0)
        {
            throw new ArgumentException("At least one subtask is required.", nameof(subtasks));
        }
    }

    /// <summary>
    /// Gets the run goal contract.
    /// </summary>
    public GoalContract GoalContract { get; }

    /// <summary>
    /// Gets the planned subtasks.
    /// </summary>
    public IReadOnlyList<PlannedSubtask> Subtasks { get; }

    /// <summary>
    /// Gets subtasks whose dependencies are complete and which can execute next.
    /// </summary>
    public IReadOnlyList<PlannedSubtask> GetReadySubtasks()
    {
        return Subtasks
            .Where(static subtask => subtask.Status == PlannedSubtaskStatus.Pending)
            .Where(subtask => subtask.Dependencies.All(dependencyId =>
                Subtasks.Any(candidate =>
                    string.Equals(candidate.Id, dependencyId, StringComparison.OrdinalIgnoreCase)
                    && candidate.Status == PlannedSubtaskStatus.Done)))
            .ToArray();
    }
}
