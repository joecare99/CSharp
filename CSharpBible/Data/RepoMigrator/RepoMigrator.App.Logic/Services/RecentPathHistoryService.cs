using RepoMigrator.Core;

namespace RepoMigrator.App.Logic.Services;

/// <summary>
/// Maintains recent path histories grouped by provider key.
/// </summary>
public sealed class RecentPathHistoryService
{
    /// <summary>
    /// Adds a recent path for the supplied provider key.
    /// </summary>
    public IReadOnlyList<ProviderRecentValues> AddPath(IReadOnlyList<ProviderRecentValues> lstEntries, string providerKey, string? sPath)
        => ProviderRecentValueHistory.AddValue(lstEntries, providerKey, sPath);

    /// <summary>
    /// Returns the recent paths for the supplied provider key.
    /// </summary>
    public IReadOnlyList<string> GetPaths(IReadOnlyList<ProviderRecentValues> lstEntries, string providerKey)
        => ProviderRecentValueHistory.GetValues(lstEntries, providerKey);
}
