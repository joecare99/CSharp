using DBus.Services.Secrets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace AA98_AvlnCodeStudio.Config.Secrets;

internal interface ILinuxSecretServiceBackend
{
    ValueTask<CredentialProviderAvailability> GetAvailabilityAsync(CancellationToken cancellationToken);
    ValueTask<byte[]?> GetAsync(string credentialKey, CancellationToken cancellationToken);
    ValueTask SetAsync(string credentialKey, ReadOnlyMemory<byte> secret, CancellationToken cancellationToken);
    ValueTask DeleteAsync(string credentialKey, CancellationToken cancellationToken);
}

internal sealed class LinuxSecretServiceBackend : ILinuxSecretServiceBackend
{
    private const string AttributeName = "avlncodestudio.credential-id";
    private const string ContentType = "text/plain; charset=utf-8";
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly Func<Task<Collection?>>? _loadCollection;
    private SecretService? _service;
    private Collection? _collection;

    internal LinuxSecretServiceBackend(Func<Task<Collection?>>? loadCollection = null)
    {
        _loadCollection = loadCollection;
    }

    public async ValueTask<CredentialProviderAvailability> GetAvailabilityAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            Collection? collection = await GetDefaultCollectionAsync().ConfigureAwait(false);
            return collection is null
                ? CredentialProviderAvailability.Unavailable
                : await collection.IsLockedAsync().ConfigureAwait(false)
                    ? CredentialProviderAvailability.Locked
                    : CredentialProviderAvailability.Available;
        }
        catch (Exception exception) when (IsConnectionFailure(exception))
        {
            InvalidateConnection();
            return CredentialProviderAvailability.Unavailable;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask<byte[]?> GetAsync(string credentialKey, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            Collection collection = await GetDefaultCollectionAsync().ConfigureAwait(false)
                ?? throw new InvalidOperationException("The operating-system Secret Service has no default collection.");

            await EnsureUnlockedAsync(collection).ConfigureAwait(false);
            Item? item = GetSingleItem(await collection.SearchItemsAsync(CreateAttributes(credentialKey)).ConfigureAwait(false));
            if (item is null)
            {
                return null;
            }

            await EnsureUnlockedAsync(item).ConfigureAwait(false);
            return await item.GetSecretAsync().ConfigureAwait(false);
        }
        catch (Exception exception) when (IsConnectionFailure(exception))
        {
            InvalidateConnection();
            throw;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask SetAsync(
        string credentialKey,
        ReadOnlyMemory<byte> secret,
        CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            Collection collection = await GetDefaultCollectionAsync().ConfigureAwait(false)
                ?? throw new InvalidOperationException("The operating-system Secret Service has no default collection.");
            await EnsureUnlockedAsync(collection).ConfigureAwait(false);

            Dictionary<string, string> attributes = CreateAttributes(credentialKey);
            Item? existingItem = GetSingleItem(await collection.SearchItemsAsync(attributes).ConfigureAwait(false));
            if (existingItem is not null)
            {
                await EnsureUnlockedAsync(existingItem).ConfigureAwait(false);
                byte[] secretBytes = secret.ToArray();
                try
                {
                    await existingItem.SetSecret(secretBytes, ContentType).ConfigureAwait(false);
                }
                finally
                {
                    CryptographicOperations.ZeroMemory(secretBytes);
                }

                return;
            }

            byte[] newSecretBytes = secret.ToArray();
            try
            {
                Item? created = await collection.CreateItemAsync(
                    "AvlnCodeStudio credential",
                    attributes,
                    newSecretBytes,
                    ContentType,
                    replace: false).ConfigureAwait(false);
                if (created is null)
                {
                    throw new InvalidOperationException(
                        "The Secret Service did not store the credential; the operation may have been cancelled or the collection is unavailable.");
                }
            }
            finally
            {
                CryptographicOperations.ZeroMemory(newSecretBytes);
            }
        }
        catch (Exception exception) when (IsConnectionFailure(exception))
        {
            InvalidateConnection();
            throw;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask DeleteAsync(string credentialKey, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            Collection collection = await GetDefaultCollectionAsync().ConfigureAwait(false)
                ?? throw new InvalidOperationException("The operating-system Secret Service has no default collection.");

            await EnsureUnlockedAsync(collection).ConfigureAwait(false);
            Item? item = GetSingleItem(await collection.SearchItemsAsync(CreateAttributes(credentialKey)).ConfigureAwait(false));
            if (item is not null)
            {
                await EnsureUnlockedAsync(item).ConfigureAwait(false);
                await item.DeleteAsync().ConfigureAwait(false);
            }
        }
        catch (Exception exception) when (IsConnectionFailure(exception))
        {
            InvalidateConnection();
            throw;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<Collection?> GetDefaultCollectionAsync()
    {
        if (_loadCollection is not null)
        {
            return await _loadCollection().ConfigureAwait(false);
        }

        if (_collection is null)
        {
            SecretService service = _service ?? await SecretService.ConnectAsync(EncryptionType.Dh).ConfigureAwait(false);
            Collection? collection = await service.GetDefaultCollectionAsync().ConfigureAwait(false);
            _service = service;
            _collection = collection;
        }

        return _collection;
    }

    private void InvalidateConnection()
    {
        _service = null;
        _collection = null;
    }

    private static bool IsConnectionFailure(Exception exception) =>
        exception is DBusErrorReplyException or DBusConnectFailedException or IOException or SocketException;

    private static async Task EnsureUnlockedAsync(Collection collection)
    {
        if (await collection.IsLockedAsync().ConfigureAwait(false))
        {
            throw new CredentialProviderLockedException("linux-secret-service");
        }
    }

    private static async Task EnsureUnlockedAsync(Item item)
    {
        if (await item.IsLockedAsync().ConfigureAwait(false))
        {
            throw new CredentialProviderLockedException("linux-secret-service");
        }
    }

    private static Dictionary<string, string> CreateAttributes(string credentialKey) => new()
    {
        [AttributeName] = credentialKey,
    };

    private static Item? GetSingleItem(IReadOnlyCollection<Item> items)
    {
        return items.Count switch
        {
            0 => null,
            1 => items.Single(),
            _ => throw new InvalidDataException("Multiple Secret Service items match this credential ID; resolve the duplicates before retrying."),
        };
    }
}
