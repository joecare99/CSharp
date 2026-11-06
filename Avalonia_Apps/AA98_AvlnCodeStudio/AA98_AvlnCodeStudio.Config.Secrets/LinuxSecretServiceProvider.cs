using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Config.Secrets;

/// <summary>
/// Stores credentials in the default Linux Secret Service collection over an encrypted D-Bus session.
/// One service connection is retained for this provider instance because the upstream binding does not
/// expose a disposal contract.
/// </summary>
public sealed class LinuxSecretServiceProvider : ICredentialProvider
{
    private static readonly UTF8Encoding s_utf8 = new(false, true);
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly ILinuxSecretServiceBackend _backend;
    private readonly bool _requiresLinux;

    /// <summary>Creates a provider backed by the current Linux session's Secret Service.</summary>
    public LinuxSecretServiceProvider()
        : this(new LinuxSecretServiceBackend(), requiresLinux: true)
    {
    }

    internal LinuxSecretServiceProvider(ILinuxSecretServiceBackend backend, bool requiresLinux = true)
    {
        _backend = backend ?? throw new ArgumentNullException(nameof(backend));
        _requiresLinux = requiresLinux;
    }

    /// <inheritdoc/>
    public string ProviderId => "linux-secret-service";

    /// <inheritdoc/>
    public async ValueTask<CredentialProviderStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        if (_requiresLinux
            && (!OperatingSystem.IsLinux()
                || string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DBUS_SESSION_BUS_ADDRESS"))))
        {
            return new CredentialProviderStatus(ProviderId, CredentialProviderAvailability.Unavailable);
        }

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new CredentialProviderStatus(
                ProviderId,
                await _backend.GetAvailabilityAsync(cancellationToken).ConfigureAwait(false));
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
        EnsureSupportedPlatform();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            byte[]? secretBytes = await _backend.GetAsync(GetCredentialKey(id), cancellationToken).ConfigureAwait(false);
            if (secretBytes is null)
            {
                return null;
            }

            char[] characters = [];
            try
            {
                characters = new char[s_utf8.GetCharCount(secretBytes)];
                s_utf8.GetChars(secretBytes, characters);
                return new CredentialSecret(characters);
            }
            catch (DecoderFallbackException exception)
            {
                throw new InvalidDataException(
                    "The stored Secret Service credential is not valid UTF-8. Delete and recreate it.",
                    exception);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(secretBytes);
                CryptographicOperations.ZeroMemory(System.Runtime.InteropServices.MemoryMarshal.AsBytes(characters.AsSpan()));
            }
        }
        finally
        {
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
        EnsureSupportedPlatform();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            byte[] secretBytes = new byte[s_utf8.GetByteCount(secret.Reveal())];
            try
            {
                s_utf8.GetBytes(secret.Reveal(), secretBytes);
                cancellationToken.ThrowIfCancellationRequested();
                await _backend.SetAsync(GetCredentialKey(id), secretBytes, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(secretBytes);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc/>
    public async ValueTask DeleteAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        EnsureSupportedPlatform();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _backend.DeleteAsync(GetCredentialKey(id), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    private static string GetCredentialKey(CredentialId id)
    {
        string identity = string.Concat(id.ServiceId, "\0", id.Name, "\0", id.ScopeId ?? string.Empty);
        byte[] identityBytes = Encoding.UTF8.GetBytes(identity);
        try
        {
            return Convert.ToHexString(SHA256.HashData(identityBytes));
        }
        finally
        {
            CryptographicOperations.ZeroMemory(identityBytes);
        }
    }

    private void EnsureSupportedPlatform()
    {
        if (_requiresLinux && !OperatingSystem.IsLinux())
        {
            throw new PlatformNotSupportedException("Linux Secret Service is available only on Linux.");
        }
    }
}
