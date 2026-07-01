namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Identifies the normalized planning item status.
/// </summary>
public enum PlanningItemStatus
{
    /// <summary>
    /// The planning item status is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The planning item is proposed.
    /// </summary>
    Proposed = 1,

    /// <summary>
    /// The planning item is active or in progress.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// The planning item is blocked.
    /// </summary>
    Blocked = 3,

    /// <summary>
    /// The planning item is completed.
    /// </summary>
    Completed = 4,

    /// <summary>
    /// The planning item is cancelled.
    /// </summary>
    Cancelled = 5,
}

