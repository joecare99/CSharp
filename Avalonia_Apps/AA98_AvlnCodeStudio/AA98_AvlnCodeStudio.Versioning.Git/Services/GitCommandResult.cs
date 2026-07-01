namespace AA98_AvlnCodeStudio.Versioning.Git.Services;

/// <summary>
/// Represents the captured result of a Git command.
/// </summary>
public sealed class GitCommandResult
{
    /// <summary>
    /// Gets or sets the process exit code.
    /// </summary>
    public int ExitCode { get; set; }

    /// <summary>
    /// Gets or sets the captured standard output.
    /// </summary>
    public string StandardOutput { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the captured standard error.
    /// </summary>
    public string StandardError { get; set; } = string.Empty;
}
