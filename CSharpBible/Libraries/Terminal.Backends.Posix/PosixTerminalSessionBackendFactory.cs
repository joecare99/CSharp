using System;
using Terminal.Core;

namespace Terminal.Backends.Posix;

/// <summary>
/// Provides terminal sessions for Linux and macOS hosts.
/// </summary>
public sealed class PosixTerminalSessionBackendFactory : ITerminalSessionBackendFactory
{
    /// <inheritdoc/>
    public string Name => nameof(PosixTerminalSessionBackendFactory);

    /// <inheritdoc/>
    public bool IsSupported => OperatingSystem.IsLinux() || OperatingSystem.IsMacOS();

    /// <inheritdoc/>
    public ITerminalSession CreateSession()
    {
        return new PosixTerminalSession();
    }
}
