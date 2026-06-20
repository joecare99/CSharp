namespace Workbench.Builder.Cli;

/// <summary>
/// Defines process exit codes for builder command-line hosts.
/// </summary>
public static class HostExitCodes
{
    /// <summary>
    /// Indicates a successful run.
    /// </summary>
    public const int Success = 0;

    /// <summary>
    /// Indicates invalid command-line usage.
    /// </summary>
    public const int InvalidArguments = 1;

    /// <summary>
    /// Indicates an unhandled host execution failure.
    /// </summary>
    public const int ExecutionFailed = 2;
}
