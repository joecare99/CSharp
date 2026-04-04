using System;
using System.IO;
using System.Security.Cryptography;
using GenSecure.Contracts;

namespace GenSecure.Core;

/// <summary>
/// Creates and restores recovery material for the local master key.
/// </summary>
public sealed class MasterKeyBackupService : IMasterKeyBackupService
{
    private const int MasterKeySize = 32;
    private const int RecoveryKeySize = 32;
    private const int RecoveryIterationCount = 600000;
    private readonly GenSecureStoreOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MasterKeyBackupService"/> class.
    /// </summary>
    /// <param name="options">The store options.</param>
    public MasterKeyBackupService(GenSecureStoreOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void CreateRecoveryKeyBackup(string sPassphrase, bool xOverwrite = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPassphrase);

        string sRecoveryKeyFilePath = _options.RecoveryKeyFilePath;
        if (File.Exists(sRecoveryKeyFilePath) && !xOverwrite)
        {
            throw new InvalidOperationException($"The recovery key file '{sRecoveryKeyFilePath}' already exists.");
        }

        byte[] arrMasterKey = LoadOrCreateMasterKey();
        byte[] arrSalt = RandomNumberGenerator.GetBytes(16);
        byte[] arrRecoveryKey = Rfc2898DeriveBytes.Pbkdf2(sPassphrase, arrSalt, RecoveryIterationCount, HashAlgorithmName.SHA256, RecoveryKeySize);
        byte[] arrCiphertext = CryptoUtilities.Encrypt(arrRecoveryKey, arrMasterKey, out byte[] arrNonce, out byte[] arrTag);

        var record = new RecoveryKeyRecord
        {
            Algorithm = "PBKDF2-SHA256+A256GCM",
            Salt = CryptoUtilities.ToBase64(arrSalt),
            IterationCount = RecoveryIterationCount,
            Nonce = CryptoUtilities.ToBase64(arrNonce),
            Ciphertext = CryptoUtilities.ToBase64(arrCiphertext),
            Tag = CryptoUtilities.ToBase64(arrTag),
            CreatedUtc = DateTimeOffset.UtcNow,
        };

        Directory.CreateDirectory(_options.MasterKeyDirectoryPath);
        CryptoUtilities.WriteJson(sRecoveryKeyFilePath, record);
    }

    /// <inheritdoc />
    public bool TryRestoreLocalMasterKey(string sPassphrase)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPassphrase);

        string sRecoveryKeyFilePath = _options.RecoveryKeyFilePath;
        if (!File.Exists(sRecoveryKeyFilePath))
        {
            return false;
        }

        RecoveryKeyRecord record = CryptoUtilities.ReadJson<RecoveryKeyRecord>(sRecoveryKeyFilePath);
        byte[] arrSalt = CryptoUtilities.FromBase64(record.Salt);
        byte[] arrRecoveryKey = Rfc2898DeriveBytes.Pbkdf2(sPassphrase, arrSalt, record.IterationCount, HashAlgorithmName.SHA256, RecoveryKeySize);
        byte[] arrMasterKey = CryptoUtilities.Decrypt(
            arrRecoveryKey,
            CryptoUtilities.FromBase64(record.Nonce),
            CryptoUtilities.FromBase64(record.Ciphertext),
            CryptoUtilities.FromBase64(record.Tag));

        PersistLocalMasterKey(arrMasterKey);
        return true;
    }

    internal byte[] LoadOrCreateMasterKey()
    {
        string sLocalMasterKeyFilePath = _options.LocalMasterKeyFilePath;
        if (File.Exists(sLocalMasterKeyFilePath))
        {
            byte[] arrProtectedMasterKey = File.ReadAllBytes(sLocalMasterKeyFilePath);
            return ProtectedData.Unprotect(arrProtectedMasterKey, optionalEntropy: null, DataProtectionScope.CurrentUser);
        }

        byte[] arrMasterKey = RandomNumberGenerator.GetBytes(MasterKeySize);
        PersistLocalMasterKey(arrMasterKey);
        return arrMasterKey;
    }

    internal void PersistLocalMasterKey(byte[] arrMasterKey)
    {
        ArgumentNullException.ThrowIfNull(arrMasterKey);

        Directory.CreateDirectory(_options.MasterKeyDirectoryPath);
        byte[] arrProtectedMasterKey = ProtectedData.Protect(arrMasterKey, optionalEntropy: null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(_options.LocalMasterKeyFilePath, arrProtectedMasterKey);
    }
}
