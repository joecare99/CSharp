using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

public sealed class AxamlMvvmMemberPlanner
{
    public IReadOnlyList<AxamlMvvmMemberPlanEntry> Plan(
        IEnumerable<AxamlMvvmMemberRequirement> requirements,
        IEnumerable<AxamlMvvmSemanticMember> existingMembers,
        bool createMissingMembers)
    {
        ArgumentNullException.ThrowIfNull(requirements);
        ArgumentNullException.ThrowIfNull(existingMembers);

        Dictionary<string, AxamlMvvmSemanticMember> members = existingMembers
            .GroupBy(static member => member.Name, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.Ordinal);
        Dictionary<string, AxamlMvvmMemberRequirement> requirementNames = new(StringComparer.Ordinal);
        List<AxamlMvvmMemberPlanEntry> plan = [];

        foreach (AxamlMvvmMemberRequirement requirement in requirements)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(requirement.Name);
            ArgumentException.ThrowIfNullOrWhiteSpace(requirement.TypeName);
            if (requirementNames.TryGetValue(requirement.Name, out AxamlMvvmMemberRequirement? priorRequirement))
            {
                if (!string.Equals(priorRequirement.TypeName, requirement.TypeName, StringComparison.Ordinal)
                    || priorRequirement.IsCommand != requirement.IsCommand)
                {
                    plan.Add(new AxamlMvvmMemberPlanEntry(
                        requirement,
                        AxamlMvvmMemberPlanAction.Conflict,
                        $"Bindings for '{requirement.Name}' require incompatible member types."));
                }

                continue;
            }
            requirementNames.Add(requirement.Name, requirement);

            if (!members.TryGetValue(requirement.Name, out AxamlMvvmSemanticMember? existingMember))
            {
                plan.Add(new AxamlMvvmMemberPlanEntry(
                    requirement,
                    createMissingMembers
                        ? AxamlMvvmMemberPlanAction.Create
                        : AxamlMvvmMemberPlanAction.NeedsCreationChoice,
                    createMissingMembers
                        ? null
                        : $"Choose whether to create ViewModel member '{requirement.Name}'."));
                continue;
            }

            if (IsCompatible(requirement, existingMember))
            {
                plan.Add(new AxamlMvvmMemberPlanEntry(
                    requirement,
                    AxamlMvvmMemberPlanAction.Reuse,
                    null));
            }
            else
            {
                plan.Add(new AxamlMvvmMemberPlanEntry(
                    requirement,
                    AxamlMvvmMemberPlanAction.Conflict,
                    $"Existing ViewModel member '{requirement.Name}' is incompatible with the requested binding."));
            }
        }

        return plan;
    }

    private static bool IsCompatible(
        AxamlMvvmMemberRequirement requirement,
        AxamlMvvmSemanticMember existingMember)
    {
        if (requirement.IsCommand)
        {
            return existingMember.IsCommand;
        }

        return !existingMember.IsCommand
            && existingMember.IsWritable
            && string.Equals(existingMember.TypeName, requirement.TypeName, StringComparison.Ordinal);
    }
}

public sealed record AxamlMvvmMemberRequirement(
    string Name,
    string TypeName,
    bool IsCommand);

public sealed record AxamlMvvmMemberPlanEntry(
    AxamlMvvmMemberRequirement Requirement,
    AxamlMvvmMemberPlanAction Action,
    string? Diagnostic);

public enum AxamlMvvmMemberPlanAction
{
    Reuse,
    Create,
    NeedsCreationChoice,
    Conflict,
}
