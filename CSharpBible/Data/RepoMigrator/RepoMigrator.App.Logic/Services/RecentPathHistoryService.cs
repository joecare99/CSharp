using RepoMigrator.Core;

namespace RepoMigrator.App.Logic.Services;

/// <summary>
/// Maintains recent path histories grouped by repository type.
/// </summary>
public sealed class RecentPathHistoryService
{
    /// <summary>
    /// Adds a recent path for the supplied repository type.
    /// </summary>
    public IReadOnlyList<RepoTypeRecentValues> AddPath(IReadOnlyList<RepoTypeRecentValues> lstEntries, RepoType repoType, string? sPath)
        => RepoTypeRecentValueHistory.AddValue(lstEntries, repoType, sPath);

    /// <summary>
    /// Returns the recent paths for the supplied repository type.
    /// </summary>
    public IReadOnlyList<string> GetPaths(IReadOnlyList<RepoTypeRecentValues> lstEntries, RepoType repoType)
        => RepoTypeRecentValueHistory.GetValues(lstEntries, repoType);
}
