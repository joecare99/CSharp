namespace AA98_AvlnCodeStudio.Base.Testing.Models;

/// <summary>
/// Represents a provider-neutral summary of a test run.
/// </summary>
public sealed class TestRunSummary
{
    /// <summary>
    /// Gets or sets the high-level outcome.
    /// </summary>
    public TestRunOutcome Outcome { get; set; } = TestRunOutcome.Unknown;

    /// <summary>
    /// Gets or sets the total number of discovered or executed tests.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the number of passed tests.
    /// </summary>
    public int PassedCount { get; set; }

    /// <summary>
    /// Gets or sets the number of failed tests.
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// Gets or sets the number of skipped tests.
    /// </summary>
    public int SkippedCount { get; set; }
}
