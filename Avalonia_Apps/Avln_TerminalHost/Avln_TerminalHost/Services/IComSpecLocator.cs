namespace Avln_TerminalHost.Services;

/// <summary>
/// Resolves the command interpreter path used by the terminal host.
/// </summary>
public interface IComSpecLocator
{
    /// <summary>
    /// Gets the shell executable path.
    /// </summary>
    /// <returns>The resolved executable path.</returns>
    string GetShellPath();
}
