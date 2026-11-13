namespace AA98_AvlnCodeStudio.Base.Profiling.Models;

/// <summary>
/// The outcome of a completed profiling process.
/// </summary>
public sealed record DotNetProfilingResult(
    int ProcessId,
    DotNetProfilingOperation Operation,
    int ExitCode,
    string? ArtifactPath,
    string StandardOutput,
    string StandardError,
    bool WasStopped,
    bool StandardOutputTruncated,
    bool StandardErrorTruncated)
{
    /// <summary>
    /// Gets whether the operation completed normally and produced any required artifact.
    /// </summary>
    public bool Succeeded => !WasStopped && ExitCode == 0;
}
