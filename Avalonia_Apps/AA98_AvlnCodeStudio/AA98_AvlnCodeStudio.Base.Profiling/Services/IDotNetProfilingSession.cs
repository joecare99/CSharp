using AA98_AvlnCodeStudio.Base.Profiling.Models;
using System;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Profiling.Services;

/// <summary>
/// Represents the lifetime of a .NET profiling tool process.
/// </summary>
public interface IDotNetProfilingSession : IAsyncDisposable
{
    int ProcessId { get; }

    DotNetProfilingOperation Operation { get; }

    string? ArtifactPath { get; }

    /// <summary>
    /// Gets the final result. Retained stdout and stderr are bounded; truncation is
    /// indicated by the result flags and an annotation in the corresponding text.
    /// </summary>
    Task<DotNetProfilingResult> Completion { get; }

    event EventHandler<DotNetProfilingOutputEventArgs>? OutputReceived;

    /// <summary>
    /// Force-stops the profiling tool and waits for its process and redirected output to finish.
    /// This confirms only that the tool stopped; <see cref="Completion"/> reports
    /// <c>WasStopped</c> and <c>Succeeded</c> false, and does not expose a trace/dump
    /// artifact as complete.
    /// </summary>
    Task StopAsync();
}
