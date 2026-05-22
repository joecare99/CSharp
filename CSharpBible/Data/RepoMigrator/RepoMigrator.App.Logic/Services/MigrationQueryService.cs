using RepoMigrator.App.Logic.Models;
using RepoMigrator.Core;

namespace RepoMigrator.App.Logic.Services;

/// <summary>
/// Builds migration queries and resume values from application state.
/// </summary>
public sealed class MigrationQueryService
{
    public const string SvnProviderKey = "svn";

    /// <summary>
    /// Creates the changeset query for the current application state.
    /// </summary>
    public ChangeSetQuery CreateQuery(
        string sourceProviderKey,
        string? sFromId,
        string? sToId,
        int? iMaxCount,
        bool xOldestFirst,
        IReadOnlyList<RepositoryRevisionInfo> lstSvnRevisionSelections,
        string? sSelectedSvnFromRevisionId,
        string? sSelectedSvnToRevisionId)
    {
        if (string.Equals(sourceProviderKey, SvnProviderKey, StringComparison.OrdinalIgnoreCase))
        {
            var sFromExclusiveId = !string.IsNullOrWhiteSpace(sSelectedSvnFromRevisionId)
                && !string.Equals(sSelectedSvnFromRevisionId, sFromId, StringComparison.OrdinalIgnoreCase)
                    ? SvnRevisionRangeResolver.ResolveFromExclusiveId(lstSvnRevisionSelections, sSelectedSvnFromRevisionId)
                    : sFromId;

            return new ChangeSetQuery
            {
                FromExclusiveId = sFromExclusiveId,
                ToInclusiveId = SvnRevisionRangeResolver.ResolveToInclusiveId(sSelectedSvnToRevisionId),
                MaxCount = iMaxCount,
                OldestFirst = xOldestFirst
            };
        }

        return new ChangeSetQuery
        {
            FromExclusiveId = sFromId,
            ToInclusiveId = sToId,
            MaxCount = iMaxCount,
            OldestFirst = xOldestFirst
        };
    }

    /// <summary>
    /// Creates updated resume values after a successful commit.
    /// </summary>
    public ResumeUpdateResult UpdateResumeAfterCommit(string sourceProviderKey, string? sCurrentChangeSetId, IReadOnlyList<RepositoryRevisionInfo> lstSvnRevisionSelections)
    {
        if (string.IsNullOrWhiteSpace(sCurrentChangeSetId))
            return new ResumeUpdateResult();

        if (!string.Equals(sourceProviderKey, SvnProviderKey, StringComparison.OrdinalIgnoreCase))
            return new ResumeUpdateResult { FromExclusiveId = sCurrentChangeSetId };

        var sNextRevisionId = SvnRevisionRangeResolver.GetNextRevisionId(lstSvnRevisionSelections, sCurrentChangeSetId);
        return new ResumeUpdateResult
        {
            FromExclusiveId = sCurrentChangeSetId,
            SelectedSvnFromRevisionId = sNextRevisionId ?? sCurrentChangeSetId
        };
    }
}
