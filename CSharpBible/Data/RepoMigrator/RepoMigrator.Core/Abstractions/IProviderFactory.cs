// RepoMigrator.Core/Abstractions/IProviderFactory.cs
using System;

namespace RepoMigrator.Core.Abstractions;

public interface IProviderFactory
{
    IVersionControlProvider Create(RepoType type);
}
