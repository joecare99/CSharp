namespace GenSecure.Contracts;

/// <summary>
/// Provides the stable identifier of the current principal for secure-store access control.
/// </summary>
public interface ICurrentPrincipalProvider
{
    /// <summary>
    /// Gets the identifier of the current principal.
    /// </summary>
    /// <returns>The current principal identifier.</returns>
    string GetCurrentPrincipalId();
}
