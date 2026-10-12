using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Core.Models;
using OFBCreator.Core.Models.Gedcom;

namespace OFBCreator.Core.Services;

/// <summary>
/// Infers missing birth dates on a detached export genealogy without changing source records.
/// </summary>
public sealed partial class OFBDateEstimationService
{
    private const int ConflictWarningYears = 20;

    /// <summary>
    /// Applies family/parent estimates and then bounded two-year sibling estimates until stable.
    /// </summary>
    public IReadOnlyList<OFBGroupingDiagnostic> Apply(IEnumerable<IGenFamily> families)
    {
        ArgumentNullException.ThrowIfNull(families);
        var familyList = families.ToArray();
        var diagnostics = new List<OFBGroupingDiagnostic>();
        var reportedConflicts = new HashSet<string>(StringComparer.Ordinal);
        var maximumPassCount = familyList
            .SelectMany(family => family.Children.OfType<IGenPerson>()
                .Concat(new[] { family.Husband, family.Wife }.OfType<IGenPerson>()))
            .Distinct((IEqualityComparer<IGenPerson>)ReferenceEqualityComparer.Instance)
            .Count() + 1;

        for (var pass = 0; pass < maximumPassCount; pass++)
        {
            var parentDatesChanged = ApplyParentDateCandidates(familyList, diagnostics, reportedConflicts);
            var siblingDatesChanged = ApplySiblingDateCandidates(familyList);
            if (!parentDatesChanged && !siblingDatesChanged)
                return diagnostics;
        }

        throw new InvalidOperationException(
            $"Date estimation did not reach a stable result after {maximumPassCount} passes.");
    }

    private static bool ApplyParentDateCandidates(
        IReadOnlyList<IGenFamily> families,
        ICollection<OFBGroupingDiagnostic> diagnostics,
        ISet<string> reportedConflicts)
    {
        var candidates = families
            .OrderBy(family => family.FamilyRefID, StringComparer.Ordinal)
            .SelectMany(CreateCandidates)
            .ToArray();
        var changed = false;

        foreach (var group in candidates
            .Where(candidate => candidate.Year is >= 1 and <= 9999)
            .GroupBy(candidate => candidate.Person))
        {
            var person = group.Key;
            var currentYear = GetBirthYear(person, out _);
            if (currentYear is int existingYear)
            {
                foreach (var candidate in group)
                    ReportConflict(person, existingYear, candidate, diagnostics, reportedConflicts);
                continue;
            }

            var ordered = group
                .OrderBy(candidate => candidate.Priority)
                .ThenBy(candidate => candidate.FamilyReference, StringComparer.Ordinal)
                .ThenBy(candidate => candidate.Year)
                .ToArray();
            if (ordered.Length == 0)
                continue;

            var selected = ordered[0];
            foreach (var candidate in ordered.Skip(1))
                ReportCandidateConflict(person, selected, candidate, diagnostics, reportedConflicts);
            SetEstimatedBirthYear(person, selected.Year);
            changed = true;
        }

        return changed;
    }

    private static IEnumerable<DateCandidate> CreateCandidates(IGenFamily family)
    {
        var datedChildren = family.Children
            .OfType<IGenPerson>()
            .Select((person, index) =>
            {
                var year = GetBirthYear(person, out var estimated);
                return year is int value ? new DatedChild(person, value, estimated) : null;
            })
            .OfType<DatedChild>()
            .ToArray();
        if (datedChildren.Length == 0)
            yield break;

        var oldest = datedChildren.MinBy(child => child.Year)!;
        var youngest = datedChildren.MaxBy(child => child.Year)!;
        var childrenContainEstimate = datedChildren.Any(child => child.IsEstimated);
        var familyReference = family.FamilyRefID ?? string.Empty;

        if (family.Wife is IGenPerson mother)
        {
            var span = youngest.Year - oldest.Year;
            if ((!childrenContainEstimate && span < 24)
                || (childrenContainEstimate && span <= 30))
            {
                var estimatedYear = oldest.Year - 20;
                if (childrenContainEstimate)
                    estimatedYear = RoundMotherYear(estimatedYear, oldest.Year);
                yield return new DateCandidate(
                    mother, estimatedYear, 0, "mother-from-children", familyReference);
            }
        }

        if (family.Husband is IGenPerson husband && family.Wife is IGenPerson wife)
        {
            var husbandYear = GetBirthYear(husband, out _);
            var wifeYear = GetBirthYear(wife, out _);
            if (wifeYear is int knownWifeYear)
            {
                var candidateYear = knownWifeYear - 5;
                if (GetBirthYear(wife, out var wifeEstimated) is not null && wifeEstimated)
                    candidateYear = RoundToFiveYears(candidateYear);
                yield return new DateCandidate(husband, candidateYear, 1, "spouse-age-difference", familyReference);
            }
            if (husbandYear is int knownHusbandYear)
            {
                var candidateYear = knownHusbandYear + 5;
                if (GetBirthYear(husband, out var husbandEstimated) is not null && husbandEstimated)
                    candidateYear = RoundToFiveYears(candidateYear);
                yield return new DateCandidate(wife, candidateYear, 1, "spouse-age-difference", familyReference);
            }
        }

        var weightedYear = (int)Math.Round(
            (3d * oldest.Year + youngest.Year) / 4d,
            MidpointRounding.AwayFromZero);
        var estimatedAnchor = oldest.IsEstimated || youngest.IsEstimated;
        if (family.Wife is IGenPerson generationMother)
        {
            var year = weightedYear - 25;
            if (estimatedAnchor)
                year = RoundToFiveYears(year);
            yield return new DateCandidate(
                generationMother, year, 2, "mother-generation-gap", familyReference);
        }
        if (family.Husband is IGenPerson generationFather)
        {
            var year = weightedYear - 30;
            if (estimatedAnchor)
                year = RoundToFiveYears(year);
            yield return new DateCandidate(
                generationFather, year, 2, "father-generation-gap", familyReference);
        }
    }

    private static bool ApplySiblingDateCandidates(IReadOnlyList<IGenFamily> families)
    {
        var changed = false;
        foreach (var family in families.OrderBy(item => item.FamilyRefID, StringComparer.Ordinal))
        {
            var children = family.Children.OfType<IGenPerson>().ToArray();
            for (var firstIndex = 0; firstIndex < children.Length; firstIndex++)
            {
                var firstYear = GetBirthYear(children[firstIndex], out _);
                if (firstYear is null)
                    continue;

                var nextIndex = firstIndex + 1;
                while (nextIndex < children.Length && GetBirthYear(children[nextIndex], out _) is null)
                    nextIndex++;
                if (nextIndex >= children.Length || nextIndex - firstIndex < 2)
                    continue;

                var nextYear = GetBirthYear(children[nextIndex], out _);
                if (nextYear is null)
                    continue;
                var slots = nextIndex - firstIndex;
                var yearDifference = nextYear.Value - firstYear.Value;
                if (Math.Abs(yearDifference) != slots * 2)
                    continue;

                var direction = Math.Sign(yearDifference);
                for (var index = firstIndex + 1; index < nextIndex; index++)
                {
                    var estimatedYear = firstYear.Value + direction * 2 * (index - firstIndex);
                    SetEstimatedBirthYear(children[index], estimatedYear);
                    changed = true;
                }
                firstIndex = nextIndex - 1;
            }
        }

        return changed;
    }

    private static int RoundMotherYear(int targetYear, int oldestChildYear)
    {
        var roundedYear = RoundToFiveYears(targetYear);
        var maximumYearForAge18 = oldestChildYear - 18;
        return roundedYear <= maximumYearForAge18 ? roundedYear : roundedYear - 5;
    }

    private static int RoundToFiveYears(int year) =>
        (int)(Math.Round(year / 5d, MidpointRounding.AwayFromZero) * 5);

    private static int? GetBirthYear(IGenPerson person, out bool isEstimated)
    {
        var date = person.BirthDate ?? person.Birth?.Date;
        isEstimated = IsEstimatedBirth(date);
        if (date is null)
            return null;
        if (date.Date1 != default && date.Date1.Year is >= 1 and <= 9999)
            return date.Date1.Year;

        var match = YearToken().Match(date.DateText ?? string.Empty);
        return match.Success
            && int.TryParse(match.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var year)
                ? year
                : null;
    }

    private static bool IsEstimatedBirth(IGenDate? date) =>
        date is not null
        && (date.eDateModifier == EDateModifier.Estimated
            || date.DateText?.TrimStart().StartsWith("EST ", StringComparison.OrdinalIgnoreCase) == true);

    private static void SetEstimatedBirthYear(IGenPerson person, int year)
    {
        if (year is < 1 or > 9999)
            return;
        var estimatedDate = new GedcomDate
        {
            Date1 = new DateTime(year, 1, 1),
            eDateType1 = EDateType.Year,
            eDateModifier = EDateModifier.Estimated,
            DateText = $"EST {year.ToString(CultureInfo.InvariantCulture)}"
        };

        person.BirthDate = estimatedDate;
        if (person is not GedcomPerson gedcomPerson)
            throw new InvalidOperationException(
                $"Date estimation requires a detached GEDCOM person; '{person.IndRefID}' is not detached.");

        if (gedcomPerson.Birth is GedcomFact birthFact)
        {
            birthFact.Date = estimatedDate;
            return;
        }

        birthFact = new GedcomFact { eFactType = EFactType.Birth, Date = estimatedDate };
        birthFact.SetOwner(gedcomPerson);
        gedcomPerson.Facts.Add(birthFact);
        gedcomPerson.Birth = birthFact;
    }

    private static void ReportConflict(
        IGenPerson person,
        int existingYear,
        DateCandidate candidate,
        ICollection<OFBGroupingDiagnostic> diagnostics,
        ISet<string> reportedConflicts)
    {
        if (Math.Abs(existingYear - candidate.Year) < ConflictWarningYears)
            return;

        var conflictKey = $"{GetPersonKey(person)}|{candidate.Rule}|{candidate.Year}|{existingYear}";
        if (!reportedConflicts.Add(conflictKey))
            return;
        diagnostics.Add(new OFBGroupingDiagnostic(
            "DATE_ESTIMATE_CONFLICT",
            candidate.FamilyReference,
            $"Date rule '{candidate.Rule}' suggests {candidate.Year} for '{GetPersonLabel(person)}', " +
            $"which differs by {Math.Abs(existingYear - candidate.Year)} years from the retained date {existingYear}."));
    }

    private static void ReportCandidateConflict(
        IGenPerson person,
        DateCandidate selected,
        DateCandidate alternative,
        ICollection<OFBGroupingDiagnostic> diagnostics,
        ISet<string> reportedConflicts)
    {
        if (Math.Abs(selected.Year - alternative.Year) < ConflictWarningYears)
            return;

        var conflictKey = $"{GetPersonKey(person)}|{selected.Rule}|{selected.Year}|{alternative.Rule}|{alternative.Year}";
        if (!reportedConflicts.Add(conflictKey))
            return;
        diagnostics.Add(new OFBGroupingDiagnostic(
            "DATE_ESTIMATE_CONFLICT",
            selected.FamilyReference,
            $"Date rules '{selected.Rule}' and '{alternative.Rule}' suggest {selected.Year} and " +
            $"{alternative.Year} for '{GetPersonLabel(person)}'; the higher-priority estimate was retained."));
    }

    private static string GetPersonKey(IGenPerson person) =>
        string.IsNullOrWhiteSpace(person.IndRefID)
            ? $"{person.GivenName}|{person.Surname}|{person.Name}"
            : person.IndRefID;

    private static string GetPersonLabel(IGenPerson person) =>
        string.Join(' ', new[] { person.GivenName, person.Surname }
            .Where(value => !string.IsNullOrWhiteSpace(value)));

    private sealed record DatedChild(IGenPerson Person, int Year, bool IsEstimated);

    private sealed record DateCandidate(
        IGenPerson Person,
        int Year,
        int Priority,
        string Rule,
        string FamilyReference);

    [GeneratedRegex(@"(?<!\d)\d{1,4}(?!\d)", RegexOptions.CultureInvariant)]
    private static partial Regex YearToken();
}
