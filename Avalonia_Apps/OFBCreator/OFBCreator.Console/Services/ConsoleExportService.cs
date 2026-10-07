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
    private readonly Dictionary<string, string> _propertyIndexAnchorByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _placeIndexAnchorByName = new(StringComparer.OrdinalIgnoreCase);
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
        var familyNamesByGroup = groupingResult.Groups.ToDictionary(
            group => group.Key,
            group => (IReadOnlyList<string>)group.Value
                .Select(OFBFamilyGroupingService.SelectFamilySurname)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.Create(CultureInfo.GetCultureInfo("de-DE"), ignoreCase: true))
                .ToArray(),
            StringComparer.Ordinal);

        var germanComparer = StringComparer.Create(CultureInfo.GetCultureInfo("de-DE"), ignoreCase: true);
        var orderedFamilies = abFamilies
            .OrderBy(family => groupByFamilyReference.TryGetValue(family.SourceRefId, out var groupName)
                ? groupName
                : family.FamilyName, germanComparer)
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
        _propertyIndexAnchorByName.Clear();
        for (var index = 0; index < propIndex.Length; index++)
        {
            var propertyName = string.IsNullOrWhiteSpace(propIndex[index].Name)
                ? propIndex[index].SortKey
                : propIndex[index].Name;
            _propertyIndexAnchorByName.TryAdd(propertyName, CreateIndexAnchor("property", index));
        }

        System.Console.WriteLine( "Generating Place Hierarchy Index..." );
        var placeHierarchyIndex = await GeneratePlaceHierarchyIndexAsync( sortedFamilies );

        System.Console.WriteLine( "Generating Place Alphabetical Index..." );
        var placeAlphaIndex = await GeneratePlaceAlphabeticalIndexAsync( sortedFamilies );
        _placeIndexAnchorByName.Clear();
        for (var index = 0; index < placeAlphaIndex.Length; index++)
            _placeIndexAnchorByName.TryAdd(placeAlphaIndex[index].Name ?? placeAlphaIndex[index].SortKey, CreateIndexAnchor("place", index));

        // Step 5: Create document and compose content
        System.Console.WriteLine( "Composing document..." );
        var doc = _documentFactory.CreateDocument( options.UseDocxFormat ? OFBOutputFormat.Docx : OFBOutputFormat.Odt );

        await ComposeTitlePageAsync( doc, options );
        await ComposePrefaceAsync( doc, options );
        await ComposeFamiliesAsync(doc, sortedFamilies, entryTemplate, groupByFamilyReference, familyNamesByGroup);
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

    // ExportAsync is intentionally closed above; helper methods continue below.
    private static IReadOnlyList<FamilyReferenceTokenModel> CreateFamilyReferenceTokens(
        IReadOnlyList<FamilyReferenceEntryTemplateModel> references)
    {
        var numbered = references
            .Select(reference => (Reference: reference, Number: int.TryParse(reference.Number, NumberStyles.None, CultureInfo.InvariantCulture, out var number) ? number : (int?)null))
            .Where(item => item.Number.HasValue)
            .OrderBy(item => item.Number)
            .GroupBy(item => item.Number!.Value)
            .Select(group => group.First())
            .ToArray();
        var tokens = new List<FamilyReferenceTokenModel>();
        var index = 0;
        while (index < numbered.Length)
        {
            var end = index;
            while (end + 1 < numbered.Length && numbered[end + 1].Number == numbered[end].Number + 1)
                end++;
            if (tokens.Count > 0)
                tokens.Add(new FamilyReferenceTokenModel { Text = ", " });
            if (end - index >= 2)
            {
                tokens.Add(new FamilyReferenceTokenModel { Text = numbered[index].Reference.Number, Anchor = numbered[index].Reference.Anchor });
                tokens.Add(new FamilyReferenceTokenModel { Text = " - " });
                tokens.Add(new FamilyReferenceTokenModel { Text = numbered[end].Reference.Number, Anchor = numbered[end].Reference.Anchor });
            }
            else
            {
                for (var itemIndex = index; itemIndex <= end; itemIndex++)
                {
                    if (itemIndex > index)
                        tokens.Add(new FamilyReferenceTokenModel { Text = ", " });
                    tokens.Add(new FamilyReferenceTokenModel { Text = numbered[itemIndex].Reference.Number, Anchor = numbered[itemIndex].Reference.Anchor });
                }
            }
            index = end + 1;
        }
        return tokens;
    }

    /// <summary>
    /// Imports source data and returns the same evidence candidates used by document export.
    /// </summary>
    public async Task<OFBFamilyGroupingResult> PreviewGroupingAsync(
        OFBGenerateOptions options,
        CancellationToken cancellationToken = default)
    {
        var preview = await PreviewWorkspaceAsync(options, cancellationToken).ConfigureAwait(false);
        return preview.Grouping;
    }

    /// <summary>
    /// Imports the source once and previews privacy-filtered people and family grouping together.
    /// </summary>
    public async Task<OFBWorkspacePreview> PreviewWorkspaceAsync(
        OFBGenerateOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (string.IsNullOrWhiteSpace(options.InputPath))
            throw new ArgumentException("GEDCOM input path is required.", nameof(options));
        if (!File.Exists(options.InputPath))
            throw new FileNotFoundException($"GEDCOM input file was not found: '{options.InputPath}'.", options.InputPath);

        var preparation = await PrepareGroupingAsync(options, cancellationToken).ConfigureAwait(false);
        return new OFBWorkspacePreview(preparation.GroupingResult, preparation.PersonPreviews);
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
        return new ExportGroupingPreparation(
            families,
            grouping with { Diagnostics = combinedDiagnostics },
            overlay.PersonPreviews);
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
        OFBFamilyGroupingResult GroupingResult,
        IReadOnlyList<OFBPersonPrivacyPreview> PersonPreviews);

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
            AppendFamilyTokens(para, " — Kind: ", CreateFamilyReferenceTokens(entry.ChildFamilyReferences.Select(CreateNumberedFamilyReference).ToArray()));
            AppendFamilyTokens(para, "; Parent: ", CreateFamilyReferenceTokens(entry.ParentFamilyReferences.Select(CreateNumberedFamilyReference).ToArray()));
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

    private static void AppendFamilyTokens(IDocParagraph paragraph, string prefix, IReadOnlyList<FamilyReferenceTokenModel> tokens)
    {
        if (tokens.Count == 0)
            return;
        paragraph.AddSpan(prefix, DocxFontStyle.Default).TextContent = prefix;
        foreach (var token in tokens)
        {
            if (string.IsNullOrWhiteSpace(token.Anchor))
                paragraph.AddSpan(token.Text, DocxFontStyle.Default).TextContent = token.Text;
            else
                paragraph.AddLink($"#{token.Anchor}", DocxFontStyle.UnderlineStyle).TextContent = token.Text;
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
        IReadOnlyDictionary<string, string> groupByFamilyReference,
        IReadOnlyDictionary<string, IReadOnlyList<string>> familyNamesByGroup)
    {
        var templateRenderer = new EntryTemplateRenderer();
        var renderedPeople = new List<PersonEntryTemplateModel>();
        string? currentGroupName = null;
        foreach (var family in sortedFamilies)
        {
            var groupName = GetFamilyGroupName(family, groupByFamilyReference);
            if (!string.Equals(groupName, currentGroupName, StringComparison.Ordinal))
            {
                var groupHeadline = doc.AddHeadline(1, $"group-{SanitizeAnchor(groupName)}-{family.GlobalNumber}");
                groupHeadline.TextContent = groupName;
                if (familyNamesByGroup.TryGetValue(groupName, out var familyNames) && familyNames.Count > 1)
                {
                    var subtitle = doc.AddParagraph("Normaler Absatz");
                    subtitle.TextContent = string.Join(", ", familyNames);
                }
                currentGroupName = groupName;
            }

            var entryModel = CreateEntryTemplateModel(family, sortedFamilies);
            renderedPeople.AddRange(entryModel.Parents);
            renderedPeople.AddRange(entryModel.Children);
            doc.AddHeadline(3, GetFamilyAnchor(family.GlobalNumber));
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

    private static string FormatFamilyEvent(OFBFamily family)
    {
        var date = family.MarriageDate?.ToString();
        var place = family.MarriagePlace?.Name;
        return string.Join(" in ", new[] { date, place }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static string GetFamilyMarriageMark(OFBFamily family)
    {
        var eventText = family.Marriage?.Data;
        if (string.IsNullOrWhiteSpace(eventText))
            return family.Marriage is null && family.MarriageDate is null ? "o-o" : "oo";

        if (eventText.Contains("unverheirat", StringComparison.OrdinalIgnoreCase)
            || eventText.Contains("unehelich", StringComparison.OrdinalIgnoreCase))
            return "o-o";
        return "oo";
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

    private static string CreateIndexAnchor(string category, int ordinal) => $"index-{category}-{ordinal + 1}";

    private static string SanitizeAnchor(string value)
    {
        var characters = value.Select(character => char.IsLetterOrDigit(character) ? character : '-').ToArray();
        return new string(characters).Trim('-');
    }

    private FamilyEntryTemplateModel CreateEntryTemplateModel(OFBFamily family, IReadOnlyList<OFBFamily> allFamilies)
    {
        var parents = new[] { family.Husband, family.Wife }
            .Where(person => person is not null)
            .Select(person => CreatePersonTemplateModel(person!, ordinal: null, allFamilies, family.GlobalNumber))
            .ToArray();
        var children = family.Children
            .OrderBy(person => person.BirthDate?.Date1 ?? DateTime.MaxValue)
            .ThenBy(person => person.IndRefID ?? person.Name, StringComparer.Ordinal)
            .Select((person, index) => CreatePersonTemplateModel(person, index + 1, allFamilies, family.GlobalNumber))
            .ToArray();

        return new FamilyEntryTemplateModel
        {
            Number = family.GlobalNumber,
            Anchor = GetFamilyAnchor(family.GlobalNumber),
            Union = FormatFamilyEvent(family),
            MarriageMark = GetFamilyMarriageMark(family),
            MarriageDate = family.MarriageDate?.ToString() ?? string.Empty,
            MarriagePlace = family.MarriagePlace?.Name ?? string.Empty,
            MarriagePlaceShort = GetShortPlaceName(family.MarriagePlace?.Name),
            MarriagePlaceAnchor = GetPlaceIndexAnchor(family.MarriagePlace?.Name),
            Properties = CreatePropertyTemplateModels(family.Facts),
            Parents = parents,
            Children = children
        };
    }

    private FamilyReferenceEntryTemplateModel? CreateFamilyReferenceTemplateModel(IGenFamily? family)
    {
        if (family is null || string.IsNullOrWhiteSpace(family.FamilyRefID))
            return null;

        return _familyAnchorBySourceReference.TryGetValue(family.FamilyRefID, out var anchor)
            ? new FamilyReferenceEntryTemplateModel
            {
                Number = anchor["family-".Length..],
                Anchor = anchor
            }
            : new FamilyReferenceEntryTemplateModel
            {
                Number = family.FamilyRefID,
                Anchor = string.Empty
            };
    }

    private static bool IsSamePerson(IGenPerson? candidate, IGenPerson person)
    {
        if (candidate is null)
            return false;
        var reference = person.IndRefID ?? person.Name;
        return string.Equals(candidate.IndRefID ?? candidate.Name, reference, StringComparison.Ordinal);
    }

    private PersonEntryTemplateModel CreatePersonTemplateModel(IGenPerson person, int? ordinal, IReadOnlyList<OFBFamily> allFamilies, string currentFamilyNumber)
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

        var parentFamily = CreateFamilyReferenceTemplateModel(person.ParentFamily);
        var childFamilies = parentFamily is null
            ? Array.Empty<FamilyReferenceEntryTemplateModel>()
            : new[] { parentFamily };
        var parentFamilies = allFamilies
            .Where(candidate => IsSamePerson(candidate.Husband, person) || IsSamePerson(candidate.Wife, person))
            .Where(candidate => !string.Equals(candidate.GlobalNumber, currentFamilyNumber, StringComparison.Ordinal))
            .Select(candidate => new FamilyReferenceEntryTemplateModel
            {
                Number = candidate.GlobalNumber,
                Anchor = GetFamilyAnchor(candidate.GlobalNumber)
            })
            .ToArray();

        return new PersonEntryTemplateModel
        {
            NameGc = FormatPersonName(person, gcFormat: true),
            NameAk = FormatPersonName(person, gcFormat: false),
            Anchor = GetPersonAnchor(reference),
            Reference = reference,
            IndexLabel = FormatPersonName(person, gcFormat: true),
            VitalEventsGc = vitalEventsGc.Length == 0 ? string.Empty : "  " + vitalEventsGc,
            VitalEventsAk = vitalEventsAk.Length == 0 ? string.Empty : ", " + vitalEventsAk,
            Birth = birth,
            Death = death,
            IndexAnchor = personIndexAnchor ?? string.Empty,
            Residence = person.Residence?.Name,
            Ordinal = ordinal,
            ParentFamily = parentFamily,
            ChildFamilies = childFamilies,
            ParentFamilies = parentFamilies,
            ChildFamilyTokens = CreateFamilyReferenceTokens(childFamilies),
            ParentFamilyTokens = CreateFamilyReferenceTokens(parentFamilies),
            Occupations = GetOccupations(person)
                .Select(occupation => new OccupationEntryTemplateModel
                {
                    Name = occupation.Name,
                    Date = occupation.Date?.ToString(),
                    Place = occupation.Place?.Name,
                    PlaceAnchor = GetPlaceIndexAnchor(occupation.Place?.Name),
                    IndexAnchor = _occupationIndexAnchorByName.TryGetValue(occupation.Name, out var anchor)
                        ? anchor
                        : string.Empty
                })
                .ToArray(),
            Properties = CreatePropertyTemplateModels(person.Facts)
        };
    }

    private static IEnumerable<(string Name, IGenDate? Date, IGenPlace? Place)> GetOccupations(IGenPerson person)
    {
        var hasOccupationFacts = false;
        foreach (var fact in person.Facts)
        {
            if (fact?.eFactType != EFactType.Occupation || string.IsNullOrWhiteSpace(fact.Data))
                continue;

            hasOccupationFacts = true;
            yield return (fact.Data.Trim(), fact.Date, fact.Place);
        }

        if (!hasOccupationFacts && !string.IsNullOrWhiteSpace(person.Occupation))
            yield return (person.Occupation.Trim(), null, person.OccuPlace);
    }

    private IReadOnlyList<PropertyEntryTemplateModel> CreatePropertyTemplateModels(IEnumerable<IGenFact?> facts) => facts
        .Where(fact => fact?.eFactType == EFactType.Property && !string.IsNullOrWhiteSpace(fact.Data))
        .Select(fact => new PropertyEntryTemplateModel
        {
            Name = fact!.Data!.Trim(),
            Date = fact.Date?.ToString(),
            Place = fact.Place?.Name,
            PlaceAnchor = GetPlaceIndexAnchor(fact.Place?.Name),
            IndexAnchor = _propertyIndexAnchorByName.TryGetValue(fact.Data.Trim(), out var anchor) ? anchor : string.Empty
        })
        .ToArray();

    private string GetPlaceIndexAnchor(string? placeName) =>
        !string.IsNullOrWhiteSpace(placeName) && _placeIndexAnchorByName.TryGetValue(placeName, out var anchor)
            ? anchor
            : string.Empty;

    private static string GetShortPlaceName(string? placeName) =>
        string.IsNullOrWhiteSpace(placeName)
            ? string.Empty
            : placeName.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? placeName;

    /// <summary>
    /// Generates alphabetical person index entries for all individuals.
    /// </summary>
    private async Task<OFBIndexEntry[]> GeneratePersonIndexAsync( IReadOnlyList<OFBFamily> sortedFamilies )
    {
        // Collect unique persons from all families (use OFBFamily which has GlobalNumber)
        var seen = new HashSet<string>();
        var entries = new List<OFBIndexEntry>();
        var referencesByPerson = new Dictionary<string, (List<FamilyReferenceEntryTemplateModel> Child, List<FamilyReferenceEntryTemplateModel> Parent)>(StringComparer.Ordinal);
        _personReferenceBySortKey.Clear();
        _personIndexAnchorByReference.Clear();

        foreach ( var family in sortedFamilies )
        {
            if ( family.Husband != null )
            {
                var key = family.Husband.IndRefID ?? family.Husband.Name;
                AddPersonFamilyReference(family.Husband, family, isChild: false, key, seen, entries, referencesByPerson);
            }

            if ( family.Wife != null )
            {
                var key = family.Wife.IndRefID ?? family.Wife.Name;
                AddPersonFamilyReference(family.Wife, family, isChild: false, key, seen, entries, referencesByPerson);
            }

            foreach ( var child in family.Children )
            {
                var key = child.IndRefID ?? child.Name;
                AddPersonFamilyReference(child, family, isChild: true, key, seen, entries, referencesByPerson);
            }
        }

        // Sort alphabetically by full name key
        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        entries = entries.Select(entry => _personReferenceBySortKey.TryGetValue(entry.SortKey, out var personReference)
            && referencesByPerson.TryGetValue(personReference, out var references)
            ? new OFBIndexEntry
            {
                SortKey = entry.SortKey,
                Name = entry.Name,
                Ref = entry.Ref,
                ChildFamilyReferences = references.Child.Select(reference => reference.Number).ToArray(),
                ParentFamilyReferences = references.Parent.Select(reference => reference.Number).ToArray()
            }
            : entry).ToList();
        _personIndexAnchorByReference.Clear();
        for (var index = 0; index < entries.Count; index++)
            if (_personReferenceBySortKey.TryGetValue(entries[index].SortKey, out var personReference))
                _personIndexAnchorByReference.TryAdd(personReference, CreateIndexAnchor("person", index));
        return [.. entries];
    }

    private static FamilyReferenceEntryTemplateModel CreateNumberedFamilyReference(string number) => new()
    {
        Number = number,
        Anchor = GetFamilyAnchor(number)
    };

    private void AddPersonFamilyReference(
        IGenPerson person,
        OFBFamily family,
        bool isChild,
        string personReference,
        HashSet<string> seen,
        List<OFBIndexEntry> entries,
        Dictionary<string, (List<FamilyReferenceEntryTemplateModel> Child, List<FamilyReferenceEntryTemplateModel> Parent)> referencesByPerson)
    {
        if (string.IsNullOrWhiteSpace(personReference))
            personReference = person.Name ?? $"{person.GivenName}|{person.Surname}";
        if (!referencesByPerson.TryGetValue(personReference, out var references))
            referencesByPerson[personReference] = references = (new List<FamilyReferenceEntryTemplateModel>(), new List<FamilyReferenceEntryTemplateModel>());
        var familyReference = new FamilyReferenceEntryTemplateModel
        {
            Number = family.GlobalNumber,
            Anchor = GetFamilyAnchor(family.GlobalNumber)
        };
        (isChild ? references.Child : references.Parent).Add(familyReference);

        if (seen.Add(personReference))
        {
            var entry = OFBIndexEntry.FromCombined(
                person.Surname ?? string.Empty,
                person.GivenName ?? string.Empty,
                personReference);
            entries.Add(entry);
            _personReferenceBySortKey.TryAdd(entry.SortKey, personReference);
        }
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
        var propertyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach ( var family in sortedFamilies )
        {
            var properties = ExtractPropertiesFromFamily( family );
            foreach ( var prop in properties )
                if (propertyNames.Add(prop))
                    entries.Add( OFBIndexEntry.FromSingle( prop, $"Fam.{family.GlobalNumber}" ));
        }

        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        return [.. entries];
    }

    /// <summary>
    /// Extracts property names from family and related person property facts.
    /// </summary>
    private IEnumerable<string> ExtractPropertiesFromFamily( OFBFamily family )
    {
        var people = new[] { family.Husband, family.Wife }.Where(person => person is not null).Cast<IGenPerson>()
            .Concat(family.Children);
        return family.Facts.Concat(people.SelectMany(person => person.Facts))
            .Where(fact => fact?.eFactType == EFactType.Property && !string.IsNullOrWhiteSpace(fact.Data))
            .Select(fact => fact!.Data!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase);
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
                Marriage = m.Facts.FirstOrDefault(fact => fact?.eFactType == EFactType.Mariage),
                Facts = m.Facts,
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
            AddPlace(family.MarriagePlace);
            foreach (var fact in family.Facts.OfType<IGenFact>())
                AddPlace(fact.Place);

            foreach (var person in new[] { family.Husband, family.Wife }.Where(person => person is not null).Cast<IGenPerson>().Concat(family.Children))
            {
                AddPlace(person.BirthPlace);
                AddPlace(person.DeathPlace);
                AddPlace(person.BurialPlace);
                AddPlace(person.Residence);
                AddPlace(person.OccuPlace);
                foreach (var fact in person.Facts.OfType<IGenFact>())
                    AddPlace(fact.Place);
            }
        }

        // TODO: Build parent-child relationships from GEDCOM place hierarchy data
        return [.. placeMap.Values];

        void AddPlace(IGenPlace? place)
        {
            if (string.IsNullOrWhiteSpace(place?.GOV_ID) || string.IsNullOrWhiteSpace(place.Name))
                return;
            if (!placeMap.ContainsKey(place.GOV_ID))
                placeMap[place.GOV_ID] = new OFBPlaceHierarchyNode(place.Name, place.GOV_ID);
        }
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
            AddPlace(family.MarriagePlace, $"Fam.{family.GlobalNumber}");
            foreach (var fact in family.Facts.OfType<IGenFact>())
                AddPlace(fact.Place, $"Fam.{family.GlobalNumber}");

            foreach (var person in new[] { family.Husband, family.Wife }.Where(person => person is not null).Cast<IGenPerson>().Concat(family.Children))
            {
                var reference = person == family.Husband || person == family.Wife
                    ? $"Fam.{family.GlobalNumber}"
                    : $"Fam.{family.GlobalNumber}K";
                AddPlace(person.BirthPlace, reference);
                AddPlace(person.DeathPlace, reference);
                AddPlace(person.BurialPlace, reference);
                AddPlace(person.Residence, reference);
                AddPlace(person.OccuPlace, reference);
                foreach (var fact in person.Facts.OfType<IGenFact>())
                    AddPlace(fact.Place, reference);
            }
        }

        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        return [.. entries];

        void AddPlace(IGenPlace? place, string reference)
        {
            if (!string.IsNullOrWhiteSpace(place?.Name) && seen.Add(place.Name))
                entries.Add(OFBIndexEntry.FromSingle(place.Name, reference));
        }
    }
}
