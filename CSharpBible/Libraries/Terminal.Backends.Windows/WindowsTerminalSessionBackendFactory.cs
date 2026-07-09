using System;
using Terminal.Core;

namespace Terminal.Backends.Windows;

/// <summary>
/// Provides Windows terminal sessions backed by ConPTY.
/// </summary>
public sealed class WindowsTerminalSessionBackendFactory : ITerminalSessionBackendFactory
{
    /// <inheritdoc/>
    public string Name => nameof(WindowsTerminalSessionBackendFactory);

    /// <inheritdoc/>
    public bool IsSupported => OperatingSystem.IsWindows();

    /// <inheritdoc/>
    public ITerminalSession CreateSession()
    {
        return new WindowsConPtyTerminalSession();
    }
}
