using RepoMigrator.Core;

namespace RepoMigrator.App.Logic.Services;

/// <summary>
/// Creates repository endpoints from application input values.
/// </summary>
public sealed class MigrationEndpointFactory
{
    /// <summary>
    /// Creates a source endpoint.
    /// </summary>
    public RepositoryEndpoint CreateSourceEndpoint(RepoType repoType, string sUrlOrPath, string? sBranchOrTrunk, string? sUser, string? sPassword)
        => new()
        {
            Type = repoType,
            UrlOrPath = sUrlOrPath,
            BranchOrTrunk = sBranchOrTrunk,
            Credentials = new RepoCredentials { Username = sUser, Password = sPassword }
        };

    /// <summary>
    /// Creates a target endpoint.
    /// </summary>
    public RepositoryEndpoint CreateTargetEndpoint(RepoType repoType, string sUrlOrPath, string? sBranchOrTrunk, string? sUser, string? sPassword)
        => new()
        {
            Type = repoType,
            UrlOrPath = sUrlOrPath,
            BranchOrTrunk = sBranchOrTrunk,
            Credentials = new RepoCredentials { Username = sUser, Password = sPassword }
        };
}
