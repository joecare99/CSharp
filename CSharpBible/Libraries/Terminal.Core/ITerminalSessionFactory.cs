namespace Terminal.Core;

/// <summary>
/// Defines a factory that resolves the active terminal session backend for the current platform.
/// </summary>
public interface ITerminalSessionFactory
{
    /// <summary>
    /// Creates a terminal session for the current platform.
    /// </summary>
    ITerminalSession CreateSession();
}
