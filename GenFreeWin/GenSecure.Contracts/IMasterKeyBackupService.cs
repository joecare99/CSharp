namespace GenSecure.Contracts;

/// <summary>
/// Creates and restores recovery material for the locally protected master key.
/// </summary>
public interface IMasterKeyBackupService
{
    /// <summary>
    /// Creates or updates the recovery key backup using a passphrase.
    /// </summary>
    /// <param name="sPassphrase">The passphrase that protects the recovery material.</param>
    /// <param name="xOverwrite">
    /// <see langword="true"/> to replace an existing recovery file; otherwise <see langword="false"/>.
    /// </param>
    void CreateRecoveryKeyBackup(string sPassphrase, bool xOverwrite = false);

    /// <summary>
    /// Restores the local master key from the recovery material.
    /// </summary>
    /// <param name="sPassphrase">The passphrase used when the recovery backup was created.</param>
    /// <returns><see langword="true"/> if the restore succeeded; otherwise <see langword="false"/>.</returns>
    bool TryRestoreLocalMasterKey(string sPassphrase);
}
