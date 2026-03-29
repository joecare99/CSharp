namespace RepoMigrator.Core;

/// <summary>
/// Resolves UI-facing SVN revision selections into the existing migration query boundaries.
/// </summary>
public static class SvnRevisionRangeResolver
{
    /// <summary>
    /// Returns the first available revision id that should be preselected for the inclusive SVN start selection.
    /// </summary>
    /// <param name="lstRevisions">The ordered revision list for the selected SVN path.</param>
    /// <returns>The first revision id if available; otherwise <see langword="null" />.</returns>
    public static string? GetSuggestedFromRevisionId(IReadOnlyList<RepositoryRevisionInfo> lstRevisions)
        => lstRevisions.Count == 0 ? null : lstRevisions[0].Id;

    /// <summary>
    /// Converts an inclusive SVN start selection into the exclusive lower boundary used by <see cref="ChangeSetQuery" />.
    /// </summary>
    /// <param name="lstRevisions">The ordered revision list for the selected SVN path.</param>
    /// <param name="sFromRevisionId">The inclusive SVN start revision selected in the UI.</param>
    /// <returns>The exclusive previous revision id, or <see langword="null" /> when the first revision should be included.</returns>
    public static string? ResolveFromExclusiveId(IReadOnlyList<RepositoryRevisionInfo> lstRevisions, string? sFromRevisionId)
    {
        if (string.IsNullOrWhiteSpace(sFromRevisionId) || lstRevisions.Count == 0)
            return null;

        for (var iIndex = 0; iIndex < lstRevisions.Count; iIndex++)
        {
            if (!string.Equals(lstRevisions[iIndex].Id, sFromRevisionId, StringComparison.OrdinalIgnoreCase))
                continue;

            return iIndex == 0 ? null : lstRevisions[iIndex - 1].Id;
        }

        return null;
    }

    /// <summary>
    /// Normalizes the inclusive SVN end selection. An empty value means HEAD and therefore maps to <see langword="null" />.
    /// </summary>
    /// <param name="sToRevisionId">The inclusive SVN end revision selected in the UI.</param>
    /// <returns>The selected inclusive revision id, or <see langword="null" /> for HEAD.</returns>
    public static string? ResolveToInclusiveId(string? sToRevisionId)
        => string.IsNullOrWhiteSpace(sToRevisionId) ? null : sToRevisionId;
}
