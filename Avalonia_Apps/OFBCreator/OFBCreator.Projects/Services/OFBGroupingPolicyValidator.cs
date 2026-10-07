using OFBCreator.Projects.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace OFBCreator.Projects.Services;

/// <summary>Validates automatic grouping settings and persisted editorial decisions.</summary>
public static class OFBGroupingPolicyValidator
{
    public static void Validate(OFBGroupingPolicy? policy, IReadOnlyList<OFBGroupingDecision>? decisions)
    {
        if (policy is null)
            throw new InvalidDataException("Project grouping policy cannot be null.");
        if (policy.AutoAcceptThreshold is < 0 or > 100)
            throw new InvalidDataException("Grouping automatic-acceptance threshold must be between 0 and 100.");
        if (decisions is null)
            throw new InvalidDataException("Project grouping decisions cannot be null.");

        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var orders = new HashSet<int>();
        var pairs = new HashSet<string>(StringComparer.Ordinal);
        foreach (var decision in decisions)
        {
            if (decision is null)
                throw new InvalidDataException("Project grouping decisions cannot contain null entries.");
            if (!Guid.TryParse(decision.Id, out _) || !ids.Add(decision.Id))
                throw new InvalidDataException($"Grouping decision identifier '{decision.Id}' is invalid or duplicated.");
            if (!orders.Add(decision.Order))
                throw new InvalidDataException($"Grouping decision order {decision.Order} is duplicated.");
            if (!OFBExportRuleValidator.TryParseTarget(decision.LeftFamilyTargetId, out var left)
                || left.Kind != "family"
                || !OFBExportRuleValidator.TryParseTarget(decision.RightFamilyTargetId, out var right)
                || right.Kind != "family"
                || !string.Equals(left.ProviderId, right.ProviderId, StringComparison.OrdinalIgnoreCase)
                || string.Equals(left.ExternalId, right.ExternalId, StringComparison.Ordinal))
                throw new InvalidDataException($"Grouping decision '{decision.Id}' requires two distinct family targets from the same provider.");

            var leftTarget = $"{left.ProviderId.ToLowerInvariant()}:family:{left.ExternalId}";
            var rightTarget = $"{right.ProviderId.ToLowerInvariant()}:family:{right.ExternalId}";
            var first = string.CompareOrdinal(leftTarget, rightTarget) <= 0 ? leftTarget : rightTarget;
            var second = string.CompareOrdinal(leftTarget, rightTarget) <= 0 ? rightTarget : leftTarget;
            if (!pairs.Add($"{first}\u001f{second}"))
                throw new InvalidDataException($"Grouping decision '{decision.Id}' duplicates an existing target pair.");

            if (decision.Action is not ("acceptMerge" or "rejectMerge" or "manualMerge"))
                throw new InvalidDataException($"Grouping decision '{decision.Id}' has unsupported action '{decision.Action}'.");
            if (decision.Action == "manualMerge" && string.IsNullOrWhiteSpace(decision.GroupName))
                throw new InvalidDataException($"Manual grouping decision '{decision.Id}' requires a group name.");
            if (decision.Action != "manualMerge" && decision.GroupName is not null)
                throw new InvalidDataException($"Grouping decision '{decision.Id}' cannot have a group name for action '{decision.Action}'.");
        }
    }
}
