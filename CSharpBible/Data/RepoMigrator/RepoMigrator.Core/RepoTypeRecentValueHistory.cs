namespace RepoMigrator.Core;

/// <summary>
/// Represents a bounded list of recent values for a specific repository type.
/// </summary>
public sealed class RepoTypeRecentValues
{
    /// <summary>
    /// Gets or sets the repository type that owns the recent values.
    /// </summary>
    public RepoType RepoType { get; init; }

    /// <summary>
    /// Gets or sets the recent values for the associated repository type.
    /// </summary>
    public List<string> Values { get; init; } = [];
}

/// <summary>
/// Provides helper methods for maintaining recent values grouped by repository type.
/// </summary>
public static class RepoTypeRecentValueHistory
{
    /// <summary>
    /// Returns the recent values stored for the specified repository type.
    /// </summary>
    /// <param name="lstEntries">The grouped recent values.</param>
    /// <param name="repoType">The repository type to read.</param>
    /// <returns>The stored values for the requested repository type.</returns>
    public static IReadOnlyList<string> GetValues(IReadOnlyList<RepoTypeRecentValues> lstEntries, RepoType repoType)
        => lstEntries.FirstOrDefault(entry => entry.RepoType == repoType)?.Values ?? [];

    /// <summary>
    /// Adds a value to the recent history for the specified repository type while preserving histories for other types.
    /// </summary>
    /// <param name="lstEntries">The grouped recent values.</param>
    /// <param name="repoType">The repository type to update.</param>
    /// <param name="sValue">The value to add.</param>
    /// <param name="iMaxCount">The maximum number of values to keep per repository type.</param>
    /// <returns>A new grouped history list containing the updated values.</returns>
    public static IReadOnlyList<RepoTypeRecentValues> AddValue(IReadOnlyList<RepoTypeRecentValues> lstEntries, RepoType repoType, string? sValue, int iMaxCount = 10)
    {
        var lstUpdatedEntries = lstEntries
            .Select(entry => new RepoTypeRecentValues
            {
                RepoType = entry.RepoType,
                Values = [.. entry.Values]
            })
            .ToList();

        var recentEntry = lstUpdatedEntries.FirstOrDefault(entry => entry.RepoType == repoType);
        if (recentEntry is null)
        {
            recentEntry = new RepoTypeRecentValues { RepoType = repoType };
            lstUpdatedEntries.Add(recentEntry);
        }

        recentEntry = new RepoTypeRecentValues
        {
            RepoType = repoType,
            Values = [.. RecentValueHistory.AddValue(recentEntry.Values, sValue, iMaxCount)]
        };

        var iIndex = lstUpdatedEntries.FindIndex(entry => entry.RepoType == repoType);
        lstUpdatedEntries[iIndex] = recentEntry;
        return lstUpdatedEntries;
    }
}
