using System.Threading;
using System.Threading.Tasks;
using System;
using Terminal.Core;

namespace ConsoleLib.Showcase.Services;

/// <summary>Owns the interactive terminal session displayed by the showcase.</summary>
public interface IShowcaseTerminalService
{
    event EventHandler<TerminalSnapshot>? SnapshotChanged;

    ITerminalSession? Session { get; }

    TerminalDocument? Document { get; }

    bool IsRunning { get; }

    Task StartAsync(TerminalSize size, CancellationToken cancellationToken = default);

    Task SendInputAsync(string input, CancellationToken cancellationToken = default);

    Task ResizeAsync(TerminalSize size, CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>Runs a short, isolated bridge probe for the gallery command.</summary>
    Task<string> RunProbeAsync(CancellationToken cancellationToken = default);
}
