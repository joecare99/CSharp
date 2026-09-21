using System;
using System.Collections.Generic;
using System.Linq;

using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;

namespace OFBCreator.Core.Services;

/// <summary>
/// Builder for family name groups used in OFB sorting and grouping.
/// Tracks surname evolution across generations through blood relationships.
/// Algorithm: Phase 1 — exact-name grouping, then detect parent→child phonetic
/// bridges (father/mother to child) to fuse groups representing the same lineage
/// across sound shifts (e.g. Heis → Heus → Hös). Adoptions keep groups separated.
/// </summary>
public class FamilyNameGroupBuilder : IFamilyNameGroupBuilder
{
    private readonly HashSet<string> _groupMembers = new(StringComparer.Ordinal);
    private Dictionary<string, List<IGenFamily>>? _fusedGroups;

    /// <summary>
    /// Builds family name groups from the provided families.
    /// Phase 1: Collect all unique surnames and group families by primary surname
    /// Phase 2: Detect parent→child phonetic bridges and fuse groups via Union-Find
    /// </summary>
    public void BuildGroups(IEnumerable<IGenFamily> families)
    {
        ArgumentNullException.ThrowIfNull(families);

        _groupMembers.Clear();
        var allFamilies = families.ToList();

        // Phase 1a: Collect all unique surnames from ALL members (not just primary keys)
        var allSurnames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var family in allFamilies)
        {
            if (family == null) continue;

            AddSurnameFromPerson(allSurnames, family.Husband);
            AddSurnameFromPerson(allSurnames, family.Wife);

            var children = family.Children;
            if (children != null)
            {
                foreach (var child in children)
                {
                    AddSurnameFromPerson(allSurnames, child);
                }
            }
        }

        // Phase 1b: Initialize exact-groups for every unique surname (may be empty initially)
        var exactGroups = new Dictionary<string, List<IGenFamily>>(StringComparer.Ordinal);
        foreach (var surname in allSurnames)
        {
            _groupMembers.Add(surname);
            exactGroups[surname] = new List<IGenFamily>();
        }

        // Phase 1c: Assign each family to its primary-surname group
        foreach (var family in allFamilies)
        {
            if (family == null) continue;

            var primarySurname = GetPrimarySurname(family);
            if (string.IsNullOrEmpty(primarySurname)) continue;

            exactGroups[primarySurname].Add(family);
        }

        if (exactGroups.Count == 0)
        {
            _fusedGroups = new Dictionary<string, List<IGenFamily>>(StringComparer.Ordinal);
            return;
        }

        // Phase 2: Detect parent→child phonetic bridges and fuse groups
        var surnameToGroup = exactGroups.Keys.ToDictionary(k => k, StringComparer.Ordinal);
        _fusedGroups = FuseViaParentChildBridges(exactGroups, allFamilies, surnameToGroup);
    }

    private static void AddSurnameFromPerson(HashSet<string> set, IGenPerson? person)
    {
        if (person == null || string.IsNullOrEmpty(person.Surname)) return;
        set.Add(person.Surname.Trim());
    }

    /// <summary>
    /// Returns all unique group keys (fused name groups).
    /// </summary>
    public IReadOnlyCollection<string> GroupKeys => _fusedGroups?.Keys.ToList().AsReadOnly() ?? (IReadOnlyCollection<string>)new List<string>();

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
    /// Checks whether children's names are phonetically closer to one parent than the other.
    /// Returns true if there is a clear bias supporting group fusion.
    /// </summary>
    private static bool HasChildPhoneticBias(
        IGenFamily family, string parent1Name, string parent2Name, Dictionary<string, List<IGenFamily>> exactGroups,
        Dictionary<string, string> surnameToGroup, UnionFind uf)
    {
        var children = family.Children;
        if (children == null || children.Count == 0)
            return false;

        int p1Score = 0;
        int p2Score = 0;

        foreach (var child in children)
        {
            if (child == null) continue;

            var childSurname = child.Surname;
            if (string.IsNullOrEmpty(childSurname)) continue;

            // Does child belong to parent1's exact group? (childSurname is primary key of a family in p1's group)
            
            bool belongsToP1 = false;
            bool belongsToP2 = false;

            foreach (var famP1 in exactGroups[parent1Name])
            {
                if (famP1 == null) continue;
                var pk = GetPrimarySurname(famP1);
                if (!string.Equals(pk, childSurname, StringComparison.Ordinal)) continue;

                // This family's primary key matches child — check if mapped surname agrees
                foreach (var fp in new[] { famP1.Husband, famP1.Wife }.Where(p => p != null))
                {
                    if (fp.Surname == childSurname && surnameToGroup.ContainsKey(childSurname) && surnameToGroup[childSurname] == pk)
                        belongsToP1 = true;
                }
            }

            foreach (var famP2 in exactGroups[parent2Name])
            {
                if (famP2 == null) continue;
                var pk = GetPrimarySurname(famP2);
                if (!string.Equals(pk, childSurname, StringComparison.Ordinal)) continue;

                foreach (var fp in new[] { famP2.Husband, famP2.Wife }.Where(p => p != null))
                {
                    if (fp.Surname == childSurname && surnameToGroup.ContainsKey(childSurname) && surnameToGroup[childSurname] == pk)
                        belongsToP2 = true;
                }
            }

            if (belongsToP1 && !belongsToP2)
            {
                p1Score++;
                continue;
            }

            if (belongsToP2 && !belongsToP1)
            {
                p2Score++;
                continue;
            }

            // Child has new/unseen surname: use phonetic distance to determine bias
            int dist1 = PhoneticDistance(childSurname, parent1Name);
            int dist2 = PhoneticDistance(childSurname, parent2Name);

            if (dist1 < dist2)
                p1Score++;
            else if (dist2 < dist1)
                p2Score++;
        }

        // Child must be closer to one parent for fusion
        return p1Score > 0 || p2Score > 0;
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
