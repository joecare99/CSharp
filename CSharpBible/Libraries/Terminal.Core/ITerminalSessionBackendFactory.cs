namespace Terminal.Core;

/// <summary>
/// Defines a platform-specific terminal session backend factory.
/// </summary>
public interface ITerminalSessionBackendFactory
{
    /// <summary>
    /// Gets the backend display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the backend is supported on the current platform.
    /// </summary>
    bool IsSupported { get; }

    /// <summary>
    /// Creates a new terminal session instance.
    /// </summary>
    ITerminalSession CreateSession();
}
