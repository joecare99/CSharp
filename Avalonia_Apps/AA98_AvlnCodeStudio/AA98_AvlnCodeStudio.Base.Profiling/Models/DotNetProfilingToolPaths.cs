using System;

namespace AA98_AvlnCodeStudio.Base.Profiling.Models;

/// <summary>
/// Executable names or paths for the optional diagnostics CLI tools.
/// </summary>
public sealed class DotNetProfilingToolPaths
{
    public DotNetProfilingToolPaths(
        string counters = "dotnet-counters",
        string trace = "dotnet-trace",
        string dump = "dotnet-dump")
    {
        Counters = Validate(counters, nameof(counters));
        Trace = Validate(trace, nameof(trace));
        Dump = Validate(dump, nameof(dump));
    }

    public string Counters { get; }

    public string Trace { get; }

    public string Dump { get; }

    private static string Validate(string value, string parameterName)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("A profiling tool executable is required.", parameterName)
            : value;
    }
}
