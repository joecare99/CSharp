using System.Text;

namespace Ollama.CodingAgent;

/// <summary>
/// Renders plan state summaries for diagnostics and final reporting.
/// </summary>
public static class PlanStateRenderer
{
    /// <summary>
    /// Renders a compact plan summary.
    /// </summary>
    /// <param name="planState">The plan state.</param>
    /// <returns>The summary text.</returns>
    public static string Render(PlanState planState)
    {
        StringBuilder builder = new();
        builder.AppendLine("Goal contract:");
        builder.AppendLine(planState.GoalContract.Objective);
        builder.AppendLine();
        builder.AppendLine("Subtasks:");
        foreach (PlannedSubtask subtask in planState.Subtasks)
        {
            builder.Append("- ");
            builder.Append(subtask.Id);
            builder.Append(" | ");
            builder.Append(subtask.Status);
            builder.Append(" | ");
            builder.AppendLine(subtask.Title);
        }

        return builder.ToString().TrimEnd();
    }
}
