namespace RepoMigrator.Core;

/// <summary>
/// Provides helper methods for maintaining a bounded list of recent string values.
/// </summary>
public static class RecentValueHistory
{
    /// <summary>
    /// Adds the supplied value to the front of the history, removes duplicates, and limits the result size.
    /// </summary>
    /// <param name="lstExistingValues">The currently stored values.</param>
    /// <param name="sNewValue">The new value that should be added.</param>
    /// <param name="iMaxCount">The maximum number of values to keep.</param>
    /// <returns>A new ordered list containing the recent values.</returns>
    public static IReadOnlyList<string> AddValue(IReadOnlyList<string> lstExistingValues, string? sNewValue, int iMaxCount = 10)
    {
        var lstRecentValues = new List<string>(iMaxCount);
        if (!string.IsNullOrWhiteSpace(sNewValue))
            lstRecentValues.Add(sNewValue.Trim());

        foreach (var sExistingValue in lstExistingValues)
        {
            if (string.IsNullOrWhiteSpace(sExistingValue))
                continue;

            var sTrimmedValue = sExistingValue.Trim();
            if (lstRecentValues.Contains(sTrimmedValue, StringComparer.OrdinalIgnoreCase))
                continue;

            lstRecentValues.Add(sTrimmedValue);
            if (lstRecentValues.Count == iMaxCount)
                break;
        }

        return lstRecentValues;
    }
}
