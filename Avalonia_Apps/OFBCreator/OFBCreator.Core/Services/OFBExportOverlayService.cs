using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Core.Models;
using OFBCreator.Core.Models.Gedcom;
using OFBCreator.Projects.Models;
using OFBCreator.Projects.Services;

namespace OFBCreator.Core.Services;

/// <summary>
/// Creates a detached export view and applies project rules without changing imported genealogy data.
/// </summary>
public sealed partial class OFBExportOverlayService
{
    public OFBExportOverlayResult Apply(
        IGenealogy source,
        string providerId,
        IReadOnlyList<OFBExportRule> rules,
        IReadOnlyCollection<IGenFamily>? selectedFamilies = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerId);
        ArgumentNullException.ThrowIfNull(rules);
        OFBExportRuleValidator.Validate(rules);

        var enabledRules = rules.Where(rule => rule.Enabled).OrderBy(rule => rule.Order).ToArray();
        if (enabledRules.Length == 0 && selectedFamilies is null)
            return new OFBExportOverlayResult(
                source,
                Array.Empty<OFBExportRuleDiagnostic>(),
                CreateUnchangedPersonPreviews(source, providerId));

        var genealogy = new GedcomGenealogy();
        var diagnostics = new List<OFBExportRuleDiagnostic>();
        var matchedRules = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var allSourceFamilies = source.Entitys.OfType<IGenFamily>().ToArray();
        var sourcePeople = source.Entitys.OfType<IGenPerson>()
            .Concat(allSourceFamilies.SelectMany(GetFamilyPeople))
            .Distinct((IEqualityComparer<IGenPerson>)ReferenceEqualityComparer.Instance)
            .ToArray();
        var sourceFamilies = allSourceFamilies;
        if (selectedFamilies is not null)
        {
            var selectedSet = new HashSet<IGenFamily>(
                selectedFamilies,
                (IEqualityComparer<IGenFamily>)ReferenceEqualityComparer.Instance);
            sourceFamilies = sourceFamilies.Where(selectedSet.Contains).ToArray();
        }
        MarkResolvableRules(providerId, sourcePeople, allSourceFamilies, enabledRules, matchedRules);

        var peopleBySource = new Dictionary<IGenPerson, GedcomPerson>(ReferenceEqualityComparer.Instance);
        var familyBySource = new Dictionary<IGenFamily, GedcomFamily>(ReferenceEqualityComparer.Instance);
        foreach (var person in sourcePeople)
        {
            var targetId = GetPersonTarget(providerId, person) ?? string.Empty;
            if (!IsIncluded("person", targetId, enabledRules, matchedRules))
                continue;

            var clone = ClonePerson(person, targetId, enabledRules, matchedRules, genealogy, diagnostics);
            peopleBySource.Add(person, clone);
            genealogy.Entitys.Add(clone);
        }

        foreach (var family in sourceFamilies)
        {
            var targetId = GetFamilyTarget(providerId, family) ?? string.Empty;
            if (!IsIncluded("family", targetId, enabledRules, matchedRules))
                continue;

            var husband = family.Husband is not null && peopleBySource.TryGetValue(family.Husband, out var clonedHusband)
                ? clonedHusband
                : null;
            var wife = family.Wife is not null && peopleBySource.TryGetValue(family.Wife, out var clonedWife)
                ? clonedWife
                : null;
            var children = family.Children
                .Where(child => child is not null && peopleBySource.ContainsKey(child))
                .Select(child => peopleBySource[child!])
                .ToArray();
            if (husband is null && wife is null && children.Length == 0)
                continue;

            var clone = CloneFamily(family, targetId, enabledRules, matchedRules, genealogy, diagnostics);
            clone.Husband = husband;
            clone.Wife = wife;
            clone.FamilyName = husband?.Surname ?? wife?.Surname;
            foreach (var child in children)
                clone.Children.Add(child);

            familyBySource.Add(family, clone);
            genealogy.Entitys.Add(clone);
        }

        LinkClonedRelationships(familyBySource);
        var retainedPeople = new HashSet<IGenPerson>(
            familyBySource.Values
                .SelectMany(family => new[] { family.Husband, family.Wife }.OfType<IGenPerson>()
                    .Concat(family.Children.OfType<IGenPerson>())),
            (IEqualityComparer<IGenPerson>)ReferenceEqualityComparer.Instance);
        var personPreviews = sourcePeople
            .Where(peopleBySource.ContainsKey)
            .Where(person => retainedPeople.Contains(peopleBySource[person]))
            .Select(person => CreatePersonPreview(
                providerId,
                person,
                peopleBySource[person]))
            .ToArray();
        foreach (var rule in enabledRules)
        {
            if (!matchedRules.Contains(rule.Id))
                diagnostics.Add(new OFBExportRuleDiagnostic(
                    "STALE_RULE_TARGET",
                    rule.Id,
                    $"Rule target '{rule.TargetId}' or its selected fact no longer exists in the imported source.",
                    false));
        }

        return new OFBExportOverlayResult(genealogy, diagnostics, personPreviews);
    }

    private static OFBPersonPrivacyPreview CreatePersonPreview(
        string providerId,
        IGenPerson source,
        IGenPerson exported)
    {
        var changes = new List<OFBPersonPrivacyChange>();
        AddChange(changes, "givenName", source.GivenName, exported.GivenName);
        AddChange(changes, "surname", source.Surname, exported.Surname);
        AddChange(changes, "title", source.Title, exported.Title);
        AddChange(changes, "religion", source.Religion, exported.Religion);
        AddChange(changes, "occupation", source.Occupation, exported.Occupation);
        AddChange(changes, "birthDate", FormatDate(source.BirthDate), FormatDate(exported.BirthDate));
        AddChange(changes, "birthPlace", FormatPlace(source.BirthPlace), FormatPlace(exported.BirthPlace));
        AddChange(changes, "baptismDate", FormatDate(source.BaptDate), FormatDate(exported.BaptDate));
        AddChange(changes, "baptismPlace", FormatPlace(source.BaptPlace), FormatPlace(exported.BaptPlace));
        AddChange(changes, "deathDate", FormatDate(source.DeathDate), FormatDate(exported.DeathDate));
        AddChange(changes, "deathPlace", FormatPlace(source.DeathPlace), FormatPlace(exported.DeathPlace));
        AddChange(changes, "burialDate", FormatDate(source.BurialDate), FormatDate(exported.BurialDate));
        AddChange(changes, "burialPlace", FormatPlace(source.BurialPlace), FormatPlace(exported.BurialPlace));
        AddChange(changes, "residence", FormatPlace(source.Residence), FormatPlace(exported.Residence));
        AddChange(changes, "occupationPlace", FormatPlace(source.OccuPlace), FormatPlace(exported.OccuPlace));
        AddChange(changes, "facts", FormatFacts(source), FormatFacts(exported));

        var displayName = string.Join(" ", new[] { exported.GivenName, exported.Surname }
            .Where(value => !string.IsNullOrWhiteSpace(value)));
        if (string.IsNullOrWhiteSpace(displayName))
            displayName = GetPersonTarget(providerId, source) ?? string.Empty;

        return new OFBPersonPrivacyPreview(
            GetPersonTarget(providerId, source) ?? string.Empty,
            displayName,
            changes);
    }

    private static IReadOnlyList<OFBPersonPrivacyPreview> CreateUnchangedPersonPreviews(
        IGenealogy source,
        string providerId)
    {
        var families = source.Entitys.OfType<IGenFamily>().ToArray();
        var people = source.Entitys.OfType<IGenPerson>()
            .Concat(families.SelectMany(GetFamilyPeople))
            .Distinct((IEqualityComparer<IGenPerson>)ReferenceEqualityComparer.Instance);
        return people
            .Where(person => !string.IsNullOrWhiteSpace(GetPersonTarget(providerId, person)))
            .Select(person => CreatePersonPreview(providerId, person, person))
            .ToArray();
    }

    private static void AddChange(
        ICollection<OFBPersonPrivacyChange> changes,
        string field,
        string? originalValue,
        string? exportValue)
    {
        if (!string.Equals(originalValue, exportValue, StringComparison.Ordinal))
            changes.Add(new OFBPersonPrivacyChange(field, originalValue, exportValue));
    }

    private static string? FormatDate(IGenDate? date) => date is null
        ? null
        : string.IsNullOrWhiteSpace(date.DateText)
            ? date.Date1 == default ? null : date.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : date.DateText;

    private static string? FormatPlace(IGenPlace? place) => place?.Name;

    private static string? FormatFacts(IGenEntity person)
    {
        var facts = person.Facts.OfType<IGenFact>()
            .Select(fact => string.Join(" | ", new[]
            {
                fact.eFactType.ToString(),
                fact.Data,
                FormatDate(fact.Date),
                FormatPlace(fact.Place)
            }.Where(value => !string.IsNullOrWhiteSpace(value))))
            .ToArray();
        return facts.Length == 0 ? null : string.Join("; ", facts);
    }

    private static GedcomPerson ClonePerson(
        IGenPerson source,
        string targetId,
        IReadOnlyList<OFBExportRule> rules,
        ISet<string> matchedRules,
        GedcomGenealogy genealogy,
        ICollection<OFBExportRuleDiagnostic> diagnostics)
    {
        var fieldRules = GetFieldRules("person", targetId, rules, matchedRules);
        var nameVariants = CanonicalGenealogyAdapter.GetNameVariants(source);
        var nameEvents = CanonicalGenealogyAdapter.GetNameEvents(source);
        var clone = new GedcomPerson
        {
            UId = source.UId,
            ID = source.ID,
            Name = source.Name,
            GivenName = source.GivenName,
            Surname = source.Surname,
            BirthSurname = nameVariants.BirthSurname,
            ReferenceNumber = source is GedcomPerson gedcomPerson ? gedcomPerson.ReferenceNumber : null,
            Title = source.Title,
            Sex = source.Sex,
            IndRefID = source.IndRefID,
            Religion = source.Religion,
            Occupation = source.Occupation
        };
        clone.BirthDate = CloneDate(source.BirthDate ?? source.Birth?.Date);
        clone.DeathDate = CloneDate(source.DeathDate ?? source.Death?.Date);
        clone.BaptDate = CloneDate(source.BaptDate ?? source.Baptism?.Date);
        clone.BurialDate = CloneDate(source.BurialDate ?? source.Burial?.Date);
        clone.SetOwner(genealogy);
        clone.AliasNames.AddRange(nameVariants.AliasNames);
        clone.NameEvents.AddRange(nameEvents.Select(nameEvent => new GedcomNameEvent
        {
            GivenName = nameEvent.GivenName,
            Surname = nameEvent.Surname,
            Type = nameEvent.Type,
            DateText = nameEvent.DateText,
            Date = nameEvent.Date
        }));

        clone.GivenName = ApplyTextRules(clone.GivenName, fieldRules, "givenName");
        clone.Surname = ApplyTextRules(clone.Surname, fieldRules, "surname");
        clone.Title = ApplyTextRules(clone.Title, fieldRules, "title");
        clone.Religion = ApplyTextRules(clone.Religion, fieldRules, "religion");
        clone.Occupation = ApplyTextRules(clone.Occupation, fieldRules, "occupation");
        var displayNameRule = LastRule(fieldRules, "displayName");
        if (displayNameRule is not null)
        {
            clone.Name = displayNameRule.Action == "redact" ? string.Empty : displayNameRule.Value!;
            clone.GivenName = clone.Name;
            clone.Surname = null;
        }

        var factCopies = CloneFacts(source, targetId, fieldRules, rules, matchedRules, clone, diagnostics);
        clone.ReferenceNumber = clone.Facts.FirstOrDefault(fact => fact?.eFactType == EFactType.Reference)?.Data;
        if (source.Facts.Any(fact => fact?.eFactType == EFactType.Occupation))
            clone.Occupation = clone.Facts
                .FirstOrDefault(fact => fact?.eFactType == EFactType.Occupation)?.Data;
        if (source.Facts.Any(fact => fact?.eFactType == EFactType.Religion))
            clone.Religion = clone.Facts
                .FirstOrDefault(fact => fact?.eFactType == EFactType.Religion)?.Data;
        clone.Birth = FindFactCopy(source.Birth, factCopies);
        clone.Baptism = FindFactCopy(source.Baptism, factCopies);
        clone.Death = FindFactCopy(source.Death, factCopies);
        clone.Burial = FindFactCopy(source.Burial, factCopies);
        clone.BirthDate = ResolveDate(source.BirthDate, source.Birth, clone.Birth, fieldRules, "birthDate", diagnostics)
            ?? clone.Birth?.Date;
        clone.BirthPlace = ResolvePlace(source.BirthPlace, source.Birth, clone.Birth, fieldRules, "birthPlace");
        clone.BaptDate = ResolveDate(source.BaptDate, source.Baptism, clone.Baptism, fieldRules, "baptismDate", diagnostics);
        clone.BaptPlace = ResolvePlace(source.BaptPlace, source.Baptism, clone.Baptism, fieldRules, "baptismPlace");
        clone.DeathDate = ResolveDate(source.DeathDate, source.Death, clone.Death, fieldRules, "deathDate", diagnostics)
            ?? clone.Death?.Date;
        clone.DeathPlace = ResolvePlace(source.DeathPlace, source.Death, clone.Death, fieldRules, "deathPlace");
        clone.BurialDate = ResolveDate(source.BurialDate, source.Burial, clone.Burial, fieldRules, "burialDate", diagnostics);
        clone.BurialPlace = ResolvePlace(source.BurialPlace, source.Burial, clone.Burial, fieldRules, "burialPlace");

        var residence = LastRule(fieldRules, "residence");
        clone.Residence = ApplyPlaceRule(source.Residence, residence);
        var occupationPlace = LastRule(fieldRules, "occupationPlace");
        clone.OccuPlace = ApplyPlaceRule(source.OccuPlace, occupationPlace);
        return clone;
    }

    private static GedcomFamily CloneFamily(
        IGenFamily source,
        string targetId,
        IReadOnlyList<OFBExportRule> rules,
        ISet<string> matchedRules,
        GedcomGenealogy genealogy,
        ICollection<OFBExportRuleDiagnostic> diagnostics)
    {
        var fieldRules = GetFieldRules("family", targetId, rules, matchedRules);
        var clone = new GedcomFamily
        {
            UId = source.UId,
            ID = source.ID,
            FamilyRefID = source.FamilyRefID,
            FamilyName = source.FamilyName
        };
        clone.SetOwner(genealogy);

        var factCopies = CloneFacts(source, targetId, fieldRules, rules, matchedRules, clone, diagnostics);
        clone.Marriage = FindFactCopy(source.Marriage, factCopies);
        clone.MarriageDate = ResolveDate(
            source.MarriageDate, source.Marriage, clone.Marriage, fieldRules, "marriageDate", diagnostics);
        clone.MarriagePlace = ResolvePlace(
            source.MarriagePlace, source.Marriage, clone.Marriage, fieldRules, "marriagePlace");
        return clone;
    }

    private static Dictionary<Guid, IGenFact> CloneFacts(
        IGenEntity source,
        string ownerTargetId,
        IReadOnlyList<OFBExportRule> fieldRules,
        IReadOnlyList<OFBExportRule> rules,
        ISet<string> matchedRules,
        GedcomEntity owner,
        ICollection<OFBExportRuleDiagnostic> diagnostics)
    {
        var copies = new Dictionary<Guid, IGenFact>();
        var occurrenceByType = new Dictionary<EFactType, int>();
        foreach (var sourceFact in source.Facts.OfType<IGenFact>())
        {
            occurrenceByType.TryGetValue(sourceFact.eFactType, out var occurrence);
            occurrence++;
            occurrenceByType[sourceFact.eFactType] = occurrence;
            var factRules = rules.Where(rule =>
                    rule.TargetKind == "fact"
                && TargetEquals(rule.TargetId, ownerTargetId)
                    && string.Equals(rule.Field, sourceFact.eFactType.ToString(), StringComparison.OrdinalIgnoreCase)
                    && (rule.Occurrence is null || rule.Occurrence == occurrence))
                .OrderBy(rule => rule.Order)
                .ToArray();
            foreach (var rule in factRules)
                matchedRules.Add(rule.Id);
            if (factRules.Length > 0)
                continue;

            var dateField = GetFactDateField(sourceFact.eFactType);
            var placeField = GetFactPlaceField(sourceFact.eFactType);
            var factDate = dateField is null
                ? CloneDate(sourceFact.Date)
                : ApplyDateRule(sourceFact.Date, LastRule(fieldRules, dateField), diagnostics);
            var factPlace = placeField is null
                ? ClonePlace(sourceFact.Place)
                : ApplyPlaceRule(sourceFact.Place, LastRule(fieldRules, placeField));
            var dataField = sourceFact.eFactType switch
            {
                EFactType.Occupation => "occupation",
                EFactType.Religion => "religion",
                _ => null
            };
            var factData = dataField is null
                ? sourceFact.Data
                : ApplyTextRules(sourceFact.Data, fieldRules, dataField);
            var fact = new GedcomFact
            {
                UId = sourceFact.UId,
                ID = sourceFact.ID,
                eFactType = sourceFact.eFactType,
                Date = factDate,
                Place = factPlace,
                Data = factData
            };
            fact.SetOwner(owner);
            foreach (var factSource in sourceFact.Sources)
                fact.Sources.Add(factSource);
            foreach (var media in sourceFact.Medias)
                fact.Medias.Add(media);
            owner.Facts.Add(fact);
            copies[sourceFact.UId] = fact;
        }
        return copies;
    }

    private static IGenFact? FindFactCopy(IGenFact? source, IReadOnlyDictionary<Guid, IGenFact> copies) =>
        source is not null && copies.TryGetValue(source.UId, out var copy) ? copy : null;

    private static IGenDate? ResolveDate(
        IGenDate? original,
        IGenFact? sourceFact,
        IGenFact? clonedFact,
        IReadOnlyList<OFBExportRule> rules,
        string field,
        ICollection<OFBExportRuleDiagnostic> diagnostics)
    {
        if (sourceFact is not null)
            return clonedFact?.Date;
        return ApplyDateRule(original, LastRule(rules, field), diagnostics);
    }

    private static IGenPlace? ResolvePlace(
        IGenPlace? original,
        IGenFact? sourceFact,
        IGenFact? clonedFact,
        IReadOnlyList<OFBExportRule> rules,
        string field)
    {
        if (sourceFact is not null)
            return clonedFact?.Place;
        return ApplyPlaceRule(original, LastRule(rules, field));
    }

    private static IGenDate? ApplyDateRule(
        IGenDate? source,
        OFBExportRule? rule,
        ICollection<OFBExportRuleDiagnostic> diagnostics)
    {
        if (source is null)
            return null;
        if (rule is null)
            return CloneDate(source);
        if (rule.Action == "redact")
            return null;
        if (rule.Action == "replace")
            return new GedcomDate { DateText = rule.Value, eDateModifier = EDateModifier.Text };

        var year = source.Date1 != default
            ? source.Date1.Year
            : ExtractYear(source.DateText);
        if (year is null)
        {
            diagnostics.Add(new OFBExportRuleDiagnostic(
                "DATE_GENERALIZATION_FAILED",
                rule.Id,
                $"Date rule '{rule.Id}' could not determine a year for the source date '{source}'.",
                true));
            return CloneDate(source);
        }

        var generalizedYear = rule.Value == "decade" ? year.Value / 10 * 10 : year.Value;
        if (generalizedYear < 1)
        {
            diagnostics.Add(new OFBExportRuleDiagnostic(
                "DATE_GENERALIZATION_FAILED",
                rule.Id,
                $"Date rule '{rule.Id}' cannot generalize source year {year.Value} to a decade.",
                true));
            return CloneDate(source);
        }
        return new GedcomDate
        {
            Date1 = new DateTime(generalizedYear, 1, 1),
            eDateType1 = EDateType.Year,
            eDateModifier = EDateModifier.None,
            DateText = rule.Value == "decade"
                ? $"{generalizedYear.ToString(CultureInfo.InvariantCulture)}s"
                : generalizedYear.ToString(CultureInfo.InvariantCulture)
        };
    }

    private static IGenPlace? ApplyPlaceRule(IGenPlace? source, OFBExportRule? rule)
    {
        if (source is null)
            return null;
        if (rule?.Action == "redact")
            return null;

        var place = ClonePlace(source);
        if (place is null)
            return null;
        if (rule?.Action is "replace" or "generalize")
        {
            place.Name = rule.Value;
            place.GOV_ID = null;
            place.Type = null;
            place.Latitude = 0;
            place.Longitude = 0;
            place.Notes = null;
            place.Parent = null;
        }
        return place;
    }

    private static string? ApplyTextRules(
        string? source,
        IReadOnlyList<OFBExportRule> rules,
        string field)
    {
        var rule = LastRule(rules, field);
        if (rule is null)
            return source;
        return rule.Action == "redact" ? null : rule.Value;
    }

    private static IReadOnlyList<OFBExportRule> GetFieldRules(
        string targetKind,
        string targetId,
        IReadOnlyList<OFBExportRule> rules,
        ISet<string> matchedRules)
    {
        var matches = rules.Where(rule =>
                rule.TargetKind == targetKind
                && TargetEquals(rule.TargetId, targetId)
                && rule.Action is "replace" or "redact" or "generalize")
            .OrderBy(rule => rule.Order)
            .ToArray();
        foreach (var rule in matches)
            matchedRules.Add(rule.Id);
        return matches;
    }

    private static OFBExportRule? LastRule(IReadOnlyList<OFBExportRule> rules, string field) =>
        rules.LastOrDefault(rule => string.Equals(rule.Field, field, StringComparison.Ordinal));

    private static string? GetPersonTarget(string providerId, IGenPerson person) =>
        string.IsNullOrWhiteSpace(person.IndRefID) ? null : OFBExportRuleTarget.Person(providerId, person.IndRefID);

    private static string? GetFamilyTarget(string providerId, IGenFamily family) =>
        string.IsNullOrWhiteSpace(family.FamilyRefID) ? null : OFBExportRuleTarget.Family(providerId, family.FamilyRefID);

    private static bool TargetEquals(string? left, string? right) =>
        OFBExportRuleValidator.TryParseTarget(left, out var leftTarget)
        && OFBExportRuleValidator.TryParseTarget(right, out var rightTarget)
        && string.Equals(leftTarget.ProviderId, rightTarget.ProviderId, StringComparison.OrdinalIgnoreCase)
        && string.Equals(leftTarget.Kind, rightTarget.Kind, StringComparison.Ordinal)
        && string.Equals(leftTarget.ExternalId, rightTarget.ExternalId, StringComparison.Ordinal);

    private static bool IsIncluded(
        string targetKind,
        string targetId,
        IReadOnlyList<OFBExportRule> rules,
        ISet<string> matchedRules)
    {
        var matching = rules.Where(rule =>
                rule.TargetKind == targetKind
                && TargetEquals(rule.TargetId, targetId)
                && rule.Action is "include" or "exclude")
            .OrderBy(rule => rule.Order)
            .ToArray();
        foreach (var rule in matching)
            matchedRules.Add(rule.Id);

        var included = !rules.Any(rule => rule.TargetKind == targetKind && rule.Action == "include");
        foreach (var rule in matching)
            included = rule.Action == "include";
        return included;
    }

    private static void MarkResolvableRules(
        string providerId,
        IReadOnlyList<IGenPerson> people,
        IReadOnlyList<IGenFamily> families,
        IReadOnlyList<OFBExportRule> rules,
        ISet<string> matchedRules)
    {
        foreach (var rule in rules)
        {
            if (!OFBExportRuleValidator.TryParseTarget(rule.TargetId, out var target)
                || !string.Equals(target.ProviderId, providerId, StringComparison.OrdinalIgnoreCase))
                continue;

            if (rule.TargetKind == "person" && people.Any(person =>
                    TargetEquals(GetPersonTarget(providerId, person), rule.TargetId))
                || rule.TargetKind == "family" && families.Any(family =>
                    TargetEquals(GetFamilyTarget(providerId, family), rule.TargetId)))
            {
                matchedRules.Add(rule.Id);
                continue;
            }

            if (rule.TargetKind != "fact")
                continue;
            var ownerFacts = target.Kind == "person"
                ? people.Where(person => TargetEquals(GetPersonTarget(providerId, person), rule.TargetId))
                    .SelectMany(person => person.Facts.OfType<IGenFact>())
                : families.Where(family => TargetEquals(GetFamilyTarget(providerId, family), rule.TargetId))
                    .SelectMany(family => family.Facts.OfType<IGenFact>());
            var occurrences = ownerFacts.Where(fact =>
                string.Equals(fact.eFactType.ToString(), rule.Field, StringComparison.OrdinalIgnoreCase));
            if (rule.Occurrence is int occurrence)
                occurrences = occurrences.Skip(occurrence - 1).Take(1);
            if (occurrences.Any())
                matchedRules.Add(rule.Id);
        }
    }

    private static IEnumerable<IGenPerson> GetFamilyPeople(IGenFamily family)
    {
        if (family.Husband is not null)
            yield return family.Husband;
        if (family.Wife is not null)
            yield return family.Wife;
        foreach (var child in family.Children)
            if (child is not null)
                yield return child;
    }

    private static string? GetFactDateField(EFactType type) => type switch
    {
        EFactType.Birth => "birthDate",
        EFactType.Baptism => "baptismDate",
        EFactType.Death => "deathDate",
        EFactType.Burial => "burialDate",
        EFactType.Mariage => "marriageDate",
        _ => null
    };

    private static string? GetFactPlaceField(EFactType type) => type switch
    {
        EFactType.Birth => "birthPlace",
        EFactType.Baptism => "baptismPlace",
        EFactType.Death => "deathPlace",
        EFactType.Burial => "burialPlace",
        EFactType.Residence => "residence",
        EFactType.Occupation => "occupationPlace",
        EFactType.Mariage => "marriagePlace",
        _ => null
    };

    private static IGenDate? CloneDate(IGenDate? source) => source is null
        ? null
        : new GedcomDate
        {
            UId = source.UId,
            ID = source.ID,
            eDateModifier = source.eDateModifier,
            eDateType1 = source.eDateType1,
            Date1 = source.Date1,
            eDateType2 = source.eDateType2,
            Date2 = source.Date2,
            DateText = source.DateText
        };

    private static GedcomPlace? ClonePlace(IGenPlace? source) => source is null
        ? null
        : new GedcomPlace
        {
            UId = source.UId,
            ID = source.ID,
            Name = source.Name,
            Type = source.Type,
            GOV_ID = source.GOV_ID,
            Latitude = source.Latitude,
            Longitude = source.Longitude,
            Notes = source.Notes
        };

    private static void LinkClonedRelationships(IReadOnlyDictionary<IGenFamily, GedcomFamily> families)
    {
        foreach (var pair in families)
        {
            var family = pair.Value;
            if (family.Husband is not null)
                family.Husband.Families.Add(family);
            if (family.Wife is not null)
                family.Wife.Families.Add(family);
            if (family.Husband is not null && family.Wife is not null)
            {
                family.Husband.Spouses.Add(family.Wife);
                family.Wife.Spouses.Add(family.Husband);
            }
            foreach (var child in family.Children)
            {
                if (child is null)
                    continue;
                child.ParentFamily ??= family;
                child.Father ??= family.Husband;
                child.Mother ??= family.Wife;
                family.Husband?.Children.Add(child);
                family.Wife?.Children.Add(child);
            }
        }
    }

    private static int? ExtractYear(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var match = YearPattern().Match(value);
        return match.Success && int.TryParse(match.Value, CultureInfo.InvariantCulture, out var year)
            && year is >= 1 and <= 9999
                ? year
                : null;
    }

    [GeneratedRegex(@"(?<!\d)\d{3,4}(?!\d)", RegexOptions.CultureInvariant)]
    private static partial Regex YearPattern();
}
