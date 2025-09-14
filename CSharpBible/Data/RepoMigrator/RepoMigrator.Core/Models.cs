// RepoMigrator.Core/Models.cs
namespace RepoMigrator.Core;

public enum RepoType { Git, Svn, Custom }

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