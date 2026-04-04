using System;
using System.IO;

namespace GenSecure.Core;

/// <summary>
/// Configures file locations for the encrypted person store.
/// </summary>
public sealed class GenSecureStoreOptions
{
    /// <summary>
    /// Gets or sets the store root directory.
    /// </summary>
    public string? RootDirectory { get; set; }

    /// <summary>
    /// Gets or sets the encrypted data directory name.
    /// </summary>
    public string DataDirectoryName { get; set; } = "data";

    /// <summary>
    /// Gets or sets the wrapped person key directory name.
    /// </summary>
    public string KeyDirectoryName { get; set; } = "keys";

    /// <summary>
    /// Gets or sets the master key directory name.
    /// </summary>
    public string MasterKeyDirectoryName { get; set; } = "master";

    /// <summary>
    /// Gets or sets the local DPAPI-protected master key file name.
    /// </summary>
    public string LocalMasterKeyFileName { get; set; } = "local-master.bin";

    /// <summary>
    /// Gets or sets the recovery key file name.
    /// </summary>
    public string RecoveryKeyFileName { get; set; } = "recovery-key.json";

    /// <summary>
    /// Gets the encrypted data directory path.
    /// </summary>
    public string DataDirectoryPath => Path.Combine(GetValidatedRootDirectory(), DataDirectoryName);

    /// <summary>
    /// Gets the wrapped person key directory path.
    /// </summary>
    public string KeyDirectoryPath => Path.Combine(GetValidatedRootDirectory(), KeyDirectoryName);

    /// <summary>
    /// Gets the master key directory path.
    /// </summary>
    public string MasterKeyDirectoryPath => Path.Combine(GetValidatedRootDirectory(), MasterKeyDirectoryName);

    /// <summary>
    /// Gets the local master key file path.
    /// </summary>
    public string LocalMasterKeyFilePath => Path.Combine(MasterKeyDirectoryPath, LocalMasterKeyFileName);

    /// <summary>
    /// Gets the recovery key file path.
    /// </summary>
    public string RecoveryKeyFilePath => Path.Combine(MasterKeyDirectoryPath, RecoveryKeyFileName);

    /// <summary>
    /// Gets the validated store root directory.
    /// </summary>
    /// <returns>The validated root directory.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the root directory is missing.</exception>
    public string GetValidatedRootDirectory()
    {
        if (string.IsNullOrWhiteSpace(RootDirectory))
        {
            throw new InvalidOperationException("A root directory must be configured for the secure person store.");
        }

        return RootDirectory;
    }
}
