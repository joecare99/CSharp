using System.Collections.Generic;
using Terminal.Backends.Posix;
using Terminal.Backends.Windows;
using Terminal.Core;

namespace Terminal.Wpf;

/// <summary>
/// Creates the default terminal session factory for the supported platform backends.
/// </summary>
public static class TerminalBackendCatalog
{
    /// <summary>
    /// Creates the default terminal session factory.
    /// </summary>
    public static ITerminalSessionFactory CreateDefaultFactory()
    {
        IReadOnlyList<ITerminalSessionBackendFactory> backends =
        [
            new WindowsTerminalSessionBackendFactory(),
            new PosixTerminalSessionBackendFactory()
        ];

        return new TerminalSessionFactory(backends);
    }
}
