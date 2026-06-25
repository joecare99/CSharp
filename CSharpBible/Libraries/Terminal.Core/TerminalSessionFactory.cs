using System;
using System.Collections.Generic;
using System.Linq;

namespace Terminal.Core;

/// <summary>
/// Selects the first supported terminal session backend for the current platform.
/// </summary>
public sealed class TerminalSessionFactory : ITerminalSessionFactory
{
    private readonly IReadOnlyList<ITerminalSessionBackendFactory> _backendFactories;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalSessionFactory"/> class.
    /// </summary>
    public TerminalSessionFactory(IEnumerable<ITerminalSessionBackendFactory> backendFactories)
    {
        _backendFactories = backendFactories?.ToArray() ?? throw new ArgumentNullException(nameof(backendFactories));
    }

    /// <inheritdoc/>
    public ITerminalSession CreateSession()
    {
        var backend = _backendFactories.FirstOrDefault(item => item.IsSupported);
        if (backend is null)
        {
            throw new PlatformNotSupportedException("No supported terminal backend factory is available for the current platform.");
        }

        return backend.CreateSession();
    }
}
