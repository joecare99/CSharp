using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.Config.Secrets;

/// <summary>
/// Provides explicit lookup of registered credential backends without choosing a fallback.
/// </summary>
public sealed class CredentialProviderCatalog
{
    private readonly IReadOnlyDictionary<string, ICredentialProvider> _providers;

    /// <summary>Creates a catalog from the providers available to one host.</summary>
    public CredentialProviderCatalog(IEnumerable<ICredentialProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);
        Dictionary<string, ICredentialProvider> catalog = new(StringComparer.Ordinal);
        foreach (ICredentialProvider provider in providers)
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentException.ThrowIfNullOrWhiteSpace(provider.ProviderId);
            if (!string.Equals(provider.ProviderId, provider.ProviderId.Trim(), StringComparison.Ordinal))
            {
                throw new ArgumentException("Credential provider IDs must be trimmed.", nameof(providers));
            }

            if (!catalog.TryAdd(provider.ProviderId, provider))
            {
                throw new ArgumentException(
                    $"Credential provider ID '{provider.ProviderId}' is registered more than once.",
                    nameof(providers));
            }
        }

        _providers = catalog;
    }

    /// <summary>Gets the explicitly selected provider or fails without falling back.</summary>
    public ICredentialProvider GetRequired(string providerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerId);
        return _providers.TryGetValue(providerId, out ICredentialProvider? provider)
            ? provider
            : throw new KeyNotFoundException($"Credential provider '{providerId}' is not registered.");
    }

    /// <summary>Gets the stable IDs of registered providers.</summary>
    public IReadOnlyList<string> ProviderIds => _providers.Keys.OrderBy(static id => id, StringComparer.Ordinal).ToArray();
}
