namespace RepoMigrator.Core;

/// <summary>
/// Builds branch projection plans from tracked snapshot paths using the configured root branch and directory depth.
/// </summary>
public static class SubdirectoryBranchProjectionPlanner
{
    /// <summary>
    /// Builds branch projection plans for the supplied tracked paths.
    /// </summary>
    /// <param name="lstTrackedPaths">The tracked paths from a snapshot.</param>
    /// <param name="sRootBranchName">The root target branch name.</param>
    /// <param name="iDepth">The number of directory levels that should be appended below the root branch.</param>
    /// <returns>The generated branch projection plans ordered by branch name.</returns>
    public static IReadOnlyList<SubdirectoryBranchProjectionPlan> BuildPlans(IEnumerable<string> lstTrackedPaths, string sRootBranchName, int iDepth)
    {
        if (string.IsNullOrWhiteSpace(sRootBranchName))
            throw new ArgumentException("The root branch name must be provided.", nameof(sRootBranchName));

        if (iDepth is < 1 or > 2)
            throw new ArgumentOutOfRangeException(nameof(iDepth), "The supported branch split depth is 1 or 2.");

        var dctPlans = new SortedDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        var hsRootPaths = new HashSet<string>(StringComparer.Ordinal);
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
                hsRootPaths.Add(sPath);
                continue;
            }

            var iDirectorySegments = Math.Min(iDepth, arrSegments.Length - 1);
            AddPath(dctPlans, ComposeBranchName(sRootBranchName, arrSegments.Take(iDirectorySegments).ToArray()), sPath);
        }

        if (hsRootPaths.Count > 0)
        {
            var sRootContentBranchName = dctPlans.Count == 0
                ? ComposeBranchName(sRootBranchName)
                : ResolveRootContentBranchName(sRootBranchName, dctPlans.Keys);

            foreach (var sPath in hsRootPaths)
                AddPath(dctPlans, sRootContentBranchName, sPath);
        }

        return dctPlans
            .Select(kvp => new SubdirectoryBranchProjectionPlan(kvp.Key, kvp.Value))
            .ToList();
    }

    private static string ResolveRootContentBranchName(string sRootBranchName, IEnumerable<string> lstExistingBranchNames)
    {
        var hsExistingBranchNames = lstExistingBranchNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        for (var iSuffix = 0; ; iSuffix++)
        {
            var sCandidateSegment = iSuffix == 0 ? "_root" : $"_root{iSuffix + 1}";
            var sCandidateBranchName = ComposeBranchName(sRootBranchName, sCandidateSegment);
            if (!hsExistingBranchNames.Contains(sCandidateBranchName))
                return sCandidateBranchName;
        }
    }

    private static void AddPath(IDictionary<string, HashSet<string>> dctPlans, string sBranchName, string sPath)
    {
        if (!dctPlans.TryGetValue(sBranchName, out var hsPaths))
        {
            hsPaths = new HashSet<string>(StringComparer.Ordinal);
            dctPlans[sBranchName] = hsPaths;
        }

        hsPaths.Add(sPath);
    }

    private static string NormalizePath(string sPath)
        => sPath.Replace('\\', '/').Trim('/');

    private static string ComposeBranchName(string sRootBranchName, params string[] arrSubdirectorySegments)
    {
        var arrBranchSegments = sRootBranchName
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Concat(arrSubdirectorySegments)
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
