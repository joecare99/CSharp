// RepoMigrator.Core/Abstractions/IVersionControlProvider.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoMigrator.Core.Abstractions;

public interface IVersionControlProvider : IAsyncDisposable
{
    Task OpenAsync(RepositoryEndpoint endpoint, CancellationToken ct);
    Task<IReadOnlyList<ChangeSetInfo>> GetChangeSetsAsync(ChangeSetQuery query, CancellationToken ct);

    Task MaterializeSnapshotAsync(string workDir, string changeSetId, CancellationToken ct);

    Task InitializeTargetAsync(string targetPath, bool emptyInit, CancellationToken ct);
    Task CommitSnapshotAsync(string workDir, CommitMetadata metadata, CancellationToken ct);

    string Name { get; }
    bool SupportsRead { get; }
    bool SupportsWrite { get; }
}


