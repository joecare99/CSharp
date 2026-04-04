using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using GenSecure.Contracts;

namespace GenSecure.Core;

/// <summary>
/// Stores person records as encrypted JSON files with per-person keys.
/// </summary>
public sealed class PersonSecureStore : IPersonSecureStore
{
    private readonly MasterKeyBackupService _masterKeyBackupService;
    private readonly GenSecureStoreOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonSecureStore"/> class.
    /// </summary>
    /// <param name="masterKeyBackupService">The master key provider and recovery service.</param>
    /// <param name="options">The store options.</param>
    public PersonSecureStore(MasterKeyBackupService masterKeyBackupService, GenSecureStoreOptions options)
    {
        _masterKeyBackupService = masterKeyBackupService ?? throw new ArgumentNullException(nameof(masterKeyBackupService));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Save<T>(string sPersonId, T value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPersonId);
        ArgumentNullException.ThrowIfNull(value);

        string sCurrentUserSid = WindowsIdentityUtilities.GetCurrentUserSid();
        byte[] arrMasterKey = _masterKeyBackupService.LoadOrCreateMasterKey();
        PersonKeyRecord keyRecord = LoadOrCreatePersonKeyRecord(sPersonId, sCurrentUserSid, arrMasterKey);
        EnsureAccessAllowed(keyRecord, sCurrentUserSid);

        byte[] arrPersonKey = UnwrapPersonKey(keyRecord, arrMasterKey);
        byte[] arrPlaintext = JsonSerializer.SerializeToUtf8Bytes(value, CryptoUtilities.JsonSerializerOptions);
        byte[] arrCiphertext = CryptoUtilities.Encrypt(arrPersonKey, arrPlaintext, out byte[] arrNonce, out byte[] arrTag);

        var encryptedPersonRecord = new EncryptedPersonRecord
        {
            PersonId = sPersonId,
            Algorithm = "A256GCM",
            Nonce = CryptoUtilities.ToBase64(arrNonce),
            Ciphertext = CryptoUtilities.ToBase64(arrCiphertext),
            Tag = CryptoUtilities.ToBase64(arrTag),
            UpdatedUtc = DateTimeOffset.UtcNow,
        };

        Directory.CreateDirectory(_options.DataDirectoryPath);
        CryptoUtilities.WriteJson(GetDataFilePath(sPersonId), encryptedPersonRecord);
    }

    /// <inheritdoc />
    public T Load<T>(string sPersonId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPersonId);

        string sCurrentUserSid = WindowsIdentityUtilities.GetCurrentUserSid();
        string sDataFilePath = GetDataFilePath(sPersonId);
        string sKeyFilePath = GetKeyFilePath(sPersonId);
        if (!File.Exists(sDataFilePath))
        {
            throw new FileNotFoundException($"The encrypted person record '{sPersonId}' does not exist.", sDataFilePath);
        }

        if (!File.Exists(sKeyFilePath))
        {
            throw new InvalidOperationException($"The wrapped key for person '{sPersonId}' is missing. The record may have been securely deleted.");
        }

        byte[] arrMasterKey = _masterKeyBackupService.LoadOrCreateMasterKey();
        PersonKeyRecord keyRecord = CryptoUtilities.ReadJson<PersonKeyRecord>(sKeyFilePath);
        EnsurePersonIdMatches(sPersonId, keyRecord.PersonId, sKeyFilePath);
        EnsureAccessAllowed(keyRecord, sCurrentUserSid);

        byte[] arrPersonKey = UnwrapPersonKey(keyRecord, arrMasterKey);
        EncryptedPersonRecord encryptedPersonRecord = CryptoUtilities.ReadJson<EncryptedPersonRecord>(sDataFilePath);
        EnsurePersonIdMatches(sPersonId, encryptedPersonRecord.PersonId, sDataFilePath);

        byte[] arrPlaintext = CryptoUtilities.Decrypt(
            arrPersonKey,
            CryptoUtilities.FromBase64(encryptedPersonRecord.Nonce),
            CryptoUtilities.FromBase64(encryptedPersonRecord.Ciphertext),
            CryptoUtilities.FromBase64(encryptedPersonRecord.Tag));

        T? value = JsonSerializer.Deserialize<T>(arrPlaintext, CryptoUtilities.JsonSerializerOptions);
        if (value is null)
        {
            throw new InvalidDataException($"The encrypted person record '{sPersonId}' could not be deserialized.");
        }

        return value;
    }

    /// <inheritdoc />
    public bool Exists(string sPersonId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPersonId);

        return File.Exists(GetDataFilePath(sPersonId));
    }

    /// <inheritdoc />
    public void Delete(string sPersonId, DeleteMode eMode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPersonId);

        string sKeyFilePath = GetKeyFilePath(sPersonId);
        string sDataFilePath = GetDataFilePath(sPersonId);

        switch (eMode)
        {
            case DeleteMode.SoftDelete:
                if (File.Exists(sDataFilePath))
                {
                    File.Delete(sDataFilePath);
                }

                if (File.Exists(sKeyFilePath))
                {
                    File.Delete(sKeyFilePath);
                }

                break;
            case DeleteMode.SecureDelete:
                if (File.Exists(sKeyFilePath))
                {
                    File.Delete(sKeyFilePath);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eMode), eMode, "Unsupported delete mode.");
        }
    }

    /// <inheritdoc />
    public void GrantAccess(string sPersonId, string sWindowsSid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPersonId);
        ArgumentException.ThrowIfNullOrWhiteSpace(sWindowsSid);

        string sCurrentUserSid = WindowsIdentityUtilities.GetCurrentUserSid();
        byte[] arrMasterKey = _masterKeyBackupService.LoadOrCreateMasterKey();
        string sKeyFilePath = GetKeyFilePath(sPersonId);
        if (!File.Exists(sKeyFilePath))
        {
            throw new FileNotFoundException($"The wrapped key for person '{sPersonId}' does not exist.", sKeyFilePath);
        }

        PersonKeyRecord keyRecord = CryptoUtilities.ReadJson<PersonKeyRecord>(sKeyFilePath);
        EnsureAccessAllowed(keyRecord, sCurrentUserSid);

        if (!keyRecord.AllowedWindowsSids.Contains(sWindowsSid, StringComparer.OrdinalIgnoreCase))
        {
            keyRecord.AllowedWindowsSids.Add(sWindowsSid);
            keyRecord.AllowedWindowsSids.Sort(StringComparer.OrdinalIgnoreCase);
            keyRecord.UpdatedUtc = DateTimeOffset.UtcNow;
            CryptoUtilities.WriteJson(sKeyFilePath, keyRecord);
        }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> GetAllowedWindowsSids(string sPersonId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPersonId);

        string sCurrentUserSid = WindowsIdentityUtilities.GetCurrentUserSid();
        string sKeyFilePath = GetKeyFilePath(sPersonId);
        if (!File.Exists(sKeyFilePath))
        {
            throw new FileNotFoundException($"The wrapped key for person '{sPersonId}' does not exist.", sKeyFilePath);
        }

        PersonKeyRecord keyRecord = CryptoUtilities.ReadJson<PersonKeyRecord>(sKeyFilePath);
        EnsureAccessAllowed(keyRecord, sCurrentUserSid);
        return keyRecord.AllowedWindowsSids.AsReadOnly();
    }

    private PersonKeyRecord LoadOrCreatePersonKeyRecord(string sPersonId, string sCurrentUserSid, byte[] arrMasterKey)
    {
        string sKeyFilePath = GetKeyFilePath(sPersonId);
        if (File.Exists(sKeyFilePath))
        {
            PersonKeyRecord existingRecord = CryptoUtilities.ReadJson<PersonKeyRecord>(sKeyFilePath);
            EnsurePersonIdMatches(sPersonId, existingRecord.PersonId, sKeyFilePath);
            return existingRecord;
        }

        byte[] arrPersonKey = RandomNumberGenerator.GetBytes(32);
        byte[] arrWrappedPersonKey = CryptoUtilities.Encrypt(arrMasterKey, arrPersonKey, out byte[] arrNonce, out byte[] arrTag);

        var newRecord = new PersonKeyRecord
        {
            PersonId = sPersonId,
            Algorithm = "A256GCM",
            Nonce = CryptoUtilities.ToBase64(arrNonce),
            WrappedPersonKey = CryptoUtilities.ToBase64(arrWrappedPersonKey),
            Tag = CryptoUtilities.ToBase64(arrTag),
            OwnerWindowsSid = sCurrentUserSid,
            AllowedWindowsSids = new List<string> { sCurrentUserSid },
            CreatedUtc = DateTimeOffset.UtcNow,
            UpdatedUtc = DateTimeOffset.UtcNow,
        };

        Directory.CreateDirectory(_options.KeyDirectoryPath);
        CryptoUtilities.WriteJson(sKeyFilePath, newRecord);
        return newRecord;
    }

    private byte[] UnwrapPersonKey(PersonKeyRecord keyRecord, byte[] arrMasterKey)
    {
        return CryptoUtilities.Decrypt(
            arrMasterKey,
            CryptoUtilities.FromBase64(keyRecord.Nonce),
            CryptoUtilities.FromBase64(keyRecord.WrappedPersonKey),
            CryptoUtilities.FromBase64(keyRecord.Tag));
    }

    private void EnsureAccessAllowed(PersonKeyRecord keyRecord, string sCurrentUserSid)
    {
        if (!keyRecord.AllowedWindowsSids.Contains(sCurrentUserSid, StringComparer.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException($"The current Windows user SID '{sCurrentUserSid}' is not allowed to decrypt person '{keyRecord.PersonId}'.");
        }
    }

    private static void EnsurePersonIdMatches(string sExpectedPersonId, string sActualPersonId, string sFilePath)
    {
        if (!string.Equals(sExpectedPersonId, sActualPersonId, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"The file '{sFilePath}' does not belong to person '{sExpectedPersonId}'.");
        }
    }

    private string GetDataFilePath(string sPersonId)
    {
        string sFileId = CryptoUtilities.ToDeterministicFileId(sPersonId);
        return Path.Combine(_options.DataDirectoryPath, sFileId + ".person.json");
    }

    private string GetKeyFilePath(string sPersonId)
    {
        string sFileId = CryptoUtilities.ToDeterministicFileId(sPersonId);
        return Path.Combine(_options.KeyDirectoryPath, sFileId + ".key.json");
    }
}
