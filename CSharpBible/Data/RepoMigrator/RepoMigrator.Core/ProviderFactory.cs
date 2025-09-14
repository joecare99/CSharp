// RepoMigrator.Core/ProviderFactory.cs
using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Core;

public sealed class ProviderFactory : IProviderFactory
{
    private readonly IServiceProvider _sp;
    public ProviderFactory(IServiceProvider sp) => _sp = sp;

    public IVersionControlProvider Create(RepoType type)
        => type switch
        {
            RepoType.Git => _sp.GetRequiredService<IVersionControlProvider>() as dynamic, // wird via named registrations gemappt
            RepoType.Svn => _sp.GetRequiredService<IEnumerable<IVersionControlProvider>>().First(p => p.GetType().Name.Contains("Svn")),
            _ => throw new NotSupportedException(type.ToString())
        };
}
