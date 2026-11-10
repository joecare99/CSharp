namespace AA98_AvlnCodeStudio.Base.Testing.Models;

/// <summary>
/// Represents a failed test and its optional source location.
/// </summary>
public sealed class TestRunFailure
{
    /// <summary>
    /// Gets or sets the test name reported by the test provider.
    /// </summary>
    public string TestName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the failure message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the failure stack trace.
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the optional source file path.
    /// </summary>
    public string? SourcePath { get; set; }

    /// <summary>
    /// Gets or sets the optional one-based source line number.
    /// </summary>
    public int? LineNumber { get; set; }
}
