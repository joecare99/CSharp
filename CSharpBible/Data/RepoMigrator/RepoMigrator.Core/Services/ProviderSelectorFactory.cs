// RepoMigrator.Core/Services/ProviderSelectorFactory.cs
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Core;

public sealed class ProviderSelectorFactory : IProviderFactory
{
    private readonly IDictionary<RepoType, Func<IVersionControlProvider>> _map;
    public ProviderSelectorFactory(IDictionary<RepoType, Func<IVersionControlProvider>> map) => _map = map;
    public IVersionControlProvider Create(RepoType type)
        => _map.TryGetValue(type, out var f) ? f() : throw new NotSupportedException(type.ToString());
}
