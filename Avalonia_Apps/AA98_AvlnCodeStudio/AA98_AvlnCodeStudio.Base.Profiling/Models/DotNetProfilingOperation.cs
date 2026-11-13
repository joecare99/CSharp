namespace AA98_AvlnCodeStudio.Base.Profiling.Models;

/// <summary>
/// An opt-in .NET diagnostic operation against an already-running process.
/// </summary>
public enum DotNetProfilingOperation
{
    Counters,
    Trace,
    HeapDump,
}
