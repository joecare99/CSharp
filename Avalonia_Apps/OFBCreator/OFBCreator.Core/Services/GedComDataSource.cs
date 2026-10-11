using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.Extensions.Logging;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Core.Models.Gedcom;

namespace OFBCreator.Core.Services;

/// <summary>
/// GEDCOM-based family data source that reads .ged files and returns an IGenealogy payload.
/// Parses GEDCOM format directly without external dependencies.
/// TODO: Complete full entity population when GenInterfaces concrete classes are accessible.
/// </summary>
public class GedComDataSource : IFamilyDataSource
{
    private readonly ILogger<GedComDataSource>? _logger;

    public string SourceId => "gedcom";
    public string DisplayName => "GEDCOM Import (Standard)";

    public GedComDataSource(ILogger<GedComDataSource>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool CanRead(Stream stream)
    {
        if (stream == null || !stream.CanSeek)
            throw new ArgumentException("Stream must support seeking for GEDCOM detection.", nameof(stream));

        var originalPosition = stream.Position;
        try
        {
            stream.Position = 0;
            var peekLength = Math.Min(256, stream.Length);
            var buffer = new byte[peekLength];
            stream.Read(buffer, 0, buffer.Length);

            // GEDCOM files typically start with a header record: "0 HEAD" or "0 FILE"
            var text = Encoding.UTF8.GetString(buffer.TakeWhile(b => b != 0).ToArray());
            return text.TrimStart('\uFEFF', ' ', '\t', '\r', '\n')
                .StartsWith("0 HEAD", StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }

    /// <inheritdoc />
    public async Task<IGenealogy> ImportAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        _logger?.LogInformation("Importing GEDCOM data from stream...");

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var rootRecords = new List<GedcomRecord>();
        var recordStack = new Stack<GedcomRecord>();
        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line) || !TryParseGedComLine(line.AsSpan(), out var level, out var xref, out var tag, out var data))
                continue;

            var record = new GedcomRecord(level, string.IsNullOrEmpty(xref) ? null : xref, tag.ToUpperInvariant(), data);
            while (recordStack.Count > 0 && recordStack.Peek().Level >= level)
                recordStack.Pop();

            if (recordStack.Count == 0)
                rootRecords.Add(record);
            else
                recordStack.Peek().Children.Add(record);

            recordStack.Push(record);
        }

        if (rootRecords.Count == 0)
            throw new InvalidDataException("The input does not contain parseable GEDCOM records.");

        LogVersionDiagnostic(rootRecords);
        var genealogy = CreateGenealogy(rootRecords);
        var individualCount = genealogy.Entitys.OfType<IGenPerson>().Count();
        var familyCount = genealogy.Entitys.OfType<IGenFamily>().Count();

        _logger?.LogInformation("GEDCOM import complete: {PersonCount} persons, {FamilyCount} families.",
            individualCount, familyCount);

        return genealogy;
    }

    /// <summary>
    /// Parses a GEDCOM line into level, cross-reference ID, tag, and data.
    /// Format: [level] XREF@...@ TAG datum
    /// </summary>
    private static bool TryParseGedComLine(ReadOnlySpan<char> line, out int level, out string xref, out string tag, out string data)
    {
        xref = string.Empty;
        tag = string.Empty;
        data = string.Empty;

        if (line.Length < 2 || char.IsLetter(line[0]) || char.IsWhiteSpace(line[0]))
        {
            level = -1;
            return false;
        }

        var pos = 0;
        // Parse numeric level
        while (pos < line.Length && char.IsDigit(line[pos]))
            pos++;

        if (pos == 0 || pos >= line.Length || line[pos] != ' ')
        {
            level = -1;
            return false;
        }

        if (!int.TryParse(line.ToString().Substring(0, pos), out level))
        {
            level = -1;
            return false;
        }

        // Skip space after level
        pos++;

        // Parse cross-reference ID: @XXXX@
        if (pos < line.Length && line[pos] == '@')
        {
            var start = pos + 1;
            var end = start;
            while (end < line.Length && line[end] != '@')
                end++;

            if (end >= line.Length)
            {
                level = -1;
                return false;
            }

            xref = line.Slice(start, end - start).ToString();
            pos = end + 1;
        }

        // Skip space after xref
        while (pos < line.Length && char.IsWhiteSpace(line[pos]))
            pos++;

        // Parse tag (uppercase letters and digits)
        var tagStart = pos;
        while (pos < line.Length && (char.IsLetterOrDigit(line[pos]) || line[pos] == '_'))
            pos++;

        if (pos == tagStart)
        {
            level = -1;
            return false;
        }

        tag = line.ToString().Substring(tagStart, pos - tagStart);

        // Skip space before data
        while (pos < line.Length && char.IsWhiteSpace(line[pos]))
            pos++;

        data = line[pos..].ToString();

        return level >= 0;
    }

    private IGenealogy CreateGenealogy(IEnumerable<GedcomRecord> rootRecords)
    {
        var genealogy = new GedcomGenealogy();
        var people = new Dictionary<string, GedcomPerson>(StringComparer.Ordinal);
        var families = new Dictionary<string, GedcomFamily>(StringComparer.Ordinal);
        var personRecords = GetUniqueRecords(rootRecords, "INDI");
        var familyRecords = GetUniqueRecords(rootRecords, "FAM");
        var places = new Dictionary<string, GedcomPlace>(StringComparer.Ordinal);

        foreach (var record in personRecords.Values)
        {
            var person = new GedcomPerson { IndRefID = record.Xref };
            person.SetOwner(genealogy);
            PopulatePerson(person, record, places, genealogy);
            PopulateReferenceFact(person);
            people[record.Xref!] = person;
            genealogy.Entitys.Add(person);
        }

        foreach (var record in familyRecords.Values)
        {
            var family = new GedcomFamily { FamilyRefID = record.Xref };
            family.SetOwner(genealogy);
            PopulateFamily(family, record, people, places, genealogy);
            families[record.Xref!] = family;
            genealogy.Entitys.Add(family);
        }

        foreach (var personRecord in personRecords.Values)
        {
            if (!people.TryGetValue(personRecord.Xref!, out var person))
                continue;

            foreach (var parentFamilyRef in personRecord.Children.Where(child => child.Tag == "FAMC").Select(child => child.Value))
            {
                var familyId = parentFamilyRef.Trim('@');
                if (families.TryGetValue(familyId, out var parentFamily) && !parentFamily.Children.Contains(person))
                    parentFamily.Children.Add(person);
            }

            foreach (var spouseFamilyRef in personRecord.Children.Where(child => child.Tag == "FAMS").Select(child => child.Value))
            {
                if (!families.TryGetValue(spouseFamilyRef.Trim('@'), out var spouseFamily))
                    continue;

                if (string.Equals(person.Sex, "M", StringComparison.OrdinalIgnoreCase))
                    spouseFamily.Husband ??= person;
                else if (string.Equals(person.Sex, "F", StringComparison.OrdinalIgnoreCase))
                    spouseFamily.Wife ??= person;
            }
        }

        foreach (var family in families.Values)
            ConnectFamily(family);

        return genealogy;
    }

    private Dictionary<string, GedcomRecord> GetUniqueRecords(IEnumerable<GedcomRecord> records, string tag)
    {
        var uniqueRecords = new Dictionary<string, GedcomRecord>(StringComparer.Ordinal);
        foreach (var record in records.Where(record => record.Tag == tag && record.Xref is not null))
        {
            if (!uniqueRecords.TryAdd(record.Xref!, record))
                _logger?.LogWarning("Duplicate GEDCOM {RecordType} cross-reference '{CrossReference}' was ignored.", tag, record.Xref);
        }

        return uniqueRecords;
    }

    private static void PopulatePerson(GedcomPerson person, GedcomRecord record, IDictionary<string, GedcomPlace> places, GedcomGenealogy genealogy)
    {
        var name = record.Children.FirstOrDefault(child => child.Tag == "NAME");
        person.Name = name?.Value ?? string.Empty;
        person.GivenName = FindValue(name?.Children, "GIVN") ?? ParseNamePart(person.Name, false);
        person.Surname = FindValue(name?.Children, "SURN") ?? ParseNamePart(person.Name, true);
        person.Sex = FindValue(record.Children, "SEX") ?? string.Empty;
        person.ReferenceNumber = FindValue(record.Children, "REFN");
        person.Occupation = FindValue(record.Children, "OCCU");
        person.Religion = FindValue(record.Children, "RELI");
        PopulateVitalEvent(record.Children, "BIRT", places, genealogy, out var birthDate, out var birthPlace);
        person.BirthDate = birthDate;
        person.BirthPlace = birthPlace;
        PopulateVitalEvent(record.Children, "DEAT", places, genealogy, out var deathDate, out var deathPlace);
        person.DeathDate = deathDate;
        person.DeathPlace = deathPlace;
        PopulateVitalEvent(record.Children, "BAPM", places, genealogy, out var baptismDate, out var baptismPlace);
        person.BaptDate = baptismDate;
        person.BaptPlace = baptismPlace;
        PopulateVitalEvent(record.Children, "BURI", places, genealogy, out var burialDate, out var burialPlace);
        person.BurialDate = burialDate;
        person.BurialPlace = burialPlace;
    }

    private static void PopulateReferenceFact(GedcomPerson person)
    {
        if (string.IsNullOrWhiteSpace(person.ReferenceNumber))
            return;
        var fact = new GedcomFact
        {
            eFactType = EFactType.Reference,
            Data = person.ReferenceNumber
        };
        fact.SetOwner(person);
        person.Facts.Add(fact);
    }

    private static void PopulateFamily(GedcomFamily family, GedcomRecord record, IReadOnlyDictionary<string, GedcomPerson> people, IDictionary<string, GedcomPlace> places, GedcomGenealogy genealogy)
    {
        family.Husband = FindPerson(people, FindValue(record.Children, "HUSB"));
        family.Wife = FindPerson(people, FindValue(record.Children, "WIFE"));
        family.FamilyName = family.Husband?.Surname ?? family.Wife?.Surname;
        foreach (var childRef in record.Children.Where(child => child.Tag == "CHIL").Select(child => child.Value))
        {
            var child = FindPerson(people, childRef);
            if (child is not null)
                family.Children.Add(child);
        }

        var marriageRecord = record.Children.FirstOrDefault(child => child.Tag == "MARR");
        PopulateVitalEvent(record.Children, "MARR", places, genealogy, out var marriageDate, out var marriagePlace);
        family.MarriageDate = marriageDate;
        family.MarriagePlace = marriagePlace;
        if (marriageRecord is not null)
        {
            var marriage = new GedcomFact
            {
                eFactType = EFactType.Mariage,
                Data = marriageRecord.Value,
                Date = marriageDate,
                Place = marriagePlace
            };
            marriage.SetOwner(family);
            family.Marriage = marriage;
            family.Facts.Add(marriage);
        }
    }

    private static void ConnectFamily(GedcomFamily family)
    {
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

            child.ParentFamily = family;
            child.Father = family.Husband;
            child.Mother = family.Wife;
            family.Husband?.Children.Add(child);
            family.Wife?.Children.Add(child);
        }
    }

    private static void PopulateVitalEvent(IEnumerable<GedcomRecord> records, string tag, IDictionary<string, GedcomPlace> places, GedcomGenealogy genealogy, out IGenDate? date, out IGenPlace? place)
    {
        var eventRecord = records.FirstOrDefault(record => record.Tag == tag);
        date = eventRecord is null ? null : CreateDate(FindValue(eventRecord.Children, "DATE"));
        place = eventRecord is null ? null : CreatePlace(FindValue(eventRecord.Children, "PLAC"), places, genealogy);
    }

    private static IGenDate? CreateDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var gedcomDate = new GedcomDate { DateText = value, eDateModifier = EDateModifier.Text };
        var normalized = value.Trim();
        var dateTokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var dateOffset = 0;

        if (dateTokens.Length > 0)
        {
            gedcomDate.eDateModifier = dateTokens[0].ToUpperInvariant() switch
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
            if (gedcomDate.eDateModifier != EDateModifier.None)
                dateOffset = 1;
        }

        if (gedcomDate.eDateModifier == EDateModifier.Between ||
            gedcomDate.eDateModifier == EDateModifier.From && dateTokens.Contains("TO", StringComparer.OrdinalIgnoreCase))
        {
            var separator = gedcomDate.eDateModifier == EDateModifier.Between ? "AND" : "TO";
            var separatorIndex = Array.FindIndex(dateTokens, dateOffset, token => token.Equals(separator, StringComparison.OrdinalIgnoreCase));
            if (separatorIndex > dateOffset)
            {
                var firstDate = string.Join(' ', dateTokens.Skip(dateOffset).Take(separatorIndex - dateOffset));
                var secondDate = string.Join(' ', dateTokens.Skip(separatorIndex + 1));
                if (TryParseDate(firstDate, out var first) && TryParseDate(secondDate, out var second))
                {
                    gedcomDate.eDateModifier = gedcomDate.eDateModifier == EDateModifier.Between ? EDateModifier.Between : EDateModifier.FromTo;
                    gedcomDate.Date1 = first;
                    gedcomDate.Date2 = second;
                    gedcomDate.eDateType1 = GetDateType(firstDate);
                    gedcomDate.eDateType2 = GetDateType(secondDate);
                }
            }
            return gedcomDate;
        }

        var dateValue = string.Join(' ', dateTokens.Skip(dateOffset));
        if (TryParseDate(dateValue, out var parsedDate))
        {
            gedcomDate.Date1 = parsedDate;
            gedcomDate.eDateType1 = GetDateType(dateValue);
        }
        else if (gedcomDate.eDateModifier == EDateModifier.None)
        {
            gedcomDate.eDateModifier = EDateModifier.Text;
        }

        return gedcomDate;
    }

    private static bool TryParseDate(string value, out DateTime date)
    {
        string[] formats = ["d MMM yyyy", "dd MMM yyyy", "d.M.yyyy", "dd.MM.yyyy", "d/M/yyyy", "dd/MM/yyyy", "MMM yyyy", "MMMM yyyy", "yyyy"];
        return DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out date);
    }

    private static EDateType GetDateType(string value)
    {
        var tokens = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 1)
            return EDateType.Year;
        if (tokens.Length == 2)
            return EDateType.MonthYear;
        return EDateType.Full;
    }

    private void LogVersionDiagnostic(IEnumerable<GedcomRecord> rootRecords)
    {
        var header = rootRecords.FirstOrDefault(record => record.Tag == "HEAD");
        var version = FindValue(header?.Children.FirstOrDefault(child => child.Tag == "GEDC")?.Children, "VERS");
        if (version is not ("5.5" or "5.5.1" or "7" or "7.0"))
            _logger?.LogWarning("GEDCOM header reports unrecognized version '{Version}'. Import will continue with compatible common tags.", version ?? "unspecified");
    }

    private static IGenPlace? CreatePlace(string? value, IDictionary<string, GedcomPlace> places, GedcomGenealogy genealogy)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (places.TryGetValue(value, out var place))
            return place;

        place = new GedcomPlace { Name = value, GOV_ID = value };
        place.SetOwner(genealogy);
        places.Add(value, place);
        genealogy.Places.Add(place);
        return place;
    }

    private static GedcomPerson? FindPerson(IReadOnlyDictionary<string, GedcomPerson> people, string? reference)
    {
        return reference is not null && people.TryGetValue(reference.Trim('@'), out var person) ? person : null;
    }

    private static string? FindValue(IEnumerable<GedcomRecord>? records, string tag)
    {
        var record = records?.FirstOrDefault(child => child.Tag == tag);
        return record is null ? null : GetText(record);
    }

    private static string GetText(GedcomRecord record)
    {
        var builder = new StringBuilder(record.Value);
        foreach (var continuation in record.Children.Where(child => child.Tag is "CONC" or "CONT"))
        {
            if (continuation.Tag == "CONT")
                builder.AppendLine();
            builder.Append(continuation.Value);
        }
        return builder.ToString();
    }

    private static string? ParseNamePart(string name, bool surname)
    {
        var firstDelimiter = name.IndexOf('/');
        var lastDelimiter = name.LastIndexOf('/');
        if (firstDelimiter >= 0 && lastDelimiter > firstDelimiter)
            return surname ? name.Substring(firstDelimiter + 1, lastDelimiter - firstDelimiter - 1).Trim() : name.Substring(0, firstDelimiter).Trim();
        return surname ? null : name.Trim();
    }
}