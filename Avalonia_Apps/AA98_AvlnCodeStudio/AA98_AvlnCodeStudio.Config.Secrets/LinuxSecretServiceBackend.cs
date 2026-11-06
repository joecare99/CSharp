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
    private SecretService? _service;
    private Collection? _collection;

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
        catch (DBusErrorReplyException)
        {
            return CredentialProviderAvailability.Unavailable;
        }
        catch (DBusConnectFailedException)
        {
            return CredentialProviderAvailability.Unavailable;
        }
        catch (IOException)
        {
            return CredentialProviderAvailability.Unavailable;
        }
        catch (SocketException)
        {
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
            Collection? collection = await GetDefaultCollectionAsync().ConfigureAwait(false);
            if (collection is null)
            {
                return null;
            }

            await EnsureUnlockedAsync(collection).ConfigureAwait(false);
            Item? item = GetSingleItem(await collection.SearchItemsAsync(CreateAttributes(credentialKey)).ConfigureAwait(false));
            if (item is null)
            {
                return null;
            }

            await EnsureUnlockedAsync(item).ConfigureAwait(false);
            return await item.GetSecretAsync().ConfigureAwait(false);
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
            Collection? collection = await GetDefaultCollectionAsync().ConfigureAwait(false);
            if (collection is null)
            {
                return;
            }

            await EnsureUnlockedAsync(collection).ConfigureAwait(false);
            Item? item = GetSingleItem(await collection.SearchItemsAsync(CreateAttributes(credentialKey)).ConfigureAwait(false));
            if (item is not null)
            {
                await EnsureUnlockedAsync(item).ConfigureAwait(false);
                await item.DeleteAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<Collection?> GetDefaultCollectionAsync()
    {
        if (_service is null)
        {
            _service = await SecretService.ConnectAsync(EncryptionType.Dh).ConfigureAwait(false);
            _collection = await _service.GetDefaultCollectionAsync().ConfigureAwait(false);
        }

        return _collection;
    }

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
