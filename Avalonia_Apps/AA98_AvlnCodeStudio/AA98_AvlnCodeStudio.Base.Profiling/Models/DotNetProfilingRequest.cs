namespace AA98_AvlnCodeStudio.Base.Profiling.Models;

/// <summary>
/// Describes a profiling operation against an explicitly selected process.
/// </summary>
/// <param name="ProcessId">The process to profile; this service never launches or selects the target process.</param>
/// <param name="Operation">The diagnostic operation to run.</param>
/// <param name="ArtifactRootDirectory">Destination root for trace and heap-dump artifacts; unused for counters.</param>
public sealed record DotNetProfilingRequest(
    int ProcessId,
    DotNetProfilingOperation Operation,
    string? ArtifactRootDirectory = null);
