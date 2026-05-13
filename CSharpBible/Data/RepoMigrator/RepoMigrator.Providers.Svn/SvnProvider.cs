// RepoMigrator.Providers.Svn/SvnProvider.cs
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.SvnCli;

namespace RepoMigrator.Providers.Svn;

public sealed class SvnProvider : IVersionControlProvider
{
    private readonly SvnCliProvider _innerProvider = new();

    public string Name => _innerProvider.Name;
    public bool SupportsRead => _innerProvider.SupportsRead;
    public bool SupportsWrite => _innerProvider.SupportsWrite;

    public Task OpenAsync(RepositoryEndpoint endpoint, CancellationToken ct)
        => _innerProvider.OpenAsync(endpoint, ct);

    public Task<RepositoryCapabilities> GetCapabilitiesAsync(RepositoryEndpoint endpoint, CancellationToken ct)
        => _innerProvider.GetCapabilitiesAsync(endpoint, ct);

    public Task<RepositorySelectionData> GetSelectionDataAsync(RepositoryEndpoint endpoint, CancellationToken ct)
        => _innerProvider.GetSelectionDataAsync(endpoint, ct);

    public Task<RepositoryProbeResult> ProbeAsync(RepositoryEndpoint endpoint, RepositoryAccessMode accessMode, CancellationToken ct)
        => _innerProvider.ProbeAsync(endpoint, accessMode, ct);

    public Task TransferAsync(RepositoryEndpoint source, RepositoryEndpoint target, MigrationOptions options, IMigrationProgress progress, CancellationToken ct)
        => _innerProvider.TransferAsync(source, target, options, progress, ct);

    public Task<IReadOnlyList<ChangeSetInfo>> GetChangeSetsAsync(ChangeSetQuery query, CancellationToken ct)
        => _innerProvider.GetChangeSetsAsync(query, ct);

    public Task MaterializeSnapshotAsync(string workDir, string changeSetId, CancellationToken ct)
        => _innerProvider.MaterializeSnapshotAsync(workDir, changeSetId, ct);

    public Task InitializeTargetAsync(RepositoryEndpoint endpoint, bool emptyInit, CancellationToken ct)
        => _innerProvider.InitializeTargetAsync(endpoint, emptyInit, ct);

    public Task CommitSnapshotAsync(string workDir, CommitMetadata metadata, CancellationToken ct)
        => _innerProvider.CommitSnapshotAsync(workDir, metadata, ct);

    public Task CommitSnapshotAsync(string workDir, CommitMetadata metadata, IMigrationProgress progress, CancellationToken ct)
        => _innerProvider.CommitSnapshotAsync(workDir, metadata, progress, ct);

    public Task FlushAsync(CancellationToken ct)
        => _innerProvider.FlushAsync(ct);

    public ValueTask DisposeAsync()
        => _innerProvider.DisposeAsync();
}
