using System.Collections.Generic;

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

    /// <summary>
    /// Gets the structured failures reported by the test run.
    /// </summary>
    public IList<TestRunFailure> Failures { get; } = new List<TestRunFailure>();

    /// <summary>
    /// Gets individual results reported by the provider, if available.
    /// </summary>
    public IList<TestCaseResult> Cases { get; } = new List<TestCaseResult>();

    /// <summary>
    /// Gets provider messages reported during the run.
    /// </summary>
    public IList<string> Messages { get; } = new List<string>();
}
