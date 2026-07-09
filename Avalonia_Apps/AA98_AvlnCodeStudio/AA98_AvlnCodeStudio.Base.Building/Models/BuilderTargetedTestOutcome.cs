namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents the summarized outcome of a targeted test run.
/// </summary>
public enum BuilderTargetedTestOutcome
{
    /// <summary>
    /// The outcome is not known.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The targeted test run succeeded.
    /// </summary>
    Passed = 1,

    /// <summary>
    /// One or more targeted tests failed.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// The targeted test run was canceled.
    /// </summary>
    Canceled = 3,

    /// <summary>
    /// The targeted test run completed with mixed results.
    /// </summary>
    Partial = 4,

    /// <summary>
    /// The targeted test run was skipped or not executed.
    /// </summary>
    Skipped = 5,
}
