using System;
using System.Collections.Generic;
using System.Linq;

using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;

namespace OFBCreator.Core.Services;

/// <summary>
/// Builder for family-name groups based on repeated parent-to-family-surname transitions
/// and close phonetic relationships between surnames.
/// </summary>
public class FamilyNameGroupBuilder : IFamilyNameGroupBuilder
{
    private readonly HashSet<string> _groupMembers = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, List<IGenFamily>>? _fusedGroups;

    /// <summary>
    /// Builds groups from single family surnames and their parent-surname transitions.
    /// </summary>
    public void BuildGroups(IEnumerable<IGenFamily> families)
    {
        ArgumentNullException.ThrowIfNull(families);

        _groupMembers.Clear();
        var allFamilies = families.ToList();

        // Keep families without a real surname for publication, but exclude their section from surname merging.
        var noNameFamilies = new List<IGenFamily>();
        var exactGroups = new Dictionary<string, List<IGenFamily>>(StringComparer.OrdinalIgnoreCase);
        foreach (var family in allFamilies)
        {
            if (family == null) continue;

            var familySurname = FamilySurnameSelector.Select(family);
            if (!FamilySurnameSelector.HasRealSurname(family))
            {
                noNameFamilies.Add(family);
                continue;
            }

            _groupMembers.Add(familySurname);
            if (!exactGroups.ContainsKey(familySurname))
                exactGroups[familySurname] = new List<IGenFamily>();
        }

        // Assign each family to its primary-surname group
        foreach (var family in allFamilies)
        {
            if (family == null) continue;

            var primarySurname = FamilySurnameSelector.Select(family);
            if (!FamilySurnameSelector.HasRealSurname(family))
                continue;

            exactGroups[primarySurname].Add(family);
        }

        if (exactGroups.Count == 0)
        {
            _fusedGroups = new Dictionary<string, List<IGenFamily>>(StringComparer.Ordinal);
            if (noNameFamilies.Count > 0)
                _fusedGroups.Add(
                    SurnamePlaceholderClassifier.GetUniqueNoNameSectionName(_fusedGroups.Keys),
                    noNameFamilies);
            return;
        }

        // Repeated surname transitions represent a shared family group.
        var surnameToGroup = exactGroups.Keys.ToDictionary(k => k, StringComparer.OrdinalIgnoreCase);
        var unionFind = new UnionFind(surnameToGroup.Keys.ToList());
        var transitions = new Dictionary<(string Parent, string Family), int>();
        foreach (var family in allFamilies)
        {
            if (family is null || IsAdoptedFamily(family))
                continue;

            var familySurname = FamilySurnameSelector.Select(family);
            if (!surnameToGroup.ContainsKey(familySurname))
                continue;

            foreach (var parentSurname in new[] { family.Husband?.Surname, family.Wife?.Surname }
                         .Where(surname => !string.IsNullOrWhiteSpace(surname))
                         .Select(surname => surname!.Trim())
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (string.Equals(parentSurname, familySurname, StringComparison.OrdinalIgnoreCase))
                    continue;

                var key = (parentSurname, familySurname);
                transitions.TryGetValue(key, out var count);
                transitions[key] = count + 1;
            }
        }

        foreach (var transition in transitions.Where(transition => transition.Value >= 2))
        {
            if (surnameToGroup.TryGetValue(transition.Key.Parent, out var parentGroup))
                unionFind.Union(parentGroup, transition.Key.Family);
        }

        var surnames = surnameToGroup.Keys.ToArray();
        for (var first = 0; first < surnames.Length; first++)
        {
            for (var second = first + 1; second < surnames.Length; second++)
            {
                if (PhoneticDistance(surnames[first], surnames[second]) <= 2)
                    unionFind.Union(surnames[first], surnames[second]);
            }
        }

        _fusedGroups = BuildFusedGroups(exactGroups, unionFind);
        if (noNameFamilies.Count > 0)
            _fusedGroups.Add(
                SurnamePlaceholderClassifier.GetUniqueNoNameSectionName(_fusedGroups.Keys),
                noNameFamilies);
    }

    /// <summary>
    /// Returns all unique group keys (fused name groups).
    /// </summary>
    public IReadOnlyCollection<string> GroupKeys => _fusedGroups?.Keys.ToList().AsReadOnly() ?? (IReadOnlyCollection<string>)Array.Empty<string>();

    /// <summary>
    /// Gets families in a specific fused name group.
    /// </summary>
    public IReadOnlyList<IGenFamily>? GetGroup(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            return null;

        if (_fusedGroups == null || !_fusedGroups.TryGetValue(groupName, out var list))
            return null;

        return list.AsReadOnly();
    }

    /// <summary>
    /// Fuses groups by detecting parent→child phonetic bridges.
    /// For each family: if father's surname is in group A and child's surname maps to group B
    /// (A ≠ B) AND they are phonetically similar → fuse A with B. Same for mother→child.
    /// Adoptions are excluded — groups stay separated.
    /// </summary>
    private static Dictionary<string, List<IGenFamily>> FuseViaParentChildBridges(
        Dictionary<string, List<IGenFamily>> exactGroups,
        IReadOnlyList<IGenFamily> allFamilies,
        Dictionary<string, string> surnameToGroup)
    {
        var uf = new UnionFind(surnameToGroup.Keys.ToList());

        foreach (var family in allFamilies)
        {
            if (family == null) continue;

            // Skip adoptions — groups stay separated for adopted families
            if (IsAdoptedFamily(family))
                continue;

            var children = family.Children;
            if (children == null || children.Count == 0)
                continue;

            // Father → Child bridges
            var father = family.Husband;
            if (father != null && !string.IsNullOrEmpty(father.Surname))
            {
                foreach (var child in children)
                {
                    if (child == null) continue;
                    var childSurname = child.Surname;
                    if (string.IsNullOrEmpty(childSurname)) continue;

                    LinkParentChildGroup(father.Surname, childSurname, exactGroups, surnameToGroup, uf);
                }
            }

            // Mother → Child bridges
            var mother = family.Wife;
            if (mother != null && !string.IsNullOrEmpty(mother.Surname))
            {
                foreach (var child in children)
                {
                    if (child == null) continue;
                    var childSurname = child.Surname;
                    if (string.IsNullOrEmpty(childSurname)) continue;

                    LinkParentChildGroup(mother.Surname, childSurname, exactGroups, surnameToGroup, uf);
                }
            }
        }

        // Build fused groups from Union-Find roots
        return BuildFusedGroups(exactGroups, uf);
    }

    /// <summary>
    /// Links parent's group to child's group if surnames are phonetically similar but in different exact groups.
    /// If childSurname is itself an exact-group key and differs from parent's group → fuse.
    /// If childSurname is NOT a known key (new/unknown surname) → skip (nothing to fuse to yet).
    /// </summary>
    private static void LinkParentChildGroup(
        string parentSurname, string childSurname,
        Dictionary<string, List<IGenFamily>> exactGroups,
        Dictionary<string, string> surnameToGroup,
        UnionFind uf)
    {
        if (!surnameToGroup.TryGetValue(parentSurname, out var parentGroup))
            return;

        if (!surnameToGroup.TryGetValue(childSurname, out var childGroup))
            return; // Child's surname not yet an exact-group key — nothing to fuse

        if (string.Equals(parentGroup, childGroup, StringComparison.Ordinal))
            return; // Already in same group

        // Parent and child surnames are in different groups — check phonetic similarity
        int distance = PhoneticDistance(parentSurname, childSurname);
        if (distance > 2)
            return; // Too dissimilar to be the same lineage

        // Phonetically similar → fuse the groups
        uf.Union(parentGroup, childGroup);
    }

    /// <summary>
    /// Builds the final fused group dictionary from Union-Find state.
    /// </summary>
    private static Dictionary<string, List<IGenFamily>> BuildFusedGroups(
        Dictionary<string, List<IGenFamily>> exactGroups, UnionFind uf)
    {
        var fusedGroups = new Dictionary<string, List<IGenFamily>>(StringComparer.Ordinal);

        foreach (var kvp in exactGroups)
        {
            var groupName = kvp.Key;
            var rootGroup = uf.Find(groupName);

            if (!fusedGroups.TryGetValue(rootGroup, out var group))
            {
                group = new List<IGenFamily>();
                fusedGroups[rootGroup] = group;
            }

            foreach (var fam in kvp.Value)
            {
                if (!group.Contains(fam))
                    group.Add(fam);
            }
        }

        return fusedGroups;
    }

    /// <summary>
    /// Computes a simple phonetic distance between two surnames.
    /// Uses normalized string comparison: erases umlauts, ß→ss, and applies German sound equivalence.
    /// </summary>
    private static int PhoneticDistance(string name1, string name2)
    {
        var n1 = NormalizeForPhonetics(name1);
        var n2 = NormalizeForPhonetics(name2);

        if (string.Equals(n1, n2, StringComparison.Ordinal))
            return 0;

        // Simple Levenshtein distance on normalized names
        return LevenshteinDistance(n1, n2);
    }

    /// <summary>
    /// Normalizes a surname for phonetic comparison.
    /// - Erases umlauts: ä→a, ö→o, ü→u (with ae/oe/ue variants treated as equivalent)
    /// - Converts ß to ss
    /// - Lowercases for case-insensitive matching
    /// </summary>
    private static string NormalizeForPhonetics(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var normalized = name.Trim().ToLowerInvariant();

        // Umlaut erasing (ä→a, ö→o, ü→u) — handles both direct and ae/oe/ue variants
        normalized = normalized.Replace("ä", "a").Replace("ö", "o").Replace("ü", "u");
        normalized = normalized.Replace("ae", "a").Replace("oe", "o").Replace("ue", "u");

        // ß → ss
        normalized = normalized.Replace("ß", "ss");

        return normalized;
    }

    /// <summary>
    /// Computes the Levenshtein edit distance between two strings.
    /// </summary>
    private static int LevenshteinDistance(string s1, string s2)
    {
        int n = s1.Length;
        int m = s2.Length;

        if (n == 0) return m;
        if (m == 0) return n;

        var d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++)
            d[i, 0] = i;
        for (int j = 0; j <= m; j++)
            d[0, j] = j;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }

    /// <summary>
    /// Checks if a family is an adoption (groups stay separated for adopted families).
    /// </summary>
    private static bool IsAdoptedFamily(IGenFamily family)
    {
        // Check GEDCOM-style adoption indicators
        // HUSB/WIFE with ADOP tag or RELI=Adopted
        var husband = family.Husband;
        var wife = family.Wife;

        // In GEDCOM, adopted children are marked separately
        // For simplicity: check if any child has ADOP indicator
        var children = family.Children;
        if (children != null)
        {
            foreach (var child in children)
            {
                if (child == null) continue;

                // Check for GEDCOM adoption flags (implementation depends on IGenPerson interface)
                if (IsAdoptedChild(child))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines if a child is marked as adopted.
    /// </summary>
    private static bool IsAdoptedChild(IGenPerson child)
    {
        // Placeholder: check for adoption indicators in the GEDCOM data source
        // This depends on whether IGenPerson exposes adoption flags or relation type
        return false; // Default to non-adopted (no adoption flag available)
    }

    /// <summary>
    /// Extracts the primary surname for a family (used as exact-group key in Phase 1).
    /// Priority: husband's surname → wife's surname → first child's surname.
    /// </summary>
    private static string GetPrimarySurname(IGenFamily family)
    {
        var husband = family.Husband;
        if (husband != null && !string.IsNullOrEmpty(husband.Surname))
            return husband.Surname.Trim();

        var wife = family.Wife;
        if (wife != null && !string.IsNullOrEmpty(wife.Surname))
            return wife.Surname.Trim();

        // Fallback: first child's surname
        var children = family.Children;
        if (children != null && children.Count > 0)
        {
            foreach (var child in children)
            {
                if (child != null && !string.IsNullOrEmpty(child.Surname))
                    return child.Surname.Trim();
            }
        }

        return string.Empty;
    }
}
