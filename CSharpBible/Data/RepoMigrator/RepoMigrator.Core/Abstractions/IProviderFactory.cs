// RepoMigrator.Core/Abstractions/IProviderFactory.cs
namespace RepoMigrator.Core.Abstractions;

public interface IProviderFactory
{
    IVersionControlProvider Create(RepoType type);
}
