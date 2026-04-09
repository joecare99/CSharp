namespace RepoMigrator.Tools.GitBranchSplitter;

/// <summary>
/// Creates branch plans for a repository snapshot by grouping tracked paths into first-level and second-level branches.
/// </summary>
public static class GitPathBranchSplitPlanner
{
    /// <summary>
    /// Builds branch plans for the supplied tracked paths.
    /// </summary>
    /// <param name="lstTrackedPaths">The tracked paths from the source branch snapshot.</param>
    /// <param name="sBranchPrefix">The prefix that should be prepended to all generated branch names.</param>
    /// <returns>The generated branch plans ordered by branch name.</returns>
    public static IReadOnlyList<BranchSplitPlan> BuildPlans(IEnumerable<string> lstTrackedPaths, string sBranchPrefix)
    {
        var dicPlans = new SortedDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var sRawPath in lstTrackedPaths)
        {
            var sPath = NormalizePath(sRawPath);
            if (string.IsNullOrWhiteSpace(sPath))
                continue;

            var arrSegments = sPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (arrSegments.Length == 0)
                continue;

            if (arrSegments.Length == 1)
            {
                AddPath(dicPlans, ComposeBranchName(sBranchPrefix, "_root"), sPath);
                continue;
            }

            AddPath(dicPlans, ComposeBranchName(sBranchPrefix, arrSegments[0]), sPath);

            if (arrSegments.Length >= 3)
                AddPath(dicPlans, ComposeBranchName(sBranchPrefix, arrSegments[0], arrSegments[1]), sPath);
        }

        return dicPlans
            .Select(kvp => new BranchSplitPlan(kvp.Key, kvp.Value))
            .ToList();
    }

    private static void AddPath(IDictionary<string, HashSet<string>> dicPlans, string sBranchName, string sPath)
    {
        if (!dicPlans.TryGetValue(sBranchName, out var hsPaths))
        {
            hsPaths = new HashSet<string>(StringComparer.Ordinal);
            dicPlans[sBranchName] = hsPaths;
        }

        hsPaths.Add(sPath);
    }

    private static string NormalizePath(string sPath)
        => sPath.Replace('\\', '/').Trim('/');

    private static string ComposeBranchName(string sBranchPrefix, params string[] arrSegments)
    {
        var arrBranchSegments = sBranchPrefix
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Concat(arrSegments)
            .Select(SanitizeBranchSegment)
            .Where(static sSegment => !string.IsNullOrWhiteSpace(sSegment))
            .ToArray();

        if (arrBranchSegments.Length == 0)
            throw new InvalidOperationException("The generated branch name must not be empty.");

        return string.Join('/', arrBranchSegments);
    }

    private static string SanitizeBranchSegment(string sSegment)
    {
        var sTrimmedSegment = sSegment.Trim();
        if (string.IsNullOrWhiteSpace(sTrimmedSegment))
            return "_";

        var arrChars = sTrimmedSegment
            .Select(ch => char.IsLetterOrDigit(ch) || ch is '-' or '_' or '.' ? ch : '-')
            .ToArray();

        var sSanitized = new string(arrChars).Trim('.');
        while (sSanitized.Contains("..", StringComparison.Ordinal))
            sSanitized = sSanitized.Replace("..", ".", StringComparison.Ordinal);

        return string.IsNullOrWhiteSpace(sSanitized) ? "_" : sSanitized;
    }
}
