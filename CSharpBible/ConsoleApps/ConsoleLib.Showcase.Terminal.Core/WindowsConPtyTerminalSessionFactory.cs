using Terminal.Core;

namespace ConsoleLib.Showcase.Terminal.Core;

/// <summary>Creates showcase-owned Windows ConPTY sessions.</summary>
public sealed class WindowsConPtyTerminalSessionFactory : ITerminalSessionBackendFactory
{
    /// <inheritdoc />
    public string Name => nameof(WindowsConPtyTerminalSessionFactory);

    /// <inheritdoc />
    public bool IsSupported => WindowsConPtyTerminalBridge.IsSupported;

    /// <inheritdoc />
    public ITerminalSession CreateSession() => new WindowsConPtyTerminalBridge();
}
