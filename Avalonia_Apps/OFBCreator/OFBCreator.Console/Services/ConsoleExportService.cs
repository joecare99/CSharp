using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

using BaseLib.Models.Interfaces;
using Document.Docx;
using Document.Base.Models.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Abstractions.Models;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;

/// <summary>
/// Headless export service that orchestrates the full OFB (Ortsfamilienbuch) generation pipeline.
/// </summary>
/// <remarks>
/// Responsibilities:
/// 1. Load genealogical data via IFamilyDataSource dependency injection.
/// 2. Build name groups and sort families by FamilyName + FormationDate.
/// 3. Assign global family numbers (4-5 digit with leading zeros).
/// 4. Generate all indices (Person, Occupation, Property, PlaceHierarchy, PlaceAlphabetical).
/// 5. Compose the OFB document via IUserDocument abstraction (DOCX or ODT).
/// </remarks>
public sealed class ConsoleExportService( IFamilyDataSource dataSource, IUserDocumentFactory documentFactory )
{
    private readonly IFamilyDataSource _dataSource = dataSource;
    private readonly IUserDocumentFactory _documentFactory = documentFactory;
    private readonly Dictionary<string, string> _personReferenceBySortKey = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _personIndexAnchorByReference = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _familyAnchorBySourceReference = new(StringComparer.Ordinal);

    /// <summary>
    /// Executes the full OFB generation pipeline for the given options.
    /// </summary>
    public async Task ExportAsync( OFBGenerateOptions options )
    {
        ArgumentNullException.ThrowIfNull( options );

        if ( string.IsNullOrWhiteSpace( options.InputPath ) )
            throw new ArgumentException( "GEDCOM input path is required.", nameof( options ) );

        if ( string.IsNullOrWhiteSpace( options.OutputPath ) )
            throw new ArgumentException( "Output path is required.", nameof( options ) );

        if ( string.IsNullOrWhiteSpace( options.Title ) )
            throw new ArgumentException( "OFB title is required.", nameof( options ) );

        if ( !File.Exists( options.InputPath ) )
            throw new FileNotFoundException( $"GEDCOM input file was not found: '{options.InputPath}'.", options.InputPath );

        // Step 1: Load genealogical data via IFamilyDataSource (GEDCOM or other source)
        System.Console.WriteLine( $"Loading from {_dataSource.DisplayName}..." );
        using var fs = new FileStream( options.InputPath, FileMode.Open, FileAccess.Read, FileShare.Read );
        var genealogy = await _dataSource.ImportAsync( fs );
        var allFamilies = genealogy.Entitys.OfType<IGenFamily>().ToList();

        if ( allFamilies.Count == 0 )
            throw new InvalidOperationException( $"No families found in '{options.InputPath}'" );

        // Step 2: Build name groups and filter by place
        System.Console.WriteLine( "Building family name groups..." );
        var nameGrouping = new FamilyNameGroupBuilder();
        nameGrouping.BuildGroups( allFamilies );

        // Step 3: Convert Abstraction models to Core models, sort, and assign global numbers
        System.Console.WriteLine( "Sorting and numbering families..." );
        var sorter = new OFBFamilySorter();
        var abFamilies = sorter.SortAndNumber( allFamilies, options.PlaceId, options.IncludeDescendants );
        var groupByFamilyReference = nameGrouping.GroupKeys
            .SelectMany(groupName => (nameGrouping.GetGroup(groupName) ?? Array.Empty<IGenFamily>())
                .Where(family => !string.IsNullOrWhiteSpace(family.FamilyRefID))
                .Select(family => (family.FamilyRefID!, groupName)))
            .GroupBy(pair => pair.Item1, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First().groupName, StringComparer.Ordinal);

        var germanComparer = StringComparer.Create(CultureInfo.GetCultureInfo("de-DE"), ignoreCase: true);
        var orderedFamilies = abFamilies
            .OrderBy(family => groupByFamilyReference.TryGetValue(family.SourceRefId, out var groupName) ? groupName : family.FamilyName, germanComparer)
            .ThenBy(family => family.FamilyName, germanComparer)
            .ThenBy(family => family.FormationDate ?? DateTime.MaxValue)
            .ThenBy(family => family.SourceRefId, StringComparer.Ordinal)
            .ToArray();
        for (var index = 0; index < orderedFamilies.Length; index++)
            orderedFamilies[index].GlobalNumber = (index + 1).ToString("D5", CultureInfo.InvariantCulture);

        // Convert Abstractions.OFBFamilyModel → Core.OFBFamily
        var sortedFamilies = ConvertToCoreFamilies(orderedFamilies);
        _familyAnchorBySourceReference.Clear();
        foreach (var family in sortedFamilies)
        {
            if (!string.IsNullOrWhiteSpace(family.SourceRefId))
                _familyAnchorBySourceReference[family.SourceRefId] = GetFamilyAnchor(family.GlobalNumber);
        }

        // Step 4: Generate indices
        System.Console.WriteLine( "Generating Person Index..." );
        var personIndex = await GeneratePersonIndexAsync( sortedFamilies );

        System.Console.WriteLine( "Generating Occupation Index..." );
        var occIndex = await GenerateOccupationIndexAsync( sortedFamilies );

        System.Console.WriteLine( "Generating Property Index..." );
        var propIndex = await GeneratePropertyIndexAsync( sortedFamilies );

        System.Console.WriteLine( "Generating Place Hierarchy Index..." );
        var placeHierarchyIndex = await GeneratePlaceHierarchyIndexAsync( sortedFamilies );

        System.Console.WriteLine( "Generating Place Alphabetical Index..." );
        var placeAlphaIndex = await GeneratePlaceAlphabeticalIndexAsync( sortedFamilies );

        // Step 5: Create document and compose content
        System.Console.WriteLine( "Composing document..." );
        var doc = _documentFactory.CreateDocument( options.UseDocxFormat ? OFBOutputFormat.Docx : OFBOutputFormat.Odt );

        await ComposeTitlePageAsync( doc, options );
        await ComposePrefaceAsync( doc, options );
        await ComposeFamiliesAsync(doc, sortedFamilies, options.EntryFormat, groupByFamilyReference);
        await ComposeIndicesAsync( doc, personIndex, occIndex, propIndex, placeHierarchyIndex, placeAlphaIndex );
        await ComposeLegendAsync( doc, options.Legend ?? string.Empty );

        // Step 6: Save document
        System.Console.WriteLine( "Saving output..." );
        var outputDirectory = Path.GetDirectoryName( Path.GetFullPath( options.OutputPath ) );
        if ( !string.IsNullOrEmpty( outputDirectory ) )
            Directory.CreateDirectory( outputDirectory );

        var success = doc.SaveTo( options.OutputPath );
        if ( !success )
            throw new InvalidOperationException( $"Failed to save OFB document to '{options.OutputPath}'" );
    }

    /// <summary>
    /// Composes the title page with OFB title, place name, and date.
    /// </summary>
    private async Task ComposeTitlePageAsync( IUserDocument doc, OFBGenerateOptions options )
    {
        // Title (headline level 1)
        var titleHeadline = doc.AddHeadline( 1, "Titel" );
        titleHeadline.TextContent = options.Title;

        // Subtitle with place info
        var placeInfo = string.IsNullOrEmpty( options.PlaceId ) ? "alle Orte" : $"Ort: {options.PlaceId}";
        var subtitle = doc.AddParagraph( "Normaler Absatz" );
        subtitle.TextContent = $"{placeInfo}, erstellt {DateTime.Now:yyyy-MM-dd}";
    }

    /// <summary>
    /// Composes the preface section if provided.
    /// </summary>
    private async Task ComposePrefaceAsync( IUserDocument doc, OFBGenerateOptions options )
    {
        if ( string.IsNullOrWhiteSpace( options.Preface ) )
            return;

        var prefaceHeadline = doc.AddHeadline( 1, "Vorwort" );
        prefaceHeadline.TextContent = "Vorwort";
        var prefacePara = doc.AddParagraph( "Normaler Absatz" );
        prefacePara.TextContent = options.Preface;
    }

    /// <summary>
    /// Composes the character explanation/legend showing OFB symbols.
    /// </summary>
    private async Task ComposeLegendAsync( IUserDocument doc, string legend )
    {
        var legendHeadline = doc.AddHeadline( 1, "Zeichenerklärung" );
        legendHeadline.TextContent = "Zeichenerklärung";

        // Standard OFB character conventions (user confirmed)
        var lines = new[]
        {
            "* = Geboren",
            "~ = Getauft",
            "+ = Gestorben",
            "= = Begraben",
            "",
            "oo = Hochzeit (verheiratet)",
            "o-o = Verbindung (unverheiratet)",
            "o/o = Geschieden"
        };

        foreach ( var line in lines )
        {
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.TextContent = line;
        }

        if ( !string.IsNullOrWhiteSpace( legend ) )
        {
            var legendPara = doc.AddParagraph( "Normaler Absatz" );
            legendPara.TextContent = $"\n{legend}";
        }
    }

    /// <summary>
    /// Composes all index sections (Person, Occupation, Property, Place).
    /// </summary>
    private async Task ComposeIndicesAsync( IUserDocument doc, OFBIndexEntry[] personIndex,
        OFBIndexEntry[] occIndex, OFBIndexEntry[] propIndex,
        OFBPlaceHierarchyNode[] placeHierarchy, OFBIndexEntry[] placeAlpha )
    {
        // Person Index (alphabetical)
        var personTitle = doc.AddHeadline( 1, "Personenindex" );
        personTitle.TextContent = "Personenindex";
        for (var index = 0; index < personIndex.Length; index++)
        {
            var entry = personIndex[index];
            var para = doc.AddParagraph( "Normaler Absatz" );
            var entryAnchor = CreateIndexAnchor("person", index);
            para.AddBookmark(entryAnchor, DocxFontStyle.Default).TextContent = entry.SortKey;
            var personReference = FindPersonReference(entry.SortKey);
            if (personReference is not null)
                _personIndexAnchorByReference[personReference] = entryAnchor;
            var targetAnchor = personReference is null ? ResolveFamilyAnchor(entry.Ref) : GetPersonAnchor(personReference);
            if (targetAnchor is null)
                para.AppendText($" — Fam. {entry.Ref}");
            else
                para.AddLink($"#{targetAnchor}", DocxFontStyle.UnderlineStyle).TextContent = $" — Fam. {entry.Ref}";
        }

        // Occupation Index (alphabetical)
        var occTitle = doc.AddHeadline( 1, "Berufsindex" );
        occTitle.TextContent = "Berufsindex";
        for (var index = 0; index < occIndex.Length; index++)
        {
            var entry = occIndex[index];
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.AddBookmark(CreateIndexAnchor("occupation", index), DocxFontStyle.Default).TextContent = entry.SortKey;
            AppendFamilyReference(para, entry.Ref);
        }

        // Property Index (alphabetical, decision 1A)
        var propTitle = doc.AddHeadline( 1, "Besitzindex" );
        propTitle.TextContent = "Eigentums-/Besitz-Index";
        for (var index = 0; index < propIndex.Length; index++)
        {
            var entry = propIndex[index];
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.AddBookmark(CreateIndexAnchor("property", index), DocxFontStyle.Default).TextContent = entry.SortKey;
            AppendFamilyReference(para, entry.Ref);
        }

        // Place Hierarchy Index (tree structure)
        var phTitle = doc.AddHeadline( 1, "Ortsindex (Hierarchisch)" );
        phTitle.TextContent = "Ortsindex";
        await WritePlaceHierarchyAsync( doc, placeHierarchy, indent: 0 );

        // Place Alphabetical Index (A-Z flat list)
        var paTitle = doc.AddHeadline( 1, "Ortsindex (Alphabetisch)" );
        paTitle.TextContent = "Ortsindex Alphabetisch";
        for (var index = 0; index < placeAlpha.Length; index++)
        {
            var entry = placeAlpha[index];
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.AddBookmark(CreateIndexAnchor("place", index), DocxFontStyle.Default).TextContent = entry.SortKey;
            AppendFamilyReference(para, entry.Ref);
        }
    }

    /// <summary>
    /// Recursively writes place hierarchy nodes as indented paragraphs.
    /// </summary>
    private async Task WritePlaceHierarchyAsync( IUserDocument doc, OFBPlaceHierarchyNode[] nodes, int indent )
    {
        foreach ( var node in nodes )
        {
            var prefix = new string( ' ', indent * 2 );
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.TextContent = $"{prefix}{node.Name} ({node.PlaceId})";

            if ( node.Children != null && node.Children.Length > 0 )
                await WritePlaceHierarchyAsync( doc, node.Children, indent + 1 );
        }
    }

    /// <summary>
    /// Composes the main body: all families with full details.
    /// </summary>
    private async Task ComposeFamiliesAsync(
        IUserDocument doc,
        IReadOnlyList<OFBFamily> sortedFamilies,
        OFBEntryFormat entryFormat,
        IReadOnlyDictionary<string, string> groupByFamilyReference)
    {
        string? currentGroupName = null;
        string? currentFamilyName = null;
        foreach (var family in sortedFamilies)
        {
            var groupName = GetFamilyGroupName(family, groupByFamilyReference);
            var individualFamilyName = GetIndividualFamilyName(family);
            if (!string.Equals(groupName, currentGroupName, StringComparison.Ordinal))
            {
                var groupHeadline = doc.AddHeadline(1, $"group-{SanitizeAnchor(groupName)}-{family.GlobalNumber}");
                groupHeadline.TextContent = groupName;
                currentGroupName = groupName;
                currentFamilyName = null;
            }

            if (!string.Equals(individualFamilyName, currentFamilyName, StringComparison.Ordinal))
            {
                var familyNameHeadline = doc.AddHeadline(2, $"family-name-{SanitizeAnchor(individualFamilyName)}-{family.GlobalNumber}");
                familyNameHeadline.TextContent = individualFamilyName;
                currentFamilyName = individualFamilyName;
            }

            var header = $"{family.GlobalNumber}";
            var headline = doc.AddHeadline(3, GetFamilyAnchor(family.GlobalNumber));
            headline.TextContent = header;

            if ( entryFormat == OFBEntryFormat.GC )
                ComposeGcFamilyEntry(doc, family);
            else
                ComposeAkFamilyEntry(doc, family);

            var blankPara = doc.AddParagraph( "Normaler Absatz" );
            blankPara.TextContent = "";
        }
    }

    private static string GetFamilyGroupName(OFBFamily family, IReadOnlyDictionary<string, string> groupByFamilyReference)
    {
        if (groupByFamilyReference.TryGetValue(family.SourceRefId, out var groupName))
            return groupName;

        return family.FamilyName;
    }

    private static string GetIndividualFamilyName(OFBFamily family)
    {
        return family.FamilyName;
    }

    private void ComposeGcFamilyEntry(IUserDocument doc, OFBFamily family)
    {
        var marriage = doc.AddParagraph("Normaler Absatz");
        marriage.TextContent = $"Ehe: {FormatFamilyEvent(family)}";

        AppendGcPartner(doc, family.Husband);
        AppendGcPartner(doc, family.Wife);
        AppendChildren(doc, family, OFBEntryFormat.GC);
    }

    private void AppendGcPartner(IUserDocument doc, IGenPerson? person)
    {
        if (person is null)
            return;

        AppendPersonDetails(doc, person);
        var paragraph = doc.AddParagraph("Normaler Absatz");
        paragraph.TextContent = $"PN = {person.IndRefID ?? person.Name}";
        AppendPersonIndexBacklink(paragraph, person);
    }

    private void ComposeAkFamilyEntry(IUserDocument doc, OFBFamily family)
    {
        var marriage = doc.AddParagraph("Normaler Absatz");
        var familyEvent = FormatFamilyEvent(family);
        marriage.TextContent = string.IsNullOrEmpty(familyEvent) ? "⚭" : $"⚭ {familyEvent}";

        AppendAkPartner(doc, family.Husband);
        AppendAkPartner(doc, family.Wife);

        if (family.Children.Count > 0)
        {
            var childrenHeading = doc.AddParagraph("Normaler Absatz");
            childrenHeading.TextContent = $"{family.Children.Count} Kdr:";
        }

        AppendChildren(doc, family, OFBEntryFormat.AK);
    }

    private void AppendAkPartner(IUserDocument doc, IGenPerson? person)
    {
        if (person is null)
            return;

        var paragraph = doc.AddParagraph("Normaler Absatz");
        AppendBookmarkedPersonName(paragraph, person);
        AppendPersonEventDetails(paragraph, person);
        AppendPersonIndexBacklink(paragraph, person);
    }

    private void AppendChildren(IUserDocument doc, OFBFamily family, OFBEntryFormat entryFormat)
    {
        foreach (var child in family.Children)
        {
            var birth = child.BirthDate?.ToString();
            var childParagraph = doc.AddParagraph("Normaler Absatz");
            childParagraph.AddBookmark(GetPersonAnchor(child.IndRefID ?? child.Name), DocxFontStyle.Default).TextContent = string.Empty;
            childParagraph.TextContent = $"- {FormatPersonName(child, entryFormat)}{(string.IsNullOrWhiteSpace(birth) ? string.Empty : $" {birth}")}";
            AppendPersonIndexBacklink(childParagraph, child);
        }
    }

    private static string FormatFamilyEvent(OFBFamily family)
    {
        var date = family.MarriageDate?.ToString();
        var place = family.MarriagePlace?.Name;
        return string.Join(" in ", new[] { date, place }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private void AppendBookmarkedPersonName(IDocParagraph paragraph, IGenPerson person)
    {
        var personName = FormatPersonName(person, OFBEntryFormat.AK);
        paragraph.AddBookmark(GetPersonAnchor(person.IndRefID ?? personName), DocxFontStyle.Default).TextContent = personName;
        AppendPersonEventDetails(paragraph, person);
        AppendPersonIndexBacklink(paragraph, person);
    }

    private static string FormatPersonName(IGenPerson person, OFBEntryFormat entryFormat)
    {
        var givenName = person.GivenName?.Trim();
        var surname = person.Surname?.Trim();
        if (!string.IsNullOrWhiteSpace(givenName) && !string.IsNullOrWhiteSpace(surname))
            return entryFormat == OFBEntryFormat.GC
                ? $"{surname}, {givenName}"
                : $"{givenName} {surname}";
        if (!string.IsNullOrWhiteSpace(givenName))
            return givenName;
        if (!string.IsNullOrWhiteSpace(surname))
            return surname;

        var name = person.Name?.Trim() ?? string.Empty;
        var normalized = name.Replace("/", " ", StringComparison.Ordinal);
        return string.Join(' ', normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    private void AppendPersonEventDetails(IDocParagraph paragraph, IGenPerson person)
    {
        if (person.BirthDate is not null)
            paragraph.TextContent += $", * {person.BirthDate}";
        if (person.DeathDate is not null)
            paragraph.TextContent += $", † {person.DeathDate}";
    }

    private static string GetFamilyAnchor(string familyNumber) => $"family-{familyNumber}";

    private string? ResolveFamilyAnchor(string reference)
    {
        var sourceReference = reference.StartsWith("Fam.", StringComparison.Ordinal)
            ? reference[4..].TrimEnd('K')
            : reference;
        return _familyAnchorBySourceReference.TryGetValue(sourceReference, out var anchor)
            ? anchor
            : int.TryParse(sourceReference, out _) ? GetFamilyAnchor(sourceReference) : null;
    }

    private void AppendFamilyReference(IDocParagraph paragraph, string reference)
    {
        var targetAnchor = ResolveFamilyAnchor(reference);
        if (targetAnchor is null)
            paragraph.AppendText($" — {reference}");
        else
            paragraph.AddLink($"#{targetAnchor}", DocxFontStyle.UnderlineStyle).TextContent = $" — {reference}";
    }

    private static string GetPersonAnchor(string personReference) => $"person-{SanitizeAnchor(personReference)}";

    private string? FindPersonReference(string personSortKey) =>
        _personReferenceBySortKey.TryGetValue(personSortKey, out var personReference)
            ? personReference
            : null;

    private static string CreateIndexAnchor(string category, int ordinal) => $"index-{category}-{ordinal + 1}";

    private static string SanitizeAnchor(string value)
    {
        var characters = value.Select(character => char.IsLetterOrDigit(character) ? character : '-').ToArray();
        return new string(characters).Trim('-');
    }

    /// <summary>
    /// Appends person details (vital events) to the document.
    /// </summary>
    private void AppendPersonDetails( IUserDocument doc, IGenPerson person )
    {
        var parts = new List<string>() { FormatPersonName(person, OFBEntryFormat.GC) };

        if ( !string.IsNullOrEmpty( person.BirthDate?.ToString() ) )
            parts.Add( $"* {person.BirthDate}" );
        if ( !string.IsNullOrEmpty( person.DeathDate?.ToString() ) )
            parts.Add( $"+ {person.DeathDate}" );

        var detailLine = string.Join( "  ", parts );
        var para = doc.AddParagraph( "Normaler Absatz" );
        para.AddBookmark(GetPersonAnchor(person.IndRefID ?? person.Name), DocxFontStyle.Default).TextContent = string.Empty;
        para.TextContent = detailLine;
        AppendPersonIndexBacklink(para, person);
    }

    private void AppendPersonIndexBacklink(IDocParagraph paragraph, IGenPerson person)
    {
        var personReference = person.IndRefID ?? person.Name;
        if (_personIndexAnchorByReference.TryGetValue(personReference, out var indexAnchor))
            paragraph.AddLink($"#{indexAnchor}", DocxFontStyle.UnderlineStyle).TextContent = " [Index]";
    }

    /// <summary>
    /// Generates alphabetical person index entries for all individuals.
    /// </summary>
    private async Task<OFBIndexEntry[]> GeneratePersonIndexAsync( IReadOnlyList<OFBFamily> sortedFamilies )
    {
        // Collect unique persons from all families (use OFBFamily which has GlobalNumber)
        var seen = new HashSet<string>();
        var entries = new List<OFBIndexEntry>();
        _personReferenceBySortKey.Clear();
        _personIndexAnchorByReference.Clear();

        foreach ( var family in sortedFamilies )
        {
            if ( family.Husband != null )
            {
                var key = family.Husband.IndRefID ?? family.Husband.Name;
                if ( seen.Add( key ) )
                {
                    var entry = OFBIndexEntry.FromCombined( family.Husband.Surname ?? string.Empty, family.Husband.GivenName ?? string.Empty, $"Fam.{family.GlobalNumber}" );
                    entries.Add(entry);
                    _personReferenceBySortKey.TryAdd(entry.SortKey, key);
                }
            }

            if ( family.Wife != null )
            {
                var key = family.Wife.IndRefID ?? family.Wife.Name;
                if ( seen.Add( key ) )
                {
                    var entry = OFBIndexEntry.FromCombined( family.Wife.Surname ?? string.Empty, family.Wife.GivenName ?? string.Empty, $"Fam.{family.GlobalNumber}" );
                    entries.Add(entry);
                    _personReferenceBySortKey.TryAdd(entry.SortKey, key);
                }
            }

            foreach ( var child in family.Children )
            {
                var key = child.IndRefID ?? child.Name;
                if ( seen.Add( key ) )
                {
                    var entry = OFBIndexEntry.FromCombined( child.Surname ?? string.Empty, child.GivenName ?? string.Empty, $"Fam.{family.GlobalNumber}K" );
                    entries.Add(entry);
                    _personReferenceBySortKey.TryAdd(entry.SortKey, key);
                }
            }
        }

        // Sort alphabetically by full name key
        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        _personIndexAnchorByReference.Clear();
        for (var index = 0; index < entries.Count; index++)
        {
            if (_personReferenceBySortKey.TryGetValue(entries[index].SortKey, out var personReference))
                _personIndexAnchorByReference.TryAdd(personReference, CreateIndexAnchor("person", index));
        }
        return [.. entries];
    }

    /// <summary>
    /// Generates alphabetical occupation index with person/family references.
    /// </summary>
    private async Task<OFBIndexEntry[]> GenerateOccupationIndexAsync( IReadOnlyList<OFBFamily> sortedFamilies )
    {
        var entries = new List<OFBIndexEntry>();
        var seen = new HashSet<string>( StringComparer.Ordinal );

        foreach ( var family in sortedFamilies )
        {
            foreach ( var person in new[] { family.Husband, family.Wife }.Where( p => p != null ) )
                if ( !string.IsNullOrEmpty( person.Occupation ) && seen.Add( person.Occupation ) )
                    entries.Add( OFBIndexEntry.FromSingle( person.Occupation, $"Fam.{family.GlobalNumber}" ) );

            foreach ( var child in family.Children )
                if ( !string.IsNullOrEmpty( child.Occupation ) && seen.Add( child.Occupation ) )
                    entries.Add( OFBIndexEntry.FromSingle( child.Occupation, $"Fam.{family.GlobalNumber}K" ) );
        }

        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        return [.. entries];
    }

    /// <summary>
    /// Generates property/ownership index from family data.
    /// Implements decision 1A: Property Index included.
    /// </summary>
    private async Task<OFBIndexEntry[]> GeneratePropertyIndexAsync( IReadOnlyList<OFBFamily> sortedFamilies )
    {
        var entries = new List<OFBIndexEntry>();

        foreach ( var family in sortedFamilies )
        {
            // Extract property info from family facts/narratives
            var properties = ExtractPropertiesFromFamily( family );
            foreach ( var prop in properties )
                entries.Add( OFBIndexEntry.FromSingle( prop, $"Fam.{family.GlobalNumber}" ) );
        }

        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        return [.. entries];
    }

    /// <summary>
    /// Extracts property names from family facts.
    /// Placeholder — actual extraction depends on GEDCOM NARR/NOTE parsing.
    /// </summary>
    private IEnumerable<string> ExtractPropertiesFromFamily( OFBFamily family )
    {
        // TODO: Parse family notes/facts for property references (e.g., "Besitz", "Eigentum")
        yield break;
    }

    /// <summary>
    /// Converts abstraction-layer OFBFamilyModel instances to Core-layer OFBFamily instances.
    /// Both models share identical properties; this bridges the layer boundary.
    /// </summary>
    private static IReadOnlyList<OFBFamily> ConvertToCoreFamilies( IReadOnlyList<OFBFamilyModel> models )
    {
        var result = new OFBFamily[models.Count];
        for ( int i = 0; i < models.Count; i++ )
        {
            var m = models[i];
            result[i] = new OFBFamily
            {
                GlobalNumber   = m.GlobalNumber,
                FamilyName     = m.FamilyName,
                Husband        = m.Husband,
                Wife           = m.Wife,
                Children       = m.Children,
                MarriageDate   = m.MarriageDate,
                MarriagePlace  = m.MarriagePlace,
                FormationDate  = m.FormationDate,
                SourceRefId    = m.SourceRefId
            };
        }
        return result;
    }

    /// <summary>
    /// Generates hierarchical place index from all families in the output set.
    /// </summary>
    private async Task<OFBPlaceHierarchyNode[]> GeneratePlaceHierarchyIndexAsync( IReadOnlyList<OFBFamily> families )
    {
        // Collect unique places and build tree structure
        var placeMap = new Dictionary<string, OFBPlaceHierarchyNode>( StringComparer.Ordinal );

        foreach ( var family in families )
        {
            if ( !string.IsNullOrEmpty( family.MarriagePlace?.GOV_ID ) && !placeMap.ContainsKey( family.MarriagePlace.GOV_ID ) )
                placeMap[family.MarriagePlace.GOV_ID] = new OFBPlaceHierarchyNode( family.MarriagePlace.Name, family.MarriagePlace.GOV_ID );

            foreach ( var child in family.Children )
            {
                if ( child.BirthPlace != null && !string.IsNullOrEmpty( child.BirthPlace.GOV_ID ) )
                    placeMap[child.BirthPlace.GOV_ID] = new OFBPlaceHierarchyNode( child.BirthPlace.Name, child.BirthPlace.GOV_ID );

                // Add parents' birth places too
                if ( family.Husband?.BirthPlace != null && !string.IsNullOrEmpty( family.Husband.BirthPlace.GOV_ID ) )
                    placeMap[family.Husband.BirthPlace.GOV_ID] = new OFBPlaceHierarchyNode( family.Husband.BirthPlace.Name, family.Husband.BirthPlace.GOV_ID );

                if ( family.Wife?.BirthPlace != null && !string.IsNullOrEmpty( family.Wife.BirthPlace.GOV_ID ) )
                    placeMap[family.Wife.BirthPlace.GOV_ID] = new OFBPlaceHierarchyNode( family.Wife.BirthPlace.Name, family.Wife.BirthPlace.GOV_ID );
            }
        }

        // TODO: Build parent-child relationships from GEDCOM place hierarchy data
        return [.. placeMap.Values];
    }

    /// <summary>
    /// Generates flat alphabetical place index (A-Z) for cross-reference.
    /// Implements decision 2A: Place Alphabetical Index included.
    /// </summary>
    private async Task<OFBIndexEntry[]> GeneratePlaceAlphabeticalIndexAsync( IReadOnlyList<OFBFamily> families )
    {
        var seen = new HashSet<string>( StringComparer.Ordinal );
        var entries = new List<OFBIndexEntry>();

        foreach ( var family in families )
        {
            if ( !string.IsNullOrEmpty( family.MarriagePlace?.Name ) && seen.Add( family.MarriagePlace.Name ) )
                entries.Add( OFBIndexEntry.FromSingle( family.MarriagePlace.Name, $"Fam.{family.SourceRefId}" ) );

            foreach ( var child in family.Children )
            {
                if ( child.BirthPlace != null && !string.IsNullOrEmpty( child.BirthPlace.Name ) && seen.Add( child.BirthPlace.Name ) )
                    entries.Add( OFBIndexEntry.FromSingle( child.BirthPlace.Name, $"Fam.{family.SourceRefId}K" ) );
            }
        }

        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        return [.. entries];
    }
}
