using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Core.Models;
using OFBCreator.Projects.Models;
using OFBCreator.Projects.Services;

namespace OFBCreator.Core.Services;

/// <summary>
/// Scores qualifying parent-to-family surname transitions and applies only threshold-qualified or explicitly reviewed decisions.
/// </summary>
public sealed class OFBFamilyGroupingService
{
    /// <summary>Returns the representative surname selected for a source family.</summary>
    public static string SelectFamilySurname(IGenFamily family) => FamilySurnameSelector.Select(family);

    public OFBFamilyGroupingResult BuildGroups(
        IEnumerable<IGenFamily> families,
        string providerId,
        OFBGroupingPolicy policy,
        IReadOnlyList<OFBGroupingDecision> decisions)
    {
        ArgumentNullException.ThrowIfNull(families);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerId);
        ArgumentNullException.ThrowIfNull(policy);
        ArgumentNullException.ThrowIfNull(decisions);
        OFBGroupingPolicyValidator.Validate(policy, decisions);

        var sourceFamilies = families.ToArray();
        var noNameFamilies = new List<IGenFamily>();
        var exactGroups = new Dictionary<string, List<IGenFamily>>(StringComparer.OrdinalIgnoreCase);
        foreach (var family in sourceFamilies)
        {
            var surname = FamilySurnameSelector.Select(family);
            if (!FamilySurnameSelector.HasRealSurname(family))
            {
                noNameFamilies.Add(family);
                continue;
            }
            if (!exactGroups.ContainsKey(surname))
                exactGroups.Add(surname, []);
            exactGroups[surname].Add(family);
        }

        if (exactGroups.Count == 0)
        {
            var noNameGroups = new Dictionary<string, IReadOnlyList<IGenFamily>>(StringComparer.OrdinalIgnoreCase);
            string? unnamedGroupName = null;
            if (noNameFamilies.Count > 0)
            {
                unnamedGroupName = SurnamePlaceholderClassifier.GetUniqueNoNameSectionName(noNameGroups.Keys);
                noNameGroups.Add(unnamedGroupName, noNameFamilies.AsReadOnly());
            }
            return new OFBFamilyGroupingResult(
                noNameGroups,
                Array.Empty<OFBGroupingCandidate>(),
                Array.Empty<OFBGroupingDiagnostic>())
            {
                NoNameGroupName = unnamedGroupName
            };
        }

        var surnames = exactGroups.Keys.ToArray();
        var unionFind = new UnionFind(surnames);
        var transitions = CountSurnameTransitions(sourceFamilies, exactGroups);
        var stableTargets = exactGroups.ToDictionary(
            pair => pair.Key,
            pair => pair.Value
                .Where(family => !string.IsNullOrWhiteSpace(family.FamilyRefID))
                .OrderBy(family => family.FamilyRefID, StringComparer.Ordinal)
                .Select(family => OFBExportRuleTarget.Family(providerId, family.FamilyRefID!))
                .FirstOrDefault(),
            StringComparer.OrdinalIgnoreCase);
        var usedDecisions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var diagnostics = new List<OFBGroupingDiagnostic>();
        var candidates = new List<OFBGroupingCandidate>();
        var manualLabels = new List<(string Left, string Right, int Order, string Label)>();

        var candidateDrafts = new List<CandidateDraft>();
        for (var leftIndex = 0; leftIndex < surnames.Length; leftIndex++)
        {
            for (var rightIndex = leftIndex + 1; rightIndex < surnames.Length; rightIndex++)
            {
                var leftSurname = surnames[leftIndex];
                var rightSurname = surnames[rightIndex];
                var distance = PhoneticDistance(leftSurname, rightSurname);
                var transitionCount = GetTransitionCount(transitions, leftSurname, rightSurname)
                    + GetTransitionCount(transitions, rightSurname, leftSurname);
                var leftTargetId = stableTargets[leftSurname];
                var rightTargetId = stableTargets[rightSurname];
                var decision = decisions.FirstOrDefault(item =>
                    TargetsMatch(item.LeftFamilyTargetId, leftTargetId)
                    && TargetsMatch(item.RightFamilyTargetId, rightTargetId)
                    || TargetsMatch(item.LeftFamilyTargetId, rightTargetId)
                    && TargetsMatch(item.RightFamilyTargetId, leftTargetId));
                if (transitionCount == 0 && decision is null)
                    continue;

                candidateDrafts.Add(new CandidateDraft(
                    leftSurname,
                    rightSurname,
                    leftTargetId,
                    rightTargetId,
                    distance,
                    transitionCount,
                    decision));
            }
        }

        var minimumDistanceBySurname = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var draft in candidateDrafts)
        {
            SetMinimumDistance(minimumDistanceBySurname, draft.LeftSurname, draft.Distance);
            SetMinimumDistance(minimumDistanceBySurname, draft.RightSurname, draft.Distance);
        }

        foreach (var draft in candidateDrafts.Where(draft =>
                     draft.Distance == minimumDistanceBySurname[draft.LeftSurname]
                     || draft.Distance == minimumDistanceBySurname[draft.RightSurname]))
        {
            var leftSurname = draft.LeftSurname;
            var rightSurname = draft.RightSurname;
            var leftTargetId = draft.LeftTargetId;
            var rightTargetId = draft.RightTargetId;
            var distance = draft.Distance;
            var transitionCount = draft.TransitionCount;
            var decision = draft.Decision;
            var evidence = BuildEvidence(distance, transitionCount);
            var score = evidence.Count == 0 ? 0 : evidence.Max(item => item.Score);
            var candidateId = CreateCandidateId(leftTargetId, rightTargetId, leftSurname, rightSurname);
            var status = "suggested";

            if (leftTargetId is null || rightTargetId is null)
            {
                status = "requiresStableTargets";
                diagnostics.Add(new OFBGroupingDiagnostic(
                    "GROUPING_TARGET_UNSTABLE",
                    null,
                    $"Surname groups '{leftSurname}' and '{rightSurname}' cannot be reviewed or persisted because both lack stable family identifiers."));
            }
            else if (decision is not null)
            {
                usedDecisions.Add(decision.Id);
                status = decision.Action switch
                {
                    "acceptMerge" => "accepted",
                    "rejectMerge" => "rejected",
                    _ => "manualMerge"
                };
                if (decision.Action is "acceptMerge" or "manualMerge")
                    unionFind.Union(leftSurname, rightSurname);
                if (decision.Action == "manualMerge")
                    manualLabels.Add((leftSurname, rightSurname, decision.Order, decision.GroupName!));
            }
            else if (score >= policy.AutoAcceptThreshold)
            {
                status = "autoAccepted";
                unionFind.Union(leftSurname, rightSurname);
            }

            candidates.Add(new OFBGroupingCandidate(
                candidateId,
                leftSurname,
                rightSurname,
                leftTargetId,
                rightTargetId,
                score,
                status,
                evidence));
        }

        foreach (var decision in decisions)
        {
            if (!usedDecisions.Contains(decision.Id))
                diagnostics.Add(new OFBGroupingDiagnostic(
                    "GROUPING_DECISION_STALE",
                    decision.Id,
                    $"Grouping decision '{decision.Id}' no longer matches a current evidence candidate; it was not applied."));
        }

        var germanComparer = StringComparer.Create(CultureInfo.GetCultureInfo("de-DE"), ignoreCase: true);
        var components = surnames
            .GroupBy(unionFind.Find, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(name => name, germanComparer).ToArray(),
                StringComparer.Ordinal);
        var labelsByRoot = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var component in components)
        {
            var label = component.Value
                .SelectMany(surname => exactGroups[surname])
                .Select(FamilySurnameSelector.Select)
                .GroupBy(surname => surname, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key, germanComparer)
                .Select(group => group.Key)
                .First();
            var applicableManualLabel = manualLabels
                .Where(item => string.Equals(unionFind.Find(item.Left), component.Key, StringComparison.Ordinal)
                    && string.Equals(unionFind.Find(item.Right), component.Key, StringComparison.Ordinal))
                .OrderBy(item => item.Order)
                .LastOrDefault();
            if (applicableManualLabel.Label is not null)
                label = applicableManualLabel.Label;
            labelsByRoot[component.Key] = label;
        }

        var groups = new Dictionary<string, List<IGenFamily>>(StringComparer.OrdinalIgnoreCase);
        foreach (var surname in surnames)
        {
            var root = unionFind.Find(surname);
            var groupName = labelsByRoot[root];
            if (!groups.TryGetValue(groupName, out var group))
                groups.Add(groupName, group = []);
            group.AddRange(exactGroups[surname]);
        }

        string? noNameGroupName = null;
        if (noNameFamilies.Count > 0)
        {
            noNameGroupName = SurnamePlaceholderClassifier.GetUniqueNoNameSectionName(groups.Keys);
            groups.Add(noNameGroupName, noNameFamilies);
        }

        return new OFBFamilyGroupingResult(
            groups.ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyList<IGenFamily>)pair.Value.AsReadOnly(),
                StringComparer.OrdinalIgnoreCase),
            candidates.AsReadOnly(),
            diagnostics.AsReadOnly())
        {
            NoNameGroupName = noNameGroupName
        };
    }

    private static Dictionary<(string Parent, string Family), int> CountSurnameTransitions(
        IReadOnlyList<IGenFamily> families,
        IReadOnlyDictionary<string, List<IGenFamily>> exactGroups)
    {
        var transitions = new Dictionary<(string Parent, string Family), int>(SurnamePairComparer.Instance);
        var knownSurnames = new HashSet<string>(exactGroups.Keys, StringComparer.OrdinalIgnoreCase);
        foreach (var family in families)
        {
            if (!FamilySurnameSelector.HasRealSurname(family))
                continue;

            var familySurname = FamilySurnameSelector.Select(family);
            if (!knownSurnames.Contains(familySurname))
                continue;

            var parentSurnames = new[] { family.Husband?.Surname, family.Wife?.Surname }
                .Where(surname => !SurnamePlaceholderClassifier.IsPlaceholder(surname))
                .Select(surname => surname!.Trim())
                .ToArray();
            if (parentSurnames.Any(parentSurname =>
                    string.Equals(parentSurname, familySurname, StringComparison.OrdinalIgnoreCase)))
                continue;

            var distinctParentSurnames = parentSurnames
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (distinctParentSurnames.Length != parentSurnames.Length
                || distinctParentSurnames.Any(parentSurname => !knownSurnames.Contains(parentSurname)))
                continue;

            var distinctFamilyGroups = distinctParentSurnames
                .Append(familySurname)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
            if (distinctFamilyGroups != parentSurnames.Length + 1)
                continue;

            foreach (var parentSurname in distinctParentSurnames)
            {
                var key = (parentSurname, familySurname);
                transitions.TryGetValue(key, out var count);
                transitions[key] = count + 1;
            }
        }
        return transitions;
    }

    private static void SetMinimumDistance(IDictionary<string, int> minimums, string surname, int distance)
    {
        if (!minimums.TryGetValue(surname, out var currentMinimum) || distance < currentMinimum)
            minimums[surname] = distance;
    }

    private sealed record CandidateDraft(
        string LeftSurname,
        string RightSurname,
        string? LeftTargetId,
        string? RightTargetId,
        int Distance,
        int TransitionCount,
        OFBGroupingDecision? Decision);

    private static IReadOnlyList<OFBGroupingEvidence> BuildEvidence(int distance, int transitionCount)
    {
        var evidence = new List<OFBGroupingEvidence>();
        if (distance <= 2)
        {
            var score = distance switch
            {
                0 => 80,
                1 => 70,
                _ => 50
            };
            evidence.Add(new OFBGroupingEvidence("phoneticSimilarity", distance, 0, score));
        }
        if (transitionCount > 0)
        {
            var score = Math.Min(100, 65 + transitionCount * 10);
            evidence.Add(new OFBGroupingEvidence("parentFamilySurnameTransition", null, transitionCount, score));
        }
        return evidence.AsReadOnly();
    }

    private static int GetTransitionCount(
        IReadOnlyDictionary<(string Parent, string Family), int> transitions,
        string parentSurname,
        string familySurname) =>
        transitions.TryGetValue((parentSurname, familySurname), out var count) ? count : 0;

    private static bool TargetsMatch(string? first, string? second)
    {
        return OFBExportRuleValidator.TryParseTarget(first, out var left)
            && OFBExportRuleValidator.TryParseTarget(second, out var right)
            && string.Equals(left.ProviderId, right.ProviderId, StringComparison.OrdinalIgnoreCase)
            && left.Kind == right.Kind
            && string.Equals(left.ExternalId, right.ExternalId, StringComparison.Ordinal);
    }

    private static string CreateCandidateId(
        string? leftTargetId,
        string? rightTargetId,
        string leftSurname,
        string rightSurname)
    {
        var first = string.CompareOrdinal(leftTargetId, rightTargetId) <= 0 ? leftTargetId : rightTargetId;
        var second = string.CompareOrdinal(leftTargetId, rightTargetId) <= 0 ? rightTargetId : leftTargetId;
        var identity = first is not null && second is not null
            ? $"{first}\u001f{second}"
            : $"{leftSurname.ToUpperInvariant()}\u001f{rightSurname.ToUpperInvariant()}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity)));
    }

    private static int PhoneticDistance(string first, string second)
    {
        var left = NormalizeSurname(first);
        var right = NormalizeSurname(second);
        if (left.Length == 0 || right.Length == 0)
            return int.MaxValue;

        var distances = new int[left.Length + 1, right.Length + 1];
        for (var i = 0; i <= left.Length; i++)
            distances[i, 0] = i;
        for (var j = 0; j <= right.Length; j++)
            distances[0, j] = j;
        for (var i = 1; i <= left.Length; i++)
        {
            for (var j = 1; j <= right.Length; j++)
            {
                var cost = left[i - 1] == right[j - 1] ? 0 : 1;
                distances[i, j] = Math.Min(
                    Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost);
            }
        }
        return distances[left.Length, right.Length];
    }

    private static string NormalizeSurname(string surname) =>
        surname.Trim().ToLowerInvariant()
            .Replace("ä", "a", StringComparison.Ordinal)
            .Replace("ö", "o", StringComparison.Ordinal)
            .Replace("ü", "u", StringComparison.Ordinal)
            .Replace("ae", "a", StringComparison.Ordinal)
            .Replace("oe", "o", StringComparison.Ordinal)
            .Replace("ue", "u", StringComparison.Ordinal)
            .Replace("ß", "ss", StringComparison.Ordinal);

    private sealed class SurnamePairComparer : IEqualityComparer<(string Parent, string Family)>
    {
        public static SurnamePairComparer Instance { get; } = new();

        public bool Equals((string Parent, string Family) left, (string Parent, string Family) right) =>
            StringComparer.OrdinalIgnoreCase.Equals(left.Parent, right.Parent)
            && StringComparer.OrdinalIgnoreCase.Equals(left.Family, right.Family);

        public int GetHashCode((string Parent, string Family) pair) =>
            HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(pair.Parent),
                StringComparer.OrdinalIgnoreCase.GetHashCode(pair.Family));
    }
}
