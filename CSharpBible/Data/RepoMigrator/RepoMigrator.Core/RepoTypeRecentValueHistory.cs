namespace RepoMigrator.Core;

/// <summary>
/// Represents a bounded list of recent values for a specific repository provider.
/// </summary>
public sealed class ProviderRecentValues
{
    /// <summary>
    /// Gets or sets the provider key that owns the recent values.
    /// </summary>
    public string ProviderKey { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the recent values for the associated repository type.
    /// </summary>
    public List<string> Values { get; init; } = [];
}

/// <summary>
/// Provides helper methods for maintaining recent values grouped by provider key.
/// </summary>
public static class ProviderRecentValueHistory
{
    /// <summary>
    /// Returns the recent values stored for the specified provider key.
    /// </summary>
    /// <param name="lstEntries">The grouped recent values.</param>
    /// <param name="providerKey">The provider key to read.</param>
    /// <returns>The stored values for the requested repository type.</returns>
    public static IReadOnlyList<string> GetValues(IReadOnlyList<ProviderRecentValues> lstEntries, string providerKey)
        => lstEntries.FirstOrDefault(entry => string.Equals(entry.ProviderKey, providerKey, StringComparison.OrdinalIgnoreCase))?.Values ?? [];

    /// <summary>
    /// Adds a value to the recent history for the specified provider key while preserving histories for other keys.
    /// </summary>
    /// <param name="lstEntries">The grouped recent values.</param>
    /// <param name="providerKey">The provider key to update.</param>
    /// <param name="sValue">The value to add.</param>
    /// <param name="iMaxCount">The maximum number of values to keep per provider key.</param>
    /// <returns>A new grouped history list containing the updated values.</returns>
    public static IReadOnlyList<ProviderRecentValues> AddValue(IReadOnlyList<ProviderRecentValues> lstEntries, string providerKey, string? sValue, int iMaxCount = 10)
    {
        var lstUpdatedEntries = lstEntries
            .Select(entry => new ProviderRecentValues
            {
                ProviderKey = entry.ProviderKey,
                Values = [.. entry.Values]
            })
            .ToList();

        var recentEntry = lstUpdatedEntries.FirstOrDefault(entry => string.Equals(entry.ProviderKey, providerKey, StringComparison.OrdinalIgnoreCase));
        if (recentEntry is null)
        {
            recentEntry = new ProviderRecentValues { ProviderKey = providerKey };
            lstUpdatedEntries.Add(recentEntry);
        }

        recentEntry = new ProviderRecentValues
        {
            ProviderKey = providerKey,
            Values = [.. RecentValueHistory.AddValue(recentEntry.Values, sValue, iMaxCount)]
        };

        var iIndex = lstUpdatedEntries.FindIndex(entry => string.Equals(entry.ProviderKey, providerKey, StringComparison.OrdinalIgnoreCase));
        lstUpdatedEntries[iIndex] = recentEntry;
        return lstUpdatedEntries;
    }
}
