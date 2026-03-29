namespace RepoMigrator.Core;

/// <summary>
/// Resolves a non-conflicting Git reference name by applying the agreed repository suffix rules.
/// </summary>
public static class GitReferenceNameResolver
{
    /// <summary>
    /// Returns the first available name using the original value, then the <c>-import</c> suffix,
    /// and finally a date-based suffix in <c>yyyyMMdd</c> format. If all of these names already
    /// exist, a numeric suffix is appended to the dated variant.
    /// </summary>
    /// <param name="sBaseName">The desired branch or tag name.</param>
    /// <param name="lstExistingNames">The existing branch or tag names to avoid.</param>
    /// <param name="dtToday">The date that should be used for the dated suffix.</param>
    /// <returns>A non-conflicting Git reference name.</returns>
    public static string ResolveAvailableName(string sBaseName, IReadOnlyCollection<string> lstExistingNames, DateOnly dtToday)
    {
        var hsExistingNames = new HashSet<string>(lstExistingNames, StringComparer.OrdinalIgnoreCase);
        if (!hsExistingNames.Contains(sBaseName))
            return sBaseName;

        var sImportedName = $"{sBaseName}-import";
        if (!hsExistingNames.Contains(sImportedName))
            return sImportedName;

        var sDatedName = $"{sImportedName}-{dtToday:yyyyMMdd}";
        if (!hsExistingNames.Contains(sDatedName))
            return sDatedName;

        var iSuffix = 2;
        while (hsExistingNames.Contains($"{sDatedName}-{iSuffix}"))
            iSuffix++;

        return $"{sDatedName}-{iSuffix}";
    }
}
