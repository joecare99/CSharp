namespace Ollama.CodingAgent;

/// <summary>
/// Describes the execution status of one planned subtask.
/// </summary>
public enum PlannedSubtaskStatus
{
    /// <summary>
    /// Subtask has not started.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Subtask is currently running.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Subtask completed successfully.
    /// </summary>
    Done = 2,

    /// <summary>
    /// Subtask failed or was blocked.
    /// </summary>
    Blocked = 3,
}
