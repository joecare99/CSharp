using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Genealogy.Models;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Core.Models.Gedcom;

namespace OFBCreator.Core.Services;

/// <summary>
/// Adapts the canonical genealogy document to the existing OFB rendering contracts.
/// This compatibility boundary can be removed after OFB selection and rendering consume Genealogy.Core directly.
/// </summary>
public sealed class CanonicalGenealogyAdapter
{
    public IGenealogy Adapt(GenealogyDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var genealogy = new GedcomGenealogy();
        var entitiesById = new Dictionary<Guid, GedcomEntity>();
        var peopleById = new Dictionary<Guid, GedcomPerson>();
        var familiesById = new Dictionary<Guid, GedcomFamily>();
        var placesByName = new Dictionary<string, GedcomPlace>(StringComparer.Ordinal);

        foreach (var record in document.Records)
        {
            if (record.Kind == "Person" || record.TypeCode == "INDI")
            {
                var person = CreatePerson(record, genealogy, placesByName);
                entitiesById[record.Id] = person;
                peopleById[record.Id] = person;
                genealogy.Entitys.Add(person);
            }
            else if (record.Kind == "Family" || record.TypeCode == "FAM")
            {
                var family = CreateFamily(record, genealogy, placesByName);
                entitiesById[record.Id] = family;
                familiesById[record.Id] = family;
                genealogy.Entitys.Add(family);
            }
        }

        LinkRelationships(document.Records, peopleById, familiesById);
        return genealogy;
    }

    private static GedcomPerson CreatePerson(
        GenealogyRecord record,
        GedcomGenealogy genealogy,
        IDictionary<string, GedcomPlace> placesByName)
    {
        var person = new GedcomPerson
        {
            IndRefID = GetExternalId(record),
            Name = GetContent(record, "NAME")?.Value ?? record.DisplayName ?? string.Empty,
            GivenName = record.GivenName,
            Surname = record.Surname,
            Sex = record.Sex ?? string.Empty
        };
        person.SetOwner(genealogy);

        foreach (var node in record.Content)
        {
            var type = MapFactType(node.TypeCode);
            var fact = CreateFact(node, type, person, genealogy, placesByName);
            if (fact is not null)
                person.Facts.Add(fact);
        }

        person.Occupation = GetContent(record, "OCCU")?.Value;
        person.Religion = GetContent(record, "RELI")?.Value;
        person.Birth = GetFact(person, EFactType.Birth);
        person.BirthDate = person.Birth?.Date;
        person.BirthPlace = person.Birth?.Place;
        person.Baptism = GetFact(person, EFactType.Baptism);
        person.BaptDate = person.Baptism?.Date;
        person.BaptPlace = person.Baptism?.Place;
        person.Death = GetFact(person, EFactType.Death);
        person.DeathDate = person.Death?.Date;
        person.DeathPlace = person.Death?.Place;
        person.Burial = GetFact(person, EFactType.Burial);
        person.BurialDate = person.Burial?.Date;
        person.BurialPlace = person.Burial?.Place;
        person.Residence = GetFact(person, EFactType.Residence)?.Place;
        person.OccuPlace = GetFact(person, EFactType.Occupation)?.Place;
        return person;
    }

    private static GedcomFamily CreateFamily(
        GenealogyRecord record,
        GedcomGenealogy genealogy,
        IDictionary<string, GedcomPlace> placesByName)
    {
        var family = new GedcomFamily
        {
            FamilyRefID = GetExternalId(record)
        };
        family.SetOwner(genealogy);
        foreach (var node in record.Content)
        {
            var fact = CreateFact(node, MapFactType(node.TypeCode), family, genealogy, placesByName);
            if (fact is not null)
                family.Facts.Add(fact);
        }

        family.Marriage = GetFact(family, EFactType.Mariage);
        family.MarriageDate = family.Marriage?.Date;
        family.MarriagePlace = family.Marriage?.Place;
        return family;
    }

    private static GedcomFact? CreateFact(
        GenealogyNode node,
        EFactType type,
        GedcomEntity owner,
        GedcomGenealogy genealogy,
        IDictionary<string, GedcomPlace> placesByName)
    {
        if (type == EFactType.Description && node.TypeCode.StartsWith('_'))
            return null;

        var fact = new GedcomFact
        {
            eFactType = type,
            Data = GetText(node),
            Date = CreateDate(GetContent(node, "DATE")?.Value),
            Place = CreatePlace(GetContent(node, "PLAC")?.Value, placesByName, genealogy)
        };
        fact.SetOwner(owner);
        return fact;
    }

    private static void LinkRelationships(
        IEnumerable<GenealogyRecord> records,
        IReadOnlyDictionary<Guid, GedcomPerson> people,
        IReadOnlyDictionary<Guid, GedcomFamily> families)
    {
        var recordList = records.ToList();
        foreach (var record in recordList)
        {
            if (!people.TryGetValue(record.Id, out var person))
                continue;
            foreach (var association in record.Associations)
            {
                if (association.TargetRecordId is not Guid familyId
                    || !families.TryGetValue(familyId, out var family))
                    continue;
                if (association.Role == "ChildInFamily")
                {
                    if (!family.Children.Contains(person))
                        family.Children.Add(person);
                }
                else if (association.Role == "PartnerInFamily")
                {
                    if (string.Equals(person.Sex, "M", StringComparison.OrdinalIgnoreCase))
                        family.Husband ??= person;
                    else if (string.Equals(person.Sex, "F", StringComparison.OrdinalIgnoreCase))
                        family.Wife ??= person;
                }
            }
        }

        foreach (var record in recordList)
        {
            if (!families.TryGetValue(record.Id, out var family))
                continue;
            foreach (var association in record.Associations)
            {
                if (association.TargetRecordId is not Guid targetId || !people.TryGetValue(targetId, out var person))
                    continue;
                switch (association.Role)
                {
                    case "Husband":
                        family.Husband ??= person;
                        break;
                    case "Wife":
                        family.Wife ??= person;
                        break;
                    case "Child":
                        if (!family.Children.Contains(person))
                            family.Children.Add(person);
                        break;
                }
            }

            family.FamilyName = family.Husband?.Surname ?? family.Wife?.Surname;
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

    private static IGenFact? GetFact(GedcomEntity entity, EFactType type) =>
        entity.Facts.FirstOrDefault(fact => fact?.eFactType == type);

    private static GenealogyNode? GetContent(GenealogyRecord record, string tag) =>
        record.Content.FirstOrDefault(node => node.TypeCode.Equals(tag, StringComparison.OrdinalIgnoreCase));

    private static GenealogyNode? GetContent(GenealogyNode node, string tag) =>
        node.Children.FirstOrDefault(child => child.TypeCode.Equals(tag, StringComparison.OrdinalIgnoreCase));

    private static string? GetExternalId(GenealogyRecord record) =>
        record.Identifiers.FirstOrDefault(identifier =>
            identifier.Provider.Equals("gedcom", StringComparison.OrdinalIgnoreCase))?.Value.Trim('@');

    private static string GetText(GenealogyNode node)
    {
        var text = node.Value;
        foreach (var child in node.Children)
        {
            if (child.TypeCode.Equals("CONC", StringComparison.OrdinalIgnoreCase))
                text += child.Value;
            else if (child.TypeCode.Equals("CONT", StringComparison.OrdinalIgnoreCase))
                text += Environment.NewLine + child.Value;
        }
        return text;
    }

    private static EFactType MapFactType(string tag) =>
        tag.ToUpperInvariant() switch
        {
            "GIVN" => EFactType.Givenname,
            "SURN" => EFactType.Surname,
            "TITL" => EFactType.Title,
            "NICK" => EFactType.Nickname,
            "SEX" => EFactType.Sex,
            "BIRT" => EFactType.Birth,
            "BAPM" or "BAPL" => EFactType.Baptism,
            "DEAT" => EFactType.Death,
            "BURI" => EFactType.Burial,
            "RELI" => EFactType.Religion,
            "OCCU" => EFactType.Occupation,
            "RESI" => EFactType.Residence,
            "EDUC" => EFactType.Education,
            "MARR" => EFactType.Mariage,
            "DIV" or "DIVF" => EFactType.Divorce,
            "ADOP" => EFactType.Adoption,
            "EMIG" => EFactType.Emigration,
            "IMMI" => EFactType.Immigration,
            "NATU" => EFactType.Naturalization,
            "CENS" => EFactType.Census,
            "CAST" => EFactType.Cast,
            "PROP" => EFactType.Property,
            "ORDN" => EFactType.Ordination,
            "CONF" => EFactType.Confirmation,
            "BLES" => EFactType.Blessing,
            "EVEN" or "FACT" => EFactType.Description,
            _ => EFactType.Info
        };

    private static IGenDate? CreateDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var normalized = value.Trim();
        var tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var modifier = tokens.Length == 0 ? EDateModifier.Text : tokens[0].ToUpperInvariant() switch
        {
            "BEF" => EDateModifier.Before,
            "AFT" => EDateModifier.After,
            "ABT" => EDateModifier.About,
            "EST" => EDateModifier.Estimated,
            "CAL" => EDateModifier.Calculated,
            "BET" => EDateModifier.Between,
            "FROM" => EDateModifier.From,
            "TO" => EDateModifier.To,
            _ => EDateModifier.None
        };
        var offset = modifier == EDateModifier.None ? 0 : 1;
        var valuePart = string.Join(' ', tokens.Skip(offset));
        DateTime.TryParse(valuePart, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var parsed);
        DateTime? second = null;
        var secondSeparator = modifier == EDateModifier.Between ? "AND"
            : modifier == EDateModifier.From && tokens.Contains("TO", StringComparer.OrdinalIgnoreCase) ? "TO"
            : null;
        if (secondSeparator is not null)
        {
            var separatorIndex = Array.FindIndex(tokens, offset, token =>
                token.Equals(secondSeparator, StringComparison.OrdinalIgnoreCase));
            if (separatorIndex > offset)
            {
                var firstText = string.Join(' ', tokens.Skip(offset).Take(separatorIndex - offset));
                valuePart = firstText;
                DateTime.TryParse(firstText, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsed);
                var secondText = string.Join(' ', tokens.Skip(separatorIndex + 1));
                if (DateTime.TryParse(secondText, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var parsedSecond))
                    second = parsedSecond;
                if (modifier == EDateModifier.From)
                    modifier = EDateModifier.FromTo;
            }
        }
        return new GedcomDate
        {
            DateText = value,
            Date1 = parsed,
            Date2 = second,
            eDateModifier = parsed == default && modifier == EDateModifier.None ? EDateModifier.Text : modifier,
            eDateType1 = GetDateType(valuePart),
            eDateType2 = second is null ? null : EDateType.Full
        };
    }

    private static EDateType GetDateType(string value)
    {
        var tokens = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return tokens.Length switch
        {
            1 => EDateType.Year,
            2 => EDateType.MonthYear,
            _ => EDateType.Full
        };
    }

    private static IGenPlace? CreatePlace(
        string? name,
        IDictionary<string, GedcomPlace> placesByName,
        GedcomGenealogy genealogy)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;
        if (placesByName.TryGetValue(name, out var existing))
            return existing;

        var place = new GedcomPlace { Name = name, GOV_ID = name };
        place.SetOwner(genealogy);
        placesByName.Add(name, place);
        genealogy.Places.Add(place);
        return place;
    }
}
