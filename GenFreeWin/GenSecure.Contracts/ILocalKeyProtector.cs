namespace GenSecure.Contracts;

/// <summary>
/// Protects and unprotects the local master key material on the current machine.
/// </summary>
public interface ILocalKeyProtector
{
    /// <summary>
    /// Protects the specified plaintext bytes for local storage.
    /// </summary>
    /// <param name="arrPlaintext">The plaintext bytes to protect.</param>
    /// <returns>The protected payload.</returns>
    byte[] Protect(byte[] arrPlaintext);

    /// <summary>
    /// Unprotects the specified locally protected bytes.
    /// </summary>
    /// <param name="arrProtectedData">The protected payload to unprotect.</param>
    /// <returns>The original plaintext bytes.</returns>
    byte[] Unprotect(byte[] arrProtectedData);
}
