using System;
using System.Collections.Generic;

namespace Osb.Core.Selection;

public sealed record FamilySelectionRequest
{
    public FamilySelectionRequest(
        int initialPersonId,
        int initialFamilyId,
        int malePersonId,
        int femalePersonId,
        short traversalStep,
        FamilySelectionOptions options)
    {
        if (initialPersonId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialPersonId));
        }

        if (initialFamilyId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialFamilyId));
        }

        if (malePersonId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(malePersonId));
        }

        if (femalePersonId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(femalePersonId));
        }

        if (traversalStep < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(traversalStep));
        }

        InitialPersonId = initialPersonId;
        InitialFamilyId = initialFamilyId;
        MalePersonId = malePersonId;
        FemalePersonId = femalePersonId;
        TraversalStep = traversalStep;
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public int InitialPersonId { get; }

    public int InitialFamilyId { get; }

    public int MalePersonId { get; }

    public int FemalePersonId { get; }

    public short TraversalStep { get; }

    public FamilySelectionOptions Options { get; }

    public static FamilySelectionRequest FromLegacyState(
        int initialPersonId,
        int initialFamilyId,
        int malePersonId,
        int femalePersonId,
        short traversalStep,
        IReadOnlyList<string?> legacyOptions)
    {
        if (legacyOptions == null)
        {
            throw new ArgumentNullException(nameof(legacyOptions));
        }

        return new FamilySelectionRequest(
            initialPersonId,
            initialFamilyId,
            malePersonId,
            femalePersonId,
            traversalStep,
            FamilySelectionOptions.FromLegacySlots(legacyOptions));
    }
}

public sealed record FamilySelectionOptions
{
    public FamilySelectionOptions(
        string? personCutoffDate,
        bool excludeFamiliesAfterCutoff,
        string? familyCutoffDate,
        bool excludeSponsorOrWitnessOnlyPeople)
    {
        if (personCutoffDate == null)
        {
            throw new ArgumentNullException(nameof(personCutoffDate));
        }

        if (familyCutoffDate == null)
        {
            throw new ArgumentNullException(nameof(familyCutoffDate));
        }

        if (personCutoffDate.Length != 8 || !IsDigitsOnly(personCutoffDate))
        {
            throw new ArgumentException("The person cutoff must use the legacy yyyyMMdd-shaped eight-digit value.", nameof(personCutoffDate));
        }

        if (familyCutoffDate.Length != 8 || !IsDigitsOnly(familyCutoffDate))
        {
            throw new ArgumentException("The family cutoff must use the legacy yyyyMMdd-shaped eight-digit value.", nameof(familyCutoffDate));
        }

        PersonCutoffDate = personCutoffDate;
        ExcludeFamiliesAfterCutoff = excludeFamiliesAfterCutoff;
        FamilyCutoffDate = familyCutoffDate;
        ExcludeSponsorOrWitnessOnlyPeople = excludeSponsorOrWitnessOnlyPeople;
    }

    public string PersonCutoffDate { get; }

    public bool ExcludeFamiliesAfterCutoff { get; }

    public string FamilyCutoffDate { get; }

    public bool ExcludeSponsorOrWitnessOnlyPeople { get; }

    public static FamilySelectionOptions FromLegacySlots(IReadOnlyList<string?> legacyOptions)
    {
        if (legacyOptions == null)
        {
            throw new ArgumentNullException(nameof(legacyOptions));
        }

        return new FamilySelectionOptions(
            GetRequiredSlot(legacyOptions, 81),
            GetSwitch(legacyOptions, 82),
            GetRequiredSlot(legacyOptions, 83),
            GetSwitch(legacyOptions, 94));
    }

    private static bool IsDigitsOnly(string value)
    {
        for (var index = 0; index < value.Length; index++)
        {
            if (value[index] < '0' || value[index] > '9')
            {
                return false;
            }
        }

        return true;
    }

    private static string GetRequiredSlot(IReadOnlyList<string?> legacyOptions, int index)
    {
        if (index >= legacyOptions.Count)
        {
            throw new ArgumentException($"Legacy option slot {index} is missing.", nameof(legacyOptions));
        }

        var value = legacyOptions[index];
        if (value == null)
        {
            throw new ArgumentException($"Legacy option slot {index} is null.", nameof(legacyOptions));
        }

        return value;
    }

    private static bool GetSwitch(IReadOnlyList<string?> legacyOptions, int index)
    {
        var value = GetRequiredSlot(legacyOptions, index);
        return value switch
        {
            "0" => false,
            "1" => true,
            _ => throw new ArgumentException($"Legacy option slot {index} must contain 0 or 1.", nameof(legacyOptions))
        };
    }
}
