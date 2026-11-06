using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Config.Secrets;

/// <summary>
/// Stores credentials in an explicitly initialized, password-encrypted local vault.
/// The passphrase and derived key are kept only in memory until the vault is locked.
/// </summary>
public sealed class EncryptedFileCredentialProvider : ICredentialProvider, IDisposable, IAsyncDisposable
{
    private const int FormatVersion = 1;
    private const int KdfIterations = 600_000;
    private const int KeySizeBytes = 32;
    private const int SaltSizeBytes = 16;
    private const int NonceSizeBytes = 12;
    private const int TagSizeBytes = 16;
    private const int MinimumPassphraseLength = 12;
    private const int MaximumPassphraseLength = 1024;
    private const int MaximumSecretBytes = 1024 * 1024;
    private const int MaximumVaultBytes = 16 * 1024 * 1024;
    private const int MaximumEntries = 4096;
    private const string KdfName = "PBKDF2-HMAC-SHA256";
    private static readonly UTF8Encoding s_utf8 = new(false, true);
    private static readonly byte[] s_verifier = Encoding.ASCII.GetBytes("AvlnCodeStudio local vault verifier v1");

    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly string _vaultPath;
    private VaultDocument? _document;
    private byte[]? _key;
    private byte[]? _loadedFileHash;
    private bool _disposed;

    /// <summary>Creates a provider for the explicitly selected vault file.</summary>
    public EncryptedFileCredentialProvider(string vaultPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(vaultPath);
        _vaultPath = Path.GetFullPath(vaultPath);
    }

    /// <inheritdoc/>
    public string ProviderId => "encrypted-file-vault";

    /// <summary>Gets the absolute path of this vault; the file contains only encrypted data.</summary>
    public string VaultPath => _vaultPath;

    /// <summary>Gets whether a vault file has been initialized.</summary>
    public bool IsInitialized => File.Exists(_vaultPath);

    /// <summary>Gets whether this provider currently holds an unlocked encryption key.</summary>
    public bool IsUnlocked => Volatile.Read(ref _key) is not null;

    /// <summary>
    /// Creates a new vault using the supplied passphrase. This is the only operation that
    /// creates the parent directory; it never replaces an existing vault.
    /// </summary>
    public async Task InitializeAsync(
        CredentialSecret passphrase,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(passphrase);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        byte[]? key = null;
        try
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            if (File.Exists(_vaultPath))
            {
                throw new IOException("A vault already exists at the selected path.");
            }

            ValidatePassphrase(passphrase.Reveal(), requireMinimumLength: true);
            EnsureNoReparsePoints(createParentDirectory: true);

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            try
            {
                key = DeriveKey(passphrase.Reveal(), salt);
                VaultDocument document = CreateDocument(salt, key);
                byte[] fileHash = await WriteDocumentAsync(
                    document,
                    expectedFileHash: null,
                    createOnly: true,
                    cancellationToken).ConfigureAwait(false);

                _document = document;
                _loadedFileHash = fileHash;
                Volatile.Write(ref _key, key);
                key = null;
            }
            finally
            {
                CryptographicOperations.ZeroMemory(salt);
            }
        }
        finally
        {
            if (key is not null)
            {
                CryptographicOperations.ZeroMemory(key);
            }

            _gate.Release();
        }
    }

    /// <summary>Unlocks an existing vault after authenticating its contents.</summary>
    public async Task UnlockAsync(
        CredentialSecret passphrase,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(passphrase);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        byte[]? key = null;
        byte[]? fileBytes = null;
        try
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            if (_key is not null)
            {
                throw new InvalidOperationException("The vault is already unlocked.");
            }

            fileBytes = ReadVaultFile();
            VaultDocument document = DeserializeDocument(fileBytes);
            ValidateDocument(document);
            byte[] salt = Convert.FromBase64String(document.Salt);
            try
            {
                ValidatePassphrase(passphrase.Reveal(), requireMinimumLength: false);
                key = DeriveKey(passphrase.Reveal(), salt);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(salt);
            }

            try
            {
                VerifyDocument(document, key);
            }
            catch (CryptographicException)
            {
                throw new CredentialVaultUnlockException();
            }

            _document = document;
            _loadedFileHash = SHA256.HashData(fileBytes);
            Volatile.Write(ref _key, key);
            key = null;
        }
        finally
        {
            if (key is not null)
            {
                CryptographicOperations.ZeroMemory(key);
            }

            if (fileBytes is not null)
            {
                CryptographicOperations.ZeroMemory(fileBytes);
            }

            _gate.Release();
        }
    }

    /// <summary>Locks the vault and clears its in-memory encryption key.</summary>
    public async ValueTask LockAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            ClearSession();
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc/>
    public async ValueTask<CredentialProviderStatus> GetStatusAsync(
        CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            CredentialProviderAvailability availability = _key is not null
                ? CredentialProviderAvailability.Available
                : IsInitialized
                    ? CredentialProviderAvailability.Locked
                    : CredentialProviderAvailability.Unavailable;
            return new CredentialProviderStatus(ProviderId, availability);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc/>
    public async ValueTask<CredentialSecret?> GetAsync(
        CredentialId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        byte[]? plaintext = null;
        char[]? characters = null;
        byte[]? associatedData = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            (VaultDocument document, byte[] key) = GetUnlockedState();
            EnsureLoadedFileUnchanged();
            string opaqueId = GetOpaqueId(id, key);
            if (!document.Entries.TryGetValue(opaqueId, out VaultEntry? entry))
            {
                return null;
            }

            byte[] nonce = Convert.FromBase64String(entry.Nonce);
            byte[] ciphertext = Convert.FromBase64String(entry.Ciphertext);
            byte[] tag = Convert.FromBase64String(entry.Tag);
            plaintext = new byte[ciphertext.Length];
            associatedData = CreateAssociatedData(document, opaqueId);
            try
            {
                using AesGcm aes = new(key, TagSizeBytes);
                aes.Decrypt(nonce, ciphertext, tag, plaintext, associatedData);
                characters = new char[s_utf8.GetCharCount(plaintext)];
                s_utf8.GetChars(plaintext, characters);
                return new CredentialSecret(characters);
            }
            catch (CryptographicException)
            {
                throw new InvalidDataException("A vault entry failed authentication. Lock and reopen the vault.");
            }
            catch (DecoderFallbackException exception)
            {
                throw new InvalidDataException("A vault entry is not valid UTF-8.", exception);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(nonce);
                CryptographicOperations.ZeroMemory(ciphertext);
                CryptographicOperations.ZeroMemory(tag);
            }
        }
        finally
        {
            if (plaintext is not null)
            {
                CryptographicOperations.ZeroMemory(plaintext);
            }

            if (characters is not null)
            {
                CryptographicOperations.ZeroMemory(MemoryMarshal.AsBytes(characters.AsSpan()));
            }

            if (associatedData is not null)
            {
                CryptographicOperations.ZeroMemory(associatedData);
            }

            _gate.Release();
        }
    }

    /// <inheritdoc/>
    public async ValueTask SetAsync(
        CredentialId id,
        CredentialSecret secret,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(secret);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        byte[]? plaintext = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            (VaultDocument document, byte[] key) = GetUnlockedState();
            EnsureLoadedFileUnchanged();
            int byteCount;
            try
            {
                byteCount = s_utf8.GetByteCount(secret.Reveal());
            }
            catch (EncoderFallbackException exception)
            {
                throw new ArgumentException("The credential contains characters that cannot be encoded as UTF-8.", nameof(secret), exception);
            }

            if (byteCount > MaximumSecretBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(secret), "The vault credential exceeds the supported size limit.");
            }

            plaintext = new byte[byteCount];
            s_utf8.GetBytes(secret.Reveal(), plaintext);
            string opaqueId = GetOpaqueId(id, key);
            VaultEntry entry = EncryptEntry(document, opaqueId, key, plaintext);
            Dictionary<string, VaultEntry> entries = new(document.Entries, StringComparer.Ordinal)
            {
                [opaqueId] = entry,
            };
            if (entries.Count > MaximumEntries)
            {
                throw new InvalidOperationException("The vault has reached its supported credential count limit.");
            }

            VaultDocument updated = CopyDocument(document, entries);
            cancellationToken.ThrowIfCancellationRequested();
            byte[] updatedHash = await WriteDocumentAsync(
                updated,
                _loadedFileHash,
                createOnly: false,
                cancellationToken).ConfigureAwait(false);
            _document = updated;
            ReplaceLoadedHash(updatedHash);
        }
        finally
        {
            if (plaintext is not null)
            {
                CryptographicOperations.ZeroMemory(plaintext);
            }

            _gate.Release();
        }
    }

    /// <inheritdoc/>
    public async ValueTask DeleteAsync(
        CredentialId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            (VaultDocument document, byte[] key) = GetUnlockedState();
            EnsureLoadedFileUnchanged();
            string opaqueId = GetOpaqueId(id, key);
            if (!document.Entries.ContainsKey(opaqueId))
            {
                return;
            }

            Dictionary<string, VaultEntry> entries = new(document.Entries, StringComparer.Ordinal);
            entries.Remove(opaqueId);
            VaultDocument updated = CopyDocument(document, entries);
            cancellationToken.ThrowIfCancellationRequested();
            byte[] updatedHash = await WriteDocumentAsync(
                updated,
                _loadedFileHash,
                createOnly: false,
                cancellationToken).ConfigureAwait(false);
            _document = updated;
            ReplaceLoadedHash(updatedHash);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _gate.Wait();
        try
        {
            if (_disposed)
            {
                return;
            }

            ClearSession();
            _disposed = true;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_disposed)
            {
                return;
            }

            ClearSession();
            _disposed = true;
        }
        finally
        {
            _gate.Release();
        }
    }

    private static VaultDocument CreateDocument(byte[] salt, byte[] key)
    {
        VaultDocument document = new()
        {
            Version = FormatVersion,
            Kdf = KdfName,
            Iterations = KdfIterations,
            Salt = Convert.ToBase64String(salt),
            Entries = new Dictionary<string, VaultEntry>(StringComparer.Ordinal),
        };
        byte[] associatedData = CreateAssociatedData(document, "$verifier");
        try
        {
            (byte[] nonce, byte[] ciphertext, byte[] tag) = EncryptBytes(key, associatedData, s_verifier);
            try
            {
                document.VerifierNonce = Convert.ToBase64String(nonce);
                document.VerifierCiphertext = Convert.ToBase64String(ciphertext);
                document.VerifierTag = Convert.ToBase64String(tag);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(nonce);
                CryptographicOperations.ZeroMemory(ciphertext);
                CryptographicOperations.ZeroMemory(tag);
            }
        }
        finally
        {
            CryptographicOperations.ZeroMemory(associatedData);
        }

        return document;
    }

    private static VaultEntry EncryptEntry(VaultDocument document, string opaqueId, byte[] key, byte[] plaintext)
    {
        byte[] associatedData = CreateAssociatedData(document, opaqueId);
        try
        {
            (byte[] nonce, byte[] ciphertext, byte[] tag) = EncryptBytes(key, associatedData, plaintext);
            try
            {
                return new VaultEntry
                {
                    Nonce = Convert.ToBase64String(nonce),
                    Ciphertext = Convert.ToBase64String(ciphertext),
                    Tag = Convert.ToBase64String(tag),
                };
            }
            finally
            {
                CryptographicOperations.ZeroMemory(nonce);
                CryptographicOperations.ZeroMemory(ciphertext);
                CryptographicOperations.ZeroMemory(tag);
            }
        }
        finally
        {
            CryptographicOperations.ZeroMemory(associatedData);
        }
    }

    private static (byte[] Nonce, byte[] Ciphertext, byte[] Tag) EncryptBytes(
        byte[] key,
        byte[] associatedData,
        byte[] plaintext)
    {
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSizeBytes);
        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[TagSizeBytes];
        using AesGcm aes = new(key, TagSizeBytes);
        aes.Encrypt(nonce, plaintext, ciphertext, tag, associatedData);
        return (nonce, ciphertext, tag);
    }

    private static byte[] DeriveKey(ReadOnlySpan<char> passphrase, byte[] salt)
    {
        byte[] key = new byte[KeySizeBytes];
        Rfc2898DeriveBytes.Pbkdf2(
            passphrase,
            salt,
            key,
            KdfIterations,
            HashAlgorithmName.SHA256);
        return key;
    }

    private static string GetOpaqueId(CredentialId id, byte[] key)
    {
        string identity = string.Concat(id.ServiceId, "\0", id.Name, "\0", id.ScopeId ?? string.Empty);
        byte[] identityBytes = s_utf8.GetBytes(identity);
        byte[] opaqueBytes = [];
        try
        {
            opaqueBytes = HMACSHA256.HashData(key, identityBytes);
            return Convert.ToHexString(opaqueBytes);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(identityBytes);
            CryptographicOperations.ZeroMemory(opaqueBytes);
        }
    }

    private static byte[] CreateAssociatedData(VaultDocument document, string opaqueId) =>
        Encoding.ASCII.GetBytes(string.Concat(
            "AvlnCodeStudio.LocalVault:",
            document.Version,
            ":",
            document.Kdf,
            ":",
            document.Iterations,
            ":",
            document.Salt,
            ":",
            opaqueId));

    private static VaultDocument CopyDocument(VaultDocument source, Dictionary<string, VaultEntry> entries) =>
        new()
        {
            Version = source.Version,
            Kdf = source.Kdf,
            Iterations = source.Iterations,
            Salt = source.Salt,
            VerifierNonce = source.VerifierNonce,
            VerifierCiphertext = source.VerifierCiphertext,
            VerifierTag = source.VerifierTag,
            Entries = entries,
        };

    private static void VerifyDocument(VaultDocument document, byte[] key)
    {
        byte[] verifier = DecryptEntryBytes(
            document,
            "$verifier",
            key,
            document.VerifierNonce,
            document.VerifierCiphertext,
            document.VerifierTag);
        try
        {
            if (!CryptographicOperations.FixedTimeEquals(verifier, s_verifier))
            {
                throw new CryptographicException();
            }
        }
        finally
        {
            CryptographicOperations.ZeroMemory(verifier);
        }

        foreach ((string opaqueId, VaultEntry entry) in document.Entries)
        {
            byte[] plaintext = DecryptEntryBytes(
                document,
                opaqueId,
                key,
                entry.Nonce,
                entry.Ciphertext,
                entry.Tag);
            try
            {
                _ = s_utf8.GetCharCount(plaintext);
            }
            catch (DecoderFallbackException exception)
            {
                throw new InvalidDataException("A vault entry is not valid UTF-8.", exception);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(plaintext);
            }
        }
    }

    private static byte[] DecryptEntryBytes(
        VaultDocument document,
        string opaqueId,
        byte[] key,
        string nonceValue,
        string ciphertextValue,
        string tagValue)
    {
        byte[] nonce = Convert.FromBase64String(nonceValue);
        byte[] ciphertext = Convert.FromBase64String(ciphertextValue);
        byte[] tag = Convert.FromBase64String(tagValue);
        byte[] associatedData = CreateAssociatedData(document, opaqueId);
        byte[] plaintext = new byte[ciphertext.Length];
        bool decrypted = false;
        try
        {
            using AesGcm aes = new(key, TagSizeBytes);
            aes.Decrypt(nonce, ciphertext, tag, plaintext, associatedData);
            decrypted = true;
            return plaintext;
        }
        finally
        {
            if (!decrypted)
            {
                CryptographicOperations.ZeroMemory(plaintext);
            }

            CryptographicOperations.ZeroMemory(nonce);
            CryptographicOperations.ZeroMemory(ciphertext);
            CryptographicOperations.ZeroMemory(tag);
            CryptographicOperations.ZeroMemory(associatedData);
        }
    }

    private static void ValidateDocument(VaultDocument document)
    {
        if (document.Kdf is null
            || document.Salt is null
            || document.VerifierNonce is null
            || document.VerifierCiphertext is null
            || document.VerifierTag is null
            || document.Entries is null
            || document.Version != FormatVersion
            || !string.Equals(document.Kdf, KdfName, StringComparison.Ordinal)
            || document.Iterations != KdfIterations)
        {
            throw new InvalidDataException("The vault format or key derivation parameters are not supported.");
        }

        if (GetBase64Length(document.Salt, "salt") != SaltSizeBytes
            || GetBase64Length(document.VerifierNonce, "verifier nonce") != NonceSizeBytes
            || GetBase64Length(document.VerifierCiphertext, "verifier ciphertext") != s_verifier.Length
            || GetBase64Length(document.VerifierTag, "verifier tag") != TagSizeBytes
            || document.Entries.Count > MaximumEntries)
        {
            throw new InvalidDataException("The vault metadata is invalid or exceeds supported limits.");
        }

        foreach ((string opaqueId, VaultEntry entry) in document.Entries)
        {
            if (opaqueId is null
                || entry is null
                || entry.Nonce is null
                || entry.Ciphertext is null
                || entry.Tag is null
                || opaqueId.Length != 64
                || !opaqueId.All(Uri.IsHexDigit)
                || GetBase64Length(entry.Nonce, "entry nonce") != NonceSizeBytes
                || GetBase64Length(entry.Tag, "entry tag") != TagSizeBytes
                || GetBase64Length(entry.Ciphertext, "entry ciphertext") > MaximumSecretBytes)
            {
                throw new InvalidDataException("A vault entry is invalid or exceeds supported limits.");
            }
        }
    }

    private static void ValidatePassphrase(ReadOnlySpan<char> passphrase, bool requireMinimumLength)
    {
        if (passphrase.Length > MaximumPassphraseLength)
        {
            throw new ArgumentOutOfRangeException(nameof(passphrase), "The vault passphrase exceeds the supported length.");
        }

        if (requireMinimumLength && passphrase.Length < MinimumPassphraseLength)
        {
            throw new ArgumentException(
                $"A vault passphrase must contain at least {MinimumPassphraseLength} characters.",
                nameof(passphrase));
        }
    }

    private static int GetBase64Length(string value, string fieldName)
    {
        byte[] decoded;
        try
        {
            decoded = Convert.FromBase64String(value);
        }
        catch (FormatException exception)
        {
            throw new InvalidDataException($"The vault {fieldName} is not valid base64.", exception);
        }

        try
        {
            return decoded.Length;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(decoded);
        }
    }

    private static VaultDocument DeserializeDocument(byte[] fileBytes)
    {
        try
        {
            return JsonSerializer.Deserialize<VaultDocument>(fileBytes)
                ?? throw new InvalidDataException("The vault file is empty.");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("The vault file is not valid JSON.", exception);
        }
    }

    private (VaultDocument Document, byte[] Key) GetUnlockedState()
    {
        ThrowIfDisposed();
        return _document is not null && _key is not null
            ? (_document, _key)
            : throw new CredentialProviderLockedException(ProviderId);
    }

    private void EnsureLoadedFileUnchanged()
    {
        if (_loadedFileHash is null)
        {
            throw new InvalidOperationException("The unlocked vault has no file snapshot.");
        }

        byte[] currentFile = ReadVaultFile();
        try
        {
            byte[] currentHash = SHA256.HashData(currentFile);
            try
            {
                if (!CryptographicOperations.FixedTimeEquals(currentHash, _loadedFileHash))
                {
                    throw new IOException("The vault file changed outside this provider. Lock and reopen it before retrying.");
                }
            }
            finally
            {
                CryptographicOperations.ZeroMemory(currentHash);
            }
        }
        finally
        {
            CryptographicOperations.ZeroMemory(currentFile);
        }
    }

    private byte[] ReadVaultFile()
    {
        EnsureNoReparsePoints(createParentDirectory: false);
        FileInfo file = new(_vaultPath);
        if (!file.Exists)
        {
            throw new FileNotFoundException("The encrypted vault has not been initialized.", _vaultPath);
        }

        if (file.Length > MaximumVaultBytes)
        {
            throw new InvalidDataException("The vault file exceeds the supported size limit.");
        }

        byte[] bytes = File.ReadAllBytes(_vaultPath);
        if (bytes.Length > MaximumVaultBytes)
        {
            CryptographicOperations.ZeroMemory(bytes);
            throw new InvalidDataException("The vault file exceeds the supported size limit.");
        }

        return bytes;
    }

    private async Task<byte[]> WriteDocumentAsync(
        VaultDocument document,
        byte[]? expectedFileHash,
        bool createOnly,
        CancellationToken cancellationToken)
    {
        EnsureNoReparsePoints(createParentDirectory: false);
        string directory = Path.GetDirectoryName(_vaultPath)
            ?? throw new InvalidOperationException("The vault path has no parent directory.");
        byte[] fileBytes = JsonSerializer.SerializeToUtf8Bytes(document);
        if (fileBytes.Length > MaximumVaultBytes)
        {
            CryptographicOperations.ZeroMemory(fileBytes);
            throw new InvalidOperationException("The vault has reached its supported file-size limit.");
        }

        string temporaryPath = Path.Combine(directory, $".{Path.GetFileName(_vaultPath)}.{Guid.NewGuid():N}.tmp");
        try
        {
            await using (FileStream stream = new(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                if (!OperatingSystem.IsWindows())
                {
                    File.SetUnixFileMode(
                        temporaryPath,
                        UnixFileMode.UserRead | UnixFileMode.UserWrite);
                }

                await stream.WriteAsync(fileBytes, cancellationToken).ConfigureAwait(false);
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                stream.Flush(flushToDisk: true);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (expectedFileHash is not null)
            {
                byte[] currentFile = ReadVaultFile();
                try
                {
                    byte[] currentHash = SHA256.HashData(currentFile);
                    try
                    {
                        if (!CryptographicOperations.FixedTimeEquals(currentHash, expectedFileHash))
                        {
                            throw new IOException("The vault file changed outside this provider. Lock and reopen it before retrying.");
                        }
                    }
                    finally
                    {
                        CryptographicOperations.ZeroMemory(currentHash);
                    }
                }
                finally
                {
                    CryptographicOperations.ZeroMemory(currentFile);
                }
            }
            else if (File.Exists(_vaultPath))
            {
                throw new IOException("A vault already exists at the selected path.");
            }

            File.Move(temporaryPath, _vaultPath, overwrite: !createOnly);
            return SHA256.HashData(fileBytes);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(fileBytes);
            File.Delete(temporaryPath);
        }
    }

    private void EnsureNoReparsePoints(bool createParentDirectory)
    {
        string directory = Path.GetDirectoryName(_vaultPath)
            ?? throw new InvalidOperationException("The vault path has no parent directory.");
        CheckExistingPathSegments(directory);
        if (createParentDirectory)
        {
            Directory.CreateDirectory(directory);
            CheckExistingPathSegments(directory);
        }

        if (File.Exists(_vaultPath)
            && File.GetAttributes(_vaultPath).HasFlag(FileAttributes.ReparsePoint))
        {
            throw new IOException("The vault file cannot be a symbolic link or reparse point.");
        }
    }

    private static void CheckExistingPathSegments(string path)
    {
        string? current = Path.GetPathRoot(path);
        if (string.IsNullOrEmpty(current))
        {
            throw new IOException("The vault directory path is invalid.");
        }

        foreach (string segment in path[current.Length..]
                     .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries))
        {
            current = Path.Combine(current, segment);
            if (Directory.Exists(current)
                && File.GetAttributes(current).HasFlag(FileAttributes.ReparsePoint))
            {
                throw new IOException("The vault directory cannot contain a symbolic link or reparse point.");
            }
        }
    }

    private void ReplaceLoadedHash(byte[] hash)
    {
        if (_loadedFileHash is not null)
        {
            CryptographicOperations.ZeroMemory(_loadedFileHash);
        }

        _loadedFileHash = hash;
    }

    private void ClearSession()
    {
        byte[]? key = Interlocked.Exchange(ref _key, null);
        if (key is not null)
        {
            CryptographicOperations.ZeroMemory(key);
        }

        if (_loadedFileHash is not null)
        {
            CryptographicOperations.ZeroMemory(_loadedFileHash);
            _loadedFileHash = null;
        }

        _document = null;
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    private sealed class VaultDocument
    {
        public int Version { get; set; }
        public string Kdf { get; set; } = string.Empty;
        public int Iterations { get; set; }
        public string Salt { get; set; } = string.Empty;
        public string VerifierNonce { get; set; } = string.Empty;
        public string VerifierCiphertext { get; set; } = string.Empty;
        public string VerifierTag { get; set; } = string.Empty;
        public Dictionary<string, VaultEntry> Entries { get; set; } = new(StringComparer.Ordinal);
    }

    private sealed class VaultEntry
    {
        public string Nonce { get; set; } = string.Empty;
        public string Ciphertext { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
    }
}

/// <summary>Indicates that a vault passphrase did not authenticate the vault contents.</summary>
public sealed class CredentialVaultUnlockException : InvalidOperationException
{
    /// <summary>Creates a generic, non-secret unlock failure.</summary>
    public CredentialVaultUnlockException()
        : base("The vault could not be unlocked. The passphrase may be incorrect or the vault may be damaged.")
    {
    }
}
