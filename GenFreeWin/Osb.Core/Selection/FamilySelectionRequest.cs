using System;

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
}
