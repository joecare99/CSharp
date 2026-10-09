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
using OFBCreator.Publishing.Models;
using OFBCreator.Publishing.Services.Templates;

namespace OFBCreator.Publishing.Services;

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
public sealed class OFBExportService(
    IFamilyDataSource dataSource,
    IUserDocumentFactory documentFactory,
    EntryTemplateStore? templateStore = null)
{
    private readonly IFamilyDataSource _dataSource = dataSource;
    private readonly IUserDocumentFactory _documentFactory = documentFactory;
    private readonly EntryTemplateStore _templateStore = templateStore ?? new EntryTemplateStore();
    private IReadOnlyList<EntryTemplateLegendEntry> _legendEntries = Array.Empty<EntryTemplateLegendEntry>();
    private readonly Dictionary<string, string> _personReferenceBySortKey = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _personIndexAnchorByReference = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _occupationIndexAnchorByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _propertyIndexAnchorByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _placeIndexAnchorByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _familyAnchorBySourceReference = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _personReferenceByIndexName = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _personIndexAnchorByName = new(StringComparer.Ordinal);

    /// <summary>
    /// Executes the full OFB generation pipeline for the given options.
    /// </summary>
    public async Task ExportAsync(OFBGenerateOptions options, CancellationToken cancellationToken = default, IProgress<OFBExportProgress>? progress = null)
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

        var entryTemplate = _templateStore.Load( options.Template );
        _legendEntries = entryTemplate.Legend;
        if (!string.Equals(entryTemplate.EntryRoot, "Family", StringComparison.Ordinal))
            throw new InvalidDataException(
                $"Template '{entryTemplate.Id}' uses the Individual root, but the current export pipeline generates Family entries.");

        progress?.Report(new OFBExportProgress($"Loading from {_dataSource.DisplayName}..."));
        var preparation = await PrepareGroupingAsync(options, cancellationToken).ConfigureAwait(false);
        var allFamilies = preparation.Families;
        var groupingResult = preparation.GroupingResult;
        foreach (var diagnostic in groupingResult.Diagnostics)
            progress?.Report(new OFBExportProgress($"Warning [{diagnostic.Code}]: {diagnostic.Message}", IsWarning: true));

        // Step 3: Convert Abstraction models to Core models, sort, and assign global numbers
        progress?.Report(new OFBExportProgress("Sorting and numbering families..."));
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
            .OrderBy(family =>
                groupingResult.NoNameGroupName is not null
                && groupByFamilyReference.TryGetValue(family.SourceRefId, out var groupName)
                && string.Equals(groupName, groupingResult.NoNameGroupName, StringComparison.OrdinalIgnoreCase))
            .ThenBy(family => groupByFamilyReference.TryGetValue(family.SourceRefId, out var groupName)
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
        progress?.Report(new OFBExportProgress("Generating Person Index..."));
        var personIndex = await GeneratePersonIndexAsync( sortedFamilies );

        progress?.Report(new OFBExportProgress("Generating Occupation Index..."));
        var occIndex = await GenerateOccupationIndexAsync( sortedFamilies );
        _occupationIndexAnchorByName.Clear();
        for (var index = 0; index < occIndex.Length; index++)
        {
            var occupationName = string.IsNullOrWhiteSpace(occIndex[index].Name)
                ? occIndex[index].SortKey
                : occIndex[index].Name;
            _occupationIndexAnchorByName.TryAdd(occupationName, CreateIndexAnchor("occupation", index));
        }

        progress?.Report(new OFBExportProgress("Generating Property Index..."));
        var propIndex = await GeneratePropertyIndexAsync( sortedFamilies );
        _propertyIndexAnchorByName.Clear();
        for (var index = 0; index < propIndex.Length; index++)
        {
            var propertyName = string.IsNullOrWhiteSpace(propIndex[index].Name)
                ? propIndex[index].SortKey
                : propIndex[index].Name;
            _propertyIndexAnchorByName.TryAdd(propertyName, CreateIndexAnchor("property", index));
        }

        progress?.Report(new OFBExportProgress("Generating Place Hierarchy Index..."));
        var placeHierarchyIndex = await GeneratePlaceHierarchyIndexAsync( sortedFamilies );

        progress?.Report(new OFBExportProgress("Generating Place Alphabetical Index..."));
        var placeAlphaIndex = await GeneratePlaceAlphabeticalIndexAsync( sortedFamilies );
        _placeIndexAnchorByName.Clear();
        for (var index = 0; index < placeAlphaIndex.Length; index++)
            _placeIndexAnchorByName.TryAdd(placeAlphaIndex[index].Name ?? placeAlphaIndex[index].SortKey, CreateIndexAnchor("place", index));

        // Step 5: Create document and compose content
        progress?.Report(new OFBExportProgress("Composing document..."));
        var doc = _documentFactory.CreateDocument( options.UseDocxFormat ? OFBOutputFormat.Docx : OFBOutputFormat.Odt );

        await ComposeTitlePageAsync( doc, options );
        await ComposePrefaceAsync(doc, options, entryTemplate);
        await ComposeFamiliesAsync(doc, sortedFamilies, entryTemplate, groupByFamilyReference, familyNamesByGroup);
        await ComposeIndicesAsync( doc, personIndex, occIndex, propIndex, placeHierarchyIndex, placeAlphaIndex );

        // Step 6: Save document
        progress?.Report(new OFBExportProgress("Saving output..."));
        var outputDirectory = Path.GetDirectoryName( Path.GetFullPath( options.OutputPath ) );
        if ( !string.IsNullOrEmpty( outputDirectory ) )
            Directory.CreateDirectory( outputDirectory );

        var success = doc.SaveTo( options.OutputPath );
        if ( !success )
            throw new InvalidOperationException( $"Failed to save OFB document to '{options.OutputPath}'" );
    }

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
        families.RemoveAll(family =>
            family.Marriage is null
            && family.MarriageDate is null
            && family.MarriagePlace is null
            && family.Children.Count == 0);
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
    private async Task ComposePrefaceAsync(
        IUserDocument doc,
        OFBGenerateOptions options,
        EntryTemplateDefinition entryTemplate)
    {
        var preface = LoadEmbeddedPreface();
        if (!string.IsNullOrWhiteSpace(options.Preface))
            preface += Environment.NewLine + Environment.NewLine + options.Preface;
        if (string.IsNullOrWhiteSpace(preface))
            return;

        var prefaceHeadline = doc.AddHeadline(1, "Vorwort");
        prefaceHeadline.TextContent = "Vorwort";
        var markdown = preface.Replace("<!-- OFB-LEGEND -->", FormatLegend(entryTemplate.Legend, options.Legend), StringComparison.Ordinal);
        foreach (var line in markdown.Split(["\r\n", "\n"], StringSplitOptions.None))
        {
            var text = line.Trim();
            if (text.Length == 0)
                continue;
            if (text.StartsWith('|'))
            {
                var cells = text.Trim('|').Split('|', StringSplitOptions.TrimEntries);
                if (cells.Length >= 2 && !cells[0].StartsWith('-'))
                {
                    var symbolParagraph = doc.AddParagraph("Normaler Absatz");
                    symbolParagraph.TextContent = $"{cells[0]} = {cells[1]}";
                }
                continue;
            }
            if (text.StartsWith("# ", StringComparison.Ordinal))
            {
                doc.AddHeadline(2, SanitizeAnchor(text[2..])).TextContent = text[2..];
                continue;
            }
            if (text.StartsWith("## ", StringComparison.Ordinal))
            {
                doc.AddHeadline(3, SanitizeAnchor(text[3..])).TextContent = text[3..];
                continue;
            }
            var paragraph = doc.AddParagraph("Normaler Absatz");
            paragraph.TextContent = text;
        }
    }

    private void AppendIndexFamilyTokens(IDocParagraph paragraph, OFBIndexEntry entry, bool addLeadingSpace = false)
    {
        var childNumbers = entry.ChildFamilyReferences.Distinct(StringComparer.Ordinal).ToArray();
        var parentNumbers = entry.ParentFamilyReferences.Distinct(StringComparer.Ordinal).ToArray();
        if (childNumbers.Length == 0 && parentNumbers.Length == 0)
            return;
        if (addLeadingSpace)
            paragraph.AddSpan("Referenz-Abstand", DocxFontStyle.Default).TextContent = " ";
        var childTokens = childNumbers.Length > 0
            ? CreateFamilyReferenceTokens(childNumbers.Select(CreateNumberedFamilyReference).ToArray())
            : Array.Empty<FamilyReferenceTokenModel>();
        if (childTokens.Count > 0)
            AppendFamilyTokens(paragraph, "<", childTokens, closeWith: ">");

        if (parentNumbers.Length > 0)
        {
            if (childTokens.Count > 0)
                paragraph.AddSpan("Referenz-Abstand", DocxFontStyle.Default).TextContent = " ";
            var tokens = CreateFamilyReferenceTokens(parentNumbers.Select(CreateNumberedFamilyReference).ToArray());
            AppendFamilyTokens(paragraph, "[", tokens);
        }
    }

    /// <summary>
    /// Composes the character explanation/legend showing OFB symbols.
    /// </summary>
    private async Task ComposeLegendAsync(
        IUserDocument doc,
        IReadOnlyList<EntryTemplateLegendEntry> legendEntries,
        string additionalLegend)
    {
        var legendHeadline = doc.AddHeadline( 1, "Zeichenerklärung" );
        legendHeadline.TextContent = "Zeichenerklärung";

        foreach (var entry in legendEntries)
        {
            var para = doc.AddParagraph("Normaler Absatz");
            para.TextContent = $"{entry.Symbol} = {entry.Meaning}";
        }

        if (!string.IsNullOrWhiteSpace(additionalLegend))
        {
            var legendPara = doc.AddParagraph("Normaler Absatz");
            legendPara.TextContent = additionalLegend;
        }
    }

    private static string LoadEmbeddedPreface()
    {
        var resourceName = typeof(OFBExportService).Assembly.GetManifestResourceNames()
            .SingleOrDefault(name => name.EndsWith("Templates.Vorwort.md", StringComparison.Ordinal));
        if (resourceName is null)
            return string.Empty;
        using var resource = typeof(OFBExportService).Assembly.GetManifestResourceStream(resourceName);
        if (resource is null)
            return string.Empty;
        using var reader = new StreamReader(resource);
        return reader.ReadToEnd();
    }

    private static string FormatLegend(IReadOnlyList<EntryTemplateLegendEntry> entries, string? additionalLegend) =>
        string.Join(Environment.NewLine, entries.Select(entry => $"{entry.Symbol} = {entry.Meaning}")
            .Concat(string.IsNullOrWhiteSpace(additionalLegend) ? [] : [additionalLegend]));

    /// <summary>
    /// Composes all index sections (Person, Occupation, Property, Place).
    /// </summary>
    private async Task ComposeIndicesAsync( IUserDocument doc, OFBIndexEntry[] personIndex,
        OFBIndexEntry[] occIndex, OFBIndexEntry[] propIndex,
        OFBPlaceHierarchyNode[] placeHierarchy, OFBIndexEntry[] placeAlpha )
    {
        // Person Index (alphabetical)
        var personTitle = doc.AddHeadline(1, "Personenindex");
        personTitle.TextContent = "Personenindex";
        string? previousSurname = null;
        for (var index = 0; index < personIndex.Length; index++)
        {
            var entry = personIndex[index];
            if (!string.Equals(previousSurname, entry.Name, StringComparison.OrdinalIgnoreCase))
            {
                previousSurname = entry.Name;
                if (!string.IsNullOrWhiteSpace(previousSurname))
                {
                    var surnameHeading = doc.AddHeadline(2, SanitizeAnchor($"person-index-{previousSurname}"));
                    surnameHeading.TextContent = previousSurname;
                }
            }
            var para = doc.AddParagraph( "Normaler Absatz" );
            var entryAnchor = CreateIndexAnchor("person", index);
            var personAnchor = GetPersonAnchor(entry.Ref);
            var givenName = GetPersonIndexGivenName(entry.SortKey, entry.Name);
            var personIndexLabel = string.IsNullOrEmpty(entry.LifeSpan)
                ? givenName
                : $"{givenName} ({entry.LifeSpan})";
            para.AddBookmark(entryAnchor, DocxFontStyle.Default).TextContent = personIndexLabel;
            para.AddBookmark(personAnchor, DocxFontStyle.Default).TextContent = string.Empty;
            if (entry.ChildFamilyReferences.Count > 0 && entry.ParentFamilyReferences.Count > 0)
                para.AppendText(" ");
            if (!string.IsNullOrEmpty(entry.NameEventDate))
                para.AppendText($" [{entry.NameEventDate}]");
            AppendIndexFamilyTokens(para, entry, addLeadingSpace: true);
        }
        // Occupation Index (alphabetical)
        var occTitle = doc.AddHeadline( 1, "Berufsindex" );
        occTitle.TextContent = "Berufsindex";
        for (var index = 0; index < occIndex.Length; index++)
        {
            var entry = occIndex[index];
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.AddBookmark(CreateIndexAnchor("occupation", index), DocxFontStyle.Default).TextContent = entry.SortKey;
            AppendIndexFamilyReferences(para, entry, entry.Ref);
        }

        // Property Index (alphabetical, decision 1A)
        var propTitle = doc.AddHeadline( 1, "Besitzindex" );
        propTitle.TextContent = "Eigentums-/Besitz-Index";
        for (var index = 0; index < propIndex.Length; index++)
        {
            var entry = propIndex[index];
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.AddBookmark(CreateIndexAnchor("property", index), DocxFontStyle.Default).TextContent = entry.SortKey;
            AppendIndexFamilyReferences(para, entry, entry.Ref);
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
            AppendIndexFamilyReferences(para, entry, entry.Ref);
        }
    }

    private static string GetPersonIndexGivenName(string sortKey, string surname)
    {
        var namePrefix = string.IsNullOrWhiteSpace(surname) ? string.Empty : $"{surname}, ";
        return sortKey.StartsWith(namePrefix, StringComparison.Ordinal)
            ? sortKey[namePrefix.Length..]
            : sortKey;
    }

    private static void AppendFamilyTokens(IDocParagraph paragraph, string prefix, IReadOnlyList<FamilyReferenceTokenModel> tokens, string closeWith = "]")
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
        paragraph.AddSpan(closeWith, DocxFontStyle.Default).TextContent = closeWith;
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
            para.TextContent = $"{prefix}{node.Name}";
            var familyTokens = CreateFamilyReferenceTokens(node.FamilyReferences
                .Distinct(StringComparer.Ordinal)
                .Select(number => new FamilyReferenceEntryTemplateModel
                {
                    Number = number,
                    Anchor = GetFamilyAnchor(number)
                })
                .ToArray());
            AppendFamilyTokens(para, "[", familyTokens);

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
            var existingParagraphs = doc.Enumerate().OfType<IDocParagraph>().ToHashSet();
            templateRenderer.RenderFamily(doc, entryTemplate, entryModel);
            EnsureFamilyAnchor(doc, entryModel.Anchor, existingParagraphs);
        }

        templateRenderer.EnsureNavigation(doc, Array.Empty<PersonEntryTemplateModel>());
    }

    private static void EnsureFamilyAnchor(
        IUserDocument document,
        string anchor,
        IReadOnlySet<IDocParagraph> existingParagraphs)
    {
        var spans = document.Enumerate().OfType<IDocSpan>().ToArray();
        if (spans.Any(span => string.Equals(span.Id, anchor, StringComparison.Ordinal)))
            return;

        var firstEntryParagraph = document.Enumerate()
            .OfType<IDocParagraph>()
            .FirstOrDefault(paragraph => !existingParagraphs.Contains(paragraph))
            ?? throw new InvalidDataException($"The family template rendered no paragraph for anchor '{anchor}'.");
        firstEntryParagraph.AddBookmark(anchor, DocxFontStyle.Default).TextContent = string.Empty;
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

    private string GetFamilyMarriageMark(OFBFamily family)
    {
        var partnerSymbol = GetLegendSymbol(EFactType.Partner);
        if (string.IsNullOrWhiteSpace(partnerSymbol))
            partnerSymbol = "o-o";
        var marriageSymbol = GetLegendSymbol(EFactType.Mariage);
        if (string.IsNullOrWhiteSpace(marriageSymbol))
            marriageSymbol = "oo";

        var eventText = family.Marriage?.Data;
        if (string.IsNullOrWhiteSpace(eventText))
            return family.Marriage is null && family.MarriageDate is null ? partnerSymbol : marriageSymbol;

        if (eventText.Contains("unverheirat", StringComparison.OrdinalIgnoreCase)
            || eventText.Contains("unehelich", StringComparison.OrdinalIgnoreCase))
            return partnerSymbol;
        return marriageSymbol;
    }

    private static string FormatPersonName(IGenPerson person, bool gcFormat, string? familySurnameToAbbreviate = null)
    {
        var givenName = person.GivenName?.Trim();
        var surname = person.Surname?.Trim();
        var title = person.Title;
        if (string.IsNullOrWhiteSpace(title))
            title = person.Facts.FirstOrDefault(fact => fact?.eFactType == EFactType.Title)?.Data;
        title = title?.Trim();
        string formattedName;
        if (!string.IsNullOrWhiteSpace(givenName) && !string.IsNullOrWhiteSpace(surname))
        {
            formattedName = gcFormat && !string.IsNullOrWhiteSpace(familySurnameToAbbreviate)
                && string.Equals(surname, familySurnameToAbbreviate.Trim(), StringComparison.OrdinalIgnoreCase)
                    ? givenName
                    : gcFormat
                        ? $"{surname}, {givenName}"
                        : $"{givenName} {surname}";
        }
        else if (!string.IsNullOrWhiteSpace(givenName))
            formattedName = givenName;
        else if (!string.IsNullOrWhiteSpace(surname))
            formattedName = surname;
        else
        {
            var name = person.Name?.Trim() ?? string.Empty;
            var normalized = name.Replace("/", " ", StringComparison.Ordinal);
            formattedName = string.Join(' ', normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        return !string.IsNullOrWhiteSpace(title)
            ? $"{formattedName}, {title}"
            : formattedName;
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

    private void AppendFamilyNumber(IDocParagraph paragraph, string familyNumber)
    {
        var targetAnchor = ResolveFamilyAnchor(familyNumber);
        var token = $"[{familyNumber}]";
        if (targetAnchor is null)
            paragraph.AppendText(token);
        else
            paragraph.AddLink($"#{targetAnchor}", DocxFontStyle.UnderlineStyle).TextContent = token;
    }

    private static string NormalizeFamilyNumber(string reference)
    {
        var value = reference.StartsWith("Fam.", StringComparison.Ordinal) ? reference[4..] : reference;
        return value.EndsWith('K') ? value[..^1] : value;
    }

    private void AppendIndexFamilyReferences(IDocParagraph paragraph, OFBIndexEntry entry, string? fallbackReference = null)
    {
        var references = entry.FamilyReferences.Count > 0
            ? entry.FamilyReferences
            : string.IsNullOrWhiteSpace(fallbackReference) ? Array.Empty<string>() : [fallbackReference];
        var grouped = references
            .Select(NormalizeFamilyNumber)
            .Distinct(StringComparer.Ordinal)
            .Select(number => CreateNumberedFamilyReference(number))
            .ToArray();
        var tokens = CreateFamilyReferenceTokens(grouped);
        AppendFamilyTokens(paragraph, "[", tokens);
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
            .Select((person, index) => CreatePersonTemplateModel(
                person, index + 1, allFamilies, family.GlobalNumber, family.FamilyName))
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

    private static IReadOnlyList<FamilyReferenceTokenModel> CreateIndexFamilyReferenceTokens(OFBIndexEntry entry)
    {
        var tokens = new List<FamilyReferenceTokenModel>();
        var childTokens = CreateFamilyReferenceTokens(entry.ChildFamilyReferences
            .Distinct(StringComparer.Ordinal)
            .Select(CreateNumberedFamilyReference)
            .ToArray());
        if (childTokens.Count > 0)
        {
            tokens.Add(new FamilyReferenceTokenModel { Text = "<" });
            tokens.AddRange(childTokens);
            tokens.Add(new FamilyReferenceTokenModel { Text = ">" });
        }

        var parentTokens = CreateFamilyReferenceTokens(entry.ParentFamilyReferences
            .Distinct(StringComparer.Ordinal)
            .Select(CreateNumberedFamilyReference)
            .ToArray());
        if (parentTokens.Count > 0)
        {
            if (tokens.Count > 0)
                tokens.Add(new FamilyReferenceTokenModel { Text = " " });
            tokens.Add(new FamilyReferenceTokenModel { Text = "[" });
            tokens.AddRange(parentTokens);
            tokens.Add(new FamilyReferenceTokenModel { Text = "]" });
        }
        return tokens;
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

    private PersonEntryTemplateModel CreatePersonTemplateModel(
        IGenPerson person,
        int? ordinal,
        IReadOnlyList<OFBFamily> allFamilies,
        string currentFamilyNumber,
        string? familySurnameToAbbreviate = null)
    {
        var reference = person.IndRefID ?? person.Name ?? string.Empty;
        _personIndexAnchorByReference.TryGetValue(reference, out var personIndexAnchor);
        var birth = (person.BirthDate ?? person.Birth?.Date)?.ToString();
        var death = (person.DeathDate ?? person.Death?.Date)?.ToString();
        var vitalEventValues = new (EFactType Event, IGenDate? Date, IGenFact? Fact)[]
        {
            (EFactType.Birth, person.BirthDate ?? person.Birth?.Date, person.Birth),
            (EFactType.Baptism, person.BaptDate ?? person.Baptism?.Date, person.Baptism),
            (EFactType.Death, person.DeathDate ?? person.Death?.Date, person.Death),
            (EFactType.Burial, person.BurialDate ?? person.Burial?.Date, person.Burial)
        };
        var vitalEvents = vitalEventValues
            .Where(item => item.Date is not null)
            .OrderBy(item => GetEventSortDate(item.Date))
            .Select(item => $"{GetLegendSymbol(item.Event)} {item.Date}")
            .ToArray();
        var vitalEventsGc = string.Join(", ", vitalEvents);

        var vitalEventsAk = string.Join(", ", vitalEvents);
        var occupations = GetOccupations(person).ToArray();
        var preferredOccupation = GetPreferredOccupation(occupations);
        var events = person.Facts
            .Where(fact => fact is not null
                && (!IsVitalEvent(fact.eFactType) || fact.Date is not null)
                && fact.eFactType is not (EFactType.Mariage or EFactType.Reference)
                && (!IsVitalEvent(fact.eFactType)
                    || ReferenceEquals(fact, vitalEventValues.First(item => item.Event == fact.eFactType).Fact)))
            .Select(fact => (
                Event: CreatePersonEventTemplateModel(
                    fact!,
                    allFamilies,
                    IsVitalEvent(fact!.eFactType)
                        ? vitalEventValues.First(item => item.Event == fact.eFactType).Date?.ToString()
                        : null),
                Date: IsVitalEvent(fact!.eFactType)
                    ? vitalEventValues.First(item => item.Event == fact.eFactType).Date
                    : fact.Date))
            .Concat(vitalEventValues
                .Where(item => item.Date is not null
                    && !person.Facts.Any(fact => fact is not null
                        && ReferenceEquals(fact, item.Fact)
                        && fact.Date is not null))
                .Select(item => (
                    Event: item.Fact is null
                        ? CreatePersonEventTemplateModel(item.Event, item.Date!.ToString())
                        : CreatePersonEventTemplateModel(item.Fact, allFamilies, item.Date?.ToString()),
                    Date: item.Date)))
            .OrderBy(item => GetEventSortDate(item.Date))
            .Select(item => item.Event)
            .ToArray();

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
            NameGc = FormatPersonName(person, gcFormat: true, familySurnameToAbbreviate),
            NameAk = FormatPersonName(person, gcFormat: false),
            AkaNames = FormatAlternativeNames(person),
            Religion = GetPersonReligion(person),
            Anchor = GetPersonAnchor(reference),
            Reference = reference,
            ReferenceNumber = GetReferenceNumber(person),
            Events = events,
            IndexLabel = FormatPersonName(person, gcFormat: true),
            VitalEventsGc = vitalEventsGc.Length == 0 ? string.Empty : ", " + vitalEventsGc,
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
            ShowNonVitalEvents = ordinal is null
                ? ShouldShowParentNonVitalEvents(person, currentFamilyNumber, allFamilies)
                : parentFamilies.Length == 0,
            Occupations = preferredOccupation is null
                ? Array.Empty<OccupationEntryTemplateModel>()
                : new[]
                {
                    new OccupationEntryTemplateModel
                    {
                        Name = preferredOccupation.Value.Name,
                        Date = preferredOccupation.Value.Date?.ToString(),
                        Place = preferredOccupation.Value.Place?.Name,
                        PlaceAnchor = GetPlaceIndexAnchor(preferredOccupation.Value.Place?.Name),
                        IndexAnchor = _occupationIndexAnchorByName.TryGetValue(preferredOccupation.Value.Name, out var anchor)
                            ? anchor
                            : string.Empty
                    }
                },
            Properties = CreatePropertyTemplateModels(person.Facts)
        };
    }

    private static string FormatAlternativeNames(IGenPerson person)
    {
        var primaryName = NormalizeGedcomName(FormatPersonName(person, gcFormat: false));
        var birthSurname = CanonicalGenealogyAdapter.GetNameVariants(person).BirthSurname;
        var birthName = NormalizeGedcomName(string.Join(' ', new[] { person.GivenName, birthSurname }
            .Where(part => !string.IsNullOrWhiteSpace(part))));
        var aliases = CanonicalGenealogyAdapter.GetNameVariants(person).AliasNames
            .Concat(CanonicalGenealogyAdapter.GetNameEvents(person)
                .Where(nameEvent => string.Equals(nameEvent.Type, "AKA", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace( nameEvent.Surname )
                    && !string.Equals(nameEvent.Surname, person.Surname, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(nameEvent.Surname, birthSurname, StringComparison.OrdinalIgnoreCase))
                .Select(nameEvent => string.Join(' ', new[] { nameEvent.GivenName, nameEvent.Surname }
                    .Where(part => !string.IsNullOrWhiteSpace(part)))))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(NormalizeGedcomName)
            .Where(name => !string.Equals(name, primaryName, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return string.Join("; ", aliases);
    }

    private static string NormalizeGedcomName(string name) =>
        string.Join(' ', name.Replace("/", " ", StringComparison.Ordinal)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));

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

    private static (string Name, IGenDate? Date, IGenPlace? Place)? GetPreferredOccupation(
        IReadOnlyList<(string Name, IGenDate? Date, IGenPlace? Place)> occupations)
    {
        foreach (var occupation in occupations)
        {
            if (occupation.Date is null)
                return occupation;
        }

        return occupations.Count == 0 ? null : occupations[^1];
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

    private static string GetReferenceNumber(IGenPerson person) => person.Facts
        .FirstOrDefault(fact => fact?.eFactType == EFactType.Reference)?.Data ?? string.Empty;

    private PersonEventEntryTemplateModel CreatePersonEventTemplateModel(
        IGenFact fact,
        IReadOnlyList<OFBFamily> allFamilies,
        string? dateOverride = null)
    {
        var relatedPerson = fact.Entities
            .Select(connection => connection?.Entity)
            .OfType<IGenPerson>()
            .FirstOrDefault();
        var relatedFamily = relatedPerson is null
            ? null
            : allFamilies.FirstOrDefault(family => IsSamePerson(family.Husband, relatedPerson)
                || IsSamePerson(family.Wife, relatedPerson)
                || family.Children.Any(child => IsSamePerson(child, relatedPerson)));
        var eventType = fact.eFactType;
        var date = dateOverride ?? fact.Date?.ToString() ?? string.Empty;
        var symbol = GetLegendSymbol(eventType);
        var additional = string.IsNullOrWhiteSpace(fact.Data) ? string.Empty : fact.Data.Trim();

        return new PersonEventEntryTemplateModel
        {
            Symbol = symbol,
            Date = date,
            EventName = GetLegendEntry(eventType)?.Meaning ?? GetEventFallbackName(eventType),
            PlacePreposition = GetLegendPlacePreposition(eventType),
            Place = GetShortPlaceName(fact.Place?.Name),
            PlaceAnchor = GetPlaceIndexAnchor(fact.Place?.Name),
            Additional = additional,
            RelatedPersonName = relatedPerson is null ? null : FormatPersonName(relatedPerson, gcFormat: false),
            RelatedPersonAnchor = relatedPerson is null
                ? string.Empty
                : GetPersonAnchor(relatedPerson.IndRefID ?? relatedPerson.Name),
            RelatedFamilyNumber = relatedFamily?.GlobalNumber,
            RelatedFamilyAnchor = relatedFamily is null ? string.Empty : GetFamilyAnchor(relatedFamily.GlobalNumber),
            OccupationIndexAnchor = eventType == EFactType.Occupation
                && _occupationIndexAnchorByName.TryGetValue(additional, out var occupationAnchor)
                    ? occupationAnchor
                    : string.Empty,
            IsVital = IsVitalEvent(eventType),
            IsListableNonVital = IsListableNonVitalEvent(eventType, date),
            IsOccupation = eventType == EFactType.Occupation
        };
    }

    private PersonEventEntryTemplateModel CreatePersonEventTemplateModel(
        EFactType eventType,
        string? date) => new()
        {
            Symbol = GetLegendSymbol(eventType),
            Date = date ?? string.Empty,
            EventName = GetLegendEntry(eventType)?.Meaning ?? GetEventFallbackName(eventType),
            PlacePreposition = GetLegendPlacePreposition(eventType),
            IsVital = IsVitalEvent(eventType),
            IsListableNonVital = IsListableNonVitalEvent(eventType, date)
        };

    private static bool IsListableNonVitalEvent(EFactType eventType, string? date) =>
        !string.IsNullOrWhiteSpace(date)
        && !IsVitalEvent(eventType)
        && eventType is not (EFactType.Mariage or EFactType.Reference
            or EFactType.Property or EFactType.Residence
            or EFactType.Title or EFactType.Religion or EFactType.Sex
            or EFactType.Info or EFactType.Description);

    private bool ShouldShowParentNonVitalEvents(
        IGenPerson person,
        string currentFamilyNumber,
        IReadOnlyList<OFBFamily> allFamilies)
    {
        var parentFamilies = allFamilies
            .Where(family => IsSamePerson(family.Husband, person) || IsSamePerson(family.Wife, person))
            .ToArray();
        var selectedFamily = parentFamilies.FirstOrDefault(IsMarriageFamily)
            ?? parentFamilies.FirstOrDefault();
        return selectedFamily is not null
            && string.Equals(selectedFamily.GlobalNumber, currentFamilyNumber, StringComparison.Ordinal);
    }

    private static bool IsMarriageFamily(OFBFamily family)
    {
        var marriageText = family.Marriage?.Data;
        if (!string.IsNullOrWhiteSpace(marriageText)
            && (marriageText.Contains("unverheirat", StringComparison.OrdinalIgnoreCase)
                || marriageText.Contains("unehelich", StringComparison.OrdinalIgnoreCase)))
            return false;

        return family.Marriage is not null || family.MarriageDate is not null;
    }

    private static string GetEventFallbackName(EFactType eventType) => eventType switch
    {
        EFactType.Birth => "Geburt",
        EFactType.Baptism => "Taufe",
        EFactType.Death => "Tod",
        EFactType.Burial => "Beerdigung",
        EFactType.Occupation => "Beruf",
        EFactType.Residence => "Wohnsitz",
        EFactType.Education => "Ausbildung",
        EFactType.Property => "Besitz",
        EFactType.Religion => "Religion",
        EFactType.Military => "Militär",
        EFactType.Emigration => "Auswanderung",
        EFactType.Immigration => "Einwanderung",
        EFactType.Naturalization => "Einbürgerung",
        EFactType.Divorce => "Scheidung",
        EFactType.Adoption => "Adoption",
        EFactType.Census => "Volkszählung",
        EFactType.Title => "Titel",
        _ => eventType.ToString()
    };

    private static bool IsVitalEvent(EFactType eventType) =>
        eventType is EFactType.Birth or EFactType.Baptism or EFactType.Death or EFactType.Burial;

    private static string GetPersonReligion(IGenPerson person)
    {
        var religion = person.Religion;
        if (string.IsNullOrWhiteSpace(religion))
            religion = person.Facts.FirstOrDefault(fact => fact?.eFactType == EFactType.Religion)?.Data;
        return religion?.Trim() ?? string.Empty;
    }

    private static DateTime GetEventSortDate(IGenDate? date)
    {
        if (date is null)
            return DateTime.MaxValue;
        if (date.Date1 != default)
            return date.Date1;
        if (int.TryParse(date.DateText, NumberStyles.None, CultureInfo.InvariantCulture, out var year)
            && year is >= 1 and <= 9999)
            return new DateTime(year, 1, 1);
        return DateTime.TryParse(date.DateText, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var parsed)
            ? parsed
            : DateTime.MaxValue;
    }

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
        _personReferenceByIndexName.Clear();
        _personIndexAnchorByName.Clear();

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

        var germanComparer = StringComparer.Create(CultureInfo.GetCultureInfo("de-DE"), ignoreCase: true);
        entries = entries.OrderBy(entry => entry.Name, germanComparer)
            .ThenBy(entry => entry.SortKey, germanComparer)
            .ToList();
        for (var index = 0; index < entries.Count; index++)
        {
            var entry = entries[index];
            if (!referencesByPerson.TryGetValue(entry.Ref, out var references))
                continue;
            entries[index] = new OFBIndexEntry
            {
                SortKey = entry.SortKey,
                Name = entry.Name,
                Ref = entry.Ref,
                LifeSpan = entry.LifeSpan,
                NameEventDate = entry.NameEventDate,
                ChildFamilyReferences = references.Child.Select(reference => reference.Number).Distinct(StringComparer.Ordinal).ToArray(),
                ParentFamilyReferences = references.Parent.Select(reference => reference.Number).Distinct(StringComparer.Ordinal).ToArray()
            };
            _personIndexAnchorByName[entry.SortKey] = CreateIndexAnchor("person", index);
        }
        _personIndexAnchorByReference.Clear();
        for (var index = 0; index < entries.Count; index++)
            _personIndexAnchorByReference.TryAdd(entries[index].Ref, CreateIndexAnchor("person", index));
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
            var givenName = person.GivenName?.Trim() ?? string.Empty;
            var lifeSpan = FormatLifeSpan(person.BirthDate ?? person.Birth?.Date, person.DeathDate ?? person.Death?.Date);
            var birthSurname = CanonicalGenealogyAdapter.GetNameVariants(person).BirthSurname;
            var nameEvents = CanonicalGenealogyAdapter.GetNameEvents(person);
            var surnameEvents = nameEvents
                .Where(nameEvent => !string.IsNullOrWhiteSpace(nameEvent.Surname))
                .Where(nameEvent => string.Equals(nameEvent.Type, "BIRTH", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(nameEvent.Type, "AKA", StringComparison.OrdinalIgnoreCase))
                .GroupBy(nameEvent => nameEvent.Surname.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(group => group.OrderBy(nameEvent => nameEvent.Date ?? DateTime.MaxValue).First())
                .OrderBy(nameEvent => nameEvent.Date ?? DateTime.MaxValue)
                .ToArray();
            var indexedNames = surnameEvents
                .Select(nameEvent => (
                    Surname: nameEvent.Surname.Trim(),
                    GivenName: string.IsNullOrWhiteSpace(nameEvent.GivenName) ? givenName : nameEvent.GivenName.Trim(),
                    Date: nameEvent.DateText ?? string.Empty))
                .ToList();
            if (!string.IsNullOrWhiteSpace(person.Surname)
                && !indexedNames.Any(name => string.Equals(name.Surname, person.Surname.Trim(), StringComparison.OrdinalIgnoreCase)))
                indexedNames.Add((person.Surname.Trim(), givenName, string.Empty));
            if (!string.IsNullOrWhiteSpace(birthSurname)
                && !indexedNames.Any(name => string.Equals(name.Surname, birthSurname.Trim(), StringComparison.OrdinalIgnoreCase)))
                indexedNames.Add((birthSurname.Trim(), givenName, string.Empty));
            if (indexedNames.Count == 0)
                indexedNames.Add((string.Empty, givenName, string.Empty));
            foreach (var indexedName in indexedNames)
            {
                var sortKey = string.IsNullOrEmpty(indexedName.Surname)
                    ? indexedName.GivenName
                    : string.IsNullOrEmpty(indexedName.GivenName) ? indexedName.Surname : $"{indexedName.Surname}, {indexedName.GivenName}";
                entries.Add(new OFBIndexEntry
                {
                    SortKey = sortKey,
                    Name = indexedName.Surname,
                    Ref = personReference,
                    LifeSpan = lifeSpan,
                    NameEventDate = indexedName.Date
                });
                _personReferenceByIndexName.TryAdd(sortKey, personReference);
            }
        }
    }

    private static string FormatLifeSpan(IGenDate? birthDate, IGenDate? deathDate)
    {
        static string? GetYear(IGenDate? date) => date?.Date1 is DateTime value && value != default
            ? value.Year.ToString(CultureInfo.InvariantCulture)
            : null;
        var birthYear = GetYear(birthDate);
        var deathYear = GetYear(deathDate);
        return birthYear is null && deathYear is null ? string.Empty : $"{birthYear ?? "?"}–{deathYear ?? "?"}";
    }

    private EntryTemplateLegendEntry? GetLegendEntry(EFactType eventType) =>
        _legendEntries.FirstOrDefault(entry => entry.Event == eventType);

    private string GetLegendSymbol(EFactType eventType) =>
        GetLegendEntry(eventType)?.Symbol ?? string.Empty;

    private string GetLegendPlacePreposition(EFactType eventType)
    {
        var preposition = GetLegendEntry(eventType)?.PlacePreposition;
        return string.IsNullOrWhiteSpace(preposition) ? "in" : preposition.Trim();
    }

    /// <summary>
    /// Generates alphabetical occupation index with person/family references.
    /// </summary>
    private async Task<OFBIndexEntry[]> GenerateOccupationIndexAsync( IReadOnlyList<OFBFamily> sortedFamilies )
    {
        var referencesByOccupation = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach ( var family in sortedFamilies )
        {
            foreach ( var person in new[] { family.Husband, family.Wife }.Where( p => p != null ) )
                foreach (var occupation in GetOccupations(person!))
                    AddReference(occupation.Name, family.GlobalNumber);

            foreach ( var child in family.Children )
                foreach (var occupation in GetOccupations(child))
                    AddReference(occupation.Name, family.GlobalNumber);
        }

        return referencesByOccupation
            .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(pair => new OFBIndexEntry
            {
                SortKey = pair.Key,
                Name = pair.Key,
                Ref = pair.Value.First(),
                FamilyReferences = pair.Value.OrderBy(number => number, StringComparer.Ordinal).ToArray()
            })
            .ToArray();

        void AddReference(string occupation, string familyNumber)
        {
            if (!referencesByOccupation.TryGetValue(occupation, out var references))
                referencesByOccupation[occupation] = references = new HashSet<string>(StringComparer.Ordinal);
            references.Add(familyNumber);
        }
    }

    /// <summary>
    /// Generates property/ownership index from family data.
    /// Implements decision 1A: Property Index included.
    /// </summary>
    private async Task<OFBIndexEntry[]> GeneratePropertyIndexAsync( IReadOnlyList<OFBFamily> sortedFamilies )
    {
        var referencesByProperty = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach ( var family in sortedFamilies )
        {
            var properties = ExtractPropertiesFromFamily( family );
            foreach ( var prop in properties )
            {
                if (!referencesByProperty.TryGetValue(prop, out var references))
                    referencesByProperty[prop] = references = new HashSet<string>(StringComparer.Ordinal);
                references.Add(family.GlobalNumber);
            }
        }

        return referencesByProperty
            .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(pair => new OFBIndexEntry
            {
                SortKey = pair.Key,
                Name = pair.Key,
                Ref = pair.Value.First(),
                FamilyReferences = pair.Value.OrderBy(number => number, StringComparer.Ordinal).ToArray()
            })
            .ToArray();
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
        var rootNodes = new Dictionary<string, OFBPlaceHierarchyNode>(StringComparer.OrdinalIgnoreCase);

        foreach ( var family in families )
        {
            AddPlace(family.MarriagePlace, family.GlobalNumber);
            foreach (var fact in family.Facts.OfType<IGenFact>())
                AddPlace(fact.Place, family.GlobalNumber);

            foreach (var person in new[] { family.Husband, family.Wife }.Where(person => person is not null).Cast<IGenPerson>().Concat(family.Children))
            {
                AddPlace(person.BirthPlace, family.GlobalNumber);
                AddPlace(person.DeathPlace, family.GlobalNumber);
                AddPlace(person.BurialPlace, family.GlobalNumber);
                AddPlace(person.Residence, family.GlobalNumber);
                AddPlace(person.OccuPlace, family.GlobalNumber);
                foreach (var fact in person.Facts.OfType<IGenFact>())
                    AddPlace(fact.Place, family.GlobalNumber);
            }
        }

        return SortPlaceHierarchyNodes(rootNodes.Values);

        void AddPlace(IGenPlace? place, string familyNumber)
        {
            if (string.IsNullOrWhiteSpace(place?.Name))
                return;

            var components = place.Name.Split(new[] { ',' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            Array.Reverse(components);
            components = components.Take(5).ToArray();
            if (components.Length == 0)
                return;

            OFBPlaceHierarchyNode? currentNode = null;
            var parentKey = string.Empty;
            foreach (var component in components)
            {
                var nodeKey = $"{parentKey}/{component}";
                var siblings = currentNode is null
                    ? rootNodes
                    : currentNode.Children.ToDictionary(node => node.Name, StringComparer.OrdinalIgnoreCase);
                if (!siblings.TryGetValue(component, out var nextNode))
                {
                    nextNode = new OFBPlaceHierarchyNode(component, nodeKey);
                    if (currentNode is null)
                        rootNodes.Add(component, nextNode);
                    else
                        currentNode.Children = currentNode.Children.Append(nextNode).ToArray();
                }
                currentNode = nextNode;

                if (!currentNode.FamilyReferences.Contains(familyNumber, StringComparer.Ordinal))
                    currentNode.FamilyReferences = currentNode.FamilyReferences.Concat([familyNumber]).ToArray();

                parentKey = nodeKey;
            }
        }
    }

    private static OFBPlaceHierarchyNode[] SortPlaceHierarchyNodes(IEnumerable<OFBPlaceHierarchyNode> nodes)
    {
        var comparer = StringComparer.Create(CultureInfo.GetCultureInfo("de-DE"), true);
        var sortedNodes = nodes.OrderBy(node => node.Name, comparer).ToArray();
        foreach (var node in sortedNodes)
            node.Children = SortPlaceHierarchyNodes(node.Children);
        return sortedNodes;
    }

    /// <summary>
    /// Generates flat alphabetical place index (A-Z) for cross-reference.
    /// Implements decision 2A: Place Alphabetical Index included.
    /// </summary>
    private async Task<OFBIndexEntry[]> GeneratePlaceAlphabeticalIndexAsync( IReadOnlyList<OFBFamily> families )
    {
        var referencesByPlace = new Dictionary<string, (string DisplayName, HashSet<string> References)>(StringComparer.OrdinalIgnoreCase);

        foreach ( var family in families )
        {
            AddPlace(family.MarriagePlace, family.GlobalNumber);
            foreach (var fact in family.Facts.OfType<IGenFact>())
                AddPlace(fact.Place, family.GlobalNumber);

            foreach (var person in new[] { family.Husband, family.Wife }.Where(person => person is not null).Cast<IGenPerson>().Concat(family.Children))
            {
                var reference = family.GlobalNumber;
                AddPlace(person.BirthPlace, reference);
                AddPlace(person.DeathPlace, reference);
                AddPlace(person.BurialPlace, reference);
                AddPlace(person.Residence, reference);
                AddPlace(person.OccuPlace, reference);
                foreach (var fact in person.Facts.OfType<IGenFact>())
                    AddPlace(fact.Place, reference);
            }
        }

        return referencesByPlace
            .OrderBy(pair => pair.Key, StringComparer.Create(CultureInfo.GetCultureInfo("de-DE"), true))
            .Select(pair => new OFBIndexEntry
            {
                SortKey = pair.Value.DisplayName,
                Name = pair.Value.DisplayName,
                Ref = pair.Value.References.First(),
                FamilyReferences = pair.Value.References.OrderBy(number => number, StringComparer.Ordinal).ToArray()
            })
            .ToArray();

        void AddPlace(IGenPlace? place, string reference)
        {
            if (string.IsNullOrWhiteSpace(place?.Name))
                return;
            if (!referencesByPlace.TryGetValue(place.Name.Trim(), out var placeData))
                referencesByPlace[place.Name.Trim()] = placeData = (place.Name.Trim(), new HashSet<string>(StringComparer.Ordinal));
            placeData.References.Add(reference);
        }
    }
}
