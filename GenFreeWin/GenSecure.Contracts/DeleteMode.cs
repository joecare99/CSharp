namespace GenSecure.Contracts;

/// <summary>
/// Defines how a stored person record should be removed.
/// </summary>
public enum DeleteMode
{
    /// <summary>
    /// Removes the encrypted data file and the wrapped person key file.
    /// Recovery remains possible through backups or filesystem journaling.
    /// </summary>
    SoftDelete = 0,

    /// <summary>
    /// Removes only the wrapped person key file.
    /// The encrypted payload stays on disk but becomes unreadable.
    /// </summary>
    SecureDelete = 1,
}
