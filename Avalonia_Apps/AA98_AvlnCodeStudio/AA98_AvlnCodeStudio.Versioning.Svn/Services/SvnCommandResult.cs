namespace AA98_AvlnCodeStudio.Versioning.Svn.Services;

/// <summary>
/// Captures the result of a Subversion CLI command.
/// </summary>
public sealed class SvnCommandResult
{
    public int ExitCode { get; init; }

    public string StandardOutput { get; init; } = string.Empty;

    public string StandardError { get; init; } = string.Empty;
}
