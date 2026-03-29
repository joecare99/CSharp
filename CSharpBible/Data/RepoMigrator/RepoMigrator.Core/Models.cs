// RepoMigrator.Core/Models.cs
namespace RepoMigrator.Core;

public enum RepoType { Git, Svn, Custom }

public enum RepositoryAccessMode { Read, Write }

/// <summary>
/// Defines the migration strategy that should be used for a repository transfer.
/// </summary>
public enum RepositoryTransferMode
{
    LinearSnapshots,
    NativeHistory
}

public sealed class RepositoryEndpoint
{
    public RepoType Type { get; init; }
    public string UrlOrPath { get; init; } = "";
    public string? BranchOrTrunk { get; init; } // e.g. "main", "trunk", or null
    public RepoCredentials? Credentials { get; init; }
}

public sealed class RepoCredentials
{
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string? PrivateKeyPath { get; init; }     // optional for SSH
    public string? PrivateKeyPassphrase { get; init; }
}

public sealed class ChangeSetInfo
{
    public string Id { get; init; } = "";
    public string Message { get; init; } = "";
    public string AuthorName { get; init; } = "";
    public string? AuthorEmail { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}

public sealed class ChangeSetQuery
{
    public string? FromExclusiveId { get; init; }  // start after this id
    public string? ToInclusiveId { get; init; }    // stop at this id
    public int? MaxCount { get; init; }            // optional cap
    public bool OldestFirst { get; init; } = true;
}

public sealed class CommitMetadata
{
    public string Message { get; init; } = "";
    public string AuthorName { get; init; } = "";
    public string? AuthorEmail { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}

public sealed class RepositoryProbeResult
{
    public bool Success { get; init; }
    public string Summary { get; init; } = "";
    public IReadOnlyList<string> Details { get; init; } = Array.Empty<string>();
}

/// <summary>
/// Describes feature support of a repository provider for a specific endpoint.
/// </summary>
public sealed class RepositoryCapabilities
{
    public bool SupportsNativeHistoryTransfer { get; init; }
    public bool SupportsBranchSelection { get; init; }
    public bool SupportsTagSelection { get; init; }
    public bool SupportsMergeTopology { get; init; }
    public bool SupportsRevisionSelection { get; init; }
}

/// <summary>
/// Provides selectable repository references for UI-driven migrations.
/// </summary>
public sealed class RepositorySelectionData
{
    public string? DefaultBranch { get; init; }
    public string? SuggestedFromRevisionId { get; init; }
    public string? SuggestedToRevisionId { get; init; }
    public IReadOnlyList<RepositoryReferenceInfo> Branches { get; init; } = Array.Empty<RepositoryReferenceInfo>();
    public IReadOnlyList<RepositoryReferenceInfo> Tags { get; init; } = Array.Empty<RepositoryReferenceInfo>();
    public IReadOnlyList<RepositoryRevisionInfo> Revisions { get; init; } = Array.Empty<RepositoryRevisionInfo>();
}

/// <summary>
/// Represents a branch or tag that can be selected for migration.
/// </summary>
public sealed class RepositoryReferenceInfo
{
    public string Name { get; init; } = "";
    public string? CommitId { get; init; }
}

/// <summary>
/// Represents a selectable SVN revision for a path-scoped migration.
/// </summary>
public sealed class RepositoryRevisionInfo
{
    public string Id { get; init; } = "";
    public string Message { get; init; } = "";
    public string AuthorName { get; init; } = "";
    public DateTimeOffset Timestamp { get; init; }
}

/// <summary>
/// Captures migration options that are independent from source-control-specific endpoints.
/// </summary>
public sealed class MigrationOptions
{
    public RepositoryTransferMode TransferMode { get; init; } = RepositoryTransferMode.LinearSnapshots;
    public bool TransferBranches { get; init; }
    public bool TransferTags { get; init; }
    public IReadOnlyList<string> SelectedBranches { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> SelectedTags { get; init; } = Array.Empty<string>();
}