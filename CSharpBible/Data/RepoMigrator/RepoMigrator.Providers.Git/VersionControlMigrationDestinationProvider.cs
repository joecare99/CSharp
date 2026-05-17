using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Providers.Git;

/// <summary>
/// Bridges normalized repository-backed destination definitions to Git target behavior.
/// </summary>
public sealed class VersionControlMigrationDestinationProvider : IMigrationDestinationProvider, IMigrationDestinationRefOperations, IGitTargetRefOperations
{
    private readonly IVersionControlProvider _versionControlProvider;
    private readonly IGitTargetRefOperations _gitTargetRefOperations;
    private RepositoryEndpoint? _endpoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionControlMigrationDestinationProvider"/> class.
    /// </summary>
    /// <param name="versionControlProvider">The repository provider used to initialize and write target snapshots.</param>
    public VersionControlMigrationDestinationProvider(IVersionControlProvider versionControlProvider)
    {
        _versionControlProvider = versionControlProvider ?? throw new ArgumentNullException(nameof(versionControlProvider));
        _gitTargetRefOperations = versionControlProvider as IGitTargetRefOperations
            ?? throw new ArgumentException("The supplied version control provider must support Git target ref operations.", nameof(versionControlProvider));
    }

    /// <inheritdoc/>
    public string Name => $"Repository destination ({_versionControlProvider.Name})";

    /// <inheritdoc/>
    public bool CanHandle(MigrationDestinationDefinition destination)
        => destination is not null
           && destination.Kind == MigrationDestinationKind.Repository
           && destination.Repository?.Type == RepoType.Git;

    /// <inheritdoc/>
    public async Task InitializeAsync(MigrationDestinationDefinition destination, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!CanHandle(destination))
            throw new NotSupportedException("The supplied destination definition is not a Git-backed repository destination.");

        _endpoint = destination.Repository!;
        await _versionControlProvider.InitializeTargetAsync(_endpoint, emptyInit: true, ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task WriteSnapshotAsync(string workDir, MigrationDestinationCommit metadata, IMigrationProgress progress, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workDir);
        ArgumentNullException.ThrowIfNull(metadata);
        EnsureInitialized();

        return _versionControlProvider.CommitSnapshotAsync(workDir, new CommitMetadata
        {
            Message = metadata.Message,
            AuthorName = metadata.AuthorName,
            AuthorEmail = metadata.AuthorEmail,
            Timestamp = metadata.Timestamp,
            TargetBranch = metadata.DestinationReference ?? _endpoint!.BranchOrTrunk
        }, progress, ct);
    }

    /// <inheritdoc/>
    public Task FinalizeAsync(CancellationToken ct)
    {
        EnsureInitialized();
        return _versionControlProvider.FlushAsync(ct);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
        => _versionControlProvider.DisposeAsync();

    /// <inheritdoc/>
    public Task<string?> GetHeadCommitIdAsync(CancellationToken ct)
        => _gitTargetRefOperations.GetHeadCommitIdAsync(ct);

    /// <inheritdoc/>
    public Task<bool> BranchExistsAsync(string branchName, CancellationToken ct)
        => _gitTargetRefOperations.BranchExistsAsync(branchName, ct);

    /// <inheritdoc/>
    public Task<bool> TagExistsAsync(string tagName, CancellationToken ct)
        => _gitTargetRefOperations.TagExistsAsync(tagName, ct);

    /// <inheritdoc/>
    public Task EnsureBranchAsync(string branchName, string commitId, CancellationToken ct)
        => _gitTargetRefOperations.EnsureBranchAsync(branchName, commitId, ct);

    /// <inheritdoc/>
    public Task EnsureTagAsync(string tagName, string commitId, CancellationToken ct)
        => _gitTargetRefOperations.EnsureTagAsync(tagName, commitId, ct);

    private void EnsureInitialized()
    {
        if (_endpoint is null)
            throw new InvalidOperationException("The destination provider has not been initialized.");
    }
}
