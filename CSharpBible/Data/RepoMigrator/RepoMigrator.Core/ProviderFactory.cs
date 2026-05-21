// RepoMigrator.Core/ProviderFactory.cs
using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Core;

public sealed class ProviderFactory : IProviderFactory
{
    private readonly IServiceProvider _sp;
    public ProviderFactory(IServiceProvider sp) => _sp = sp;

    public IVersionControlProvider Create(string providerKey)
        => providerKey switch
        {
            "git" => _sp.GetRequiredService<IVersionControlProvider>() as dynamic, // wird via named registrations gemappt
            "svn" => _sp.GetRequiredService<IEnumerable<IVersionControlProvider>>().First(p => p.GetType().Name.Contains("Svn")),
            _ => throw new NotSupportedException(providerKey)
        };
}
