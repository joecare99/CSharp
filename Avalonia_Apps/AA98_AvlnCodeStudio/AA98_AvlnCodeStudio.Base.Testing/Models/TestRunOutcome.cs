namespace AA98_AvlnCodeStudio.Base.Testing.Models;

/// <summary>
/// Describes the high-level outcome of a test execution.
/// </summary>
public enum TestRunOutcome
{
    /// <summary>
    /// The outcome is not yet known.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The test run passed.
    /// </summary>
    Passed,

    /// <summary>
    /// The test run failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The test run was canceled.
    /// </summary>
    Canceled,

    /// <summary>
    /// The test run completed with mixed results.
    /// </summary>
    Partial
}
