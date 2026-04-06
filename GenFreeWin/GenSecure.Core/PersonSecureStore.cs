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
    public void Save<T>(string sPersonId, T value, StoreMode eStoreMode = StoreMode.Encrypted)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPersonId);
        ArgumentNullException.ThrowIfNull(value);

        if (eStoreMode == StoreMode.Plaintext)
        {
            SavePlaintext(sPersonId, value);
            return;
        }

        string sCurrentUserSid = WindowsIdentityUtilities.GetCurrentUserSid();
        byte[] arrMasterKey = _masterKeyBackupService.LoadOrCreateMasterKey();
        byte[] arrSidPepperKey = CryptoUtilities.DeriveSidPepperKey(arrMasterKey);
        PersonKeyRecord keyRecord = LoadOrCreatePersonKeyRecord(sPersonId, sCurrentUserSid, arrMasterKey, arrSidPepperKey);
        EnsureAccessAllowed(keyRecord, sCurrentUserSid, arrSidPepperKey);

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

        string sDataFilePath = GetDataFilePath(sPersonId);
        string sKeyFilePath = GetKeyFilePath(sPersonId);
        if (!File.Exists(sDataFilePath))
        {
            throw new FileNotFoundException($"The person record '{sPersonId}' does not exist.", sDataFilePath);
        }

        // Detect storage mode from the algorithm field without a second file read.
        string sDataJson = File.ReadAllText(sDataFilePath, System.Text.Encoding.UTF8);
        string sAlgorithm = ReadAlgorithmField(sDataJson);

        if (sAlgorithm == "plaintext")
        {
            return LoadPlaintext<T>(sPersonId, sDataFilePath, sDataJson);
        }

        // Encrypted path
        if (!File.Exists(sKeyFilePath))
        {
            throw new InvalidOperationException($"The wrapped key for person '{sPersonId}' is missing. The record may have been securely deleted.");
        }

        string sCurrentUserSid = WindowsIdentityUtilities.GetCurrentUserSid();
        byte[] arrMasterKey = _masterKeyBackupService.LoadOrCreateMasterKey();
        byte[] arrSidPepperKey = CryptoUtilities.DeriveSidPepperKey(arrMasterKey);
        PersonKeyRecord keyRecord = CryptoUtilities.ReadJson<PersonKeyRecord>(sKeyFilePath);
        EnsurePersonIdMatches(sPersonId, keyRecord.PersonId, sKeyFilePath);
        EnsureAccessAllowed(keyRecord, sCurrentUserSid, arrSidPepperKey);

        byte[] arrPersonKey = UnwrapPersonKey(keyRecord, arrMasterKey);
        EncryptedPersonRecord encryptedPersonRecord = JsonSerializer.Deserialize<EncryptedPersonRecord>(sDataJson, CryptoUtilities.JsonSerializerOptions)
            ?? throw new InvalidDataException($"The encrypted person record '{sPersonId}' could not be deserialized.");
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
                    // Crypto-deletion: removing the DEK makes the ciphertext permanently unreadable.
                    File.Delete(sKeyFilePath);
                }
                else if (File.Exists(sDataFilePath))
                {
                    // Plaintext record — no crypto-deletion possible; remove the data file directly.
                    File.Delete(sDataFilePath);
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
        byte[] arrSidPepperKey = CryptoUtilities.DeriveSidPepperKey(arrMasterKey);
        string sKeyFilePath = GetKeyFilePath(sPersonId);
        if (!File.Exists(sKeyFilePath))
        {
            throw new FileNotFoundException($"The wrapped key for person '{sPersonId}' does not exist. GrantAccess is not applicable to plaintext records.", sKeyFilePath);
        }

        PersonKeyRecord keyRecord = CryptoUtilities.ReadJson<PersonKeyRecord>(sKeyFilePath);
        EnsureAccessAllowed(keyRecord, sCurrentUserSid, arrSidPepperKey);

        string sSidHash = CryptoUtilities.ToSidHash(sWindowsSid, arrSidPepperKey);
        if (!keyRecord.AllowedWindowsSidHashes.Contains(sSidHash, StringComparer.OrdinalIgnoreCase))
        {
            keyRecord.AllowedWindowsSidHashes.Add(sSidHash);
            keyRecord.AllowedWindowsSidHashes.Sort(StringComparer.OrdinalIgnoreCase);
            keyRecord.UpdatedUtc = DateTimeOffset.UtcNow;
            CryptoUtilities.WriteJson(sKeyFilePath, keyRecord);
        }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> GetAllowedWindowsSidHashes(string sPersonId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPersonId);

        string sCurrentUserSid = WindowsIdentityUtilities.GetCurrentUserSid();
        byte[] arrMasterKey = _masterKeyBackupService.LoadOrCreateMasterKey();
        byte[] arrSidPepperKey = CryptoUtilities.DeriveSidPepperKey(arrMasterKey);
        string sKeyFilePath = GetKeyFilePath(sPersonId);
        if (!File.Exists(sKeyFilePath))
        {
            throw new FileNotFoundException($"The wrapped key for person '{sPersonId}' does not exist. GetAllowedWindowsSidHashes is not applicable to plaintext records.", sKeyFilePath);
        }

        PersonKeyRecord keyRecord = CryptoUtilities.ReadJson<PersonKeyRecord>(sKeyFilePath);
        EnsureAccessAllowed(keyRecord, sCurrentUserSid, arrSidPepperKey);
        return keyRecord.AllowedWindowsSidHashes.AsReadOnly();
    }

    private PersonKeyRecord LoadOrCreatePersonKeyRecord(string sPersonId, string sCurrentUserSid, byte[] arrMasterKey, byte[] arrSidPepperKey)
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
            OwnerWindowsSidHash = CryptoUtilities.ToSidHash(sCurrentUserSid, arrSidPepperKey),
            AllowedWindowsSidHashes = new List<string> { CryptoUtilities.ToSidHash(sCurrentUserSid, arrSidPepperKey) },
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

    private void EnsureAccessAllowed(PersonKeyRecord keyRecord, string sCurrentUserSid, byte[] arrSidPepperKey)
    {
        string sSidHash = CryptoUtilities.ToSidHash(sCurrentUserSid, arrSidPepperKey);
        if (!keyRecord.AllowedWindowsSidHashes.Contains(sSidHash, StringComparer.OrdinalIgnoreCase))
        {
            // Avoid logging the raw SID to prevent metadata leaks in exception messages.
            throw new UnauthorizedAccessException($"The current Windows user is not allowed to decrypt person '{keyRecord.PersonId}'.");
        }
    }

    private static void EnsurePersonIdMatches(string sExpectedPersonId, string sActualPersonId, string sFilePath)
    {
        if (!string.Equals(sExpectedPersonId, sActualPersonId, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"The file '{sFilePath}' does not belong to person '{sExpectedPersonId}'.");
        }
    }

    private void SavePlaintext<T>(string sPersonId, T value)
    {
        Directory.CreateDirectory(_options.DataDirectoryPath);

        var record = new PlaintextPersonRecord
        {
            PersonId = sPersonId,
            Data = JsonSerializer.SerializeToElement(value, CryptoUtilities.JsonSerializerOptions),
            UpdatedUtc = DateTimeOffset.UtcNow,
        };

        CryptoUtilities.WriteJson(GetDataFilePath(sPersonId), record);
    }

    private T LoadPlaintext<T>(string sPersonId, string sDataFilePath, string sDataJson)
    {
        PlaintextPersonRecord plainRecord = JsonSerializer.Deserialize<PlaintextPersonRecord>(sDataJson, CryptoUtilities.JsonSerializerOptions)
            ?? throw new InvalidDataException($"The plaintext person record '{sPersonId}' could not be deserialized.");

        EnsurePersonIdMatches(sPersonId, plainRecord.PersonId, sDataFilePath);

        T? value = JsonSerializer.Deserialize<T>(plainRecord.Data.GetRawText(), CryptoUtilities.JsonSerializerOptions);
        return value ?? throw new InvalidDataException($"The plaintext person record '{sPersonId}' could not be deserialized.");
    }

    private static string ReadAlgorithmField(string sDataJson)
    {
        using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(sDataJson);
        if (doc.RootElement.TryGetProperty("Algorithm", out System.Text.Json.JsonElement algElement))
        {
            return algElement.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    private string GetDataFilePath(string sPersonId)
    {
        return CryptoUtilities.GetShardedFilePath(_options.DataDirectoryPath, sPersonId, ".person.json");
    }

    private string GetKeyFilePath(string sPersonId)
    {
        return CryptoUtilities.GetShardedFilePath(_options.KeyDirectoryPath, sPersonId, ".key.json");
    }
}
