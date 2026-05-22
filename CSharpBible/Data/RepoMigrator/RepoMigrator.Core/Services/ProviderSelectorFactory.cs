// RepoMigrator.Core/Services/ProviderSelectorFactory.cs
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Core;

public sealed class ProviderSelectorFactory : IProviderFactory
{
    private readonly IDictionary<string, Func<IVersionControlProvider>> _map;

    public ProviderSelectorFactory(IDictionary<string, Func<IVersionControlProvider>> map)
        => _map = map ?? throw new ArgumentNullException(nameof(map));

    public IVersionControlProvider Create(string providerKey)
        => _map.TryGetValue(providerKey, out var f) ? f() : throw new NotSupportedException(providerKey);
}
