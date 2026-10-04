using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

using BaseLib.Models.Interfaces;
using Document.Docx;
using Document.Base.Models.Interfaces;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Abstractions.Models;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;
using OFBCreator.Console.Models;
using OFBCreator.Console.Services.Templates;

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
public sealed class ConsoleExportService(
    IFamilyDataSource dataSource,
    IUserDocumentFactory documentFactory,
    EntryTemplateStore? templateStore = null)
{
    private readonly IFamilyDataSource _dataSource = dataSource;
    private readonly IUserDocumentFactory _documentFactory = documentFactory;
    private readonly EntryTemplateStore _templateStore = templateStore ?? new EntryTemplateStore();
    private readonly Dictionary<string, string> _personReferenceBySortKey = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _personIndexAnchorByReference = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _occupationIndexAnchorByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _familyAnchorBySourceReference = new(StringComparer.Ordinal);

    /// <summary>
    /// Executes the full OFB generation pipeline for the given options.
    /// </summary>
    public async Task ExportAsync(OFBGenerateOptions options, CancellationToken cancellationToken = default)
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

        var entryTemplate = _templateStore.Load(options.Template);
        if (!string.Equals(entryTemplate.EntryRoot, "Family", StringComparison.Ordinal))
            throw new InvalidDataException(
                $"Template '{entryTemplate.Id}' uses the Individual root, but the current export pipeline generates Family entries.");

        System.Console.WriteLine($"Loading from {_dataSource.DisplayName}...");
        var preparation = await PrepareGroupingAsync(options, cancellationToken).ConfigureAwait(false);
        var allFamilies = preparation.Families;
        var groupingResult = preparation.GroupingResult;
        foreach (var diagnostic in groupingResult.Diagnostics)
            System.Console.Error.WriteLine($"Warning [{diagnostic.Code}]: {diagnostic.Message}");

        // Step 3: Convert Abstraction models to Core models, sort, and assign global numbers
        System.Console.WriteLine( "Sorting and numbering families..." );
        var sorter = new OFBFamilySorter();
        var abFamilies = sorter.SortAndNumber(allFamilies);
        var groupByFamilyReference = groupingResult.Groups
            .SelectMany(group => group.Value
                .Where(family => !string.IsNullOrWhiteSpace(family.FamilyRefID))
                .Select(family => (FamilyReference: family.FamilyRefID!, GroupName: group.Key)))
            .GroupBy(pair => pair.FamilyReference, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First().GroupName, StringComparer.Ordinal);

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
        _occupationIndexAnchorByName.Clear();
        for (var index = 0; index < occIndex.Length; index++)
        {
            var occupationName = string.IsNullOrWhiteSpace(occIndex[index].Name)
                ? occIndex[index].SortKey
                : occIndex[index].Name;
            _occupationIndexAnchorByName.TryAdd(occupationName, CreateIndexAnchor("occupation", index));
        }

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
        await ComposeFamiliesAsync(doc, sortedFamilies, entryTemplate, groupByFamilyReference);
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
    /// Imports source data and returns the same evidence candidates used by document export.
    /// </summary>
    public async Task<OFBFamilyGroupingResult> PreviewGroupingAsync(
        OFBGenerateOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (string.IsNullOrWhiteSpace(options.InputPath))
            throw new ArgumentException("GEDCOM input path is required.", nameof(options));
        if (!File.Exists(options.InputPath))
            throw new FileNotFoundException($"GEDCOM input file was not found: '{options.InputPath}'.", options.InputPath);

        var preparation = await PrepareGroupingAsync(options, cancellationToken).ConfigureAwait(false);
        return preparation.GroupingResult;
    }

    private async Task<ExportGroupingPreparation> PrepareGroupingAsync(
        OFBGenerateOptions options,
        CancellationToken cancellationToken)
    {
        using var stream = new FileStream(options.InputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var sourceGenealogy = await _dataSource.ImportAsync(stream, cancellationToken).ConfigureAwait(false);
        var sourceFamilies = sourceGenealogy.Entitys.OfType<IGenFamily>().ToArray();
        if (sourceFamilies.Length == 0)
            throw new InvalidOperationException($"No families found in '{options.InputPath}'");

        var selectedModels = new OFBFamilySorter()
            .SortAndNumber(sourceFamilies, options.PlaceId, options.IncludeDescendants);
        var selectedSourceFamilies = MatchSelectedFamilies(sourceFamilies, selectedModels);
        var overlay = new OFBExportOverlayService().Apply(
            sourceGenealogy, _dataSource.SourceId, options.ExportRules, selectedSourceFamilies);
        foreach (var diagnostic in overlay.Diagnostics)
        {
            if (diagnostic.IsError)
                throw new InvalidDataException($"Export rule {diagnostic.RuleId}: {diagnostic.Message}");
        }

        var families = overlay.Genealogy.Entitys.OfType<IGenFamily>().ToList();
        if (families.Count == 0)
            throw new InvalidOperationException($"No families found in '{options.InputPath}' after applying project rules.");

        var grouping = new OFBFamilyGroupingService().BuildGroups(
            families, _dataSource.SourceId, options.GroupingPolicy, options.GroupingDecisions);
        var overlayDiagnostics = overlay.Diagnostics.Select(diagnostic =>
            new OFBGroupingDiagnostic(diagnostic.Code, diagnostic.RuleId, diagnostic.Message));
        var combinedDiagnostics = grouping.Diagnostics.Concat(overlayDiagnostics).ToArray();
        return new ExportGroupingPreparation(families, grouping with { Diagnostics = combinedDiagnostics });
    }

    private static IReadOnlyCollection<IGenFamily> MatchSelectedFamilies(
        IReadOnlyList<IGenFamily> sourceFamilies,
        IReadOnlyList<OFBFamilyModel> selectedModels)
    {
        var matched = new HashSet<IGenFamily>((IEqualityComparer<IGenFamily>)ReferenceEqualityComparer.Instance);
        foreach (var model in selectedModels)
        {
            var sourceFamily = !string.IsNullOrWhiteSpace(model.SourceRefId)
                ? sourceFamilies.FirstOrDefault(family =>
                    !matched.Contains(family)
                    && string.Equals(family.FamilyRefID, model.SourceRefId, StringComparison.Ordinal))
                : sourceFamilies.FirstOrDefault(family =>
                    !matched.Contains(family)
                    && ReferenceEquals(family.Husband, model.Husband)
                    && ReferenceEquals(family.Wife, model.Wife)
                    && family.Children.OfType<IGenPerson>().SequenceEqual(
                        model.Children,
                        (IEqualityComparer<IGenPerson>)ReferenceEqualityComparer.Instance));
            if (sourceFamily is null)
                throw new InvalidOperationException(
                    $"Selected family '{model.SourceRefId}' could not be matched back to its source record.");
            matched.Add(sourceFamily);
        }
        return matched;
    }

    private sealed record ExportGroupingPreparation(
        IReadOnlyList<IGenFamily> Families,
        OFBFamilyGroupingResult GroupingResult);

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
        EntryTemplateDefinition entryTemplate,
        IReadOnlyDictionary<string, string> groupByFamilyReference)
    {
        var templateRenderer = new EntryTemplateRenderer();
        var renderedPeople = new List<PersonEntryTemplateModel>();
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

            var entryModel = CreateEntryTemplateModel(family);
            renderedPeople.AddRange(entryModel.Parents);
            renderedPeople.AddRange(entryModel.Children);
            templateRenderer.RenderFamily(doc, entryTemplate, entryModel);

            var blankPara = doc.AddParagraph( "Normaler Absatz" );
            blankPara.TextContent = "";
        }

        templateRenderer.EnsureNavigation(doc, renderedPeople);
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

    private static string FormatFamilyEvent(OFBFamily family)
    {
        var date = family.MarriageDate?.ToString();
        var place = family.MarriagePlace?.Name;
        return string.Join(" in ", new[] { date, place }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static string FormatPersonName(IGenPerson person, bool gcFormat)
    {
        var givenName = person.GivenName?.Trim();
        var surname = person.Surname?.Trim();
        if (!string.IsNullOrWhiteSpace(givenName) && !string.IsNullOrWhiteSpace(surname))
            return gcFormat
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

    private FamilyEntryTemplateModel CreateEntryTemplateModel(OFBFamily family)
    {
        var parents = new[] { family.Husband, family.Wife }
            .Where(person => person is not null)
            .Select(person => CreatePersonTemplateModel(person!, ordinal: null))
            .ToArray();
        var children = family.Children
            .Select((person, index) => CreatePersonTemplateModel(person, index + 1))
            .ToArray();

        return new FamilyEntryTemplateModel
        {
            Number = family.GlobalNumber,
            Anchor = GetFamilyAnchor(family.GlobalNumber),
            Union = FormatFamilyEvent(family),
            Parents = parents,
            Children = children
        };
    }

    private PersonEntryTemplateModel CreatePersonTemplateModel(IGenPerson person, int? ordinal)
    {
        var reference = person.IndRefID ?? person.Name ?? string.Empty;
        _personIndexAnchorByReference.TryGetValue(reference, out var personIndexAnchor);
        var birth = person.BirthDate?.ToString();
        var death = person.DeathDate?.ToString();
        var vitalEventsGc = string.Join("  ", new[]
        {
            string.IsNullOrWhiteSpace(birth) ? null : $"* {birth}",
            string.IsNullOrWhiteSpace(death) ? null : $"+ {death}"
        }.Where(value => value is not null));
        var vitalEventsAk = string.Join(", ", new[]
        {
            string.IsNullOrWhiteSpace(birth) ? null : $"* {birth}",
            string.IsNullOrWhiteSpace(death) ? null : $"† {death}"
        }.Where(value => value is not null));

        return new PersonEntryTemplateModel
        {
            NameGc = FormatPersonName(person, gcFormat: true),
            NameAk = FormatPersonName(person, gcFormat: false),
            Anchor = GetPersonAnchor(reference),
            Reference = reference,
            VitalEventsGc = vitalEventsGc.Length == 0 ? string.Empty : "  " + vitalEventsGc,
            VitalEventsAk = vitalEventsAk.Length == 0 ? string.Empty : ", " + vitalEventsAk,
            Birth = birth,
            Death = death,
            IndexAnchor = personIndexAnchor ?? string.Empty,
            Ordinal = ordinal,
            Occupations = GetOccupations(person)
                .Select(occupation => new OccupationEntryTemplateModel
                {
                    Name = occupation.Name,
                    Date = occupation.Date?.ToString(),
                    IndexAnchor = _occupationIndexAnchorByName.TryGetValue(occupation.Name, out var anchor)
                        ? anchor
                        : string.Empty
                })
                .ToArray()
        };
    }

    private static IEnumerable<(string Name, IGenDate? Date)> GetOccupations(IGenPerson person)
    {
        var hasOccupationFacts = false;
        foreach (var fact in person.Facts)
        {
            if (fact?.eFactType != EFactType.Occupation || string.IsNullOrWhiteSpace(fact.Data))
                continue;

            hasOccupationFacts = true;
            yield return (fact.Data.Trim(), fact.Date);
        }

        if (!hasOccupationFacts && !string.IsNullOrWhiteSpace(person.Occupation))
            yield return (person.Occupation.Trim(), null);
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
        var seen = new HashSet<string>( StringComparer.OrdinalIgnoreCase);

        foreach ( var family in sortedFamilies )
        {
            foreach ( var person in new[] { family.Husband, family.Wife }.Where( p => p != null ) )
                foreach (var occupation in GetOccupations(person!))
                    if (seen.Add(occupation.Name))
                        entries.Add(OFBIndexEntry.FromSingle(occupation.Name, $"Fam.{family.GlobalNumber}"));

            foreach ( var child in family.Children )
                foreach (var occupation in GetOccupations(child))
                    if (seen.Add(occupation.Name))
                        entries.Add(OFBIndexEntry.FromSingle(occupation.Name, $"Fam.{family.GlobalNumber}K"));
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
